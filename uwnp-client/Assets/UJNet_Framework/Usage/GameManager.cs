using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UJNet;
using UJNet.Data;
using System;
using System.Text;

/// <summary>
/// Game manager. 
///  notice : this is the first one to run all scene
/// </summary>
public class GameManager : MonoBehaviour
{



	public const int CMD_PAYMENT_REQUEST = 1007; // prepare to buy
	public const int CMD_LOAD_PAY_COMPLETE_LIST = 1010; //
	/// <summary>
	/// 顽石刷新订单协议号
	/// </summary>
	public const int CMD_WISTONE_GET_ORDER = 1016;

	/// <summary>
	/// 临时缓存产品id的map
	/// </summary>
	private Dictionary<long, long> payMentList = new Dictionary<long, long> ();
	public bool isShowShop = true;

	public bool IsShowShop {
		get {
			return this.isShowShop;
		}
	}

	public static GameManager GetInstance ()
	{
		return GameObject.FindObjectOfType (typeof(GameManager)) as GameManager;
	}

	public void OnResponse (int cmd, SFSObject param)
	{
		switch (cmd) {
		case CMD_PAYMENT_REQUEST:
			Debug.Log ("OnResponse CMD_PAYMENT_REQUEST");
			LoadPaymentRequest (param, cmd);
			break;
		case CMD_LOAD_PAY_COMPLETE_LIST:
			LoadPayCompleteList (param, cmd);
			break;
		case CMD_WISTONE_GET_ORDER:
			LoadWistoneGetOrder (param, cmd);
			break;
		case CMD_GEN_360_ORDER_ID:
			Load360GetOrder (param, cmd);
			break;
		case CMD_GEN_UC_ORDER_ID:
			LoadUCGetOrder (param, cmd);
			break;
        case CMD_GEN_DangLe_ORDER_ID:
            LoadDangLeGetOrder(param, cmd);
            break;
        case CMD_GEN_CHINATELECOM_ORDER_ID:
            LoadCHINATELECOMOrder(param, cmd);
            break;
        case CMD_PAY_CHINATELECOM_CHECK:
            LoadCHINATELECOMPayCheck(param, cmd);
            break;
		case CMD_GEN_MI_ORDER_ID:
			LoadMIGetOrder (param, cmd);
			break;
		case CMD_GEN_TSTORE_ORDER_ID:
			LoadTStoreOrder (param, cmd);
			break;
		case CMD_PAY_TSTORE_CHECK:
			LoadTStorePayCheck (param, cmd);
			break;
		case CMD_GEN_OLLEH_ORDER_ID:
			LoadOLLEHOrder (param, cmd);
			break;
		case CMD_PAY_OLLEH_CHECK:
			LoadOLLEHPayCheck (param, cmd);
			break;
		case CMD_GEN_LGU_ORDER_ID:
			LoadLGUOrder (param, cmd);
			break;
		case CMD_PAY_LGU_CHECK:
			LoadLGUPayCheck (param, cmd);
			break;
		}
	}

