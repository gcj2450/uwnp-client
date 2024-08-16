using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;

namespace Sfs2X.WebSocketSharp.Server
{
	/// <summary>
	/// Provides the management function for the sessions in a WebSocket service.
	/// </summary>
	/// <remarks>
	/// This class manages the sessions in a WebSocket service provided by
	/// the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketServer" /> or <see cref="T:Sfs2X.WebSocketSharp.Server.HttpServer" />.
	/// </remarks>
	public class WebSocketSessionManager
	{
		private volatile bool _clean;

		private object _forSweep;

		private Logger _log;

		private Dictionary<string, IWebSocketSession> _sessions;

		private volatile ServerState _state;

		private volatile bool _sweeping;

		private System.Timers.Timer _sweepTimer;

		private object _sync;

		private TimeSpan _waitTime;

		internal ServerState State
		{
			get
			{
				return _state;
			}
		}

		/// <summary>
		/// Gets the IDs for the active sessions in the WebSocket service.
		/// </summary>
		/// <value>
		///   <para>
		///   An <c>IEnumerable&lt;string&gt;</c> instance.
		///   </para>
		///   <para>
		///   It provides an enumerator which supports the iteration over
		///   the collection of the IDs for the active sessions.
		///   </para>
		/// </value>
		public IEnumerable<string> ActiveIDs
		{
			get
			{
				foreach (KeyValuePair<string, bool> res in broadping(WebSocketFrame.EmptyPingBytes))
				{
					if (res.Value)
					{
						yield return res.Key;
					}
				}
			}
		}

		/// <summary>
		/// Gets the number of the sessions in the WebSocket service.
		/// </summary>
		/// <value>
		/// An <see cref="T:System.Int32" /> that represents the number of the sessions.
		/// </value>
		public int Count
		{
			get
			{
				lock (_sync)
				{
					return _sessions.Count;
				}
			}
		}

		/// <summary>
		/// Gets the IDs for the sessions in the WebSocket service.
		/// </summary>
		/// <value>
		///   <para>
		///   An <c>IEnumerable&lt;string&gt;</c> instance.
		///   </para>
		///   <para>
		///   It provides an enumerator which supports the iteration over
		///   the collection of the IDs for the sessions.
		///   </para>
		/// </value>
		public IEnumerable<string> IDs
		{
			get
			{
				if (_state != ServerState.Start)
				{
					return Enumerable.Empty<string>();
				}
				lock (_sync)
				{
					if (_state != ServerState.Start)
					{
						return Enumerable.Empty<string>();
					}
					return _sessions.Keys.ToList();
				}
			}
		}

		/// <summary>
		/// Gets the IDs for the inactive sessions in the WebSocket service.
		/// </summary>
		/// <value>
		///   <para>
		///   An <c>IEnumerable&lt;string&gt;</c> instance.
		///   </para>
		///   <para>
		///   It provides an enumerator which supports the iteration over
		///   the collection of the IDs for the inactive sessions.
		///   </para>
		/// </value>
		public IEnumerable<string> InactiveIDs
		{
			get
			{
				foreach (KeyValuePair<string, bool> res in broadping(WebSocketFrame.EmptyPingBytes))
				{
					if (!res.Value)
					{
						yield return res.Key;
					}
				}
			}
		}

