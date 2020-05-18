using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.FFMpeg
{
    public enum FFmpegStreamType { Video, Audio, Subtitle, Unknown }
    public unsafe class FFmpegStreamInfo
    {
        internal AVStream* AVStream;

        /// <summary>The index of the stream</summary>
        public int Index                        { get; set; }
        /// <summary>The type of the stream</summary>
        public FFmpegStreamType StreamType      { get; set; }
        /// <summary>The name of the codec</summary>
        public string CodecName                 { get; set; }
        /// <summary>The FourCC code of the codec</summary>
        public string FourCC                    { get; set; }
        /// <summary>The average bit rate of this stream</summary>
        public long BitRate                      { get; set; }
        /// <summary>Video width (only filled if this is a video stream)</summary>
        public int Video_Width                  { get; set; }
        /// <summary>Video height (only filled if this is a video stream)</summary>
        public int Video_Height                 { get; set; }
        /// <summary>Video frames per second (only filled if this is a video stream)</summary>
        public double Video_FPS                 { get; set; }
        /// <summary>Audio sample rate (only filled if this is a audio stream)</summary>
        public int Audio_SampleRate             { get; set; }
        /// <summary>Number of audio channels in this stream (only filled if this is a audio stream)</summary>
        public int Audio_Channels               { get; set; }
        /// <summary>Copy of MetaData["language"] if exists</summary>
        public string Language                  { get; set; }
        /// <summary>A collection of meta information of this stream</summary>
        public Dictionary<string, string> 
                      MetaData                  { get; set; }

        internal FFmpegStreamInfo()
        {
            this.MetaData = new Dictionary<string, string>();
            this.StreamType = FFmpegStreamType.Unknown;
        }

        public override string ToString()
        {
            switch (this.StreamType)
            {
                case FFmpegStreamType.Video:
                    return String.Format("Video[{0}/{1}]", this.FourCC, this.CodecName, this.BitRate);
                case FFmpegStreamType.Audio:
                    return String.Format(String.IsNullOrEmpty(this.Language) || this.Language.Equals("und", StringComparison.OrdinalIgnoreCase) ? "Audio[{0}/{1}]" : "Audio[{0}/{1}/{3}]", this.FourCC, this.CodecName, this.BitRate, this.Language);
                case FFmpegStreamType.Subtitle:
                    return String.Format("Subs[{0}]", this.Language);
                default:
                    return "Unknown";
            }
        }
    }
}
