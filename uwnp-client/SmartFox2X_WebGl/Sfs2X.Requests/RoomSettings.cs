using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Requests
{
	/// <summary>
	/// The RoomSettings class is a container for the settings required to create a Room.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Requests.CreateRoomRequest" />
	public class RoomSettings
	{
		private string name;

		private string password;

		private string groupId;

		private bool isGame;

		private short maxUsers;

		private short maxSpectators;

		private short maxVariables;

		private List<RoomVariable> variables;

		private RoomPermissions permissions;

		private RoomEvents events;

		private RoomExtension extension;

		private bool allowOwnerOnlyInvitation;

		/// <summary>
		/// Defines the name of the Room.
		/// </summary>
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

		/// <summary>
		/// Defines the password of the Room.
		/// </summary>
		///
		/// <remarks>
		/// If the password is set to an empty string, the Room won't be password protected.
		/// <para />
		/// The default value is an empty string.
		/// </remarks>
		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = value;
			}
		}

		/// <summary>
		/// Indicates whether the Room is a Game Room or not.
		/// </summary>
		///
		/// <remarks>
		/// The default value is <em>false</em>.
		/// </remarks>
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

		/// <summary>
		/// Defines the maximum number of users allowed in the Room.
		/// </summary>
		///
		/// <remarks>
		/// In case of Game Rooms, this is the maximum number of players.
		/// <para />
		/// The default value is <em>10</em>.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Requests.RoomSettings.MaxSpectators" />
		public short MaxUsers
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

		/// <summary>
		/// Defines the maximum number of Room Variables allowed for the Room.
		/// </summary>
		///
		/// <remarks>
		/// The default value is <em>5</em>.
		/// </remarks>
		public short MaxVariables
		{
			get
			{
				return maxVariables;
			}
			set
			{
				maxVariables = value;
			}
		}

		/// <summary>
		/// Defines the maximum number of spectators allowed in the Room (only for Game Rooms).
		/// </summary>
		///
		/// <remarks>
		/// The default value is <em>0</em>.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Requests.RoomSettings.MaxUsers" />
		public short MaxSpectators
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

		/// <summary>
		/// Defines a list of RooomVariable objects to be attached to the Room.
		/// </summary>
		///
		/// <remarks>
		/// The default value is <c>null</c>.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Entities.Variables.RoomVariable" />
		public List<RoomVariable> Variables
		{
			get
			{
				return variables;
			}
			set
			{
				variables = value;
			}
		}

		/// <summary>
		/// Defines the flags indicating which operations are permitted on the Room.
		/// </summary>
		///
		/// <remarks>
		/// Permissions include: name and password change, maximum users change and public messaging. If set to <c>null</c>,
		/// the permissions configured on the server-side are used (see the SmartFoxServer 2X Administration Tool documentation).
		/// <para />
		/// The default value is <c>null</c>.
		/// </remarks>
		public RoomPermissions Permissions
		{
			get
			{
				return permissions;
			}
			set
			{
				permissions = value;
			}
		}

		/// <summary>
		/// Defines the flags indicating which events related to the Room are dispatched by the <em>SmartFox</em> client.
		/// </summary>
		///
		/// <remarks>
		/// Room events include: users entering or leaving the room, user count change and user variables update. If set to <c>null</c>,
		/// the events configured on the server-side are used (see the SmartFoxServer 2X Administration Tool documentation).
		/// <para />
		/// The default value is <c>null</c>.
		/// </remarks>
		public RoomEvents Events
		{
			get
			{
				return events;
			}
			set
			{
				events = value;
			}
		}

		/// <summary>
		/// Defines the Extension that must be attached to the Room on the server-side, and its settings.
		/// </summary>
		public RoomExtension Extension
		{
			get
			{
				return extension;
			}
			set
			{
				extension = value;
			}
		}

		/// <summary>
		/// Defines the id of the Group to which the Room should belong.
		/// </summary>
		///
		/// <remarks>
		/// If the Group doesn't exist yet, a new one is created before assigning the Room to it.
		/// <para />
		/// The default value is <em>default</em>.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.Room.GroupId" />
		public string GroupId
		{
			get
			{
				return groupId;
			}
			set
			{
				groupId = value;
			}
		}

		/// <summary>
		/// Specifies if the Room allows "Join Room" invitations to be sent by any user or just by its owner.
		/// </summary>
		///
		/// <remarks>
		/// The default value is <em>true</em>.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Requests.Game.JoinRoomInvitationRequest" />
		public bool AllowOwnerOnlyInvitation
		{
			get
			{
				return allowOwnerOnlyInvitation;
			}
			set
			{
				allowOwnerOnlyInvitation = value;
			}
		}

		/// <summary>
		/// Creates a new RoomSettings instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="T:Sfs2X.Requests.CreateRoomRequest" /> class constructor.
		/// </remarks>
		///
		/// <param name="name">The name of the Room to be created.</param>
		///
		/// <seealso cref="T:Sfs2X.Requests.CreateRoomRequest" />
		public RoomSettings(string name)
		{
			this.name = name;
			password = "";
			isGame = false;
			maxUsers = 10;
			maxSpectators = 0;
			maxVariables = 5;
			groupId = SFSConstants.DEFAULT_GROUP_ID;
			variables = new List<RoomVariable>();
			allowOwnerOnlyInvitation = true;
		}
	}
}
