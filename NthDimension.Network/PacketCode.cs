using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Network
{
    public struct PacketCode
    {
        // Framework 0 - 255
        public const ushort EstablishEncryptionRequest = 0;
        public const ushort EstablishEncryptionResponse = 1;

        // Logic 256+
        public const ushort AuthenticationRequest = 259;
        public const ushort AuthenticationConfirmation = 359;
        public const ushort AverageRoundTripTime = 260;

        public const ushort AuthenticationFailed = 360;
        public const ushort FileHashRequest = 1021;
        public const ushort FileHashEvent = 1022;
        public const ushort FileUpload = 1050;
        public const ushort FileUploadAcknowledge = 1050;
        public const ushort FileDownload = 1070;
        public const ushort FileDownloadAcknowledge = 1071;
        public const ushort FileInSync = 1100;
        public const ushort RemotePlayerEnter = 2001;
        public const ushort RemotePlayerLeave = 1998;
        public const ushort RemotePlayerManifestRequest = 2002;

        public const ushort AvatarConfigurationSubmit = 3001;
        public const ushort AvatarInGame = 3002;

        public const ushort AvatarTransformation = 4271;
        public const ushort AvatarAnimation = 4281;

        public const ushort ChatEnter = 5253;
        public const ushort ChatLeave = 5254;
        public const ushort ChatUsersManifestRequest = 5255;
        public const ushort ChatUsersResponse = 5256;

        public const ushort ChatMessageEvent_SignalR = 5258;
        public const ushort ChatMessageEvent_Client = 5259;

        public const ushort UserDetailRequest = 6019;
        public const ushort UserDetailResponse = 6020;

        public const ushort WallMessagesRequest = 6100;
        public const ushort WallMessagesResponse = 6210;
        public const ushort WallMessagesResponseEnd = 6211;
        public const ushort WallMessageLikesCountRequest = 6301;     // Modified Jun-11 S.P.
        public const ushort WallMessageLikesCountResponse = 6302;     // Modified Jun-11 S.P.
        public const ushort WallMessageLikeRequest = 6303;     // Added Jun-11 S.P.
        public const ushort WallMessageUnLikeRequest = 6304;     // Added Jun-12 S.P.
        public const ushort WallMessageLikeAckRequest = 6305;     // Added Jun-13 S.P.
        public const ushort WallMessageLikeAckResponse = 6306;     // Added Jun-13 S.P.

        public const ushort WallMessageCommentRequest = 6503;
        public const ushort WallMessageCommentResponse = 6604;
        public const ushort WallMessageCommentLikeRequest = 6705;
        public const ushort WallMessageCommentLikeResponse = 6806;
        public const ushort WallMessageDeleteRequest = 6807;


        public const ushort FriendsListRequest = 6901;
        public const ushort FriendsListResponse = 6902;

        public const ushort FriendRequest = 7000;
        public const ushort FriendshipResponse = 7100;
        public const ushort FriendshipAccept = 7200;

        public const ushort PhotosListRequest = 8000;
        public const ushort PhotosListResponse = 8100;

        // Server Management
        public const ushort UserGraphRequest = 59279;
        public const ushort UserGraphResponse = 59282;

    }
}
