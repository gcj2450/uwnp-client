using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	/// <summary>
	/// The SFSRoomManager class is the entity in charge of managing the client-side Rooms list.
	/// </summary>
	///
	/// <remarks>
	/// This manager keeps track of all the Rooms available in the client-side Rooms list and of subscriptions to multiple Room Groups. It also provides various utility methods to look for Rooms by name and id, retrieve Rooms belonging to a specific Group, etc.
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.SmartFox.RoomManager" />
	public class SFSRoomManager : IRoomManager
	{
		private string ownerZone;

		private List<string> groups;

		private Dictionary<int, Room> roomsById;

		private Dictionary<string, Room> roomsByName;

		private readonly object listLock = new object();

		/// <exclude />
		protected SmartFox smartFox;

		/// <exclude />
		public string OwnerZone
		{
			get
			{
				return ownerZone;
			}
			set
			{
				ownerZone = value;
			}
		}

		/// <exclude />
		public SmartFox SmartFoxClient
		{
			get
			{
				return smartFox;
			}
		}

		/// <exclude />
		public SFSRoomManager(SmartFox sfs)
		{
			smartFox = sfs;
			groups = new List<string>();
			roomsById = new Dictionary<int, Room>();
			roomsByName = new Dictionary<string, Room>();
		}

		/// <exclude />
		public void AddRoom(Room room)
		{
			AddRoom(room, true);
		}

		/// <exclude />
		public void AddRoom(Room room, bool addGroupIfMissing)
		{
			lock (listLock)
			{
				roomsById[room.Id] = room;
				roomsByName[room.Name] = room;
				if (addGroupIfMissing)
				{
					AddGroup(room.GroupId);
				}
				else
				{
					room.IsManaged = false;
				}
			}
		}

		/// <exclude />
		public Room ReplaceRoom(Room room)
		{
			return ReplaceRoom(room, true);
		}

		/// <exclude />
		public Room ReplaceRoom(Room room, bool addToGroupIfMissing)
		{
			Room roomById = GetRoomById(room.Id);
			if (roomById != null)
			{
				roomById.Merge(room);
				return roomById;
			}
			AddRoom(room, addToGroupIfMissing);
			return room;
		}

		/// <exclude />
		public void ChangeRoomName(Room room, string newName)
		{
			string name = room.Name;
			room.Name = newName;
			lock (listLock)
			{
				roomsByName[newName] = room;
				roomsByName.Remove(name);
			}
		}

		/// <exclude />
		public void ChangeRoomPasswordState(Room room, bool isPassProtected)
		{
			room.IsPasswordProtected = isPassProtected;
		}

		/// <exclude />
		public void ChangeRoomCapacity(Room room, int maxUsers, int maxSpect)
		{
			room.MaxUsers = maxUsers;
			room.MaxSpectators = maxSpect;
		}

		/// <exclude />
		public List<string> GetRoomGroups()
		{
			lock (groups)
			{
				return new List<string>(groups);
			}
		}

		/// <exclude />
		public void AddGroup(string groupId)
		{
			lock (groups)
			{
				if (!ContainsGroup(groupId))
				{
					groups.Add(groupId);
				}
			}
		}

		/// <exclude />
		public void RemoveGroup(string groupId)
		{
			lock (groups)
			{
				groups.Remove(groupId);
			}
			List<Room> roomListFromGroup = GetRoomListFromGroup(groupId);
			foreach (Room item in roomListFromGroup)
			{
				if (!item.IsJoined)
				{
					RemoveRoom(item);
				}
				else
				{
					item.IsManaged = false;
				}
			}
		}

		/// <inheritdoc /> 
		public bool ContainsGroup(string groupId)
		{
			lock (groups)
			{
				return groups.Contains(groupId);
			}
		}

		/// <inheritdoc /> 
		public bool ContainsRoom(object idOrName)
		{
			lock (listLock)
			{
				if (idOrName is int)
				{
					return roomsById.ContainsKey((int)idOrName);
				}
				return roomsByName.ContainsKey((string)idOrName);
			}
		}

		/// <inheritdoc /> 
		public bool ContainsRoomInGroup(object idOrName, string groupId)
		{
			List<Room> roomListFromGroup = GetRoomListFromGroup(groupId);
			bool flag = idOrName is int;
			foreach (Room item in roomListFromGroup)
			{
				if (flag)
				{
					if (item.Id == (int)idOrName)
					{
						return true;
					}
				}
				else if (item.Name == (string)idOrName)
				{
					return true;
				}
			}
			return false;
		}

		/// <inheritdoc /> 
		public Room GetRoomById(int id)
		{
			lock (listLock)
			{
				Room value = null;
				roomsById.TryGetValue(id, out value);
				return value;
			}
		}

		/// <inheritdoc /> 
		public Room GetRoomByName(string name)
		{
			lock (listLock)
			{
				Room value = null;
				roomsByName.TryGetValue(name, out value);
				return value;
			}
		}

		/// <inheritdoc /> 
		public List<Room> GetRoomList()
		{
			lock (listLock)
			{
				return new List<Room>(roomsById.Values);
			}
		}

		/// <inheritdoc /> 
		public int GetRoomCount()
		{
			lock (listLock)
			{
				return roomsById.Count;
			}
		}

		/// <inheritdoc /> 
		public List<Room> GetRoomListFromGroup(string groupId)
		{
			List<Room> list = new List<Room>();
			lock (listLock)
			{
				foreach (Room value in roomsById.Values)
				{
					if (value.GroupId == groupId)
					{
						list.Add(value);
					}
				}
			}
			return list;
		}

		/// <exclude />
		public void RemoveRoom(Room room)
		{
			RemoveRoom(room.Id, room.Name);
		}

		/// <exclude />
		public void RemoveRoomById(int id)
		{
			lock (listLock)
			{
				if (ContainsRoom(id))
				{
					Room room = roomsById[id];
					RemoveRoom(id, room.Name);
				}
			}
		}

		/// <exclude />
		public void RemoveRoomByName(string name)
		{
			lock (listLock)
			{
				if (ContainsRoom(name))
				{
					Room room = roomsByName[name];
					RemoveRoom(room.Id, name);
				}
			}
		}

		/// <inheritdoc /> 
		public List<Room> GetJoinedRooms()
		{
			List<Room> list = new List<Room>();
			lock (listLock)
			{
				foreach (Room value in roomsById.Values)
				{
					if (value.IsJoined)
					{
						list.Add(value);
					}
				}
			}
			return list;
		}

		/// <inheritdoc /> 
		public List<Room> GetUserRooms(User user)
		{
			List<Room> list = new List<Room>();
			lock (listLock)
			{
				foreach (Room value in roomsById.Values)
				{
					if (value.ContainsUser(user))
					{
						list.Add(value);
					}
				}
			}
			return list;
		}

		/// <exclude />
		public void RemoveUser(User user)
		{
			lock (listLock)
			{
				foreach (Room value in roomsById.Values)
				{
					if (!(value is MMORoom) && value.ContainsUser(user))
					{
						value.RemoveUser(user);
					}
				}
			}
		}

		/// <exclude />
		private void RemoveRoom(int id, string name)
		{
			lock (listLock)
			{
				roomsById.Remove(id);
				roomsByName.Remove(name);
			}
		}
	}
}
