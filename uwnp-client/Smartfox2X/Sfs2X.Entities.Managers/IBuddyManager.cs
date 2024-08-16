using System.Collections.Generic;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities.Managers
{
	/// <summary>
	/// The IBuddyManager interface defines all the methods and properties exposed by the client-side manager of the SmartFoxServer Buddy List system.
	/// </summary>
	///
	/// <remarks>
	/// In the SmartFoxServer 2X client API this interface is implemented by the <see cref="T:Sfs2X.Entities.Managers.SFSBuddyManager" /> class. Read the class description for additional informations.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.Managers.SFSBuddyManager" />
	public interface IBuddyManager
	{
		/// <summary>
		/// Indicates whether the client's Buddy List system is initialized or not.
		/// </summary>
		///
		/// <remarks>
		/// If not initialized, an <see cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" /> request should be sent to the server in order to retrieve the persistent Buddy List data.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" />
		bool Inited { get; set; }

		/// <summary>
		/// Returns a list of Buddy objects representing all the offline buddies in the user's buddies list.
		/// </summary>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.IsOnline" />
		List<Buddy> OfflineBuddies { get; }

		/// <summary>
		/// Returns a list of Buddy objects representing all the online buddies in the user's buddies list.
		/// </summary>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.IsOnline" />
		List<Buddy> OnlineBuddies { get; }

		/// <summary>
		/// Returns a list of Buddy objects representing all the buddies in the user's buddies list.
		/// </summary>
		///
		/// <remarks>
		/// The list is <c>null</c> if the Buddy List system is not initialized.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" />
		List<Buddy> BuddyList { get; }

		/// <summary>
		/// Returns a list of strings representing the available custom buddy states.
		/// </summary>
		///
		/// <remarks>
		/// The custom states are received by the client upon initialization of the Buddy List system. They can be configured by means of the SmartFoxServer 2X Administration Tool.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.State" />
		List<string> BuddyStates { get; set; }

		/// <summary>
		/// Returns all the Buddy Variables associated with the current user.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Entities.Variables.BuddyVariable" />
		/// <seealso cref="M:Sfs2X.Entities.Managers.IBuddyManager.GetMyVariable(System.String)" />
		List<BuddyVariable> MyVariables { get; set; }

		/// <summary>
		/// Returns the current user's online/offline state.
		/// </summary>
		///
		/// <remarks>
		/// If <c>true</c>, the user appears to be online in the buddies list of other users who have him as a buddy.
		/// <para />
		/// The online state of a user in a buddy list is handled by means of a reserved Buddy Variable (see <see cref="T:Sfs2X.Entities.Variables.ReservedBuddyVariables" /> class);
		/// it can be changed using the dedicated <see cref="T:Sfs2X.Requests.Buddylist.GoOnlineRequest" /> request.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.IsOnline" />
		/// <seealso cref="T:Sfs2X.Entities.Variables.ReservedBuddyVariables" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.GoOnlineRequest" />
		bool MyOnlineState { get; set; }

		/// <summary>
		/// Returns the current user's nickname (if set).
		/// </summary>
		///
		/// <remarks>
		/// If the nickname was never set before, <c>null</c> is returned.
		/// <para />
		/// As the nickname of a user in a buddy list is handled by means of a reserved Buddy Variable (see <see cref="T:Sfs2X.Entities.Variables.ReservedBuddyVariables" /> class),
		/// it can be set using the <see cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" /> request.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.NickName" />
		/// <seealso cref="T:Sfs2X.Entities.Variables.ReservedBuddyVariables" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" />
		string MyNickName { get; set; }

		/// <summary>
		/// Returns the current user's custom state (if set).
		/// </summary>
		///
		/// <remarks>
		/// Examples of custom states are "Available", "Busy", "Be right back", etc. If the custom state was never set before, <c>null</c> is returned.
		/// <para />
		/// As the custom state of a user in a buddy list is handled by means of a reserved Buddy Variable (see <see cref="T:Sfs2X.Entities.Variables.ReservedBuddyVariables" /> class),
		/// it can be set using the <see cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" /> request.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.State" />
		/// <seealso cref="T:Sfs2X.Entities.Variables.ReservedBuddyVariables" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" />
		string MyState { get; set; }

		/// <exclude />
		void AddBuddy(Buddy buddy);

		/// <exclude />
		Buddy RemoveBuddyById(int id);

		/// <exclude />
		Buddy RemoveBuddyByName(string name);

		/// <summary>
		/// Indicates whether a buddy exists in user's buddies list or not.
		/// </summary>
		///
		/// <param name="name">The name of the buddy whose presence in the buddies list is to be tested.</param>
		///
		/// <returns><c>true</c> if the specified buddy exists in the buddies list.</returns>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.Name" />
		bool ContainsBuddy(string name);

		/// <summary>
		/// Retrieves a Buddy object from its id property.
		/// </summary>
		///
		/// <param name="id">The id of the buddy to be found.</param>
		///
		/// <returns>The object representing the buddy, or <c>null</c> if no buddy with the passed id exists in the buddies list.</returns>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.Id" />
		/// <seealso cref="M:Sfs2X.Entities.Managers.IBuddyManager.GetBuddyByName(System.String)" />
		/// <seealso cref="M:Sfs2X.Entities.Managers.IBuddyManager.GetBuddyByNickName(System.String)" />
		Buddy GetBuddyById(int id);

		/// <summary>
		/// Retrieves a Buddy object from its name property.
		/// </summary>
		///
		/// <param name="name">The name of the buddy to be found.</param>
		///
		/// <returns>The object representing the buddy, or <c>null</c> if no buddy with the passed name exists in the buddies list.</returns>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.Name" />
		/// <seealso cref="M:Sfs2X.Entities.Managers.IBuddyManager.GetBuddyById(System.Int32)" />
		/// <seealso cref="M:Sfs2X.Entities.Managers.IBuddyManager.GetBuddyByNickName(System.String)" />
		Buddy GetBuddyByName(string name);

		/// <summary>
		/// Retrieves a Buddy object from its nickName property (if set).
		/// </summary>
		///
		/// <param name="nickName">The nickName of the buddy to be found.</param>
		///
		/// <returns>The object representing the buddy, or <c>null</c> if no buddy with the passed nickName exists in the buddies list.</returns>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.NickName" />
		/// <seealso cref="M:Sfs2X.Entities.Managers.IBuddyManager.GetBuddyById(System.Int32)" />
		/// <seealso cref="M:Sfs2X.Entities.Managers.IBuddyManager.GetBuddyByName(System.String)" />
		Buddy GetBuddyByNickName(string nickName);

		/// <summary>
		/// Retrieves a Buddy Variable from its name.
		/// </summary>
		///
		/// <param name="varName">The name of the Buddy Variable to be retrieved.</param>
		///
		/// <returns>The object representing the Buddy Variable, or <c>null</c> if no Buddy Variable with the passed name is associated with the current user.</returns>
		///
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" />
		/// <seealso cref="P:Sfs2X.Entities.Managers.IBuddyManager.MyVariables" />
		BuddyVariable GetMyVariable(string varName);

		/// <exclude />
		void SetMyVariable(BuddyVariable bVar);

		/// <exclude />
		void ClearAll();
	}
}
