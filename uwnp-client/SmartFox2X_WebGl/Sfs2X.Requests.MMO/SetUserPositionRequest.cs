using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.MMO
{
	/// <summary>
	/// Updates the User position inside an MMORoom.
	/// </summary>
	///
	/// <remarks>
	/// MMORooms represent virtual environments and can host any number of users. Based on their position, the system allows users within a certain range
	/// from each other (Area of Interest, or AoI) to interact.
	/// <para />
	/// This request allows the current user to update his position inside the MMORoom, which in turn will trigger a
	/// <see cref="F:Sfs2X.Core.SFSEvent.PROXIMITY_LIST_UPDATE">PROXIMITY_LIST_UPDATE</see> event for all users that fall within his AoI.
	/// </remarks>
	///
	/// <example>
	/// The following example changes the position of the user in a 2D coordinates space and handles the related event:
	/// <code>
	/// private void UpdatePlayerPosition(int px, int py)
	/// {
	/// 	var newPos = new Vec3D(px, py);
	/// 	sfs.Send(new SetUserPositionRequest(newPos));
	/// }
	///
	/// private void OnProximityListUpdate(BaseEvent evt)
	/// {
	/// 	var added = (List&lt;User&gt;) evt.params["addedUsers"];
	/// 	var removed = (List&lt;User&gt;) evt.params["removedUsers"];
	///
	/// 	// Add users that entered the proximity list
	/// 	foreach (User user in added)
	/// 	{
	/// 		// Obtain the coordinates at which the user "appeared" in our range
	/// 		Vec3D entryPoint = user.AoiEntryPoint;
	///
	/// 		// Add new avatar in the scene
	/// 		var avatarSprite = new AvatarSprite();
	/// 		avatarSprite.x = entryPoint.px;
	/// 		avatarSprite.y = entryPoint.py;
	/// 		...
	/// 	}
	///
	/// 	// Remove users that left the proximity list
	/// 	foreach (User user in removed)
	/// 	{
	/// 		// Remove the avatar from the scene
	/// 		...
	/// 	}
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.PROXIMITY_LIST_UPDATE" />
	/// <seealso cref="T:Sfs2X.Entities.MMORoom" />
	public class SetUserPositionRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ROOM = "r";

		/// <exclude />
		public static readonly string KEY_VEC3D = "v";

		/// <exclude />
		public static readonly string KEY_PLUS_USER_LIST = "p";

		/// <exclude />
		public static readonly string KEY_MINUS_USER_LIST = "m";

		/// <exclude />
		public static readonly string KEY_PLUS_ITEM_LIST = "q";

		/// <exclude />
		public static readonly string KEY_MINUS_ITEM_LIST = "n";

		private Vec3D pos;

		private Room room;

		/// <summary>
		/// Creates a new SetUserPositionRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="position">The user position.</param>
		/// <param name="room">The <em>MMORoom</em> object corresponding to the Room where the position should be set; if <c>null</c>, the last Room joined by the user is used.</param>
		public SetUserPositionRequest(Vec3D position, Room room)
			: base(RequestType.SetUserPosition)
		{
			this.room = room;
			pos = position;
		}

		/// <summary>
		/// See <em>SetUserPositionRequest(Vec3D, Room)</em> constructor.
		/// </summary>
		public SetUserPositionRequest(Vec3D position)
			: this(position, null)
		{
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (pos == null)
			{
				list.Add("Position must be a valid Vec3D ");
			}
			if (room == null)
			{
				room = sfs.LastJoinedRoom;
			}
			if (room == null)
			{
				list.Add("You are not joined in any room");
			}
			if (!(room is MMORoom))
			{
				list.Add("Selected Room is not an MMORoom");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("SetUserVariables request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_ROOM, room.Id);
			if (pos.IsFloat())
			{
				sfso.PutFloatArray(KEY_VEC3D, pos.ToFloatArray());
			}
			else
			{
				sfso.PutIntArray(KEY_VEC3D, pos.ToIntArray());
			}
		}
	}
}
