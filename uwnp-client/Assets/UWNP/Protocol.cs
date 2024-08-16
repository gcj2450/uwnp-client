using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ServerSDK.Network;
using System.Data;
using UJNet.Data;
using System.Net.Sockets;
using UJNet;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
//using WebSocket4Net;

namespace UWNP
{
    public class Protocol
    {
        Dictionary<string, Action<Package>> packAction = new Dictionary<string, Action<Package>>();
        UniTaskCompletionSource<bool> handshakeTcs;
        Dictionary<uint, UniTaskCompletionSource<Package>> packTcs = new Dictionary<uint, UniTaskCompletionSource<Package>>();
        IWebSocket socket;
        public HeartBeatServiceGameObject heartBeatServiceGo;
        public Action OnReconected;
        public Action<string> OnError;

        ByteBuffer HBbuffer;
        public Protocol()
        {
            HBbuffer = new ByteBuffer();
        }

        public void SetSocket(IWebSocket socket)
        {
            this.socket = socket;
        }

        /// <summary>
        /// 发送Protobuf的Package数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UniTask<bool> HandShakeAsync(string token)
        {
            Debug.Log($"HandShakeAsync:{token}");
            handshakeTcs = new UniTaskCompletionSource<bool>();

            byte[] package = PackageProtocol.Encode<HandShake>(
                PackageType.HANDSHAKE,
                0,
                "SystemController.handShake",
                new HandShake() { token = token });

            ByteBuffer buffer = ByteBuffer.Allocate(package.Length + 4);
            buffer.PutInt(package.Length);
            buffer.Put(package, true);
            byte[] sendBytes = buffer.array();

            if (socket.ReadyState == WebSocketState.Open)
                socket.SendAsync(sendBytes);
            else
                Debug.Log("no connect");
            return handshakeTcs.Task;
        }

        /// <summary>
        /// 发送UJObject结构数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UniTask<bool> SendUJObjectAsync(string token)
        {
            handshakeTcs = new UniTaskCompletionSource<bool>();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            SFSObject uJObject = SFSObject.NewInstance();
            int userId = UnityEngine.Random.Range(0, 100000);
            uJObject.PutLong("id", userId);
            uJObject.PutUtfString("userName", userId.ToString());
            uJObject.PutUtfString("pwd", "654321");

            SFSObject param = SFSObject.NewInstance();
            param.PutInt("c", 800);
            param.PutSFSObject("p", uJObject);
            byte[] data = param.ToBinary().Bytes;
            ByteBuffer buffer = ByteBuffer.Allocate(data.Length + 4);
            buffer.PutInt(data.Length);
            buffer.Put(data, true);
            byte[] sendBytes = buffer.array();
            sw.Stop();
            Debug.Log("usedtime: " + sw.ElapsedMilliseconds);
            if (socket.ReadyState == WebSocketState.Open)
                socket.SendAsync(sendBytes);
            else
                Debug.Log("no connect");
            return handshakeTcs.Task;
        }

        public void SendViaSocket(int cmd, SFSObject obj)
        {
            if (socket.ReadyState != WebSocketState.Open)
            {
                Debug.Log("WriteToSocket: Not Connected.");
                return;
            }

            try
            {
                SFSObject param = SFSObject.NewInstance();
                param.PutInt("c", cmd);
                param.PutSFSObject("p", obj);
                //==============自定义修改=====================
                //            if (ScopeHolder.attr.ContainsKey(Const.SCOPE_ACC_ID)) {
                //	param.PutLong("acid", long.Parse(ScopeHolder.attr[Const.SCOPE_ACC_ID].ToString()));
                //}
                //==============自定义修改=====================
                byte[] data = param.ToBinary().Bytes;
                ByteBuffer buffer = ByteBuffer.Allocate(data.Length + 4);
                buffer.PutInt(data.Length);
                buffer.Put(data, true);

                byte[] sendBytes = buffer.array();
                if (socket.ReadyState == WebSocketState.Open)
                    socket.SendAsync(sendBytes);
                else
                    Debug.Log("no connect");
            }
            catch (NullReferenceException e)
            {
                Debug.Log(e.Message);
            }
            catch (SocketException e)
            {
                Debug.Log(e.Message);
            }
        }


