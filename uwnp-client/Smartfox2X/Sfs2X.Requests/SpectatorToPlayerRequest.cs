using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Turns the current user from spectator to player in a Game Room.
	/// </summary>
	///
	/// <remarks>
	/// If the operation is successful, all the users in the target Room are notified with the <see cref="F:Sfs2X.Core.SFSEvent.SPECTATOR_TO_PLAYER" /> event.
	/// The operation could fail if no player slots are available in the Game Room at the time of the request;
	/// in this case the <see cref="F:Sfs2X.Core.SFSEvent.SPECTATOR_TO_PLAYER_ERROR" /> event is dispatched to the requester's client.
	/// </remarks>
	///
	/// <example>
	/// The following example turns the current user from spectator to player in the last joined Game Room:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.SPECTATOR_TO_PLAYER, OnSpectatorToPlayerSwitch);
	/// 	sfs.AddEventListener(SFSEvent.SPECTATOR_TO_PLAYER_ERROR, OnSpectatorToPlayerSwitchError);
	///
	/// 	// Switch spectator to player
	/// 	sfs.Send( new PlayerToSpectatorRequest() );
	/// }
	///
	/// void OnSpectatorToPlayerSwitch(BaseEvent evt) {
	/// 	User user = (User)evt.Params["user"];
	/// 	Console.WriteLine("Spectator " + user.Name + " is now a player");                           // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Spectator " + user.Name + " is now a player");          // UWP
	/// }
	///
	/// void OnSpectatorToPlayerSwitchError(BaseEvent evt) {
	/// 	Console.WriteLine("Unable to become a player due to the following error: " + (string)evt.Params["errorMessage"]);                       // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Unable to become a player due to the following error: " + (string)evt.Params["errorMessage"]);      // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.SPECTATOR_TO_PLAYER" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.SPECTATOR_TO_PLAYER_ERROR" />
	/// <seealso cref="T:Sfs2X.Requests.PlayerToSpectatorRequest" />
	public class SpectatorToPlayerRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ROOM_ID = "r";

		/// <exclude />
		public static readonly string KEY_USER_ID = "u";

		/// <exclude />
		public static readonly string KEY_PLAYER_ID = "p";

		private Room room;

		private void Init(Room targetRoom)
		{
			room = targetRoom;
		}

		/// <summary>
		/// Creates a new SpectatorToPlayerRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="targetRoom">The object corresponding to the Room in which the spectator should be turned to player. If <c>null</c>, the last Room joined by the user is used (default = <c>null</c>).</param>
		public SpectatorToPlayerRequest(Room targetRoom)
			: base(RequestType.SpectatorToPlayer)
		{
			Init(targetRoom);
		}

		/// <summary>
		/// See <em>SpectatorToPlayerRequest(Room)</em> constructor.
		/// </summary>
		public SpectatorToPlayerRequest()
			: base(RequestType.SpectatorToPlayer)
		{
			Init(null);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (sfs.JoinedRooms.Count < 1)
			{
				list.Add("You are not joined in any rooms");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("LeaveRoom request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			if (room == null)
			{
				room = sfs.LastJoinedRoom;
			}
			sfso.PutInt(KEY_ROOM_ID, room.Id);
		}
	}
}
