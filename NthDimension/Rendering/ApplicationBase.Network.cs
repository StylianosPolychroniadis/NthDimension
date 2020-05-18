namespace NthDimension.Rendering
{
    // TODO:: Move to MySociNet
    using System;
    using System.Drawing;
    using System.Net;
    using NthDimension.Algebra;
    using NthDimension.Server;

    public partial class ApplicationBase
    {
        private const int VAR_NETWORK_MAX_USERS = 32;

        private IPEndPoint mServerIpEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 35565);

        public IPEndPoint ServerIpPoint { get { return mServerIpEndPoint; } }

        public NServer Server { get; internal set; }

        /// <summary>
        /// Todo -Move outside of scope
        /// </summary>
        #region Network
        public virtual void ReceiveFile() { }

        /// <summary>
        /// Todo -Move outside of scope
        /// </summary>
        /// <param name="position"></param>
        /// <param name="positionGoto"></param>
        /// <param name="orientation"></param>
        public virtual void BroadcastTransformationChange(Vector3 position, Vector3 positionGoto, Matrix4 orientation) { }

        //public virtual void BroadcastOrientation(Matrix4 orientation) { }
        /// <summary>
        /// Todo -Move outside of scope
        /// </summary>
        /// <param name="body"></param>
        /// <param name="tshirt"></param>
        /// <param name="leggings"></param>
        /// <param name="shoes"></param>
        public virtual void BroadcastAnimationChange(string body, string tshirt, string leggings, string shoes) { }
        /// <summary>
        /// Todo -Move outside of scope
        /// </summary>
        /// <param name="location"></param>
        /// <param name="recipientUserId"></param>
        /// <param name="message"></param>
        public virtual void BroadcastChatMessage(string location, Guid recipientUserId, string message) { }

        /// <summary>
        /// Todo -Move outside of scope
        /// </summary>
        public virtual void BroadcastPlayerLeave()
        {

        }


        /// <summary>
        /// Todo -Move outside of scope
        /// </summary>
        public virtual void DisconnectClient()
        {

        }

        /// <summary>
        /// Todo -Move outside of scope
        /// </summary>
        /// <returns></returns>
        public virtual string GetUserId()
        {
            return String.Empty;
        }
        /// <summary>
        /// Todo -Move outside of scope
        /// </summary>
        /// <returns></returns>
        public virtual string GetUserName()
        {
            return String.Empty;
        }

        public virtual Bitmap GetUserProfileImage_Thumbnail(Guid userId) { return null; }
        public virtual Bitmap GetUserProfileImage(Guid userId) { return null; }
        public virtual Bitmap GetUserImage(string imgUrl) { return null; }
        public virtual Bitmap GetUserImage_Thumbnail(string imgUrl) { return null; }

        //public virtual void ReceiveChatMessage(PacketChat.EnvelopeSignalR chatEvent) { }

        //public virtual void UpdateChatUsers(PacketChatMeta.OnlineUsers onlineUsers) { }

        //public virtual void RequestUserDetails(string userId, string userName) { }

        //public virtual void ReceiveUserDetails(UserInfoDesc desc) { }

        //public virtual void EnterChat(string location) { }
        //public virtual void LeaveChat(string location) { }
        //public virtual void RequestChatUsers(string location) { }

        //public virtual void RequestWallMessages(string userId) { }
        //public virtual void ReceiveWallMessage(string[] details) { }
        //public virtual void RequestWallMessageDelete(string userId, string messageId) { }
        //public virtual void RequestWallMessagesEnded(string userId) { }

        //public virtual void RequestWallMessageComments(string wallItemId) { }
        ////public virtual void ReceiveWallMessageComment(CommentInfoDesc dec) { }

        //public virtual void RequestWallMessageLikesCount(string messageId) { }
        //public virtual void ReceiveWallMessageLikesCount(LikeInfoDesc desc) { }

        //public virtual void RequestWallMessageLike(string messageOwnerId, string messageId) { }

        //public virtual void RequestWallMessageUnlike(string messageOwnerId, string messageId) { }

        //public virtual void RequestWallMessageLikeAcknowledge(LikeInfoDesc like) { }

        //public virtual void RequestFriendsList(string userId) { }
        //public virtual void ReceiveFriendInfo(string friendUserId, string friendId, string friendFirstName, string friendLastName) { }

        //public virtual void RequestFriendshipRequests(string userId) { }
        //public virtual void ReceiveFriendshipRequests(FriendshipInfoDesc desc) { }
        //public virtual void AcceptFriendRequest(string userId, string friendUserId) { }

        //public virtual void RequestPhotosList(string userId) { }
        //public virtual void ReceivePhotoInfo(string userId, string imgUrl, string time) { }

        #endregion

    }
}
