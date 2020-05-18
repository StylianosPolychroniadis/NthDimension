using Lidgren.Network;
using NthDimension.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Server.Api
{
    public abstract class BasePeer
    {
        #region Fields
        private RSACryptoServiceProvider _cryptoServiceProvider;

        #endregion

        #region Properties
        public NetConnection Connection { get; private set; }
        public bool EncryptionAvailable { get; private set; }
        #endregion

        #region Ctor
        protected BasePeer(NetConnection connection)
        {
            Connection = connection;
            EncryptionAvailable = false;
        }
        #endregion



        internal string EstablishEncryption()
        {
            if (EncryptionAvailable)
            {
                Log.Warn("Encryption already available!");
                return null;
            }

            EncryptionAvailable = true;
            _cryptoServiceProvider = new RSACryptoServiceProvider(1024) { PersistKeyInCsp = false };
            return _cryptoServiceProvider.ToXmlString(false);
        }

        public byte[] Decrypt(byte[] data)
        {
            if (EncryptionAvailable) return _cryptoServiceProvider.Decrypt(data, true);
            Log.Error("Cannot decrypt if encryption is not available!");
            return new byte[0];
        }

        public void Kick()
        {
            Kick("Kicked from server.");
        }
        public void Kick(string reason)
        {
            Connection.Disconnect(reason);
        }

        public abstract void OnConnect();
        public abstract void OnDisconnect(string reason);
    }
}
