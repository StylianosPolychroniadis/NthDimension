using System;
//using static NthDimension.FFMpeg.FFmpeg4;
using static FFmpeg.AutoGen.ffmpeg;

namespace NthDimension.FFMpeg
{
    public unsafe class VideoPacket : Packet
    {

        public IntPtr RgbaBuffer { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        public VideoPacket(byte* rgbaBuffer, TimeSpan timestamp)
        {
            Timestamp = timestamp;
            RgbaBuffer = new IntPtr(rgbaBuffer);
        }

        public override void Dispose()
        {
            if (RgbaBuffer == IntPtr.Zero) return;
            av_free(RgbaBuffer.ToPointer());
            RgbaBuffer = IntPtr.Zero;
            Timestamp = TimeSpan.Zero;
        }
    }
}
