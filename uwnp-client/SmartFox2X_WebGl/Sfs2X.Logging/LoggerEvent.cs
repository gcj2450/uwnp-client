using System;
using System.Collections.Generic;
using Sfs2X.Core;

namespace Sfs2X.Logging
{
	/// <summary>
	/// LoggerEvent is the class representing all the events dispatched by the SmartFoxServer 2X C# API internal logger.
	/// </summary>
	public class LoggerEvent : BaseEvent, ICloneable
	{
		private LogLevel level;

		/// <exclude />
		public LoggerEvent(LogLevel level, Dictionary<string, object> parameters)
			: base(LogEventType(level), parameters)
		{
			this.level = level;
		}

		/// <exclude />
		public static string LogEventType(LogLevel level)
		{
			return "LOG_" + level;
		}

		/// <exclude />
		public override string ToString()
		{
			return string.Format("LoggerEvent " + type);
		}

		/// <exclude />
		public new object Clone()
		{
			return new LoggerEvent(level, arguments);
		}
	}
}