		/// <summary>
		/// Gets the session instance with <paramref name="id" />.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:Sfs2X.WebSocketSharp.Server.IWebSocketSession" /> instance or <see langword="null" />
		///   if not found.
		///   </para>
		///   <para>
		///   The session instance provides the function to access the information
		///   in the session.
		///   </para>
		/// </value>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session to find.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="id" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="id" /> is an empty string.
		/// </exception>
		public IWebSocketSession this[string id]
		{
			get
			{
				if (id == null)
				{
					throw new ArgumentNullException("id");
				}
				if (id.Length == 0)
				{
					throw new ArgumentException("An empty string.", "id");
				}
				IWebSocketSession session;
				tryGetSession(id, out session);
				return session;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the inactive sessions in
		/// the WebSocket service are cleaned up periodically.
		/// </summary>
		/// <remarks>
		/// The set operation does nothing if the service has already started or
		/// it is shutting down.
		/// </remarks>
		/// <value>
		/// <c>true</c> if the inactive sessions are cleaned up every 60 seconds;
		/// otherwise, <c>false</c>.
		/// </value>
		public bool KeepClean
		{
			get
			{
				return _clean;
			}
			set
			{
				string message;
				if (!canSet(out message))
				{
					_log.Warn(message);
					return;
				}
				lock (_sync)
				{
					if (!canSet(out message))
					{
						_log.Warn(message);
					}
					else
					{
						_clean = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets the session instances in the WebSocket service.
		/// </summary>
		/// <value>
		///   <para>
		///   An <c>IEnumerable&lt;IWebSocketSession&gt;</c> instance.
		///   </para>
		///   <para>
		///   It provides an enumerator which supports the iteration over
		///   the collection of the session instances.
		///   </para>
		/// </value>
		public IEnumerable<IWebSocketSession> Sessions
		{
			get
			{
				if (_state != ServerState.Start)
				{
					return Enumerable.Empty<IWebSocketSession>();
				}
				lock (_sync)
				{
					if (_state != ServerState.Start)
					{
						return Enumerable.Empty<IWebSocketSession>();
					}
					return _sessions.Values.ToList();
				}
			}
		}

		/// <summary>
		/// Gets or sets the time to wait for the response to the WebSocket Ping or
		/// Close.
		/// </summary>
		/// <remarks>
		/// The set operation does nothing if the service has already started or
		/// it is shutting down.
		/// </remarks>
		/// <value>
		/// A <see cref="T:System.TimeSpan" /> to wait for the response.
		/// </value>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// The value specified for a set operation is zero or less.
		/// </exception>
		public TimeSpan WaitTime
		{
			get
			{
				return _waitTime;
			}
			set
			{
				if (value <= TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value", "Zero or less.");
				}
				string message;
				if (!canSet(out message))
				{
					_log.Warn(message);
					return;
				}
				lock (_sync)
				{
					if (!canSet(out message))
					{
						_log.Warn(message);
					}
					else
					{
						_waitTime = value;
					}
				}
			}
		}

		internal WebSocketSessionManager(Logger log)
		{
			_log = log;
			_clean = true;
			_forSweep = new object();
			_sessions = new Dictionary<string, IWebSocketSession>();
			_state = ServerState.Ready;
			_sync = ((ICollection)_sessions).SyncRoot;
			_waitTime = TimeSpan.FromSeconds(1.0);
			setSweepTimer(60000.0);
		}

		private void broadcast(Opcode opcode, byte[] data, Action completed)
		{
			Dictionary<CompressionMethod, byte[]> dictionary = new Dictionary<CompressionMethod, byte[]>();
			try
			{
				foreach (IWebSocketSession session in Sessions)
				{
					if (_state != ServerState.Start)
					{
						_log.Error("The service is shutting down.");
						break;
					}
					session.Context.WebSocket.Send(opcode, data, dictionary);
				}
				if (completed != null)
				{
					completed();
				}
			}
			catch (Exception ex)
			{
				_log.Error(ex.Message);
				_log.Debug(ex.ToString());
			}
			finally
			{
				dictionary.Clear();
			}
		}

		private void broadcast(Opcode opcode, Stream stream, Action completed)
		{
			Dictionary<CompressionMethod, Stream> dictionary = new Dictionary<CompressionMethod, Stream>();
			try
			{
				foreach (IWebSocketSession session in Sessions)
				{
					if (_state != ServerState.Start)
					{
						_log.Error("The service is shutting down.");
						break;
					}
					session.Context.WebSocket.Send(opcode, stream, dictionary);
				}
				if (completed != null)
				{
					completed();
				}
			}
			catch (Exception ex)
			{
				_log.Error(ex.Message);
				_log.Debug(ex.ToString());
			}
			finally
			{
				foreach (Stream value in dictionary.Values)
				{
					value.Dispose();
				}
				dictionary.Clear();
			}
		}

		private void broadcastAsync(Opcode opcode, byte[] data, Action completed)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				broadcast(opcode, data, completed);
			});
		}

		private void broadcastAsync(Opcode opcode, Stream stream, Action completed)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				broadcast(opcode, stream, completed);
			});
		}

		private Dictionary<string, bool> broadping(byte[] frameAsBytes)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (IWebSocketSession session in Sessions)
			{
				if (_state != ServerState.Start)
				{
					_log.Error("The service is shutting down.");
					break;
				}
				bool value = session.Context.WebSocket.Ping(frameAsBytes, _waitTime);
				dictionary.Add(session.ID, value);
			}
			return dictionary;
		}