	public void SendLoad360GetOrder (int gid, string proId, int type)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutInt ("gid", gid);
		param.PutUtfString ("productid", proId);
		//  360  shi 6
		param.PutInt ("type", type);
		Debug.Log ("SendLoad360GetOrder with gid : " + gid + " proId : " + proId + " type : " + type);
		if (client != null) {
			client.SendRequest (CMD_GEN_360_ORDER_ID, param);
		}
	}

	public void SendLoadUCGetOrder (int gid, string proId, int type)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutInt ("gid", gid);
		param.PutUtfString ("productid", proId);
		param.PutInt ("type", type);
		Debug.Log ("SendLoadUCGetOrder with gid : " + gid + " proId : " + proId + " type : " + type);
		if (client != null) {
			client.SendRequest (CMD_GEN_UC_ORDER_ID, param);
		}
	}

	public void SendLoadMIGetOrder (int gid, string proId, int type)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutInt ("gid", gid);
		param.PutUtfString ("productid", proId);
		param.PutInt ("type", type);
		Debug.Log ("SendLoadUCGetOrder with gid : " + gid + " proId : " + proId + " type : " + type);
		if (client != null) {
			client.SendRequest (CMD_GEN_MI_ORDER_ID, param);
		}
	}

	public void SendLoadTStoreGetOrder (int gid, string proId)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutInt ("gid", gid);
		param.PutUtfString ("productid", proId);
		Debug.Log ("SendLoadTStoreGetOrder with gid : " + gid + " proId : " + proId);
		if (client != null) {
			client.SendRequest (CMD_GEN_TSTORE_ORDER_ID, param);
		}
	}

	public void SendCheckTStorePayState (long prid, string orderid)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutLong ("prid", prid);
		param.PutUtfString ("orderid", orderid);
		Debug.Log ("SendCheckTStorePayState with prid : " + prid + " orderid : " + orderid);
		if (client != null) {
			client.SendRequest (CMD_PAY_TSTORE_CHECK, param);
		}
	}

    public void SendLoadCHINATELECOMGetOrder(int gid, string orderId)
    {
        SFSObject param = SFSObject.NewInstance();
        param.PutInt("gid", gid);
        param.PutUtfString("OrderId", orderId);
        Debug.Log("SendLoadCHINATELECOMGetOrder with gid : " + gid + " proId : " + orderId);
        if (client != null)
        {
            client.SendRequest(CMD_GEN_CHINATELECOM_ORDER_ID, param);
        }
    }

    public void SendCheckCHINATELECOMPayState(long prid, string orderid, string pcode, string pmsg)
    {
        SFSObject param = SFSObject.NewInstance();
        param.PutLong("prid", prid);
        param.PutUtfString("orderid", orderid);
        param.PutUtfString("pcode", pcode);
        param.PutUtfString("pmsg", pmsg);
        Debug.Log("SendCheckCHINATELECOMPayState with prid : " + prid + " orderid : " + orderid);
        if (client != null)
        {
            client.SendRequest(CMD_PAY_CHINATELECOM_CHECK, param);
        }
    }

	public void SendLoadOLLEHGetOrder (int gid, string proId)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutInt ("gid", gid);
		param.PutUtfString ("productid", proId);
		Debug.Log ("SendLoadOLLEHGetOrder with gid : " + gid + " proId : " + proId);
		if (client != null) {
			client.SendRequest (CMD_GEN_OLLEH_ORDER_ID, param);
		}
	}

	public void SendCheckOLLEHPayState (long prid, string orderid, string pcode, string pmsg)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutLong ("prid", prid);
		param.PutUtfString ("orderid", orderid);
		param.PutUtfString ("pcode", pcode);
		param.PutUtfString ("pmsg", pmsg);
		Debug.Log ("SendCheckOllehPayState with prid : " + prid + " orderid : " + orderid);
		if (client != null) {
			client.SendRequest (CMD_PAY_OLLEH_CHECK, param);
		}
	}

	public void SendCheckLGUPayState (long prid, string orderid, string pcode, string pmsg)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutLong ("prid", prid);
		param.PutUtfString ("orderid", orderid);
		param.PutUtfString ("pcode", pcode);
		param.PutUtfString ("pstatus", pmsg);
		Debug.Log ("SendCheckLGUPayState with prid : " + prid + " orderid : " + orderid);
		if (client != null) {
			client.SendRequest (CMD_PAY_LGU_CHECK, param);
		}
	}

	public void SendLoadLGUGetOrder (int gid, string proId)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutInt ("gid", gid);
		param.PutUtfString ("productid", proId);
		Debug.Log ("SendLoadLGUGetOrder with gid : " + gid + " proId : " + proId);
		if (client != null) {
			client.SendRequest (CMD_GEN_LGU_ORDER_ID, param);
		}
	}

	private void Load360GetOrder (SFSObject param, int cmd)
	{
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("Load360GetOrder : " + param.GetUtfString ("msg"));
			return;
		}
		//int type = param.GetInt ("type");
		long prid = param.GetLong ("prid");
		string orderId = param.GetUtfString ("app_ext1");
		string notifyUrl = param.GetUtfString ("notifyurl");
		Debug.Log ("Load360GetOrder  : prid : " + prid + " orderId : " + orderId + " notifyUrl : " + notifyUrl);
		if (severRequest360OrderCallBack != null) {
			severRequest360OrderCallBack (prid, orderId, notifyUrl);
			severRequest360OrderCallBack = null;
		}
	}

	private void LoadUCGetOrder (SFSObject param, int cmd)
	{
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("LoadUCGetOrder : " + param.GetUtfString ("msg"));
			return;
		}

		//int type = param.GetInt ("type");
		long prid = param.GetLong ("prid");
		string orderId = param.GetUtfString ("orderid");
		Debug.Log ("LoadUCGetOrder  : prid : " + prid + " orderId : " + orderId);

		if (severRequestUCOrderCallBack != null) {
			severRequestUCOrderCallBack (prid, orderId);
			severRequestUCOrderCallBack = null;
		}
	}

    public void SendLoadDangLeGetOrder(int gid, string orderId, int type)
    {
        SFSObject param = SFSObject.NewInstance();
        param.PutInt("gid", gid);
        param.PutUtfString("orderId", orderId);
        param.PutInt("type", type);
        Debug.Log("SendLoadDangLeGetOrder with gid : " + gid + " proId : " + orderId + " type : " + type);
        if (client != null)
        {
            client.SendRequest(CMD_GEN_DangLe_ORDER_ID, param);
        }
    }

    private void LoadDangLeGetOrder(SFSObject param, int cmd)
    {
        if (param.GetInt("state") == -1)
        {
            Debug.LogWarning("LoadDangLeGetOrder : " + param.GetUtfString("msg"));
            return;
        }

        //int type = param.GetInt("type");
        long prid = param.GetLong("prid");
        string orderId = param.GetUtfString("orderid");
        Debug.Log("LoadDangLeGetOrder  : prid : " + prid + " orderId : " + orderId);

        if (severRequestDangLeOrderCallBack != null)
        {
            severRequestDangLeOrderCallBack(prid, orderId);
            severRequestDangLeOrderCallBack = null;
        }
    }

	private void LoadMIGetOrder (SFSObject param, int cmd)
	{
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("LoadMIGetOrder : " + param.GetUtfString ("msg"));
			return;
		}

		//int type = param.GetInt ("type");
		long prid = param.GetLong ("prid");
		string orderId = param.GetUtfString ("orderid");
		Debug.Log ("LoadMIGetOrder  : prid : " + prid + " orderId : " + orderId);
		if (severRequestMIOrderCallBack != null) {
			severRequestMIOrderCallBack (prid, orderId);
			severRequestMIOrderCallBack = null;
		}
	}

	private void LoadTStoreOrder (SFSObject param, int cmd)
	{
		//		Debug.LogWarning ("loadTStoreOrder : " + param.GetUtfString ("msg"));
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("loadTStoreOrder : " + param.GetUtfString ("msg"));
			return;
		}

		int type = param.GetInt ("type");
		long prid = param.GetLong ("prid");
		string orderid = param.GetUtfString ("orderid");
		Debug.Log ("LoadTStoreOrder : orderid:" + orderid + "_prid : " + prid + "_type : " + type);
		if (severRequestTStoreCallBack != null) {
			severRequestTStoreCallBack (prid, orderid);
			//SendCheckTStorePayState(prid,orderid);
			severRequestTStoreCallBack = null;
		}
	}

	private void LoadTStorePayCheck (SFSObject param, int cmd)
	{
		//		Debug.LogWarning ("LoadTStorePayCheck : " + param.GetUtfString ("msg"));
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("LoadTStorePayCheck : " + param.GetUtfString ("msg"));
			return;
		}
		int iapstate = param.GetInt ("iapstate");
		string prid = param.GetUtfString ("prid");
		Debug.Log ("LoadTStorePayCheck : iapstate:" + iapstate + " _prid : " + prid);
		if (severRequestTStorePayCheckCallBack != null) {
			severRequestTStorePayCheckCallBack (iapstate, prid);
			severRequestTStorePayCheckCallBack = null;
		}

	}

    //====================
    private void LoadCHINATELECOMOrder(SFSObject param, int cmd)
    {
        //		Debug.LogWarning ("loadTStoreOrder : " + param.GetUtfString ("msg"));
        if (param.GetInt("state") == -1)
        {
            Debug.LogWarning("loadCHINATELECOMOrder : " + param.GetUtfString("msg"));
            return;
        }

        int type = param.GetInt("type");
        long prid = param.GetLong("prid");
        string orderid = param.GetUtfString("orderid");
        Debug.Log("LoadCHINATELECOMOrder : orderid:" + orderid + "_prid : " + prid + "_type : " + type);
        if (severRequestCHINATELECOMCallBack != null)
        {
            Debug.Log("call send check ----------------------------CHINATELECOM");
            severRequestCHINATELECOMCallBack(prid, orderid);		//after Test open this  close the belowui
            //SendCheckCHINATELECOMPayState(prid,orderid,"0","SUCCESS");
            severRequestCHINATELECOMCallBack = null;
        }
    }

    private void LoadCHINATELECOMPayCheck(SFSObject param, int cmd)
    {
        //		Debug.LogWarning ("LoadTStorePayCheck : " + param.GetUtfString ("msg"));
        if (param.GetInt("state") == -1)
        {
            Debug.LogWarning("LoadCHINATELECOMPayCheck : " + param.GetUtfString("msg"));
            return;
        }
        int iapstate = param.GetInt("iapstate");
        string prid = param.GetUtfString("prid");
        Debug.Log("LoadCHINATELECOMPayCheck : iapstate:" + iapstate + " _prid : " + prid);
        if (severRequestCHINATELECOMPayCheckCallBack != null)
        {
            severRequestCHINATELECOMPayCheckCallBack(iapstate, prid);
            severRequestCHINATELECOMPayCheckCallBack = null;
        }
        else
        {
            Debug.Log("severRequestCHINATELECOMPayCheckCallBack == null--------------");
        }

    }
    //==================

	//====================
	private void LoadOLLEHOrder (SFSObject param, int cmd)
	{
		//		Debug.LogWarning ("loadTStoreOrder : " + param.GetUtfString ("msg"));
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("loadOLLEHOrder : " + param.GetUtfString ("msg"));
			return;
		}

		int type = param.GetInt ("type");
		long prid = param.GetLong ("prid");
		string orderid = param.GetUtfString ("orderid");
		Debug.Log ("LoadOLLEHOrder : orderid:" + orderid + "_prid : " + prid + "_type : " + type);
		if (severRequestOLLEHCallBack != null) {
			Debug.Log ("call send check ----------------------------olleh");
			severRequestOLLEHCallBack (prid, orderid);		//after Test open this  close the belowui
			//SendCheckOLLEHPayState(prid,orderid,"0","SUCCESS");
			severRequestOLLEHCallBack = null;
		}
	}

	private void LoadOLLEHPayCheck (SFSObject param, int cmd)
	{
		//		Debug.LogWarning ("LoadTStorePayCheck : " + param.GetUtfString ("msg"));
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("LoadOLLEHPayCheck : " + param.GetUtfString ("msg"));
			return;
		}
		int iapstate = param.GetInt ("iapstate");
		string prid = param.GetUtfString ("prid");
		Debug.Log ("LoadOLLEHPayCheck : iapstate:" + iapstate + " _prid : " + prid);
		if (severRequestOLLEHPayCheckCallBack != null) {
			severRequestOLLEHPayCheckCallBack (iapstate, prid);
			severRequestOLLEHPayCheckCallBack = null;
		} else {
			Debug.Log ("severRequestOLLEHPayCheckCallBack == null--------------");
		}

	}
	//==================

	private void LoadLGUOrder (SFSObject param, int cmd)
	{
		//		Debug.LogWarning ("loadTStoreOrder : " + param.GetUtfString ("msg"));
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("loadLGUOrder : " + param.GetUtfString ("msg"));
			return;
		}

		int type = param.GetInt ("type");
		long prid = param.GetLong ("prid");
		string orderid = param.GetUtfString ("orderid");
		Debug.Log ("LoadLGUOrder : orderid:" + orderid + "_prid : " + prid + "_type : " + type);
		if (severRequestLGUCallBack != null) {
			severRequestLGUCallBack (prid, orderid);	//after Test open this  close the below
			//SendCheckLGUPayState(prid,orderid,"1","2");
			severRequestLGUCallBack = null;
		}
	}

	private void LoadLGUPayCheck (SFSObject param, int cmd)
	{
		//		Debug.LogWarning ("LoadTStorePayCheck : " + param.GetUtfString ("msg"));
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("LoadLGUPayCheck : " + param.GetUtfString ("msg"));
			return;
		}
		int iapstate = param.GetInt ("iapstate");
		string prid = param.GetUtfString ("prid");
		Debug.Log ("LoadLGUPayCheck : iapstate:" + iapstate + " _prid : " + prid);
		if (severRequestLGUPayCheckCallBack != null) {
			severRequestLGUPayCheckCallBack (iapstate, prid);
			severRequestLGUPayCheckCallBack = null;
		}

	}

	public void SendLoadWistoneGetOrder (int gid, string proId, int type)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutInt ("gid", gid);
		param.PutUtfString ("productid", proId);
		param.PutInt ("type", type);
		Debug.Log ("SendLoadWistoneGetOrder with gid : " + gid + " proId : " + proId + "type : " + type);
		if (client != null) {
			client.SendRequest (CMD_WISTONE_GET_ORDER, param);
		}
	}

	private void LoadWistoneGetOrder (SFSObject param, int cmd)
	{
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("LoadWistoneGetOrder : " + param.GetUtfString ("msg"));
			return;
		}
		Debug.Log ("LoadWistoneGetOrder ");
		int type = param.GetInt ("type");
		Dictionary<string, string> dic = new Dictionary<string, string> ();
		string content;
		dic ["type"] = Convert.ToString (type);
		dic ["prid"] = Convert.ToString (param.GetLong ("prid"));
		content = param.GetUtfString ("orderid");
		dic ["orderid"] = content;
		content = param.GetUtfString ("privatekey");
		dic ["privatekey"] = content;
		//switch (type) {
		//case Const.ANDROID_WISTONE_ALIPAY:
		//	content = param.GetUtfString ("partner");
		//	dic ["partner"] = content;
		//	content = param.GetUtfString ("seller");
		//	dic ["seller"] = content;
		//	content = param.GetUtfString ("publickey");
		//	dic ["publickey"] = content;
		//	content = param.GetUtfString ("notifyurl");
		//	dic ["notifyurl"] = content;

		//	break;
		//case Const.ANDROID_WISTONE_SHENZHOUFU:
		//	content = param.GetUtfString ("version");
		//	dic ["version"] = content;
		//	content = param.GetUtfString ("merid");
		//	dic ["merid"] = content;
		//	content = param.GetUtfString ("returnurl");
		//	dic ["returnurl"] = content;
		//	content = param.GetUtfString ("deskey");
		//	dic ["deskey"] = content;
		//	content = param.GetUtfString ("merusernam");
		//	dic ["merusernam"] = content;
		//	content = param.GetUtfString ("merusermail");
		//	dic ["merusermail"] = content;
		//	content = param.GetUtfString ("privatefield");
		//	dic ["privatefield"] = content;
		//	content = param.GetUtfString ("signstring");
		//	dic ["signstring"] = content;
		//	break;
		//}
		Debug.Log ("LoadWistoneGetOrder got dic :");
		foreach (KeyValuePair<string, string> a in dic) {
			Debug.Log (a.Key + " : " + a.Value);
		}

		if (severRequestWistoneOrderCallBack != null) {
			severRequestWistoneOrderCallBack (dic);
			severRequestWistoneOrderCallBack = null;
		}
	}

	private UJNet.NetClient client;

	//	private static int sceneId = Const.LEVEL_MAP;
	private int lastSceneIndex;
	public bool testMode;

	void Awake ()
	{
		DontDestroyOnLoad (this);

	}

	void Start ()
	{
		client = UJNetManager.Instance.Client;
		if (client != null) {
			client.AddResponseListener (OnResponse);
		} else {
			Debug.Log ("client is null!");
		}
        
	}

	//void OnLevelWasLoaded (int level)
	//{
	//	if (client == null) {
	//		client = UJNetManager.Instance.Client;
	//	}
	//	client.AddResponseListener (OnResponse);
	//}

	//	void OnHttpEnd (int lk)
	//	{
	//		if (this.GetType () == typeof(GameManager)) {
	//			Debug.Log ("OnHttpEnd : " + lk + " " + syncLocker);
	//		}
	//		if (lk == syncLocker) {
	//		}
	//	}

	void Update ()
	{

		if (isFlushPayMent) {
			if (Time.time > nextSyncPaymentQueueTime && !paymentSyncing) {
				paymentSyncing = true;
				//			Debug.Log ("SendLoadPayCompleteList");
				SendLoadPayCompleteList ();
			}
		}

	}


	//-----------------------------订单支付完成查询订单状态----------------------------------------
	public float nextSyncPaymentQueueTime;
	private bool paymentSyncing = false;
	//private int syncLocker = CMD_LOAD_PAY_COMPLETE_LIST;
	public float syncDuration = 10.0f;
	public Action<long, string> severRequestPayment91CallBack;
	private bool isFlushPayMent = false;

	//---------------------------------------------------------------------------------------------

	//-------------------------------wistone order get--------------------------------------
	public Action<Dictionary<string, string>> severRequestWistoneOrderCallBack;
	//---------------------------------------------------------------------------------------------

	public void FlushSamsungOrderData (long proid)
	{

		//		Debug.Log ("run FlushPaymentData proid : " + proid);
		//		if (!payMentList.ContainsValue (proid)) {
		//			Debug.Log ("FlushPaymentData none  have proid: ");	
		//			payMentList.Add (proid, proid);
		//		} else {
		//			Debug.Log ("FlushPaymentData have proid: ");	
		//		}

		isFlushPayMent = true;
	}

	void OnSyncPayMentQueneEnd ()
	{
		paymentSyncing = false;
		nextSyncPaymentQueueTime = Time.time + syncDuration;
		if (payMentList.Count == 0) { // no need update
			Debug.Log ("OnSyncPayMentQueneEnd : " + "payMentList.Count == 0");
			isFlushPayMent = false;
		}
		Debug.Log ("OnSyncPayMentQueneEnd : " + "nextSyncPaymentQueue");
	}

	public void FlushPaymentData (long proid)
	{

		Debug.Log ("run FlushPaymentData proid : " + proid);
		if (!payMentList.ContainsValue (proid)) {
			Debug.Log ("FlushPaymentData none  have proid: ");
			payMentList.Add (proid, proid);
		} else {
			Debug.Log ("FlushPaymentData have proid: ");
		}
		isFlushPayMent = true;
	}

	public void SendLoadPaymentRequest (int gid, string proId)
	{
		SFSObject param = SFSObject.NewInstance ();
		param.PutInt ("gid", gid);
		param.PutUtfString ("productid", proId);
		if (client != null) {
			client.SendRequest (CMD_PAYMENT_REQUEST, param);
		}

		//		SFSObject param = SFSObject.NewInstance ();
		//		param.PutInt ("state", 1);
		//		param.PutLong ("prid", 123123213123123);
		//		param.PutUtfString ("uuid", "12345678912345678912345678912345");
		//		LoadPaymentRequest (param, CMD_PAYMENT_REQUEST);
	}

	private void LoadPaymentRequest (SFSObject param, int cmd)
	{
		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("LoadPaymentRequest : " + param.GetUtfString ("msg"));
			return;
		}
		long prid = param.GetLong ("prid"); // prid
		string orderid = param.GetUtfString ("orderid");
		Debug.Log ("orderid : " + orderid);
		if (severRequestPayment91CallBack != null) {
			severRequestPayment91CallBack (prid, orderid);
		}
	}

	public void SendLoadPayCompleteList ()
	{
		SFSObject param = SFSObject.NewInstance ();
		if (client != null) {
			client.SendRequest (CMD_LOAD_PAY_COMPLETE_LIST, param, false);
			Debug.Log ("client is have!!!");
		} else {
			Debug.Log ("client is null!!!");
		}
	}

	private void LoadPayCompleteList (SFSObject param, int cmd)
	{

		if (param.GetInt ("state") == -1) {
			Debug.LogWarning ("LoadPayCompleteList : " + param.GetUtfString ("msg"));
			return;
		}
		Debug.Log ("LoadPayCompleteList compltete");
		SFSArray arr = (SFSArray)param.GetSFSArray ("data");
		if (arr != null) {
			//for (int i = 0; i < arr.Size(); i++) {
			//	RecoveryItemData itemData = new RecoveryItemData ();
			//	itemData.LoadFromSFSObject (arr.GetSFSObject (i));
			//	foreach (long key in payMentList.Keys) {
			//		if (key == itemData.Prid) {
			//			payMentList.Remove (key);
			//			UserManager.Instance.SendLoadCityResource (false);
			//			Debug.Log ("LoadPayCompleteList : found proid with : " + key);
			//			// add resource
			//			break;
			//		}
			//	}
			//}
		}
		OnSyncPayMentQueneEnd ();
	}

	//-------------------------------360 orderid -------------------------

	public const int CMD_GEN_360_ORDER_ID = 1018;


	//-------------------------------360 order get--------------------------------------
	public Action<long, string, string> severRequest360OrderCallBack;


	//-------------------------------UC orderid -------------------------
	public const int CMD_GEN_UC_ORDER_ID = 1019;
	//-------------------------------UC order get--------------------------------------
	public Action<long, string> severRequestUCOrderCallBack;

    //-------------------------------DangLe orderid ----------------------------------------------------------------------------------------------
    public const int CMD_GEN_DangLe_ORDER_ID = 1020;
    //-------------------------------DangLe order get---------------------------------------------------------------------------------------------
    public Action<long, string> severRequestDangLeOrderCallBack;


	//-------------------------------MI orderid -------------------------
	public const int CMD_GEN_MI_ORDER_ID = 1021;
	//-------------------------------MI order get--------------------------------------
	public Action<long, string> severRequestMIOrderCallBack;


	//-------------------------------TStore orderid ---------------------------
	public const int CMD_GEN_TSTORE_ORDER_ID = 3200;
	//-------------------------------TStore order get -------------------------
	public Action<long, string> severRequestTStoreCallBack;
	//-------------------------------TStore payCheck ---------------------------
	public const int CMD_PAY_TSTORE_CHECK = 3201;
	//-------------------------------TStore check get -------------------------
	public Action<int, string> severRequestTStorePayCheckCallBack;

    //-------------------------------CHINATELECOM orderid ---------------------------
    public const int CMD_GEN_CHINATELECOM_ORDER_ID = 3200003;
    //-------------------------------CHINATELECOM order get -------------------------
    public Action<long, string> severRequestCHINATELECOMCallBack;
    //-------------------------------CHINATELECOM payCheck ---------------------------
    public const int CMD_PAY_CHINATELECOM_CHECK = 3200004;
    //-------------------------------CHINATELECOM check get -------------------------
    public Action<int, string> severRequestCHINATELECOMPayCheckCallBack;

	//-------------------------------OLLEH orderid ---------------------------
	public const int CMD_GEN_OLLEH_ORDER_ID = 3203;
	//-------------------------------TStore order get -------------------------
	public Action<long, string> severRequestOLLEHCallBack;
	//-------------------------------OLLEH payCheck ---------------------------
	public const int CMD_PAY_OLLEH_CHECK = 3204;
	//-------------------------------OLLEH check get -------------------------
	public Action<int, string> severRequestOLLEHPayCheckCallBack;

	//-------------------------------LGU orderid ---------------------------
	public const int CMD_GEN_LGU_ORDER_ID = 3205;
	//-------------------------------LGU  order get -------------------------
	public Action<long, string> severRequestLGUCallBack;
	//-------------------------------LGU  payCheck ---------------------------
	public const int CMD_PAY_LGU_CHECK = 3206;
	//-------------------------------LGU  check get -------------------------
	public Action<int, string> severRequestLGUPayCheckCallBack;
	
}
