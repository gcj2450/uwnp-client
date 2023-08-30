using System;
using System.Net;
using System.Net.Sockets;

namespace SuperSocket.ClientEngine
{
	public static class ConnectAsyncExtension
	{
		private class ConnectToken
		{
			public object State { get; set; }

			public ConnectedCallback Callback { get; set; }
		}

		private static void SocketAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
		{
			e.Completed -= SocketAsyncEventCompleted;
			ConnectToken connectToken = (ConnectToken)e.UserToken;
			e.UserToken = null;
			connectToken.Callback(sender as Socket, connectToken.State, e, null);
		}

		private static SocketAsyncEventArgs CreateSocketAsyncEventArgs(EndPoint remoteEndPoint, ConnectedCallback callback, object state)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
			socketAsyncEventArgs.UserToken = new ConnectToken
			{
				State = state,
				Callback = callback
			};
			socketAsyncEventArgs.RemoteEndPoint = remoteEndPoint;
			socketAsyncEventArgs.Completed += SocketAsyncEventCompleted;
			return socketAsyncEventArgs;
		}

		internal static bool PreferIPv4Stack()
		{
			return Environment.GetEnvironmentVariable("PREFER_IPv4_STACK") != null;
		}

		public static void ConnectAsync(this EndPoint remoteEndPoint, EndPoint localEndPoint, ConnectedCallback callback, object state)
		{
			SocketAsyncEventArgs e = CreateSocketAsyncEventArgs(remoteEndPoint, callback, state);
			Socket socket = (PreferIPv4Stack() ? new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) : new Socket(SocketType.Stream, ProtocolType.Tcp));
			if (localEndPoint != null)
			{
				try
				{
					socket.ExclusiveAddressUse = false;
					socket.Bind(localEndPoint);
				}
				catch (Exception exception)
				{
					callback(null, state, null, exception);
					return;
				}
			}
			socket.ConnectAsync(e);
		}
	}
}
