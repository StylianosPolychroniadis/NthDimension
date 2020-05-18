using System;

namespace NthDimension.Server.Api
{
    public abstract class PacketHandler
    {
        protected IServer Server;           

        protected PacketHandler(IServer server)
        {
            Server = server;
        }

        public abstract void Process(Object packetData, BasePeer sender);
    }
}
