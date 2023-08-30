using System.Collections.Generic;
using UnityEngine;

namespace ServerSDK.Network
{
    internal class WebSocketManager:MonoBehaviour
    {
        private const string rootName = "[UnityWebSocket]";
        private static WebSocketManager _instance;
        public static WebSocketManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("WebSocketMgr");
                    _instance= go.AddComponent<WebSocketManager>();
                }
                return _instance;
            }
        }

        private readonly List<WebSocket> sockets = new List<WebSocket>();

        public void Add(WebSocket socket)
        {
            if (!sockets.Contains(socket))
                sockets.Add(socket);
        }

        public void Remove(WebSocket socket)
        {
            if (sockets.Contains(socket))
                sockets.Remove(socket);
        }

        public void Update()
        {
            if (sockets.Count <= 0) return;
            for (int i = sockets.Count - 1; i >= 0; i--)
            {
                sockets[i].Update();
            }
        }
    }
}