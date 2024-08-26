using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Reflection;
using UJNet;
using UJNet.Data;
using UnityEngine;
using ServerSDK.Network;
using Unity.Jobs;
using UWNP;
using Cysharp.Threading.Tasks;
using UnityEngine.SocialPlatforms;
//已修改为支持websocket和tcp连接
[assembly: SuppressIldasmAttribute()]
//[assembly: AssemblyKeyFile("/keyfile.snk")]
namespace UJNet
{
    /// <summary>
    /// 网络连接类型
    /// </summary>
    public enum NetworkTransportType
    {
        TCP,
        UDP,
        WebSocket
    }

    public class NetClient: IDisposable {

        Protocol protocol;
        IWebSocket socket;
		public NetworkTransportType NetWorkTransType = NetworkTransportType.TCP;
		private TcpClient socketConnection;
		private NetworkStream networkStream;

		private Thread thrSocketReader;
	    private Thread thrConnect;
				
		private bool connected;
		private bool connecting = false;
		private int socketPollSleep = 0;
		private IPAddress ipAddress;
		private int port;
		
		private bool isDisposed = false;
		private bool debug = true;
		
		private object disconnectionLocker = new object(); 
		
		private Queue queuedEvents = Queue.Synchronized(new Queue());
		private System.Object queuedEventsLocker = new System.Object();
		
		private CommandHandler commandHandler;
		
		public delegate void OnHttpCloseDelegate(int locker);
		public delegate void OnHttpErrorDelegate(Exception ex, int locker);
		public delegate void OnResponseDelegate(int cmd, SFSObject ujObj);
		public delegate void OnHttpResponseDelegate(int cmd, string resp);
		public delegate void OnConnectionDelegate(bool success, string error);
		public delegate void OnConnectionLostDelegate();
		public delegate void OnDebugMessageDelegate(string message);
		
		public OnHttpCloseDelegate OnHttpClose;
		public OnHttpErrorDelegate OnHttpError;
		public OnResponseDelegate OnResponse;
		public OnHttpResponseDelegate OnHttpResponse;
		public OnConnectionDelegate OnConnection;
		public OnConnectionLostDelegate OnConnectionLost;
		public OnDebugMessageDelegate OnDebugMessage;
		
		// register event
		public void AddHttpCloseListener(OnHttpCloseDelegate del){
			this.OnHttpClose += del;
		}
		public void RemoveHttpCloseListener(OnHttpCloseDelegate del){
			this.OnHttpClose -= del;
		}
		public void AddHttpErrorListener(OnHttpErrorDelegate del){
			this.OnHttpError += del;
		}
		public void RemoveHttpErrorListener(OnHttpErrorDelegate del){
			this.OnHttpError -= del;
		}
		public void AddResponseListener(OnResponseDelegate del){
			this.OnResponse += del;
		}
		public void RemoveResponseListener(OnResponseDelegate del){
			this.OnResponse -= del;
		}
		public void AddHttpResponseListener(OnHttpResponseDelegate del){
			this.OnHttpResponse += del;
		}
		public void RemoveHttpResponseListener(OnHttpResponseDelegate del){
			this.OnHttpResponse -= del;
		}
		public void AddConnectionListener(OnConnectionDelegate del){
			this.OnConnection += del;
		}
		public void RemoveConnectionListener(OnConnectionDelegate del){
			this.OnConnection -= del;
		}
		public void AddConnectionLostListener(OnConnectionLostDelegate del){
			this.OnConnectionLost += del;
		}
		public void RemoveConnectionLostListener(OnConnectionLostDelegate del){
			this.OnConnectionLost -= del;
		}
		public void AddDebugMessageListener(OnDebugMessageDelegate del){
			this.OnDebugMessage += del;
		}
		public void RemoveDebugMessageListener(OnDebugMessageDelegate del){
			this.OnDebugMessage -= del;
		}
		
