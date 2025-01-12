namespace WebSocket4Net.Command
{
	public class Text : WebSocketCommandBase
	{
		public override string Name
		{
			get
			{
				return 1.ToString();
			}
		}

		public override void ExecuteCommand(WebSocket session, WebSocketCommandInfo commandInfo)
		{
			session.FireMessageReceived(commandInfo.Text);
		}
	}
}
