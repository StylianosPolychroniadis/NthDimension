using Lidgren.Network;
using NthDimension.Network;
using NthDimension.Server.Api;
using NthDimension.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Server
{
    public partial class NServer
    {
        #region LISTEN T
        private void unhandled(NetIncomingMessage inc)
        {
            Log.Warn("Unhandled message type {0}", inc.MessageType);
        }
        private void discoveryRequest(NetIncomingMessage inc)
        {
            NetOutgoingMessage response = _server.CreateMessage();
            response.Write("My server name");
            _server.SendDiscoveryResponse(response, inc.SenderEndPoint);    // Send the response to the sender of the request
        }
        private void connectionApproval(NetIncomingMessage inc)
        {
            //NetOutgoingMessage avatarInfo = new 
            inc.SenderConnection.Approve();
        }
        private void statusChanged(NetIncomingMessage inc)
        {
            var status = (NetConnectionStatus)inc.ReadByte();
            Log.Info("Status changed. Code: {0}", status);
            switch (status)
            {
                case NetConnectionStatus.Connected:
                    lock (peersLock)
                    {
                        Peers.Add(inc.SenderConnection, CreatePeer<NClient>(inc.SenderConnection));
                        Peers[inc.SenderConnection].OnConnect();

                        ////// TODO: Check if necessary
                        ////inc.SenderConnection.Tag = new StreamingClient(inc.SenderConnection, defaultCachePackage);
                        ////inc.Write(" *** THIS IS YOUR XML AVATAR INFORMATION USER ***");
                    }
                    break;
                case NetConnectionStatus.Disconnected:
                    lock (peersLock)
                    {
                        try
                        {
                            Peers[inc.SenderConnection].OnDisconnect(inc.ReadString());
                            Peers.Remove(inc.SenderConnection);
                        }
                        catch
                        {

                        }

                        // TODO:: Broadcast Client disconnected
                        //this.BroadcastPacket();
                    }
                    break;
            }
        }
        private void incomingData(NetIncomingMessage inc)
        {
            Object packetData;
            try
            {
                var packetCode = Packet.Instance.Deserialize(inc, out packetData);

                if (!_packetHandlers.ContainsKey(packetCode))
                    return;

                try
                {
                    _packetHandlers[packetCode].Process(packetData, Peers[inc.SenderConnection]);
                }
                catch (Exception pE)
                {
                    Log.Error("IncomingMessage Code {0} Process {1}\n{2} ", new string[3] { packetCode.ToString(), pE.Message, pE.StackTrace });
                }
            }
            catch (Exception pE)
            {
                Log.Warn("Invalid Incoming Data", pE.Message);
            }
        }
        private void debugMessage(NetIncomingMessage inc)
        {
            Log.Info(inc.ReadString());
        }
        private void warningMessage(NetIncomingMessage inc)
        {
            Log.Warn(inc.ReadString());
        }
        private void errorMessage(NetIncomingMessage inc)
        {
            Log.Error(inc.ReadString());
        }
        protected object Listen_Thread()
        {
            while (!ShuttingDown)
            {
                NetIncomingMessage incomingMessage;
                if ((incomingMessage = _server.ReadMessage()) != null)
                {
                    //Log.Info("Channel Code: {0}", incomingMessage.SequenceChannel);

                    switch (incomingMessage.SequenceChannel)
                    {
                        #region Channel File Transfer (30)
                        case ChannelCode.ChannelFileTransfer:

                            break;
                        #endregion

                        #region Default Channel (0)
                        case ChannelCode.ChannelUnauthorized:

                        case ChannelCode.ChannelDefault:
                        case ChannelCode.ChannelAuthorized:
                        case ChannelCode.ChannelUserData:
                        case ChannelCode.ChannelUserStatus:
                        case ChannelCode.ChannelAvatarData:
                        case ChannelCode.ChannelAvatarStatus:
                        case ChannelCode.ChannelWebsiteFunctions:
                            switch (incomingMessage.MessageType)
                            {
                                case NetIncomingMessageType.DiscoveryRequest:
                                    this.discoveryRequest(incomingMessage);
                                    break;
                                case NetIncomingMessageType.DebugMessage:
                                case NetIncomingMessageType.VerboseDebugMessage:
                                    this.debugMessage(incomingMessage);
                                    break;
                                case NetIncomingMessageType.WarningMessage:
                                    this.warningMessage(incomingMessage);
                                    break;
                                case NetIncomingMessageType.ErrorMessage:
                                case NetIncomingMessageType.Error:
                                    this.errorMessage(incomingMessage);
                                    break;
                                case NetIncomingMessageType.StatusChanged:
                                    this.statusChanged(incomingMessage);
                                    break;
                                case NetIncomingMessageType.Data:
                                    this.incomingData(incomingMessage);
                                    break;
                                case NetIncomingMessageType.ConnectionApproval:
                                    this.connectionApproval(incomingMessage);
                                    break;
                                default:
                                    this.unhandled(incomingMessage);
                                    break;
                            }
                            break;
                            #endregion
                    }
                }

                lock (peersLock)
                {
                    foreach (KeyValuePair<NetConnection, BasePeer> p in Peers)
                    {
                        if (p.Value is NClient && ((NClient)p.Value).Authenticated && ((NClient)p.Value).CacheNeedsUpdate)
                        {
                            StreamingClient client = p.Key.Tag as StreamingClient;
                            if (null != client)
                                client.Heartbeat();
                        }

                    }
                }
            }

            return null;
        }
        #endregion LISTEN T
    }
}
