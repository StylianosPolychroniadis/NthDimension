using NthDimension.Rasterizer;
using System.Collections.Generic;
using System.Linq;
//using OpenTK.Audio.OpenAL;

namespace NthDimension.FFMpeg
{
    public class AudioBuffer
    {
        private AudioBase AL;

        private static readonly Dictionary<int, AudioBuffer>    Buffers     = new Dictionary<int, AudioBuffer>();
        private static readonly List<AudioBuffer>               Free        = new List<AudioBuffer>();

        public readonly int                                     Id;
        public readonly int                                     Handle;

        public short[]                                          Data;
        public AudioBase.ALFormat                                         Format      = AudioBase.ALFormat.Stereo16;
        public int                                              SampleRate;

        private AudioBuffer(AudioBase renderer, int size, AudioBase.ALFormat format, int sampleRate)
        {
            AL = renderer;
            Id = Buffers.Count;
            Handle = AL.GenBuffer();

            Init(AL, size, format, sampleRate);
            Bind();

            Buffers.Add(Handle, this);
            //Console.WriteLine("Allocated buffer " + Id);
        }

        public void Init(AudioBase renderer, int size, AudioBase.ALFormat format, int sampleRate)
        {
            Format = format;
            SampleRate = sampleRate;
            if (!Resize(size)) Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[0] = 0;
            }
        }

        public bool Resize(int size)
        {
            if (Data != null && Data.Length == size) return false;

            Data = new short[size];
            return true;
        }

        public void Bind()
        {
            AL.BufferData(Handle, Format, Data, Data.Length, 44100);
        }

        public void MakeAvailable()
        {
            lock (Free) Free.Add(this);
        }

        public void Dispose()
        {
            AL.DeleteBuffer(Handle);
            foreach (var pair in Buffers)
            {
                if (pair.Value != this) continue;
                Buffers.Remove(pair.Key);
                break;
            }
        }

        ~AudioBuffer() => Dispose();

        public static AudioBuffer ByHandle(int handle)
        {
            return Buffers[handle];
        }

        public static AudioBuffer Get(AudioBase audio, int size, AudioBase.ALFormat format, int sampleRate)
        {
            AudioBuffer buffer;
            lock (Free)
            {
                buffer = Free.FirstOrDefault(b => b.Data.Length == size);
                if (buffer != null)
                {
                    Free.Remove(buffer);
                    //Console.WriteLine("Reusing buffer " + buffer.Id);
                }
                else if (Free.Count > 0)
                {
                    buffer = Free[0];
                    //Console.WriteLine("Reusing buffer " + buffer.Id);
                }
                else buffer = new AudioBuffer(audio, size, format, sampleRate);
            }
            buffer.Init(audio, size, format, sampleRate);
            return buffer;
        }
    }
}