		public NetClient(bool debug, NetworkTransportType netType){
			this.debug = debug;
			this.NetWorkTransType = netType;
			commandHandler = new CommandHandler(this);
			
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.DefaultConnectionLimit = 10;
			Debug.Log("DefaultConnectionLimit// " + ServicePointManager.DefaultConnectionLimit);
		}
		public int networkPort
		{
			get
			{
				return port;
			}
		}


        ~NetClient() {
			Dispose(false);
		}
									
		// ========Via Socket=======//
		public bool Connected {

			//get { return this.connected; }
			get
			{
				switch (NetWorkTransType)
				{
					case NetworkTransportType.TCP:
                        return this.connected;
					case NetworkTransportType.UDP:
                        return this.connected;
					case NetworkTransportType.WebSocket:
                        if (socket == null) return false;
                        return socket.ReadyState == WebSocketState.Open;
					default:
						return this.connected;
				}
			}
		}

		public void SendViaSocket(int cmd, SFSObject obj) {
			if ( !connected ) {
				DebugMessage("WriteToSocket: Not Connected.");
				return;
			}

			try {
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

                switch (NetWorkTransType)
                {
                    case NetworkTransportType.TCP:
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
						break;
                    case NetworkTransportType.UDP:
						break;
                    case NetworkTransportType.WebSocket:
                        if (socket.ReadyState == WebSocketState.Open)
                            socket.SendAsync(sendBytes);
                        else
                            HandleIOError("Not Connect");
						break;
                }

            } catch ( NullReferenceException e ) {
				HandleIOError(e.Message);
			} catch ( SocketException e ) {
				HandleIOError(e.Message);
			}			
		}

		string hostName = "";

        public void Connect(string _hostName, int port)
        {
			hostName = _hostName;
            DebugMessage("Trying to connect");
            if (!connected && !connecting)
            {
                try
                {
                    connecting = true;
                  	connected = false;
					
					IPAddress[] addresses = Dns.GetHostAddresses(_hostName);
					if (addresses[0] == null) {
						throw new ArgumentException("Hostname did not resolve to address");
					}
					
					
                    this.ipAddress = addresses[0];
                    this.port = port;
					
					Debug.Log("connect socket: ip " + ipAddress + "/port " + port);

                    switch (NetWorkTransType)
                    {
                        case NetworkTransportType.TCP:
                            thrConnect = new Thread(ConnectThread);
                            thrConnect.Start();
                            break;
                        case NetworkTransportType.UDP:
                            break;
                        case NetworkTransportType.WebSocket:
                            ConnectThread();
                            break;
                    }

				}
                catch (Exception e)
                {
                    connecting = false;
                    HandleIOError(e.Message);
                }
            }
            else
                DebugMessage("*** ALREADY CONNECTED ***");
        }
		
		public void Disconnect(){
			DebugMessage("// Net Disconnect");
            lock (disconnectionLocker){
           		if (connected){
                    try{

                        switch (NetWorkTransType)
                        {
                            case NetworkTransportType.TCP:
                                socketConnection.Close();
                                break;
                            case NetworkTransportType.UDP:
                                break;
                            case NetworkTransportType.WebSocket:
                                if (socket.ReadyState != WebSocketState.Closed)
                                {
                                    socket.CloseAsync();
                                }
                                if (protocol != null)
                                {
                                    protocol.StopHeartbeat();
                                    protocol.CanceledAllUTcs();
                                }
                                break;
                        }

                    }
                    catch (Exception e){
                        DebugMessage("Disconnect Exception: " + e.ToString());
                    }
                    connected = false;
                }

                HandleSocketDisconnection();
            }
        }

        ByteBuffer HBbuffer;

