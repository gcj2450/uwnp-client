using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Managers;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;

namespace Sfs2X.Entities
{
	/// <summary>
	/// The SFSUser object represents a client logged in SmartFoxServer.
	/// </summary>
	///
	/// <remarks>
	/// The SmartFoxServer 2X client API are not aware of all the clients (users) connected to the server, but only of those that are in the same Rooms joined by the current client;
	/// this reduces the traffic between the client and the server considerably. In order to interact with other users the client should join other Rooms
	/// or use the Buddy List system to keep track of and interact with friends.
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.SmartFox.UserManager" />
	public class SFSUser : User
	{
		/// <exclude />
		protected int id = -1;

		/// <exclude />
		protected int privilegeId = 0;

		/// <exclude />
		protected string name;

		/// <exclude />
		protected bool isItMe;

		/// <exclude />
		protected Dictionary<string, UserVariable> variables;

		/// <exclude />
		protected Dictionary<string, object> properties;

		/// <exclude />
		protected bool isModerator;

		/// <exclude />
		protected Dictionary<int, int> playerIdByRoomId;

		/// <exclude />
		protected IUserManager userManager;

		/// <exclude />
		protected Vec3D aoiEntryPoint;

		/// <inheritdoc />
		public int Id
		{
			get
			{
				return id;
			}
		}

		/// <inheritdoc />
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <inheritdoc />
		public int PlayerId
		{
			get
			{
				return GetPlayerId(userManager.SmartFoxClient.LastJoinedRoom);
			}
		}

		/// <inheritdoc />
		public int PrivilegeId
		{
			get
			{
				return privilegeId;
			}
			set
			{
				privilegeId = value;
			}
		}

		/// <inheritdoc />
		public bool IsPlayer
		{
			get
			{
				return PlayerId > 0;
			}
		}

		/// <inheritdoc />
		public bool IsSpectator
		{
			get
			{
				return !IsPlayer;
			}
		}

		/// <inheritdoc />
		public bool IsItMe
		{
			get
			{
				return isItMe;
			}
		}

		/// <exclude />
		public IUserManager UserManager
		{
			get
			{
				return userManager;
			}
			set
			{
				if (userManager != null)
				{
					throw new SFSError("Cannot re-assign the User manager. Already set. User: " + ((this != null) ? ToString() : null));
				}
				userManager = value;
			}
		}

		/// <inheritdoc />
		public Dictionary<string, object> Properties
		{
			get
			{
				return properties;
			}
			set
			{
				properties = value;
			}
		}

		/// <inheritdoc />
		public Vec3D AOIEntryPoint
		{
			get
			{
				return aoiEntryPoint;
			}
			set
			{
				aoiEntryPoint = value;
			}
		}

		/// <exclude />
		public static User FromSFSArray(ISFSArray sfsa, Room room)
		{
			User user = new SFSUser(sfsa.GetInt(0), sfsa.GetUtfString(1));
			user.PrivilegeId = sfsa.GetShort(2);
			if (room != null)
			{
				user.SetPlayerId(sfsa.GetShort(3), room);
			}
			ISFSArray sFSArray = sfsa.GetSFSArray(4);
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				user.SetVariable(SFSUserVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
			}
			return user;
		}

		/// <exclude />
		public static User FromSFSArray(ISFSArray sfsa)
		{
			return FromSFSArray(sfsa, null);
		}

		/// <exclude />
		public SFSUser(int id, string name)
		{
			Init(id, name, false);
		}

		/// <exclude />
		public SFSUser(int id, string name, bool isItMe)
		{
			Init(id, name, isItMe);
		}

		/// <exclude />
		private void Init(int id, string name, bool isItMe)
		{
			this.id = id;
			this.name = name;
			this.isItMe = isItMe;
			variables = new Dictionary<string, UserVariable>();
			properties = new Dictionary<string, object>();
			isModerator = false;
			playerIdByRoomId = new Dictionary<int, int>();
		}

		/// <inheritdoc />
		public bool IsJoinedInRoom(Room room)
		{
			return room.ContainsUser(this);
		}

		/// <inheritdoc />
		public bool IsGuest()
		{
			return privilegeId == 0;
		}

		/// <inheritdoc />
		public bool IsStandardUser()
		{
			return privilegeId == 1;
		}

		/// <inheritdoc />
		public bool IsModerator()
		{
			return privilegeId == 2;
		}

		/// <inheritdoc />
		public bool IsAdmin()
		{
			return privilegeId == 3;
		}

		/// <inheritdoc />
		public int GetPlayerId(Room room)
		{
			int result = 0;
			if (playerIdByRoomId.ContainsKey(room.Id))
			{
				result = playerIdByRoomId[room.Id];
			}
			return result;
		}

		/// <exclude />
		public void SetPlayerId(int id, Room room)
		{
			playerIdByRoomId[room.Id] = id;
		}

		/// <exclude />
		public void RemovePlayerId(Room room)
		{
			playerIdByRoomId.Remove(room.Id);
		}

		/// <inheritdoc />
		public bool IsPlayerInRoom(Room room)
		{
			return playerIdByRoomId[room.Id] > 0;
		}

		/// <inheritdoc />
		public bool IsSpectatorInRoom(Room room)
		{
			return playerIdByRoomId[room.Id] < 0;
		}

		/// <inheritdoc />
		public List<UserVariable> GetVariables()
		{
			lock (variables)
			{
				return new List<UserVariable>(variables.Values);
			}
		}

		/// <inheritdoc />
		public UserVariable GetVariable(string name)
		{
			lock (variables)
			{
				if (!variables.ContainsKey(name))
				{
					return null;
				}
				return variables[name];
			}
		}

		/// <exclude />
		public void SetVariable(UserVariable userVariable)
		{
			if (userVariable == null)
			{
				return;
			}
			lock (variables)
			{
				if (userVariable.IsNull())
				{
					variables.Remove(userVariable.Name);
				}
				else
				{
					variables[userVariable.Name] = userVariable;
				}
			}
		}

		/// <exclude />
		public void SetVariables(ICollection<UserVariable> userVariables)
		{
			lock (variables)
			{
				foreach (UserVariable userVariable in userVariables)
				{
					SetVariable(userVariable);
				}
			}
		}

		/// <inheritdoc />
		public bool ContainsVariable(string name)
		{
			lock (variables)
			{
				return variables.ContainsKey(name);
			}
		}

		/// <summary>
		/// Returns a string that contains the user id, name and a boolean indicating if the this object represents the current client.
		/// </summary>
		///
		/// <returns>
		/// The string representation of the <see cref="T:Sfs2X.Entities.SFSUser" /> object.
		/// </returns>
		public override string ToString()
		{
			return "[User: " + name + ", Id: " + id + ", isMe: " + isItMe + "]";
		}
	}
}
