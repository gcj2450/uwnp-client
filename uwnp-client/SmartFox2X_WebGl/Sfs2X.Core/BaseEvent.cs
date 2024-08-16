using System.Collections.Generic;

namespace Sfs2X.Core
{
	/// <summary>
	/// This is the base class of all the events dispatched by the SmartFoxServer 2X C# API.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Core.SFSEvent" />
	/// <seealso cref="T:Sfs2X.Core.SFSBuddyEvent" />
	public class BaseEvent
	{
		/// <exclude />
		protected Dictionary<string, object> arguments;

		/// <exclude />
		protected string type;

		/// <exclude />
		protected object target;

		/// <exclude />
		public string Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		/// <summary>
		/// A Dictionary containing the event's parameters.
		/// </summary>
		public IDictionary<string, object> Params
		{
			get
			{
				return arguments;
			}
			set
			{
				arguments = value as Dictionary<string, object>;
			}
		}

		/// <exclude />
		public object Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		/// <exclude />
		public override string ToString()
		{
			return type + " [ " + ((target != null) ? target.ToString() : "null") + "]";
		}

		/// <exclude />
		public BaseEvent Clone()
		{
			return new BaseEvent(type, arguments);
		}

		/// <exclude />
		public BaseEvent(string type)
		{
			Type = type;
			if (arguments == null)
			{
				arguments = new Dictionary<string, object>();
			}
		}

		/// <exclude />
		public BaseEvent(string type, Dictionary<string, object> args)
		{
			Type = type;
			arguments = args;
			if (arguments == null)
			{
				arguments = new Dictionary<string, object>();
			}
		}
	}
}
