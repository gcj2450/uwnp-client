using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using SuperSocket.ClientEngine;

namespace WebSocket4Net
{
	public class JsonWebSocket : IDisposable
	{
		private WebSocket m_WebSocket;

		private bool m_disposed;

		private EventHandler<SuperSocket.ClientEngine.ErrorEventArgs> m_Error;

		private EventHandler m_Opened;

		private EventHandler m_Closed;

		private static Random m_Random = new Random();

		private const string m_QueryTemplateA = "{0}-{1} {2}";

		private const string m_QueryTemplateB = "{0}-{1}";

		private const string m_QueryTemplateC = "{0} {1}";

		private const string m_QueryKeyTokenTemplate = "{0}-{1}";

		private Dictionary<string, IJsonExecutor> m_ExecutorDict = new Dictionary<string, IJsonExecutor>(StringComparer.OrdinalIgnoreCase);

		public bool EnableAutoSendPing
		{
			get
			{
				return m_WebSocket.EnableAutoSendPing;
			}
			set
			{
				m_WebSocket.EnableAutoSendPing = value;
			}
		}

		public int AutoSendPingInterval
		{
			get
			{
				return m_WebSocket.AutoSendPingInterval;
			}
			set
			{
				m_WebSocket.AutoSendPingInterval = value;
			}
		}

		public WebSocketState State
		{
			get
			{
				return m_WebSocket.State;
			}
		}

		public int ReceiveBufferSize
		{
			get
			{
				return m_WebSocket.ReceiveBufferSize;
			}
			set
			{
				m_WebSocket.ReceiveBufferSize = value;
			}
		}

		public SecurityOption Security
		{
			get
			{
				return m_WebSocket.Security;
			}
		}

		public event EventHandler<SuperSocket.ClientEngine.ErrorEventArgs> Error
		{
			add
			{
				m_Error = (EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>)Delegate.Combine(m_Error, value);
			}
			remove
			{
				m_Error = (EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>)Delegate.Remove(m_Error, value);
			}
		}

		public event EventHandler Opened
		{
			add
			{
				m_Opened = (EventHandler)Delegate.Combine(m_Opened, value);
			}
			remove
			{
				m_Opened = (EventHandler)Delegate.Remove(m_Opened, value);
			}
		}

		public event EventHandler Closed
		{
			add
			{
				m_Closed = (EventHandler)Delegate.Combine(m_Closed, value);
			}
			remove
			{
				m_Closed = (EventHandler)Delegate.Remove(m_Closed, value);
			}
		}

		public JsonWebSocket(string uri)
			: this(uri, string.Empty)
		{
		}

		public JsonWebSocket(string uri, WebSocketVersion version)
			: this(uri, string.Empty, null, version)
		{
		}

		public JsonWebSocket(string uri, string subProtocol)
			: this(uri, subProtocol, null, WebSocketVersion.None)
		{
		}

		public JsonWebSocket(string uri, List<KeyValuePair<string, string>> cookies)
			: this(uri, string.Empty, cookies, WebSocketVersion.None)
		{
		}

		public JsonWebSocket(string uri, string subProtocol, List<KeyValuePair<string, string>> cookies)
			: this(uri, subProtocol, cookies, WebSocketVersion.None)
		{
		}

		public JsonWebSocket(string uri, string subProtocol, WebSocketVersion version)
			: this(uri, subProtocol, null, version)
		{
		}

		public JsonWebSocket(string uri, string subProtocol, List<KeyValuePair<string, string>> cookies, WebSocketVersion version)
			: this(uri, subProtocol, cookies, null, string.Empty, string.Empty, version)
		{
		}

		public JsonWebSocket(string uri, string subProtocol, List<KeyValuePair<string, string>> cookies, List<KeyValuePair<string, string>> customHeaderItems, string userAgent, WebSocketVersion version)
			: this(uri, subProtocol, cookies, customHeaderItems, userAgent, string.Empty, version)
		{
		}

		public JsonWebSocket(string uri, string subProtocol, List<KeyValuePair<string, string>> cookies, List<KeyValuePair<string, string>> customHeaderItems, string userAgent, string origin, WebSocketVersion version)
		{
			m_WebSocket = new WebSocket(uri, subProtocol, cookies, customHeaderItems, userAgent, origin, version);
			m_WebSocket.EnableAutoSendPing = true;
			SubscribeEvents();
		}

		public JsonWebSocket(WebSocket websocket)
		{
			if (websocket == null)
			{
				throw new ArgumentNullException("websocket");
			}
			if (websocket.State != WebSocketState.None)
			{
				throw new ArgumentException("Thed websocket must be in the initial state.", "websocket");
			}
			m_WebSocket = websocket;
			SubscribeEvents();
		}

		private void SubscribeEvents()
		{
			m_WebSocket.Closed += m_WebSocket_Closed;
			m_WebSocket.MessageReceived += m_WebSocket_MessageReceived;
			m_WebSocket.Opened += m_WebSocket_Opened;
			m_WebSocket.Error += m_WebSocket_Error;
		}

		public void Open()
		{
			if (m_WebSocket.StateCode == -1 || m_WebSocket.StateCode == 3)
			{
				m_WebSocket.Open();
			}
		}

		public void Close()
		{
			if (m_WebSocket != null && (m_WebSocket.StateCode == 1 || m_WebSocket.StateCode == 0))
			{
				m_WebSocket.Close();
			}
		}

