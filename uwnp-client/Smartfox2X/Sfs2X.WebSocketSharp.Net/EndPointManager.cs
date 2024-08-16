using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Sfs2X.WebSocketSharp.Net
{
	internal sealed class EndPointManager
	{
		private static readonly Dictionary<IPEndPoint, EndPointListener> _endpoints;

		static EndPointManager()
		{
			_endpoints = new Dictionary<IPEndPoint, EndPointListener>();
		}

		private EndPointManager()
		{
		}

		private static void addPrefix(string uriPrefix, HttpListener listener)
		{
			HttpListenerPrefix httpListenerPrefix = new HttpListenerPrefix(uriPrefix);
			IPAddress iPAddress = convertToIPAddress(httpListenerPrefix.Host);
			if (iPAddress == null)
			{
				throw new HttpListenerException(87, "Includes an invalid host.");
			}
			if (!iPAddress.IsLocal())
			{
				throw new HttpListenerException(87, "Includes an invalid host.");
			}
			int result;
			if (!int.TryParse(httpListenerPrefix.Port, out result))
			{
				throw new HttpListenerException(87, "Includes an invalid port.");
			}
			if (!result.IsPortNumber())
			{
				throw new HttpListenerException(87, "Includes an invalid port.");
			}
			string path = httpListenerPrefix.Path;
			if (path.IndexOf('%') != -1)
			{
				throw new HttpListenerException(87, "Includes an invalid path.");
			}
			if (path.IndexOf("//", StringComparison.Ordinal) != -1)
			{
				throw new HttpListenerException(87, "Includes an invalid path.");
			}
			IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, result);
			EndPointListener value;
			if (_endpoints.TryGetValue(iPEndPoint, out value))
			{
				if (value.IsSecure ^ httpListenerPrefix.IsSecure)
				{
					throw new HttpListenerException(87, "Includes an invalid scheme.");
				}
			}
			else
			{
				value = new EndPointListener(iPEndPoint, httpListenerPrefix.IsSecure, listener.CertificateFolderPath, listener.SslConfiguration, listener.ReuseAddress);
				_endpoints.Add(iPEndPoint, value);
			}
			value.AddPrefix(httpListenerPrefix, listener);
		}

		private static IPAddress convertToIPAddress(string hostname)
		{
			if (hostname == "*")
			{
				return IPAddress.Any;
			}
			if (hostname == "+")
			{
				return IPAddress.Any;
			}
			return hostname.ToIPAddress();
		}

		private static void removePrefix(string uriPrefix, HttpListener listener)
		{
			HttpListenerPrefix httpListenerPrefix = new HttpListenerPrefix(uriPrefix);
			IPAddress iPAddress = convertToIPAddress(httpListenerPrefix.Host);
			int result;
			if (iPAddress == null || !iPAddress.IsLocal() || !int.TryParse(httpListenerPrefix.Port, out result) || !result.IsPortNumber())
			{
				return;
			}
			string path = httpListenerPrefix.Path;
			if (path.IndexOf('%') == -1 && path.IndexOf("//", StringComparison.Ordinal) == -1)
			{
				IPEndPoint key = new IPEndPoint(iPAddress, result);
				EndPointListener value;
				if (_endpoints.TryGetValue(key, out value) && !(value.IsSecure ^ httpListenerPrefix.IsSecure))
				{
					value.RemovePrefix(httpListenerPrefix, listener);
				}
			}
		}

		internal static bool RemoveEndPoint(IPEndPoint endpoint)
		{
			lock (((ICollection)_endpoints).SyncRoot)
			{
				EndPointListener value;
				if (!_endpoints.TryGetValue(endpoint, out value))
				{
					return false;
				}
				_endpoints.Remove(endpoint);
				value.Close();
				return true;
			}
		}

		public static void AddListener(HttpListener listener)
		{
			List<string> list = new List<string>();
			lock (((ICollection)_endpoints).SyncRoot)
			{
				try
				{
					foreach (string prefix in listener.Prefixes)
					{
						addPrefix(prefix, listener);
						list.Add(prefix);
					}
				}
				catch
				{
					foreach (string item in list)
					{
						removePrefix(item, listener);
					}
					throw;
				}
			}
		}

		public static void AddPrefix(string uriPrefix, HttpListener listener)
		{
			lock (((ICollection)_endpoints).SyncRoot)
			{
				addPrefix(uriPrefix, listener);
			}
		}

		public static void RemoveListener(HttpListener listener)
		{
			lock (((ICollection)_endpoints).SyncRoot)
			{
				foreach (string prefix in listener.Prefixes)
				{
					removePrefix(prefix, listener);
				}
			}
		}

		public static void RemovePrefix(string uriPrefix, HttpListener listener)
		{
			lock (((ICollection)_endpoints).SyncRoot)
			{
				removePrefix(uriPrefix, listener);
			}
		}
	}
}
