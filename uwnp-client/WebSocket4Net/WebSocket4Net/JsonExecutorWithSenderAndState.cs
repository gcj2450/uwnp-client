using System;

namespace WebSocket4Net
{
	internal class JsonExecutorWithSenderAndState<T> : JsonExecutorBase<T>
	{
		private Action<JsonWebSocket, T, object> m_ExecutorAction;

		private object m_State;

		public JsonExecutorWithSenderAndState(Action<JsonWebSocket, T, object> action, object state)
		{
			m_ExecutorAction = action;
			m_State = state;
		}

		public override void Execute(JsonWebSocket websocket, string token, object param)
		{
			m_ExecutorAction.Method.Invoke(m_ExecutorAction.Target, new object[3] { websocket, param, m_State });
		}
	}
}
