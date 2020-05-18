
using System;
using System.Drawing;
using System.Threading;

using NthDimension.Rasterizer;
//using static NthDimension.FFMpeg.FFmpeg4;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;

namespace NthDimension.FFMpeg
{
    public delegate void VideoPlaybackStart();
    public delegate void VideoPlaybackStop();
    public delegate void VideoPlaybackPause();
    public unsafe class VideoSource : IDisposable
    {
        private const int MaxAudioSamples = 192000;
        private const int MaxQueuedPackets = 10;

        #region Fields

        private RendererBaseGL3 _renderer;
        private AudioBase _audio;

        private int _videoStreamId = -1;
        private Size _videoSize = Size.Empty;
        private AVCodecContext* _videoContext;
        private AVCodec* _videoCodec;
        private AVFrame* _videoRawFrame;
        private AVFrame* _videoRgbaFrame;
        private byte* _videoRawBuffer;
        private byte* _videoRgbaBuffer;
        private SwsContext* _videoSwContext;

        private int _audioStreamId = -1;
        private AVCodecContext* _audioContext;
        private AVCodec* _audioCodec;
        private AVFrame* _audioRawBuffer;
        private byte* _audioPcmBuffer;
        private SwrContext* _audioSwContext;

        private AVFormatContext* _formatContext;
        private Thread _decodeThread;
        private bool _runDecodeThread;
        private bool _playingToEof;
        private TimeSpan _playingOffset = TimeSpan.Zero;
        private DateTime _lastUpdate;

        #endregion

        #region Properties
        public bool Loop { get; set; } = false;
        public bool HasVideo => _videoStreamId != -1;
        public bool HasAudio => _audioStreamId != -1;
        public VideoPlayback VideoPlayback { get; private set; }
        public AudioPlayback AudioPlayback { get; private set; }

        public Size VideoSize => new Size(_videoSize.Width, _videoSize.Height);
        public PlayState State { get; private set; } = PlayState.Stopped;
        public int AudioChannelCount { get; private set; } = -1;
        public int AudioSampleRate => HasAudio ? _audioContext->sample_rate : -1;
        public TimeSpan FileLength { get; private set; } = TimeSpan.Zero;
        public bool IsEndOfFileReached { get; private set; }

        public bool IsFull => (!HasVideo || VideoPlayback.QueuedPackets >= MaxQueuedPackets)
                           && (!HasAudio || AudioPlayback.QueuedPackets >= MaxQueuedPackets);

        public TimeSpan PlayingOffset
        {
            get => _playingOffset;
            set
            {
                if (!HasVideo && !HasAudio) return;
                bool startDecode = _runDecodeThread;
                bool startPlaying = State == PlayState.Playing;
                StopDecodeThread();
                _playingOffset = value;
                _playingToEof = false;

                if (State != PlayState.Stopped)
                {
                    IsEndOfFileReached = true;
                    NotifyStateChanged(PlayState.Stopped);
                    State = PlayState.Stopped;
                    IsEndOfFileReached = false;
                }

                if (HasVideo)
                {
                    var timebase = _formatContext->streams[_videoStreamId]->time_base;
                    float ftb = (float)timebase.den / timebase.num;
                    long pos = (long)(PlayingOffset.TotalSeconds * ftb);
                    av_seek_frame(_formatContext, _videoStreamId, pos, AVSEEK_FLAG_ANY);
                    avcodec_flush_buffers(_videoContext);
                    VideoPlayback.Flush();
                }

                if (HasAudio)
                {
                    var timebase = _formatContext->streams[_audioStreamId]->time_base;
                    float ftb = (float)timebase.den / timebase.num;
                    long pos = (long)(PlayingOffset.TotalSeconds * ftb);
                    av_seek_frame(_formatContext, _audioStreamId, pos, AVSEEK_FLAG_ANY);
                    avcodec_flush_buffers(_audioContext);
                    AudioPlayback.Flush();
                }

                if (startDecode) StartDecodeThread();
                if (startPlaying) Play();
            }
        }

        public TimeSpan FrameDuration
        {
            get
            {
                if (!HasVideo) return TimeSpan.Zero;

                var a = _formatContext->streams[_videoStreamId]->avg_frame_rate;
                if (a.num != 0 || a.den != 0) return TimeSpan.FromTicks(TimeSpan.TicksPerSecond * a.den / a.num);

                var r = _formatContext->streams[_videoStreamId]->r_frame_rate;
                if (r.num != 0 || r.den != 0) return TimeSpan.FromTicks(TimeSpan.TicksPerSecond * r.den / r.num);

                double tickCount = _formatContext->streams[_videoStreamId]->duration;
                double frameCount = _formatContext->streams[_videoStreamId]->nb_frames;
                double ticksPerFrame = tickCount / frameCount;
                double tickDuration = (double)_formatContext->streams[_videoStreamId]->time_base.num / _formatContext->streams[_videoStreamId]->time_base.den;

                double frameDuration = ticksPerFrame * tickDuration;
                return TimeSpan.FromTicks((long)(frameDuration * TimeSpan.TicksPerSecond));
            }
        }


