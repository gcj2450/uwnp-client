using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Managers;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;

namespace Sfs2X.Entities
{
	/// <summary>
	/// The SFSRoom object represents a SmartFoxServer Room entity on the client.
	/// </summary>
	///
	/// <remarks>
	/// The SmartFoxServer 2X client API are not aware of all the Rooms which exist on the server side, but only of those that are joined by the user
	/// and those in the Room Groups that have been subscribed. Subscribing to one or more Groups allows the client to listen to Room events in specific "areas" of the Zone,
	/// without having to retrieve and keep synchronized the details of all available Rooms, thus reducing the traffic between the client and the server considerably.
	/// <para />
	/// The list of available Rooms is created after a successful login and it is kept updated continuously by the server.
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.SmartFox.RoomManager" />
	/// <seealso cref="T:Sfs2X.Requests.CreateRoomRequest" />
	/// <seealso cref="T:Sfs2X.Requests.JoinRoomRequest" />
	/// <seealso cref="T:Sfs2X.Requests.SubscribeRoomGroupRequest" />
	/// <seealso cref="T:Sfs2X.Requests.UnsubscribeRoomGroupRequest" />
	/// <seealso cref="T:Sfs2X.Requests.ChangeRoomNameRequest" />
	/// <seealso cref="T:Sfs2X.Requests.ChangeRoomPasswordStateRequest" />
	/// <seealso cref="T:Sfs2X.Requests.ChangeRoomCapacityRequest" />
	public class SFSRoom : Room
	{
		/// <exclude />
		protected int id;

		/// <exclude />
		protected string name;

		/// <exclude />
		protected string groupId;

		/// <exclude />
		protected bool isGame;

		/// <exclude />
		protected bool isHidden;

		/// <exclude />
		protected bool isJoined;

		/// <exclude />
		protected bool isPasswordProtected;

		/// <exclude />
		protected bool isManaged;

		/// <exclude />
		protected Dictionary<string, RoomVariable> variables;

		/// <exclude />
		protected Dictionary<object, object> properties;

		/// <exclude />
		protected IUserManager userManager;

		/// <exclude />
		protected int maxUsers;

		/// <exclude />
		protected int maxSpectators;

		/// <exclude />
		protected int userCount;

		/// <exclude />
		protected int specCount;

		/// <exclude />
		protected IRoomManager roomManager;

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
			set
			{
				name = value;
			}
		}

		/// <inheritdoc />
		public string GroupId
		{
			get
			{
				return groupId;
			}
		}

		/// <inheritdoc />
		public bool IsGame
		{
			get
			{
				return isGame;
			}
			set
			{
				isGame = value;
			}
		}

		/// <inheritdoc />
		public bool IsHidden
		{
			get
			{
				return isHidden;
			}
			set
			{
				isHidden = value;
			}
		}

		/// <inheritdoc />
		public bool IsJoined
		{
			get
			{
				return isJoined;
			}
			set
			{
				isJoined = value;
			}
		}

		/// <inheritdoc />
		public bool IsPasswordProtected
		{
			get
			{
				return isPasswordProtected;
			}
			set
			{
				isPasswordProtected = value;
			}
		}

		/// <exclude />
		public bool IsManaged
		{
			get
			{
				return isManaged;
			}
			set
			{
				isManaged = value;
			}
		}

		/// <inheritdoc />
		public int MaxSpectators
		{
			get
			{
				return maxSpectators;
			}
			set
			{
				maxSpectators = value;
			}
		}

		/// <inheritdoc />
		public Dictionary<object, object> Properties
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
		public int UserCount
		{
			get
			{
				if (!isJoined)
				{
					return userCount;
				}
				if (isGame)
				{
					return PlayerList.Count;
				}
				return userManager.UserCount;
			}
			set
			{
				userCount = value;
			}
		}

		/// <inheritdoc />
		public int MaxUsers
		{
			get
			{
				return maxUsers;
			}
			set
			{
				maxUsers = value;
			}
		}

		/// <inheritdoc />
		public int Capacity
		{
			get
			{
				return maxUsers + maxSpectators;
			}
		}

		/// <inheritdoc />
		public int SpectatorCount
		{
			get
			{
				if (!isGame)
				{
					return 0;
				}
				if (isJoined)
				{
					return SpectatorList.Count;
				}
				return specCount;
			}
			set
			{
				specCount = value;
			}
		}

		/// <inheritdoc />
		public List<User> UserList
		{
			get
			{
				return userManager.GetUserList();
			}
		}

		/// <inheritdoc />
		public List<User> PlayerList
		{
			get
			{
				List<User> list = new List<User>();
				foreach (User user in userManager.GetUserList())
				{
					if (user.IsPlayerInRoom(this))
					{
						list.Add(user);
					}
				}
				return list;
			}
		}

