using UnityEngine;
using System;
using System.Collections;
using System.IO;
using UJNet;
using UJNet.Data;

public class UJNetManager : MonoBehaviour
{
	private static UJNetManager mInstance;
	public static UJNetManager Instance {
		get {
			if (mInstance == null) {
				mInstance = GameObject.FindObjectOfType (typeof(UJNetManager)) as UJNetManager;
			}
			
			return mInstance;
		}
	}
	public bool UseWebSocket = true;
	private NetClient client;
	
	public NetClient Client {
		get { return client; }
	}
	
	void Awake ()
	{
		client = new NetClient(true, UseWebSocket);
		client.AddDebugMessageListener(OnDebugMessage);

		Application.runInBackground = true;
		
		mInstance = this;
		DontDestroyOnLoad (this);
#if UNITY_ANDROID
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif		
	}
	
	void OnDebugMessage(string message){
		Debug.Log("NET: " + message);
	}
	
	void Update()
	{
		if (client != null) {
			client.ProcessEventQueue ();
		}
	}
	void OnApplicationFocus (){
#if UNITY_IPHONE
		EtceteraBinding.setBadgeCount(0);
#endif
		
	}
	
	void OnApplicationQuit ()
	{
        //==============自定义修改=====================
        // save jid
  //      if (ScopeHolder.attr.ContainsKey(Const.SCOPE_JID)) {
		//	string jid = (string)ScopeHolder.attr[Const.SCOPE_JID];
		//	if (!string.IsNullOrEmpty(jid)) {
		//		PlayerPrefs.SetString(Util.GetPrefKey(Const.PREF_KEY_JID), jid);
		//	}
		//}
		
		//PlayerPrefs.DeleteKey(Const.NOTICE_CITY);
        //==============自定义修改=====================
        // notify svr logout
        if (client != null){
            SFSObject param = SFSObject.NewInstance ();
			client.SendRequest (101, param, false);
		}
	}

	
	void LateUpdate ()
	{
#if UNITY_ANDROID
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Debug.Log("press escape");
            //==============自定义修改=====================
   //         ChangeUIEventArgs args = ConfirmBoxCtrl.CreateSelfArgs (null, I18NManager.Instance.GetProp ("quit.comfirm"), null, null);
			//ConfirmBoxCtrl.Instance.OpenSelf (this, OnQuitByEscapeConfirm, null, args);
            //==============自定义修改=====================
        }
#endif
    }
	
	public void OnQuitByEscapeConfirm (int callBackCMD, object sender, ChangeUIEventArgs args, out bool destoryCallBack, out bool closePanel, ChangeUIEventArgs attr)
	{
		closePanel = destoryCallBack = false;
        //==============自定义修改=====================
  //      switch (callBackCMD) {
		//case ConfirmBoxCtrl.BTN_CMD_COMPLETE_BRING_IN:
			
		//	break;
		//case ConfirmBoxCtrl.BTN_CMD_LEFT_PRESSED:
		//	System.Diagnostics.Process.GetCurrentProcess().Kill();
		//	closePanel = destoryCallBack = true;
		//	break;
		//case ConfirmBoxCtrl.BTN_CMD_RIGHT_PRESSED:
			
		//	closePanel = destoryCallBack = true;
		//	break;
		//}
        //==============自定义修改=====================
    }

    void HandleLog (string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Exception || type == LogType.Error) {
            //==============自定义修改=====================
            //UserManager.Instance.SendClientLog(logString + "\n" + stackTrace);
        }
    }
	
}
