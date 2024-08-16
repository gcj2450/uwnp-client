namespace Sfs2X.Requests
{
	/// <summary>
	/// This is used by the system. Never send this directly.
	/// </summary>
	///
	/// <exclude />
	public class ManualDisconnectionRequest : BaseRequest
	{
		/// <exclude />
		public ManualDisconnectionRequest()
			: base(RequestType.ManualDisconnection)
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
