//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UJNet;
//using UJNet.Data;

//public class BattleManager : MonoBehaviour
//{
//	public enum State{
//		Waiting,
//		Start,
//		Running,
//		Finished
//	}
	
//	public static long battleId;
//	public static int expType;
//	public static string battleSvrAddr;
//	// drop 
//	public GameObject chestPrefab;
//	public GameObject powerDefensePrefab;
//	public GameObject recoverLifePrefab;
//	public GameObject fallStonePrefab;
//	public GameObject leaveWarBtn;
//	//
//	public GameObject[] bulletPrefabs;
//	public GameObject[] flyPickupPrefab;
//	public GameObject[] lifebarPrefabs;
//	// camera
//	public Camera mainCam;
//	public Camera uiCamera;
//	// type ref
//	public Dictionary<int, GameObject> militaryMedalPrefabMap;
//	public Dictionary<int, GameObject> lifebarMap;
//	public Dictionary<int, GameObject> bulletPrefabMap;
//	public Dictionary<int, GameObject> dropTypeMap;
//	public Dictionary<long, PVPDropCtrl> dropCtrlMap;
//	//
//	private BattleTitleCtrl battleTitleCtrl;
//	private GeneralSkillPanelCtrl skillPanelCtrl;
//	//
//	public BattleInfo battle;
//	public State state = State.Waiting;
//	public List<PVPUnitCtrl> battleUnits;
//	private bool inited = false;
//	public NetClient client;
//	// 
//	public delegate void BattleChangedDeleage ();
//	public BattleChangedDeleage OnBattleChanged;
	
//	void Awake ()
//	{
//		lifebarMap = Util.ConvertMap (lifebarPrefabs);
		
//		AddDropTypePrefab();
//		dropCtrlMap = new Dictionary<long, PVPDropCtrl>();
//	}

//	void Start ()
//	{	
//		mainCam.backgroundColor = Color.black;
//		GameObject obj = Instantiate(leaveWarBtn) as GameObject;
//		obj.GetComponent<UIButton>().SetValueChangedDelegate(OnLeaveWarPressed);

//		client = new NetClient(true);
//		client.AddResponseListener (OnResponse);
//		client.AddConnectionListener (OnConnection);
//		client.AddConnectionLostListener(OnConnectionLost);
//		client.AddDebugMessageListener(OnDebugMessage);
		
//		string[] args = battleSvrAddr.Split(new char[]{':'});
//		client.Connect (args[0], int.Parse(args[1]));
		
//		TimeManager.Instance.Init ();
//	}
	
//	void OnConnection (bool success, string error)
//	{
//		if (!success) {
//			Debug.Log ("OnConnection faild " + error);
//			TipBoxCtrl.Instance.Show(I18NManager.Instance.GetProp("pvp.net.connect.faild"), OnJoinBattleFaild, null, null);
//			return;
//		} 
		
//		SendLoginBattle();
//	}
	
//	void OnDebugMessage(string message){
//		Debug.Log("BAT NET: " + message);
//	}	
	
//	void OnConnectionLost ()
//	{
//		Debug.Log ("OnConnectionLost");
//		TipBoxCtrl.Instance.Show (I18NManager.Instance.GetProp("pvp.net.connect.lost"), OnJoinBattleFaild, this, ChangeUIEventArgs.Empty);
//	}	
	
//	void OnEnable ()
//	{
//		OnBattleChanged += OnSwitchBattle;
//	}
	
//	void OnDisable ()
//	{
//		OnBattleChanged -= OnSwitchBattle;
//	}
	
//	void Update ()
//	{
//		if (client != null)
//			client.ProcessEventQueue ();
//	}

//	void InitBattle (BattleInfo battleInfo)
//	{
//		battle = battleInfo;
		
//		BattleTitleCtrl vsPanelCtrl = FindObjectOfType(typeof(BattleTitleCtrl)) as BattleTitleCtrl;
//		vsPanelCtrl.FlushContent (battle.Offense.general, battle.Defense.general, battle.Offense.name, battle.Defense.name);
		
//		InitWallStyle ();
//		skillPanelCtrl = FindObjectOfType (typeof(GeneralSkillPanelCtrl)) as GeneralSkillPanelCtrl;
//		if (battle.Self.general != null && battle.Self.general.GenSkills.Length > 0) {
//			skillPanelCtrl.Init();
//		}
		
