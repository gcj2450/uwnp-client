using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;

public class I18NManager : MonoBehaviour
{
	// TODO static util class
	private static I18NManager mInstance;
	
	public static I18NManager Instance {
		get {
			if (mInstance == null) {
				mInstance = GameObject.FindObjectOfType (typeof(I18NManager)) as I18NManager;
				
				if (mInstance == null) {
					GameObject obj = new GameObject ("I18NManager");
					mInstance = obj.AddComponent (typeof(I18NManager)) as I18NManager;
				}
			}
			return mInstance;
		}
	}
	
	private Dictionary<string, string> props;
	
	void Awake ()
	{
		DontDestroyOnLoad (gameObject);
		
		string fileName = string.Format("i18n/message_{0}", ScopeHolder.globalAttr[Const.LANG]);
		Debug.Log ("i18n file = " + fileName);
		TextAsset asset = Resources.Load (fileName) as TextAsset;

		props = new Dictionary<string, string> ();
		Util.ReadDictionary (asset.text, props);
	}
	
	public string GetProp (string key)
	{
		if (props.ContainsKey (key)) {
			return props [key];
		}
		return null;
	}
	
	public string GetProp (string key, string def)
	{
		if (props.ContainsKey (key)) {
			return props [key];
		}
		return def;
	}
	
	
	
}
