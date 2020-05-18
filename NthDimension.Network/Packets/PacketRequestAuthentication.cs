using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Network.Packets
{
    public class PacketAuthenticationRequest
    {
        public readonly string Name;
        public readonly string Password;
        //public readonly string PlayerXml;

        public PacketAuthenticationRequest(string name, string password)
        {
            //RSACryptoServiceProvider _cryptoServiceProvider = new RSACryptoServiceProvider(1024) { PersistKeyInCsp = false };

            // TODO:: Encrypt User Credentials

            Name = name;
            Password = password;
        }
    }
}
