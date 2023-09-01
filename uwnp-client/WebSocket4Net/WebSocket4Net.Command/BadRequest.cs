using System;
using System.Collections.Generic;

namespace WebSocket4Net.Command
{
	public class BadRequest : WebSocketCommandBase
	{
		private const string m_WebSocketVersion = "Sec-WebSocket-Version";

		private static readonly string[] m_ValueSeparator = new string[1] { ", " };

		public override string Name
		{
			get
			{
				return 400.ToString();
			}
		}

		public override void ExecuteCommand(WebSocket session, WebSocketCommandInfo commandInfo)
		{
			Dictionary<string, object> valueContainer = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			string verbLine = string.Empty;
			commandInfo.Text.ParseMimeHeader(valueContainer, out verbLine);
			string value = valueContainer.GetValue("Sec-WebSocket-Version", string.Empty);
			if (!session.NotSpecifiedVersion)
			{
				if (string.IsNullOrEmpty(value))
				{
					session.FireError(new Exception("the server doesn't support the websocket protocol version your client was using"));
				}
				else
				{
					session.FireError(new Exception(string.Format("the server(version: {0}) doesn't support the websocket protocol version your client was using", value)));
				}
				session.CloseWithoutHandshake();
				return;
			}
			if (string.IsNullOrEmpty(value))
			{
				session.FireError(new Exception("unknown server protocol version"));
				session.CloseWithoutHandshake();
				return;
			}
			string[] array = value.Split(m_ValueSeparator, StringSplitOptions.RemoveEmptyEntries);
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				int result;
				if (!int.TryParse(array[i], out result))
				{
					session.FireError(new Exception("invalid websocket version"));
					session.CloseWithoutHandshake();
					return;
				}
				array2[i] = result;
			}
			if (!session.GetAvailableProcessor(array2))
			{
				session.FireError(new Exception("unknown server protocol version"));
				session.CloseWithoutHandshake();
			}
			else
			{
				session.ProtocolProcessor.SendHandshake(session);
			}
		}
	}
}
