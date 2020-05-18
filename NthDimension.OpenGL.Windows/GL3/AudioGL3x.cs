using NthDimension.Algebra;
using NthDimension.Rasterizer;
using NthDimension.Rasterizer.Windows;
using OpenTK.Audio.OpenAL;
using System;

namespace NthDimension.Rasterizer.Windows
{
    /// <summary>
    /// NDAudio Driver based on OpenAL for OpenGL applications
    /// </summary>
    public class AudioGL3x : AudioBase
    {
        public System.IntPtr                    AudioDevice;
        internal static OpenTK.ContextHandle    AudioContext;

        private const string _VERSION = "1.1.0";

        public override void Create()
        {
            AudioDevice = Alc.OpenDevice(null);
            AudioContext = Alc.CreateContext(AudioDevice, new int[0]);
            Alc.MakeContextCurrent(AudioContext);

            

            this.DeviceName = string.Format("NthDimension NDAudio {0} driver based on OpenAL for OpenGL applications {2}\t  Version \t:{0}{2}\t  Context Id\t:{1} ", _VERSION, AudioContext.Handle, Environment.NewLine);
        }
        public override void Destroy()
        {
            Alc.MakeContextCurrent(OpenTK.ContextHandle.Zero);
            Alc.DestroyContext(AudioContext);
            Alc.CloseDevice(AudioDevice);
        }
        public override void BufferData<TBuffer>(int bid, ALFormat format, TBuffer[] buffer, int size, int freq)
        {
            OpenTK.Audio.OpenAL.AL.BufferData(bid, format.ToOpenTK(), buffer, size, freq);
        }

        public override void Source(int sid, ALSourceb param, bool value)
        {
            OpenTK.Audio.OpenAL.AL.Source(sid, param.ToOpenTK(), value);
        }
        public override void Source(int sid, ALSourcei param, int value)
        {
            OpenTK.Audio.OpenAL.AL.Source(sid, param.ToOpenTK(), value);
        }
        public override void Source(int sid, ALSourcef param, float value)
        {
            OpenTK.Audio.OpenAL.AL.Source(sid, param.ToOpenTK(), value);
        }
        public override void Source(int sid, ALSource3f param, float value1, float value2, float value3)
        {
            OpenTK.Audio.OpenAL.AL.Source(sid, param.ToOpenTK(), value1, value2, value3);
        }
        public override void Listener(ALListener3f param, Vector3 vector)
        {
            base.Listener(param, vector);
        }
        public override void Listener(ALListener3f param, float value1, float value2, float value3)
        {
            base.Listener(param, value1, value2, value3);
        }
        public override void DeleteBuffer(int buffer)
        {
            OpenTK.Audio.OpenAL.AL.DeleteBuffer(buffer);
        }
        public override void DeleteSource(int source)
        {
            OpenTK.Audio.OpenAL.AL.DeleteSource(source);
        }
        public override int GenBuffer()
        {
            return OpenTK.Audio.OpenAL.AL.GenBuffer();
        }
        public override int GenSource()
        {
            return OpenTK.Audio.OpenAL.AL.GenSource();
        }
        public override void GetSource(int sid, ALGetSourcei param, out int value)
        {
            OpenTK.Audio.OpenAL.AL.GetSource(sid, param.ToOpenTK(), out value);
        }

        public override void MakeCurrentContext()
        {
            Alc.MakeContextCurrent(AudioContext);
        }
        public override void SourcePause(int sid)
        {
            OpenTK.Audio.OpenAL.AL.SourcePause(sid);
        }
        public override void SourcePlay(int sid)
        {
            OpenTK.Audio.OpenAL.AL.SourcePlay(sid);
        }
        public override void SourceQueueBuffer(int source, int buffer)
        {
            OpenTK.Audio.OpenAL.AL.SourceQueueBuffer(source, buffer);
        }
        public override void SourceStop(int sid)
        {
            OpenTK.Audio.OpenAL.AL.SourceStop(sid);
        }
        public override void SourceUnqueueBuffers(int sid, int numEntries, int[] bids)
        {
            OpenTK.Audio.OpenAL.AL.SourceUnqueueBuffers(sid, numEntries, bids);
        }

        //public override void Update()
        //{
        //    var ctx = Alc.GetCurrentContext();

        //    if(ctx != AudioContext)
        //        Alc.MakeContextCurrent(AudioContext);
        //}
    }
}