		private bool canSet(out string message)
		{
			message = null;
			if (_state == ServerState.Start)
			{
				message = "The service has already started.";
				return false;
			}
			if (_state == ServerState.ShuttingDown)
			{
				message = "The service is shutting down.";
				return false;
			}
			return true;
		}

		private static string createID()
		{
			return Guid.NewGuid().ToString("N");
		}

		private void setSweepTimer(double interval)
		{
			_sweepTimer = new System.Timers.Timer(interval);
			_sweepTimer.Elapsed += delegate
			{
				Sweep();
			};
		}

		private void stop(PayloadData payloadData, bool send)
		{
			byte[] frameAsBytes = (send ? WebSocketFrame.CreateCloseFrame(payloadData, false).ToArray() : null);
			lock (_sync)
			{
				_state = ServerState.ShuttingDown;
				_sweepTimer.Enabled = false;
				foreach (IWebSocketSession item in _sessions.Values.ToList())
				{
					item.Context.WebSocket.Close(payloadData, frameAsBytes);
				}
				_state = ServerState.Stop;
			}
		}

		private bool tryGetSession(string id, out IWebSocketSession session)
		{
			session = null;
			if (_state != ServerState.Start)
			{
				return false;
			}
			lock (_sync)
			{
				if (_state != ServerState.Start)
				{
					return false;
				}
				return _sessions.TryGetValue(id, out session);
			}
		}

		internal string Add(IWebSocketSession session)
		{
			lock (_sync)
			{
				if (_state != ServerState.Start)
				{
					return null;
				}
				string text = createID();
				_sessions.Add(text, session);
				return text;
			}
		}

		internal void Broadcast(Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache)
		{
			foreach (IWebSocketSession session in Sessions)
			{
				if (_state != ServerState.Start)
				{
					_log.Error("The service is shutting down.");
					break;
				}
				session.Context.WebSocket.Send(opcode, data, cache);
			}
		}

		internal void Broadcast(Opcode opcode, Stream stream, Dictionary<CompressionMethod, Stream> cache)
		{
			foreach (IWebSocketSession session in Sessions)
			{
				if (_state != ServerState.Start)
				{
					_log.Error("The service is shutting down.");
					break;
				}
				session.Context.WebSocket.Send(opcode, stream, cache);
			}
		}

