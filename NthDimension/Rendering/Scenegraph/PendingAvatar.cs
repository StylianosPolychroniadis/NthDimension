using NthDimension.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Scenegraph
{
    public class PendingAvatar
    {
        public int Index;
        public AvatarInfoDesc Avatar;
        public string AvatarLocation;

        public PendingAvatar(int index, string location, AvatarInfoDesc avatar)
        {
            this.Index = index;
            this.Avatar = avatar;
            this.AvatarLocation = location;
        }
    }
}