        void ConnectThread()
        {
            try
            {
                switch (NetWorkTransType)
                {
                    case NetworkTransportType.TCP:
                        socketConnection = new TcpClient();
                        socketConnection.NoDelay = true;

                        socketConnection.Connect(ipAddress, port);

                        connected = true;
                        networkStream = socketConnection.GetStream();
                        thrSocketReader = new Thread(HandleSocketData);
                        thrSocketReader.Start();

                        HandleSocketConnection();
                        break;
                    case NetworkTransportType.UDP:
                        break;
                    case NetworkTransportType.WebSocket:
                        ServicePointManager.SecurityProtocol =
                        SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                        SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;//*/

                        string host = string.Format("ws://{0}:{1}/={2}", ipAddress, port, "1.0.0");
#if UNITY_WEBGL && !UNITY_EDITOR
								socket = new WebSocketJS(host);
#else
                        socket = new WebSocket(host);
#endif
                        HBbuffer = new ByteBuffer();
                        socket.OnMessage += OnReceived;
                        socket.OnClose += OnClose;
                        socket.OnError += OnError;
                        socket.OnOpen += OnOpen;
                        if (socket.ReadyState == WebSocketState.Open ||
                       socket.ReadyState == WebSocketState.Connecting)
                        {
                            Debug.LogError("Connect - alread connecting");
                            return;
                        }
                        Debug.Log("Connect - start ConnectAsync()");
                        socket.ConnectAsync();

                        connected = true;
                        break;
                }

            }
            catch (SocketException e)
            {
                DebugMessage("SocketExc " + e.ToString());
                connecting = false;
                HandleIOError(e.Message);
            }
            connecting = false;
        }

        private async void OnOpen(object sender, OpenEventArgs e)
        {
            //socket.OnError -= OnError;
            //socket.OnError += OnError;

            if (protocol == null)
                protocol = new Protocol();
            protocol.SetSocket(socket);
            bool isOK = await protocol.HandShakeAsync(hostName);
            HandleSocketConnection();
        }

        private void OnError(object sender, ServerSDK.Network.ErrorEventArgs e)
        {
            HandleIOError(e.Message);
        }

        private async void OnClose(object sender, CloseEventArgs e)
        {
            if (socket.ReadyState == WebSocketState.Connecting || socket.ReadyState == WebSocketState.Open) return;
            await UniTask.SwitchToMainThread();
            Disconnect();
            HandleSocketDisconnection();
        }

        private void OnReceived(object sender, MessageEventArgs e)
        {
            DecodeData(ref HBbuffer, e.RawData, e.RawData.Length);
        }

        void HandleSocketConnection(){
			Hashtable parameters = new Hashtable();
            parameters.Add("success", true);
            parameters.Add("error", "connected");
			
			UJNetEvent evt = new UJNetEvent(UJNetEvent.onConnectionEvent, parameters);
			DispatchEvent(evt);
		}

		void HandleSocketData()
		{

			int bytesRead = 0;
			byte[] readBytes = new byte[4096];
			ByteBuffer buffer = new ByteBuffer();

			try
			{
				while (true)
				{
					if (socketPollSleep > 0)
					{
						Thread.Sleep(socketPollSleep);
					}

					do
					{
						bytesRead = networkStream.Read(readBytes, 0, readBytes.Length);
						DecodeData(ref buffer, readBytes, bytesRead);
					} while (networkStream.DataAvailable);
				}
			}
			catch (Exception e)
			{
				DebugMessage("Disconnect due to: " + e.InnerException);
				Disconnect();
			}
		}

