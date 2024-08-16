namespace Sfs2X.Requests
{
	/// <summary>
	/// The RoomPermissions class contains a specific subset of the RoomSettings required to create a Room.
	/// </summary>
	///
	/// <remarks>
	/// This class defines which operations users will be able to execute on the Room after its creation.
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.Requests.RoomSettings.Permissions" />
	/// <seealso cref="T:Sfs2X.Requests.CreateRoomRequest" />
	public class RoomPermissions
	{
		private bool allowNameChange;

		private bool allowPasswordStateChange;

		private bool allowPublicMessages;

		private bool allowResizing;

		/// <summary>
		/// Indicates whether changing the Room name after its creation is allowed or not.
		/// </summary>
		///
		/// <remarks>
		/// The Room name can be changed by means of the <see cref="T:Sfs2X.Requests.ChangeRoomNameRequest" /> request.
		/// <para />
		/// The default value is <c>false</c>.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Requests.ChangeRoomNameRequest" />
		public bool AllowNameChange
		{
			get
			{
				return allowNameChange;
			}
			set
			{
				allowNameChange = value;
			}
		}

		/// <summary>
		/// Indicates whether changing (or removing) the Room password after its creation is allowed or not.
		/// </summary>
		///
		/// <remarks>
		/// The Room password can be changed by means of the <see cref="T:Sfs2X.Requests.ChangeRoomPasswordStateRequest" /> request.
		/// <para />
		/// The default value is <c>false</c>.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Requests.ChangeRoomPasswordStateRequest" />
		public bool AllowPasswordStateChange
		{
			get
			{
				return allowPasswordStateChange;
			}
			set
			{
				allowPasswordStateChange = value;
			}
		}

		/// <summary>
		/// Indicates whether users inside the Room are allowed to send public messages or not.
		/// </summary>
		///
		/// <remarks>
		/// Public messages can be sent by means of the <see cref="T:Sfs2X.Requests.PublicMessageRequest" /> request.
		/// <para />
		/// The default value is <c>false</c>.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Requests.PublicMessageRequest" />
		public bool AllowPublicMessages
		{
			get
			{
				return allowPublicMessages;
			}
			set
			{
				allowPublicMessages = value;
			}
		}

		/// <summary>
		/// Indicates whether the Room capacity can be changed after its creation or not.
		/// </summary>
		///
		/// <remarks>
		/// The capacity is the maximum number of users and spectators (in Game Rooms) allowed to enter the Room. It can be changed by means of the <see cref="T:Sfs2X.Requests.ChangeRoomCapacityRequest" /> request.
		/// <para />
		/// The default value is <c>false</c>.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Requests.ChangeRoomCapacityRequest" />
		public bool AllowResizing
		{
			get
			{
				return allowResizing;
			}
			set
			{
				allowResizing = value;
			}
		}

		/// <summary>
		/// Creates a new RoomPermissions instance.
		/// </summary>
		///
		/// <remarks>
		/// The <see cref="P:Sfs2X.Requests.RoomSettings.Permissions">RoomSettings.Permissions</see> property must be set to this instance during Room creation.
		/// </remarks>
		public RoomPermissions()
		{
		}
	}
}
