using System.Collections.Generic;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities
{
	/// <summary>
	/// The Buddy interface defines all the methods and properties that an object representing a SmartFoxServer Buddy entity exposes.
	/// </summary>
	///
	/// <remarks>
	/// In the SmartFoxServer 2X client API this interface is implemented by the <see cref="T:Sfs2X.Entities.SFSBuddy" /> class. Read the class description for additional informations.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.SFSBuddy" />
	public interface Buddy
	{
		/// <summary>
		/// Indicates the id of this buddy.
		/// </summary>
		///
		/// <remarks>
		/// This is equal to the id assigned by SmartFoxServer to the corresponding user.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.User.Id" />
		int Id { get; set; }

		/// <summary>
		/// Indicates the name of this buddy.
		/// </summary>
		///
		/// <remarks>
		/// This is equal to the name of the corresponding user.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.User.Name" />
		string Name { get; }

		/// <summary>
		/// Indicates whether this buddy is blocked in the current user's buddies list or not.
		/// </summary>
		///
		/// <remarks>
		/// A buddy can be blocked by means of a <see cref="T:Sfs2X.Requests.Buddylist.BlockBuddyRequest" /> request.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.BlockBuddyRequest" />
		bool IsBlocked { get; set; }

		/// <summary>
		/// Indicates whether this buddy is online in the Buddy List system or not.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.GoOnlineRequest" />
		bool IsOnline { get; }

		/// <summary>
		/// Indicates whether this buddy is temporary (non-persistent) in the current user's buddies list or not.
		/// </summary>
		bool IsTemp { get; }

		/// <summary>
		/// Returns the custom state of this buddy.
		/// </summary>
		///
		/// <remarks>
		/// Examples of custom states are "Available", "Busy", "Be right back", etc. If the custom state is not set, <c>null</c> is returned.<br />
		/// The list of available custom states is returned by the <see cref="P:Sfs2X.Entities.Managers.IBuddyManager.BuddyStates">IBuddyManager.BuddyStates</see> property.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.Managers.IBuddyManager.BuddyStates" />
		string State { get; }

		/// <summary>
		/// Returns the nickname of this buddy.
		/// </summary>
		///
		/// <remarks>
		/// If the nickname is not set, <c>null</c> is returned.
		/// </remarks>
		string NickName { get; }

		/// <summary>
		/// Returns a list of BuddyVariable objects associated with the buddy.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" />
		List<BuddyVariable> Variables { get; }

		/// <summary>
		/// Retrieves a Buddy Variable from its name.
		/// </summary>
		///
		/// <param name="varName">The name of the Buddy Variable to be retrieved.</param>
		///
		/// <returns>The object representing the Buddy Variable, or <c>null</c> if no Buddy Variable with the passed name is associated with this buddy.</returns>
		///
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" />
		BuddyVariable GetVariable(string varName);

		/// <summary>
		/// Indicates whether this buddy has the specified Buddy Variable set or not.
		/// </summary>
		///
		/// <param name="varName">The name of the Buddy Variable whose existance must be checked.</param>
		///
		/// <returns><c>true</c> if a Buddy Variable with the passed name is set for this buddy.</returns>
		bool ContainsVariable(string varName);

		/// <summary>
		/// Retrieves the list of persistent Buddy Variables of this buddy.
		/// </summary>
		///
		/// <returns>A list of objects representing the offline Buddy Variables.</returns>
		///
		/// <seealso cref="P:Sfs2X.Entities.Variables.BuddyVariable.IsOffline" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" />
		List<BuddyVariable> GetOfflineVariables();

		/// <summary>
		/// Retrieves the list of non-persistent Buddy Variables of this buddy.
		/// </summary>
		///
		/// <returns>A list of objects representing the online Buddy Variables.</returns>
		///
		/// <seealso cref="P:Sfs2X.Entities.Variables.BuddyVariable.IsOffline" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" />
		List<BuddyVariable> GetOnlineVariables();

		/// <exclude />
		void SetVariable(BuddyVariable bVar);

		/// <exclude />
		void SetVariables(ICollection<BuddyVariable> variables);

		/// <exclude />
		void RemoveVariable(string varName);

		/// <exclude />
		void ClearVolatileVariables();
	}
}
