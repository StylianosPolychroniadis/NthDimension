using System.Data;
using NthDimension.Network;

namespace NthDimension.Server
{
    public class NDatabase
    {
        private Database.RegisteredScenes           m_regScenes;
        private Database.RegisteredUsers            m_regUsers;
        private Database.RegisteredAvatars          m_regAvatars;
        private Database.OnlineScenes               m_onlineScenes;
        private Database.OnlineUsers                m_onlineUsers;

        public NDatabase()
        {
            this.m_regScenes            = new Database.RegisteredScenes();
            this.m_regUsers             = new Database.RegisteredUsers();
            this.m_regAvatars           = new Database.RegisteredAvatars();
            this.m_onlineScenes         = new Database.OnlineScenes();
            this.m_onlineUsers          = new Database.OnlineUsers();
        }

        public void UpdateUser(UserInfoDesc user, UserInfoDesc.UserStatus status)
        {

        }
        public void UpdateAvatar(AvatarInfoDesc avatar, AvatarInfoDesc.AvatarStates states)
        {

        }
    }
}