//		InitAllArmys ();
		
//		LoadBattleState (battle.id);
//		StartCoroutine (CameraSlide());
//		inited = true;
//	}
	
//	void OnLeaveWarPressed (IUIObject obj)
//	{
//		ConfirmBoxCtrl boxCtrl = FindObjectOfType (typeof (ConfirmBoxCtrl)) as ConfirmBoxCtrl;
//		boxCtrl.OpenSelf (this, OnLeaveWar, null, ConfirmBoxCtrl.CreateSelfArgs(null, I18NManager.Instance.GetProp("war.leave.tip"), null, null));
//	}
	
//	public void OnLeaveWar (int callBackCMD, object sender, ChangeUIEventArgs args, out bool destoryCallBack, out bool closePanel, ChangeUIEventArgs attr)
//	{
//		closePanel = destoryCallBack = false;
//		switch (callBackCMD) {
//		case ConfirmBoxCtrl.BTN_CMD_COMPLETE_BRING_IN:
//			break;
//		case ConfirmBoxCtrl.BTN_CMD_LEFT_PRESSED:
//			SendQuitBattle();
//			ReturnSceneLogic ();
//			closePanel = destoryCallBack = true;
//			break;
//		case ConfirmBoxCtrl.BTN_CMD_RIGHT_PRESSED:
//			closePanel = destoryCallBack = true;
//			break;
//		}
//	}

// private	void ReturnSceneLogic ()
//	{
//		if(!ScopeHolder.attr.ContainsKey(Const.LEAGUE_PVP_RETURN_LEAGUE_ID)){ // mei you cheng bang
//			StartCoroutine(Util.LoadScene(Const.LEVEL_CITY));	
//		}else{
//			long returnLeagueId  =  (long)ScopeHolder.attr[Const.LEAGUE_PVP_RETURN_LEAGUE_ID];
//			if(returnLeagueId <1){ // mei cheng bang
//			ScopeHolder.attr[Const.LEAGUE_PVP_RETURN_LEAGUE_ID] =  null;
//			StartCoroutine(Util.LoadScene(Const.LEVEL_CITY));		
//			}else{
//				UserManager.leagueId =returnLeagueId ;
//				ScopeHolder.attr[Const.LEAGUE_PVP_RETURN_LEAGUE_ID] =  null;
//				StartCoroutine (Util.LoadScene (Const.LEVEL_LEAGUE));					
//			}
//		}
//	}
	
//	IEnumerator CameraSlide ()
//	{	
//		Animation ani = mainCam.GetComponent<Animation>();
//		if (ani != null) {
//			string aniName = String.Format("war{0}_p{1}", battle.sceneId, battle.Self.expType);
//			string suffix = "";
//			if (SystemInfo.deviceModel.ToLower().IndexOf("ipad") != -1) {
//				suffix = "_ipad";
//			}
//			aniName = aniName + suffix;
			
//			if (ani.GetClip (aniName) == null) {
//				Debug.LogWarning ("ani not found " + aniName);
//			} else {
//				ani.Play(aniName);
//				yield return new WaitForSeconds(0.001f);
//				LevelLoadCtrl.Instance.HidePanel ();

//				int time = Mathf.RoundToInt (ani[aniName].length);
//				yield return new WaitForSeconds (time);
//			}
//		}
	
//		PinchCamera pinchCtrl = FindObjectOfType (typeof(PinchCamera)) as PinchCamera;
//		pinchCtrl.Init();
		
//		yield return null;
//	}	
	
//	void OnSwitchBattle ()
//	{
//		if (state == State.Finished) {
//			skillPanelCtrl = FindObjectOfType (typeof(GeneralSkillPanelCtrl)) as GeneralSkillPanelCtrl;
//			skillPanelCtrl.DisableSkill ();
//		}
//	}
	
//	void OnWaitTimeEnd ()
//	{
//		TimeLeftPanelCtrl waitTimeCtrl = FindObjectOfType (typeof (TimeLeftPanelCtrl)) as TimeLeftPanelCtrl;
//		waitTimeCtrl.HidePanel ();
		
