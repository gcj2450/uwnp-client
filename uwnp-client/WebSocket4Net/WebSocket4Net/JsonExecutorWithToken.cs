using System;

namespace WebSocket4Net
{
	internal class JsonExecutorWithToken<T> : JsonExecutorBase<T>
	{
		private Action<string, T> m_ExecutorAction;

		public JsonExecutorWithToken(Action<string, T> action)
		{
			m_ExecutorAction = action;
		}

		public override void Execute(JsonWebSocket websocket, string token, object param)
		{
			m_ExecutorAction.Method.Invoke(m_ExecutorAction.Target, new object[2] { token, param });
		}
	}
}
