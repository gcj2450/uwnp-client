using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCtrl : MonoBehaviour {

	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		anim.CrossFade ("WALK00_F", 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
