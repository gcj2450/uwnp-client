using System;

namespace WebSocket4Net
{
	public class ClosedEventArgs : EventArgs
	{
		public short Code { get; private set; }

		public string Reason { get; private set; }

		public ClosedEventArgs(short code, string reason)
		{
			Code = code;
			Reason = reason;
		}
	}
}
