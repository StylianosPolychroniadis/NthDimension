using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static FFmpeg.AutoGen.ffmpeg;

namespace NthDimension.FFMpeg
{
    public unsafe class FFmpegMediaInfo : IDisposable
    {
        internal AVFormatContext* AVFormatContext;

        /// <summary>The filename as passed from AVUTIL</summary>
        public string Filename { get; internal set; }
        /// <summary>The duration</summary>
        public TimeSpan Duration { get; internal set; }
        /// <summary>The average bit rate</summary>
        public long BitRate { get; internal set; }
        /// <summary>The video resolution of the first video stream</summary>
        public Size VideoResolution { get; internal set; }
        /// <summary>Found stream information</summary>
        public List<FFmpegStreamInfo> Streams { get; internal set; }
        /// <summary>Meta information catalog</summary>
        public Dictionary<string, string> Metadata { get; internal set; }

        #region Constructors

        public FFmpegMediaInfo()
        {
            InitDllDirectory();

            this.Metadata = new Dictionary<string, string>();
            this.Streams = new List<FFmpegStreamInfo>();
        }

        public FFmpegMediaInfo(string filename) : this()
        {
            this.OpenFileOrUrl(filename);
        }

        #endregion

        /// <summary>
        /// Disposes all active objects and opens a file or url
        /// </summary>
        /// <param name="url">path of the media file</param>
        public void OpenFileOrUrl(string url)
        {
            this.Dispose();

            // Initialize instance of AVUTIL, AVCODEC, AVFORMAT
            av_register_all();
            avcodec_register_all();
            avformat_network_init();

            // Open source file
            AVFormatContext* pFormatContext = avformat_alloc_context();
            if (avformat_open_input(&pFormatContext, url, null, null) != 0)
                throw new Exception("File or URL not found!");

            if (avformat_find_stream_info(pFormatContext, null) != 0)
                throw new Exception("No stream information found!");

            // Collect general media information

            throw new NotImplementedException();
            // this.Filename = new System.Text.Encoding.Unicode.GetString(pFormatContext->filename);
            this.Metadata = ToDictionary(pFormatContext->metadata);
            this.Duration = ToTimeSpan(pFormatContext->duration);
            this.BitRate = pFormatContext->bit_rate;

            // Collect information about streams
            bool isFirstVideoStream = true;
            for (int i = 0; i < pFormatContext->nb_streams; i++)
            {
                #region Read stream information
                FFmpegStreamInfo info = new FFmpegStreamInfo();
                info.AVStream = pFormatContext->streams[i];
                info.Index = pFormatContext->streams[i]->index;
                switch (pFormatContext->streams[i]->codec->codec_type)
                {
                    case AVMediaType.AVMEDIA_TYPE_VIDEO:
                        info.StreamType = FFmpegStreamType.Video;
                        info.Video_Width = pFormatContext->streams[i]->codec->width;
                        info.Video_Height = pFormatContext->streams[i]->codec->height;
                        info.Video_FPS = ToDouble(pFormatContext->streams[i]->codec->framerate);
                        if (isFirstVideoStream)
                        {
                            this.VideoResolution = new Size(info.Video_Width, info.Video_Height);
                            isFirstVideoStream = false;
                        }
                        break;
                    case AVMediaType.AVMEDIA_TYPE_AUDIO:
                        info.StreamType = FFmpegStreamType.Audio;
                        info.Audio_SampleRate = pFormatContext->streams[i]->codec->sample_rate;
                        info.Audio_Channels = pFormatContext->streams[i]->codec->channels;
                        break;
                    case AVMediaType.AVMEDIA_TYPE_SUBTITLE:
                        info.StreamType = FFmpegStreamType.Subtitle;
                        break;
                    default:
                        info.StreamType = FFmpegStreamType.Unknown;
                        break;
                }
                info.MetaData = ToDictionary(pFormatContext->streams[i]->metadata);
                if (info.MetaData.ContainsKey("language")) info.Language = info.MetaData["language"];
                info.BitRate = pFormatContext->streams[i]->codec->bit_rate;
                info.FourCC = ToFourCCString(pFormatContext->streams[i]->codec->codec_tag);
                info.CodecName = avcodec_get_name(pFormatContext->streams[i]->codec->codec_id);
                this.Streams.Add(info);
                #endregion
            }

            this.AVFormatContext = pFormatContext;
            _isDisposed = false;
        }

