namespace Sfs2X.Requests
{
	/// <summary>
	/// The RoomEvents class contains a specific subset of the RoomSettings required to create a Room.
	/// </summary>
	///
	/// <remarks>
	/// This class defines which events related to the Room will be fired by the SmartFox client.
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.Requests.RoomSettings.Events" />
	/// <seealso cref="T:Sfs2X.Requests.CreateRoomRequest" />
	public class RoomEvents
	{
		private bool allowUserEnter;

		private bool allowUserExit;

		private bool allowUserCountChange;

		private bool allowUserVariablesUpdate;

		/// <summary>
		/// Indicates whether the <see cref="F:Sfs2X.Core.SFSEvent.USER_ENTER_ROOM" /> event should be dispatched whenever a user joins the Room or not.
		/// </summary>
		///
		/// <remarks>
		/// The default value is <c>false</c>.
		/// </remarks>
		///
		/// <seealso cref="F:Sfs2X.Core.SFSEvent.USER_ENTER_ROOM" />
		public bool AllowUserEnter
		{
			get
			{
				return allowUserEnter;
			}
			set
			{
				allowUserEnter = value;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="F:Sfs2X.Core.SFSEvent.USER_EXIT_ROOM" /> event should be dispatched whenever a user leaves the Room or not.
		/// </summary>
		///
		/// <remarks>
		/// The default value is <c>false</c>.
		/// </remarks>
		///
		/// <seealso cref="F:Sfs2X.Core.SFSEvent.USER_ENTER_ROOM" />
		public bool AllowUserExit
		{
			get
			{
				return allowUserExit;
			}
			set
			{
				allowUserExit = value;
			}
		}

		/// <summary>
		/// Indicates whether or not the <see cref="F:Sfs2X.Core.SFSEvent.USER_COUNT_CHANGE" /> event should be dispatched whenever the users (or players+spectators) count changes in the Room.
		/// </summary>
		///
		/// <remarks>
		/// The default value is <c>false</c>.
		/// </remarks>
		///
		/// <seealso cref="F:Sfs2X.Core.SFSEvent.USER_COUNT_CHANGE" />
		public bool AllowUserCountChange
		{
			get
			{
				return allowUserCountChange;
			}
			set
			{
				allowUserCountChange = value;
			}
		}

		/// <summary>
		/// Indicates whether or not the <see cref="F:Sfs2X.Core.SFSEvent.USER_VARIABLES_UPDATE" /> event should be dispatched whenever a user in the Room updates his User Variables.
		/// </summary>
		///
		/// <remarks>
		/// The default value is <c>false</c>.
		/// </remarks>
		///
		/// <seealso cref="F:Sfs2X.Core.SFSEvent.USER_VARIABLES_UPDATE" />
		public bool AllowUserVariablesUpdate
		{
			get
			{
				return allowUserVariablesUpdate;
			}
			set
			{
				allowUserVariablesUpdate = value;
			}
		}

		/// <summary>
		/// Creates a new RoomEvents instance.
		/// </summary>
		///
		/// <remarks>
		/// The <see cref="P:Sfs2X.Requests.RoomSettings.Events">RoomSettings.Events</see> property must be set to this instance during Room creation.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Requests.RoomSettings.Events" />
		public RoomEvents()
		{
			allowUserEnter = false;
			allowUserExit = false;
			allowUserCountChange = false;
			allowUserVariablesUpdate = false;
		}
	}
}
