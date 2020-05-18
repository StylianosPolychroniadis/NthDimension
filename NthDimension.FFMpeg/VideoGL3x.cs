//#define DEBUG_FFMPEG
namespace NthDimension.FFMpeg
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NthDimension.Rasterizer;
    using static FFmpeg.AutoGen.ffmpeg;

    public static class VideoGL3x
    {
        public static List<NthDimension.FFMpeg.VideoSource> Sources = new List<FFMpeg.VideoSource>();
        /// <summary>
        /// VideoLoader must be instanciated and collect data before OpenGL begins. Access Violations otherwise
        /// </summary>
        /// <param name="lookupDir"></param>
        /// <param name="renderer"></param>
        /// <param name="audio"></param>
        public static void ReadFolder(string lookupDir, string extension = "*.mp4"/*, RendererBase renderer, AudioBase audio*/)
        {
            foreach (string f in Directory.GetFiles(lookupDir, extension))
            {
#if DEBUG_FFMPEG
                VideoSource vid = new FFMpeg.VideoSource(/*renderer, audio,*/ f, true, true, true);
#else
                VideoSource vid = new FFMpeg.VideoSource(/*renderer, audio,*/ f, true, true, false);
#endif
                if(!Sources.Contains(vid))
                    Sources.Add(vid);
            }
        }
        public static VideoSource Read(string filename, bool videoEnabled = true, bool audioEnabled = true)
        {
#if DEBUG_FFMPEG
            VideoSource vid = new FFMpeg.VideoSource(filename, videoEnabled, audioEnabled, true);
#else
            VideoSource vid = new FFMpeg.VideoSource(filename, videoEnabled, audioEnabled, false);
#endif
            if (!Sources.Contains(vid))
                Sources.Add(vid);
            return vid;
        }

        public static RendererBaseGL3 Renderer { get; private set; }
        public static AudioBase Audio { get; private set; }

        public static void SetupOpenGL(RendererBaseGL3 renderer, AudioBase audio)
        {
            Renderer = renderer;
            Audio = audio;

            //Console.WriteLine(string.Format("-AVUtil    {0}.{1}.{2}", LIBAVUTIL_VERSION_MAJOR, LIBAVUTIL_VERSION_MINOR, LIBAVUTIL_VERSION_MICRO));
            //Console.WriteLine(string.Format("-AVCodec   {0}.{1}.{2}", LIBAVCODEC_VERSION_MAJOR, LIBAVCODEC_VERSION_MINOR, LIBAVCODEC_VERSION_MICRO));
            //Console.WriteLine(string.Format("-AVFormat  {0}.{1}.{2}", LIBAVFORMAT_VERSION_MAJOR, LIBAVFORMAT_VERSION_MINOR, LIBAVFORMAT_VERSION_MICRO));
            //Console.WriteLine(string.Format("-AVDevice  {0}.{1}.{2}", LIBAVDEVICE_VERSION_MAJOR, LIBAVDEVICE_VERSION_MINOR, LIBAVDEVICE_VERSION_MICRO));
            //Console.WriteLine(string.Format("-AVFilter  {0}.{1}.{2}", LIBAVFILTER_VERSION_MAJOR, LIBAVFILTER_VERSION_MINOR, LIBAVFILTER_VERSION_MICRO));
            //Console.WriteLine(string.Format("-SWScale   {0}.{1}.{2}", LIBSWSCALE_VERSION_MAJOR, LIBSWSCALE_VERSION_MINOR, LIBSWSCALE_VERSION_MICRO));
            //Console.WriteLine(string.Format("-PostProc  {0}.{1}.{2}", LIBPOSTPROC_VERSION_MAJOR, LIBPOSTPROC_VERSION_MINOR, LIBPOSTPROC_VERSION_MICRO));
            Console.WriteLine(Environment.NewLine);

        }
    }
}
