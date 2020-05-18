using System;
using System.Threading;

using NthDimension.Network;
using NthDimension.Network.Packets;

namespace NthDimension.Server
{
    public partial class NServer
    {
        #region Singleton

        private static NServer _instance;

        public static NServer Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("Server is NULL");

                return _instance;
            }
        }

        #endregion

        public bool             ShuttingDown { get; private set; }

        internal Thread         T_listen;

        public void             RegisterPacketHandlers()
        {
            // 1/2. Framework Messages 1-255             
            Packet.Instance.Register(PacketCode.EstablishEncryptionRequest,             typeof(PacketEstablishEncryption.Request));
            Packet.Instance.Register(PacketCode.EstablishEncryptionResponse,            typeof(PacketEstablishEncryption.Response));
            // 2/2. NClient Message Types 256-65535
            Packet.Instance.Register(PacketCode.AuthenticationRequest,                  typeof(PacketAuthenticationRequest));
            Packet.Instance.Register(PacketCode.AuthenticationConfirmation,             typeof(PacketAuthenticationConfirmed));
        }
        public void             RegisterThreads()
        {
            this.T_listen               = new Thread(() => this.Listen_Thread());
            this.T_listen.IsBackground  = true;
        }
        
    }
}
