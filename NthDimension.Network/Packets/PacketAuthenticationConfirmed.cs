using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Network.Packets
{
    public class PacketAuthenticationConfirmed
    {
        public readonly string UserId;
        public readonly string WebsiteUrl;
        //public readonly string FirstName;
        //public readonly string LastName;

        public PacketAuthenticationConfirmed(string userId/*, string first, string last*/, string websiteUrl)
        {
            this.UserId = userId;
            //this.FirstName = first;
            //this.LastName = last;
            this.WebsiteUrl = websiteUrl;
        }
    }
}
