using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Server.Api
{
    public interface IServer
    {
        // TODO -> Add Channels
        BasePeer GetPeer(NetConnection connection);
        void BroadcastPacket(ushort packetCode, Object packetData, NetDeliveryMethod deliveryMethod, int sequenceChannel = 0);
        void SendPacket(ushort packetCode, Object packetData, BasePeer peer, NetDeliveryMethod deliveryMethod, int sequenceChannel = 0);
        void SendPacket(ushort packetCode, Object packetData, List<BasePeer> peers, NetDeliveryMethod deliveryMethod, int sequenceChannel = 0);
        void AddPacketHandler(ushort packetCode, PacketHandler packetHandler);
        void RegisterPacketHandlers();
        void RegisterThreads();
    }
}
