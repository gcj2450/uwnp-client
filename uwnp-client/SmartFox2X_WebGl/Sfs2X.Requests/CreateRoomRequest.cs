using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;
using Sfs2X.Requests.MMO;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Creates a new Room in the current Zone.
	/// </summary>
	///
	/// <remarks>
	/// If the creation is successful, a <see cref="F:Sfs2X.Core.SFSEvent.ROOM_ADD" /> event is dispatched to all the users who subscribed the Group to which the Room is associated,
	/// including the Room creator. Otherwise, a <see cref="F:Sfs2X.Core.SFSEvent.ROOM_CREATION_ERROR" /> event is returned to the creator's client.
	/// </remarks>
	///
	/// <example>
	/// The following example creates a new chat room:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_ADDED, OnRoomAdded);
	/// 	sfs.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
	///
	/// 	// Create a new Chat Room
	/// 	RoomSettings settings = new RoomSettings("My Chat Room");
	/// 	settings.MaxUsers = 40;
	/// 	settings.GroupId = "chats";
	///
	/// 	sfs.Send( new CreateRoomRequest(settings) );
	/// }
	///
	/// void OnRoomAdded(BaseEvent evt) {
	/// 	Console.WriteLine("Room created: " + (Room)evt.Params["room"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room created: " + (Room)evt.Params["room"]);        // UWP
	/// }
	///
	/// void OnRoomCreationError(BaseEvent evt) {
	/// 	Console.WriteLine("Room creation failed: " + (string)evt.Params["errorMessage"]);                       // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room creation failed: " + (string)evt.Params["errorMessage"]);      // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_ADD" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_CREATION_ERROR" />
	public class CreateRoomRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ROOM = "r";

		/// <exclude />
		public static readonly string KEY_NAME = "n";

		/// <exclude />
		public static readonly string KEY_PASSWORD = "p";

		/// <exclude />
		public static readonly string KEY_GROUP_ID = "g";

		/// <exclude />
		public static readonly string KEY_ISGAME = "ig";

		/// <exclude />
		public static readonly string KEY_MAXUSERS = "mu";

		/// <exclude />
		public static readonly string KEY_MAXSPECTATORS = "ms";

		/// <exclude />
		public static readonly string KEY_MAXVARS = "mv";

		/// <exclude />
		public static readonly string KEY_ROOMVARS = "rv";

		/// <exclude />
		public static readonly string KEY_PERMISSIONS = "pm";

		/// <exclude />
		public static readonly string KEY_EVENTS = "ev";

		/// <exclude />
		public static readonly string KEY_EXTID = "xn";

		/// <exclude />
		public static readonly string KEY_EXTCLASS = "xc";

		/// <exclude />
		public static readonly string KEY_EXTPROP = "xp";

		/// <exclude />
		public static readonly string KEY_AUTOJOIN = "aj";

		/// <exclude />
		public static readonly string KEY_ROOM_TO_LEAVE = "rl";

		/// <exclude />
		public static readonly string KEY_ALLOW_JOIN_INVITATION_BY_OWNER = "aji";

		/// <exclude />
		public static readonly string KEY_MMO_DEFAULT_AOI = "maoi";

		/// <exclude />
		public static readonly string KEY_MMO_MAP_LOW_LIMIT = "mllm";

		/// <exclude />
		public static readonly string KEY_MMO_MAP_HIGH_LIMIT = "mlhm";

		/// <exclude />
		public static readonly string KEY_MMO_USER_MAX_LIMBO_SECONDS = "muls";

		/// <exclude />
		public static readonly string KEY_MMO_PROXIMITY_UPDATE_MILLIS = "mpum";

		/// <exclude />
		public static readonly string KEY_MMO_SEND_ENTRY_POINT = "msep";

		private RoomSettings settings;

		private bool autoJoin;

		private Room roomToLeave;

		private void Init(RoomSettings settings, bool autoJoin, Room roomToLeave)
		{
			this.settings = settings;
			this.autoJoin = autoJoin;
			this.roomToLeave = roomToLeave;
		}

		/// <summary>
		/// Creates a new CreateRoomRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="settings">An object containing the Room configuration settings.</param>
		/// <param name="autoJoin">If <c>true</c>, the Room is joined as soon as it is created (default = <c>false</c>).</param>
		/// <param name="roomToLeave">An object representing the Room that should be left if the new Room is auto-joined (default = <c>null</c>).</param>
		///
		/// <seealso cref="T:Sfs2X.Requests.RoomSettings" />
		/// <seealso cref="T:Sfs2X.Requests.Game.SFSGameSettings" />
		/// <seealso cref="T:Sfs2X.Requests.MMO.MMORoomSettings" />
		public CreateRoomRequest(RoomSettings settings, bool autoJoin, Room roomToLeave)
			: base(RequestType.CreateRoom)
		{
			Init(settings, autoJoin, roomToLeave);
		}

		/// <summary>
		/// See <em>CreateRoomRequest(RoomSettings, bool, Room)</em> constructor.
		/// </summary>
		public CreateRoomRequest(RoomSettings settings, bool autoJoin)
			: base(RequestType.CreateRoom)
		{
			Init(settings, autoJoin, null);
		}

		/// <summary>
		/// See <em>CreateRoomRequest(RoomSettings, bool, Room)</em> constructor.
		/// </summary>
		public CreateRoomRequest(RoomSettings settings)
			: base(RequestType.CreateRoom)
		{
			Init(settings, false, null);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (settings.Name == null || settings.Name.Length == 0)
			{
				list.Add("Missing room name");
			}
			if (settings.MaxUsers <= 0)
			{
				list.Add("maxUsers must be > 0");
			}
			if (settings.Extension != null)
			{
				if (settings.Extension.ClassName == null || settings.Extension.ClassName.Length == 0)
				{
					list.Add("Missing Extension class name");
				}
				if (settings.Extension.Id == null || settings.Extension.Id.Length == 0)
				{
					list.Add("Missing Extension id");
				}
			}
			if (settings is MMORoomSettings)
			{
				MMORoomSettings mMORoomSettings = settings as MMORoomSettings;
				if (mMORoomSettings.DefaultAOI == null)
				{
					list.Add("Missing default AOI (area of interest)");
				}
				if (mMORoomSettings.MapLimits != null && (mMORoomSettings.MapLimits.LowerLimit == null || mMORoomSettings.MapLimits.HigherLimit == null))
				{
					list.Add("Map limits must be both defined");
				}
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("CreateRoom request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_NAME, settings.Name);
			sfso.PutUtfString(KEY_GROUP_ID, settings.GroupId);
			sfso.PutUtfString(KEY_PASSWORD, settings.Password);
			sfso.PutBool(KEY_ISGAME, settings.IsGame);
			sfso.PutShort(KEY_MAXUSERS, settings.MaxUsers);
			sfso.PutShort(KEY_MAXSPECTATORS, settings.MaxSpectators);
			sfso.PutShort(KEY_MAXVARS, settings.MaxVariables);
			sfso.PutBool(KEY_ALLOW_JOIN_INVITATION_BY_OWNER, settings.AllowOwnerOnlyInvitation);
			if (settings.Variables != null && settings.Variables.Count > 0)
			{
				ISFSArray iSFSArray = SFSArray.NewInstance();
				foreach (RoomVariable variable in settings.Variables)
				{
					if (variable is RoomVariable)
					{
						RoomVariable roomVariable = variable as RoomVariable;
						iSFSArray.AddSFSArray(roomVariable.ToSFSArray());
					}
				}
				sfso.PutSFSArray(KEY_ROOMVARS, iSFSArray);
			}
			if (settings.Permissions != null)
			{
				List<bool> list = new List<bool>();
				list.Add(settings.Permissions.AllowNameChange);
				list.Add(settings.Permissions.AllowPasswordStateChange);
				list.Add(settings.Permissions.AllowPublicMessages);
				list.Add(settings.Permissions.AllowResizing);
				sfso.PutBoolArray(KEY_PERMISSIONS, list.ToArray());
			}
			if (settings.Events != null)
			{
				List<bool> list2 = new List<bool>();
				list2.Add(settings.Events.AllowUserEnter);
				list2.Add(settings.Events.AllowUserExit);
				list2.Add(settings.Events.AllowUserCountChange);
				list2.Add(settings.Events.AllowUserVariablesUpdate);
				sfso.PutBoolArray(KEY_EVENTS, list2.ToArray());
			}
			if (settings.Extension != null)
			{
				sfso.PutUtfString(KEY_EXTID, settings.Extension.Id);
				sfso.PutUtfString(KEY_EXTCLASS, settings.Extension.ClassName);
				if (settings.Extension.PropertiesFile != null && settings.Extension.PropertiesFile.Length > 0)
				{
					sfso.PutUtfString(KEY_EXTPROP, settings.Extension.PropertiesFile);
				}
			}
			if (settings is MMORoomSettings)
			{
				MMORoomSettings mMORoomSettings = settings as MMORoomSettings;
				if (mMORoomSettings.DefaultAOI.IsFloat())
				{
					sfso.PutFloatArray(KEY_MMO_DEFAULT_AOI, mMORoomSettings.DefaultAOI.ToFloatArray());
					if (mMORoomSettings.MapLimits != null)
					{
						sfso.PutFloatArray(KEY_MMO_MAP_LOW_LIMIT, mMORoomSettings.MapLimits.LowerLimit.ToFloatArray());
						sfso.PutFloatArray(KEY_MMO_MAP_HIGH_LIMIT, mMORoomSettings.MapLimits.HigherLimit.ToFloatArray());
					}
				}
				else
				{
					sfso.PutIntArray(KEY_MMO_DEFAULT_AOI, mMORoomSettings.DefaultAOI.ToIntArray());
					if (mMORoomSettings.MapLimits != null)
					{
						sfso.PutIntArray(KEY_MMO_MAP_LOW_LIMIT, mMORoomSettings.MapLimits.LowerLimit.ToIntArray());
						sfso.PutIntArray(KEY_MMO_MAP_HIGH_LIMIT, mMORoomSettings.MapLimits.HigherLimit.ToIntArray());
					}
				}
				sfso.PutShort(KEY_MMO_USER_MAX_LIMBO_SECONDS, (short)mMORoomSettings.UserMaxLimboSeconds);
				sfso.PutShort(KEY_MMO_PROXIMITY_UPDATE_MILLIS, (short)mMORoomSettings.ProximityListUpdateMillis);
				sfso.PutBool(KEY_MMO_SEND_ENTRY_POINT, mMORoomSettings.SendAOIEntryPoint);
			}
			sfso.PutBool(KEY_AUTOJOIN, autoJoin);
			if (roomToLeave != null)
			{
				sfso.PutInt(KEY_ROOM_TO_LEAVE, roomToLeave.Id);
			}
		}
	}
}
