using System;

namespace NthDimension.FFMpeg
{
    public abstract class Packet : IDisposable
    {
        ~Packet() => Dispose();
        public virtual void Dispose() => GC.SuppressFinalize(this);
    }
}
