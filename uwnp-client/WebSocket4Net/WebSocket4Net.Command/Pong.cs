using System;

namespace WebSocket4Net.Command
{
	public class Pong : WebSocketCommandBase
	{
		public override string Name
		{
			get
			{
				return 10.ToString();
			}
		}

		public override void ExecuteCommand(WebSocket session, WebSocketCommandInfo commandInfo)
		{
			session.LastActiveTime = DateTime.Now;
			session.LastPongResponse = commandInfo.Text;
		}
	}
}
