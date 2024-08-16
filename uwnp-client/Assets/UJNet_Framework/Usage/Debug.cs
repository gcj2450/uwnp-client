//using UnityEngine;
//using System.Collections;

//public class SimpleLog {
//	public int type;
//	public string msg;

//	public SimpleLog(int type, string msg){
//		this.type = type;
//		this.msg = msg;
//	}
//}

//public class Debug
//{	
//#if UNITY_EDITOR || DEBUG_ENABLE
//	public static bool enable = true;
//#else
//	public static bool enable = false;
//#endif
	
//	public static int MAX_MSG_COUNT = 40;
//	public static Queue msgQueue = new Queue();

//	static void AddQueue(SimpleLog log)
//	{
//		msgQueue.Enqueue(log);
//		while (msgQueue.Count > MAX_MSG_COUNT)
//		{
//			msgQueue.Dequeue();
//		}
//	}
	
//	public static void Log (object message)
//	{		
//		if (enable) {
//			UnityEngine.Debug.Log (message);
//			AddQueue(new SimpleLog(1, message.ToString()));
//		}

//	}
	
//	public static void LogWarning (object message)
//	{
//		if (enable) {
//			UnityEngine.Debug.LogWarning (message);
//			AddQueue(new SimpleLog(2, message.ToString()));
//		}
//	}
	
//	public static void LogError (object message)
//	{
//		if (enable) {
//			UnityEngine.Debug.LogError (message);
//			AddQueue(new SimpleLog(3, message.ToString()));
//		}
//	}

//	public static void DrawLine (Vector3 start, Vector3 end, Color color)
//	{
//		if (enable)
//		UnityEngine.Debug.DrawLine (start, end, color);
//	}
	
//	public static void DrawLine (Vector3 start, Vector3 end, Color color, float duration)
//	{
//		if (enable)
//		UnityEngine.Debug.DrawLine (start, end, color, duration);
//	}
	
//	public static void DrawRay (Vector3 start, Vector3 dir, Color color, float duration)
//	{
//		if (enable)
//		UnityEngine.Debug.DrawRay (start, dir, color, duration);
//	}

//}
