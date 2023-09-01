using System;

namespace WebSocket4Net
{
	internal class JsonExecutorWithSender<T> : JsonExecutorBase<T>
	{
		private Action<JsonWebSocket, T> m_ExecutorAction;

		public JsonExecutorWithSender(Action<JsonWebSocket, T> action)
		{
			m_ExecutorAction = action;
		}

		public override void Execute(JsonWebSocket websocket, string token, object param)
		{
			m_ExecutorAction.Method.Invoke(m_ExecutorAction.Target, new object[2] { websocket, param });
		}
	}
}
