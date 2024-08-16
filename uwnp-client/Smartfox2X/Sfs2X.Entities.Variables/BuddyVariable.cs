namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The BuddyVariable interface defines all the public methods and properties that an object representing a SmartFoxServer Buddy Variable exposes.
	/// </summary>
	///
	/// <remarks>
	/// In the SmartFoxServer 2X client API this interface is implemented by the <see cref="T:Sfs2X.Entities.Variables.SFSBuddyVariable" /> class. Read the class description for additional informations.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.Variables.SFSBuddyVariable" />
	public interface BuddyVariable : Variable
	{
		/// <summary>
		/// Indicates whether the Buddy Variable is persistent or not.
		/// </summary>
		///
		/// <remarks>
		/// By convention any Buddy Variable whose name starts with the dollar sign ($) will be regarded as persistent and stored locally by the server.
		/// Persistent Buddy Variables are also referred to as "offline variables" because they are available to all users who have the owner in their Buddy Lists, whether that Buddy is online or not.
		/// </remarks>
		bool IsOffline { get; }
	}
}
