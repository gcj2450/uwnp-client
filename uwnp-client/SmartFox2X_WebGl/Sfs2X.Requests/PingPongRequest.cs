namespace Sfs2X.Requests
{
	/// <summary>
	/// Sends a ping-pong request in order to measure the current lag
	/// </summary>
	///
	/// <exclude />
	public class PingPongRequest : BaseRequest
	{
		public PingPongRequest()
			: base(RequestType.PingPong)
		{
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
		}
	}
}
