using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	/// <summary>
	/// The SFSUserManager class is the entity in charge of managing the local (client-side) users list.
	/// </summary>
	///
	/// <remarks>
	/// This manager keeps track of all the users that are currently joined in the same Rooms of the current user. It also provides utility methods to look for users by name and id.
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.SmartFox.UserManager" />
	public class SFSUserManager : IUserManager
	{
		private Dictionary<string, User> usersByName;

		private Dictionary<int, User> usersById;

		private readonly object listLock = new object();

		/// <exclude />
		protected Room room;

		/// <exclude />
		protected SmartFox sfs;

		/// <inheritdoc />
		public int UserCount
		{
			get
			{
				lock (listLock)
				{
					return usersById.Count;
				}
			}
		}

		/// <exclude />
		public SmartFox SmartFoxClient
		{
			get
			{
				return sfs;
			}
		}

		/// <exclude />
		protected void LogWarn(string msg)
		{
			if (sfs != null)
			{
				sfs.Log.Warn(msg);
			}
			else if (room != null && room.RoomManager != null)
			{
				room.RoomManager.SmartFoxClient.Log.Warn(msg);
			}
		}

		/// <exclude />
		public SFSUserManager(SmartFox sfs)
		{
			this.sfs = sfs;
			usersByName = new Dictionary<string, User>();
			usersById = new Dictionary<int, User>();
		}

		/// <exclude />
		public SFSUserManager(Room room)
		{
			this.room = room;
			usersByName = new Dictionary<string, User>();
			usersById = new Dictionary<int, User>();
		}

		/// <inheritdoc />
		public bool ContainsUserName(string userName)
		{
			lock (listLock)
			{
				return usersByName.ContainsKey(userName);
			}
		}

		/// <inheritdoc />
		public bool ContainsUserId(int userId)
		{
			lock (listLock)
			{
				return usersById.ContainsKey(userId);
			}
		}

		/// <inheritdoc />
		public bool ContainsUser(User user)
		{
			lock (listLock)
			{
				return usersByName.ContainsValue(user);
			}
		}

		/// <inheritdoc />
		public User GetUserByName(string userName)
		{
			lock (listLock)
			{
				User value = null;
				usersByName.TryGetValue(userName, out value);
				return value;
			}
		}

		/// <inheritdoc />
		public User GetUserById(int userId)
		{
			lock (listLock)
			{
				User value = null;
				usersById.TryGetValue(userId, out value);
				return value;
			}
		}

		/// <exclude />
		public virtual void AddUser(User user)
		{
			lock (listLock)
			{
				if (ContainsUserId(user.Id))
				{
					LogWarn("Unexpected: duplicate user in UserManager: " + ((user != null) ? user.ToString() : null));
				}
				AddUserInternal(user);
			}
		}

		/// <exclude />
		protected void AddUserInternal(User user)
		{
			lock (listLock)
			{
				usersByName[user.Name] = user;
				usersById[user.Id] = user;
			}
		}

		/// <exclude />
		public virtual void RemoveUser(User user)
		{
			lock (listLock)
			{
				usersByName.Remove(user.Name);
				usersById.Remove(user.Id);
			}
		}

		/// <exclude />
		public void RemoveUserById(int id)
		{
			lock (listLock)
			{
				if (ContainsUserId(id))
				{
					User user = usersById[id];
					RemoveUser(user);
				}
			}
		}

		/// <inheritdoc />
		public List<User> GetUserList()
		{
			lock (listLock)
			{
				return new List<User>(usersById.Values);
			}
		}

		/// <exclude />
		public void ReplaceAll(List<User> newUserList)
		{
			lock (listLock)
			{
				usersByName.Clear();
				usersById.Clear();
				foreach (User newUser in newUserList)
				{
					AddUser(newUser);
				}
			}
		}
	}
}
