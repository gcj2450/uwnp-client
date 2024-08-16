namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The UserVariable interface defines all the public methods and properties that an object representing a SmartFoxServer User Variable exposes.
	/// </summary>
	///
	/// <remarks>
	/// In the SmartFoxServer 2X client API this interface is implemented by the <see cref="T:Sfs2X.Entities.Variables.SFSUserVariable" /> class. Read the class description for additional informations.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.Variables.Variable" />
	public interface UserVariable : Variable
	{
		/// <summary>
		/// Indicates whether this User Variable is private or not.
		/// </summary>
		///
		/// <remarks>
		/// A private User Variable is visible only to its owner; any changes made to the variable will be transmitted to the owner only.
		/// <para />
		/// <b>NOTE</b>: setting this property manually on an existing User Variable returned by the API has no effect on the server and can disrupt the API functioning.
		/// This flag can be set when the User Variable object is created by the developer only (using the <em>new</em> keyword).
		/// </remarks>
		bool IsPrivate { get; set; }
	}
}
