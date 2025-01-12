using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
#if UNITY_WEBGL && !UNITY_EDITOR
namespace ServerSDK.Network
{
    /// <summary>
    /// Class providing static access methods to work with JSLIB WebSocket
    /// </summary>
    internal static class WebSocketManagerJS
    {
        /* Map of websocket instances */
        private static Dictionary<int, WebSocketJS> sockets = new Dictionary<int, WebSocketJS>();

        /* Delegates */
        public delegate void OnOpenCallback(int instanceId);
        public delegate void OnMessageCallback(int instanceId, IntPtr msgPtr, int msgSize);
        public delegate void OnMessageStrCallback(int instanceId, IntPtr msgStrPtr);
        public delegate void OnErrorCallback(int instanceId, IntPtr errorPtr);
        public delegate void OnCloseCallback(int instanceId, int closeCode, IntPtr reasonPtr);

        /* WebSocket JSLIB functions */
        [DllImport("__Internal")]
        public static extern int WebSocketConnect(int instanceId);

        [DllImport("__Internal")]
        public static extern int WebSocketClose(int instanceId, int code, string reason);

        [DllImport("__Internal")]
        public static extern int WebSocketSend(int instanceId, byte[] dataPtr, int dataLength);

        [DllImport("__Internal")]
        public static extern int WebSocketSendStr(int instanceId, string data);

        [DllImport("__Internal")]
        public static extern int WebSocketGetState(int instanceId);

        /* WebSocket JSLIB callback setters and other functions */
        [DllImport("__Internal")]
        public static extern int WebSocketAllocate(string url, string binaryType);

        [DllImport("__Internal")]
        public static extern int WebSocketAddSubProtocol(int instanceId, string protocol);

        [DllImport("__Internal")]
        public static extern void WebSocketFree(int instanceId);

        [DllImport("__Internal")]
        public static extern void WebSocketSetOnOpen(OnOpenCallback callback);

        [DllImport("__Internal")]
        public static extern void WebSocketSetOnMessage(OnMessageCallback callback);

        [DllImport("__Internal")]
        public static extern void WebSocketSetOnMessageStr(OnMessageStrCallback callback);

        [DllImport("__Internal")]
        public static extern void WebSocketSetOnError(OnErrorCallback callback);

        [DllImport("__Internal")]
        public static extern void WebSocketSetOnClose(OnCloseCallback callback);

        /* If callbacks was initialized and set */
        private static bool isInitialized = false;

        /* Initialize WebSocket callbacks to JSLIB */
        private static void Initialize()
        {
            WebSocketSetOnOpen(DelegateOnOpenEvent);
            WebSocketSetOnMessage(DelegateOnMessageEvent);
            WebSocketSetOnMessageStr(DelegateOnMessageStrEvent);
            WebSocketSetOnError(DelegateOnErrorEvent);
            WebSocketSetOnClose(DelegateOnCloseEvent);

            isInitialized = true;
        }

        [MonoPInvokeCallback(typeof(OnOpenCallback))]
        public static void DelegateOnOpenEvent(int instanceId)
        {
            if (sockets.TryGetValue(instanceId, out var socket))
            {
                socket.HandleOnOpen();
            }
        }

        [MonoPInvokeCallback(typeof(OnMessageCallback))]
        public static void DelegateOnMessageEvent(int instanceId, IntPtr msgPtr, int msgSize)
        {
            if (sockets.TryGetValue(instanceId, out var socket))
            {
                var bytes = new byte[msgSize];
                Marshal.Copy(msgPtr, bytes, 0, msgSize);
                socket.HandleOnMessage(bytes);
            }
        }

        [MonoPInvokeCallback(typeof(OnMessageCallback))]
        public static void DelegateOnMessageStrEvent(int instanceId, IntPtr msgStrPtr)
        {
            if (sockets.TryGetValue(instanceId, out var socket))
            {
                string msgStr = Marshal.PtrToStringAuto(msgStrPtr);
                socket.HandleOnMessageStr(msgStr);
            }
        }

        [MonoPInvokeCallback(typeof(OnErrorCallback))]
        public static void DelegateOnErrorEvent(int instanceId, IntPtr errorPtr)
        {
            if (sockets.TryGetValue(instanceId, out var socket))
            {
                string errorMsg = Marshal.PtrToStringAuto(errorPtr);
                socket.HandleOnError(errorMsg);
            }
        }

        [MonoPInvokeCallback(typeof(OnCloseCallback))]
        public static void DelegateOnCloseEvent(int instanceId, int closeCode, IntPtr reasonPtr)
        {
            if (sockets.TryGetValue(instanceId, out var socket))
            {
                string reason = Marshal.PtrToStringAuto(reasonPtr);
                socket.HandleOnClose((ushort)closeCode, reason);
            }
        }

        internal static int AllocateInstance(string address, string binaryType)
        {
            if (!isInitialized) Initialize();
            return WebSocketAllocate(address, binaryType);
        }

        internal static void Add(WebSocketJS socket)
        {
            if (!sockets.ContainsKey(socket.instanceId))
            {
                sockets.Add(socket.instanceId, socket);
            }
        }

        internal static void Remove(int instanceId)
        {
            if (sockets.ContainsKey(instanceId))
            {
                sockets.Remove(instanceId);
            }
        }
    }
}
#endif