using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	/// <exclude />
	public class SFSGlobalUserManager : SFSUserManager, IUserManager
	{
		private Dictionary<User, int> roomRefCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.Entities.Managers.SFSGlobalUserManager" /> class.
		/// </summary>
		/// <param name="sfs">
		/// Sfs.
		/// </param>
		public SFSGlobalUserManager(SmartFox sfs)
			: base(sfs)
		{
			roomRefCount = new Dictionary<User, int>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.Entities.Managers.SFSGlobalUserManager" /> class.
		/// </summary>
		/// <param name="room">
		/// Room.
		/// </param>
		public SFSGlobalUserManager(Room room)
			: base(room)
		{
			roomRefCount = new Dictionary<User, int>();
		}

		/// <summary>
		/// Does not allow duplicates and keeps a reference count
		/// </summary>
		/// <param name="user">
		/// A <see cref="T:Sfs2X.Entities.User" />
		/// </param>
		public override void AddUser(User user)
		{
			lock (roomRefCount)
			{
				if (!roomRefCount.ContainsKey(user))
				{
					base.AddUser(user);
					roomRefCount[user] = 1;
				}
				else
				{
					roomRefCount[user]++;
				}
			}
		}

		/// <summary>
		/// Removes the user.
		/// </summary>
		/// <param name="user">
		/// User.
		/// </param>
		public override void RemoveUser(User user)
		{
			RemoveUserReference(user, false);
		}

		public void RemoveUserReference(User user, bool disconnected)
		{
			lock (roomRefCount)
			{
				if (roomRefCount.ContainsKey(user))
				{
					if (roomRefCount[user] < 1)
					{
						LogWarn("GlobalUserManager RefCount is already at zero. User: " + ((user != null) ? user.ToString() : null));
						return;
					}
					roomRefCount[user]--;
					if (roomRefCount[user] == 0 || disconnected)
					{
						base.RemoveUser(user);
						roomRefCount.Remove(user);
					}
				}
				else
				{
					LogWarn("Can't remove User from GlobalUserManager. RefCount missing. User: " + ((user != null) ? user.ToString() : null));
				}
			}
		}
	}
}
