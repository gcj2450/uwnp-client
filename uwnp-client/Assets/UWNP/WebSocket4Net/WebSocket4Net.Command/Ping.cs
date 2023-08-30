using System;

namespace WebSocket4Net.Command
{
	public class Ping : WebSocketCommandBase
	{
		public override string Name
		{
			get
			{
				return 9.ToString();
			}
		}

		public override void ExecuteCommand(WebSocket session, WebSocketCommandInfo commandInfo)
		{
			session.LastActiveTime = DateTime.Now;
			session.ProtocolProcessor.SendPong(session, commandInfo.Text);
		}
	}
}
