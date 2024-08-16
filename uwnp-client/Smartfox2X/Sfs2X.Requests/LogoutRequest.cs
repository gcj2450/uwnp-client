using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Logs the user out of the current server Zone.
	/// </summary>
	///
	/// <remarks>
	/// The user is notified of the logout operation by means of the <see cref="F:Sfs2X.Core.SFSEvent.LOGOUT" /> event. This doesn't shut down the connection,
	/// so the user will be able to login again in the same Zone or in a different one right after the confirmation event.
	/// </remarks>
	///
	/// <example>
	/// The following example performs a logout from the current Zone:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.LOGOUT, onLogout);
	///
	/// 	// Logout
	/// 	sfs.Send( new LogoutRequest() );
	/// }
	///
	/// void onLogout(BaseEvent evt) {
	/// 	Console.WriteLine("Logout executed!");                          // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Logout executed!");         // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.LOGOUT" />
	/// <seealso cref="T:Sfs2X.Requests.LoginRequest" />
	public class LogoutRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ZONE_NAME = "zn";

		/// <summary>
		/// Creates a new LogoutRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		public LogoutRequest()
			: base(RequestType.Logout)
		{
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			if (sfs.MySelf == null)
			{
				throw new SFSValidationError("LogoutRequest Error", new string[1] { "You are not logged in at the moment!" });
			}
		}
	}
}
