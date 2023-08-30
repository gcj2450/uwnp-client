﻿using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
//using WebSocket4Net;
using System.Net;
using System.Threading;
using ServerSDK.Network;
//using SuperSocket.ClientEngine;

namespace UWNP
{
    public enum NetWorkState
    {
        CONNECTING,
        CONNECTED,
        DISCONNECTED,
        TIMEOUT,
        ERROR,
        KICK
    }

    public class Client
    {
        private static int RqID = 0;

        //public NetWorkState state;

        public Action OnReconected,OnDisconnect,OnConnected;
        public Action<string> OnError;
        public uint retry;
        Protocol protocol;
        IWebSocket socket;
        UniTaskCompletionSource<bool> utcs;
        private bool isForce;
        private string token;

        public Client(string host)
        {
            ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;//*/
            Debug.Log($"host: {host}");
            utcs = new UniTaskCompletionSource<bool>();
            //if (Application.platform == RuntimePlatform.WebGLPlayer)
#if UNITY_WEBGL && !UNITY_EDITOR
                socket = new WebSocketJS(host);
#else
                socket = new WebSocket(host);
#endif
            socket.OnMessage += OnReceived;
            socket.OnClose += OnClose;
            //EventHandler<SuperSocket.ClientEngine.ErrorEventArgs> onErr = (sender, e) =>
            //{
            //    OnError?.Invoke(e.Exception.Message);
            //    utcs.TrySetResult(false);
            //};
            socket.OnError += OnErr;
            socket.OnOpen += async (sender, e) =>
            {
                socket.OnError -= OnErr;
                socket.OnError += OnErr;

                if (protocol == null)
                    protocol = new Protocol();
                protocol.SetSocket(socket);
                protocol.OnReconected = OnReconected;
                protocol.OnError = OnError;
                bool isOK = await protocol.HandsharkAsync(this.token);
                Debug.Log("open:" + e);
                utcs.TrySetResult(isOK);
                OnConnected?.Invoke();
            };
        }

        public UniTask<bool> ConnectAsync(string token)
        {
            this.token = token;
           GameObject.FindObjectOfType<TestClient>().StartCoroutine( socket.ConnectSync());
            return utcs.Task;
        }

        private async void OnClose(object sender, EventArgs e)
        {
            if (socket.ReadyState == WebSocketState.Connecting || socket.ReadyState == WebSocketState.Open) return;
            await UniTask.SwitchToMainThread();
            Cancel();
            if (!isForce)
            {
                await UniTask.Delay(1000);
                GameObject.FindObjectOfType<TestClient>().StartCoroutine(socket.ConnectSync());
            }
            OnDisconnect?.Invoke();
        }

        public void OnErr(object sender, ErrorEventArgs e)
        {
            OnError?.Invoke(e.Exception.Message);
            utcs.TrySetResult(false);
            Debug.LogError(e.Exception.Message);
        }

        private void OnReceived(object sender, MessageEventArgs e)
        {
            protocol.OnReceive(e.RawData);
        }

        //private void OnReceived(object sender, DataReceivedEventArgs e)
        //{
        //    protocol.OnReceive(e.Data);
        //}

        public void On(string route, Action<Package> cb)
        {
            protocol.SetOn(route, cb);
        }

        public void Notify<T>(string route, T info = default)
        {
            uint rqID = (uint)Interlocked.Increment(ref RqID);
            try
            {
#if SOCKET_DEBUG
                Debug.Log(string.Format("[Notify] -->> [{0}] {1}", route, JsonUtility.ToJson(info)));
#endif
                protocol.Notify<T>(route, info);
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("[Notify Exception]{0}", e.Message));
                throw e;
            }
        }

        public async UniTask<Message<S>> RequestAsync<T,S>(string route, T info = default, string modelName = null)
        {
            Debug.Log("AAAAAAAAAAAAA");
            uint rqID = (uint)Interlocked.Increment(ref RqID);
            try
            {
#if SOCKET_DEBUG
                Debug.Log(string.Format("[{0}][Request] -->> [{1}] {2}", rqID, route, JsonUtility.ToJson(info)));
#endif
                Package pack = await protocol.RequestAsync<T>(rqID, route, info, modelName);
                Message<S> msg = MessageProtocol.Decode<S>(pack.buff);
#if SOCKET_DEBUG
                Debug.Log(string.Format("[{0}][Request] <<-- [{1}] {2} {3}", rqID, route, JsonUtility.ToJson(msg), JsonUtility.ToJson(msg.info)));
#endif
                return msg;
            }
            catch (Exception e)
            {
                //Debug.Log(string.Format("[{0}][RequestAsync Exception]{1}", rqID, e.Message));
                throw e;
            }
        }

        public void Cancel(bool isForce = false) {
            this.isForce = isForce;
            utcs.TrySetCanceled();
            if (socket.ReadyState != WebSocketState.Closed)
            {
                socket.CloseAsync();
            }
            if (protocol!=null)
            {
                protocol.StopHeartbeat();
                protocol.CanceledAllUTcs();
            }
        }

    }
}