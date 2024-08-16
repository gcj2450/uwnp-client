using UnityEngine;
using System;
using System.Collections;
using UJNet;
using UJNet.Data;

public class TimeManager : MonoBehaviour {

	private static TimeManager instance;
	public static TimeManager Instance {
		get {
			return instance;
		}
	}
		
	public float period = 3.0f;
	
	private float lastRequestTime = float.MaxValue;
	private float timeBeforeSync = 0;
	private bool synchronized = false;
		
	private double lastServerTime = 0;
	private double lastLocalTime = 0;
	
	private bool running = false;
	
	public double averagePing = 0;
	public int pingCount = 0;
	
	private readonly int averagePingCount = 10;
	public double[] pingValues;
	public int pingValueIndex;

	private NetClient client;

	void Awake() {
		instance = this;
	}
	
	void OnDestroy()
	{
		instance = null;
	}
	
	void Start()
	{
		client = UJNetManager.Instance.Client;
		client.AddResponseListener (OnResponse);
	}
	
	public void Init() {
		pingValues = new double[averagePingCount];
		pingCount = 0;
		pingValueIndex = 0;
		running = true;
	}
	int i = 0;	
	public void Synchronize(double timeValue) {
		//Debug.Log("TS ED::" + Time.time * 1000 + "/" +  Decimal.ToInt64(Decimal.Divide(DateTime.Now.Ticks - new DateTime(1970, 1, 1, 8, 0, 0).Ticks, 10000)));
		// Measure the ping in milliseconds
		double ping = (Time.time - timeBeforeSync)*1000;
		CalculateAveragePing(ping);
		//Debug.Log("//// " + ++i + "/" + ping);
				
		// Take the time passed between server sends response and we get it 
		// as half of the average ping value
		double timePassed = ping / 2.0f;//averagePing / 2.0f;
		lastServerTime = timeValue + timePassed;
		lastLocalTime = Time.time;
		
		synchronized = true;	
		
//		UserManager.curTime = timeValue/1000.0f;
		//Debug.Log("TS END:/" + (Time.time - timeBeforeSync)*1000 + "/" + timeValue+"/"+ this.NetworkTime + "/" + this.AveragePing);
	}
		
	void Update () {
		if (!running) return;
		
		if (lastRequestTime > period) {
			lastRequestTime = 0;
			timeBeforeSync = Time.time;
			this.SyncTime();
			//Debug.Log("TS Start:/" + Time.time * 1000+"/"+  Decimal.ToInt64(Decimal.Divide(DateTime.Now.Ticks - new DateTime(1970, 1, 1, 8, 0, 0).Ticks, 10000)));
		}
		else {
			lastRequestTime += Time.deltaTime;
		}
	}
	
	public double NetworkTime {
		get {
			// Taking server timestamp + time passed locally since the last server time received			
			return (Time.time - lastLocalTime)*1000 + lastServerTime;
		}
	}
			
	public double AveragePing {
		get {
			return averagePing;
		}
	}
	
	
	private void CalculateAveragePing(double ping) {
		pingValues[pingValueIndex] = ping;
		pingValueIndex++;
		if (pingValueIndex >= averagePingCount) pingValueIndex = 0;
		if (pingCount < averagePingCount) pingCount++;
					
		double pingSum = 0;
		for (int i=0; i<pingCount; i++) {
			pingSum += pingValues[i];
		}
		
		averagePing = pingSum / pingCount;
	}
	
		
//	void OnGUI() {
//		int screenW = Screen.width;
//		int screenH = Screen.height;
//		
//		GUI.Label(new Rect(screenW*0.01f, screenH*0.25f, 2000, 20), "//"+ Math.Floor(TimeManager.Instance.AveragePing) + "/" + Util.Msec2DateTime((long)TimeManager.Instance.NetworkTime).ToString("hh:mm:ss:ffff"));
//	}
	
	public void SyncTime ()
	{
        SFSObject param = SFSObject.NewInstance ();
		client.SendRequest (805, param, false);
	}
	
	void OnResponse (int cmd, SFSObject param)
	{
		if (cmd == 805) {
			SyncTime_Intl (param);
		}
	}
	
	void SyncTime_Intl (SFSObject param)
	{
		int state = param.GetInt ("state");
		if (state != 1) {
			Debug.LogWarning ("SyncTime_Intl ST: " + state);
			return;
		}
		
		long time = param.GetLong ("t");
		this.Synchronize (Convert.ToDouble (time));
	}
}
