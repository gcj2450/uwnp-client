namespace Sfs2X.Requests
{
	/// <summary>
	/// The BanMode enumeration contains the costants describing the possible banning modalities for a BanUserRequest.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Requests.BanUserRequest" />
	public enum BanMode
	{
		/// <summary>
		/// User is banned by IP address.
		/// </summary>
		BY_ADDRESS,
		/// <summary>
		/// User is banned by name.
		/// </summary>
		BY_NAME
	}
}
