using System;
using System.Collections.Concurrent;

namespace NthDimension.FFMpeg
{
    public abstract class Playback<TPacket> : IDisposable where TPacket : Packet
    {

        public readonly VideoSource DataSource;
        protected readonly BlockingCollection<TPacket> PacketQueue = new BlockingCollection<TPacket>();

        internal int QueuedPackets => PacketQueue.Count;

        protected Playback(VideoSource dataSource)
        {
            DataSource = dataSource;
        }

        internal abstract void SourceReloaded();
        internal abstract void StateChanged(PlayState oldState, PlayState newState);

        internal virtual void Flush()
        {
            while (PacketQueue.TryTake(out var packet)) packet.Dispose();
        }

        internal void PushPacket(TPacket packet)
        {
            PacketQueue.Add(packet);
        }

        public virtual void Dispose()
        {
            Flush();
            PacketQueue.Dispose();
        }
    }
}
