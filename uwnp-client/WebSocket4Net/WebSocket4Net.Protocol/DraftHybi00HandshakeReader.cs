using System;

namespace WebSocket4Net.Protocol
{
	internal class DraftHybi00HandshakeReader : HandshakeReader
	{
		private int m_ReceivedChallengeLength = -1;

		private int m_ExpectedChallengeLength = 16;

		private WebSocketCommandInfo m_HandshakeCommand;

		private byte[] m_Challenges = new byte[16];

		public DraftHybi00HandshakeReader(WebSocket websocket)
			: base(websocket)
		{
		}

		private void SetDataReader()
		{
			base.NextCommandReader = new DraftHybi00DataReader(this);
		}

		public override WebSocketCommandInfo GetCommandInfo(byte[] readBuffer, int offset, int length, out int left)
		{
			if (m_ReceivedChallengeLength < 0)
			{
				WebSocketCommandInfo commandInfo = base.GetCommandInfo(readBuffer, offset, length, out left);
				if (commandInfo == null)
				{
					return null;
				}
				if (HandshakeReader.BadRequestCode.Equals(commandInfo.Key))
				{
					return commandInfo;
				}
				m_ReceivedChallengeLength = 0;
				m_HandshakeCommand = commandInfo;
				int srcOffset = offset + length - left;
				if (left < m_ExpectedChallengeLength)
				{
					if (left > 0)
					{
						Buffer.BlockCopy(readBuffer, srcOffset, m_Challenges, 0, left);
						m_ReceivedChallengeLength = left;
						left = 0;
					}
					return null;
				}
				if (left == m_ExpectedChallengeLength)
				{
					Buffer.BlockCopy(readBuffer, srcOffset, m_Challenges, 0, left);
					SetDataReader();
					m_HandshakeCommand.Data = m_Challenges;
					left = 0;
					return m_HandshakeCommand;
				}
				Buffer.BlockCopy(readBuffer, srcOffset, m_Challenges, 0, m_ExpectedChallengeLength);
				left -= m_ExpectedChallengeLength;
				SetDataReader();
				m_HandshakeCommand.Data = m_Challenges;
				return m_HandshakeCommand;
			}
			int num = m_ReceivedChallengeLength + length;
			if (num < m_ExpectedChallengeLength)
			{
				Buffer.BlockCopy(readBuffer, offset, m_Challenges, m_ReceivedChallengeLength, length);
				left = 0;
				m_ReceivedChallengeLength = num;
				return null;
			}
			if (num == m_ExpectedChallengeLength)
			{
				Buffer.BlockCopy(readBuffer, offset, m_Challenges, m_ReceivedChallengeLength, length);
				left = 0;
				SetDataReader();
				m_HandshakeCommand.Data = m_Challenges;
				return m_HandshakeCommand;
			}
			int num2 = m_ExpectedChallengeLength - m_ReceivedChallengeLength;
			Buffer.BlockCopy(readBuffer, offset, m_Challenges, m_ReceivedChallengeLength, num2);
			left = length - num2;
			SetDataReader();
			m_HandshakeCommand.Data = m_Challenges;
			return m_HandshakeCommand;
		}
	}
}
