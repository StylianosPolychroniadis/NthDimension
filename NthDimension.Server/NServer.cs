using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Lidgren.Network;
using NthDimension.Network;
using NthDimension.Server.Api;
using NthDimension.Server.Utilities;


namespace NthDimension.Server
{
    public partial class NServer : IServer
    {
        public readonly Dictionary<NetConnection, BasePeer>     Peers;

        internal const string        defaultName                 = "NthDimension.NServer";
        internal const int           defaultConnections          = 10;
        //internal const string        defaultCachePackage         = "mysocinet.dat";              // The zip file containing cache files
        internal const int           defaultPort                 = 35565;
        internal const int           statisticsUpdateInterval    = 1000;                         // milliseconds

        internal object              peersLock                   = new object();

        internal readonly Dictionary<ushort, PacketHandler>  _packetHandlers;
        internal readonly NetServer                          _server;

        private readonly string                             _appName;
        private readonly IPEndPoint                         _endPoint;
        private readonly int                                _maxClientConnections;
        private readonly NetPeerConfiguration               _networkConfiguration;

        #region Ctor
        /// <summary>
        /// Creates a Server Instance to listen to remote procedure calls from network clients
        /// (TODO: Expand protocol by enabling MessageTypes in [Network Configuration] section)
        /// </summary>
        /// <param name="appName">A string identifier to be used as DOMAIN with client connections</param>
        /// <param name="endpoint">The Network Interface to bind NServer to</param>
        /// <param name="userMaxConnections">Maximum number of concurrent connections. Introduces necessity for LoadBalancer</param>
        public NServer(string appName, IPEndPoint endpoint, int userMaxConnections)
        {
            _instance = this;
            this._appName               = appName;
            this._endPoint              = endpoint;
            this._maxClientConnections  = userMaxConnections;

            #region Network Configuration (Setup)
            this._networkConfiguration                          = new NetPeerConfiguration(_appName);
            this._networkConfiguration.AutoExpandMTU            = true;
            this._networkConfiguration.LocalAddress             = this._endPoint.Address;
            this._networkConfiguration.BroadcastAddress         = this._endPoint.Address;
            this._networkConfiguration.Port                     = this._endPoint.Port;
            this._networkConfiguration.MaximumConnections       = this._maxClientConnections;
            // Message Types Lidgren APIUsage
            this._networkConfiguration.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            this._networkConfiguration.EnableMessageType(NetIncomingMessageType.Data);
            this._networkConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            this._networkConfiguration.EnableMessageType(NetIncomingMessageType.StatusChanged);
            //// TODO
            this._networkConfiguration.EnableMessageType(NetIncomingMessageType.Receipt);
            //this._networkConfiguration.EnableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
            //this._networkConfiguration.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            //this._networkConfiguration.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            #endregion Network configuration

            this._server            = new NetServer(this._networkConfiguration);
            this._packetHandlers    = new Dictionary<ushort, PacketHandler>();
            this.Peers              = new Dictionary<NetConnection, BasePeer>();

            this.RegisterPacketHandlers();
            this.RegisterThreads();

            if (!this.CreateDatabase())
                throw new Exception("Database Failed to Start");
        }
        public NServer()
            :this(defaultName, new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), defaultPort), defaultConnections)
        {

        }
        #endregion

        #region IServer Implementation
        public void AddPacketHandler(ushort packetCode, PacketHandler packetHandler)
        {
            if (packetCode <= 256)
            {
                Log.Warn("Could not add packet handler for code {0}: codes 0-256 reserved for framework.", packetCode);
                return;
            }
            _packetHandlers.Add(packetCode, packetHandler);
        }
        public void AddFrameworkPacketHandler(ushort packetCode, PacketHandler packetHandler)
        {
            if (packetCode > 256)
            {
                Log.Warn("Could not add packet handler for code {0}: codes more than 256 reserved for application.",
                    packetCode);
                return;
            }
            _packetHandlers.Add(packetCode, packetHandler);
        }
        public BasePeer GetPeer(NetConnection connection)
        {
            return Peers[connection];
        }
        public void BroadcastPacket(ushort packetCode, object packetData, NetDeliveryMethod deliveryMethod, int sequenceChannel = 0)
        {
            _server.SendToAll(Packet.Instance.Serialize(packetCode, packetData, _server.CreateMessage()), deliveryMethod, sequenceChannel);
        }
        public void SendPacket(ushort packetCode, object packetData, BasePeer peer, NetDeliveryMethod deliveryMethod, int sequenceChannel = 0)
        {
            _server.SendMessage(Packet.Instance.Serialize(packetCode, packetData, _server.CreateMessage()), peer.Connection, deliveryMethod, sequenceChannel);
        }
        public void SendPacket(ushort packetCode, object packetData, List<BasePeer> peers, NetDeliveryMethod deliveryMethod, int sequenceChannel = 0)
        {
            var sendList = new List<NetConnection>();
            sendList.AddRange(peers.Select(peer => peer.Connection));
            _server.SendMessage(Packet.Instance.Serialize(packetCode, packetData, _server.CreateMessage()), sendList, deliveryMethod, sequenceChannel);
        }
        
        #endregion

        #region Peer Functions
        //protected BasePeer CreatePeer(NetConnection connection)
        //{
        //    return new GamePeer(connection);
        //}
        protected T CreatePeer<T>(NetConnection connection) where T : BasePeer
        {
            //return new T(connection);
            return (T)Activator.CreateInstance(typeof(T), connection);
        }
        public void Kick(BasePeer peer, string reason)
        {
            peer.Connection.Disconnect(reason);
        }
        #endregion

        #region Static Functions
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        #endregion

        public override string ToString()
        {
            return string.Format("{0}@{1}:{2}", defaultName, _endPoint.Address.ToString(), _endPoint.Port.ToString());
        }


    }
}