        #endregion

        public event VideoPlaybackStart OnVideoStarted;
        public event VideoPlaybackPause OnVideoPaused;
        public event VideoPlaybackStop OnVideoStopped;

        public string Path { get; private set; }
        private bool m_videoEnabled;
        private bool m_audioEnabled;

        private bool m_sourceInitializedFlag = false;
        private bool m_decodeThreadRunning = false;
        #region Setup

        private bool consoleDebug = false;      

        public VideoSource(/*RendererBase renderer, AudioBase audio,*/ string path, bool videoEnabled = true, bool audioEnabled = true, bool displayDebug = false)
        {
            consoleDebug = displayDebug;
            _renderer   = VideoGL3x.Renderer;
            _audio      = VideoGL3x.Audio;

            Path        = path;

            m_videoEnabled = videoEnabled;
            m_audioEnabled = audioEnabled;

            AVFormatContext* formatContext;
            if (avformat_open_input(&formatContext, Path, null, null) != 0)
            {
                if(consoleDebug)
                    Console.WriteLine("NthDimension.FFMpeg: Failed to open file: " + Path);
                return;
            }
            _formatContext = formatContext;
            if (avformat_find_stream_info(formatContext, null) != 0)
            {
                if (consoleDebug)
                    Console.WriteLine("NthDimension.FFMpeg: Failed to find stream information: " + Path);
                return;
            }

            for (int i = 0; i < formatContext->nb_streams; i++)
            {
                switch (formatContext->streams[i]->codec->codec_type)
                {
                    case AVMediaType.AVMEDIA_TYPE_VIDEO:
                        if (_videoStreamId == -1 && m_videoEnabled) _videoStreamId = i;
                        break;
                    case AVMediaType.AVMEDIA_TYPE_AUDIO:
                        if (_audioStreamId == -1 && m_audioEnabled) _audioStreamId = i;
                        break;
                }
            }

        }

        private void InitVideo()
        {
            if (!HasVideo) return;
            
            _videoContext = _formatContext->streams[_videoStreamId]->codec;
            if (_videoContext == null)
            {
                if (consoleDebug)
                    Console.WriteLine("NthDimension ffmpeg: Failed to get video codec context");
                _videoStreamId = -1;
                return;
            }

            _videoCodec = avcodec_find_decoder(_videoContext->codec_id);
            if (_videoCodec == null)
            {
                if (consoleDebug)
                    Console.WriteLine("NthDimension ffmpeg: Failed to find video codec");
                _videoStreamId = -1;
                return;
            }

            VideoPlayback = new VideoPlayback(_renderer, this);
        }
        private void InitAudio()
        {
            if (!HasAudio) return;

            _audioContext = _formatContext->streams[_audioStreamId]->codec;
            if (_audioContext == null)
            {
                if (consoleDebug)
                    Console.WriteLine("\tNthDimension.FFMpeg: Failed to get audio codec context");
                _audioStreamId = -1;
                return;
            }

            _audioCodec = avcodec_find_decoder(_audioContext->codec_id);
            if (_audioCodec == null)
            {
                if (consoleDebug)
                    Console.WriteLine("\tNthDimension.FFMpeg: Failed to find audio codec");
                _audioStreamId = -1;
                return;
            }

            if (avcodec_open2(_audioContext, _audioCodec, null) != 0)
            {
                if (consoleDebug)
                    Console.WriteLine("\tNthDimension.FFMpeg: Failed to load audio codec");
                _audioStreamId = -1;
                return;
            }

            _audioRawBuffer = av_frame_alloc();
            if (_audioRawBuffer == null)
            {
                if (consoleDebug)
                    Console.WriteLine("\tNthDimension.FFMpeg: Failed to allocate audio buffer");
                _audioStreamId = -1;
                return;
            }

            var audioPcmBuffer = _audioPcmBuffer;
            if (av_samples_alloc(&audioPcmBuffer, null, _audioContext->channels, av_samples_get_buffer_size(null, _audioContext->channels, MaxAudioSamples, AVSampleFormat.AV_SAMPLE_FMT_S16, 0), AVSampleFormat.AV_SAMPLE_FMT_S16, 0) < 0)
            {
                if (consoleDebug)
                    Console.WriteLine("\tNthDimension.FFMpeg: Failed to create audio samples buffer");
                _audioStreamId = -1;
                return;
            }
            _audioPcmBuffer = audioPcmBuffer;

            av_frame_unref(_audioRawBuffer);
            _audioSwContext = swr_alloc();
            if (_videoSwContext == null)
            {
                if (consoleDebug)
                    Console.WriteLine("\tNthDimension.FFMpeg: Failed to create audio resampling context");
                _audioStreamId = -1;
                return;
            }

            ulong inchanlayout = _audioContext->channel_layout;
            if (inchanlayout == 0) inchanlayout = (ulong)av_get_default_channel_layout(_audioContext->channels);
            ulong outchanlayout = inchanlayout;
            if (outchanlayout != AV_CH_LAYOUT_MONO) outchanlayout = AV_CH_LAYOUT_STEREO;
            av_opt_set_int(_audioSwContext, "in_channel_layout", (long)inchanlayout, 0);
            av_opt_set_int(_audioSwContext, "out_channel_layout", (long)outchanlayout, 0);
            av_opt_set_int(_audioSwContext, "in_sample_rate", _audioContext->sample_rate, 0);
            av_opt_set_int(_audioSwContext, "out_sample_rate", _audioContext->sample_rate, 0);
            av_opt_set_sample_fmt(_audioSwContext, "in_sample_fmt", _audioContext->sample_fmt, 0);
            av_opt_set_sample_fmt(_audioSwContext, "out_sample_fmt", AVSampleFormat.AV_SAMPLE_FMT_S16, 0);
            swr_init(_audioSwContext);
            AudioChannelCount = av_get_channel_layout_nb_channels(outchanlayout);

            AudioPlayback = new AudioPlayback(_audio, this);
        }
       

