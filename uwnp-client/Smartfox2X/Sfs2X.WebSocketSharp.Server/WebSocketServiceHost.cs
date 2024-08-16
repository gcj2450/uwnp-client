using System;
using Sfs2X.WebSocketSharp.Net.WebSockets;

namespace Sfs2X.WebSocketSharp.Server
{
	/// <summary>
	/// Exposes the methods and properties used to access the information in
	/// a WebSocket service provided by the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketServer" /> or
	/// <see cref="T:Sfs2X.WebSocketSharp.Server.HttpServer" />.
	/// </summary>
	/// <remarks>
	/// This class is an abstract class.
	/// </remarks>
	public abstract class WebSocketServiceHost
	{
		private Logger _log;

		private string _path;

		private WebSocketSessionManager _sessions;

		internal ServerState State
		{
			get
			{
				return _sessions.State;
			}
		}

		/// <summary>
		/// Gets the logging function for the service.
		/// </summary>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Logger" /> that provides the logging function.
		/// </value>
		protected Logger Log
		{
			get
			{
				return _log;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the service cleans up
		/// the inactive sessions periodically.
		/// </summary>
		/// <remarks>
		/// The set operation does nothing if the service has already started or
		/// it is shutting down.
		/// </remarks>
		/// <value>
		/// <c>true</c> if the service cleans up the inactive sessions every
		/// 60 seconds; otherwise, <c>false</c>.
		/// </value>
		public bool KeepClean
		{
			get
			{
				return _sessions.KeepClean;
			}
			set
			{
				_sessions.KeepClean = value;
			}
		}

		/// <summary>
		/// Gets the path to the service.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the absolute path to
		/// the service.
		/// </value>
		public string Path
		{
			get
			{
				return _path;
			}
		}

		/// <summary>
		/// Gets the management function for the sessions in the service.
		/// </summary>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketSessionManager" /> that manages the sessions in
		/// the service.
		/// </value>
		public WebSocketSessionManager Sessions
		{
			get
			{
				return _sessions;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:System.Type" /> of the behavior of the service.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Type" /> that represents the type of the behavior of
		/// the service.
		/// </value>
		public abstract Type BehaviorType { get; }

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
				return _sessions.WaitTime;
			}
			set
			{
				_sessions.WaitTime = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketServiceHost" /> class
		/// with the specified <paramref name="path" /> and <paramref name="log" />.
		/// </summary>
		/// <param name="path">
		/// A <see cref="T:System.String" /> that represents the absolute path to the service.
		/// </param>
		/// <param name="log">
		/// A <see cref="T:Sfs2X.WebSocketSharp.Logger" /> that represents the logging function for the service.
		/// </param>
		protected WebSocketServiceHost(string path, Logger log)
		{
			_path = path;
			_log = log;
			_sessions = new WebSocketSessionManager(log);
		}

		internal void Start()
		{
			_sessions.Start();
		}

		internal void StartSession(WebSocketContext context)
		{
			CreateSession().Start(context, _sessions);
		}

		internal void Stop(ushort code, string reason)
		{
			_sessions.Stop(code, reason);
		}

		/// <summary>
		/// Creates a new session for the service.
		/// </summary>
		/// <returns>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketBehavior" /> instance that represents
		/// the new session.
		/// </returns>
		protected abstract WebSocketBehavior CreateSession();
	}
	internal class WebSocketServiceHost<TBehavior> : WebSocketServiceHost where TBehavior : WebSocketBehavior
	{
		private Func<TBehavior> _creator;

		public override Type BehaviorType
		{
			get
			{
				return typeof(TBehavior);
			}
		}

		internal WebSocketServiceHost(string path, Func<TBehavior> creator, Logger log)
			: this(path, creator, (Action<TBehavior>)null, log)
		{
		}

		internal WebSocketServiceHost(string path, Func<TBehavior> creator, Action<TBehavior> initializer, Logger log)
			: base(path, log)
		{
			_creator = createCreator(creator, initializer);
		}

		private Func<TBehavior> createCreator(Func<TBehavior> creator, Action<TBehavior> initializer)
		{
			if (initializer == null)
			{
				return creator;
			}
			return delegate
			{
				TBehavior val = creator();
				initializer(val);
				return val;
			};
		}

		protected override WebSocketBehavior CreateSession()
		{
			return _creator();
		}
	}
}
