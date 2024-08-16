namespace Sfs2X.Logging
{
	/// <summary>
	/// The LogLevel enumeration contains the costants describing the importance levels of logged messages.
	/// </summary>
	///
	/// <seealso cref="M:Sfs2X.SmartFox.AddLogListener(Sfs2X.Logging.LogLevel,Sfs2X.Core.EventListenerDelegate)" />
	public enum LogLevel
	{
		/// <summary>
		/// A DEBUG message is a fine-grained information on the client activity.
		/// </summary>
		DEBUG = 100,
		/// <summary>
		/// An INFO message contains informations on the standard client activities.
		/// </summary>
		INFO = 200,
		/// <summary>
		/// A WARN message is a warning caused by an unexpected behavior of the client.
		/// </summary>
		///
		/// <remarks>
		/// Client operations are not compromised when a warning is raised.
		/// </remarks>
		WARN = 300,
		/// <summary>
		/// An ERROR message contains informations on a problem that occurred during the client activities.
		/// </summary>
		///
		/// <remarks>
		/// Client operations might be compromised when an error is raised.
		/// </remarks>
		ERROR = 400
	}
}
