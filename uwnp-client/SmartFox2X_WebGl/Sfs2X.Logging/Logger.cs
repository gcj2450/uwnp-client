using System;
using System.Collections.Generic;
using Sfs2X.Core;
using UnityEngine;

namespace Sfs2X.Logging
{
	/// <summary>
	/// The internal logger used by the SmartFoxServer 2X client API.
	/// </summary>
	///
	/// <remarks>
	/// You can get a reference to the Logger by means of the <see cref="P:Sfs2X.SmartFox.Logger">SmartFox.Logger</see> property.
	/// Accessing the logger can be useful to control the client-side logging level, enable or disable the output towards the console and enable or disable the events dispatching.
	/// When logger events are enabled, you can add your own listners to this class, in order to have a lower access to logged messages (for example you could display them in a dedicated panel in the application interface).
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Logging.LoggerEvent" />
	/// <seealso cref="P:Sfs2X.SmartFox.Logger" />
	public class Logger
	{
		private SmartFox smartFox;

		private bool enableConsoleTrace = true;

		private bool enableEventDispatching = true;

		private LogLevel loggingLevel;

		/// <summary>
		/// Indicates whether or not the output of logged messages to the console is enabled.
		/// </summary>
		///
		/// <remarks>
		/// If <c>true</c>, logged messages are displayed using the <c>Console.WriteLine()</c> method (or <c>System.Diagnostics.Debug.WriteLine</c> under Universal Windows Platform).
		/// </remarks>
		public bool EnableConsoleTrace
		{
			get
			{
				return enableConsoleTrace;
			}
			set
			{
				enableConsoleTrace = value;
			}
		}

		/// <summary>
		/// Indicates whether dispatching of log events is enabled or not.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Logging.LoggerEvent" />
		public bool EnableEventDispatching
		{
			get
			{
				return enableEventDispatching;
			}
			set
			{
				enableEventDispatching = value;
			}
		}

		/// <summary>
		/// Determines the current logging level.
		/// </summary>
		///
		/// <remarks>
		/// Messages with a level lower than this value are not logged. The available log levels are contained in the <see cref="T:Sfs2X.Logging.LogLevel" /> class.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Logging.LogLevel" />
		public LogLevel LoggingLevel
		{
			get
			{
				return loggingLevel;
			}
			set
			{
				loggingLevel = value;
			}
		}

		/// <exclude />
		public Logger(SmartFox smartFox)
		{
			this.smartFox = smartFox;
			loggingLevel = LogLevel.INFO;
		}

		/// <exclude />
		public void Debug(params string[] messages)
		{
			Log(LogLevel.DEBUG, string.Join(" ", messages));
		}

		/// <exclude />
		public void Info(params string[] messages)
		{
			Log(LogLevel.INFO, string.Join(" ", messages));
		}

		/// <exclude />
		public void Warn(params string[] messages)
		{
			Log(LogLevel.WARN, string.Join(" ", messages));
		}

		/// <exclude />
		public void Error(params string[] messages)
		{
			Log(LogLevel.ERROR, string.Join(" ", messages));
		}

		private void Log(LogLevel level, string message)
		{
			if (level >= loggingLevel)
			{
				if (enableConsoleTrace)
				{
					UnityEngine.Debug.Log("[SFS - " + level.ToString() + "] " + message);
				}
				if (enableEventDispatching && smartFox != null)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					dictionary.Add("message", message);
					LoggerEvent evt = new LoggerEvent(level, dictionary);
					smartFox.DispatchEvent(evt);
				}
			}
		}

		/// <summary>
		/// Registers a delegate method for log messages callbacks.
		/// </summary>
		///
		/// <remarks>
		/// Calling this method is just like calling the <see cref="M:Sfs2X.SmartFox.AddLogListener(Sfs2X.Logging.LogLevel,Sfs2X.Core.EventListenerDelegate)">SmartFox.AddLogListener</see> method. 
		/// </remarks>
		///
		/// <param name="level">The level of the log events to register a listener for.</param>
		/// <param name="listener">The event listener to register.</param>
		///
		/// <example>
		/// <code>
		/// void SomeMethod() {
		/// 	sfs.logger.EnableEventDispatching = true;
		/// 	sfs.Logger.AddEventListener(LogLevel.INFO, OnInfoLogMessage);
		/// 	sfs.Logger.AddEventListener(LogLevel.WARN, OnWarnLogMessage);
		/// }
		///
		/// void OnInfoLogMessage(BaseEvent evt) {
		/// 	string message = (string)evt.Params["message"];
		/// 	Console.WriteLine("[SFS2X INFO] " + message);                           // .Net / Unity
		/// 	System.Diagnostics.Debug.WriteLine("[SFS2X INFO] " + message);          // UWP
		/// }
		///
		/// void OnWarnLogMessage(BaseEvent evt) {
		/// 	string message = (string)evt.Params["message"];
		/// 	Console.WriteLine("[SFS2X WARN] " + message);                           // .Net / Unity
		/// 	System.Diagnostics.Debug.WriteLine("[SFS2X WARN] " + message);          // UWP
		/// }
		/// </code>
		/// </example>
		///
		/// <seealso cref="M:Sfs2X.SmartFox.AddLogListener(Sfs2X.Logging.LogLevel,Sfs2X.Core.EventListenerDelegate)" />
		public void AddEventListener(LogLevel level, EventListenerDelegate listener)
		{
			if (smartFox != null)
			{
				smartFox.AddEventListener(LoggerEvent.LogEventType(level), listener);
			}
		}

		/// <summary>
		/// Removes a delegate method for log messages callbacks.
		/// </summary>
		///
		/// <remarks>
		/// Calling this method is just like calling the <see cref="M:Sfs2X.SmartFox.RemoveLogListener(Sfs2X.Logging.LogLevel,Sfs2X.Core.EventListenerDelegate)">Sfs2X.SmartFox.RemoveLogListener</see> method. 
		/// </remarks>
		///
		/// <param name="logLevel">The level of the log events to remove the listener for.</param>
		/// <param name="listener">The event listener to remove.</param>
		///
		/// <seealso cref="M:Sfs2X.SmartFox.RemoveLogListener(Sfs2X.Logging.LogLevel,Sfs2X.Core.EventListenerDelegate)" />
		public void RemoveEventListener(LogLevel logLevel, EventListenerDelegate listener)
		{
			if (smartFox != null)
			{
				smartFox.RemoveEventListener(LoggerEvent.LogEventType(logLevel), listener);
			}
		}
	}
}
