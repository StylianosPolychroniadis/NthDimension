using Lidgren.Network;
using NthDimension.Network;
using NthDimension.Network.Packets;
using NthDimension.Server.Api;
using NthDimension.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Server
{
    public class NClient : BasePeer
    {
        public bool Authenticated { get; private set; }
        public string Username { get; private set; }

        public string Id { get; set; }
        public bool CacheNeedsUpdate = false;
        public NClient(Lidgren.Network.NetConnection connection) : base(connection)
        {
            Authenticated = false;
        }

        public override void OnConnect()
        {
            Log.Info("An unauthenticated user connected.");
        }

        public void Authenticate(string username, string id)
        {
            this.Authenticated = true;            // TODO:: Invole EntityFrameWork HERE
            this.Username = username;
            this.Id = id;
            Log.Info(string.Format("User: {0} width Id: {1} Authenticated and Joined", Username, Id));
        }


        // TODO: Implement SQL
        public override void OnDisconnect(string reason)
        {
            if (Authenticated)
            {
                Log.Info("Authenticated User {0} left. Reason: {1}", Username, reason);

                Guid user = new Guid();
                Guid.TryParse(this.Id, out user);

                ////var chatPacket = new PacketChat.Event(DateTime.Now, user == Guid.Empty ? Guid.Empty : user, "Server", "Player " + Username + " left.");

                ////Server.Instance.OnlineUsers = Server.Instance.OnlineUsers.Where(xid => xid.UserId != Id).ToList();
                KeyValuePair<AvatarInfoDesc, string> toBeRemoved = new KeyValuePair<AvatarInfoDesc, string>();
                toBeRemoved = NServer.Instance.OnlineUsers.FirstOrDefault(x => x.Key.UserId == Id);

                NServer.Instance.OnlineUsers.Remove(toBeRemoved.Key);

                NServer.Instance.BroadcastPacket(PacketCode.RemotePlayerLeave, new PacketClientLeave.Event(Id), NetDeliveryMethod.ReliableOrdered);

                Log.Info(string.Format("Setting User {0} offline", user));
                //SQL.Updates.User.SetUserOnline(user.ToString(), false);
            }
            else
            {
                Log.Info("An unauthenticated user disconnected.");
            }

        }
    }
}