        internal void Notify<T>(string route, T info)
        {
            byte[] packBuff = PackageProtocol.Encode<T>(
                PackageType.NOTIFY,
                0,
                route,
                info);

            ByteBuffer buffer = ByteBuffer.Allocate(packBuff.Length + 4);
            buffer.PutInt(packBuff.Length);
            buffer.Put(packBuff, true);
            byte[] sendBytes = buffer.array();

            socket.SendAsync(sendBytes);
        }

        public UniTask<Package> RequestAsync<T>(uint packID, string route, T info = default, string modelName = null)
        {
            lock (packTcs)
            {
                UniTaskCompletionSource<Package> pack = new UniTaskCompletionSource<Package>();
                byte[] packBuff = PackageProtocol.Encode<T>(
                PackageType.REQUEST,
                packID,
                route,
                info,
                modelName);
           
                packTcs.Add(packID, pack);

                ByteBuffer buffer = ByteBuffer.Allocate(packBuff.Length + 4);
                buffer.PutInt(packBuff.Length);
                buffer.Put(packBuff, true);
                byte[] sendBytes = buffer.array();
                Debug.Log(socket == null);
                socket.SendAsync(sendBytes);
                return pack.Task;
            }
        }

        public void CanceledAllUTcs() {
            lock (packTcs)
            {
                foreach (var tcs in packTcs)
                {
                    tcs.Value.TrySetCanceled();
                }
                packTcs.Clear();
                handshakeTcs.TrySetCanceled();
            }
        }

        /// <summary>
        /// 解析服务端发来的Protobuf Package数据
        /// </summary>
        /// <param name="bytes"></param>
        public async void OnReceive(byte[] bytes)
        {
            //DecodeNoLengthPackage(bytes);

            DecodePackageData(ref HBbuffer, bytes, bytes.Length);
            //DecodeUJObjectData(ref HBbuffer, bytes, bytes.Length);
            await UniTask.SwitchToMainThread();

        }


