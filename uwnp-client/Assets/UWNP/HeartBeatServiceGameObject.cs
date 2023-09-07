﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using WebSocket4Net;
using ServerSDK.Network;
using UJNet;

namespace UWNP
{
    public class HeartBeatServiceGameObject : MonoBehaviour
    {
        public Action OnServerTimeout;
        private IWebSocket socket;
        public float interval = 5;

        public long lastReceiveHeartbeatTime;

        void Start()
        {
            
        }

        static DateTime dt = new DateTime(1970, 1, 1);
        public static long GetTimestamp()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - dt;
            return (long)ts.TotalSeconds;
        }

        public float t;

        void Update()
        {
            t += Time.deltaTime;
            if (t > interval)
            {
                CheckAndSendHearbeat();
                t = 0;
            }
        }

        private void CheckAndSendHearbeat()
        {
            //檢查最後一次取得心跳包的時間是否小於客戶端心跳間隔時間
            long curTime = GetTimestamp();
            long intervalSec = curTime - lastReceiveHeartbeatTime;
            if (intervalSec > interval)
            {
                Debug.Log(string.Format("XXXX CheckAndSendHearbeat：s1:{0} l:{1} s:{2}", curTime, lastReceiveHeartbeatTime, intervalSec));
                this.enabled = false;
                OnServerTimeout?.Invoke();
            }
            else
            {
                //Debug.Log(string.Format(" CheckAndSendHearbeat：s1:{0} l:{1} s:{2}", curTime, lastReceiveHeartbeatTime, intervalSec));
                this.enabled = true;
                SendHeartbeatPack();
            }
        }

        public void HitHole()
        {
            Debug.LogWarning("HeartBeat HitHole");
            lastReceiveHeartbeatTime = GetTimestamp();
        }

        private void SendHeartbeatPack()
        {
            Debug.Log("AAA: SendHeartbeatPack");
            //lastSendHeartbeatPackTime = DateTime.Now;
            byte[] package = PackageProtocol.Encode(
                PackageType.HEARTBEAT);

            ByteBuffer buffer = ByteBuffer.Allocate(package.Length + 4);
            buffer.PutInt(package.Length);
            buffer.Put(package, true);
            byte[] sendBytes = buffer.array();

            socket.SendAsync(sendBytes);//*/
        }

        internal void Setup(uint interval, Action onServerTimeout, IWebSocket socket)
        {
            Debug.Log("interval: "+interval);
            this.socket = socket;
            this.interval = (interval / 1000 )/2;
            this.OnServerTimeout = onServerTimeout;
            this.enabled = true;
            SendHeartbeatPack();
        }

        internal void ResetTimeout(uint interval)
        {
            this.enabled = true;
            this.interval = (interval / 1000) / 2;
            t = 0;
            //long s1 = GetTimestamp();
            //long s = (s1 - lastReceiveHeartbeatTime);
            //Debug.Log(string.Format("ResetTimeout： s1:{0} l:{1} s:{2} s > interval:{3}", s1, lastReceiveHeartbeatTime, s, s > interval));
            lastReceiveHeartbeatTime = GetTimestamp();
            SendHeartbeatPack();
        }

        internal void Stop()
        {
            this.enabled = false;
            t = 0;
        }
    }
}