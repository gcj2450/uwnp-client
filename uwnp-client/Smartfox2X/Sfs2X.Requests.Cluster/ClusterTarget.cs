namespace Sfs2X.Requests.Cluster
{
	/// <summary>
	/// The ClusterTarget class is a container for the identifier of a Room on a Game Node in a SmartFoxServer 2X Cluster.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Requests.Cluster.ClusterInviteUsersRequest" />
	public class ClusterTarget
	{
		private string serverId;

		private int roomId;

		/// <summary>
		/// Indicates the identifier of the Game Node where the Room is located.
		/// </summary>
		public string ServerId
		{
			get
			{
				return serverId;
			}
		}

		/// <summary>
		/// Indicates the identifier of the Room on the Game Node.
		/// </summary>
		public int RoomId
		{
			get
			{
				return roomId;
			}
		}

		/// <summary>
		/// Creates a new ClusterTarget instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="T:Sfs2X.Requests.Cluster.ClusterInviteUsersRequest" /> class constructor.
		/// </remarks>
		///
		/// <param name="serverId">The identifier of the Game Node where the Room is located.</param>
		/// <param name="roomId">The identifier of the Room on the Game Node.</param>
		///
		/// <seealso cref="T:Sfs2X.Requests.Cluster.ClusterJoinOrCreateRequest" />
		public ClusterTarget(string serverId, int roomId)
		{
			this.serverId = serverId;
			this.roomId = roomId;
		}
	}
}