//		if (state == State.Waiting) {
//			state = State.Running;
//			OnBattleChanged ();
//		}
//	}
	
//	void InitAllArmys ()
//	{
//		battleUnits = new List<PVPUnitCtrl> ();
//		foreach (BattleArmy unit in battle.ArmyList) {
//			PVPUnitCtrl ctrl = CreatePVPUnit (unit);
//			ctrl.Init (unit);
//			battleUnits.Add (ctrl);
//		}
		
//		foreach (BattleArmy unit in battle.ArmyList) {
//			PVPUnitCtrl ctrl = GetPVPUnitById (unit.id);
//			if (ctrl != null) {
//				IgnoreCollision (ctrl);
//			}
//		}		
//	}
	
//	public void IgnoreCollision (PVPUnitCtrl unitCtrl)
//	{
//		foreach (BattleArmy unit in battle.ArmyList) {
//			PVPUnitCtrl ctrl = GetPVPUnitById (unit.id);
//			if (ctrl != null && !ctrl.Dead && ctrl != unitCtrl) {
//				Physics.IgnoreCollision (ctrl.GetComponent<Collider>(), unitCtrl.GetComponent<Collider>());
//			}
//		}
//	}	
	
//	void InitWallStyle ()
//	{
//		int level = battle.Defense.wallLevel;
//		if (level <= 0) return;
		
//		Transform cell = GameObject.Find("pos_chengqiang").transform;
		
//		CityStyleCtrl styleCtrl = FindObjectOfType (typeof(CityStyleCtrl)) as CityStyleCtrl;
//		string styleName = "style.wall";
//		GameObject prefab = styleCtrl.GetCityStylePrefabByIdAndLevel (12, level);
//		GameObject newStyle = Instantiate (prefab) as GameObject;
//		newStyle.name = styleName;
		
//		newStyle.transform.parent = cell;
//		newStyle.transform.localPosition = Vector3.zero;
//		newStyle.transform.localRotation = Quaternion.identity;
//		newStyle.transform.localScale = Vector3.one;
//	}

//	void AddDropTypePrefab()
//	{
//		dropTypeMap = new Dictionary<int, GameObject>();
//		dropTypeMap.Add(1, chestPrefab);
//		dropTypeMap.Add(2, recoverLifePrefab);
//		dropTypeMap.Add(3, powerDefensePrefab);
//		dropTypeMap.Add(4, fallStonePrefab);
//	}
	
//	public int GetUnitPrefabId (BattleArmy unit)
//	{
//		return unit.model;
//	}	
		
//	public PVPUnitCtrl GetPVPUnitById (long armyId)
//	{
//		foreach (PVPUnitCtrl ctrl in battleUnits) {
//			if (ctrl.unitInfo.id == armyId) return ctrl;
//		}
//		return null;
//	}
	
//	public GameObject GetLifebarPrefab (BattleArmy army)
//	{
//		int id = 1;
//		if (army.prototypeId == 10) id = 2;
//		return lifebarMap [id];		
//	}	

//	public PVPUnitCtrl CreatePVPUnit (BattleArmy unit)
//	{
//		Debug.Log ("Unit: " + unit.owner.id + "/" + unit.id + "/" + unit.name + "/" + unit.prototypeId);
//		GameObject obj = Instantiate (LoadUnitPrefab(unit, unit.owner.Self)) as GameObject;
//		return obj.GetComponent<PVPUnitCtrl>();
//	}
	
//	public GameObject LoadUnitPrefab(BattleArmy unit, bool self)
//	{
//		int prefabId = GetUnitPrefabId (unit);
//		string prefabPath = "prefab/pvp_units/army_group_" + prefabId;
//		if (!self) {
//			prefabPath = "prefab/pvp_units/blue/army_group_" + prefabId;
//		}
//		return (GameObject)Resources.Load(prefabPath);
//	}	
	
//	public GameObject LoadUnitDiePrefab(BattleArmy unit, bool self)
//	{
//		int prefabId = GetUnitPrefabId (unit);
//		string prefabPath = "prefab/units_die/armyPrefab_die_" + prefabId;
//		if (!self) {
//			prefabPath = "prefab/units_die/blue/armyPrefab_die_" + prefabId;
//		}
//		return (GameObject)Resources.Load(prefabPath);
//	}