		/// <inheritdoc />
		public List<User> SpectatorList
		{
			get
			{
				List<User> list = new List<User>();
				foreach (User user in userManager.GetUserList())
				{
					if (user.IsSpectatorInRoom(this))
					{
						list.Add(user);
					}
				}
				return list;
			}
		}

		/// <exclude />
		public IRoomManager RoomManager
		{
			get
			{
				return roomManager;
			}
			set
			{
				if (roomManager != null)
				{
					throw new SFSError("Room manager already assigned. Room: " + ((this != null) ? ToString() : null));
				}
				roomManager = value;
			}
		}

		/// <exclude />
		public static Room FromSFSArray(ISFSArray sfsa)
		{
			bool flag = sfsa.Size() == 14;
			Room room = null;
			room = ((!flag) ? new SFSRoom(sfsa.GetInt(0), sfsa.GetUtfString(1), sfsa.GetUtfString(2)) : new MMORoom(sfsa.GetInt(0), sfsa.GetUtfString(1), sfsa.GetUtfString(2)));
			room.IsGame = sfsa.GetBool(3);
			room.IsHidden = sfsa.GetBool(4);
			room.IsPasswordProtected = sfsa.GetBool(5);
			room.UserCount = sfsa.GetShort(6);
			room.MaxUsers = sfsa.GetShort(7);
			ISFSArray sFSArray = sfsa.GetSFSArray(8);
			if (sFSArray.Size() > 0)
			{
				List<RoomVariable> list = new List<RoomVariable>();
				for (int i = 0; i < sFSArray.Size(); i++)
				{
					list.Add(SFSRoomVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
				}
				room.SetVariables(list);
			}
			if (room.IsGame)
			{
				room.SpectatorCount = sfsa.GetShort(9);
				room.MaxSpectators = sfsa.GetShort(10);
			}
			if (flag)
			{
				MMORoom mMORoom = room as MMORoom;
				mMORoom.DefaultAOI = Vec3D.fromArray(sfsa.GetElementAt(11));
				if (!sfsa.IsNull(13))
				{
					mMORoom.LowerMapLimit = Vec3D.fromArray(sfsa.GetElementAt(12));
					mMORoom.HigherMapLimit = Vec3D.fromArray(sfsa.GetElementAt(13));
				}
			}
			return room;
		}

		/// <exclude />
		public SFSRoom(int id, string name)
		{
			Init(id, name, "default");
		}

		/// <exclude />
		public SFSRoom(int id, string name, string groupId)
		{
			Init(id, name, groupId);
		}

		private void Init(int id, string name, string groupId)
		{
			this.id = id;
			this.name = name;
			this.groupId = groupId;
			isJoined = (isGame = (isHidden = false));
			isManaged = true;
			userCount = (specCount = 0);
			variables = new Dictionary<string, RoomVariable>();
			properties = new Dictionary<object, object>();
			userManager = new SFSUserManager(this);
		}

		/// <inheritdoc />
		public List<RoomVariable> GetVariables()
		{
			lock (variables)
			{
				return new List<RoomVariable>(variables.Values);
			}
		}

		/// <inheritdoc />
		public RoomVariable GetVariable(string name)
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

		/// <inheritdoc />
		public User GetUserByName(string name)
		{
			return userManager.GetUserByName(name);
		}

		/// <inheritdoc />
		public User GetUserById(int id)
		{
			return userManager.GetUserById(id);
		}

		/// <exclude />
		public void RemoveUser(User user)
		{
			userManager.RemoveUser(user);
		}

		/// <exclude />
		public void SetVariable(RoomVariable roomVariable)
		{
			lock (variables)
			{
				if (roomVariable.IsNull())
				{
					variables.Remove(roomVariable.Name);
				}
				else
				{
					variables[roomVariable.Name] = roomVariable;
				}
			}
		}

		/// <exclude />
		public void SetVariables(ICollection<RoomVariable> roomVariables)
		{
			lock (variables)
			{
				foreach (RoomVariable roomVariable in roomVariables)
				{
					SetVariable(roomVariable);
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

		/// <exclude />
		public void AddUser(User user)
		{
			userManager.AddUser(user);
		}

		/// <inheritdoc />
		public bool ContainsUser(User user)
		{
			return userManager.ContainsUser(user);
		}

		/// <summary>
		/// Returns a string that contains the Room id, name and id of the Group to which it belongs.
		/// </summary>
		///
		/// <returns>
		/// The string representation of the <see cref="T:Sfs2X.Entities.SFSRoom" /> object.
		/// </returns>
		public override string ToString()
		{
			return "[Room: " + name + ", Id: " + id + ", GroupId: " + groupId + "]";
		}

		/// <exclude />
		public void Merge(Room anotherRoom)
		{
			if (IsJoined)
			{
				return;
			}
			lock (variables)
			{
				variables.Clear();
				foreach (RoomVariable variable in anotherRoom.GetVariables())
				{
					variables[variable.Name] = variable;
				}
			}
			userManager.ReplaceAll(anotherRoom.UserList);
		}
	}
}
