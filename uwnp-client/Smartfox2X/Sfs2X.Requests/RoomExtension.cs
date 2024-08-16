namespace Sfs2X.Requests
{
	/// <summary>
	/// The RoomExtension class contains a specific subset of the RoomSettings required to create a Room.
	/// </summary>
	///
	/// <remarks>
	/// This class defines which server-side Extension should be attached to the Room upon creation.
	/// <para />
	/// The client can communicate with the Room Extension by means of the <see cref="T:Sfs2X.Requests.ExtensionRequest" /> request.
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.Requests.RoomSettings.Extension" />
	/// <seealso cref="T:Sfs2X.Requests.CreateRoomRequest" />
	public class RoomExtension
	{
		private string id;

		private string className;

		private string propertiesFile;

		/// <summary>
		/// Returns the name of the Extension to be attached to the Room.
		/// </summary>
		///
		/// <remarks>
		/// This is the name of the server-side folder containing the Extension classes inside the main <em>[sfs2x-install-folder]/SFS2X/extensions</em> folder.
		/// </remarks>
		public string Id
		{
			get
			{
				return id;
			}
		}

		/// <summary>
		/// Returns the fully qualified name of the main class of the Extension.
		/// </summary>
		public string ClassName
		{
			get
			{
				return className;
			}
		}

		/// <summary>
		/// Defines the name of an optional properties file that should be loaded on the server-side during the Extension initialization.
		/// </summary>
		///
		/// <remarks>
		/// The file must be located in the server-side folder containing the Extension classes.
		/// </remarks>
		public string PropertiesFile
		{
			get
			{
				return propertiesFile;
			}
			set
			{
				propertiesFile = value;
			}
		}

		/// <summary>
		/// Creates a new RoomExtension instance.
		/// </summary>
		///
		/// <remarks>
		/// The <see cref="P:Sfs2X.Requests.RoomSettings.Extension">RoomSettings.Extension</see> property must be set to this instance during Room creation.
		/// </remarks>
		///
		/// <param name="id">The name of the Extension as deployed on the server; it's the name of the folder containing the Extension classes inside the main <em>[sfs2x-install-folder]/SFS2X/extensions</em> folder.</param>
		/// <param name="className">The fully qualified name of the main class of the Extension.</param>
		///
		/// <seealso cref="P:Sfs2X.Requests.RoomSettings.Extension" />
		public RoomExtension(string id, string className)
		{
			this.id = id;
			this.className = className;
			propertiesFile = "";
		}
	}
}