//	public void SendLoadBattle (long id)
//	{
//		Debug.Log("2");
//		UJObject param = UJObject.NewInstance ();
//		param.PutLong ("bid", id);
//		client.SendViaSocket (802, param);
//	}

//	public void SendTransform (long armyId, NetTransform trans)
//	{
//		UJObject param = UJObject.NewInstance ();
//		param.PutLong ("aid", armyId);
//		param.PutLong ("bid", battle.id);
//		param.PutUJObject ("transform", trans.ToUJObject ());
//		client.SendViaSocket (804, param);
//	}
	
//	public void SendAttack (long killer, long target)
//	{
//		UJObject param = UJObject.NewInstance ();
//		param.PutLong("kaid", killer);
//		param.PutLong ("aid", target);
//		param.PutLong ("bid", battle.id);
//		client.SendViaSocket (807, param);
//	}
	
//	public void SendDamage (long killer, long target)
//	{
//		UJObject param = UJObject.NewInstance ();
//		param.PutLong("aid", target);
//		param.PutInt("type", 1);
//		param.PutLong("kaid", killer);
//		param.PutLong ("bid", battle.id);
//		client.SendViaSocket (827, param);
//	}
	
//	public void SendDamage (long killer, Vector3 pos, float radius)
//	{
//		UJObject param = UJObject.NewInstance ();
//		param.PutFloat("x", pos.x);
//		param.PutFloat("z", pos.z);
//		param.PutFloat("rd", radius);
//		param.PutInt("type", 2);
//		param.PutLong("kaid", killer);
//		param.PutLong ("bid", battle.id);
//		client.SendViaSocket (827, param);
//	}	

//	public void LoadBattleState (long bid)
//	{
//		UJObject param = UJObject.NewInstance ();
//		param.PutLong("bid",bid);
//		client.SendViaSocket (800, param);
//	}

//	public void SendTriggerFallStone(int id) {
//		UJObject param = UJObject.NewInstance ();
//		param.PutInt("id", id);
//		param.PutLong ("bid", battle.id);
//		client.SendViaSocket (819, param);
//	}
	
//	public void SendTriggerChest(int id, long aid,long cid) {
//		UJObject param = UJObject.NewInstance ();
//		param.PutInt("id", id);
//		param.PutLong ("bid", battle.id);
//		param.PutLong("aid", aid);
//		param.PutLong("cid", cid);
//		client.SendViaSocket (821, param);
//	}
	
//	public void SendTriggerPowerDefense(int id, long aid) {
//		UJObject param = UJObject.NewInstance ();
//		param.PutInt("id", id);
//		param.PutLong ("bid", battle.id);
//		param.PutLong("aid", aid);
//		client.SendViaSocket (820, param);
//	}

//	public void SendTriggerRecoverLife(int id, long aid) {
//		UJObject param = UJObject.NewInstance ();
//		param.PutInt("id", id);
//		param.PutLong ("bid", battle.id);
//		param.PutLong("aid", aid);
//		client.SendViaSocket (822, param);
//	}
	
//	public void SendQuitBattle ()
//	{
//		UJObject param = UJObject.NewInstance ();
//		param.PutLong ("bid", battle.id);
//		client.SendViaSocket (810, param);		
//	}
	
//	public void SendLoginBattle()
//	{
//		UJObject param = UJObject.NewInstance ();
//		param.PutUtfString("jid", (string)ScopeHolder.attr[Const.SCOPE_JID]);
//		param.PutUtfString("luid", (string)ScopeHolder.attr[Const.SCOPE_AUTH_KEY]);
//		client.SendViaSocket (828, param);		
//	}

