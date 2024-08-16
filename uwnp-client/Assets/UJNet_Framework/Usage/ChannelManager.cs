using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChannelManager : MonoBehaviour
{
	public GameObject[] prevPrefabs;
	public TextAsset globalConfig;
	private static bool inited = false;
	private static bool playSplash = false;
	
	void Awake ()
	{
		Camera.main.backgroundColor = Color.black;
		if (!inited) {
			Util.ReadDictionary(globalConfig.text, ScopeHolder.globalAttr);
		}
		TextManager.lang = ScopeHolder.globalAttr[Const.LANG];
	}
	
	void Start ()
	{
//		if (!playSplash) {
//#if UNITY_IPHONE
//			Handheld.PlayFullScreenMovie("logo_trailer.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput,FullScreenMovieScalingMode.Fill);
//#elif UNITY_ANDROID			
//			Handheld.PlayFullScreenMovie("logo_trailer.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput,FullScreenMovieScalingMode.AspectFill);
//#endif
//			playSplash = true;
//		}
		
		if (!inited) {
			Debug.Log (" InitContext " + DateTime.Now);
			InitContext ();
			inited = true;
		}
	}
	
	bool did = false;

	void Update ()
	{
		if (Time.time == 0 || Time.deltaTime == 0)
			return;
		
		if (!did && playSplash) {
			Do ();
			did = true;
		}
	}
	
	void Do ()
	{
		// init prev loader
		Instantiate (Resources.Load ("prefab/gate loader"));
		
		//GateEntry entry = FindObjectOfType (typeof(GateEntry)) as GateEntry;
		//entry.OnInit ();	
		//Debug.Log (" entry.OnInit () " + DateTime.Now + "/" + entry.name);
	}
	
	void InitContext ()
	{
		if (prevPrefabs == null)
			return;
		
		DateTime startTime = DateTime.Now;
		foreach (GameObject prefab in prevPrefabs) {
			if (prefab != null) {
				Instantiate (prefab);
			}
		}
		
		//bool ipad = Util.GetDeviceType() == Const.IOS_DEVICE_IPAD;
		//QualitySettings.SetQualityLevel(ipad ? (int)QualityLevel.Beautiful : (int)QualityLevel.Fastest, true);
		//Debug.Log("// quality settings level " + QualitySettings.GetQualityLevel());
		
		string currentChannel = ScopeHolder.globalAttr[Const.STYPE_CURRENT];
		Debug.Log(string.Format("=================CURRENT ENTRY: {0}===================", currentChannel));
		
		Instantiate (Resources.Load("prefab/channel/" + currentChannel));
		//LevelLoadCtrl.Instance.ShowPanel ();
		TimeSpan elapse = DateTime.Now - startTime;
		Debug.Log ("// InitContext elapse: " + elapse.TotalSeconds + " s");
	}		
}
