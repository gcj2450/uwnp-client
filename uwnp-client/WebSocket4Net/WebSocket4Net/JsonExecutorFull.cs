using System;

namespace WebSocket4Net
{
	internal class JsonExecutorFull<T> : JsonExecutorBase<T>
	{
		private Action<JsonWebSocket, string, T> m_ExecutorAction;

		public JsonExecutorFull(Action<JsonWebSocket, string, T> action)
		{
			m_ExecutorAction = action;
		}

		public override void Execute(JsonWebSocket websocket, string token, object param)
		{
			m_ExecutorAction.Method.Invoke(m_ExecutorAction.Target, new object[3] { websocket, token, param });
		}
	}
}