//	void OnResponse (int cmd, UJObject param)
//	{
//		if (cmd == 802) {
//			LoadBattleInfo_Itnl (param);
//		} else if (cmd == 804) {
//			RecvTransform_Itnl (param);
//		}  else if (cmd == 808) {
//			Recv_Attack (param);
//		} else if (cmd == 809) {
//			Recv_Damage (param);
//		} else if (cmd == 800) {
//			LoadBattleState_Itnl (param);
//		} else if (cmd == 813) {
//			Recv_PlayerStateChanged (param);
//		} else if (cmd == 814) {
//			Recv_BattleFinish (param);
//		}
//		// drop item
//		else if (cmd == 825) {
//			Recv_RespawnDrop_Itnl(param);
//		} else if (cmd == 823) {
//			Recv_TimeoutDrop_Itnl(param);
//		} else if (cmd == 824) {
//			Recv_DisappearDrop_Itnl(param);
//		} else if (cmd == 820) {
//			SendTriggerPowerDefense_Itnl(param);
//		} else if (cmd == 821) {
//			SendTriggerChest_Itnl(param);
//		} else if (cmd == 822) {
//			SendTriggerRecoverLife_Itnl(param);
//		} 
//		// login
//		else if (cmd == 828) {
//			SendLoginBattle_Itnl(param);
//		}
//	}
	
//	void LoadBattleInfo_Itnl (UJObject param)
//	{
//		int state = param.GetInt ("state");
//		if (state != 1) {
//			TipBoxCtrl.Instance.Show (param.GetUtfString ("msg"), OnJoinBattleFaild, null, null);
//			return;
//		}
		
//		BattleInfo battleInfo = new BattleInfo ();
//		battleInfo.LoadFromUJObject (param);
	
//		InitBattle (battleInfo);
//	}
	
//	void OnJoinBattleFaild (object sender, ChangeUIEventArgs args) {
//		ReturnSceneLogic();
//	}

//	void RecvTransform_Itnl (UJObject param)
//	{
//		if (!inited) return;
		
//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("RecvTransform_Itnl ST: " + state);
//			return;
//		}
		
//		long armyId = param.GetLong ("aid");
//		UJObject trans = param.GetUJObject ("transform");
		
//		PVPUnitCtrl ctrl = GetPVPUnitById (armyId);
//		if (ctrl == null || ctrl.Dead) return;
		
//		if (!ctrl.unitInfo.owner.Self && ctrl.unitInfo.owner.state == BattlePlayer.JOIN) {
//			NetTransform netTran = NetTransform.FromUJObject (trans);
//			ctrl.GetComponent<TransformReceiver>().Receive (netTran);
//		}
//	}

//	void Recv_Attack (UJObject param)
//	{
//		if (!inited) return;
		
//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("Recv_Attack ST: " + state);
//			return;
//		}
		
//		long armyId = param.GetLong ("aid");
//		long killer = param.GetLong ("kaid");
		
//		PVPUnitCtrl ctrl = GetPVPUnitById (killer);
//		if (ctrl == null || ctrl.Dead) return;
		
//		PVPUnitCtrl target = GetPVPUnitById (armyId);
//		if (target == null || target.Dead) return;
		
//		Debug.Log("recv fire " + ctrl.name);
//		ctrl.Fire (target);
//	}

//	void Recv_Damage (UJObject param)
//	{
//		if (!inited) return;
		
//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("Recv_Damage ST: " + state);
//			return;
//		}
		
//		long killer = param.GetLong ("kaid");
//		UJArray units = param.GetUJArray("armys");

//		PVPUnitCtrl ctrl = GetPVPUnitById (killer);
//		if (ctrl == null) return;
		
//		Debug.Log("recv damage " + ctrl.name);
		
//		for(int i = 0; i < units.Size(); i++) {
//			UJObject obj = units.GetUJObject (i);
//			long armyId = obj.GetLong("aid");
//			int damage = obj.GetInt("dmg");
//			int remain = obj.GetInt("rem");

//			PVPUnitCtrl target = GetPVPUnitById (armyId);
//			if (target == null || target.Dead) return;
			
//			Debug.Log("damage " + target.name + "/" + damage + "/" + remain);
//			if (remain + damage != target.Health) {
//				Debug.LogWarning (" no sync " + target.name + "/" + target.Health + "/" + (remain + damage));
//			}
//			target.OnDamage (ctrl, Mathf.Max(0, target.Health - remain));
//		}		
//	}

//	void Recv_BattleFinish (UJObject param)
//	{
//		if (!inited) return;
		
//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("Recv_BattleFinish ST: " + state+"/"+param.GetUtfString("msg"));
//			return;
//		}

//		int battleState = param.GetInt ("batstate");
//		long reportId = param.GetLong ("rid");
		
