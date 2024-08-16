using UnityEngine;
using System.Collections;
using UJNet.Data;
using UJNet;

public class WaitPanelCtrl : MonoBehaviour
{

	private static WaitPanelCtrl mInstance;
	public static WaitPanelCtrl Instance {
		get {
			if (mInstance == null) {
				mInstance = GameObject.FindObjectOfType (typeof(WaitPanelCtrl)) as WaitPanelCtrl;
			}
			return mInstance;
		}
	}
	
	protected GameObject GetBasePanel ()
	{
		return transform.GetChild (0).gameObject;
	}
	
	protected void Awake ()
	{
		mInstance = this;
		DontDestroyOnLoad (gameObject);
	}
	
	public void HidePanel ()
	{
	}
	
	public void ShowPanel ()
	{
	}
}