        public bool Created
        {
            get { return m_sourceInitializedFlag; }
        }

        public void InitializeSource()
        {
            try
            {
                if (m_sourceInitializedFlag) return;

                if (m_videoEnabled) InitVideo();
                if (m_audioEnabled) InitAudio();

                if (_formatContext->duration != AV_NOPTS_VALUE)
                    FileLength = TimeSpan.FromTicks((long)(_formatContext->duration / 1000d * TimeSpan.TicksPerMillisecond));

                if (HasVideo || HasAudio)
                {
                    StartDecodeThread(); // ORIGINAL
                    if (m_videoEnabled) VideoPlayback?.SourceReloaded();
                    if (m_audioEnabled) AudioPlayback?.SourceReloaded();
                }
                else
                {
                    if (consoleDebug)
                        Console.WriteLine("\t(!) NthDimension.FFMpeg: Failed to load audio or video");
                    Dispose();
                }

                m_sourceInitializedFlag = true;
            }
            catch(Exception iE)
            {

#if DEBUG
                if (consoleDebug)
                    System.Diagnostics.Debug.Print(iE.Message);
#endif
            }
        }


        private void createFrame()
        {
            StartDecodeThread(); // ORIGINALy in InitializeSource()

            if (avcodec_open2(_videoContext, _videoCodec, null) != 0)
            {
                if (consoleDebug)
                    Console.WriteLine("Motion: Failed to load video codec");
                _videoStreamId = -1;
                return;
            }

            _videoSize = new Size(_videoContext->width, _videoContext->height);
            _videoRawFrame = CreateVideoFrame(_videoContext->pix_fmt, _videoSize.Width, _videoSize.Height, ref _videoRawBuffer);
            _videoRgbaFrame = CreateVideoFrame(AVPixelFormat.AV_PIX_FMT_RGBA, _videoSize.Width, _videoSize.Height, ref _videoRgbaBuffer);

            if (_videoRawFrame == null || _videoRgbaFrame == null)
            {
                if (consoleDebug)
                    Console.WriteLine("Motion: Failed to create video frames");
                _videoStreamId = -1;
                return;
            }

            int swapmode = SWS_FAST_BILINEAR;
            if (_videoSize.Width * _videoSize.Height <= 500000 && _videoSize.Width % 8 != 0) swapmode |= SWS_ACCURATE_RND;
            _videoSwContext = sws_getCachedContext(null, _videoSize.Width, _videoSize.Height, _videoContext->pix_fmt, _videoSize.Width, _videoSize.Height, AVPixelFormat.AV_PIX_FMT_RGBA, swapmode, null, null, null);

            if (_videoSwContext == null)
            {
                if (consoleDebug)
                    Console.WriteLine("Motion: Failed to create video scaling context");
                _videoStreamId = -1;
            }


        }

      

