using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Sfs2X.WebSocketSharp.Net;

namespace Sfs2X.WebSocketSharp
{
	/// <summary>
	/// Provides a set of static methods for websocket-sharp.
	/// </summary>
	public static class Ext
	{
		private static readonly byte[] _last = new byte[1];

		private static readonly int _retry = 5;

		private const string _tspecials = "()<>@,;:\\\"/[]?={} \t";

		private static byte[] compress(this byte[] data)
		{
			if (data.LongLength == 0)
			{
				return data;
			}
			using (MemoryStream stream = new MemoryStream(data))
			{
				return stream.compressToArray();
			}
		}

		private static MemoryStream compress(this Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			if (stream.Length == 0)
			{
				return memoryStream;
			}
			stream.Position = 0L;
			using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
			{
				stream.CopyTo(deflateStream, 1024);
				deflateStream.Close();
				memoryStream.Write(_last, 0, 1);
				memoryStream.Position = 0L;
				return memoryStream;
			}
		}

		private static byte[] compressToArray(this Stream stream)
		{
			using (MemoryStream memoryStream = stream.compress())
			{
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		private static byte[] decompress(this byte[] data)
		{
			if (data.LongLength == 0)
			{
				return data;
			}
			using (MemoryStream stream = new MemoryStream(data))
			{
				return stream.decompressToArray();
			}
		}

		private static MemoryStream decompress(this Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			if (stream.Length == 0)
			{
				return memoryStream;
			}
			stream.Position = 0L;
			using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true))
			{
				deflateStream.CopyTo(memoryStream, 1024);
				memoryStream.Position = 0L;
				return memoryStream;
			}
		}