		internal Dictionary<string, bool> Broadping(byte[] frameAsBytes, TimeSpan timeout)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (IWebSocketSession session in Sessions)
			{
				if (_state != ServerState.Start)
				{
					_log.Error("The service is shutting down.");
					break;
				}
				bool value = session.Context.WebSocket.Ping(frameAsBytes, timeout);
				dictionary.Add(session.ID, value);
			}
			return dictionary;
		}

		internal bool Remove(string id)
		{
			lock (_sync)
			{
				return _sessions.Remove(id);
			}
		}

		internal void Start()
		{
			lock (_sync)
			{
				_sweepTimer.Enabled = _clean;
				_state = ServerState.Start;
			}
		}

		internal void Stop(ushort code, string reason)
		{
			if (code == 1005)
			{
				stop(PayloadData.Empty, true);
			}
			else
			{
				stop(new PayloadData(code, reason), !code.IsReserved());
			}
		}

		/// <summary>
		/// Sends <paramref name="data" /> to every client in the WebSocket service.
		/// </summary>
		/// <param name="data">
		/// An array of <see cref="T:System.Byte" /> that represents the binary data to send.
		/// </param>
		/// <exception cref="T:System.InvalidOperationException">
		/// The current state of the manager is not Start.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="data" /> is <see langword="null" />.
		/// </exception>
		public void Broadcast(byte[] data)
		{
			if (_state != ServerState.Start)
			{
				string message = "The current state of the manager is not Start.";
				throw new InvalidOperationException(message);
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.LongLength <= WebSocket.FragmentLength)
			{
				broadcast(Opcode.Binary, data, null);
			}
			else
			{
				broadcast(Opcode.Binary, new MemoryStream(data), null);
			}
		}

		/// <summary>
		/// Sends <paramref name="data" /> to every client in the WebSocket service.
		/// </summary>
		/// <param name="data">
		/// A <see cref="T:System.String" /> that represents the text data to send.
		/// </param>
		/// <exception cref="T:System.InvalidOperationException">
		/// The current state of the manager is not Start.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="data" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="data" /> could not be UTF-8-encoded.
		/// </exception>
		public void Broadcast(string data)
		{
			if (_state != ServerState.Start)
			{
				string message = "The current state of the manager is not Start.";
				throw new InvalidOperationException(message);
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] bytes;
			if (!data.TryGetUTF8EncodedBytes(out bytes))
			{
				string message2 = "It could not be UTF-8-encoded.";
				throw new ArgumentException(message2, "data");
			}
			if (bytes.LongLength <= WebSocket.FragmentLength)
			{
				broadcast(Opcode.Text, bytes, null);
			}
			else
			{
				broadcast(Opcode.Text, new MemoryStream(bytes), null);
			}
		}

		/// <summary>
		/// Sends the data from <paramref name="stream" /> to every client in
		/// the WebSocket service.
		/// </summary>
		/// <remarks>
		/// The data is sent as the binary data.
		/// </remarks>
		/// <param name="stream">
		/// A <see cref="T:System.IO.Stream" /> instance from which to read the data to send.
		/// </param>
		/// <param name="length">
		/// An <see cref="T:System.Int32" /> that specifies the number of bytes to send.
		/// </param>
		/// <exception cref="T:System.InvalidOperationException">
		/// The current state of the manager is not Start.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="stream" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="stream" /> cannot be read.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="length" /> is less than 1.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   No data could be read from <paramref name="stream" />.
		///   </para>
		/// </exception>
		public void Broadcast(Stream stream, int length)
		{
			if (_state != ServerState.Start)
			{
				string message = "The current state of the manager is not Start.";
				throw new InvalidOperationException(message);
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				string message2 = "It cannot be read.";
				throw new ArgumentException(message2, "stream");
			}
			if (length < 1)
			{
				string message3 = "Less than 1.";
				throw new ArgumentException(message3, "length");
			}
			byte[] array = stream.ReadBytes(length);
			int num = array.Length;
			if (num == 0)
			{
				string message4 = "No data could be read from it.";
				throw new ArgumentException(message4, "stream");
			}
			if (num < length)
			{
				_log.Warn(string.Format("Only {0} byte(s) of data could be read from the stream.", num));
			}
			if (num <= WebSocket.FragmentLength)
			{
				broadcast(Opcode.Binary, array, null);
			}
			else
			{
				broadcast(Opcode.Binary, new MemoryStream(array), null);
			}
		}

		/// <summary>
		/// Sends <paramref name="data" /> asynchronously to every client in
		/// the WebSocket service.
		/// </summary>
		/// <remarks>
		/// This method does not wait for the send to be complete.
		/// </remarks>
		/// <param name="data">
		/// An array of <see cref="T:System.Byte" /> that represents the binary data to send.
		/// </param>
		/// <param name="completed">
		///   <para>
		///   An <see cref="T:System.Action" /> delegate or <see langword="null" />
		///   if not needed.
		///   </para>
		///   <para>
		///   The delegate invokes the method called when the send is complete.
		///   </para>
		/// </param>
		/// <exception cref="T:System.InvalidOperationException">
		/// The current state of the manager is not Start.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="data" /> is <see langword="null" />.
		/// </exception>
		public void BroadcastAsync(byte[] data, Action completed)
		{
			if (_state != ServerState.Start)
			{
				string message = "The current state of the manager is not Start.";
				throw new InvalidOperationException(message);
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.LongLength <= WebSocket.FragmentLength)
			{
				broadcastAsync(Opcode.Binary, data, completed);
			}
			else
			{
				broadcastAsync(Opcode.Binary, new MemoryStream(data), completed);
			}
		}

		/// <summary>
		/// Sends <paramref name="data" /> asynchronously to every client in
		/// the WebSocket service.
		/// </summary>
		/// <remarks>
		/// This method does not wait for the send to be complete.
		/// </remarks>
		/// <param name="data">
		/// A <see cref="T:System.String" /> that represents the text data to send.
		/// </param>
		/// <param name="completed">
		///   <para>
		///   An <see cref="T:System.Action" /> delegate or <see langword="null" />
		///   if not needed.
		///   </para>
		///   <para>
		///   The delegate invokes the method called when the send is complete.
		///   </para>
		/// </param>
		/// <exception cref="T:System.InvalidOperationException">
		/// The current state of the manager is not Start.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="data" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="data" /> could not be UTF-8-encoded.
		/// </exception>
		public void BroadcastAsync(string data, Action completed)
		{
			if (_state != ServerState.Start)
			{
				string message = "The current state of the manager is not Start.";
				throw new InvalidOperationException(message);
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] bytes;
			if (!data.TryGetUTF8EncodedBytes(out bytes))
			{
				string message2 = "It could not be UTF-8-encoded.";
				throw new ArgumentException(message2, "data");
			}
			if (bytes.LongLength <= WebSocket.FragmentLength)
			{
				broadcastAsync(Opcode.Text, bytes, completed);
			}
			else
			{
				broadcastAsync(Opcode.Text, new MemoryStream(bytes), completed);
			}
		}

		/// <summary>
		/// Sends the data from <paramref name="stream" /> asynchronously to
		/// every client in the WebSocket service.
		/// </summary>
		/// <remarks>
		///   <para>
		///   The data is sent as the binary data.
		///   </para>
		///   <para>
		///   This method does not wait for the send to be complete.
		///   </para>
		/// </remarks>
		/// <param name="stream">
		/// A <see cref="T:System.IO.Stream" /> instance from which to read the data to send.
		/// </param>
		/// <param name="length">
		/// An <see cref="T:System.Int32" /> that specifies the number of bytes to send.
		/// </param>
		/// <param name="completed">
		///   <para>
		///   An <see cref="T:System.Action" /> delegate or <see langword="null" />
		///   if not needed.
		///   </para>
		///   <para>
		///   The delegate invokes the method called when the send is complete.
		///   </para>
		/// </param>
		/// <exception cref="T:System.InvalidOperationException">
		/// The current state of the manager is not Start.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="stream" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="stream" /> cannot be read.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="length" /> is less than 1.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   No data could be read from <paramref name="stream" />.
		///   </para>
		/// </exception>
		public void BroadcastAsync(Stream stream, int length, Action completed)
		{
			if (_state != ServerState.Start)
			{
				string message = "The current state of the manager is not Start.";
				throw new InvalidOperationException(message);
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				string message2 = "It cannot be read.";
				throw new ArgumentException(message2, "stream");
			}
			if (length < 1)
			{
				string message3 = "Less than 1.";
				throw new ArgumentException(message3, "length");
			}
			byte[] array = stream.ReadBytes(length);
			int num = array.Length;
			if (num == 0)
			{
				string message4 = "No data could be read from it.";
				throw new ArgumentException(message4, "stream");
			}
			if (num < length)
			{
				_log.Warn(string.Format("Only {0} byte(s) of data could be read from the stream.", num));
			}
			if (num <= WebSocket.FragmentLength)
			{
				broadcastAsync(Opcode.Binary, array, completed);
			}
			else
			{
				broadcastAsync(Opcode.Binary, new MemoryStream(array), completed);
			}
		}

		/// <summary>
		/// Sends a ping to every client in the WebSocket service.
		/// </summary>
		/// <returns>
		///   <para>
		///   A <c>Dictionary&lt;string, bool&gt;</c>.
		///   </para>
		///   <para>
		///   It represents a collection of pairs of a session ID and
		///   a value indicating whether a pong has been received from
		///   the client within a time.
		///   </para>
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">
		/// The current state of the manager is not Start.
		/// </exception>
		[Obsolete("This method will be removed.")]
		public Dictionary<string, bool> Broadping()
		{
			if (_state != ServerState.Start)
			{
				string message = "The current state of the manager is not Start.";
				throw new InvalidOperationException(message);
			}
			return Broadping(WebSocketFrame.EmptyPingBytes, _waitTime);
		}

		/// <summary>
		/// Sends a ping with <paramref name="message" /> to every client in
		/// the WebSocket service.
		/// </summary>
		/// <returns>
		///   <para>
		///   A <c>Dictionary&lt;string, bool&gt;</c>.
		///   </para>
		///   <para>
		///   It represents a collection of pairs of a session ID and
		///   a value indicating whether a pong has been received from
		///   the client within a time.
		///   </para>
		/// </returns>
		/// <param name="message">
		///   <para>
		///   A <see cref="T:System.String" /> that represents the message to send.
		///   </para>
		///   <para>
		///   The size must be 125 bytes or less in UTF-8.
		///   </para>
		/// </param>
		/// <exception cref="T:System.InvalidOperationException">
		/// The current state of the manager is not Start.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="message" /> could not be UTF-8-encoded.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// The size of <paramref name="message" /> is greater than 125 bytes.
		/// </exception>
		[Obsolete("This method will be removed.")]
		public Dictionary<string, bool> Broadping(string message)
		{
			if (_state != ServerState.Start)
			{
				string message2 = "The current state of the manager is not Start.";
				throw new InvalidOperationException(message2);
			}
			if (message.IsNullOrEmpty())
			{
				return Broadping(WebSocketFrame.EmptyPingBytes, _waitTime);
			}
			byte[] bytes;
			if (!message.TryGetUTF8EncodedBytes(out bytes))
			{
				string message3 = "It could not be UTF-8-encoded.";
				throw new ArgumentException(message3, "message");
			}
			if (bytes.Length > 125)
			{
				string message4 = "Its size is greater than 125 bytes.";
				throw new ArgumentOutOfRangeException("message", message4);
			}
			WebSocketFrame webSocketFrame = WebSocketFrame.CreatePingFrame(bytes, false);
			return Broadping(webSocketFrame.ToArray(), _waitTime);
		}

		/// <summary>
		/// Closes the specified session.
		/// </summary>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session to close.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="id" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="id" /> is an empty string.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The session could not be found.
		/// </exception>
		public void CloseSession(string id)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			session.Context.WebSocket.Close();
		}

		/// <summary>
		/// Closes the specified session with <paramref name="code" /> and
		/// <paramref name="reason" />.
		/// </summary>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session to close.
		/// </param>
		/// <param name="code">
		///   <para>
		///   A <see cref="T:System.UInt16" /> that represents the status code indicating
		///   the reason for the close.
		///   </para>
		///   <para>
		///   The status codes are defined in
		///   <see href="http://tools.ietf.org/html/rfc6455#section-7.4">
		///   Section 7.4</see> of RFC 6455.
		///   </para>
		/// </param>
		/// <param name="reason">
		///   <para>
		///   A <see cref="T:System.String" /> that represents the reason for the close.
		///   </para>
		///   <para>
		///   The size must be 123 bytes or less in UTF-8.
		///   </para>
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="id" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="id" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="code" /> is 1010 (mandatory extension).
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="code" /> is 1005 (no status) and there is
		///   <paramref name="reason" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="reason" /> could not be UTF-8-encoded.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The session could not be found.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <para>
		///   <paramref name="code" /> is less than 1000 or greater than 4999.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   The size of <paramref name="reason" /> is greater than 123 bytes.
		///   </para>
		/// </exception>
		public void CloseSession(string id, ushort code, string reason)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			session.Context.WebSocket.Close(code, reason);
		}

		/// <summary>
		/// Closes the specified session with <paramref name="code" /> and
		/// <paramref name="reason" />.
		/// </summary>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session to close.
		/// </param>
		/// <param name="code">
		///   <para>
		///   One of the <see cref="T:Sfs2X.WebSocketSharp.CloseStatusCode" /> enum values.
		///   </para>
		///   <para>
		///   It represents the status code indicating the reason for the close.
		///   </para>
		/// </param>
		/// <param name="reason">
		///   <para>
		///   A <see cref="T:System.String" /> that represents the reason for the close.
		///   </para>
		///   <para>
		///   The size must be 123 bytes or less in UTF-8.
		///   </para>
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="id" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="id" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="code" /> is
		///   <see cref="F:Sfs2X.WebSocketSharp.CloseStatusCode.MandatoryExtension" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="code" /> is
		///   <see cref="F:Sfs2X.WebSocketSharp.CloseStatusCode.NoStatus" /> and there is
		///   <paramref name="reason" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="reason" /> could not be UTF-8-encoded.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The session could not be found.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// The size of <paramref name="reason" /> is greater than 123 bytes.
		/// </exception>
		public void CloseSession(string id, CloseStatusCode code, string reason)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			session.Context.WebSocket.Close(code, reason);
		}

		/// <summary>
		/// Sends a ping to the client using the specified session.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the send has done with no error and a pong has been
		/// received from the client within a time; otherwise, <c>false</c>.
		/// </returns>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="id" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="id" /> is an empty string.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The session could not be found.
		/// </exception>
		public bool PingTo(string id)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			return session.Context.WebSocket.Ping();
		}

		/// <summary>
		/// Sends a ping with <paramref name="message" /> to the client using
		/// the specified session.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the send has done with no error and a pong has been
		/// received from the client within a time; otherwise, <c>false</c>.
		/// </returns>
		/// <param name="message">
		///   <para>
		///   A <see cref="T:System.String" /> that represents the message to send.
		///   </para>
		///   <para>
		///   The size must be 125 bytes or less in UTF-8.
		///   </para>
		/// </param>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="id" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="id" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="message" /> could not be UTF-8-encoded.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The session could not be found.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// The size of <paramref name="message" /> is greater than 125 bytes.
		/// </exception>
		public bool PingTo(string message, string id)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message2 = "The session could not be found.";
				throw new InvalidOperationException(message2);
			}
			return session.Context.WebSocket.Ping(message);
		}

		/// <summary>
		/// Sends <paramref name="data" /> to the client using the specified session.
		/// </summary>
		/// <param name="data">
		/// An array of <see cref="T:System.Byte" /> that represents the binary data to send.
		/// </param>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <para>
		///   <paramref name="id" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="data" /> is <see langword="null" />.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="id" /> is an empty string.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <para>
		///   The session could not be found.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   The current state of the WebSocket connection is not Open.
		///   </para>
		/// </exception>
		public void SendTo(byte[] data, string id)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			session.Context.WebSocket.Send(data);
		}

		/// <summary>
		/// Sends <paramref name="data" /> to the client using the specified session.
		/// </summary>
		/// <param name="data">
		/// A <see cref="T:System.String" /> that represents the text data to send.
		/// </param>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <para>
		///   <paramref name="id" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="data" /> is <see langword="null" />.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="id" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="data" /> could not be UTF-8-encoded.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <para>
		///   The session could not be found.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   The current state of the WebSocket connection is not Open.
		///   </para>
		/// </exception>
		public void SendTo(string data, string id)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			session.Context.WebSocket.Send(data);
		}

		/// <summary>
		/// Sends the data from <paramref name="stream" /> to the client using
		/// the specified session.
		/// </summary>
		/// <remarks>
		/// The data is sent as the binary data.
		/// </remarks>
		/// <param name="stream">
		/// A <see cref="T:System.IO.Stream" /> instance from which to read the data to send.
		/// </param>
		/// <param name="length">
		/// An <see cref="T:System.Int32" /> that specifies the number of bytes to send.
		/// </param>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <para>
		///   <paramref name="id" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="stream" /> is <see langword="null" />.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="id" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="stream" /> cannot be read.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="length" /> is less than 1.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   No data could be read from <paramref name="stream" />.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <para>
		///   The session could not be found.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   The current state of the WebSocket connection is not Open.
		///   </para>
		/// </exception>
		public void SendTo(Stream stream, int length, string id)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			session.Context.WebSocket.Send(stream, length);
		}

		/// <summary>
		/// Sends <paramref name="data" /> asynchronously to the client using
		/// the specified session.
		/// </summary>
		/// <remarks>
		/// This method does not wait for the send to be complete.
		/// </remarks>
		/// <param name="data">
		/// An array of <see cref="T:System.Byte" /> that represents the binary data to send.
		/// </param>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session.
		/// </param>
		/// <param name="completed">
		///   <para>
		///   An <c>Action&lt;bool&gt;</c> delegate or <see langword="null" />
		///   if not needed.
		///   </para>
		///   <para>
		///   The delegate invokes the method called when the send is complete.
		///   </para>
		///   <para>
		///   <c>true</c> is passed to the method if the send has done with
		///   no error; otherwise, <c>false</c>.
		///   </para>
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <para>
		///   <paramref name="id" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="data" /> is <see langword="null" />.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="id" /> is an empty string.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <para>
		///   The session could not be found.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   The current state of the WebSocket connection is not Open.
		///   </para>
		/// </exception>
		public void SendToAsync(byte[] data, string id, Action<bool> completed)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			session.Context.WebSocket.SendAsync(data, completed);
		}

		/// <summary>
		/// Sends <paramref name="data" /> asynchronously to the client using
		/// the specified session.
		/// </summary>
		/// <remarks>
		/// This method does not wait for the send to be complete.
		/// </remarks>
		/// <param name="data">
		/// A <see cref="T:System.String" /> that represents the text data to send.
		/// </param>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session.
		/// </param>
		/// <param name="completed">
		///   <para>
		///   An <c>Action&lt;bool&gt;</c> delegate or <see langword="null" />
		///   if not needed.
		///   </para>
		///   <para>
		///   The delegate invokes the method called when the send is complete.
		///   </para>
		///   <para>
		///   <c>true</c> is passed to the method if the send has done with
		///   no error; otherwise, <c>false</c>.
		///   </para>
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <para>
		///   <paramref name="id" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="data" /> is <see langword="null" />.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="id" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="data" /> could not be UTF-8-encoded.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <para>
		///   The session could not be found.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   The current state of the WebSocket connection is not Open.
		///   </para>
		/// </exception>
		public void SendToAsync(string data, string id, Action<bool> completed)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			session.Context.WebSocket.SendAsync(data, completed);
		}

		/// <summary>
		/// Sends the data from <paramref name="stream" /> asynchronously to
		/// the client using the specified session.
		/// </summary>
		/// <remarks>
		///   <para>
		///   The data is sent as the binary data.
		///   </para>
		///   <para>
		///   This method does not wait for the send to be complete.
		///   </para>
		/// </remarks>
		/// <param name="stream">
		/// A <see cref="T:System.IO.Stream" /> instance from which to read the data to send.
		/// </param>
		/// <param name="length">
		/// An <see cref="T:System.Int32" /> that specifies the number of bytes to send.
		/// </param>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session.
		/// </param>
		/// <param name="completed">
		///   <para>
		///   An <c>Action&lt;bool&gt;</c> delegate or <see langword="null" />
		///   if not needed.
		///   </para>
		///   <para>
		///   The delegate invokes the method called when the send is complete.
		///   </para>
		///   <para>
		///   <c>true</c> is passed to the method if the send has done with
		///   no error; otherwise, <c>false</c>.
		///   </para>
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <para>
		///   <paramref name="id" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="stream" /> is <see langword="null" />.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="id" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="stream" /> cannot be read.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="length" /> is less than 1.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   No data could be read from <paramref name="stream" />.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <para>
		///   The session could not be found.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   The current state of the WebSocket connection is not Open.
		///   </para>
		/// </exception>
		public void SendToAsync(Stream stream, int length, string id, Action<bool> completed)
		{
			IWebSocketSession session;
			if (!TryGetSession(id, out session))
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			session.Context.WebSocket.SendAsync(stream, length, completed);
		}

		/// <summary>
		/// Cleans up the inactive sessions in the WebSocket service.
		/// </summary>
		public void Sweep()
		{
			if (_sweeping)
			{
				_log.Info("The sweeping is already in progress.");
				return;
			}
			lock (_forSweep)
			{
				if (_sweeping)
				{
					_log.Info("The sweeping is already in progress.");
					return;
				}
				_sweeping = true;
			}
			foreach (string inactiveID in InactiveIDs)
			{
				if (_state != ServerState.Start)
				{
					break;
				}
				lock (_sync)
				{
					if (_state != ServerState.Start)
					{
						break;
					}
					IWebSocketSession value;
					if (_sessions.TryGetValue(inactiveID, out value))
					{
						switch (value.ConnectionState)
						{
						case WebSocketState.Open:
							value.Context.WebSocket.Close(CloseStatusCode.Abnormal);
							break;
						default:
							_sessions.Remove(inactiveID);
							break;
						case WebSocketState.Closing:
							break;
						}
					}
					continue;
				}
			}
			_sweeping = false;
		}

		/// <summary>
		/// Tries to get the session instance with <paramref name="id" />.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the session is successfully found; otherwise,
		/// <c>false</c>.
		/// </returns>
		/// <param name="id">
		/// A <see cref="T:System.String" /> that represents the ID of the session to find.
		/// </param>
		/// <param name="session">
		///   <para>
		///   When this method returns, a <see cref="T:Sfs2X.WebSocketSharp.Server.IWebSocketSession" />
		///   instance or <see langword="null" /> if not found.
		///   </para>
		///   <para>
		///   The session instance provides the function to access
		///   the information in the session.
		///   </para>
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="id" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="id" /> is an empty string.
		/// </exception>
		public bool TryGetSession(string id, out IWebSocketSession session)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id.Length == 0)
			{
				throw new ArgumentException("An empty string.", "id");
			}
			return tryGetSession(id, out session);
		}
	}
}