        ~VideoSource() => Dispose();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Stop();
            StopDecodeThread();
            _playingOffset = TimeSpan.Zero;
            FileLength = TimeSpan.Zero;
            _videoSize = Size.Empty;
            _videoStreamId = -1;
            _audioStreamId = -1;
            AudioChannelCount = -1;

            if (_videoContext != null)
            {
                avcodec_close(_videoContext);
                _videoContext = null;
            }
            _videoCodec = null;
            if (_audioContext != null)
            {
                avcodec_close(_audioContext);
                _audioContext = null;
            }
            _audioCodec = null;
            if (_videoRawFrame != null)
            {
                DestroyVideoFrame(ref _videoRawFrame, ref _videoRawBuffer);
                _videoRawFrame = null;
            }
            if (_videoRgbaFrame != null)
            {
                DestroyVideoFrame(ref _videoRgbaFrame, ref _videoRgbaBuffer);
                _videoRgbaFrame = null;
            }
            if (_audioRawBuffer != null)
            {
                var audioRawBuffer = _audioRawBuffer;
                av_frame_free(&audioRawBuffer);
                _audioRawBuffer = null;
            }
            if (_audioPcmBuffer != null)
            {
                av_free(_audioPcmBuffer);
                _audioPcmBuffer = null;
            }
            if (_videoSwContext != null)
            {
                sws_freeContext(_videoSwContext);
                _videoSwContext = null;
            }
            if (_audioSwContext != null)
            {
                var audioSwContext = _audioSwContext;
                swr_free(&audioSwContext);
                _audioSwContext = null;
            }
            if (_formatContext != null)
            {
                var formatContext = _formatContext;
                avformat_close_input(&formatContext);
                _formatContext = null;
            }

            VideoPlayback?.Dispose();
            AudioPlayback?.Dispose();
            VideoPlayback = null;
            AudioPlayback = null;
        }

        private static AVFrame* CreateVideoFrame(AVPixelFormat format, int width, int height, ref byte* buffer)
        {
            var frame = av_frame_alloc();
            if (ClearBuffer(frame, format, width, height, ref buffer)) return frame;

            av_frame_free(&frame);
            return null;
        }

        private static bool ClearBuffer(AVFrame* frame, AVPixelFormat format, int width, int height, ref byte* buffer)
        {
            if (frame == null) return false;

            ulong size = (ulong)av_image_get_buffer_size(format, width, height, sizeof(int));
            buffer = (byte*)av_malloc(size);
            if (buffer == null) return false;

            //var data = new BytePtrArray4();
            var data = new byte_ptrArray4();
            //var linesize = new IntArray4();
            var linesize = new int_array4();
            av_image_fill_arrays(ref data, ref linesize, buffer, format, width, height, sizeof(int));
            for (uint i = 0; i < 4; i++)
            {
                frame->data[i] = data[i];
                frame->linesize[i] = linesize[i];
            }
            return true;
        }

        private static void DestroyVideoFrame(ref AVFrame* frame, ref byte* buffer)
        {
            var f = frame;
            av_frame_free(&f);
            frame = null;
            av_free(buffer);
            buffer = null;
        }

        #endregion

        #region Playpack

        public void Play()
        {
            if (!HasVideo && !HasAudio || State == PlayState.Playing) return;

            if (_videoRawFrame == null && _videoRgbaFrame == null)
                this.createFrame();

            IsEndOfFileReached = false;
            _lastUpdate = DateTime.Now;
            NotifyStateChanged(PlayState.Playing);
            State = PlayState.Playing;

            if (null != OnVideoStarted)
                this.OnVideoStarted();
        }

        public void Pause()
        {
            if (State != PlayState.Playing) return;
            NotifyStateChanged(PlayState.Paused);
            State = PlayState.Paused;

            if (null != OnVideoPaused)
                this.OnVideoPaused();
        }

        public void Stop()
        {
            if (State == PlayState.Stopped) return;
           

            if (Loop)
            {
                //_videoRawFrame = null;        // Don't! Memory leak;
                //_videoRgbaFrame = null;       // Don't! Memory Leak

                IsEndOfFileReached              = false;                                               
                PlayingOffset                   = TimeSpan.Zero;
                State                           = PlayState.Playing;
                NotifyStateChanged(PlayState.Playing);
                return;
            }
            else
            {
                IsEndOfFileReached = true;
                NotifyStateChanged(PlayState.Stopped);
                State = PlayState.Stopped;
                StopDecodeThread();
                IsEndOfFileReached = false;
                PlayingOffset = TimeSpan.Zero;
            }

            if (null != OnVideoStopped)
                this.OnVideoStopped();
        }

