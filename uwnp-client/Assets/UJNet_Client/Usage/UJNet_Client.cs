using UnityEngine;
using System.Collections;
using UJNet;
using UJNet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UWNP;
using Cysharp.Threading.Tasks;
using System.Net.Sockets;
using UnityEngine.UI;

public class UJNet_Client : MonoBehaviour
{
    public GameObject PlayerPrefab;
	public GameObject RemotePlayerPrefab;

    GameObject player;
    Dictionary<long, GameObject> remotePlayers = new Dictionary<long, GameObject>();
    public NetClient client;

    [HideInInspector]
    public long userId = 0;

    public InputField userName, password;
    public Button LoginBtn;
    public Image img;
    void Awake()
    {
        LoginBtn.onClick.AddListener(LoginBtnClick);
    }

    private void LoginBtnClick()
    {

    }


    // Use this for initialization
    void Start()
	{
        Connect();
    }

    private void OnDestroy()
    {
        if(client.Connected)
            client.Disconnect();
    }
    public void Disconnect()
    {
        if (client.Connected)
            client.Disconnect();
    }

    public void Connect()
    {
        client = UJNetManager.Instance.Client;
        client.AddResponseListener(OnResponse);
        client.AddConnectionListener(OnConnection);
        client.AddConnectionLostListener(OnConnectionLost);
        client.AddDebugMessageListener(OnDebugMessage);
        switch (UJNetManager.Instance.NetWorkTransType)
        {
            case NetworkTransportType.TCP:
                client.Connect("127.0.0.1", 32211);
                break;
            case NetworkTransportType.UDP:
                break;
            case NetworkTransportType.WebSocket:
                client.Connect("127.0.0.1", 9933);
                break;
            default:
                break;
        }
    }

    void SendSocket()
    {
        userId = UnityEngine.Random.Range(0, 100000);
        SFSObject param = SFSObject.NewInstance();
        param.PutLong("id", userId);
        param.PutUtfString("userName", userId.ToString());
        param.PutUtfString("pwd", "654321");
        client.SendViaSocket(800, param);
    }

    public void SendHttpRequest()
    {
        client = UJNetManager.Instance.Client;
        client.AddResponseListener(OnResponse);
        SFSObject param = SFSObject.NewInstance();
        param.PutUtfString("gcj", "gcj2450is my user name");
        client.SendRequest(3100, param, false);
    }

    private void OnDebugMessage(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    private void OnConnectionLost()
    {
        UnityEngine.Debug.Log("OnConnectionLost");
        img.gameObject.SetActive(false);
    }

    private void OnConnection(bool success, string error)
    {
        UnityEngine.Debug.Log("OnConnection: " + success);
        //SendSocket();
        if (success)
        {
            img.gameObject.SetActive(true);
        }
       RequestAsync().Forget();
    }

    private async UniTaskVoid RequestAsync()
    {
        // On
        client.On("TestController.testOn", (Package pack) =>
        {
            Debug.Log("%%%%%%%%%%%%");
            TestPush info = MessageProtocol.DecodeInfo<TestPush>(pack.buff);
            Debug.Log("push teston :" + info.info);
            //img.gameObject.SetActive(false);
        });

        //请求/响应
        TestRq testRq = new TestRq();
        testRq.packageType = 1024;
        Message<TestRp> a = await client.RequestAsync<TestRq, TestRp>("TestController.testA", testRq);
        if (a.err > 0)
        {
            Debug.LogWarning("err:" + a.err);
            Debug.LogWarning("err msg:" + a.errMsg);
        }
        else
        {
            Debug.Log("aaaaa:" + a.info.packageType);
        }

        //请求/响应
        testRq.packageType = 1985;
        Message<TestRp2> a3 = await client.RequestAsync<TestRq, TestRp2>("TestController.testC", testRq, "custom1");
        if (a3.err > 0)
        {
            Debug.LogWarning("err:" + a3.err);
            Debug.LogWarning("err msg:" + a3.errMsg);
        }
        else
        {
            Debug.Log("a:" + a3.info.info);
        }

        //通知
        TestNotify testRq2 = new TestNotify() { name = "小叮当" };
        client.Notify("TestController.testB", testRq2);
    }

    private void OnResponse(int cmd, SFSObject ujObj)
    {
        if (cmd == 800)
        {
            string id = ujObj.GetUtfString("id");
            Debug.Log("AAAAAAAAAAA: " + id);
        }
        //UnityEngine.Debug.Log("OnResponse " + cmd);
        if (cmd == 801)
        {
			RecvTransform_Itnl (ujObj);
		}
        else if (cmd==802)
        {
            ReceiveUserInfo(ujObj);
        }
        else if (cmd==803)
        {
            long id= ujObj.GetLong("id");
            int playercnt = ujObj.GetInt("playercnt");
            if (id == userId)
            {
                //生成Player
                player = GameObject.Instantiate(PlayerPrefab);
                player.GetComponent<TransformSender>().cntest = this;
            }
            else
            {
				Debug.Log("RemotePlayer: " + userId);
                //接收位置同步的地方会自动生成玩家，这里就不需要了
                //GameObject go = GameObject.Instantiate(RemotePlayerPrefab);
                //go.GetComponent<RemotePlayer>().userId = userId;
                //remotePlayers.Add(userId,go);
            }
        }
        else if (cmd==804)
        {
            SFSArray uJArray= (SFSArray)ujObj.GetSFSArray("players");
            Debug.Log("FFFFFFFF: " + uJArray.Size());
        }
        else
        {
            Debug.Log("cmd = " + cmd);
        }
    }

    private void ReceiveUserInfo(SFSObject ujObj)
    {
        Debug.Log("HHBBB userName: " + ujObj.GetInt("id") + "__" + ujObj.GetUtfString("userName") + "__" + ujObj.GetUtfString("pwd"));
    }

    //发送位置同步数据
	public void SendTransform (long _userId, NetTransform trans)
	{
        //		if (!isconnected)
        //			return;
        //		Debug.Log ("SendTransform");
        SFSObject param = SFSObject.NewInstance ();
        param.PutLong("id", _userId);
		//		param.PutLong ("bid", battle.id);
		param.PutSFSObject ("transform", trans.ToUJObject ());
		client.SendViaSocket (801, param);
	}

	void RecvTransform_Itnl (SFSObject param)
	{

        //		int state = param.GetInt ("state");
        //		if (state != 1) {
        //			Debug.LogWarning ("RecvTransform_Itnl ST: " + state);
        //			return;
        //		}
		//		Debug.Log ("RecvTransform_Itnl");
        		long _userId = param.GetLong ("id");
        SFSObject trans =(SFSObject) param.GetSFSObject ("transform");


		NetTransform netTran = NetTransform.FromUJObject (trans);

        if (_userId == userId)
            return;
        else
        {
            if (remotePlayers.ContainsKey(_userId))
            {
                remotePlayers[_userId].GetComponent<TransformReceiver>().Receive(netTran);
            }
            else
            {
                //这里会在没有远程玩家的时候生成RemotePlayer
                GameObject go = GameObject.Instantiate(RemotePlayerPrefab);
                go.GetComponent<RemotePlayer>().userId = _userId;
                remotePlayers.Add(_userId, go);
                go.GetComponent<TransformReceiver>().Receive(netTran);
            }
        }

	}
}