        /// <summary>Extracts frame images for each timestamp in positions</summary>
        /// <param name="streamIndex">The index of the video stream to extract the frames from (-1 selects the first video stream)</param>
        /// <param name="positions">The timestamps in the video to take a frame at</param>
        /// <param name="forceExactTimePosition">If false, the last key frame is used - can result in inaccurate time positions but is much faster</param>
        /// <param name="onProgress">Is called after each taken frame and passes (int currentIndex, int positions.Count) - if true is returned, the loop is canceled</param>
        public List<VideoFrameEntry> GetFrames(int streamIndex, List<TimeSpan> positions, bool forceExactTimePosition, Func<int, int, bool> onProgress)
        {
            FFmpegStreamInfo stream;
            if (streamIndex == -1)
                stream = this.Streams.FirstOrDefault(s => s.StreamType == FFmpegStreamType.Video);
            else
                stream = this.Streams[streamIndex];

            if (stream == null || stream.StreamType != FFmpegStreamType.Video)
                throw new Exception("No video stream selected!");

            var pos = positions.Select(ts => ts.Ticks / 10L).ToList();

            List<VideoFrameEntry> result = new List<VideoFrameEntry>();
            ExtractFrames(this.AVFormatContext, stream.AVStream, pos, forceExactTimePosition, (i, ts, img) =>
            {
                if (img != null)
                    result.Add(new VideoFrameEntry(ts, img));

                if (onProgress != null)
                    return onProgress(i, pos.Count);
                else
                    return false;
            });

            return result;
        }

        /// <summary>Creates a thumbnail sheet</summary>
        /// <param name="streamIndex">The index of the video stream to extract the frames from (-1 selects the first video stream)</param>
        /// <param name="options">Options setting the thumbnail sheet's appearence</param>
        /// <param name="onProgress">Is called after each taken frame and passes (int currentIndex, int positions.Count) - if true is returned, the loop is canceled</param>
        public Bitmap GetThumbnailSheet(int streamIndex, VideoThumbSheetOptions options, Func<int, int, bool> onProgress)
        {
            FFmpegStreamInfo stream;
            if (streamIndex == -1)
                stream = this.Streams.FirstOrDefault(s => s.StreamType == FFmpegStreamType.Video);
            else
                stream = this.Streams[streamIndex];

            if (stream == null || stream.StreamType != FFmpegStreamType.Video)
                throw new Exception("No video stream selected!");

            return ExtractVideoThumbnailSheet(this, this.AVFormatContext, stream.AVStream, options, onProgress);
        }


        private bool _isDisposed = true;
        public void Dispose()
        {
            if (_isDisposed) return;

            AVFormatContext* pFormatContext = this.AVFormatContext;
            avformat_close_input(&pFormatContext);

            _isDisposed = true;
        }


        #region static methods

        private unsafe static Bitmap ExtractNextImage(AVFormatContext* pFormatContext, AVCodecContext* pCodecContext, AVPacket* pPacket, AVStream* vidStream, SwsContext* pConvertContext, AVFrame* pDecodedFrame, AVPicture* pConvertedFrame, int width, int height, bool createCopy, double timeBase, out TimeSpan pos)
        {
            pos = new TimeSpan();
            Bitmap result = null;

            int gotPicture = 0;

            while (gotPicture != 1)
            {
                if (av_read_frame(pFormatContext, pPacket) < 0)
                {
                    result = null;
                    break;
                }

                if (pPacket->stream_index != vidStream->index)
                    continue;

                gotPicture = 0;
                int size = avcodec_decode_video2(pCodecContext, pDecodedFrame, &gotPicture, pPacket);
                if (size < 0)
                    throw new Exception("Error while decoding frame!");

                if (gotPicture == 1)
                {
                    //// Get current position from frame
                    //pos = ToTimeSpan(av_frame_get_best_effort_timestamp(pDecodedFrame), timeBase);

                    //// Extract image
                    //byte*[] src = &pDecodedFrame->data[0];// data_0;
                    //byte*[] dst = &pConvertedFrame->data[0];// data_0;
                    //sws_scale(pConvertContext, src, pDecodedFrame->linesize, 0, height, dst, pConvertedFrame->linesize);
                    //var imageBufferPtr = new IntPtr(pConvertedFrame->data[0] /*data_0*/);
                    //int linesize = pConvertedFrame->linesize[0];
                    //Bitmap img = new Bitmap(width, height, linesize, PixelFormat.Format24bppRgb, imageBufferPtr);

                    //result = createCopy ? new Bitmap(img) : img;
                }
            }

            return result;
        }

