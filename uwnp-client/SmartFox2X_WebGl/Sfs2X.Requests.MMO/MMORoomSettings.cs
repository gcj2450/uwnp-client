using Sfs2X.Entities.Data;

namespace Sfs2X.Requests.MMO
{
	/// <summary>
	/// The MMORoomSettings class is a container for the settings required to create an MMORoom using the CreateRoomRequest request.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Requests.CreateRoomRequest" />
	/// <seealso cref="T:Sfs2X.Entities.MMORoom" />
	public class MMORoomSettings : RoomSettings
	{
		private Vec3D defaultAOI;

		private MapLimits mapLimits;

		private int userMaxLimboSeconds = 50;

		private int proximityListUpdateMillis = 250;

		private bool sendAOIEntryPoint = true;

		/// <summary>
		/// Defines the Area of Interest (AoI) for the MMORoom.
		/// </summary>
		///
		/// <remarks>
		/// This value represents the area/range around the user that will be affected by server events and other users events.
		/// It is represented by a <em>Vec3D</em> object providing 2D or 3D coordinates.
		/// <para />
		/// Setting this value is mandatory.
		/// </remarks>
		///
		/// <example>
		/// A <c>Vec3D(50,50)</c> describes a range of 50 units (e.g. pixels) in all four directions (top, bottom, left, right) with respect to the user position in a 2D coordinates system.
		///
		/// A <c>Vec3D(120,120,60)</c> describes a range of 120 units in all four directions (top, bottom, left, right) and 60 units along the two Z-axis directions (backward, forward) with respect to the user position in a 3D coordinates system.
		/// </example>
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
		/// Defines the limits of the virtual environment represented by the MMORoom.
		/// </summary>
		///
		/// <remarks>
		/// When specified, this property must contain two non-null <em>Vec3D</em> objects representing the minimum and maximum limits of the 2D/3D coordinates systems.
		/// Any positional value that falls outside the provided limit will be refused by the server.
		/// <para />
		/// This setting is optional but its usage is highly recommended.
		/// </remarks>
		public MapLimits MapLimits
		{
			get
			{
				return mapLimits;
			}
			set
			{
				mapLimits = value;
			}
		}

		/// <summary>
		/// Defines the time limit before a user without a physical position set inside the MMORoom is kicked from the Room.
		/// </summary>
		///
		/// <remarks>
		/// As soon as the MMORoom is joined, the user still doesn't have a physical position set in the coordinates system, therefore it is
		/// considered in a "limbo" state. At this point the user is expected to set his position (via the <see cref="T:Sfs2X.Requests.MMO.SetUserPositionRequest" /> request) within the amount of seconds expressed by this value.
		/// <para />
		/// The default value is <c>50</c> seconds.
		/// </remarks>
		public int UserMaxLimboSeconds
		{
			get
			{
				return userMaxLimboSeconds;
			}
			set
			{
				userMaxLimboSeconds = value;
			}
		}

		/// <summary>
		/// Configures the speed at which the PROXIMITY_LIST_UPDATE event is sent by the server.
		/// </summary>
		///
		/// <remarks>
		/// In an MMORoom, the regular users list is replaced by a proximity list, which keeps an updated view of the users currently within the Area of Interest
		/// of the current user. The speed at which these updates are fired by the server is regulated by this parameter, which sets the minimum time between two subsequent updates.
		/// <para />
		/// The default value is <c>250</c> milliseconds.
		/// <para />
		/// <b>NOTE:</b> values below the default might be unnecessary for most applications unless they are in realtime.
		/// </remarks>
		///
		/// <seealso cref="F:Sfs2X.Core.SFSEvent.PROXIMITY_LIST_UPDATE" />
		public int ProximityListUpdateMillis
		{
			get
			{
				return proximityListUpdateMillis;
			}
			set
			{
				proximityListUpdateMillis = value;
			}
		}

		/// <summary>
		/// Sets if the users entry points in the current user's Area of Interest should be transmitted in the PROXIMITY_LIST_UPDATE event.
		/// </summary>
		///
		/// <remarks>
		/// If this setting is set to <c>true</c>, when a user enters the AoI of another user, the server will also send the coordinates
		/// at which the former "appeared" within the AoI. This option should be turned off in case these coordinates are not needed, in order to save bandwidth.
		/// <para />
		/// The default value is <c>true</c>.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.User.AOIEntryPoint" />
		/// <seealso cref="P:Sfs2X.Entities.MMOItem.AOIEntryPoint" />
		/// <seealso cref="F:Sfs2X.Core.SFSEvent.PROXIMITY_LIST_UPDATE" />
		public bool SendAOIEntryPoint
		{
			get
			{
				return sendAOIEntryPoint;
			}
			set
			{
				sendAOIEntryPoint = value;
			}
		}

		/// <summary>
		/// Creates a new MMORoomSettings instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="T:Sfs2X.Requests.CreateRoomRequest" /> class constructor.
		/// </remarks>
		///
		/// <param name="name">The name of the MMORoom to be created.</param>
		///
		/// <seealso cref="T:Sfs2X.Requests.CreateRoomRequest" />
		public MMORoomSettings(string name)
			: base(name)
		{
		}
	}
}
