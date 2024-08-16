using System.Collections.Generic;

namespace Sfs2X.Core
{
	/// <summary>
	/// This class represents all the cluster-related events dispatched by the SmartFoxServer 2X C# API.
	/// </summary>
	///
	/// <seealso cref="M:Sfs2X.SmartFox.AddEventListener(System.String,Sfs2X.Core.EventListenerDelegate)" />
	public class SFSClusterEvent : BaseEvent
	{
		/// <summary>
		/// Dispatched when the Lobby has found a Game Node for the client to join in a SmartFoxServer 2X Cluster.
		/// </summary>
		///
		/// <remarks>
		/// This event is fired when the Lobby signals the client should join a given Game Node.
		/// <para />
		/// The <see cref="P:Sfs2X.Core.BaseEvent.Params" /> object contains the following parameters:
		/// <list type="table">
		///   <listheader>
		///     <term>Parameter</term>
		///     <description>Description</description>
		///   </listheader>
		///  <item>
		///     <term>configData</term>
		///     <description>(<b>ConfigData</b>) The pre-populated ConfigData object to start a new connection towards the designated Game Node.</description>
		///   </item>
		///   <item>
		///     <term>userName</term>
		///     <description>(<b>String</b>) The user name to access the Game Node.</description>
		///   </item>
		///   <item>
		///     <term>password</term>
		///     <description>(<b>String</b>) A temporary and unique password to access the Game Node.</description>
		///   </item>
		/// </list>
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Util.ConfigData" />
		public static readonly string CONNECTION_REQUIRED = "connectionRequired";

		/// <summary>
		/// Dispatched when a cluster-related request cannot be satisfied, typically creating or joining a game in a SmartFoxServer 2X Cluster.
		/// </summary>
		///
		/// <remarks>
		/// The event does not provide further details as the Load Balancer simply queries the available servers and if none is found matching the contextual criteria, the cluster request cannot be completed.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Util.ConfigData" />
		public static readonly string LOAD_BALANCER_ERROR = "loadBalancerError";

		/// <exclude />
		public SFSClusterEvent(string type, Dictionary<string, object> data)
			: base(type, data)
		{
		}

		/// <exclude />
		public SFSClusterEvent(string type)
			: base(type)
		{
		}
	}
}
