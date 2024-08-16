using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Buddylist
{
	/// <summary>
	/// Removes a buddy from the current user's buddies list.
	/// </summary>
	///
	/// <remarks>
	/// In order to remove a buddy, the current user must be online in the Buddy List system. If the buddy is removed successfully,
	/// the operation is confirmed by a <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_REMOVE" /> event; otherwise the <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ERROR" /> event is fired.
	/// <para />
	/// This request can be sent if the Buddy List system was previously initialized only (see the <see cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" /> request description).
	/// </remarks>
	///
	/// <example>
	/// The following example sends a request to remove a buddy:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_REMOVE, OnBuddyRemoved);
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_ERROR, OnBuddyError);
	///
	/// 	// Remove Jack from my buddies list
	/// 	sfs.Send(new RemoveBuddyRequest("Jack"));
	/// }
	///
	/// void OnBuddyRemoved(BaseEvent evt) {
	/// 	Console.WriteLine("This buddy was removed: " + (Buddy)evt.Params["buddy"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("This buddy was removed: " + (Buddy)evt.Params["buddy"]);        // UWP
	/// }
	///
	/// void OnBuddyError(BaseEvent evt) {
	/// 	Console.WriteLine("The following error occurred while executing a buddy-related request: " + (string)evt.Params["errorMessage"]);                           // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("The following error occurred while executing a buddy-related request: " + (string)evt.Params["errorMessage"]);          // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_REMOVE" />
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ERROR" />
	/// <seealso cref="T:Sfs2X.Requests.Buddylist.AddBuddyRequest" />
	public class RemoveBuddyRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_BUDDY_NAME = "bn";

		private string name;

		/// <summary>
		/// Creates a new RemoveBuddyRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="buddyName">The name of the buddy to be removed from the user's buddies list.</param>
		public RemoveBuddyRequest(string buddyName)
			: base(RequestType.RemoveBuddy)
		{
			name = buddyName;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.BuddyManager.Inited)
			{
				list.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (!sfs.BuddyManager.MyOnlineState)
			{
				list.Add("Can't remove buddy while off-line");
			}
			if (!sfs.BuddyManager.ContainsBuddy(name))
			{
				list.Add("Can't remove buddy, it's not in your list: " + name);
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("BuddyList request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_BUDDY_NAME, name);
		}
	}
}