//		Debug.Log ("Recv_BattleFinish " + battleState + "/" + reportId);
		
//		this.state = State.Finished;
//		OnBattleChanged ();

//		BattleReportV2Ctrl battleReportCtrl = FindObjectOfType (typeof(BattleReportV2Ctrl)) as  BattleReportV2Ctrl;
//		battleReportCtrl.ShowPanel (this, OnReportCallBack, reportId, null);
//	}

//	void OnReportCallBack (int callBackCMD, object sender, ChangeUIEventArgs args, out bool destoryCallBack, out bool closePanel, ChangeUIEventArgs attr)
//	{
//		closePanel = false;
//		destoryCallBack = false;
	
//		switch (callBackCMD) {
//		case BattleReportV2Ctrl.CALLBACK_CMD_BTN_PRESSED:
//			destoryCallBack = closePanel = true;
//			ReturnSceneLogic();
//			break;
//		case BattleReportV2Ctrl.CALLBACK_CMD_RESPONES:
//			BattleReport br = args.Data as BattleReport;
//			Debug.Log("///pve battle result " + br.result);
//			GetComponent<AudioSource>().Stop ();
//			GetComponent<AudioSource>().loop = false;
//			SoundManager.Instance.PlayBattleFinish (GetComponent<AudioSource>(), br.result == BattleReport.WIN);
//			break;
//		}
//	}

//	void LoadBattleState_Itnl (UJObject param)
//	{
//		if (!inited) return;
		
//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("LoadBattleMessage_Itnl ST: " + state);
//			return;
//		}
		
//		BattleState battleState = new BattleState();
//		battleState.LoadFromUJObject (param);

//		Debug.Log ("LoadBattleMessage " + battleState.State + "/" + battleState.CurrentTime + "/" + battleState.FinishTime + "/" + (battleState.FinishTime - battleState.CurrentTime)); 
		
//		switch (battleState.State) {
//		case BattleInfo.BT_BATTLESTATE_WAIT:
//			this.state = State.Waiting;
			
//			TimeLeftPanelCtrl waitTimeCtrl = FindObjectOfType (typeof (TimeLeftPanelCtrl)) as TimeLeftPanelCtrl;
//			waitTimeCtrl.OnTimerTaskEnd += OnWaitTimeEnd;
//			waitTimeCtrl.InitTimer (Convert.ToDouble (battleState.CurrentTime), battleState.FinishTime);
//			waitTimeCtrl.ShowPanel ();
			
//			break;
//		case BattleInfo.BT_BATTLESTATE_BEGIN:
//			OnWaitTimeEnd ();
//			BattleRemainTimeCtrl remainTimeCtrl = FindObjectOfType (typeof(BattleRemainTimeCtrl)) as BattleRemainTimeCtrl;
//			remainTimeCtrl.Init (battleState.FinishTime);
			
//			break;
//		}
//	}

//	void Recv_PlayerStateChanged (UJObject param)
//	{
//		if (!inited) return;
		
//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("Recv_PlayerStateChanged ST: " + state);
//			return;
//		}
		
//		long userId = param.GetLong ("uid");
//		int batstate = param.GetInt ("ubatstate");
		
//		BattlePlayer player = battle.GetPlayerById (userId);
//		if (player == null) {
//			Debug.LogWarning ("Recv_PlayerStateChanged: Null player by " + userId); 
//			return;
//		}
		
//		Debug.Log ("Recv_PlayerStateChanged: uid " + player.id + " from: " + player.state + " to: " + batstate);
		
//		player.state = batstate;
//		foreach (BattleArmy army in player.armyList) {
//			PVPUnitCtrl ctrl = GetPVPUnitById (army.id);
//			if (ctrl == null || ctrl.Dead)
//				continue;
			
//			ctrl.OnCtrlModeChanged ();
//		}
//	}
	
//	void Recv_RespawnDrop_Itnl (UJObject param) 
//	{
//		if (!inited) return;

//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("Recv_CheckDrop_Itnl ST: " + state);
//			return;
//		}
		
//		PVPDropItem dropItem = new PVPDropItem();
//		dropItem.LoadFromUJObject(param);
		