        public void Update()
        {
            if (PlayingOffset > FileLength)
            {
                Stop();
                IsEndOfFileReached = true;
            }
            var now = DateTime.Now;
            var deltaTime = now - _lastUpdate;
            _lastUpdate = now;
            if (State == PlayState.Playing) _playingOffset += deltaTime;
           
            VideoPlayback?.Update(deltaTime);
            AudioPlayback?.Update();
        }

        private void NotifyStateChanged(PlayState newState)
        {
            VideoPlayback?.StateChanged(State, newState);
            AudioPlayback?.StateChanged(State, newState);
        }

        #endregion

        #region Decode

        private void StartDecodeThread()
        {
            if (_decodeThread != null)
                return;
            _runDecodeThread = true;
            _decodeThread = new Thread(DecodeThreadRun) { Name = "Video Decode" };
            //_decodeThread.IsBackground = true; // Stelios Added
            _decodeThread.Start();
        }

        private void StopDecodeThread()
        {
            if (_decodeThread == null || !_runDecodeThread)
                return;
            _runDecodeThread = false;
            _decodeThread.Join();
            _decodeThread = null;
        }

        private void DecodeThreadRun()
        {
            var packet = av_packet_alloc();
            while (_runDecodeThread)
            {
                while (!IsFull && _runDecodeThread && !_playingToEof)
                {
                    bool validPacket = false;
                    while (!validPacket && _runDecodeThread)
                    {
                        av_init_packet(packet);
                        if (av_read_frame(_formatContext, packet) == 0)
                        {
                            if (packet->stream_index == _videoStreamId)
                            {
                                try
                                {
                                    validPacket = DecodeVideo(packet);
                                }
                                catch(Exception dE)
                                {
                                    if (consoleDebug)
                                        Console.WriteLine(dE.Message);
                                }
                            }
                            else if (packet->stream_index == _audioStreamId)
                            {
                                try
                                {
                                    validPacket = DecodeAudio(packet);
                                }
                                catch(Exception dE)
                                {
                                    if (consoleDebug)
                                        Console.WriteLine(dE.Message);
                                }
                            }
                            else
                            {
                                string weirdIssue = "FFMpeg discarding packet for stream " + packet->stream_index; // If I dont read the packet->stream_index, no texture output... black..!!!
#if DEBUG
                                if (consoleDebug)
                                    Console.WriteLine(weirdIssue);
#else
                                System.Threading.Thread.Sleep(5);
#endif
                            }

                        }
                        else
                        {
                            _playingToEof = true;
                            validPacket = true;
                        }
                        av_packet_unref(packet);
                    }
                }
                Thread.Sleep(50);
            }
            av_packet_free(&packet);
        }

        private bool DecodeVideo(AVPacket* packet)
        {

            if(avcodec_send_packet(_videoContext, packet) < 0) return false;
			if(avcodec_receive_frame(_videoContext, _videoRawFrame) < 0) return false;

            int sws_scale_result = sws_scale(_videoSwContext,
                                     _videoRawFrame->data,
                                     _videoRawFrame->linesize,
                                     0,
                                     _videoContext->height,
                                     _videoRgbaFrame->data,
                                     _videoRgbaFrame->linesize);


            if (sws_scale_result == 0) return false;

			var videoPacket = new VideoPacket(_videoRgbaFrame->data[0], TimeSpan.Zero);
			VideoPlayback.PushPacket(videoPacket);
			ClearBuffer(_videoRgbaFrame, AVPixelFormat.AV_PIX_FMT_RGBA, _videoSize.Width, _videoSize.Height, ref _videoRgbaBuffer);

			return true;
        }

        private bool DecodeAudio(AVPacket* packet)
        {

            if (avcodec_send_packet(_audioContext, packet) < 0) return false;
            if (avcodec_receive_frame(_audioContext, _audioRawBuffer) < 0) return false;

            var audioPcmBuffer = _audioPcmBuffer;
            int convertlength = swr_convert(_audioSwContext, &audioPcmBuffer, _audioRawBuffer->nb_samples, _audioRawBuffer->extended_data, _audioRawBuffer->nb_samples);

            if (convertlength <= 0) return false;

            var audioPacket = new AudioPacket(_audioPcmBuffer, convertlength, AudioChannelCount);
            AudioPlayback.PushPacket(audioPacket);

            return true;
        }

        #endregion
    }

    public enum PlayState
    {
        Stopped, Playing, Paused
    }
}