        /// <summary>Extracts frame images</summary>
        /// <param name="pFormatContext">The format context to use</param>
        /// <param name="vidStream">The video stream to use</param>
        /// <param name="positions">A list of timestamps in 10ths of Ticks</param>
        /// <param name="anyPosition">If false, the last key frame is used - can result in inaccurate time positions but is much faster</param>
        /// <param name="onProgress">Is called after every taken frame passing the list index, frame timestamp and frame image - if true is returned, the loop is canceled</param>
        private unsafe static void ExtractFrames(AVFormatContext* pFormatContext, AVStream* vidStream, List<long> positions, bool anyPosition, Func<int, TimeSpan, Bitmap, bool> onProgress)
        {
            #region Preparations

            throw new NotImplementedException();
            
            //AVCodecContext codecContext = *(vidStream->codec);


            //int width = codecContext.width;
            //int height = codecContext.height;
            //long duration = pFormatContext->duration;
            //AVPixelFormat sourcePixFmt = codecContext.pix_fmt;
            //AVCodecID codecId = codecContext.codec_id;
            //var convertToPixFmt = AVPixelFormat.AV_PIX_FMT_BGR24;


            //SwsContext* pConvertContext = sws_getContext(width, height, sourcePixFmt, width, height, convertToPixFmt, SWS_FAST_BILINEAR, null, null, null);
            //if (pConvertContext == null)
            //    throw new Exception("Could not initialize the conversion context");
            //AVCodecContext* pCodecContext = &codecContext;

            //var pConvertedFrame = (AVPicture*)avcodec_alloc_frame();
            //int convertedFrameBufferSize = avpicture_get_size(convertToPixFmt, width, height);
            //var pConvertedFrameBuffer = (byte*)av_malloc((uint)convertedFrameBufferSize);
            //avpicture_fill(pConvertedFrame, pConvertedFrameBuffer, convertToPixFmt, width, height);

            //AVCodec* pCodec = avcodec_find_decoder(codecId);
            //if (pCodec == null)
            //    throw new Exception("Unsupported codec");

            //if ((pCodec->capabilities & CODEC_CAP_TRUNCATED) == CODEC_CAP_TRUNCATED)
            //    pCodecContext->flags |= CODEC_FLAG_TRUNCATED;

            //if (avcodec_open2(pCodecContext, pCodec, null) < 0)
            //    throw new Exception("Could not open codec");

            //AVFrame* pDecodedFrame = avcodec_alloc_frame();

            //var packet = new AVPacket();
            //AVPacket* pPacket = &packet;
            //av_init_packet(pPacket);

            //AVCodecContext cont = *vidStream->codec;
            #endregion

            // Seek for key frames only - otherwise first frames are currupted until a key frame is decoded
            pFormatContext->seek2any = 0;

            TimeSpan pos;
            long currTS;
            double timeBase = ToDouble(vidStream->time_base); // DTS or PTS timestamp to seconds multiplicator
            Bitmap img;
            for (int f = 0; f < positions.Count; f++)
            {
                //currTS = positions[f];

                //// Seek to last keyframe before next position
                //vidStream->skip_to_keyframe = 1;
                //av_seek_frame(pFormatContext, -1, currTS, AVSeek.AVSEEK_FLAG_BACKWARD);

                //// Decode next image - ATTENTION: Get a image copy or it will be unallocated!
                //img = ExtractNextImage(pFormatContext, pCodecContext, pPacket, vidStream, pConvertContext, pDecodedFrame, pConvertedFrame, width, height, true, timeBase, out pos);

                //// If any Position is to be used, go though the next images until the imate position is within timebase or is getting further away again
                //if (anyPosition)
                //{
                //    vidStream->skip_to_keyframe = 0;
                //    TimeSpan currTsSpan = ToTimeSpan(currTS);
                //    double lastDiff = double.MaxValue;
                //    double currDiff;
                //    while ((currDiff = Math.Abs((currTsSpan - pos).TotalSeconds)) > timeBase && currDiff < lastDiff)
                //    {
                //        lastDiff = Math.Abs((currTsSpan - pos).TotalSeconds);
                //        img.Dispose();
                //        img = ExtractNextImage(pFormatContext, pCodecContext, pPacket, vidStream, pConvertContext, pDecodedFrame, pConvertedFrame, width, height, true, timeBase, out pos);
                //    }
                //    vidStream->skip_to_keyframe = 1;
                //    GC.Collect();
                //}

                if (img == null) continue;

                if (onProgress != null)
                {
                    if (onProgress(f, pos, img)) break;
                }
            } // end while

            #region Free allocated memory
            //av_free(pConvertedFrame);
            //av_free(pConvertedFrameBuffer);
            //sws_freeContext(pConvertContext);
            //av_free(pDecodedFrame);
            //avcodec_close(pCodecContext);
            #endregion
        }

