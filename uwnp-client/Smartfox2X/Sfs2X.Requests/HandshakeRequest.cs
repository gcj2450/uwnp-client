namespace Sfs2X.Requests
{
	/// <summary>
	/// This request is used by the API sub-system at connection time. It's not intended for other uses.
	/// </summary>
	///
	/// <exclude />
	public class HandshakeRequest : BaseRequest, IRequest
	{
		/// <exclude />
		public static readonly string KEY_SESSION_TOKEN = "tk";

		/// <exclude />
		public static readonly string KEY_API = "api";

		/// <exclude />
		public static readonly string KEY_COMPRESSION_THRESHOLD = "ct";

		/// <exclude />
		public static readonly string KEY_RECONNECTION_TOKEN = "rt";

		/// <exclude />
		public static readonly string KEY_CLIENT_TYPE = "cl";

		/// <exclude />
		public static readonly string KEY_MAX_MESSAGE_SIZE = "ms";

		public HandshakeRequest(string apiVersion, string reconnectionToken, string clientDetails)
			: base(RequestType.Handshake)
		{
			sfso.PutUtfString(KEY_API, apiVersion);
			sfso.PutUtfString(KEY_CLIENT_TYPE, clientDetails);
			if (reconnectionToken != null)
			{
				sfso.PutUtfString(KEY_RECONNECTION_TOKEN, reconnectionToken);
			}
		}
	}
}
