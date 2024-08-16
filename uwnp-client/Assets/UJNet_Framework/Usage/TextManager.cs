using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;

public class TextManager : MonoBehaviour
{
	// TODO static util class
	private static TextManager mInstance;
	public static string lang = "tw";
	
	public static TextManager Instance {
		get {
			if (mInstance == null) {
				mInstance = GameObject.FindObjectOfType (typeof(TextManager)) as TextManager;
				
				if (mInstance == null) {
					GameObject obj = new GameObject ("TextManager");
					mInstance = obj.AddComponent (typeof(TextManager)) as TextManager;
				}
			}
			return mInstance;
		}
	}
	
	private Dictionary<string, string> props;
	
	void Awake ()
	{
		DontDestroyOnLoad (gameObject);
		
		string fileName = string.Format("i18n/message_{0}", lang);
		Debug.Log ("i18n textmgr file = " + fileName);
		TextAsset asset = Resources.Load (fileName) as TextAsset;

		props = new Dictionary<string, string> ();
		TextUtil.ReadDictionary (asset.text, props);
	}
	
	public string GetProp (string key)
	{
		if (props == null) {
			print ("props is null");
			CreatProps ();
			if (props.ContainsKey (key)) {
				
				return props [key];
			}
		} else {
			if (props.ContainsKey (key)) {
				
				return props [key];
			}
		}
		return null;
	}
	
	public string GetProp (string key, string def)
	{
		if (props == null) {
			print ("props is null");
			CreatProps ();
			if (props.ContainsKey (key)) {
				
				return props [key];
			}
		} else {
			print ("props is't null");
			if (props.ContainsKey (key)) {
				
				return props [key];
			}
		}
		return def;
	}
	
	private void CreatProps ()
	{
		TextAsset asset = Resources.Load ("i18n/message_zh") as TextAsset;
		props = new Dictionary<string, string> ();
		TextUtil.ReadDictionary (asset.text, props);
	}	
}
