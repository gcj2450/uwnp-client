using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Sfs2X.WebSocketSharp
{
	/// <summary>
	/// Represents a log data used by the <see cref="T:Sfs2X.WebSocketSharp.Logger" /> class.
	/// </summary>
	public class LogData
	{
		private StackFrame _caller;

		private DateTime _date;

		private LogLevel _level;

		private string _message;

		/// <summary>
		/// Gets the information of the logging method caller.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Diagnostics.StackFrame" /> that provides the information of the logging method caller.
		/// </value>
		public StackFrame Caller
		{
			get
			{
				return _caller;
			}
		}

		/// <summary>
		/// Gets the date and time when the log data was created.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.DateTime" /> that represents the date and time when the log data was created.
		/// </value>
		public DateTime Date
		{
			get
			{
				return _date;
			}
		}

		/// <summary>
		/// Gets the logging level of the log data.
		/// </summary>
		/// <value>
		/// One of the <see cref="T:Sfs2X.WebSocketSharp.LogLevel" /> enum values, indicates the logging level of the log data.
		/// </value>
		public LogLevel Level
		{
			get
			{
				return _level;
			}
		}

		/// <summary>
		/// Gets the message of the log data.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the message of the log data.
		/// </value>
		public string Message
		{
			get
			{
				return _message;
			}
		}

		internal LogData(LogLevel level, StackFrame caller, string message)
		{
			_level = level;
			_caller = caller;
			_message = message ?? string.Empty;
			_date = DateTime.Now;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:Sfs2X.WebSocketSharp.LogData" />.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents the current <see cref="T:Sfs2X.WebSocketSharp.LogData" />.
		/// </returns>
		public override string ToString()
		{
			string text = string.Format("{0}|{1,-5}|", _date, _level);
			MethodBase method = _caller.GetMethod();
			Type declaringType = method.DeclaringType;
			int fileLineNumber = _caller.GetFileLineNumber();
			string arg = string.Format("{0}{1}.{2}:{3}|", text, declaringType.Name, method.Name, fileLineNumber);
			string[] array = _message.Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
			if (array.Length <= 1)
			{
				return string.Format("{0}{1}", arg, _message);
			}
			StringBuilder stringBuilder = new StringBuilder(string.Format("{0}{1}\n", arg, array[0]), 64);
			string format = string.Format("{{0,{0}}}{{1}}\n", text.Length);
			for (int i = 1; i < array.Length; i++)
			{
				stringBuilder.AppendFormat(format, "", array[i]);
			}
			stringBuilder.Length--;
			return stringBuilder.ToString();
		}
	}
}
