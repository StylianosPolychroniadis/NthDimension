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

namespace NthDimension.Server.PacketHandlers
{
    public class AuthenticationRequestPacketHandler : PacketHandler
    {
        protected NServer _server;

        public AuthenticationRequestPacketHandler(NServer server) : base(server)
        {
            _server = server;
        }

        public override void Process(Object packetData, BasePeer peer)
        {
            //BUG:: If no user attachemnt info in table, Server is Crashing


            var authPacket = (PacketAuthenticationRequest)packetData;
            var gamePeer = (NClient)peer;

            // TODO Encrypt in Client, Decrypt HERE

            var encryptedName = authPacket.Name;
            //Log.Info("Encrypted Name: {0}", encryptedName);
            //var decryptedName = Encoding.UTF8.GetString(peer.Decrypt(Convert.FromBase64String(encryptedName)));
            //Log.Info("Decrypted Name: {0}", decryptedName);

            var encryptedPassword = authPacket.Password;
            //var decryptedPassword = Encoding.UTF8.GetString(peer.Decrypt(Convert.FromBase64String(encryptedPassword)));

            //var usermanager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new IdentityDbContext(_server.GetConnectionString())));  // TODO:: Decrypt
            //var auth = usermanager.FindAsync(encryptedName, encryptedPassword);

            //if (null != auth)
            //{
            //    string userId = User.GetUserId(encryptedName);

            //    if (userId == string.Empty)
            //    {
            //        Log.Warn(string.Format("User: {0} failed to Authorize - Attempt n/n", encryptedName));

            //        Server.SendPacket(PacketCode.AuthenticationFailed, new PacketAuthorizationFailed(encryptedName, "User not activated"), gamePeer, NetDeliveryMethod.ReliableOrdered);

            //        return;
            //    }

            //    gamePeer.Authenticate(encryptedName, userId);           // TODO:: DECRYPT
            //                                                            //Server.SendPacket(PacketCode.AuthenticationConfirmation, new PacketAuthenticationConfirmed(userId, Program.ExternalIP), gamePeer, NetDeliveryMethod.ReliableOrdered);

            //    // TODO:: Send Cookies Along
            //    Server.SendPacket(PacketCode.AuthenticationConfirmation, new PacketAuthenticationConfirmed(userId, Program.WebsiteUrl), gamePeer, NetDeliveryMethod.ReliableOrdered);

            //    // TODO:: Cache file 

            //    AvatarInfoDesc avatarInfo = Avatar.GetUserAvatarInfo(userId);

            //    string attString = Environment.NewLine;


            //    for (int att = 0; att < avatarInfo.Attachments.Count; att++)
            //        attString += string.Format("Attachment {0} Name: {1} Mesh: {2} Material: {3} Matrix: {4} Vertex: {5} Offset: {6} Size: {7} Orientation: {8}{9}", att,
            //                                                        avatarInfo.Attachments[att].Name,
            //                                                        avatarInfo.Attachments[att].Mesh,
            //                                                        avatarInfo.Attachments[att].Material,
            //                                                        avatarInfo.Attachments[att].Matrix,
            //                                                        avatarInfo.Attachments[att].Vertex,
            //                                                        avatarInfo.Attachments[att].Offset,
            //                                                        avatarInfo.Attachments[att].Size,
            //                                                        avatarInfo.Attachments[att].Orientation,
            //                                                        Environment.NewLine);


            //    Server.SendPacket(PacketCode.AvatarConfigurationSubmit, new PacketAvatarConfiguration(avatarInfo), gamePeer, NetDeliveryMethod.ReliableOrdered);
            //    //Console.WriteLine(string.Format("Name: {0} | Mesh: {1} | Material: {2} | Size: {3} | Rotation: {4} | Spawn: {5} Attachments {6}", 
            //    //                                                            avatarInfo.AvatarName,
            //    //                                                            avatarInfo.BodyMesh,
            //    //                                                            avatarInfo.BodyMaterial,
            //    //                                                            avatarInfo.BodySize,
            //    //                                                            avatarInfo.BodyRotation,
            //    //                                                            avatarInfo.SpawnAt,
            //    //                                                            attString));


            //    Log.Info(String.Format("Setting user {0} online", userId));
            //    SQL.Updates.User.SetUserOnline(userId, true);



            //}

#if MOCK_SQL
            // TODO:: Collect User Info from SQL Database
            AvatarInfoDesc userFromSQL = new AvatarInfoDesc(Guid.NewGuid().ToString(), 
                                                                                    ,  //RandomColor
                                                                                    , "faceMesh.obj"
                                                                                    ,  //userImage
                                                                                    , new Vector3(0f, 0f, 0f));

            PacketRemotePlayerEnter.Event pck = new PacketRemotePlayerEnter.Event(userFromSQL);

            _server.BroadcastPacket(PacketCode.RemotePlayerEnter, pck, NetDeliveryMethod.ReliableSequenced, ChannelCode.ChannelAvatarData);
#endif
        }
    }
}
