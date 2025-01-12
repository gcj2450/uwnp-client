using System;

namespace WebSocket4Net
{
	public class DataReceivedEventArgs : EventArgs
	{
		public byte[] Data { get; private set; }

		public DataReceivedEventArgs(byte[] data)
		{
			Data = data;
		}
	}
}
