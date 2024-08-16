using UnityEngine;
using System.Collections;

public class TransformReceiver : MonoBehaviour
{

//	private PVPUnitCtrl ctrl;
	private TransformInterpolation interpolator;
	private bool running = false;

	void Awake ()
	{
//		ctrl = GetComponent<PVPUnitCtrl>();
		interpolator = GetComponent<TransformInterpolation> ();
	}
	
	void OnEnable ()
	{
		running = true;
	}
	
	void OnDisable() {
		running = false;
	}

	public void Receive (NetTransform ntransform)
	{
		if (!running) return;
		//Debug.Log ("Receive ntransform : "+ ntransform.position);
		interpolator.ReceivedTransform (ntransform);
	}	
}
