using System;
using System.Runtime.InteropServices;
using System.Text;
using Sfs2X.Logging;

namespace Sfs2X.Core.Sockets
{
	public class WebSocketHelper
	{
		private Uri url;

		private Logger log;

		private int sockeInstanceRef = 0;

		public bool IsConnected
		{
			get
			{
				return SocketState(sockeInstanceRef) == 1;
			}
		}

		public WebSocketError Error
		{
			get
			{
				string text = SocketError(sockeInstanceRef);
				if (text == null || text == "")
				{
					return null;
				}
				return new WebSocketError(text);
			}
		}

		public WebSocketHelper(Uri url, Logger log)
		{
			this.url = url;
			this.log = log;
			string scheme = url.Scheme;
			if (!scheme.Equals("ws") && !scheme.Equals("wss"))
			{
				throw new ArgumentException("Unsupported protocol: " + scheme);
			}
		}

		[DllImport("__Internal")]
		private static extern int SocketCreate(string url);

		[DllImport("__Internal")]
		private static extern int SocketState(int socketInstance);

		[DllImport("__Internal")]
		private static extern void SocketSend(int socketInstance, byte[] buffer, int size, bool asString);

		[DllImport("__Internal")]
		private static extern void SocketRecv(int socketInstance, IntPtr buffer, int size);

		[DllImport("__Internal")]
		private static extern int SocketRecvLength(int socketInstance);

		[DllImport("__Internal")]
		private static extern void SocketClose(int socketInstance);

		[DllImport("__Internal")]
		private static extern string SocketError(int socketInstance);

		public void Connect()
		{
			log.Debug("Connecting with SFSWebSockets.jslib library");
			sockeInstanceRef = SocketCreate(url.ToString());
		}

		public void Send(byte[] buffer)
		{
			SocketSend(sockeInstanceRef, buffer, buffer.Length, false);
		}

		public void Send(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			SocketSend(sockeInstanceRef, bytes, bytes.Length, true);
		}

		public byte[] ReceiveByteArray()
		{
			int num = SocketRecvLength(sockeInstanceRef);
			if (num == 0)
			{
				return null;
			}
			byte[] array = new byte[num];
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			SocketRecv(sockeInstanceRef, intPtr, num);
			Marshal.Copy(intPtr, array, 0, num);
			Marshal.FreeHGlobal(intPtr);
			return array;
		}

		public string ReceiveString()
		{
			byte[] array = ReceiveByteArray();
			if (array == null)
			{
				return null;
			}
			return Encoding.UTF8.GetString(array);
		}

		public void Close()
		{
			SocketClose(sockeInstanceRef);
		}
	}
}
