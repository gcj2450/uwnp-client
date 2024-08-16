using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	/// <summary>
	/// The IRoomManager interface defines all the methods and properties exposed by the client-side manager of the SmartFoxServer Room entities.
	/// </summary>
	///
	/// <remarks>
	/// In the SmartFoxServer 2X client API this interface is implemented by the <see cref="T:Sfs2X.Entities.Managers.SFSRoomManager" /> class. Read the class description for additional informations.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.Managers.SFSRoomManager" />
	public interface IRoomManager
	{
		/// <exclude />
		string OwnerZone { get; }

		/// <exclude />
		SmartFox SmartFoxClient { get; }

		/// <exclude />
		void AddRoom(Room room, bool addGroupIfMissing);

		/// <exclude />
		void AddRoom(Room room);

		/// <exclude />
		void AddGroup(string groupId);

		/// <exclude />
		Room ReplaceRoom(Room room, bool addToGroupIfMissing);

		/// <exclude />
		Room ReplaceRoom(Room room);

		/// <exclude />
		void RemoveGroup(string groupId);

		/// <summary>
		/// Indicates whether the specified Group has been subscribed by the client or not.
		/// </summary>
		///
		/// <param name="groupId">The name of the Group.</param>
		///
		/// <returns><c>true</c> if the client subscribed the passed Group.</returns>
		bool ContainsGroup(string groupId);

		/// <summary>
		/// Indicates whether a Room exists in the Rooms list or not.
		/// </summary>
		///
		/// <param name="idOrName">The id or name of the Room object whose presence in the Rooms list is to be tested.</param>
		///
		/// <returns><c>true</c> if the passed Room exists in the Rooms list.</returns>
		///
		/// <seealso cref="P:Sfs2X.Entities.Room.Id" />
		bool ContainsRoom(object idOrName);

		/// <summary>
		/// Indicates whether the Rooms list contains a Room belonging to the specified Group or not.
		/// </summary>
		///
		/// <param name="idOrName">The id or name of the Room object whose presence in the Rooms list is to be tested.</param>
		/// <param name="groupId">The name of the Group to which the specified Room must belong.</param>
		///
		/// <returns><c>true</c> if the Rooms list contains the passed Room and it belongs to the specified Group.</returns>
		bool ContainsRoomInGroup(object idOrName, string groupId);

		/// <exclude />
		void ChangeRoomName(Room room, string newName);

		/// <exclude />
		void ChangeRoomPasswordState(Room room, bool isPassProtected);

		/// <exclude />
		void ChangeRoomCapacity(Room room, int maxUsers, int maxSpect);

		/// <summary>
		/// Retrieves a Room object from its id.
		/// </summary>
		///
		/// <param name="id">The id of the Room.</param>
		///
		/// <returns>An object representing the requested Room; <c>null</c> if no Room object with the passed id exists in the Rooms list.</returns>
		///
		/// <seealso cref="M:Sfs2X.Entities.Managers.IRoomManager.GetRoomByName(System.String)" />
		Room GetRoomById(int id);

		/// <summary>
		/// Retrieves a Room object from its name.
		/// </summary>
		///
		/// <param name="name">The name of the Room.</param>
		///
		/// <returns>An object representing the requested Room; <c>null</c> if no Room object with the passed name exists in the Rooms list.</returns>
		///
		/// <seealso cref="M:Sfs2X.Entities.Managers.IRoomManager.GetRoomById(System.Int32)" />
		Room GetRoomByName(string name);

		/// <summary>
		/// Returns a list of Rooms currently "known" by the client.
		/// </summary>
		///
		/// <remarks>
		/// The list contains all the Rooms that are currently joined and all the Rooms belonging to the Room Groups that have been subscribed.
		/// <para />
		/// At login time, the client automatically subscribes all the Room Groups specified in the Zone's <b>Default Room Groups</b> setting.
		/// </remarks>
		///
		/// <returns>The list of the available Room objects.</returns>
		///
		/// <seealso cref="T:Sfs2X.Requests.JoinRoomRequest" />
		/// <seealso cref="T:Sfs2X.Requests.SubscribeRoomGroupRequest" />
		/// <seealso cref="T:Sfs2X.Requests.UnsubscribeRoomGroupRequest" />
		List<Room> GetRoomList();

		/// <summary>
		/// Returns the current number of Rooms in the Rooms list.
		/// </summary>
		///
		/// <returns>The number of Rooms in the Rooms list.</returns>
		int GetRoomCount();

		/// <summary>
		/// Returns the names of Groups currently subscribed by the client.
		/// </summary>
		///
		/// <remarks>
		/// At login time, the client automatically subscribes all the Room Groups specified in the Zone's <b>Default Room Groups</b> setting.
		/// </remarks>
		///
		/// <returns>A list of Group names.</returns>
		///
		/// <seealso cref="P:Sfs2X.Entities.Room.GroupId" />
		/// <seealso cref="T:Sfs2X.Requests.SubscribeRoomGroupRequest" />
		/// <seealso cref="T:Sfs2X.Requests.UnsubscribeRoomGroupRequest" />
		List<string> GetRoomGroups();

		/// <summary>
		/// Retrieves the list of Rooms which are part of the specified Room Group.
		/// </summary>
		///
		/// <param name="groupId">The name of the Group.</param>
		///
		/// <returns>The list of Room objects belonging to the passed Group.</returns>
		List<Room> GetRoomListFromGroup(string groupId);

		/// <summary>
		/// Returns a list of Rooms currently joined by the client.
		/// </summary>
		///
		/// <returns>The list of objects representing the Rooms currently joined by the client.</returns>
		List<Room> GetJoinedRooms();

		/// <summary>
		/// Retrieves a list of Rooms joined by the specified user.
		/// </summary>
		///
		/// <remarks>
		/// The list contains only those Rooms "known" by the Room Manager; the user might have joined others the client is not aware of.
		/// </remarks>
		///
		/// <param name="user">The object representing the user to look for in the current Rooms list.</param>
		///
		/// <returns>The list of Rooms joined by the passed user.</returns>
		List<Room> GetUserRooms(User user);

		/// <exclude />
		void RemoveRoom(Room room);

		/// <exclude />
		void RemoveRoomById(int id);

		/// <exclude />
		void RemoveRoomByName(string name);

		/// <exclude />
		void RemoveUser(User user);
	}
}