        /// <summary>
        /// 解析不带长度头的Package数据,Nodejs服务端使用这个
        /// </summary>
        /// <param name="bytes"></param>
        public void DecodeNoLengthPackage(byte[] bytes)
        {
            try
            {
                Package package = PackageProtocol.Decode(bytes);

                Debug.Log("DecodeNoLengthPackage: "+package.route);

                switch ((PackageType)package.packageType)
                {
                    case PackageType.HEARTBEAT:
                        Debug.LogWarning("get HEARTBEAT");
                        //heartBeatServiceGo.HitHole();
                        break;
                    case PackageType.RESPONSE:
                        ResponseHandler(package);
                        break;
                    case PackageType.PUSH:
                        PushHandler(package);
                        break;
                    case PackageType.HANDSHAKE:
                        HandshakeHandler(package);
                        break;
                    case PackageType.KICK:

                        //HandleKick(package);
                        break;
                    case PackageType.NOTIFY:
                        Debug.Log($"ddddddddddddd:{package.route}");
                        NotifyHandler(package);
                        break;
                    case PackageType.ERROR:
                        ErrorHandler(package);
                        break;
                    default:
                        Debug.Log("No match packageType::" + package.packageType);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 解析带长度头的Protobuf的package数据
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <param name="readBytes"></param>
        /// <param name="bytesRead"></param>
        void DecodePackageData(ref ByteBuffer byteBuffer, byte[] readBytes, long bytesRead)
        {
            byte[] recvBytes = new byte[bytesRead];
            System.Buffer.BlockCopy(readBytes, 0, recvBytes, 0, recvBytes.Length);
            byteBuffer.Put(recvBytes, true);

            // parse HBObject
            while (true)
            {
                // check if length complete
                if (byteBuffer.Length() < 4)
                {
                    break;
                }

                // check if data complete
                byteBuffer.Flip();
                int dataLen = byteBuffer.GetInt();
                if (byteBuffer.Length() - 4 < dataLen)
                {
                    byteBuffer.Position(byteBuffer.Length());
                    break;
                }

                // get ujobj bytes
                byte[] packageBin = new byte[dataLen];
                byteBuffer.Get(packageBin, 0, dataLen);

                HandlePacket(packageBin);

                //如果是SFSObject协议从这里打开就行
                //SFSObject ujObj = SFSObject.NewFromBinaryData(new NetCoreServer.UJNet_Framework.UJNet.Data.ByteArray(ujObjBin));
                ////Handle Commands
                //HandleCommands(ujObj);

                // check bytes left  
                int leftLen = Convert.ToInt32(byteBuffer.Length() - byteBuffer.Position());
                if (leftLen == 0)
                {
                    byteBuffer = new ByteBuffer();
                    break;
                }

                // hold left data for next parse
                byte[] leftBytes = new byte[leftLen];
                byteBuffer.Get(leftBytes, 0, leftLen);
                byteBuffer = new ByteBuffer();
                byteBuffer.Put(leftBytes, true);
            }
        }

        public void HandlePacket(byte[] packageBin)
        {
            try
            {
                Package package = PackageProtocol.Decode(packageBin);

                switch ((PackageType)package.packageType)
                {
                    case PackageType.HEARTBEAT:
                        Debug.LogWarning("get HEARTBEAT");
                        heartBeatServiceGo.HitHole();
                        break;
                    case PackageType.RESPONSE:
                        ResponseHandler(package);
                        break;
                    case PackageType.PUSH:
                        PushHandler(package);
                        break;
                    case PackageType.HANDSHAKE:
                        HandshakeHandler(package);
                        break;
                    case PackageType.KICK:

                        //HandleKick(package);
                        break;
                    case PackageType.NOTIFY:
                        Debug.Log("MDDDDDDDDDDDDDD0");
                        NotifyHandler(package);
                        break;
                    case PackageType.ERROR:
                        ErrorHandler(package);
                        break;
                    default:
                        Debug.LogError("No match packageType::" + package.packageType);
                        break;
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        ///// <summary>
        ///// 解析UJObject数据结构
        ///// </summary>
        ///// <param name="byteBuffer"></param>
        ///// <param name="readBytes"></param>
        ///// <param name="bytesRead"></param>
        //void DecodeUJObjectData(ref ByteBuffer byteBuffer, byte[] readBytes, long bytesRead)
        //{
        //    byte[] recvBytes = new byte[bytesRead];
        //    System.Buffer.BlockCopy(readBytes, 0, recvBytes, 0, recvBytes.Length);
        //    byteBuffer.Put(recvBytes, true);

        //    // parse HBObject
        //    while (true)
        //    {
        //        // check if length complete
        //        if (byteBuffer.Length() < 4)
        //        {
        //            break;
        //        }

        //        // check if data complete
        //        byteBuffer.Flip();
        //        int dataLen = byteBuffer.GetInt();
        //        if (byteBuffer.Length() - 4 < dataLen)
        //        {
        //            byteBuffer.Position(byteBuffer.Length());
        //            break;
        //        }

        //        // get ujobj bytes
        //        byte[] ujObjBin = new byte[dataLen];
        //        byteBuffer.Get(ujObjBin, 0, dataLen);
        //        //Debug.Log("ujObjBin: "+ Encoding.UTF8.GetString(ujObjBin));
        //        // convert & handle event
        //        SFSObject ujObj = SFSObject.NewFromBinaryData(new NetCoreServer.UJNet_Framework.UJNet.Data.ByteArray( ujObjBin));

        //        //Handle Commands
        //        HandleCommands(ujObj);

        //        // check bytes left  
        //        int leftLen = Convert.ToInt32(byteBuffer.Length() - byteBuffer.Position());
        //        if (leftLen == 0)
        //        {
        //            byteBuffer = new ByteBuffer();
        //            break;
        //        }

        //        // hold left data for next parse
        //        byte[] leftBytes = new byte[leftLen];
        //        byteBuffer.Get(leftBytes, 0, leftLen);
        //        byteBuffer = new ByteBuffer();
        //        byteBuffer.Put(leftBytes, true);
        //    }
        //}


        public Action<int, SFSObject> OnDataReceived;
        /// <summary>
        /// 根据协议编号处理数据
        /// </summary>
        /// <param name="ujObj"></param>
        private void HandleCommands(SFSObject ujObj)
        {
            int cmd = ujObj.GetInt("c");
            SFSObject dataBin = (SFSObject)ujObj.GetSFSObject("p");
            //Debug.Log($"HandleCommands  OnDataReceived {cmd}");
            if (OnDataReceived != null)
            {
                OnDataReceived(cmd, dataBin);
            }
            if (cmd == 803)
            {
                //这里发送的是用户名和密码，进行登录，登录成功发送加入游戏消息，广播所有连接用户，有玩家加入
                HandleLogin(dataBin);
            }
            else if (cmd == 801)
            {
                //这里是接收角色位移的，所以原路把接收到的数据发送出去
                //Server.Multicast(cmd, dataBin);
            }

        }

        //处理登录信息
        private void HandleLogin(SFSObject param)
        {
            long id = param.GetLong("id");
            string user = param.GetUtfString("userName");
            string pass = param.GetUtfString("pwd");
            Debug.Log($"HandleLogin,id: {id} __userName: {user} __pass: {pass}");

        }


        public void StopHeartbeat()
        {
            if (heartBeatServiceGo != null)
            {
                Debug.Log("Stop Heartbeat");
                heartBeatServiceGo.Stop();
                //heartBeatServiceGo = null;
            }
        }

        public void SetOn(string route, Action<Package> ac)
        {
            lock (packAction)
            {
                if (!packAction.ContainsKey(route))
                {
                    packAction.Add(route,ac);
                }
            }
        }

        private void PushHandler(Package pack)
        {
            lock (packAction)
            {
                if (packAction.ContainsKey(pack.route))
                {
#if SOCKET_DEBUG
                    Debug.Log(string.Format("[Push] <<-- [{0}] {1}", pack.route, JsonUtility.ToJson(pack)));
#endif
                    packAction[pack.route]?.Invoke(pack);
                    packAction.Remove(pack.route);
                }
            }
        }

        private void ResponseHandler(Package package)
        {
            lock (packTcs)
            {
                packTcs[package.packID].TrySetResult(package);
                if (packTcs.ContainsKey(package.packID))
                {
                    packTcs.Remove(package.packID);
                }
            }
        }

        private void HandshakeHandler(Package package)
        {
            Message<Heartbeat> msg = MessageProtocol.Decode<Heartbeat>(package.buff);
            if (msg.err > 0)
            {
                Debug.Log("FFFFFFFFFFFF");
                handshakeTcs.TrySetResult(false);
                OnError?.Invoke(msg.errMsg);
                return;
            }

            if (heartBeatServiceGo == null)
            {
                GameObject go = new GameObject();
                go.name = "heartBeatServiceGo";
                heartBeatServiceGo = go.AddComponent(typeof(HeartBeatServiceGameObject)) as HeartBeatServiceGameObject;
                heartBeatServiceGo.Setup(msg.info.heartbeat, OnServerTimeout, socket);
            }
            else
            {
                OnReconected?.Invoke();
                heartBeatServiceGo.ResetTimeout(msg.info.heartbeat);
            }//*/
            handshakeTcs.TrySetResult(true);
        }

        private void NotifyHandler(Package package)
        {
            Debug.Log("NOtifyyyyyyyyyyy:"+package.route);
            TestPush info = MessageProtocol.DecodeInfo<TestPush>(package.buff);
            Debug.Log("NOtifyyyyyyyyyyy"+ info.info);
        }

        private void ErrorHandler(Package package)
        {
            Message<byte[]> msg = MessageProtocol.Decode<byte[]>(package.buff);
            Debug.LogError(string.Format("packType:{2} err:{0} msg:{1}", msg.err, msg.errMsg, package.packageType));
        }

        private void OnServerTimeout()
        {
            Debug.Log($"OnServerTimeout: {socket.ReadyState }");
            if (socket.ReadyState == WebSocketState.Connecting)
            {
                socket.CloseAsync();
            }
            if (heartBeatServiceGo != null && socket.ReadyState != WebSocketState.Connecting && socket.ReadyState != WebSocketState.Open)
            {
                heartBeatServiceGo.Stop();
            }
        }
    }
}