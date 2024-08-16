using System;
using System.Collections;
using UnityEngine;

public class TransformInterpolation : MonoBehaviour
{
	public float extrapolationForwardTime = 1.5f;
	
//	private PVPUnitCtrl ctrl;
	private CharacterController character;
	//
	public bool running = false;
	private int statesCount = 0;
	private NetTransform[] bufferedStates = new NetTransform[20];

	void Awake ()
	{
//		ctrl = GetComponent<PVPUnitCtrl> ();
		character = GetComponent<CharacterController> ();
	}
	
	void OnEnable ()
	{
		running = true;
	}
	
	void OnDisable() {
		running = false;
	}	
	
	public void ReceivedTransform (NetTransform ntransform)
	{
		if (!running)
			return;
		
		Vector3 pos = ntransform.Position;
		Quaternion rot = ntransform.Rotation;
						
		for (int i = bufferedStates.Length - 1; i >= 1; i--) {
			bufferedStates [i] = bufferedStates [i - 1];
		}
		  
		bufferedStates [0] = ntransform;
		statesCount = Mathf.Min (statesCount + 1, bufferedStates.Length);
	}

	void Update ()
	{
		if (!running)
			return;
		if (statesCount <= 1)
			return;
		
//		if (ctrl.unitInfo.prototypeId == 12) {
//			ctrl.FaceIt (bufferedStates[0].Rotation);
//			return;
//		}
		
		for (int i=1; i<statesCount-1; i++) {
			Color c = i%2 == 0 ? Color.red : Color.white;
			if (i == 1) c = Color.green;
			Debug.DrawLine(bufferedStates[i-1].Position, bufferedStates[i].Position, c);
		}
		
		double currentTime = TimeManager.Instance.NetworkTime;
		float extrapolationLength = Convert.ToSingle (currentTime - bufferedStates [0].TimeStamp) / 1000.0f;
		
		Vector3 dif = bufferedStates [0].Position - bufferedStates [1].Position;
		float distance = Vector3.Distance (bufferedStates [0].Position, bufferedStates [1].Position);
		float timeDif = Convert.ToSingle (bufferedStates [0].TimeStamp - bufferedStates [1].TimeStamp) / 1000.0f;
		
		dif = dif.normalized;
		float speed = bufferedStates [0].velocity;			
		Vector3 expectedPosition = bufferedStates [0].Position + dif * extrapolationLength * speed;
		
		if (extrapolationLength > extrapolationForwardTime || distance < 0.0001f || timeDif < 0.0001f) {
			expectedPosition = bufferedStates [0].Position;
			FaceIt (bufferedStates[0].Rotation);
		}
		
		Vector3 movement = expectedPosition - transform.position;
		movement.y = 0;
		if (movement.sqrMagnitude < 0.1f) {
			movement = Vector3.zero;
		}
		movement = movement.normalized;
		character.Move (movement * Time.deltaTime * speed);

		Vector3 faceDir = movement;
		if (faceDir != Vector3.zero) {
			FaceIt (Quaternion.LookRotation(faceDir, Vector3.up));
		}
	}

	public void FaceIt (Quaternion targetRotation)
	{
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 0.3f);

	}
}