		private void m_WebSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
		{
			if (m_Error != null)
			{
				m_Error(this, e);
			}
		}

		private void m_WebSocket_Opened(object sender, EventArgs e)
		{
			if (m_Opened != null)
			{
				m_Opened(this, e);
			}
		}

		private void m_WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Message))
			{
				return;
			}
			int num = e.Message.IndexOf(' ');
			string token = string.Empty;
			string text;
			string text2;
			if (num > 0)
			{
				text = e.Message.Substring(0, num);
				text2 = e.Message.Substring(num + 1);
				num = text.IndexOf('-');
				if (num > 0)
				{
					token = text.Substring(num + 1);
					text = text.Substring(0, num);
				}
			}
			else
			{
				text = e.Message;
				text2 = string.Empty;
			}
			IJsonExecutor executor = GetExecutor(text, token);
			if (executor == null)
			{
				return;
			}
			object param;
			try
			{
				param = ((!executor.Type.IsSimpleType()) ? DeserializeObject(text2, executor.Type) : ((!(text2.GetType() == executor.Type)) ? Convert.ChangeType(text2, executor.Type, null) : text2));
			}
			catch (Exception innerException)
			{
				m_WebSocket_Error(this, new SuperSocket.ClientEngine.ErrorEventArgs(new Exception("DeserializeObject exception", innerException)));
				return;
			}
			try
			{
				executor.Execute(this, token, param);
			}
			catch (Exception innerException2)
			{
				m_WebSocket_Error(this, new SuperSocket.ClientEngine.ErrorEventArgs(new Exception("Message handling exception", innerException2)));
			}
		}

		private void m_WebSocket_Closed(object sender, EventArgs e)
		{
			if (m_Closed != null)
			{
				m_Closed(this, e);
			}
		}

		public void On<T>(string name, Action<T> executor)
		{
			RegisterExecutor<T>(name, string.Empty, new JsonExecutor<T>(executor));
		}

		public void On<T>(string name, Action<JsonWebSocket, T> executor)
		{
			RegisterExecutor<T>(name, string.Empty, new JsonExecutorWithSender<T>(executor));
		}

		public void Send(string name, object content)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (content != null)
			{
				if (!content.GetType().IsSimpleType())
				{
					m_WebSocket.Send(string.Format("{0} {1}", name, SerializeObject(content)));
				}
				else
				{
					m_WebSocket.Send(string.Format("{0} {1}", name, content));
				}
			}
			else
			{
				m_WebSocket.Send(name);
			}
		}

		public string Query<T>(string name, object content, Action<T> executor)
		{
			return Query<T>(name, content, new JsonExecutor<T>(executor));
		}

		public string Query<T>(string name, object content, Action<string, T> executor)
		{
			return Query<T>(name, content, new JsonExecutorWithToken<T>(executor));
		}

		public string Query<T>(string name, object content, Action<JsonWebSocket, T> executor)
		{
			return Query<T>(name, content, new JsonExecutorWithSender<T>(executor));
		}

		public string Query<T>(string name, object content, Action<JsonWebSocket, string, T> executor)
		{
			return Query<T>(name, content, new JsonExecutorFull<T>(executor));
		}

		public string Query<T>(string name, object content, Action<JsonWebSocket, T, object> executor, object state)
		{
			return Query<T>(name, content, new JsonExecutorWithSenderAndState<T>(executor, state));
		}

		private string Query<T>(string name, object content, IJsonExecutor executor)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			int num = m_Random.Next(1000, 9999);
			RegisterExecutor<T>(name, num.ToString(), executor);
			if (content != null)
			{
				if (!content.GetType().IsSimpleType())
				{
					m_WebSocket.Send(string.Format("{0}-{1} {2}", name, num, SerializeObject(content)));
				}
				else
				{
					m_WebSocket.Send(string.Format("{0}-{1} {2}", name, num, content));
				}
			}
			else
			{
				m_WebSocket.Send(string.Format("{0}-{1}", name, num));
			}
			return num.ToString();
		}

		private void RegisterExecutor<T>(string name, string token, IJsonExecutor executor)
		{
			lock (m_ExecutorDict)
			{
				if (string.IsNullOrEmpty(token))
				{
					m_ExecutorDict.Add(name, executor);
				}
				else
				{
					m_ExecutorDict.Add(string.Format("{0}-{1}", name, token), executor);
				}
			}
		}

		private IJsonExecutor GetExecutor(string name, string token)
		{
			string key = name;
			bool flag = false;
			if (!string.IsNullOrEmpty(token))
			{
				key = string.Format("{0}-{1}", name, token);
				flag = true;
			}
			lock (m_ExecutorDict)
			{
				IJsonExecutor value;
				if (!m_ExecutorDict.TryGetValue(key, out value))
				{
					return null;
				}
				if (flag)
				{
					m_ExecutorDict.Remove(key);
				}
				return value;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				if (disposing && m_WebSocket != null)
				{
					m_WebSocket.Dispose();
				}
				m_disposed = true;
			}
		}

		~JsonWebSocket()
		{
			Dispose(false);
		}

		protected virtual string SerializeObject(object target)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(target.GetType());
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject((Stream)memoryStream, target);
				return Encoding.UTF8.GetString(memoryStream.ToArray());
			}
		}

		protected virtual object DeserializeObject(string json, Type type)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(type);
			using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
			{
				return dataContractJsonSerializer.ReadObject((Stream)stream);
			}
		}
	}
}
