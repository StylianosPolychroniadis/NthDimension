using NthDimension.Network;
using System.Collections.Generic;

namespace NthDimension.Server
{
    // This file makes serves for transfering to User domain from Network Client domain
    public partial class NServer 
    {
        public Dictionary<AvatarInfoDesc, string>       OnlineUsers = new Dictionary<AvatarInfoDesc, string>();

        public bool QueryUserId(string userId)
        {
            throw new System.NotImplementedException();
        }
        public bool QueryUserName(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}