        /// <summary>Creates a thumbnail</summary>
        /// <param name="info">Media information to use for header text</param>
        /// <param name="pFormatContext">The format context to use</param>
        /// <param name="vidStream">The video stream to use</param>
        /// <param name="options">Options setting the appearence of the sheet</param>
        /// <param name="onProgress">Is called after each taken frame and passes (int currentIndex, int positions.Count) - if true is returned, the loop is canceled</param>
        private unsafe static Bitmap ExtractVideoThumbnailSheet(FFmpegMediaInfo info, AVFormatContext* pFormatContext, AVStream* vidStream, VideoThumbSheetOptions options, Func<int, int, bool> onProgress)
        {
            #region Structure diagram
            /* m := Margin, p := Padding, tw := ThumbWidth, th := ThumbHeight, hh := HeaderHeight, ### := Thumbnail image
             
                 m ###tw### p ###tw### p ###tw### p ###tw### p ###tw### m
                +--------------------------------------------------------+ 
                |                                                        | m     }  Margin
                | +----------------------------------------------------+ | 
                | |Filename: xxxxxxxxxxxxxxxxxxxxxx.avi                | | xx    \
                | |Length: 0:00:00    Resolution: 640x480 @25FPS       | | hh     } Header height
                | |Streams: Video[div5/xvid], Audio[mp3a/MP3/eng]      | | xx    /
                | +----------------------------------------------------+ | 
                |                                                        | 2*p   }  2 * Padding
                | +----------------------------------------------------+ |
                | |########| |########| |########| |########| |########| | ##    \
                | |########| |########| |########| |########| |########| | th     } Thumbnail height
                | |########| |########| |########| |########| |########| | ##    /
                | +----------------------------------------------------+ |
                |                                                        | p     }  1* Padding
                | +----------------------------------------------------+ |
                | |########| |########| |########| |########| |########| |
                ...                         ...                        ...
                ...                         ...                        ...
                | |########| |########| |########| |########| |########| |
                + +----------------------------------------------------+ + 
                |                                                        | m     }  Margin
                +--------------------------------------------------------+ 
             */
            #endregion

            #region Header and image preperations
            // Get video info from codec
            int width = vidStream->codec->width;
            int height = vidStream->codec->height;
            AVRational framerate = vidStream->codec->framerate;
            long duration = pFormatContext->duration;

            // Create header text and calculate its height (using current video stream)
            string header = String.Format("Filename: {0}\r\nLength: {1}    Resolution: {2}x{3} @{4}FPS\r\nStreams: {5}",
                Path.GetFileName(info.Filename),
                ToFormattedString(info.Duration),
                width, height, ToDescString(framerate),
                String.Join(", ", info.Streams.Select(s => s.ToString()).ToArray())
            );
            int headerHight = Convert.ToInt32(MeassureString(header, options.HeaderFont).Height);

            // Calculate image sizes and create image
            int thumbHeight = height * options.ThumbWidth / width;
            int imgWidth = 2 * options.Margin + options.ThumbColumns * (options.ThumbWidth + options.Padding) - options.Padding;
            int imgHeight = 2 * options.Margin + headerHight + options.ThumbRows * (thumbHeight + options.Padding) + options.Padding;
            int thumbTop = options.Margin + headerHight + 2 * options.Padding;
            Bitmap bmp = new Bitmap(imgWidth, imgHeight);

            int count = options.ThumbColumns * options.ThumbRows;
            List<long> positions = CreateRegularTimePositionList(duration, count);
            #endregion

            using (Graphics g = Graphics.FromImage(bmp))
            using (Brush headerBrush = new SolidBrush(options.HeaderColor))
            using (Pen borderPen = new Pen(options.ThumbBorderColor, 1f))
            using (Brush indexShadowBrush = new SolidBrush(options.IndexShadowColor))
            using (Brush indexBrush = new SolidBrush(options.IndexColor))
            {
                // Set background color
                g.Clear(options.BackgroundColor);

                // Draw header text
                float marginF = Convert.ToSingle(options.Margin);
                g.DrawString(header, options.HeaderFont, headerBrush, new PointF(marginF, marginF));

                // Loop through images as they are extracted
                int c, r, x, y;
                string tsText;
                SizeF tsSize;
                float ix, iy;
                ExtractFrames(pFormatContext, vidStream, positions, options.ForceExactTimePosition, (i, ts, img) =>
                {
                    // Calculate positions and sizes
                    c = i % options.ThumbColumns; // column
                    r = i / options.ThumbColumns; // row
                    x = options.Margin + c * (options.ThumbWidth + options.Padding); // thumb left
                    y = thumbTop + r * (thumbHeight + options.Padding); // thumb top
                    tsText = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds); // timestamp text
                    tsSize = g.MeasureString(tsText, options.IndexFont); // timestamp text size
                    ix = Convert.ToSingle(x + options.ThumbWidth) - tsSize.Width - 3f; // timestamp text left
                    iy = Convert.ToSingle(y + thumbHeight) - tsSize.Height - 3f; // timestamp text top

                    // Insert thumbnail image resized
                    g.DrawImage(img, x, y, options.ThumbWidth, thumbHeight);

                    // Overdraw edges with border
                    if (options.DrawThumbnailBorder)
                        g.DrawRectangle(borderPen, x, y, options.ThumbWidth, thumbHeight);

                    // Draw timestamp shadow
                    g.DrawString(tsText, options.IndexFont, indexShadowBrush, ix - 1f, iy - 1f);
                    g.DrawString(tsText, options.IndexFont, indexShadowBrush, ix + 1f, iy + 1f);
                    // Draw timestamp
                    g.DrawString(tsText, options.IndexFont, indexBrush, ix, iy);

                    // Dispose the thumbnail image because it's not needed anymore
                    img.Dispose();

                    // Publish progress
                    if (onProgress != null)
                        return onProgress(i, count);
                    else
                        return false;
                });
            }

