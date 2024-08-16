using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	/// <summary>
	/// The IUserManager interface defines all the methods and properties exposed by the client-side manager of the SmartFoxServer User entities.
	/// </summary>
	///
	/// <remarks>
	/// In the SmartFoxServer 2X client API this interface is implemented by the <see cref="T:Sfs2X.Entities.Managers.SFSUserManager" /> class. Read the class description for additional informations.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.Managers.SFSUserManager" />
	public interface IUserManager
	{
		/// <summary>
		/// Returns the total number of users in the local users list.
		/// </summary>
		int UserCount { get; }

		/// <exclude />
		SmartFox SmartFoxClient { get; }

		/// <summary>
		/// Indicates whether a user exists in the local users list or not from the name.
		/// </summary>
		///
		/// <param name="userName">The name of the user whose presence in the users list is to be tested.</param>
		///
		/// <returns><c>true</c> if a user with the passed name exists in the users list.</returns>
		bool ContainsUserName(string userName);

		/// <summary>
		/// Indicates whether a user exists in the local users list or not from the id.
		/// </summary>
		///
		/// <param name="userId">The id of the user whose presence in the users list is to be tested.</param>
		///
		/// <returns><c>true</c> if a user corresponding to the passed id exists in the users list.</returns>
		bool ContainsUserId(int userId);

		/// <summary>
		/// Indicates whether a user exists in the local users list or not.
		/// </summary>
		///
		/// <param name="user">The object representing the user whose presence in the users list is to be tested.</param>
		///
		/// <returns><c>true</c> if the passed user exists in the users list.</returns>
		bool ContainsUser(User user);

		/// <summary>
		/// Retrieves a User object from its name property.
		/// </summary>
		///
		/// <param name="userName">The name of the user to be found.</param>
		///
		/// <returns>The object representing the user, or <c>null</c> if no user with the passed name exists in the local users list.</returns>
		User GetUserByName(string userName);

		/// <summary>
		/// Retrieves a User object from its id property.
		/// </summary>
		///
		/// <param name="userId">The id of the user to be found.</param>
		///
		/// <returns>The object representing the user, or <c>null</c> if no user with the passed id exists in the local users list.</returns>
		User GetUserById(int userId);

		/// <exclude />
		void AddUser(User user);

		/// <exclude />
		void RemoveUser(User user);

		/// <exclude />
		void RemoveUserById(int id);

		/// <summary>
		/// Get the whole list of users inside the Rooms joined by the client.
		/// </summary>
		///
		/// <returns>The list of User objects representing the users in the local users list.</returns>
		List<User> GetUserList();

		/// <exclude />
		void ReplaceAll(List<User> newUserList);
	}
}
