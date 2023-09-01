using System;

namespace WebSocket4Net
{
	internal class JsonExecutor<T> : JsonExecutorBase<T>
	{
		private Action<T> m_ExecutorAction;

		public JsonExecutor(Action<T> action)
		{
			m_ExecutorAction = action;
		}

		public override void Execute(JsonWebSocket websocket, string token, object param)
		{
			m_ExecutorAction.Method.Invoke(m_ExecutorAction.Target, new object[1] { param });
		}
	}
}