//		GameObject go = Instantiate(dropTypeMap[dropItem.type]) as GameObject;
//		PVPDropCtrl dropCtrl = go.GetComponent(typeof(PVPDropCtrl)) as PVPDropCtrl ;
//		dropCtrl.Init(dropItem);
//		dropCtrlMap.Add(dropItem.id, dropCtrl);
//		//Debug.Log("reswpan " + dropItem.id+"/"+dropItem.type+"/"+ (dropItem.disapTime - UserManager.curTime) +"/" +dropCtrlMap.Count);
//	}
	
//	void Recv_TimeoutDrop_Itnl (UJObject param) 
//	{
//		if (!inited) return;

//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("Recv_TimeoutDrop_Itnl ST: " + state);
//			return;
//		}
		
//		PVPDropItem dropItem = new PVPDropItem();
//		dropItem.LoadFromUJObject(param);

//		if (!dropCtrlMap.ContainsKey(dropItem.id)) return;
////		dropCtrlMap[dropItem.id].Disappear();
////		Debug.Log("timeout " + dropItem.id+"/"+dropCtrlMap.Count);
//	}

//	void Recv_DisappearDrop_Itnl (UJObject param) 
//	{
//		if (!inited) return;

//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("Recv_DisappearDropItem_Itnl ST: " + state);
//			return;
//		}

//		PVPDropItem dropItem = new PVPDropItem();
//		dropItem.LoadFromUJObject(param);

//		if (!dropCtrlMap.ContainsKey(dropItem.id)) return;
//		//dropCtrlMap[dropItem.id].Disappear();
//		//Debug.Log("disappear " + dropItem.id+"/"+dropCtrlMap.Count);
//	}	
	
//	void SendTriggerPowerDefense_Itnl(UJObject param) {
//		if (!inited) return;

//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("SendTriggerPowerDefense_Itnl ST: " + state+"/"+param.GetUtfString("msg"));
//			return;
//		}
		
//		PVPTriggerPowerDefense pd = new PVPTriggerPowerDefense();
//		pd.LoadFromUJObject(param);

//		Debug.Log("trigger power def " + pd.id+"/"+dropCtrlMap.Count);

//		if (!dropCtrlMap.ContainsKey(pd.id)) return;
//		dropCtrlMap[pd.id].TriggerItem(pd);
//	}
	
//	void SendTriggerChest_Itnl(UJObject param) {
//		if (!inited) return;

//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("SendTriggerChest_Itnl ST: " + state+"/"+param.GetUtfString("msg"));
//			return;
//		}
		
//		PVPTriggerChest ch = new PVPTriggerChest();
//		ch.LoadFromUJObject(param);
		
//		Debug.Log("trigger chest " + ch.id+"/"+dropCtrlMap.Count);
		
//		if (!dropCtrlMap.ContainsKey(ch.id)) return;
//		dropCtrlMap[ch.id].TriggerItem(ch);
//	}
	
//	void SendTriggerRecoverLife_Itnl(UJObject param) {
//		if (!inited) return;

//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("SendTriggerRecoverLife_Itnl ST: " + state+"/"+param.GetUtfString("msg"));
//			return;
//		}
		
//		PVPTriggerRecoverLife rl = new PVPTriggerRecoverLife();
//		rl.LoadFromUJObject(param);

//		Debug.Log("trigger recover life " + rl.id+"/"+dropCtrlMap.Count);
		
//		if (!dropCtrlMap.ContainsKey(rl.id)) return;
//		dropCtrlMap[rl.id].TriggerItem(rl);
//	}	
	
//	void SendLoginBattle_Itnl (UJObject param)
//	{
//		int state = param.GetInt ("state");
//		if (state != 1) {
//			Debug.LogWarning ("LoginBattlel_Itnl ST: " + state+"/"+param.GetUtfString("msg"));
//			TipBoxCtrl.Instance.Show (param.GetUtfString("msg"), OnJoinBattleFaild, this, ChangeUIEventArgs.Empty);
//			return;
//		}
		
//		string jid = param.GetUtfString("jid");
//		if (!string.IsNullOrEmpty(jid)) {
//			ScopeHolder.attr[Const.SCOPE_JID] = jid;
//		}
		
//		SendLoadBattle(battleId);
//	}
//}
