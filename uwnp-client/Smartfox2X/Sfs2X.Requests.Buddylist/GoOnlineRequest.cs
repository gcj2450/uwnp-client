using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Buddylist
{
	/// <summary>
	/// Toggles the current user's online/offline state as buddy in other users' buddies lists.
	/// </summary>
	///
	/// <remarks>
	/// All clients who have the current user as buddy in their buddies list will receive the <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ONLINE_STATE_UPDATE" /> event and see the <see cref="P:Sfs2X.Entities.Buddy.IsOnline" /> property change accordingly.
	/// The same event is also dispatched to the current user, who sent the request, so that the application interface can be updated accordingly. Going online/offline as buddy doesn't affect the user connection, the currently joined Zone and Rooms, etc.
	/// <para />
	/// The online state of a user in a buddy list is handled by means of a reserved and persistent Buddy Variable.
	/// <para />
	/// This request can be sent if the Buddy List system was previously initialized only (see the <see cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" /> request description).
	/// </remarks>
	///
	/// <example>
	/// The following example changes the user online state in the Buddy List system:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_ONLINE_STATE_UPDATE, OnBuddyOnlineStateUpdate);
	///
	/// 	// Put myself offline in the Buddy List system
	/// 	sfs.Send(new GoOnlineRequest(false));
	/// }
	///
	/// void OnBuddyOnlineStateUpdate(BaseEvent evt) {
	///
	/// 	// As the state change event is dispatched to me too,
	///     // I have to check if I am the one who changed his state
	/// 	bool isItMe = (bool)evt.Params["isItMe"];
	/// 	Buddy buddy = (Buddy)evt.Params["buddy"];
	///
	/// 	if (isItMe)
	/// 	{
	/// 		Console.WriteLine("I'm now " + (sfs.BuddyManager.MyOnlineState ? "online" : "offline"));                        // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine("I'm now " + (sfs.BuddyManager.MyOnlineState ? "online" : "offline"));       // UWP
	///     }
	/// 	else
	/// 	{
	/// 		Console.WriteLine("My buddy " + buddy.Name + " is now", (buddy.IsOnline ? "online" : "offline"));                       // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine("My buddy " + buddy.Name + " is now", (buddy.IsOnline ? "online" : "offline"));      // UWP
	///     }
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ONLINE_STATE_UPDATE" />
	/// <seealso cref="P:Sfs2X.Entities.Buddy.IsOnline" />
	/// <seealso cref="P:Sfs2X.Entities.Managers.IBuddyManager.MyOnlineState" />
	/// <seealso cref="T:Sfs2X.Entities.Variables.ReservedBuddyVariables" />
	public class GoOnlineRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ONLINE = "o";

		/// <exclude />
		public static readonly string KEY_BUDDY_NAME = "bn";

		/// <exclude />
		public static readonly string KEY_BUDDY_ID = "bi";

		private bool online;

		/// <summary>
		/// Creates a new GoOnlineRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="online"><c>true</c> to make the current user available (online) in the Buddy List system; <c>false</c> to make him not available (offline).</param>
		public GoOnlineRequest(bool online)
			: base(RequestType.GoOnline)
		{
			this.online = online;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.BuddyManager.Inited)
			{
				list.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("GoOnline request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfs.BuddyManager.MyOnlineState = online;
			sfso.PutBool(KEY_ONLINE, online);
		}
	}
}
