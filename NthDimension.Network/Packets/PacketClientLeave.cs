using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Network.Packets
{
    public class PacketClientLeave
    {
        public class Event
        {
            public readonly string UserId;

            public Event(string userId)
            {
                this.UserId = userId;
            }

        }
    }
}
