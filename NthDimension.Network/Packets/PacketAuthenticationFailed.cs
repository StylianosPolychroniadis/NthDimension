using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Network.Packets
{
    public class PacketAuthorizationFailed
    {
        public readonly string UserName;
        public readonly string Reason;

        public PacketAuthorizationFailed(string username, string reason)
        {
            this.UserName = username;
            this.Reason = reason;
        }
    }
}
