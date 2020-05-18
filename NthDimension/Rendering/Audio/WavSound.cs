using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Algebra;
using static NthDimension.Rasterizer.AudioBase;

namespace NthDimension.Rendering.Sound
{
    public class WavSound : Audio.AudioSource
    {
        public WavSound(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var reader = new BinaryReader(stream))
            {
                // RIFF header
                var signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                reader.ReadInt32(); // Skip Riff chunk size in header

                var format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                var formatSignature = new string(reader.ReadChars(4));
                if (formatSignature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                reader.ReadInt32(); // Skip format chunk size
                reader.ReadInt16(); // Skip audio format
                Channels = reader.ReadInt16();
                Rate = reader.ReadInt32();
                reader.ReadInt32(); // Skip byte rate
                reader.ReadInt16(); // Skip block align
                Bits = reader.ReadInt16();

                var dataSignature = new string(reader.ReadChars(4));
                if (dataSignature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                reader.ReadInt32(); // Skip data chunk size

                Data = reader.ReadBytes((int)reader.BaseStream.Length);
                Format = GetSoundFormat(Channels, Bits);
            }

            ApplicationBase.Instance.Audio.BufferData(_buffer, Format, Data, Data.Length, Rate);
            ApplicationBase.Instance.Audio.Source(_source, ALSourcei.Buffer, _buffer);
        }
        ~WavSound()
        {
            this.Release();
        }
        public int Channels { get; set; }
        public int Bits { get; set; }
        public int Rate { get; set; }
        public byte[] Data { get; set; }
        public ALFormat Format { get; set; }
        private static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

#if THREAD_PLAY
        //System.Threading.Thread _TPlay; // Testing threaded Audio Play { Todo then Repeat, Pause, Stop, Release }
#endif 
        public override void Play()
        {
            Utilities.ConsoleUtil.log(string.Format("Playing sound {0}", _source, false));

#if THREAD_PLAY
            //_TPlay = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            //{
            //    ApplicationBase.Instance.Audio.SourcePlay(_source);
            //}));
            //_TPlay.Start();
#else
            ApplicationBase.Instance.Audio.SourcePlay(_source);
#endif

        }
        public override void Repeat(bool loop)
        {
            ApplicationBase.Instance.Audio.Source(_source, ALSourceb.Looping, loop);
        }
        public override void Pause()
        {
            ApplicationBase.Instance.Audio.SourcePause(_source);
        }
        public override void Stop()
        {
            ApplicationBase.Instance.Audio.SourceStop(_source);
        }
        public override void Release()
        {
            ApplicationBase.Instance.Audio.SourceStop(_source);
            ApplicationBase.Instance.Audio.DeleteSource(_source);
            ApplicationBase.Instance.Audio.DeleteBuffer(_buffer);
            //OpenTK.Audio.OpenAL.Alc.MakeContextCurrent(OpenTK.ContextHandle.Zero);
            ApplicationBase.Instance.Audio.MakeCurrentContext();
            _context.Dispose();
        }
        public override void SetGainSource(float gain)
        {
            ApplicationBase.Instance.Audio.Source(_source, ALSourcef.Gain, gain);
        }
        public override void SetPositionListener(Vector3 position)
        {
            ApplicationBase.Instance.Audio.Listener(ALListener3f.Position, position.X, position.Y, position.Z);
        }
        public override void SetPositionSource(Vector3 position)
        {
            ApplicationBase.Instance.Audio.Source(_source, ALSource3f.Position, position.X, position.Y, position.Z);
        }
        public override void SetVelocityListener(Vector3 velocity)
        {
            ApplicationBase.Instance.Audio.Listener(ALListener3f.Velocity, velocity.X, velocity.Y, velocity.Z);
        }
        public override void SetVelocitySource(Vector3 velocity)
        {
            ApplicationBase.Instance.Audio.Source(_source, ALSource3f.Velocity, velocity.X, velocity.Y, velocity.Z);
        }
    }
}
