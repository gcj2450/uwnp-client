using UnityEngine;
using System.Collections;

public class TransformSender : MonoBehaviour
{

	public UJNet_Client cntest;

	private float lastSendTime;
	private float sendIntervel = 0.01f;
	private readonly float accuracy = 0.002f;
	
	public bool running = false;
	private NetTransform lastState;
	
//	private PVPUnitCtrl ctrl;
    private CharacterMoveCtrl cmc;
	
	void Awake ()
	{
//		ctrl = GetComponent<PVPUnitCtrl>();
        cmc = GetComponent<CharacterMoveCtrl>();
	}
	
	void OnEnable ()
	{
//		if (UserManager.Instance.playerInfo.pvpNetSyncInterval > 0) {
//			sendIntervel = UserManager.Instance.playerInfo.pvpNetSyncInterval;
//		}
		running = true;
	}
	
	void OnDisable() {
		running = false;
	}
	
	void Update ()
	{
		if (!running) return;
		lastSendTime += Time.deltaTime;
		if (lastSendTime >= sendIntervel) {
			lastSendTime = 0;
			
			Transform trans = transform;
//			if (ctrl is IPVEGroupUnit) {
//				trans = ((IPVEGroupUnit) ctrl).CorePivot.transform;
//			} else if (ctrl is PVPStoneSlingCtrl) {
//				trans = ((PVPStoneSlingCtrl)ctrl).body;
//			}
			
			lastState = NetTransform.FromTransform (trans);
			lastState.velocity = 1f;	//当前的移动速度
            lastState.TimeStamp = TimeManager.Instance.NetworkTime;
            cntest.SendTransform(cntest.userId, lastState);
		}	
		lastSendTime += Time.deltaTime;

        cmc.SideSpeed = Input.GetAxis("Horizontal");
        cmc.ForwardSpeed = Input.GetAxis("Vertical");

        //if (Input.GetKeyDown(KeyCode.W)) {
        //    cmc.ForwardSpeed = 1;
        //    cmc.SideSpeed = 0;
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    cmc.ForwardSpeed = -1;
        //    cmc.SideSpeed = 0;
        //}
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    cmc.ForwardSpeed = 0;
        //    cmc.SideSpeed = 1;
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    cmc.ForwardSpeed = 0;
        //    cmc.SideSpeed = -1;
        //}
    }
}