            return bmp;
        }

        private static List<long> CreateRegularTimePositionList(long duration, int count)
        {
            List<long> pos = new List<long>();
            long curr = 0;
            long step = duration / (count + 2); // without start and end
            for (int i = 0; i < count; i++)
            {
                curr += step;
                pos.Add(curr);
            }
            return pos;
        }

        #region InitDllDirectory

        private static void InitDllDirectory()
        {
            if (_initDllDirectoryDone) return;

            SetDllDirectory(IntPtr.Size == 8 ? "x64" : "x86"); // In .NET 4 you can use Environment.Is64BitProcess

            _initDllDirectoryDone = true;
        }

        private static bool _initDllDirectoryDone = false;

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        #endregion

        #region Conversions

        private unsafe static Dictionary<string, string> ToDictionary(AVDictionary* dict)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            int size = av_dict_count(dict);
            int AV_DICT_IGNORE_SUFFIX = 2;
            AVDictionaryEntry* curr = null;
            for (int n = 0; n < size; n++)
            {
                //curr = av_dict_get(dict, "", curr, AV_DICT_IGNORE_SUFFIX);
                //string key = new String(curr->key);
                //string val = new String(curr->value);
                //result[key] = val;
            }

            return result;
        }

        private static string ToFourCCString(int value)
        {
            return value == 0 ? "" : Encoding.ASCII.GetString(new byte[]
            {
                Convert.ToByte(value & 0xFF),
                Convert.ToByte((value >> 8) & 0xFF),
                Convert.ToByte((value >> 16) & 0xFF),
                Convert.ToByte((value >> 24) & 0xFF)
            });
        }
        private static string ToFourCCString(uint value)
        {
            return value == 0 ? "" : Encoding.ASCII.GetString(new byte[]
            {
                Convert.ToByte(value & 0xFF),
                Convert.ToByte((value >> 8) & 0xFF),
                Convert.ToByte((value >> 16) & 0xFF),
                Convert.ToByte((value >> 24) & 0xFF)
            });
        }
        private static double ToDouble(AVRational value)
        {
            return Convert.ToDouble(value.num) / Convert.ToDouble(value.den);
        }
        private static string ToDescString(AVRational value)
        {
            double dVal = Math.Floor(ToDouble(value) * 10.0) / 10;
            return dVal.ToString();
        }

        private static TimeSpan ToTimeSpan(long value)
        {
            return new TimeSpan(value * 10L);
        }
        private static TimeSpan ToTimeSpan(long value, double timeBase)
        {
            return TimeSpan.FromSeconds(Convert.ToDouble(value) * timeBase);
        }

        private static string ToFormattedString(TimeSpan s)
        {
            return String.Format("{0}:{1:00}:{2:00}" /*.{3:000}" */, s.Hours, s.Minutes, s.Seconds, s.Milliseconds);
        }

        private static Bitmap GetCopy(Bitmap source)
        {
            return new Bitmap(source);
        }

        private static SizeF MeassureString(string text, Font font)
        {
            SizeF result;

            using (Bitmap bmp = new Bitmap(4, 4))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                result = g.MeasureString(text, font);
            }

            return result;
        }

        #endregion

        #endregion
    }
}
