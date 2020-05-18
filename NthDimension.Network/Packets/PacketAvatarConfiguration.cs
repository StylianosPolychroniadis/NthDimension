using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Network.Packets
{
    public class PacketAvatarConfiguration
    {
        public readonly AvatarInfoDesc Avatar;

        public PacketAvatarConfiguration(AvatarInfoDesc avatar)
        {
            this.Avatar = avatar;
        }

    }
}