		private static byte[] decompressToArray(this Stream stream)
		{
			using (MemoryStream memoryStream = stream.decompress())
			{
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		private static bool isHttpMethod(this string value)
		{
			int result;
			switch (value)
			{
			default:
				result = ((value == "TRACE") ? 1 : 0);
				break;
			case "GET":
			case "HEAD":
			case "POST":
			case "PUT":
			case "DELETE":
			case "CONNECT":
			case "OPTIONS":
				result = 1;
				break;
			}
			return (byte)result != 0;
		}

		private static bool isHttpMethod10(this string value)
		{
			return value == "GET" || value == "HEAD" || value == "POST";
		}

		private static void times(this ulong n, Action action)
		{
			for (ulong num = 0uL; num < n; num++)
			{
				action();
			}
		}

		internal static byte[] Append(this ushort code, string reason)
		{
			byte[] array = code.InternalToByteArray(ByteOrder.Big);
			if (reason != null && reason.Length > 0)
			{
				List<byte> list = new List<byte>(array);
				list.AddRange(Encoding.UTF8.GetBytes(reason));
				array = list.ToArray();
			}
			return array;
		}

		internal static void Close(this Sfs2X.WebSocketSharp.Net.HttpListenerResponse response, Sfs2X.WebSocketSharp.Net.HttpStatusCode code)
		{
			response.StatusCode = (int)code;
			response.OutputStream.Close();
		}

		internal static void CloseWithAuthChallenge(this Sfs2X.WebSocketSharp.Net.HttpListenerResponse response, string challenge)
		{
			response.Headers.InternalSet("WWW-Authenticate", challenge, true);
			response.Close(Sfs2X.WebSocketSharp.Net.HttpStatusCode.Unauthorized);
		}

		internal static byte[] Compress(this byte[] data, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? data.compress() : data;
		}

		internal static Stream Compress(this Stream stream, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? stream.compress() : stream;
		}

		internal static byte[] CompressToArray(this Stream stream, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? stream.compressToArray() : stream.ToByteArray();
		}

		/// <summary>
		/// Determines whether the specified string contains any of characters in
		/// the specified array of <see cref="T:System.Char" />.
		/// </summary>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> contains any of characters in
		/// <paramref name="anyOf" />; otherwise, <c>false</c>.
		/// </returns>
		/// <param name="value">
		/// A <see cref="T:System.String" /> to test.
		/// </param>
		/// <param name="anyOf">
		/// An array of <see cref="T:System.Char" /> that contains one or more characters to
		/// seek.
		/// </param>
		internal static bool Contains(this string value, params char[] anyOf)
		{
			return anyOf != null && anyOf.Length != 0 && value.IndexOfAny(anyOf) > -1;
		}

		internal static bool Contains(this NameValueCollection collection, string name)
		{
			return collection[name] != null;
		}

		internal static bool Contains(this NameValueCollection collection, string name, string value, StringComparison comparisonTypeForValue)
		{
			string text = collection[name];
			if (text == null)
			{
				return false;
			}
			string[] array = text.Split(',');
			foreach (string text2 in array)
			{
				if (text2.Trim().Equals(value, comparisonTypeForValue))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> condition)
		{
			foreach (T item in source)
			{
				if (condition(item))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool ContainsTwice(this string[] values)
		{
			int len = values.Length;
			int end = len - 1;
			Func<int, bool> seek = null;
			seek = delegate(int idx)
			{
				if (idx == end)
				{
					return false;
				}
				string text = values[idx];
				for (int i = idx + 1; i < len; i++)
				{
					if (values[i] == text)
					{
						return true;
					}
				}
				return seek(++idx);
			};
			return seek(0);
		}

		internal static T[] Copy<T>(this T[] source, int length)
		{
			T[] array = new T[length];
			Array.Copy(source, 0, array, 0, length);
			return array;
		}

		internal static T[] Copy<T>(this T[] source, long length)
		{
			T[] array = new T[length];
			Array.Copy(source, 0L, array, 0L, length);
			return array;
		}

		internal static void CopyTo(this Stream source, Stream destination, int bufferLength)
		{
			byte[] buffer = new byte[bufferLength];
			int num = 0;
			while ((num = source.Read(buffer, 0, bufferLength)) > 0)
			{
				destination.Write(buffer, 0, num);
			}
		}

		internal static void CopyToAsync(this Stream source, Stream destination, int bufferLength, Action completed, Action<Exception> error)
		{
			byte[] buff = new byte[bufferLength];
			AsyncCallback callback = null;
			callback = delegate(IAsyncResult ar)
			{
				try
				{
					int num = source.EndRead(ar);
					if (num <= 0)
					{
						if (completed != null)
						{
							completed();
						}
					}
					else
					{
						destination.Write(buff, 0, num);
						source.BeginRead(buff, 0, bufferLength, callback, null);
					}
				}
				catch (Exception obj2)
				{
					if (error != null)
					{
						error(obj2);
					}
				}
			};
			try
			{
				source.BeginRead(buff, 0, bufferLength, callback, null);
			}
			catch (Exception obj)
			{
				if (error != null)
				{
					error(obj);
				}
			}
		}

		internal static byte[] Decompress(this byte[] data, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? data.decompress() : data;
		}

		internal static Stream Decompress(this Stream stream, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? stream.decompress() : stream;
		}

		internal static byte[] DecompressToArray(this Stream stream, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? stream.decompressToArray() : stream.ToByteArray();
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Int32" /> equals the specified <see cref="T:System.Char" />,
		/// and invokes the specified <c>Action&lt;int&gt;</c> delegate at the same time.
		/// </summary>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> equals <paramref name="c" />;
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <param name="value">
		/// An <see cref="T:System.Int32" /> to compare.
		/// </param>
		/// <param name="c">
		/// A <see cref="T:System.Char" /> to compare.
		/// </param>
		/// <param name="action">
		/// An <c>Action&lt;int&gt;</c> delegate that references the method(s) called
		/// at the same time as comparing. An <see cref="T:System.Int32" /> parameter to pass to
		/// the method(s) is <paramref name="value" />.
		/// </param>
		internal static bool EqualsWith(this int value, char c, Action<int> action)
		{
			action(value);
			return value == c;
		}

		/// <summary>
		/// Gets the absolute path from the specified <see cref="T:System.Uri" />.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents the absolute path if it's successfully found;
		/// otherwise, <see langword="null" />.
		/// </returns>
		/// <param name="uri">
		/// A <see cref="T:System.Uri" /> that represents the URI to get the absolute path from.
		/// </param>
		internal static string GetAbsolutePath(this Uri uri)
		{
			if (uri.IsAbsoluteUri)
			{
				return uri.AbsolutePath;
			}
			string originalString = uri.OriginalString;
			if (originalString[0] != '/')
			{
				return null;
			}
			int num = originalString.IndexOfAny(new char[2] { '?', '#' });
			return (num > 0) ? originalString.Substring(0, num) : originalString;
		}

		internal static Sfs2X.WebSocketSharp.Net.CookieCollection GetCookies(this NameValueCollection headers, bool response)
		{
			string text = headers[response ? "Set-Cookie" : "Cookie"];
			return (text != null) ? Sfs2X.WebSocketSharp.Net.CookieCollection.Parse(text, response) : new Sfs2X.WebSocketSharp.Net.CookieCollection();
		}

		internal static string GetDnsSafeHost(this Uri uri, bool bracketIPv6)
		{
			return (bracketIPv6 && uri.HostNameType == UriHostNameType.IPv6) ? uri.Host : uri.DnsSafeHost;
		}

		internal static string GetMessage(this CloseStatusCode code)
		{
			object result;
			switch (code)
			{
			default:
				result = string.Empty;
				break;
			case CloseStatusCode.TlsHandshakeFailure:
				result = "An error has occurred during a TLS handshake.";
				break;
			case CloseStatusCode.ServerError:
				result = "WebSocket server got an internal error.";
				break;
			case CloseStatusCode.MandatoryExtension:
				result = "WebSocket client didn't receive expected extension(s).";
				break;
			case CloseStatusCode.TooBig:
				result = "A too big message has been received.";
				break;
			case CloseStatusCode.PolicyViolation:
				result = "A policy violation has occurred.";
				break;
			case CloseStatusCode.InvalidData:
				result = "Invalid data has been received.";
				break;
			case CloseStatusCode.Abnormal:
				result = "An exception has occurred.";
				break;
			case CloseStatusCode.UnsupportedData:
				result = "Unsupported data has been received.";
				break;
			case CloseStatusCode.ProtocolError:
				result = "A WebSocket protocol error has occurred.";
				break;
			}
			return (string)result;
		}

		/// <summary>
		/// Gets the name from the specified string that contains a pair of
		/// name and value separated by a character.
		/// </summary>
		/// <returns>
		///   <para>
		///   A <see cref="T:System.String" /> that represents the name.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the name is not present.
		///   </para>
		/// </returns>
		/// <param name="nameAndValue">
		/// A <see cref="T:System.String" /> that contains a pair of name and value.
		/// </param>
		/// <param name="separator">
		/// A <see cref="T:System.Char" /> used to separate name and value.
		/// </param>
		internal static string GetName(this string nameAndValue, char separator)
		{
			int num = nameAndValue.IndexOf(separator);
			return (num > 0) ? nameAndValue.Substring(0, num).Trim() : null;
		}

		/// <summary>
		/// Gets the value from the specified string that contains a pair of
		/// name and value separated by a character.
		/// </summary>
		/// <returns>
		///   <para>
		///   A <see cref="T:System.String" /> that represents the value.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the value is not present.
		///   </para>
		/// </returns>
		/// <param name="nameAndValue">
		/// A <see cref="T:System.String" /> that contains a pair of name and value.
		/// </param>
		/// <param name="separator">
		/// A <see cref="T:System.Char" /> used to separate name and value.
		/// </param>
		internal static string GetValue(this string nameAndValue, char separator)
		{
			return nameAndValue.GetValue(separator, false);
		}

		/// <summary>
		/// Gets the value from the specified string that contains a pair of
		/// name and value separated by a character.
		/// </summary>
		/// <returns>
		///   <para>
		///   A <see cref="T:System.String" /> that represents the value.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the value is not present.
		///   </para>
		/// </returns>
		/// <param name="nameAndValue">
		/// A <see cref="T:System.String" /> that contains a pair of name and value.
		/// </param>
		/// <param name="separator">
		/// A <see cref="T:System.Char" /> used to separate name and value.
		/// </param>
		/// <param name="unquote">
		/// A <see cref="T:System.Boolean" />: <c>true</c> if unquotes the value; otherwise,
		/// <c>false</c>.
		/// </param>
		internal static string GetValue(this string nameAndValue, char separator, bool unquote)
		{
			int num = nameAndValue.IndexOf(separator);
			if (num < 0 || num == nameAndValue.Length - 1)
			{
				return null;
			}
			string text = nameAndValue.Substring(num + 1).Trim();
			return unquote ? text.Unquote() : text;
		}

		internal static byte[] InternalToByteArray(this ushort value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!order.IsHostOrder())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		internal static byte[] InternalToByteArray(this ulong value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!order.IsHostOrder())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		internal static bool IsCompressionExtension(this string value, CompressionMethod method)
		{
			return value.StartsWith(method.ToExtensionString());
		}

		internal static bool IsControl(this byte opcode)
		{
			return opcode > 7 && opcode < 16;
		}

		internal static bool IsControl(this Opcode opcode)
		{
			return (int)opcode >= 8;
		}

		internal static bool IsData(this byte opcode)
		{
			return opcode == 1 || opcode == 2;
		}

		internal static bool IsData(this Opcode opcode)
		{
			return opcode == Opcode.Text || opcode == Opcode.Binary;
		}

		internal static bool IsHttpMethod(this string value, Version version)
		{
			return (version == Sfs2X.WebSocketSharp.Net.HttpVersion.Version10) ? value.isHttpMethod10() : value.isHttpMethod();
		}

		internal static bool IsPortNumber(this int value)
		{
			return value > 0 && value < 65536;
		}

		internal static bool IsReserved(this ushort code)
		{
			return code == 1004 || code == 1005 || code == 1006 || code == 1015;
		}

		internal static bool IsReserved(this CloseStatusCode code)
		{
			return code == CloseStatusCode.Undefined || code == CloseStatusCode.NoStatus || code == CloseStatusCode.Abnormal || code == CloseStatusCode.TlsHandshakeFailure;
		}

		internal static bool IsSupported(this byte opcode)
		{
			return Enum.IsDefined(typeof(Opcode), opcode);
		}

		internal static bool IsText(this string value)
		{
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				if (c < ' ')
				{
					if ("\r\n\t".IndexOf(c) == -1)
					{
						return false;
					}
					if (c == '\n')
					{
						i++;
						if (i == length)
						{
							break;
						}
						c = value[i];
						if (" \t".IndexOf(c) == -1)
						{
							return false;
						}
					}
				}
				else if (c == '\u007f')
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsToken(this string value)
		{
			foreach (char c in value)
			{
				if (c < ' ')
				{
					return false;
				}
				if (c >= '\u007f')
				{
					return false;
				}
				if ("()<>@,;:\\\"/[]?={} \t".IndexOf(c) > -1)
				{
					return false;
				}
			}
			return true;
		}

		internal static bool KeepsAlive(this NameValueCollection headers, Version version)
		{
			StringComparison comparisonTypeForValue = StringComparison.OrdinalIgnoreCase;
			return (version < Sfs2X.WebSocketSharp.Net.HttpVersion.Version11) ? headers.Contains("Connection", "keep-alive", comparisonTypeForValue) : (!headers.Contains("Connection", "close", comparisonTypeForValue));
		}

		internal static string Quote(this string value)
		{
			return string.Format("\"{0}\"", value.Replace("\"", "\\\""));
		}

		internal static byte[] ReadBytes(this Stream stream, int length)
		{
			byte[] array = new byte[length];
			int num = 0;
			try
			{
				int num2 = 0;
				while (length > 0)
				{
					num2 = stream.Read(array, num, length);
					if (num2 == 0)
					{
						break;
					}
					num += num2;
					length -= num2;
				}
			}
			catch
			{
			}
			return array.SubArray(0, num);
		}

		internal static byte[] ReadBytes(this Stream stream, long length, int bufferLength)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				try
				{
					byte[] buffer = new byte[bufferLength];
					int num = 0;
					while (length > 0)
					{
						if (length < bufferLength)
						{
							bufferLength = (int)length;
						}
						num = stream.Read(buffer, 0, bufferLength);
						if (num == 0)
						{
							break;
						}
						memoryStream.Write(buffer, 0, num);
						length -= num;
					}
				}
				catch
				{
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		internal static void ReadBytesAsync(this Stream stream, int length, Action<byte[]> completed, Action<Exception> error)
		{
			byte[] buff = new byte[length];
			int offset = 0;
			int retry = 0;
			AsyncCallback callback = null;
			callback = delegate(IAsyncResult ar)
			{
				try
				{
					int num = stream.EndRead(ar);
					if (num == 0 && retry < _retry)
					{
						retry++;
						stream.BeginRead(buff, offset, length, callback, null);
					}
					else if (num == 0 || num == length)
					{
						if (completed != null)
						{
							completed(buff.SubArray(0, offset + num));
						}
					}
					else
					{
						retry = 0;
						offset += num;
						length -= num;
						stream.BeginRead(buff, offset, length, callback, null);
					}
				}
				catch (Exception obj2)
				{
					if (error != null)
					{
						error(obj2);
					}
				}
			};
			try
			{
				stream.BeginRead(buff, offset, length, callback, null);
			}
			catch (Exception obj)
			{
				if (error != null)
				{
					error(obj);
				}
			}
		}

		internal static void ReadBytesAsync(this Stream stream, long length, int bufferLength, Action<byte[]> completed, Action<Exception> error)
		{
			MemoryStream dest = new MemoryStream();
			byte[] buff = new byte[bufferLength];
			int retry = 0;
			Action<long> read = null;
			read = delegate(long len)
			{
				if (len < bufferLength)
				{
					bufferLength = (int)len;
				}
				stream.BeginRead(buff, 0, bufferLength, delegate(IAsyncResult ar)
				{
					try
					{
						int num = stream.EndRead(ar);
						if (num > 0)
						{
							dest.Write(buff, 0, num);
						}
						if (num == 0 && retry < _retry)
						{
							int num2 = retry;
							retry = num2 + 1;
							read(len);
						}
						else if (num == 0 || num == len)
						{
							if (completed != null)
							{
								dest.Close();
								completed(dest.ToArray());
							}
							dest.Dispose();
						}
						else
						{
							retry = 0;
							read(len - num);
						}
					}
					catch (Exception obj2)
					{
						dest.Dispose();
						if (error != null)
						{
							error(obj2);
						}
					}
				}, null);
			};
			try
			{
				read(length);
			}
			catch (Exception obj)
			{
				dest.Dispose();
				if (error != null)
				{
					error(obj);
				}
			}
		}

		internal static T[] Reverse<T>(this T[] array)
		{
			int num = array.Length;
			T[] array2 = new T[num];
			int num2 = num - 1;
			for (int i = 0; i <= num2; i++)
			{
				array2[i] = array[num2 - i];
			}
			return array2;
		}

		internal static IEnumerable<string> SplitHeaderValue(this string value, params char[] separators)
		{
			int len = value.Length;
			StringBuilder buff = new StringBuilder(32);
			int end = len - 1;
			bool escaped = false;
			bool quoted = false;
			for (int i = 0; i <= end; i++)
			{
				char c = value[i];
				buff.Append(c);
				switch (c)
				{
				case '"':
					if (escaped)
					{
						escaped = false;
					}
					else
					{
						quoted = !quoted;
					}
					continue;
				case '\\':
					if (i == end)
					{
						break;
					}
					if (value[i + 1] == '"')
					{
						escaped = true;
					}
					continue;
				default:
					if (Array.IndexOf(separators, c) > -1 && !quoted)
					{
						buff.Length--;
						yield return buff.ToString();
						buff.Length = 0;
					}
					continue;
				}
				break;
			}
			yield return buff.ToString();
		}

		internal static byte[] ToByteArray(this Stream stream)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.Position = 0L;
				stream.CopyTo(memoryStream, 1024);
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		internal static CompressionMethod ToCompressionMethod(this string value)
		{
			foreach (CompressionMethod value2 in Enum.GetValues(typeof(CompressionMethod)))
			{
				if (value2.ToExtensionString() == value)
				{
					return value2;
				}
			}
			return CompressionMethod.None;
		}

		internal static string ToExtensionString(this CompressionMethod method, params string[] parameters)
		{
			if (method == CompressionMethod.None)
			{
				return string.Empty;
			}
			string text = string.Format("permessage-{0}", method.ToString().ToLower());
			return (parameters != null && parameters.Length != 0) ? string.Format("{0}; {1}", text, parameters.ToString("; ")) : text;
		}

		internal static IPAddress ToIPAddress(this string value)
		{
			if (value == null || value.Length == 0)
			{
				return null;
			}
			IPAddress address;
			if (IPAddress.TryParse(value, out address))
			{
				return address;
			}
			try
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(value);
				return hostAddresses[0];
			}
			catch
			{
				return null;
			}
		}

		internal static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			return new List<TSource>(source);
		}

		internal static string ToString(this IPAddress address, bool bracketIPv6)
		{
			return (bracketIPv6 && address.AddressFamily == AddressFamily.InterNetworkV6) ? string.Format("[{0}]", address.ToString()) : address.ToString();
		}

		internal static ushort ToUInt16(this byte[] source, ByteOrder sourceOrder)
		{
			return BitConverter.ToUInt16(source.ToHostOrder(sourceOrder), 0);
		}

		internal static ulong ToUInt64(this byte[] source, ByteOrder sourceOrder)
		{
			return BitConverter.ToUInt64(source.ToHostOrder(sourceOrder), 0);
		}

		internal static IEnumerable<string> Trim(this IEnumerable<string> source)
		{
			foreach (string elm in source)
			{
				yield return elm.Trim();
			}
		}

		internal static string TrimSlashFromEnd(this string value)
		{
			string text = value.TrimEnd('/');
			return (text.Length > 0) ? text : "/";
		}

		internal static string TrimSlashOrBackslashFromEnd(this string value)
		{
			string text = value.TrimEnd('/', '\\');
			return (text.Length > 0) ? text : value[0].ToString();
		}

		internal static bool TryCreateVersion(this string versionString, out Version result)
		{
			result = null;
			try
			{
				result = new Version(versionString);
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Tries to create a new <see cref="T:System.Uri" /> for WebSocket with
		/// the specified <paramref name="uriString" />.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the <see cref="T:System.Uri" /> was successfully created;
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <param name="uriString">
		/// A <see cref="T:System.String" /> that represents a WebSocket URL to try.
		/// </param>
		/// <param name="result">
		/// When this method returns, a <see cref="T:System.Uri" /> that
		/// represents the WebSocket URL or <see langword="null" />
		/// if <paramref name="uriString" /> is invalid.
		/// </param>
		/// <param name="message">
		/// When this method returns, a <see cref="T:System.String" /> that
		/// represents an error message or <see langword="null" />
		/// if <paramref name="uriString" /> is valid.
		/// </param>
		internal static bool TryCreateWebSocketUri(this string uriString, out Uri result, out string message)
		{
			result = null;
			message = null;
			Uri uri = uriString.ToUri();
			if (uri == null)
			{
				message = "An invalid URI string.";
				return false;
			}
			if (!uri.IsAbsoluteUri)
			{
				message = "A relative URI.";
				return false;
			}
			string scheme = uri.Scheme;
			if (!(scheme == "ws") && !(scheme == "wss"))
			{
				message = "The scheme part is not 'ws' or 'wss'.";
				return false;
			}
			int port = uri.Port;
			if (port == 0)
			{
				message = "The port part is zero.";
				return false;
			}
			if (uri.Fragment.Length > 0)
			{
				message = "It includes the fragment component.";
				return false;
			}
			result = ((port != -1) ? uri : new Uri(string.Format("{0}://{1}:{2}{3}", scheme, uri.Host, (scheme == "ws") ? 80 : 443, uri.PathAndQuery)));
			return true;
		}

		internal static bool TryGetUTF8DecodedString(this byte[] bytes, out string s)
		{
			s = null;
			try
			{
				s = Encoding.UTF8.GetString(bytes);
			}
			catch
			{
				return false;
			}
			return true;
		}

		internal static bool TryGetUTF8EncodedBytes(this string s, out byte[] bytes)
		{
			bytes = null;
			try
			{
				bytes = Encoding.UTF8.GetBytes(s);
			}
			catch
			{
				return false;
			}
			return true;
		}

		internal static bool TryOpenRead(this FileInfo fileInfo, out FileStream fileStream)
		{
			fileStream = null;
			try
			{
				fileStream = fileInfo.OpenRead();
			}
			catch
			{
				return false;
			}
			return true;
		}

		internal static string Unquote(this string value)
		{
			int num = value.IndexOf('"');
			if (num == -1)
			{
				return value;
			}
			int num2 = value.LastIndexOf('"');
			if (num2 == num)
			{
				return value;
			}
			int num3 = num2 - num - 1;
			return (num3 > 0) ? value.Substring(num + 1, num3).Replace("\\\"", "\"") : string.Empty;
		}

		internal static bool Upgrades(this NameValueCollection headers, string protocol)
		{
			StringComparison comparisonTypeForValue = StringComparison.OrdinalIgnoreCase;
			return headers.Contains("Upgrade", protocol, comparisonTypeForValue) && headers.Contains("Connection", "Upgrade", comparisonTypeForValue);
		}

		internal static string UTF8Decode(this byte[] bytes)
		{
			try
			{
				return Encoding.UTF8.GetString(bytes);
			}
			catch
			{
				return null;
			}
		}

		internal static byte[] UTF8Encode(this string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		internal static void WriteBytes(this Stream stream, byte[] bytes, int bufferLength)
		{
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				memoryStream.CopyTo(stream, bufferLength);
			}
		}

		internal static void WriteBytesAsync(this Stream stream, byte[] bytes, int bufferLength, Action completed, Action<Exception> error)
		{
			MemoryStream input = new MemoryStream(bytes);
			input.CopyToAsync(stream, bufferLength, delegate
			{
				if (completed != null)
				{
					completed();
				}
				input.Dispose();
			}, delegate(Exception ex)
			{
				input.Dispose();
				if (error != null)
				{
					error(ex);
				}
			});
		}

		/// <summary>
		/// Emits the specified <see cref="T:System.EventHandler" /> delegate if it isn't <see langword="null" />.
		/// </summary>
		/// <param name="eventHandler">
		/// A <see cref="T:System.EventHandler" /> to emit.
		/// </param>
		/// <param name="sender">
		/// An <see cref="T:System.Object" /> from which emits this <paramref name="eventHandler" />.
		/// </param>
		/// <param name="e">
		/// A <see cref="T:System.EventArgs" /> that contains no event data.
		/// </param>
		public static void Emit(this EventHandler eventHandler, object sender, EventArgs e)
		{
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		/// <summary>
		/// Emits the specified <c>EventHandler&lt;TEventArgs&gt;</c> delegate if it isn't
		/// <see langword="null" />.
		/// </summary>
		/// <param name="eventHandler">
		/// An <c>EventHandler&lt;TEventArgs&gt;</c> to emit.
		/// </param>
		/// <param name="sender">
		/// An <see cref="T:System.Object" /> from which emits this <paramref name="eventHandler" />.
		/// </param>
		/// <param name="e">
		/// A <c>TEventArgs</c> that represents the event data.
		/// </param>
		/// <typeparam name="TEventArgs">
		/// The type of the event data generated by the event.
		/// </typeparam>
		public static void Emit<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e) where TEventArgs : EventArgs
		{
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		/// <summary>
		/// Gets the description of the specified HTTP status <paramref name="code" />.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents the description of the HTTP status code.
		/// </returns>
		/// <param name="code">
		/// One of <see cref="T:Sfs2X.WebSocketSharp.Net.HttpStatusCode" /> enum values, indicates the HTTP status code.
		/// </param>
		public static string GetDescription(this Sfs2X.WebSocketSharp.Net.HttpStatusCode code)
		{
			return ((int)code).GetStatusDescription();
		}

		/// <summary>
		/// Gets the description of the specified HTTP status <paramref name="code" />.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents the description of the HTTP status code.
		/// </returns>
		/// <param name="code">
		/// An <see cref="T:System.Int32" /> that represents the HTTP status code.
		/// </param>
		public static string GetStatusDescription(this int code)
		{
			switch (code)
			{
			case 100:
				return "Continue";
			case 101:
				return "Switching Protocols";
			case 102:
				return "Processing";
			case 200:
				return "OK";
			case 201:
				return "Created";
			case 202:
				return "Accepted";
			case 203:
				return "Non-Authoritative Information";
			case 204:
				return "No Content";
			case 205:
				return "Reset Content";
			case 206:
				return "Partial Content";
			case 207:
				return "Multi-Status";
			case 300:
				return "Multiple Choices";
			case 301:
				return "Moved Permanently";
			case 302:
				return "Found";
			case 303:
				return "See Other";
			case 304:
				return "Not Modified";
			case 305:
				return "Use Proxy";
			case 307:
				return "Temporary Redirect";
			case 400:
				return "Bad Request";
			case 401:
				return "Unauthorized";
			case 402:
				return "Payment Required";
			case 403:
				return "Forbidden";
			case 404:
				return "Not Found";
			case 405:
				return "Method Not Allowed";
			case 406:
				return "Not Acceptable";
			case 407:
				return "Proxy Authentication Required";
			case 408:
				return "Request Timeout";
			case 409:
				return "Conflict";
			case 410:
				return "Gone";
			case 411:
				return "Length Required";
			case 412:
				return "Precondition Failed";
			case 413:
				return "Request Entity Too Large";
			case 414:
				return "Request-Uri Too Long";
			case 415:
				return "Unsupported Media Type";
			case 416:
				return "Requested Range Not Satisfiable";
			case 417:
				return "Expectation Failed";
			case 422:
				return "Unprocessable Entity";
			case 423:
				return "Locked";
			case 424:
				return "Failed Dependency";
			case 500:
				return "Internal Server Error";
			case 501:
				return "Not Implemented";
			case 502:
				return "Bad Gateway";
			case 503:
				return "Service Unavailable";
			case 504:
				return "Gateway Timeout";
			case 505:
				return "Http Version Not Supported";
			case 507:
				return "Insufficient Storage";
			default:
				return string.Empty;
			}
		}

		/// <summary>
		/// Determines whether the specified ushort is in the range of
		/// the status code for the WebSocket connection close.
		/// </summary>
		/// <remarks>
		///   <para>
		///   The ranges are the following:
		///   </para>
		///   <list type="bullet">
		///     <item>
		///       <term>
		///       1000-2999: These numbers are reserved for definition by
		///       the WebSocket protocol.
		///       </term>
		///     </item>
		///     <item>
		///       <term>
		///       3000-3999: These numbers are reserved for use by libraries,
		///       frameworks, and applications.
		///       </term>
		///     </item>
		///     <item>
		///       <term>
		///       4000-4999: These numbers are reserved for private use.
		///       </term>
		///     </item>
		///   </list>
		/// </remarks>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> is in the range of
		/// the status code for the close; otherwise, <c>false</c>.
		/// </returns>
		/// <param name="value">
		/// A <see cref="T:System.UInt16" /> to test.
		/// </param>
		public static bool IsCloseStatusCode(this ushort value)
		{
			return value > 999 && value < 5000;
		}

		/// <summary>
		/// Determines whether the specified string is enclosed in
		/// the specified character.
		/// </summary>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> is enclosed in
		/// <paramref name="c" />; otherwise, <c>false</c>.
		/// </returns>
		/// <param name="value">
		/// A <see cref="T:System.String" /> to test.
		/// </param>
		/// <param name="c">
		/// A <see cref="T:System.Char" /> to find.
		/// </param>
		public static bool IsEnclosedIn(this string value, char c)
		{
			return value != null && value.Length > 1 && value[0] == c && value[value.Length - 1] == c;
		}

		/// <summary>
		/// Determines whether the specified byte order is host (this computer
		/// architecture) byte order.
		/// </summary>
		/// <returns>
		/// <c>true</c> if <paramref name="order" /> is host byte order; otherwise,
		/// <c>false</c>.
		/// </returns>
		/// <param name="order">
		/// One of the <see cref="T:Sfs2X.WebSocketSharp.ByteOrder" /> enum values to test.
		/// </param>
		public static bool IsHostOrder(this ByteOrder order)
		{
			return BitConverter.IsLittleEndian == (order == ByteOrder.Little);
		}

		/// <summary>
		/// Determines whether the specified IP address is a local IP address.
		/// </summary>
		/// <remarks>
		/// This local means NOT REMOTE for the current host.
		/// </remarks>
		/// <returns>
		/// <c>true</c> if <paramref name="address" /> is a local IP address;
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <param name="address">
		/// A <see cref="T:System.Net.IPAddress" /> to test.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="address" /> is <see langword="null" />.
		/// </exception>
		public static bool IsLocal(this IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Equals(IPAddress.Any))
			{
				return true;
			}
			if (address.Equals(IPAddress.Loopback))
			{
				return true;
			}
			if (Socket.OSSupportsIPv6)
			{
				if (address.Equals(IPAddress.IPv6Any))
				{
					return true;
				}
				if (address.Equals(IPAddress.IPv6Loopback))
				{
					return true;
				}
			}
			string hostName = Dns.GetHostName();
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
			IPAddress[] array = hostAddresses;
			foreach (IPAddress obj in array)
			{
				if (address.Equals(obj))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines whether the specified string is <see langword="null" /> or
		/// an empty string.
		/// </summary>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> is <see langword="null" /> or
		/// an empty string; otherwise, <c>false</c>.
		/// </returns>
		/// <param name="value">
		/// A <see cref="T:System.String" /> to test.
		/// </param>
		public static bool IsNullOrEmpty(this string value)
		{
			return value == null || value.Length == 0;
		}

		/// <summary>
		/// Determines whether the specified string is a predefined scheme.
		/// </summary>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> is a predefined scheme;
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <param name="value">
		/// A <see cref="T:System.String" /> to test.
		/// </param>
		public static bool IsPredefinedScheme(this string value)
		{
			if (value == null || value.Length < 2)
			{
				return false;
			}
			switch (value[0])
			{
			case 'h':
				return value == "http" || value == "https";
			case 'w':
				return value == "ws" || value == "wss";
			case 'f':
				return value == "file" || value == "ftp";
			case 'g':
				return value == "gopher";
			case 'm':
				return value == "mailto";
			case 'n':
			{
				char c = value[1];
				return (c != 'e') ? (value == "nntp") : (value == "news" || value == "net.pipe" || value == "net.tcp");
			}
			default:
				return false;
			}
		}

		/// <summary>
		/// Determines whether the specified string is a URI string.
		/// </summary>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> may be a URI string;
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <param name="value">
		/// A <see cref="T:System.String" /> to test.
		/// </param>
		public static bool MaybeUri(this string value)
		{
			if (value == null || value.Length == 0)
			{
				return false;
			}
			int num = value.IndexOf(':');
			if (num == -1)
			{
				return false;
			}
			if (num >= 10)
			{
				return false;
			}
			string value2 = value.Substring(0, num);
			return value2.IsPredefinedScheme();
		}

		/// <summary>
		/// Retrieves a sub-array from the specified <paramref name="array" />. A sub-array starts at
		/// the specified element position in <paramref name="array" />.
		/// </summary>
		/// <returns>
		/// An array of T that receives a sub-array, or an empty array of T if any problems with
		/// the parameters.
		/// </returns>
		/// <param name="array">
		/// An array of T from which to retrieve a sub-array.
		/// </param>
		/// <param name="startIndex">
		/// An <see cref="T:System.Int32" /> that represents the zero-based starting position of
		/// a sub-array in <paramref name="array" />.
		/// </param>
		/// <param name="length">
		/// An <see cref="T:System.Int32" /> that represents the number of elements to retrieve.
		/// </param>
		/// <typeparam name="T">
		/// The type of elements in <paramref name="array" />.
		/// </typeparam>
		public static T[] SubArray<T>(this T[] array, int startIndex, int length)
		{
			int num;
			if (array == null || (num = array.Length) == 0)
			{
				return new T[0];
			}
			if (startIndex < 0 || length <= 0 || startIndex + length > num)
			{
				return new T[0];
			}
			if (startIndex == 0 && length == num)
			{
				return array;
			}
			T[] array2 = new T[length];
			Array.Copy(array, startIndex, array2, 0, length);
			return array2;
		}

		/// <summary>
		/// Retrieves a sub-array from the specified <paramref name="array" />. A sub-array starts at
		/// the specified element position in <paramref name="array" />.
		/// </summary>
		/// <returns>
		/// An array of T that receives a sub-array, or an empty array of T if any problems with
		/// the parameters.
		/// </returns>
		/// <param name="array">
		/// An array of T from which to retrieve a sub-array.
		/// </param>
		/// <param name="startIndex">
		/// A <see cref="T:System.Int64" /> that represents the zero-based starting position of
		/// a sub-array in <paramref name="array" />.
		/// </param>
		/// <param name="length">
		/// A <see cref="T:System.Int64" /> that represents the number of elements to retrieve.
		/// </param>
		/// <typeparam name="T">
		/// The type of elements in <paramref name="array" />.
		/// </typeparam>
		public static T[] SubArray<T>(this T[] array, long startIndex, long length)
		{
			long num;
			if (array == null || (num = array.LongLength) == 0)
			{
				return new T[0];
			}
			if (startIndex < 0 || length <= 0 || startIndex + length > num)
			{
				return new T[0];
			}
			if (startIndex == 0L && length == num)
			{
				return array;
			}
			T[] array2 = new T[length];
			Array.Copy(array, startIndex, array2, 0L, length);
			return array2;
		}

		/// <summary>
		/// Executes the specified <see cref="T:System.Action" /> delegate <paramref name="n" /> times.
		/// </summary>
		/// <param name="n">
		/// An <see cref="T:System.Int32" /> is the number of times to execute.
		/// </param>
		/// <param name="action">
		/// An <see cref="T:System.Action" /> delegate that references the method(s) to execute.
		/// </param>
		public static void Times(this int n, Action action)
		{
			if (n > 0 && action != null)
			{
				((ulong)n).times(action);
			}
		}

		/// <summary>
		/// Executes the specified <see cref="T:System.Action" /> delegate <paramref name="n" /> times.
		/// </summary>
		/// <param name="n">
		/// A <see cref="T:System.Int64" /> is the number of times to execute.
		/// </param>
		/// <param name="action">
		/// An <see cref="T:System.Action" /> delegate that references the method(s) to execute.
		/// </param>
		public static void Times(this long n, Action action)
		{
			if (n > 0 && action != null)
			{
				((ulong)n).times(action);
			}
		}

		/// <summary>
		/// Executes the specified <see cref="T:System.Action" /> delegate <paramref name="n" /> times.
		/// </summary>
		/// <param name="n">
		/// A <see cref="T:System.UInt32" /> is the number of times to execute.
		/// </param>
		/// <param name="action">
		/// An <see cref="T:System.Action" /> delegate that references the method(s) to execute.
		/// </param>
		public static void Times(this uint n, Action action)
		{
			if (n != 0 && action != null)
			{
				times(n, action);
			}
		}

		/// <summary>
		/// Executes the specified <see cref="T:System.Action" /> delegate <paramref name="n" /> times.
		/// </summary>
		/// <param name="n">
		/// A <see cref="T:System.UInt64" /> is the number of times to execute.
		/// </param>
		/// <param name="action">
		/// An <see cref="T:System.Action" /> delegate that references the method(s) to execute.
		/// </param>
		public static void Times(this ulong n, Action action)
		{
			if (n != 0 && action != null)
			{
				n.times(action);
			}
		}

		/// <summary>
		/// Executes the specified <c>Action&lt;int&gt;</c> delegate <paramref name="n" /> times.
		/// </summary>
		/// <param name="n">
		/// An <see cref="T:System.Int32" /> is the number of times to execute.
		/// </param>
		/// <param name="action">
		/// An <c>Action&lt;int&gt;</c> delegate that references the method(s) to execute.
		/// An <see cref="T:System.Int32" /> parameter to pass to the method(s) is the zero-based count of
		/// iteration.
		/// </param>
		public static void Times(this int n, Action<int> action)
		{
			if (n > 0 && action != null)
			{
				for (int i = 0; i < n; i++)
				{
					action(i);
				}
			}
		}

		/// <summary>
		/// Executes the specified <c>Action&lt;long&gt;</c> delegate <paramref name="n" /> times.
		/// </summary>
		/// <param name="n">
		/// A <see cref="T:System.Int64" /> is the number of times to execute.
		/// </param>
		/// <param name="action">
		/// An <c>Action&lt;long&gt;</c> delegate that references the method(s) to execute.
		/// A <see cref="T:System.Int64" /> parameter to pass to the method(s) is the zero-based count of
		/// iteration.
		/// </param>
		public static void Times(this long n, Action<long> action)
		{
			if (n > 0 && action != null)
			{
				for (long num = 0L; num < n; num++)
				{
					action(num);
				}
			}
		}

		/// <summary>
		/// Executes the specified <c>Action&lt;uint&gt;</c> delegate <paramref name="n" /> times.
		/// </summary>
		/// <param name="n">
		/// A <see cref="T:System.UInt32" /> is the number of times to execute.
		/// </param>
		/// <param name="action">
		/// An <c>Action&lt;uint&gt;</c> delegate that references the method(s) to execute.
		/// A <see cref="T:System.UInt32" /> parameter to pass to the method(s) is the zero-based count of
		/// iteration.
		/// </param>
		public static void Times(this uint n, Action<uint> action)
		{
			if (n != 0 && action != null)
			{
				for (uint num = 0u; num < n; num++)
				{
					action(num);
				}
			}
		}

		/// <summary>
		/// Executes the specified <c>Action&lt;ulong&gt;</c> delegate <paramref name="n" /> times.
		/// </summary>
		/// <param name="n">
		/// A <see cref="T:System.UInt64" /> is the number of times to execute.
		/// </param>
		/// <param name="action">
		/// An <c>Action&lt;ulong&gt;</c> delegate that references the method(s) to execute.
		/// A <see cref="T:System.UInt64" /> parameter to pass to this method(s) is the zero-based count of
		/// iteration.
		/// </param>
		public static void Times(this ulong n, Action<ulong> action)
		{
			if (n != 0 && action != null)
			{
				for (ulong num = 0uL; num < n; num++)
				{
					action(num);
				}
			}
		}

		/// <summary>
		/// Converts the specified array of <see cref="T:System.Byte" /> to the specified type data.
		/// </summary>
		/// <returns>
		/// A T converted from <paramref name="source" />, or a default value of
		/// T if <paramref name="source" /> is an empty array of <see cref="T:System.Byte" /> or
		/// if the type of T isn't <see cref="T:System.Boolean" />, <see cref="T:System.Char" />, <see cref="T:System.Double" />,
		/// <see cref="T:System.Single" />, <see cref="T:System.Int32" />, <see cref="T:System.Int64" />, <see cref="T:System.Int16" />,
		/// <see cref="T:System.UInt32" />, <see cref="T:System.UInt64" />, or <see cref="T:System.UInt16" />.
		/// </returns>
		/// <param name="source">
		/// An array of <see cref="T:System.Byte" /> to convert.
		/// </param>
		/// <param name="sourceOrder">
		/// One of the <see cref="T:Sfs2X.WebSocketSharp.ByteOrder" /> enum values, specifies the byte order of
		/// <paramref name="source" />.
		/// </param>
		/// <typeparam name="T">
		/// The type of the return. The T must be a value type.
		/// </typeparam>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="source" /> is <see langword="null" />.
		/// </exception>
		public static T To<T>(this byte[] source, ByteOrder sourceOrder) where T : struct
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.Length == 0)
			{
				return default(T);
			}
			Type typeFromHandle = typeof(T);
			byte[] value = source.ToHostOrder(sourceOrder);
			return (typeFromHandle == typeof(bool)) ? ((T)(object)BitConverter.ToBoolean(value, 0)) : ((typeFromHandle == typeof(char)) ? ((T)(object)BitConverter.ToChar(value, 0)) : ((typeFromHandle == typeof(double)) ? ((T)(object)BitConverter.ToDouble(value, 0)) : ((typeFromHandle == typeof(short)) ? ((T)(object)BitConverter.ToInt16(value, 0)) : ((typeFromHandle == typeof(int)) ? ((T)(object)BitConverter.ToInt32(value, 0)) : ((typeFromHandle == typeof(long)) ? ((T)(object)BitConverter.ToInt64(value, 0)) : ((typeFromHandle == typeof(float)) ? ((T)(object)BitConverter.ToSingle(value, 0)) : ((typeFromHandle == typeof(ushort)) ? ((T)(object)BitConverter.ToUInt16(value, 0)) : ((typeFromHandle == typeof(uint)) ? ((T)(object)BitConverter.ToUInt32(value, 0)) : ((typeFromHandle == typeof(ulong)) ? ((T)(object)BitConverter.ToUInt64(value, 0)) : default(T))))))))));
		}

		/// <summary>
		/// Converts the specified <paramref name="value" /> to an array of <see cref="T:System.Byte" />.
		/// </summary>
		/// <returns>
		/// An array of <see cref="T:System.Byte" /> converted from <paramref name="value" />.
		/// </returns>
		/// <param name="value">
		/// A T to convert.
		/// </param>
		/// <param name="order">
		/// One of the <see cref="T:Sfs2X.WebSocketSharp.ByteOrder" /> enum values, specifies the byte order of the return.
		/// </param>
		/// <typeparam name="T">
		/// The type of <paramref name="value" />. The T must be a value type.
		/// </typeparam>
		public static byte[] ToByteArray<T>(this T value, ByteOrder order) where T : struct
		{
			Type typeFromHandle = typeof(T);
			byte[] array = ((typeFromHandle == typeof(bool)) ? BitConverter.GetBytes((bool)(object)value) : ((!(typeFromHandle == typeof(byte))) ? ((typeFromHandle == typeof(char)) ? BitConverter.GetBytes((char)(object)value) : ((typeFromHandle == typeof(double)) ? BitConverter.GetBytes((double)(object)value) : ((typeFromHandle == typeof(short)) ? BitConverter.GetBytes((short)(object)value) : ((typeFromHandle == typeof(int)) ? BitConverter.GetBytes((int)(object)value) : ((typeFromHandle == typeof(long)) ? BitConverter.GetBytes((long)(object)value) : ((typeFromHandle == typeof(float)) ? BitConverter.GetBytes((float)(object)value) : ((typeFromHandle == typeof(ushort)) ? BitConverter.GetBytes((ushort)(object)value) : ((typeFromHandle == typeof(uint)) ? BitConverter.GetBytes((uint)(object)value) : ((typeFromHandle == typeof(ulong)) ? BitConverter.GetBytes((ulong)(object)value) : WebSocket.EmptyBytes))))))))) : new byte[1] { (byte)(object)value }));
			if (array.Length > 1 && !order.IsHostOrder())
			{
				Array.Reverse(array);
			}
			return array;
		}

		/// <summary>
		/// Converts the order of elements in the specified byte array to
		/// host (this computer architecture) byte order.
		/// </summary>
		/// <returns>
		///   <para>
		///   An array of <see cref="T:System.Byte" /> converted from
		///   <paramref name="source" />.
		///   </para>
		///   <para>
		///   Or <paramref name="source" /> if the number of elements in it
		///   is less than 2 or <paramref name="sourceOrder" /> is same as
		///   host byte order.
		///   </para>
		/// </returns>
		/// <param name="source">
		/// An array of <see cref="T:System.Byte" /> to convert.
		/// </param>
		/// <param name="sourceOrder">
		///   <para>
		///   One of the <see cref="T:Sfs2X.WebSocketSharp.ByteOrder" /> enum values.
		///   </para>
		///   <para>
		///   It specifies the order of elements in <paramref name="source" />.
		///   </para>
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="source" /> is <see langword="null" />.
		/// </exception>
		public static byte[] ToHostOrder(this byte[] source, ByteOrder sourceOrder)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.Length < 2)
			{
				return source;
			}
			return (!sourceOrder.IsHostOrder()) ? source.Reverse() : source;
		}

		/// <summary>
		/// Converts the specified array to a <see cref="T:System.String" />.
		/// </summary>
		/// <returns>
		///   <para>
		///   A <see cref="T:System.String" /> converted by concatenating each element of
		///   <paramref name="array" /> across <paramref name="separator" />.
		///   </para>
		///   <para>
		///   An empty string if <paramref name="array" /> is an empty array.
		///   </para>
		/// </returns>
		/// <param name="array">
		/// An array of T to convert.
		/// </param>
		/// <param name="separator">
		/// A <see cref="T:System.String" /> used to separate each element of
		/// <paramref name="array" />.
		/// </param>
		/// <typeparam name="T">
		/// The type of elements in <paramref name="array" />.
		/// </typeparam>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="array" /> is <see langword="null" />.
		/// </exception>
		public static string ToString<T>(this T[] array, string separator)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int num = array.Length;
			if (num == 0)
			{
				return string.Empty;
			}
			if (separator == null)
			{
				separator = string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			for (int i = 0; i < num - 1; i++)
			{
				stringBuilder.AppendFormat("{0}{1}", array[i], separator);
			}
			stringBuilder.Append(array[num - 1].ToString());
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Converts the specified string to a <see cref="T:System.Uri" />.
		/// </summary>
		/// <returns>
		///   <para>
		///   A <see cref="T:System.Uri" /> converted from <paramref name="value" />.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the conversion has failed.
		///   </para>
		/// </returns>
		/// <param name="value">
		/// A <see cref="T:System.String" /> to convert.
		/// </param>
		public static Uri ToUri(this string value)
		{
			Uri result;
			Uri.TryCreate(value, value.MaybeUri() ? UriKind.Absolute : UriKind.Relative, out result);
			return result;
		}

		/// <summary>
		/// URL-decodes the specified <see cref="T:System.String" />.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that receives the decoded string or
		/// <paramref name="value" /> if it is <see langword="null" /> or empty.
		/// </returns>
		/// <param name="value">
		/// A <see cref="T:System.String" /> to decode.
		/// </param>
		public static string UrlDecode(this string value)
		{
			return (value != null && value.Length > 0) ? HttpUtility.UrlDecode(value) : value;
		}

		/// <summary>
		/// URL-encodes the specified <see cref="T:System.String" />.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that receives the encoded string or
		/// <paramref name="value" /> if it is <see langword="null" /> or empty.
		/// </returns>
		/// <param name="value">
		/// A <see cref="T:System.String" /> to encode.
		/// </param>
		public static string UrlEncode(this string value)
		{
			return (value != null && value.Length > 0) ? HttpUtility.UrlEncode(value) : value;
		}

		/// <summary>
		/// Writes and sends the specified <paramref name="content" /> data with the specified
		/// <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerResponse" />.
		/// </summary>
		/// <param name="response">
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerResponse" /> that represents the HTTP response used to
		/// send the content data.
		/// </param>
		/// <param name="content">
		/// An array of <see cref="T:System.Byte" /> that represents the content data to send.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <para>
		///   <paramref name="response" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="content" /> is <see langword="null" />.
		///   </para>
		/// </exception>
		public static void WriteContent(this Sfs2X.WebSocketSharp.Net.HttpListenerResponse response, byte[] content)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			long num = content.LongLength;
			if (num == 0)
			{
				response.Close();
				return;
			}
			response.ContentLength64 = num;
			Stream outputStream = response.OutputStream;
			if (num <= int.MaxValue)
			{
				outputStream.Write(content, 0, (int)num);
			}
			else
			{
				outputStream.WriteBytes(content, 1024);
			}
			outputStream.Close();
		}
	}
}
