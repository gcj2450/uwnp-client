using System;
using System.Collections;
#if UNITY_WEBGL && !UNITY_EDITOR

namespace ServerSDK.Network
{
    public class WebSocketJS : IWebSocket
    {
        public string Address { get; private set; }
        public string[] SubProtocols { get; private set; }
        public WebSocketState ReadyState { get { return (WebSocketState)WebSocketManagerJS.WebSocketGetState(instanceId); } }
        public string BinaryType { get; set; } = "arraybuffer";

        public event EventHandler<OpenEventArgs> OnOpen;
        public event EventHandler<CloseEventArgs> OnClose;
        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler<MessageEventArgs> OnMessage;

        private EnumertorConnect enumConnect;

        internal int instanceId = 0;

        public void Update()
        {

        }

        public WebSocketJS(string address)
        {
            this.Address = address;
            AllocateInstance();
        }

        public WebSocketJS(string address, string subProtocol)
        {
            this.Address = address;
            this.SubProtocols = new string[] { subProtocol };
            AllocateInstance();
        }

        public WebSocketJS(string address, string[] subProtocols)
        {
            this.Address = address;
            this.SubProtocols = subProtocols;
            AllocateInstance();
        }

        internal void AllocateInstance()
        {
            instanceId = WebSocketManagerJS.AllocateInstance(this.Address, this.BinaryType);
            Log($"Allocate socket with instanceId: {instanceId}");
            if (this.SubProtocols == null) return;
            foreach (var protocol in this.SubProtocols)
            {
                if (string.IsNullOrEmpty(protocol)) continue;
                Log($"Add Sub Protocol {protocol}, with instanceId: {instanceId}");
                int code = WebSocketManagerJS.WebSocketAddSubProtocol(instanceId, protocol);
                if (code < 0)
                {
                    HandleOnError(GetErrorMessageFromCode(code));
                    break;
                }
            }
        }

        ~WebSocketJS()
        {
            Log($"Free socket with instanceId: {instanceId}");
            WebSocketManagerJS.WebSocketFree(instanceId);
        }

        public IEnumerator ConnectSync()
        {
            Log($"Connect with instanceId: {instanceId}");
            WebSocketManagerJS.Add(this);
            int code = WebSocketManagerJS.WebSocketConnect(instanceId);
            if (code < 0) HandleOnError(GetErrorMessageFromCode(code));
            enumConnect = new();
            yield return enumConnect;
        }

        public void ConnectAsync()
        {
            Log($"Connect with instanceId: {instanceId}");
            WebSocketManagerJS.Add(this);
            int code = WebSocketManagerJS.WebSocketConnect(instanceId);
            if (code < 0) HandleOnError(GetErrorMessageFromCode(code));
        }

        public void CloseAsync()
        {
            Log($"Close with instanceId: {instanceId}");
            int code = WebSocketManagerJS.WebSocketClose(instanceId, (int)CloseStatusCode.Normal, "Normal Closure");
            if (code < 0) HandleOnError(GetErrorMessageFromCode(code));
        }

        public void SendAsync(string text)
        {
            Log($"Send, type: {Opcode.Text}, size: {text.Length}");
            int code = WebSocketManagerJS.WebSocketSendStr(instanceId, text);
            if (code < 0) HandleOnError(GetErrorMessageFromCode(code));
        }

        public void SendAsync(byte[] data)
        {
            Log($"Send, type: {Opcode.Binary}, size: {data.Length}");
            int code = WebSocketManagerJS.WebSocketSend(instanceId, data, data.Length);
            if (code < 0) HandleOnError(GetErrorMessageFromCode(code));
        }

        internal void HandleOnOpen()
        {
            Log("OnOpen");
            OnOpen?.Invoke(this, new OpenEventArgs());
            if (enumConnect != null)
            {
                enumConnect.IsConnected = true;
            }
        }

        internal void HandleOnMessage(byte[] rawData)
        {
            Log($"OnMessage, type: {Opcode.Binary}, size: {rawData.Length}");
            OnMessage?.Invoke(this, new MessageEventArgs(Opcode.Binary, rawData));
        }

        internal void HandleOnMessageStr(string data)
        {
            Log($"OnMessage, type: {Opcode.Text}, size: {data.Length}");
            OnMessage?.Invoke(this, new MessageEventArgs(Opcode.Text, data));
        }

        internal void HandleOnClose(ushort code, string reason)
        {
            Log($"OnClose, code: {code}, reason: {reason}");
            OnClose?.Invoke(this, new CloseEventArgs(code, reason));
            WebSocketManagerJS.Remove(instanceId);
        }

        internal void HandleOnError(string msg)
        {
            Log("OnError, error: " + msg);
            OnError?.Invoke(this, new ErrorEventArgs(msg));
        }

        internal static string GetErrorMessageFromCode(int errorCode)
        {
            switch (errorCode)
            {
                case -1: return "WebSocket instance not found.";
                case -2: return "WebSocket is already connected or in connecting state.";
                case -3: return "WebSocket is not connected.";
                case -4: return "WebSocket is already closing.";
                case -5: return "WebSocket is already closed.";
                case -6: return "WebSocket is not in open state.";
                case -7: return "Cannot close WebSocket. An invalid code was specified or reason is too long.";
                default: return $"Unknown error code {errorCode}.";
            }
        }

        [System.Diagnostics.Conditional("UNITY_WEB_SOCKET_LOG")]
        static void Log(string msg)
        {
        }
    }
}
#endif
