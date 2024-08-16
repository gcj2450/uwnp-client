using UnityEngine;
using System;
using System.IO;
using System.Collections;
using UJNet;
using UJNet.Data;

public class UserManager : MonoBehaviour
{
	private static UserManager mInstance;
	
	public static UserManager Instance {
		get {
			if (mInstance == null) {
				mInstance = GameObject.FindObjectOfType (typeof(UserManager)) as UserManager;
			}
			return mInstance;
		}
	}

	//[HideInInspector]
	//public PlayerInfo playerInfo;
	//public Action<PlayerInfo> OnPlayerInfoLoaded;
	private NetClient client;
	public static double curTime;
	public const int CMD_SEND_LOGIN = 200;
	public  const int CMD_SEND_LOAD_CITY_RESOURCE = 301;
	//private CityResourceData  cityResourceData;
	//private bool isInit = false;
	private Action<object,ChangeUIEventArgs> OnCityResourceChangedEvent;
	//private bool deviceTokenSent = false;
	// cur enter league id
	public static long leagueId;

	void Start ()
	{
		client = UJNetManager.Instance.Client;
		client.AddResponseListener (OnResponse);
	}
	
	//void OnLevelWasLoaded (int level)
	//{
	//	OnCityResourceChangedEvent = null;
	//	client.AddResponseListener (OnResponse);
	//}
	
	void Awake ()
	{
		DontDestroyOnLoad (gameObject);
	}
	
	public void InitPlayerByAccount (bool sycn)
	{
        SFSObject param = SFSObject.NewInstance ();
		client.SendRequest (CMD_SEND_LOGIN, param, sycn);
	}
	
	private void InitPlayerByAccount_Itnl (int cmd, SFSObject param)
	{
		int state = param.GetInt ("state");
		if (state != 1) {
			Debug.LogWarning ("InitPlayerByAccount_Itnl ST: " + state);
			return;
		}
		//playerInfo = new PlayerInfo ();
		//playerInfo.LoadFromUJObject (param);
		//ScopeHolder.attr [Const.SCOPE_CHAT_SVR_URL] = param.GetUtfString ("chatserver");
		
		
		//Debug.Log ("///chat svr : " + ScopeHolder.attr [Const.SCOPE_CHAT_SVR_URL]);
		
		//if (!string.IsNullOrEmpty (playerInfo.blacklist)) {
		//	ScopeHolder.globalAttr [Const.PROGRESS_BLACKLIST] = playerInfo.blacklist;
		//}
		
		//if (OnPlayerInfoLoaded != null) {
		//	OnPlayerInfoLoaded (playerInfo);
		//}
	}
	
	public void SendDeviceToken ()
	{
#if UNITY_IPHONE && (IOS_APPLE || IOS_TWM)
		if (deviceTokenSent)
			return;
		
		string deviceToken = PlayerPrefs.GetString (Const.PUSH_DEVICE_TOKEN);
		if (string.IsNullOrEmpty(deviceToken))
			return;
		
		Debug.Log ("send device token " + deviceToken);
		deviceTokenSent = true;
		
		UJObject param = UJObject.NewInstance ();
		param.PutUtfString ("token", deviceToken);
		param.PutUtfString ("model", SystemInfo.deviceModel);
		param.PutUtfString ("appname", (string)ScopeHolder.attr [Const.APP_NAME]);
		param.PutUtfString ("version", (string)ScopeHolder.attr [Const.APP_VERSION]);
		// get macaddr
		object macid;
		macid = ScopeHolder.attr.TryGetValue(Const.MACADDR, out macid) ? macid : "";
		param.PutUtfString ("macaddr", macid.ToString());
		// get udid
		object udid;
		udid = ScopeHolder.attr.TryGetValue(Const.UDID, out udid) ? udid : "";
		param.PutUtfString("udid", udid.ToString());
		client.SendRequest (3100, param, false);
#endif			
	}
	
	public void SendClientLog (string data)
	{
		Debug.Log ("send log \n" + data);
        SFSObject param = SFSObject.NewInstance ();
		param.PutUtfString ("log", data);
		client.SendRequest (202, param, false);
	}

	private void OnResponse (int cmd, SFSObject param)
	{
		switch (cmd) {
		case CMD_SEND_LOGIN:
			InitPlayerByAccount_Itnl (cmd, param);
			break;
		//case CMD_SEND_LOAD_CITY_RESOURCE:
		//	LoadCityResource (cmd, param);
		//	break;
		}
	}
	
	void Update ()
	{
		if (curTime > 0) {
			curTime += Convert.ToDouble (Time.deltaTime);
		}
		
//		Debug.Log ("OnCityResourceChangedEvent leagth : " + (OnCityResourceChangedEvent).GetInvocationList ().Length);
	}
	
	public static  bool CompareClientTime (long endTimeL)
	{
		double left = Convert.ToDouble (endTimeL) - curTime;
		left = Math.Floor (left);
		return left > 0;
	}
	
	public static long GetCurTime ()
	{
		return (long)curTime ;
	}
		
	public void SendLoadCityResource ()
	{
		//SendLoadCityResource (false);
	}
	
	//public void SendLoadCityResource (bool isSycn)
	//{
	//	UJObject param = UJObject.NewInstance ();
	//	param.PutLong ("cid", playerInfo.CityId);
	//	client.SendRequest (CMD_SEND_LOAD_CITY_RESOURCE, param, isSycn);
	//}

	//private void LoadCityResource (int cmd, UJObject param)
	//{
	//	int state = param.GetInt ("state");
	//	if (state != 1) {
	//		Debug.LogWarning ("LoadCityResource: " + param.GetUtfString ("msg"));
	//		return;
	//	}
		
	//	cityResourceData = new CityResourceData ();
	//	cityResourceData.LoadFromUJObject (param);
	//	ChangeUIEventArgs args = new ChangeUIEventArgs ();
	//	args.Data = cityResourceData;
		
	//	// fire change event
	//	if (OnCityResourceChangedEvent != null) {
	//		OnCityResourceChangedEvent (this, args);
	//	}
		
	//}
	
	public void SetCityResourceChangedEvent (Action<object,ChangeUIEventArgs> del)
	{
		OnCityResourceChangedEvent = del;
		
	}

	public  void AddCityResourceChangedEvent (Action<object,ChangeUIEventArgs> del)
	{
		OnCityResourceChangedEvent += del;
	}

	public  void RemoveCityResourceChangedEvent (Action<object,ChangeUIEventArgs> del)
	{
		OnCityResourceChangedEvent -= del;
	}
	
	//public CityResourceData CityResourceData {
	//	get {
	//		return this.cityResourceData;
	//	}
	//}
	
}
