using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Network
{
    public struct ChannelCode
    {
        public const int ChannelUnauthorized        = -1;
        public const int ChannelDefault             = 0;
        public const int ChannelAuthorized          = 1;
        public const int ChannelUserData            = 15;
        public const int ChannelUserStatus          = 16;
        public const int ChannelAvatarData          = 25;
        public const int ChannelAvatarStatus        = 27;
        public const int ChannelWebsiteFunctions    = 28;
        public const int ChannelFileTransfer        = 30;
    }
}
