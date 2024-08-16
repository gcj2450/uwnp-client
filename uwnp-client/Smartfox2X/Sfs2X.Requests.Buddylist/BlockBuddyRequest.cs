using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Buddylist
{
	/// <summary>
	/// Blocks or unblocks a buddy in the current user's buddies list. Blocked buddies won't be able to send messages or requests to that user.
	/// </summary>
	///
	/// <remarks>
	/// In order to block a buddy, the current user must be online in the Buddy List system.
	/// If the operation is successful, a <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_BLOCK" /> confirmation event is dispatched; otherwise the <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ERROR" /> event is fired.
	/// <para />
	/// This request can be sent if the Buddy List system was previously initialized only (see the <see cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" /> request description).
	/// </remarks>
	///
	/// <example>
	/// The following example sends a request to block a buddy:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_BLOCK, onBuddyBlock);
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_ERROR, OnBuddyError);
	///
	/// 	// Block Jack in my buddies list
	/// 	sfs.Send(new BlockBuddyRequest("Jack", true));
	/// }
	///
	/// void onBuddyBlock(BaseEvent evt) {
	/// 	Buddy buddy = (Buddy)evt.Params["buddy"];
	/// 	Console.WriteLine("Buddy " + buddy.Name + " is now " + (buddy.IsBlocked ? "blocked" : "unblocked"));                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Buddy " + buddy.Name + " is now " + (buddy.IsBlocked ? "blocked" : "unblocked"));       // UWP
	/// }
	///
	/// void OnBuddyError(BaseEvent evt) {
	/// 	Console.WriteLine("The following error occurred while executing a buddy-related request: " + (string)evt.Params["errorMessage"]);                       // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("The following error occurred while executing a buddy-related request: " + (string)evt.Params["errorMessage"]);      // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_BLOCK" />
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ERROR" />
	public class BlockBuddyRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_BUDDY_NAME = "bn";

		/// <exclude />
		public static readonly string KEY_BUDDY_BLOCK_STATE = "bs";

		/// <exclude />
		public static readonly string KEY_BUDDY = "bd";

		private string buddyName;

		private bool blocked;

		/// <summary>
		/// Creates a new BlockBuddyRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="buddyName">The name of the buddy to be blocked or unblocked.</param>
		/// <param name="blocked"><c>true</c> if the buddy must be blocked; <c>false</c> if he must be unblocked.</param>
		public BlockBuddyRequest(string buddyName, bool blocked)
			: base(RequestType.BlockBuddy)
		{
			this.buddyName = buddyName;
			this.blocked = blocked;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.BuddyManager.Inited)
			{
				list.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (buddyName == null || buddyName.Length < 1)
			{
				list.Add("Invalid buddy name: " + buddyName);
			}
			if (!sfs.BuddyManager.MyOnlineState)
			{
				list.Add("Can't block buddy while off-line");
			}
			Buddy buddyByName = sfs.BuddyManager.GetBuddyByName(buddyName);
			if (buddyByName == null)
			{
				list.Add("Can't block buddy, it's not in your list: " + buddyName);
			}
			else if (buddyByName.IsBlocked == blocked)
			{
				list.Add("BuddyBlock flag is already in the requested state: " + blocked + ", for buddy: " + ((buddyByName != null) ? buddyByName.ToString() : null));
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("BuddyList request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_BUDDY_NAME, buddyName);
			sfso.PutBool(KEY_BUDDY_BLOCK_STATE, blocked);
		}
	}
}