		void DecodeData(ref ByteBuffer byteBuffer, byte[] readBytes, int bytesRead)
		{
			byte[] recvBytes = new byte[bytesRead];
			Buffer.BlockCopy(readBytes, 0, recvBytes, 0, recvBytes.Length);
			byteBuffer.Put(recvBytes, true);
		
			// parse UJObject
			while ( true ) {
				// check if length complete
				if ( byteBuffer.Length() < 4) {
					break;
				}
				
				// check if data complete
				byteBuffer.Flip();
				int dataLen = byteBuffer.GetInt();
				if(byteBuffer.Length() - 4 < dataLen ){
					byteBuffer.Position(byteBuffer.Length());
					break;
				}
			
				// get ujobj bytes
				byte[] ujObjBin = new byte[dataLen];
				byteBuffer.Get(ujObjBin, 0, dataLen);

                protocol.HandlePacket(ujObjBin);

                //            // convert & handle event
                //            SFSObject ujObj = SFSObject.NewFromBinaryData(new NetCoreServer.UJNet_Framework.UJNet.Data.ByteArray( ujObjBin));
                //commandHandler.Handle(ujObj);

                // check bytes left  
                int leftLen = Convert.ToInt32(byteBuffer.Length() - byteBuffer.Position());
				if ( leftLen == 0 ){
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
		
		void HandleSocketDisconnection(){			
			this.connected = false;

			UJNetEvent evt = new UJNetEvent(UJNetEvent.onConnectionLostEvent, new Hashtable());
			DispatchEvent(evt);
		}
		
		void HandleIOError(string originalError){
			if ( !connected ){
				DebugMessage("[WARN] Socket connect failed: " + originalError);
				DispatchConnectionError(originalError);
			} else {
				// Fire event
				UJNetEvent evt = new UJNetEvent(UJNetEvent.onConnectionLostEvent, new Hashtable());
				DispatchEvent(evt);
				
				// Dispatch back the IO error
				DebugMessage("[WARN] Connection lost error: " + originalError);
			}
		}
		
		void DispatchConnectionError(string errorMessage)
        {
			// // Dispatch back connection error
            Hashtable parameters = new Hashtable();
            parameters.Add("success", false);
            parameters.Add("error", "I/O Error: " + errorMessage);
			
			UJNetEvent evt = new UJNetEvent(UJNetEvent.onConnectionEvent, parameters);
			DispatchEvent(evt);
        }


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
        private static int RqID = 0;
        public async UniTask<Message<S>> RequestAsync<T, S>(string route, T info = default, string modelName = null)
        {
			uint rqID = (uint)Interlocked.Increment(ref RqID);
			try
			{
#if SOCKET_DEBUG
                Debug.Log(string.Format("[{0}][Request] -->> [{1}] {2}", rqID, route, JsonUtility.ToJson(info)));
#endif
				Debug.Log(protocol == null);
                Package pack = await protocol.RequestAsync<T>(rqID, route, info, modelName);
                Message<S> msg = MessageProtocol.Decode<S>(pack.buff);
#if SOCKET_DEBUG
                Debug.Log(string.Format("[{0}][Request] <<-- [{1}] {2} {3}", rqID, route, JsonUtility.ToJson(msg), JsonUtility.ToJson(msg.info)));
#endif
                return msg;
			}
            catch (Exception e)
            {
				Debug.Log(string.Format("[{0}][RequestAsync Exception]{1}", rqID, e.Message));
				throw e;
			}
		}


        //==========Via Http=======//
        public void SendRequest(int cmd, SFSObject obj) {
			SendRequest(null, cmd, obj, true, -1);
		}
		
		public void SendRequest(int cmd, SFSObject obj, bool sync) {
			SendRequest(null, cmd, obj, sync, -1);
		}
		
		public void SendRequest(string url, int cmd, SFSObject obj, bool sync, int locker) {
			try {
                SFSObject param = SFSObject.NewInstance();
				param.PutInt("c", cmd);
				param.PutSFSObject("p", obj);
                //==============自定义修改=====================
    //            if (ScopeHolder.attr.ContainsKey(Const.SCOPE_ACC_ID)) {
				//	param.PutLong("acid", long.Parse(ScopeHolder.attr[Const.SCOPE_ACC_ID].ToString()));
				//}	
				//if (ScopeHolder.attr.ContainsKey(Const.APP_CHANNEL)) {
				//	param.PutUtfString("scid", (string)ScopeHolder.attr[Const.APP_CHANNEL]);
				//}
                //==============自定义修改=====================
                byte[] data = param.ToBinary().Bytes;
				ByteBuffer buffer = ByteBuffer.Allocate(data.Length + 4);
				buffer.PutInt(data.Length);
				buffer.Put(data, true);
				
				RequestSyncLocker.Instance.FireSync(cmd, sync);
				if (sync) {
					locker = RequestSyncLocker.Instance.locker;
				}
				
				byte[] sendBytes = buffer.array();
				SendViaHttp (Util.GetHttpUrl(url, cmd), sendBytes, cmd, locker);
			} catch (Exception e ) {
				DebugMessage(" SendRequest faild " + e.ToString());
			}
		}

		public void HttpPost(string url, string content, int cmd, bool sync) 
		{
			int locker = -1;
			if (sync) {
				RequestSyncLocker.Instance.FireSync(-1);
				locker = RequestSyncLocker.Instance.locker;
			}
			
			string method = "POST";
			string contentType = "application/x-www-form-urlencoded";
			
        	ThreadStart starter = delegate { SendViaCommonHttpItnl(url, UTF8Encoding.UTF8.GetBytes(content), method, contentType, cmd, locker); };
            Thread t = new Thread(new ThreadStart(starter));
            t.IsBackground = true;
            t.Start();
		}
		
		public void HttpGet(string url, Dictionary<string, object> paratable, int cmd, bool sync)
		{
			RequestSyncLocker.Instance.FireSync(cmd, sync);
			int locker = RequestSyncLocker.Instance.locker;
			
			string method = "GET";
			string contentType = null;
			string path = url;
			
			if (paratable != null && paratable.Count > 0) 
			{
				if (path.IndexOf("?") != -1) {
					path +="&";
				} else {
					path +="?";
				}
				path += Util.Dic2QueryStr(paratable);
			}
			
        	ThreadStart starter = delegate { SendViaCommonHttpItnl(path, null, method, contentType, cmd, locker); };
            Thread t = new Thread(new ThreadStart(starter));
            t.IsBackground = true;
            t.Start();			
		}
 	
		void SendViaHttp(string url, byte[] bytes, int cmd, int locker)
        {
         	ThreadStart starter = delegate { SendViaHttpItnl(url, bytes, cmd, locker); };
            Thread t = new Thread(new ThreadStart(starter));
            t.IsBackground = true;
            t.Start();
        }
		
        void SendViaHttpItnl(string url, byte[] bytes, int cmd, int locker)
        {
	        HttpWebRequest request = null;
	        HttpWebResponse response = null;
			int defaultTimeout = 1000 * 60;	
			
			if (!(cmd == 804 || cmd == 807 || cmd == 827 || cmd == 805 || cmd == 310 || cmd == 826 || cmd == 1905)) 
			Debug.Log("req " + url);
			
	        try
	        {
				
				request = (HttpWebRequest)WebRequest.Create(new Uri(url));
				request.Proxy = null;
				request.Method = "POST";
				request.ContentType = "application/octet-stream";
				request.ProtocolVersion = HttpVersion.Version11;
				request.KeepAlive = true;
	            request.ContentLength = bytes.Length;
				request.Timeout = defaultTimeout;
	          	
				using (Stream requestStream = request.GetRequestStream()) {
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Flush();
					requestStream.Close();
				}
	
	            response = (HttpWebResponse) request.GetResponse();
	            if (response.StatusCode == HttpStatusCode.OK)
	            {
	                using(Stream responseStream = response.GetResponseStream()) {
						int bytesRead = 0;
						byte[] readBytes = new byte[4096];
						ByteBuffer buffer = new ByteBuffer();
						do {
							bytesRead = responseStream.Read(readBytes, 0, readBytes.Length);
							if (bytesRead > 0) 
								DecodeData(ref buffer, readBytes, bytesRead);
						} while (bytesRead > 0);
						responseStream.Close();
					}
	            } 
				else 
				{
					DebugMessage("WARNING: Received invalid Http StatusCode " + response.StatusCode + "/" + response.StatusDescription + "/" + url);
					
					// fire http state exception
					Hashtable parameters = new Hashtable();
					parameters.Add("locker", locker);
					parameters.Add("exception", new Exception("Server exception " + response.StatusCode));
					UJNetEvent evt = new UJNetEvent(UJNetEvent.onHttpErrorEvent, parameters);
					DispatchEvent(evt);
				}
	        }
	        catch (Exception ex)
	        {			
                DebugMessage("WARNING: Exception thrown trying to send/read data via HTTP. Exception: " + ex.ToString() + "/" + url);
				
				// fire web exception
				Hashtable parameters = new Hashtable();
				parameters.Add("locker", locker);
				parameters.Add("exception", ex);
				UJNetEvent evt = new UJNetEvent(UJNetEvent.onHttpErrorEvent, parameters);
				DispatchEvent(evt);
			}
	        finally
	        {
				if (request != null) {
					try {
						request.Abort();
					}catch(Exception ex) {
						DebugMessage("WARNING: Http error closing http request: " + ex.ToString());
					}
				}
				
				if (response != null) {
					try {
						response.Close();
					} catch (Exception ex) {
						DebugMessage("WARNING: Http error closing http connection: " + ex.ToString());
					}
				}
				
	            request = null;
	            response = null;
				
				// fire http end
				Hashtable parameters = new Hashtable();
				parameters.Add("locker", locker);
				UJNetEvent evt = new UJNetEvent(UJNetEvent.onHttpCloseEvent, parameters);
				DispatchEvent(evt);
			}		
		}

        void SendViaCommonHttpItnl(string url, byte[] bytes, string method, string contentType, int cmd, int locker)
        {
	        HttpWebRequest request = null;
	        HttpWebResponse response = null;
			int defaultTimeout = 1000 * 60;	
			
			Debug.Log("req " + url + "/ " + cmd);
			
	        try
	        {
				request = (HttpWebRequest)WebRequest.Create(new Uri(url));
				request.Proxy = null;
				request.Method = method;
				request.ProtocolVersion = HttpVersion.Version11;
				request.KeepAlive = true;
				request.Timeout = defaultTimeout;
	          	
				if (request.Method.Trim().ToUpper().Equals("POST")) {
					request.ContentType = contentType;
	            	request.ContentLength = bytes.Length;
					using (Stream requestStream = request.GetRequestStream()) {
						requestStream.Write(bytes, 0, bytes.Length);
						requestStream.Flush();
						requestStream.Close();
					}
				}
	
	            response = (HttpWebResponse) request.GetResponse();
	            if (response.StatusCode == HttpStatusCode.OK)
	            {
	                using(Stream responseStream = response.GetResponseStream()) {
						StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
						string text = reader.ReadToEnd();						
						reader.Close();
						responseStream.Close();
						
						Debug.Log("resp " + text);
	
						Hashtable parameters = Hashtable.Synchronized(new Hashtable());
						parameters.Add("cmd", cmd);
						parameters.Add("resp", text);
						UJNetEvent evt = new UJNetEvent(UJNetEvent.onHttpResponseEvent, parameters);
						DispatchEvent(evt);
					}
	            } 
				else 
				{
					DebugMessage("WARNING: Received invalid Http StatusCode " + response.StatusCode + "/" + response.StatusDescription + "/" + url);
					
					// fire http state exception
					Hashtable parameters = new Hashtable();
					parameters.Add("locker", locker);
					parameters.Add("exception", new Exception("Server exception " + response.StatusCode));
					UJNetEvent evt = new UJNetEvent(UJNetEvent.onHttpErrorEvent, parameters);
					DispatchEvent(evt);
				}
	        }
	        catch (Exception ex)
	        {			
				DebugMessage("WARNING: Exception thrown trying to send/read data via HTTP. Exception: " + ex.ToString() + "/" + url);
				
				// fire web exception
				Hashtable parameters = new Hashtable();
				parameters.Add("locker", locker);
				parameters.Add("exception", ex);
				UJNetEvent evt = new UJNetEvent(UJNetEvent.onHttpErrorEvent, parameters);
				DispatchEvent(evt);
			}
	        finally
	        {
				if (request != null) {
					try {
						request.Abort();
					}catch(Exception ex) {
						DebugMessage("WARNING: Http error closing http request: " + ex.ToString());
					}
				}
				
				if (response != null) {
					try {
						response.Close();
					} catch (Exception ex) {
						DebugMessage("WARNING: Http error closing http connection: " + ex.ToString());
					}
				}
				
	            request = null;
	            response = null;
				
				// fire http end
				Hashtable parameters = new Hashtable();
				parameters.Add("locker", locker);
				UJNetEvent evt = new UJNetEvent(UJNetEvent.onHttpCloseEvent, parameters);
				DispatchEvent(evt);
			}		
		}
		
		// =========Common==========//
		void DebugMessage(string message) {
			if ( this.debug ) {
				Hashtable parameters = new Hashtable();
				parameters.Add("message", message);
				UJNetEvent evt = new UJNetEvent(UJNetEvent.onDebugMessageEvent, parameters);
				DispatchEvent(evt);
			}
		}

		public void ProcessEventQueue() {
			Queue queuedEventsClone = null;
			lock ( queuedEventsLocker ) {
				queuedEventsClone = queuedEvents.Clone() as Queue;
				queuedEvents.Clear();
			}
			
			while ( queuedEventsClone.Count > 0 ) {
				UJNetEvent evt = queuedEventsClone.Dequeue() as UJNetEvent;
				_DispatchEvent(evt);
			}
		}

		internal void DispatchEvent(UJNetEvent evt){
			lock ( queuedEventsLocker ) {
				queuedEvents.Enqueue(evt);
			}
		}
		
		internal void _DispatchEvent(UJNetEvent evt)
        {
            try
            {	
                switch (evt.GetType())
                {
                    case UJNetEvent.onConnectionEvent:
                        if (OnConnection != null)
                        {
                            OnConnection((bool)evt.GetParameter("success"), (string)evt.GetParameter("error"));
                        }
                        else
                        {
                            Debug.LogWarning("Trying to call onConnection, but no callback is registered");
                        }
                        break;
                    case UJNetEvent.onConnectionLostEvent:
                        if (OnConnectionLost != null)
                        {
                            OnConnectionLost();
                        }
                        else
                        {
                            Debug.LogWarning("Trying to call onConnectionLost, but no callback is registered");
                        }
                        break;
                    case UJNetEvent.onDebugMessageEvent:
                        if (OnDebugMessage != null)
                        {
                            OnDebugMessage((string)evt.GetParameter("message"));
                        }
                        else
                        {
                            Debug.LogWarning("Trying to call onDebugMessage, but no callback is registered");
                        }
                        break;
                    case UJNetEvent.onResponseEvent:
                        if (OnResponse != null)
                        {
							OnResponse((int)evt.GetParameter("cmd"), (SFSObject)evt.GetParameter("dataObj"));
                        }
                        else
                        {
                            Debug.LogWarning("Trying to call onExtensionResponse, but no callback is registered");
                        }
                        break;
                    case UJNetEvent.onHttpResponseEvent:
                        if (OnHttpResponse != null)
                        {
							OnHttpResponse((int)evt.GetParameter("cmd"), (string)evt.GetParameter("resp"));
                        }
                        else
                        {
                            Debug.LogWarning("Trying to call onExtensionResponse, but no callback is registered");
                        }
                        break;
					
                     case UJNetEvent.onHttpCloseEvent:
                        if (OnHttpClose != null)
                        {
                            OnHttpClose((int)evt.GetParameter("locker"));
                        }
                        else
                        {
                            Debug.LogWarning("Trying to call OnHttpClose, but no callback is registered");
                        }
                        break;
					 case UJNetEvent.onHttpErrorEvent:
                        if (OnHttpError != null)
                        {
                            OnHttpError((Exception)evt.GetParameter("exception"), (int)evt.GetParameter("locker"));
                        }
                        else
                        {
                            Debug.LogWarning("Trying to call OnHttpError, but no callback is registered");
                        }
                        break;
                    default:
                        DebugMessage("Unknown event dispatched " + evt.GetType());
                        break;
                }
            }
            catch (Exception e)
            {
				Debug.LogWarning("ERROR: Exception thrown dispatching event. Exception: " + e.ToString());
				if ( OnDebugMessage != null ) {
					OnDebugMessage("ERROR: Exception thrown dispatching event. Exception: " + e.ToString());
				}
				throw e;
            }
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing) {
			if ( !this.isDisposed ) {
				if ( disposing ) {
				}
				
				if ( connected ) {
					Disconnect();
				}
				this.isDisposed = true;
			}
		}
	}
}

