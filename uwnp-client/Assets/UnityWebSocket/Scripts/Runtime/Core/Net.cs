using System;
using Google.Protobuf;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Zion;
using ServerSDK.Common;

namespace ServerSDK.Network
{
    //传输包头
    [StructLayout(LayoutKind.Explicit, Size = 12, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct PacketTransHeader
    {
        //包的长度
        [FieldOffset(0)]
        public uint wPacketLength;

        [FieldOffset(4)]
        public uint wPacketFlag;

        //0: 未知，1: 单包, 2:多包 3：protobu单包
        //客户端使用：2
        [FieldOffset(4)]
        public byte wPacketType;

        //编码方式
        [FieldOffset(5)]
        public byte wEncodeMode;

        //协议版本：目前为1
        [FieldOffset(6)]
        public byte wVersion;

        [FieldOffset(7)]
        public byte wResvered;

        //包的编码的原始长度（不包含本数据包头长度)
        [FieldOffset(8)]
        public uint wPacketOrgLength;

        private const int _PacketHeaderSize = 12;
        private const int _PacketFlagTypeUnknown = 0;
        private const int _PacketFlagTypeSingle = 1;
        private const int _PacketFlagTypeMultiple = 2;
        private const int _PacketFlagTypeProtobufSingle = 3;
        private const int _PacketFlagEncodeModeNone = 0;
        private const int _PacketFlagEncodeModeCompressedFast = 1;
        private const int _PacketFlagEncodeModeCompressedDef = 2;
        private const int _PacketFlagEncodeModeCompressedHigh = 3;
        private const int _PacketFlagEncodeModeEncrypt1 = 4;
        private const int _PacketFlagEncodeModeEncrypt2 = 5;
        private const int _PacketFlagEncodeModeLZOMini = 7;
        private const int _PacketFlagEncodeModeLZOSnappy = 8;
        private const int _PacketFlagEncodeModeSnappyRandCrypt = 9;
        private const int _PacketFlagEncodeModeCount = 10; // 数量
        private const int _PacketFlagVersion20050609 = 1;
        private const int _PacketFlagVersionCurrent = _PacketFlagVersion20050609;
        private const int _PacketFlagReserved = 0xA;
        public const int _PacketFlagProtobuf = _PacketFlagTypeProtobufSingle |
            (_PacketFlagEncodeModeNone << 8) |
            (_PacketFlagVersionCurrent << 16) |
            (_PacketFlagReserved << 24);

        public static byte[] ConstructToByteArray(byte[] pkg)
        {
            var newheader = new PacketTransHeader
            {
                wPacketFlag = _PacketFlagProtobuf,
                wPacketLength = (uint)(_PacketHeaderSize + pkg.Length),
                wPacketOrgLength = (uint)pkg.Length
            };
            return newheader.ToByteArray();
        }

        public static uint Size()
        {
            return _PacketHeaderSize;
        }

        public byte[] ToByteArray()
        {
            byte[] ret = new byte[12];

            ret[3] = (byte)(wPacketLength % 256);
            ret[2] = (byte)(wPacketLength >> 8 % 256);
            ret[1] = (byte)(wPacketLength >> 16 % 256);
            ret[0] = (byte)(wPacketLength >> 24 % 256);

            ret[7] = (byte)(wPacketFlag % 256);
            ret[6] = (byte)(wPacketFlag >> 8 % 256);
            ret[5] = (byte)(wPacketFlag >> 16 % 256);
            ret[4] = (byte)(wPacketFlag >> 24 % 256);

            ret[11] = (byte)(wPacketOrgLength % 256);
            ret[10] = (byte)(wPacketOrgLength >> 8 % 256);
            ret[9] = (byte)(wPacketOrgLength >> 16 % 256);
            ret[8] = (byte)(wPacketOrgLength >> 24 % 256);

            return ret;
        }

        public void ReadFromByte(byte[] source)
        {
            if (source == null || source.Length < 12)
                return;

            wPacketLength = (((uint)source[0]) << 24) + (((uint)source[1]) << 16) + (((uint)source[2]) << 8) + (uint)source[3];
            wPacketFlag = (((uint)source[4]) << 24) + (((uint)source[5]) << 16) + (((uint)source[6]) << 8) + (uint)source[7];
            wPacketOrgLength = (((uint)source[8]) << 24) + (((uint)source[9]) << 16) + (((uint)source[10]) << 8) + (uint)source[11];
        }
    }
    public class Net
    {
        public class PackageEventArgs : EventArgs
        {
            public PackageEventArgs(MsgCmd msg)
            {
                Msg = msg;
            }
            public MsgCmd Msg { get; }
        }

        public class ConnectEventArgs : EventArgs
        {
            public ConnectEventArgs()
            {

            }
        }

        private readonly byte[] recvbuf;
        private readonly byte[] sockbuf;
        private PacketTransHeader recvTcpHeader;
        private int recvStartPos;
        private int recvEndPos;
        private int recvLeftLen;
        private readonly IWebSocket clientSock;

        public event EventHandler<PackageEventArgs> OnPackage;
        public event EventHandler<ConnectEventArgs> OnConnected;
        public event EventHandler<ErrorEventArgs> OnNetError;
        public event EventHandler<CloseEventArgs> OnNetClose;

        private readonly Queue<MsgCmd> recvedMsg = new();
        private readonly Queue<MsgCmd> recvedNearbyMsg = new();     // nearbymsg单独放到一个队列中

        public int LeftMsgCount { get { return recvedMsg.Count; } }
        public int LeftNearbyMsgCount { get { return recvedNearbyMsg.Count; } }

        internal delegate void ResHandler(MsgCmd.Types.MsgCmdHeader header, ByteString msgbody);

        private struct ReqResInfo
        {
            internal MsgCmd reqMsg;
            internal ResHandler resHandler;
        }
        private readonly Dictionary<uint/*sn*/, ReqResInfo> allReqRes = new();          // 请求应答队列
        private readonly Dictionary<uint/*sn*/, EnumertorReqRes> allEnumReqRes = new(); // 请求应答队列
        private uint curSN = 0;                 // 当前同步请求的sn

        public Net(string strUrl)
        {
            // //Debug.Log("Net constructor, platform = " + Application.platform.ToString());
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                #if UNITY_WEBGL && !UNITY_EDITOR
                clientSock = new WebSocketJS(strUrl);
                #endif
            }
            else
            {
                clientSock = new WebSocket(strUrl);
            }
           
            recvbuf = new byte[2 * 1024 * 1024];
            sockbuf = new byte[1024 * 1024];
            recvTcpHeader = new PacketTransHeader();
            clientSock.OnMessage += ClientSock_OnMessage;
            clientSock.OnOpen += ClientSock_OnOpen;
            clientSock.OnClose += ClientSock_OnClose;
            clientSock.OnError += ClientSock_OnError;
        }

        public bool IsConnected()
        {
            if (clientSock == null) return false;
            return clientSock.ReadyState == WebSocketState.Open;
        }

        public void Close()
        {
            clientSock.CloseAsync();
        }

        private void ClientSock_OnError(object sender, ErrorEventArgs e)
        {
            OnNetError?.Invoke(this, new ErrorEventArgs(e.Message, e.Exception));
        }

        private void ClientSock_OnClose(object sender, CloseEventArgs e)
        {
            OnNetClose?.Invoke(this, new CloseEventArgs(e.Code, e.Reason)); 
        }

        private void ClientSock_OnOpen(object sender, OpenEventArgs e)
        {
            OnConnected?.Invoke(this, new ConnectEventArgs());
        }

        private bool IsReqResCmd(EnumCmdCode cmd)
        {
            if (cmd == EnumCmdCode.ScInteractionRes)
            {
                return true;
            }

            return false;
        }

        public MsgCmd PopMessage()
        {
            recvedMsg.TryDequeue(out MsgCmd msg);
            return msg;
        }
        public MsgCmd PopNearbyMessage()
        {
            recvedNearbyMsg.TryDequeue(out MsgCmd msg);
            return msg;
        }
        private void ClientSock_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                Performance.OnMessageCallCount++;

                // 复制到recvbuf的后面
                if (e.RawData.Length + recvEndPos > recvbuf.Length)
                {
                    Close();
                    return;
                }
                Array.Copy(e.RawData, 0, recvbuf, recvEndPos, e.RawData.Length);
                recvEndPos += e.RawData.Length;
                recvLeftLen = recvEndPos - recvStartPos;

                while (true)
                {
                    // 没有一个包头的长度
                    if (recvLeftLen < PacketTransHeader.Size())
                    {
                        break;
                    }

                    if (recvbuf == null || recvbuf.Length < recvStartPos + 12)
                        break;

                    recvTcpHeader.wPacketLength = (((uint)recvbuf[recvStartPos + 0]) << 24) + (((uint)recvbuf[recvStartPos + 1]) << 16) +
                        (((uint)recvbuf[recvStartPos + 2]) << 8) + (uint)recvbuf[recvStartPos + 3];
                    recvTcpHeader.wPacketFlag = (((uint)recvbuf[recvStartPos + 4]) << 24) + (((uint)recvbuf[recvStartPos + 5]) << 16) +
                        (((uint)recvbuf[recvStartPos + 6]) << 8) + (uint)recvbuf[recvStartPos + 7];
                    recvTcpHeader.wPacketOrgLength = (((uint)recvbuf[recvStartPos + 8]) << 24) + (((uint)recvbuf[recvStartPos + 9]) << 16) +
                        (((uint)recvbuf[recvStartPos + 10]) << 8) + (uint)recvbuf[recvStartPos + 11];

                    Performance.ReadFromByteCallCount++;

                    // recvTcpHeader.ReadFromByte(recvbuf.AsSpan<byte>()[recvStartPos..].ToArray());
                    //logger.Log("tcp header content = " +
                    //    recvTcpHeader.wPacketType + recvTcpHeader.wPacketLength + recvTcpHeader.wPacketOrgLength);
                    // 包还没收完
                    if (recvLeftLen < recvTcpHeader.wPacketLength)
                    {
                        break;
                    }
                    // 收到了一个完整的包
                    if (recvLeftLen >= recvTcpHeader.wPacketLength)
                    {
                        if (recvTcpHeader.wPacketLength != recvTcpHeader.wPacketOrgLength + PacketTransHeader.Size())
                        {
                            Debug.LogError("ClientSock_OnMessage - package len check fail.");
                            Performance.InvalidPackage++;
                            Close();
                            break;
                        }
                        if (recvTcpHeader.wPacketFlag != PacketTransHeader._PacketFlagProtobuf)
                        {
                            Debug.LogError("ClientSock_OnMessage - flag check fail.");
                            Performance.InvalidPackage++;
                            Close();
                            break;
                        }

                        var newcmd = MsgCmd.Parser.ParseFrom(recvbuf, (int)(recvStartPos + PacketTransHeader.Size()),
                            (int)(recvTcpHeader.wPacketLength - PacketTransHeader.Size()));
                        recvStartPos += (int)(recvTcpHeader.wPacketLength);
                        recvLeftLen -= (int)(recvTcpHeader.wPacketLength);
                        Performance.PackageCount++;

                        // Logger.Log("ClientSock_OnMessage - recv reqres cmd, header = " + newcmd.MsgHeader.ToString());
                        if (allReqRes.TryGetValue(newcmd.MsgHeader.Sn, out ReqResInfo info))
                        {
                            allReqRes.Remove(newcmd.MsgHeader.Sn);
                            if (info.resHandler != null)
                            {
                                info.resHandler(newcmd.MsgHeader, newcmd.MsgbyBody);
                            }
                            Performance.ProcessPackageCount++;
                        }
                        else if (allEnumReqRes.TryGetValue(newcmd.MsgHeader.Sn, out EnumertorReqRes enumerator))
                        {
                            allEnumReqRes.Remove(newcmd.MsgHeader.Sn);
                            enumerator.ResMsg = newcmd;
                            Performance.ProcessPackageCount++;
                        }
                        else
                        {
                            OnPackage?.Invoke(this, new PackageEventArgs(newcmd));
                            if (newcmd.MsgHeader.Cmd == EnumCmdCode.ScNearbyUpd) recvedNearbyMsg.Enqueue(newcmd);
                            else recvedMsg.Enqueue(newcmd);

                            if (recvedMsg.Count + recvedNearbyMsg.Count > 500)
                            {
                                // Debug.LogError("ClientSock_OnMessage - recv queue full, cur count = " + recvedMsg.Count);
                                Performance.MaxPackageQueueCount++;
                                Close();
                                // break;
                            }
                        }
                    }
                }

                // 将数据移动到开始
                if (recvStartPos > 0)
                {
                    Array.Copy(recvbuf, recvStartPos, recvbuf, 0, recvLeftLen);
                    recvStartPos = 0;
                    recvEndPos = recvStartPos + recvLeftLen;
                }
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
            return;
        }

        public void RemoveEnumReqRes(uint sn)
        {
            if (allEnumReqRes.TryGetValue(sn, out EnumertorReqRes enumerator))
            {
                if (enumerator != null)
                {
                    allEnumReqRes.Remove(sn);
                }
            }
        }

        public void UpdatePublic()
        {
            if  (clientSock != null &&
                 clientSock.ReadyState == WebSocketState.Open)
            {
                clientSock.Update();
            }
        }

        public IEnumerator ConnectSync()
        {
            if (clientSock.ReadyState == WebSocketState.Open ||
                clientSock.ReadyState == WebSocketState.Connecting)
            {
                Debug.LogError("Connect - alread connecting");
                yield break;
            }

            //Debug.Log("Connect - start ConnectAsync()");
            yield return clientSock.ConnectSync();
        }

        public void Connect()
        {
            if (clientSock.ReadyState == WebSocketState.Open ||
                clientSock.ReadyState == WebSocketState.Connecting)
            {
                Debug.LogError("Connect - alread connecting");
                return;
            }

            try
            {
                //Debug.Log("Connect - start ConnectAsync()");
                clientSock.ConnectAsync();
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
            finally
            {

            }
        }

        /////////////////////////////////// 包封装函数 ///////////////////////////////////

        // 校验登录token，发往queue server
        public void VerifyLoginToken(string loginToken)
        {
            try
            {
                //Debug.Log("[login] verify login token");
                if (!IsConnected()) return;

                var req = new CSMsgVerifyLoginTokenReq
                {
                    StrLoginToken = loginToken
                };
                var msgBody = req.ToByteString();
                var msg = new MsgCmd
                {
                    MsgbyBody = msgBody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = EnumCmdCode.CsVerifyLoginTokenReq
                    }
                };

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
            }
            catch (Exception e)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(e.Message, e));
            }
        }

        // 校验queue token，发往proxy server
        // clientType 0-VR 1-mobile 2-PC
        public void VerifyQueueToken(string token, uint clientType)
        {
            try
            {
                if (!IsConnected()) return;

                var req = new CSMsgVerifyQueueTokenReq
                {
                    StrQueueToken = token,
                    NCliType = clientType
                };
                var msgBody = req.ToByteString();
                var msg = new MsgCmd
                {
                    MsgbyBody = msgBody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = EnumCmdCode.CsVerifyQueueTokenReq
                    }
                };

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
            }
            catch (Exception e)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(e.Message, e));
            }
        }

        public void LoginDirectly(string username, string password, ulong tenantid)
        {
            try
            {
                if (!IsConnected()) return;

                var req = new CSMsgLoginReq()
                {
                    ELoginType = CSMsgLoginReq.Types.EnumLoginType.Normal,
                    MsgNormalInfo = new CSMsgLoginReq.Types.MsgNormalInfo()
                    {
                        UserName = username,
                        PassWord = password,
                    },
                    StrHardware = username,
                    StrVersion = username,
                    CliType = 1,
                    VersionCliType = 1,
                    VersionChannelType = 1,
                    TenantSeq = tenantid,
                    TenantID = tenantid.ToString(),
                    StrCSVersion = "1",
                };

                var msgbody = req.ToByteString();
                var msg = new MsgCmd()
                {
                    MsgbyBody = msgbody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = EnumCmdCode.CsLoginReq
                    }
                };

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
        }

        public void Active()
        {
            try
            {
                if (!IsConnected()) return;

                var req = new CSMsgActiveReq();
                var msgbody = req.ToByteString();
                var msg = new MsgCmd()
                {
                    MsgbyBody = msgbody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = EnumCmdCode.CsUserActiveReq,
                    }
                };

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
        }
        public void WalkTo(ulong objid, Zion.GameObject.EnumGameObjectType objtype,
            float posx, float posy, float posz, float dirx, float diry, float dirz)
        {
            try
            {
                if (!IsConnected()) return;

                var req = new Zion.CSPlayerWalkReq()
                {
                    MsgPlayerID = new Zion.GameObject.MsgGameObjectID()
                    {
                        NID = objid,
                        EType = objtype,
                    },
                    MsgEndPos = new Zion.GameObject.MsgVarVector3()
                    {
                        X = posx,
                        Y = posy,
                        Z = posz,
                    },
                    MsgDir = new Zion.GameObject.MsgVarVector3()
                    {
                        X = dirx,
                        Y = diry,
                        Z = dirz,
                    }
                };
                var msgbody = req.ToByteString();
                var msg = new MsgCmd()
                {
                    MsgbyBody = msgbody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = EnumCmdCode.CsPlayerWalkReq,
                    }
                };

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
        }

        public bool CheckResMsg(EnumertorReqRes result, string from, out MsgCmd realresmsg)
        {
            realresmsg = null;

            if (result == null)
            {
                Debug.LogError(from + " fail, result is null");
                return false;
            }
            if (result.Current is not MsgCmd resmsg)
            {
                Debug.LogError(from + " fail, result.Current is: " + result.Current?.ToString());
                return false;
            }
            realresmsg = resmsg;
            if (resmsg.MsgHeader.ErrCode != EnumErrorCode.Success)
            {
                Debug.LogError(from + " fail, errcode = " + resmsg.MsgHeader.ErrCode.ToString());
                Debug.LogError(from + " fail, MsgBody = " + resmsg.MsgbyBody.ToStringUtf8());
                if (from.Equals("CheckUserName") && resmsg.MsgHeader.ErrCode == EnumErrorCode.NameCheckErr)
                {
                    return true;
                }
                else {
                    return false;
                }
                
            }

            return true;
        }

        public IEnumerator SyncReqRes(IMessage reqmsg, EnumCmdCode cmd, int timeout = 0)
        {
            if (!IsConnected())
            {
                Debug.LogError("SyncReqRes - net not connected. reqmsg = " + reqmsg.ToString());
                yield break;
            }

            var msgbody = reqmsg.ToByteString();
            var msg = new MsgCmd()
            {
                MsgbyBody = msgbody,
                MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                {
                    Cmd = cmd,
                }
            };

            msg.MsgHeader.Sn = ++curSN;

            var body = msg.ToByteArray();
            var header = PacketTransHeader.ConstructToByteArray(body);

            clientSock.SendAsync(header);
            clientSock.SendAsync(body);
            EnumertorReqRes enumerator;
            if (timeout == 0)
            {
                enumerator = new EnumertorReqRes();
            }
            else
            {
                enumerator = new EnumertorReqRes(this, msg.MsgHeader.Sn, timeout);
            }

            allEnumReqRes.Add(msg.MsgHeader.Sn, enumerator);

            yield return enumerator;
        }
        public void GetConnectError(out string err)
        {
            if (clientSock == null)
            {
                err = "server network  clientSock = null";
            }
            if (clientSock.ReadyState != WebSocketState.Open)
            {
                err = $"server network WebSocketState {clientSock.ReadyState}";
            }
            else
            {
                err = "";
            }
        }
        internal void AsyncReqRes(IMessage reqmsg, EnumCmdCode cmd, ResHandler reshandler)
        {
            if (!IsConnected()) return;

            try
            {
                var msgbody = reqmsg.ToByteString();
                var msg = new MsgCmd()
                {
                    MsgbyBody = msgbody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = cmd,
                    }
                };

                msg.MsgHeader.Sn = ++curSN;
                allReqRes.Add(msg.MsgHeader.Sn, new ReqResInfo() { reqMsg = msg, resHandler = reshandler });

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
        }

        public void GetRoleList()
        {
            try
            {
                if (!IsConnected()) return;

                var req = new CSRoleListReq() {};

                var msgbody = req.ToByteString();
                var msg = new MsgCmd()
                {
                    MsgbyBody = msgbody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = EnumCmdCode.CsRoleListReq
                    }
                };

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
        }

        public void CreateRole(string rolename, 
            Zion.GameObject.EnumGameObjectGender gender, ulong modelid)
        {
            try
            {
                if (!IsConnected()) return;

                var req = new CSCreateRoleReq() 
                {
                    StrName = rolename,
                    EGender = gender,
                    StaticID = modelid,
                };

                var msgbody = req.ToByteString();
                var msg = new MsgCmd()
                {
                    MsgbyBody = msgbody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = EnumCmdCode.CsCreateRoleReq,
                    }
                };

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
        }

        public void EnterWorld(ulong roleid, ulong directlySpaceID, string customParam)
        {
            try
            {
                if (!IsConnected()) return;

                var req = new CSEnterWorldReq()
                {
                    MsgSelectedPlayerID = new Zion.GameObject.MsgGameObjectID()
                    {
                        EType = Zion.GameObject.EnumGameObjectType.Player,
                        NID = roleid,
                    },
                    MsgWorldID = new Zion.GameObject.MsgGameObjectID()
                    {
                        EType = Zion.GameObject.EnumGameObjectType.World,
                        NID = 2,
                    },

                    SpaceID = directlySpaceID,
                    StrMsgEx = customParam
                };

                var msgbody = req.ToByteString();
                var msg = new MsgCmd()
                {
                    MsgbyBody = msgbody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = EnumCmdCode.CsEnterWorldReq,
                    }
                };

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
                
                //Debug.Log($"enter world req:{req}");
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
        }

        public void Logout()
        {
            try
            {
                if (!IsConnected()) return;

                var req = new CSMsgLogoutReq();

                var msgBody = req.ToByteString();
                var msg = new MsgCmd()
                {
                    MsgbyBody = msgBody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = EnumCmdCode.CsLogoutReq,
                    }
                };

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
        }

        public void Ping()
        {
            try
            {
                if (!IsConnected()) return;

                var req = new CSMsgPingReq() { };

                var msgbody = req.ToByteString();
                var msg = new MsgCmd()
                {
                    MsgbyBody = msgbody,
                    MsgHeader = new MsgCmd.Types.MsgCmdHeader()
                    {
                        Cmd = EnumCmdCode.CsUserPingReq
                    }
                };

                var body = msg.ToByteArray();
                var header = PacketTransHeader.ConstructToByteArray(body);

                clientSock.SendAsync(header);
                clientSock.SendAsync(body);
            }
            catch (Exception ex)
            {
                OnNetError?.Invoke(this, new ErrorEventArgs(ex.Message, ex));
            }
        }
    }
}
