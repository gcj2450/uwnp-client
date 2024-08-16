using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Buddylist
{
	/// <summary>
	/// Initializes the Buddy List system on the current client.
	/// </summary>
	///
	/// <remarks>
	/// Buddy List system initialization involves loading any previously stored buddy-specific data from the server, such as the current user's buddies list,
	/// his previous state and the persistent Buddy Variables. The initialization request is <b>the first operation to be executed</b> in order to be able
	/// to use the Buddy List system features. Once the initialization is completed, the <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_LIST_INIT" /> event id fired
	/// and the user has access to all his previously set data and can start to interact with his buddies; if the initialization failed, a <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ERROR" /> event id fired.
	/// </remarks>
	///
	/// <example>
	/// The following example initializes the Buddy List system:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_LIST_INIT, OnBuddyInited);
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_ERROR, OnBuddyError);
	///
	/// 	// Initialize the Buddy List system
	/// 	sfs.Send(new InitBuddyListRequest());
	/// }
	///
	/// void OnBuddyInited(BaseEvent evt) {
	/// 	Console.WriteLine("Buddy List system initialized successfully");                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Buddy List system initialized successfully");       // UWP
	///
	/// 	// Retrieve my buddies list
	/// 	List&lt;Buddy&gt; buddies = sfs.BuddyManager.BuddyList;
	///
	/// 	// Display the online buddies in a list component in the application interface
	/// 	...
	/// }
	///
	/// void OnBuddyError(BaseEvent evt) {
	/// 	Console.WriteLine("The following error occurred while executing a buddy-related request: " + (string)evt.Params["errorMessage"]);                       // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("The following error occurred while executing a buddy-related request: " + (string)evt.Params["errorMessage"]);      // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_LIST_INIT" />
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ERROR" />
	public class InitBuddyListRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_BLIST = "bl";

		/// <exclude />
		public static readonly string KEY_BUDDY_STATES = "bs";

		/// <exclude />
		public static readonly string KEY_MY_VARS = "mv";

		/// <summary>
		/// Creates a new InitBuddyListRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		public InitBuddyListRequest()
			: base(RequestType.InitBuddyList)
		{
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (sfs.BuddyManager.Inited)
			{
				list.Add("Buddy List is already initialized.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("InitBuddyRequest error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
		}
	}
}
