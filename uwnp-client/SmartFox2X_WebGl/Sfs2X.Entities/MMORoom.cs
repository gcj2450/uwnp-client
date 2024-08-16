using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Sfs2X.Entities
{
	/// <summary>
	/// The MMORoom object represents a specialized type of Room entity on the client.
	/// </summary>
	///
	/// <remarks>
	/// The MMORoom is ideal for huge virtual worlds and MMO games because it works with proximity lists instead of "regular" users lists.
	/// This allows thousands of users to interact with each other based on their Area of Interest (AoI). The AoI represents a range around the user
	/// that is affected by server and user events, outside which no other events are received.
	/// <para />
	/// The size of the AoI is set at Room creation time and it is the same for all users who joined it.
	/// Supposing that the MMORoom hosts a 3D virtual world, setting an AoI of (x=100, y=100, z=40) for the Room tells the server to transmit updates and broadcast
	/// events to and from those users that fall within the AoI range around the current user; this means the area within +/- 100 units on the X axis, +/- 100 units on the Y axis and +/- 40 units on the Z axis.
	/// <para />
	/// As the user moves around in the virtual environment, he can update his position in the corresponding MMORoom and thus continuously receive events
	/// about other users (and items - see below) entering and leaving his AoI.
	/// The player will be able to update his position via the <em>SetUserPositionRequest</em> request and receive updates on his current proximity list by means of the
	/// <see cref="F:Sfs2X.Core.SFSEvent.PROXIMITY_LIST_UPDATE">PROXIMITY_LIST_UPDATE</see> event.
	/// <para />
	/// Finally, MMORooms can also host any number of "MMOItems" which represent dynamic non-player objects that users can interact with.
	/// They are handled by the MMORoom using the same rules of visibility described before.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Requests.CreateRoomRequest" />
	/// <seealso cref="T:Sfs2X.Requests.MMO.MMORoomSettings" />
	/// <seealso cref="T:Sfs2X.Requests.MMO.SetUserPositionRequest" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.PROXIMITY_LIST_UPDATE" />
	/// <seealso cref="T:Sfs2X.Entities.MMOItem" />
	public class MMORoom : SFSRoom
	{
		private Vec3D defaultAOI;

		private Vec3D lowerMapLimit;

		private Vec3D higherMapLimit;

		private Dictionary<int, IMMOItem> itemsById = new Dictionary<int, IMMOItem>();

		/// <summary>
		/// Returns the default Area of Interest (AoI) of this MMORoom.
		/// </summary>
		///
		/// <seealso cref="P:Sfs2X.Requests.MMO.MMORoomSettings.DefaultAOI" />
		public Vec3D DefaultAOI
		{
			get
			{
				return defaultAOI;
			}
			set
			{
				defaultAOI = value;
			}
		}

		/// <summary>
		/// Returns the lower coordinates limit of the virtual environment represented by the MMORoom along the X,Y,Z axes.
		/// </summary>
		///
		/// <remarks>
		/// If <c>null</c> is returned, then no limits were set at Room creation time.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Requests.MMO.MMORoomSettings.MapLimits" />
		public Vec3D LowerMapLimit
		{
			get
			{
				return lowerMapLimit;
			}
			set
			{
				lowerMapLimit = value;
			}
		}

		/// <summary>
		/// Returns the higher coordinates limit of the virtual environment represented by the MMORoom along the X,Y,Z axes.
		/// </summary>
		///
		/// <remarks>
		/// If <c>null</c> is returned, then no limits were set at Room creation time.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Requests.MMO.MMORoomSettings.MapLimits" />
		public Vec3D HigherMapLimit
		{
			get
			{
				return higherMapLimit;
			}
			set
			{
				higherMapLimit = value;
			}
		}

		/// <exclude />
		public MMORoom(int id, string name, string groupId)
			: base(id, name, groupId)
		{
		}

		/// <exclude />
		public MMORoom(int id, string name)
			: base(id, name)
		{
		}

		/// <summary>
		/// Retrieves an <em>MMOItem</em> object from its <em>id</em> property.
		/// </summary>
		///
		/// <remarks>
		/// The item is available to the current user if it falls within his Area of Interest only.
		/// </remarks>
		///
		/// <param name="id">The id of the item to be retrieved.</param>
		///
		/// <returns>An <em>MMOItem</em> object, or <c>null</c> if the item with the passed id is not in proximity of the current user.</returns>
		public IMMOItem GetMMOItem(int id)
		{
			IMMOItem value;
			itemsById.TryGetValue(id, out value);
			return value;
		}

		/// <summary>
		/// Retrieves all <em>MMOItem</em> object in the MMORoom that fall within the current user's Area of Interest.
		/// </summary>
		///
		/// <returns>A list of <em>MMOItem</em> objects, or an empty list if no item is in proximity of the current user.</returns>
		public List<IMMOItem> GetMMOItems()
		{
			return new List<IMMOItem>(itemsById.Values);
		}

		/// <exclude />
		public void AddMMOItem(IMMOItem item)
		{
			itemsById[item.Id] = item;
		}

		/// <exclude />
		public void RemoveItem(int id)
		{
			itemsById.Remove(id);
		}
	}
}
