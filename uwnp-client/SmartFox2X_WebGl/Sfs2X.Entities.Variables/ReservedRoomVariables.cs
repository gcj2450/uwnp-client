namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The ReservedRoomVariables class contains the costants describing the SmartFoxServer API reserved Room Variable names.
	/// </summary>
	public class ReservedRoomVariables
	{
		/// <summary>
		/// The Room Variable with this name keeps track of the state (started or stopped) of a game created with the <see cref="T:Sfs2X.Requests.Game.CreateSFSGameRequest" /> request.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Requests.Game.CreateSFSGameRequest" />
		/// <seealso cref="P:Sfs2X.Requests.Game.SFSGameSettings.NotifyGameStarted" />
		public static readonly string RV_GAME_STARTED = "$GS";
	}
}
