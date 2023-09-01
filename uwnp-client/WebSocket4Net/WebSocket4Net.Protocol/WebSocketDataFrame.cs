using WebSocket4Net.Common;

namespace WebSocket4Net.Protocol
{
	public class WebSocketDataFrame
	{
		private ArraySegmentList m_InnerData;

		private long m_ActualPayloadLength = -1L;

		public ArraySegmentList InnerData
		{
			get
			{
				return m_InnerData;
			}
		}

		public bool IsControlFrame
		{
			get
			{
				switch (OpCode)
				{
				case 8:
				case 9:
				case 10:
					return true;
				default:
					return false;
				}
			}
		}

		public bool FIN
		{
			get
			{
				return (m_InnerData[0] & 0x80) == 128;
			}
		}

		public bool RSV1
		{
			get
			{
				return (m_InnerData[0] & 0x40) == 64;
			}
		}

		public bool RSV2
		{
			get
			{
				return (m_InnerData[0] & 0x20) == 32;
			}
		}

		public bool RSV3
		{
			get
			{
				return (m_InnerData[0] & 0x10) == 16;
			}
		}

		public sbyte OpCode
		{
			get
			{
				return (sbyte)(m_InnerData[0] & 0xF);
			}
		}

		public bool HasMask
		{
			get
			{
				return (m_InnerData[1] & 0x80) == 128;
			}
		}

		public sbyte PayloadLenght
		{
			get
			{
				return (sbyte)(m_InnerData[1] & 0x7F);
			}
		}

		public long ActualPayloadLength
		{
			get
			{
				if (m_ActualPayloadLength >= 0)
				{
					return m_ActualPayloadLength;
				}
				sbyte payloadLenght = PayloadLenght;
				if (payloadLenght < 126)
				{
					m_ActualPayloadLength = payloadLenght;
				}
				else if (payloadLenght == 126)
				{
					m_ActualPayloadLength = m_InnerData[2] * 256 + m_InnerData[3];
				}
				else
				{
					long num = 0L;
					int num2 = 1;
					for (int num3 = 7; num3 >= 0; num3--)
					{
						num += m_InnerData[num3 + 2] * num2;
						num2 *= 256;
					}
					m_ActualPayloadLength = num;
				}
				return m_ActualPayloadLength;
			}
		}

		public byte[] MaskKey { get; set; }

		public byte[] ExtensionData { get; set; }

		public byte[] ApplicationData { get; set; }

		public int Length
		{
			get
			{
				return m_InnerData.Count;
			}
		}

		public WebSocketDataFrame(ArraySegmentList data)
		{
			m_InnerData = data;
			m_InnerData.ClearSegements();
		}

		public void Clear()
		{
			m_InnerData.ClearSegements();
			ExtensionData = new byte[0];
			ApplicationData = new byte[0];
			m_ActualPayloadLength = -1L;
		}
	}
}
