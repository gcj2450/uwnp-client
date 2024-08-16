namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The RoomVariable interface defines all the public methods and properties that an object representing a SmartFoxServer Room Variable exposes.
	/// </summary>
	///
	/// <remarks>
	/// In the SmartFoxServer 2X client API this interface is implemented by the <see cref="T:Sfs2X.Entities.Variables.SFSRoomVariable" /> class. Read the class description for additional informations.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.Variables.SFSRoomVariable" />
	public interface RoomVariable : Variable
	{
		/// <summary>
		/// Indicates whether this Room Variable is private or not.
		/// </summary>
		///
		/// <remarks>
		/// A private Room Variable is visible to all users in the same Room, but it can be modified by its owner only (the user that created it).
		/// <para />
		/// <b>NOTE</b>: setting this property manually on an existing Room Variable returned by the API has no effect on the server and can disrupt the API functioning.
		/// This flag can be set when the Room Variable object is created by the developer only (using the <em>new</em> keyword).
		/// </remarks>
		bool IsPrivate { get; set; }

		/// <summary>
		/// Indicates whether this Room Variable is persistent or not.
		/// </summary>
		///
		/// <remarks>
		/// A persistent Room Variable continues to exist in the Room after the user who created it has left it and until he disconnects.
		/// <para />
		/// <b>NOTE</b>: setting this property manually on an existing Room Variable returned by the API has no effect on the server and can disrupt the API functioning.
		/// This flag can be set when the Room Variable object is created by the developer only (using the <em>new</em> keyword).
		/// </remarks>
		bool IsPersistent { get; set; }
	}
}
