using WebSocket4Net.Common;

namespace WebSocket4Net.Protocol
{
	public abstract class ReaderBase : IClientCommandReader<WebSocketCommandInfo>
	{
		private readonly ArraySegmentList m_BufferSegments;

		protected WebSocket WebSocket { get; private set; }

		protected ArraySegmentList BufferSegments
		{
			get
			{
				return m_BufferSegments;
			}
		}

		public IClientCommandReader<WebSocketCommandInfo> NextCommandReader { get; internal set; }

		public ReaderBase(WebSocket websocket)
		{
			WebSocket = websocket;
			m_BufferSegments = new ArraySegmentList();
		}

		public ReaderBase(ReaderBase previousCommandReader)
		{
			m_BufferSegments = previousCommandReader.BufferSegments;
		}

		public abstract WebSocketCommandInfo GetCommandInfo(byte[] readBuffer, int offset, int length, out int left);

		protected void AddArraySegment(byte[] buffer, int offset, int length)
		{
			BufferSegments.AddSegment(buffer, offset, length, true);
		}

		protected void ClearBufferSegments()
		{
			BufferSegments.ClearSegements();
		}
	}
}
