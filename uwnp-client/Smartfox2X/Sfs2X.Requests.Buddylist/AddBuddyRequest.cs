using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Buddylist
{
	/// <summary>
	/// Adds a new buddy to the current user's buddies list.
	/// </summary>
	///
	/// <remarks>
	/// In order to add a buddy, the current user must be online in the Buddy List system.
	/// If the buddy is added successfully, the operation is confirmed by a <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ADD" /> event;
	/// otherwise the <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ERROR" /> event is fired.
	/// <para />
	/// This request can be sent if the Buddy List system was previously initialized only (see the <see cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" /> request description).
	/// </remarks>
	///
	/// <example>
	/// The following example sends a request to add a buddy:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_ADD, OnBuddyAdded);
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_ERROR, OnBuddyError);
	///
	/// 	// Add Jack as a new buddy to my buddies list
	/// 	sfs.Send(new AddBuddyRequest("Jack"));
	/// }
	///
	/// void OnBuddyAdded(BaseEvent evt) {
	/// 	Console.WriteLine("Buddy was added: " + (Buddy)evt.Params["buddy"]);                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Buddy was added: " + (Buddy)evt.Params["buddy"]);       // UWP
	/// }
	///
	/// void OnBuddyError(BaseEvent evt) {
	/// 	Console.WriteLine("The following error occurred while executing a buddy-related request: " + (string)evt.Params["errorMessage"]);                       // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("The following error occurred while executing a buddy-related request: " + (string)evt.Params["errorMessage"]);      // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ADD" />
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_ERROR" />
	/// <seealso cref="T:Sfs2X.Requests.Buddylist.RemoveBuddyRequest" />
	public class AddBuddyRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_BUDDY_NAME = "bn";

		private string name;

		/// <summary>
		/// Creates a new AddBuddyRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="buddyName">The name of the user to be added as a buddy.</param>
		public AddBuddyRequest(string buddyName)
			: base(RequestType.AddBuddy)
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
			if (name == null || name.Length < 1)
			{
				list.Add("Invalid buddy name: " + name);
			}
			if (!sfs.BuddyManager.MyOnlineState)
			{
				list.Add("Can't add buddy while off-line");
			}
			Buddy buddyByName = sfs.BuddyManager.GetBuddyByName(name);
			if (buddyByName != null && !buddyByName.IsTemp)
			{
				list.Add("Can't add buddy, it is already in your list: " + name);
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
