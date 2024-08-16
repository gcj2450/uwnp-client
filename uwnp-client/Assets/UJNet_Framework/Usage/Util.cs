using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using UJNet;
using UJNet.Data;

public class Util
{
    private static Dictionary<int, int> cropStateRel = new Dictionary<int, int>();
    private static Dictionary<int, int> resRel = new Dictionary<int, int>();
    public static readonly Dictionary<int, int> armyRel = new Dictionary<int, int>();
    private static Dictionary<int, int> generalAttr = new Dictionary<int, int>();
    private static Dictionary<int, int> generalMinAttr = new Dictionary<int, int>();
    public static Dictionary<int, int> militaryMedal = new Dictionary<int, int>();
    //private static System.Random randomSeek;
    //	public static UIEventHandler LevelLoadCompleteEvent;
    // all icon index
    public const int UV_ICON_PRODUCE_FREE = 0;
    public const int UV_ICON_PRODUCE_GOLD = 1;
    public const int UV_ICON_PRODUCE_FOOD = 2;
    public const int UV_ICON_PRODUCE_WOOD = 3;
    public const int UV_ICON_PRODUCE_IRON = 4;
    public const int UV_ICON_PRODUCE_STONE = 5;
    public const int UV_ICON_FOOD = 6;
    public const int UV_ICON_WOOD = 7;
    public const int UV_ICON_IRON = 8;
    public const int UV_ICON_STONE = 9;
    public const int UV_ICON_GOLD = 10;
    public const int UV_ICON_POPULATION = 11;
    public const int UV_ICON_CASH = 12;
    public const int UV_ICON_TIME = 13;
    public const int UV_ICON_DAOBING = 14;
    public const int UV_ICON_DUNBING = 15;
    public const int UV_ICON_GONGJIAN = 16;
    public const int UV_ICON_QIBING = 17;
    public const int UV_ICON_QISHE = 18;
    public const int UV_ICON_GONGCHENG = 19;
    public const int UV_ICON_NUBING = 20;
    public const int UV_ICON_PAOSHI = 21;
    public const int UV_ICON_PRESTIGE = 22;
    public const int UV_ICON_EXPERIENCE = 23;
    public const int UV_ICON_JIANTA = 24;
    public const int UV_ICON_ATK = 25;
    public const int UV_ICON_ALL_ATK = 26;
    public const int UV_ICON_DEF = 27;
    public const int UV_ICON_ALL_DEF = 28;
    public const int UV_ICON_SPD = 29;
    public const int UV_ICON_ALL_SPD = 30;
    public const int UV_ICON_FLAG = 31;
    public const int UV_ICON_SKILL_POINT = 32;
    public const int UV_ICON_ACTION_POINT = 33;
    public const int UV_ICON_EQUIPMENT = 34;
    public const int UV_ICON_WALL_JIANTA = 35;
    public const int UV_ICON_WALL_PAOSHIJI = 36;
    public const int UV_ICON_WALL_DEFENCE = 37;
    public const int UV_ICON_NO_WAR = 38;
    public const int UV_ICON_NEW_POPULATION = 39;
    public const int UV_ICON_SEL_HONOUR = 40;
    public const int UV_ICON_LEA_HONOUR = 41;

    static Util()
    {
    }

    public static Vector2 GetGeneralAttrByIndex(int attIndex)
    {
        return GetUVByIndex(generalAttr[attIndex]);
    }

    public static Vector2 GetGeneralMinAttrByIndex(int attIndex)
    {
        return GetUVByIndex(generalMinAttr[attIndex]);
    }

    public static Vector2 GetCropStateUVByBuilding(int prototype)
    {
        return GetUVByIndex(cropStateRel[prototype]);
    }

    public static Vector2 GetResUVByResPrototype(int prototype)
    {
        return GetUVByIndex(resRel[prototype]);
    }

    public static Vector2 GetArmyIconByPrototype(int prototype)
    {
        return GetUVByIndex(armyRel[prototype]);
    }

    public static Vector2 GetUVByIndex(int index)
    {
        int x = index % 4;
        int y = index / 4;
        int px = (x * 64) - 1;
        px = Mathf.Clamp(px, 0, px);
        int py = ((y + 1) * 64) - 1;
        return new Vector2(px, py);
    }

    public static Vector2 GetUVByArmyPrototypeId(int armyId)
    {
        int index = armyId - 1;

        int x = index % 5;
        int y = index / 5;
        int px = (x * 90) - 1;
        px = Mathf.Clamp(px, 0, px);
        int py = ((y + 1) * 90) - 1;
        return new Vector2(px, py);
    }

    public static Vector2 GetWaitUVByIndex(int index)
    {
        int x = index % 8;
        int y = index / 8;
        int px = (x * 107) - 1;
        px = Mathf.Clamp(px, 0, px);
        int py = ((y + 1) * 107) - 1;
        return new Vector2(px, py);
    }

    //public static Vector3 GetPosition (string key)
    //{
    //	string val = I18NManager.Instance.GetProp (key);
    //	return ParsePoint (val);
    //}

    public static Vector3 ParsePoint(string str)
    {
        string[] args = str.Split(new char[] { ',' });

        float x = Convert.ToSingle(args[0]);
        float y = Convert.ToSingle(args[1]);
        float z = Convert.ToSingle(args[2]);

        return new Vector3(x, y, z);
    }

    public static DateTime LoadTime;


    public static bool IsInAssignScene(int sceneIndex)
    {
        return Application.loadedLevel == sceneIndex;
    }

    //		public static bool IsInAssignScene (int scName)
    //	{
    //		return scName.Equals (Application.loadedLevelName);
    //	}

    public static long DateTime2Msec(DateTime dt)
    {
        return Decimal.ToInt64(Decimal.Divide(dt.Ticks - new DateTime(1970, 1, 1, 8, 0, 0).Ticks, 10000));
    }

    public static DateTime Msec2DateTime(long t)
    {
        long utcTick = t * 10000 + new DateTime(1970, 1, 1, 0, 0, 0).Ticks;
        return new DateTime(utcTick, DateTimeKind.Utc).ToLocalTime();
    }

    //public static UIButton CreateCmdBtn (GameObject prefab, Transform parent, bool clear)
    //{

    //	Clear (parent, clear);

    //	GameObject go = GameObject.Instantiate (prefab) as GameObject;
    //	go.transform.parent = parent;
    //	go.transform.localPosition = Vector3.zero;
    //	go.transform.localRotation = Quaternion.identity;
    //	go.transform.localScale = Vector3.one;
    //	return go.GetComponent<UIButton> ();
    //}

    public static void Clear(Transform parent, bool destory)
    {
        foreach (Transform child in parent)
        {
            //child.gameObject.SetActiveRecursively(false);
            if (destory)
                GameObject.Destroy(child.gameObject);
        }
    }


    public static void AsChild(Transform obj, Transform parent)
    {
        obj.parent = parent;
        obj.localPosition = Vector3.zero;
        obj.localRotation = Quaternion.identity;
        obj.localScale = Vector3.one;
    }



    // ----------------------util method----------------------------------------
    /// <summary>
    /// Change user interface event call back.
    /// </summary>
    public delegate void ChangeUIEventCallBack(int callBackCMD, object sender, ChangeUIEventArgs args, out bool destoryCallBack, out bool closePanel, ChangeUIEventArgs attr);

    public delegate void UIEventHandler(object sender, ChangeUIEventArgs args);
    // a empty event
    public delegate void EmptyEventHandler();

    /// <summary>
    /// Compares the time. 
    /// </summary>
    /// <returns>
    /// The time.
    /// </returns>
    /// <param name='t1'>
    ///  
    /// </param>
    public static bool CompareTime(long t1)
    {
        //>0 dao ji shi <=0 wan cheng
        return t1 > 0;
    }

    //public static bool QuickJudgeTimeState (long endTime, out long compareTime)
    //{
    //	compareTime = GetCompareTime (endTime);
    //	return CompareTime (compareTime);
    //}

    public static int MoveSelectIndex(int currentIndex, int step, int length, bool isCircle)
    {
        if (length == 0)
        {
            return -1;
        }

        if (isCircle)
        {

            if (currentIndex + step < 0)
            {
                return length - 1;
            }

            if (currentIndex + step > length - 1)
            {
                return 0;
            }

            return currentIndex + step;
        }
        else
        {
            if (currentIndex + step < 0)
            {
                return 0;
            }

            if (currentIndex + step > length - 1)
            {
                return length - 1;
            }
            return currentIndex + step;
        }
    }

    public static void AppendGameObjToGameObj(GameObject gameObj, GameObject parentGameObj)
    {
        Transform childTran = gameObj.transform;
        Transform parentTran = parentGameObj.transform;
        AppendTransformToTransform(childTran, parentTran);
    }

    public static Dictionary<int, GameObject> ConvertMap(GameObject[] prefabs)
    {
        return ConvertMap(prefabs, false);
    }

    public static Dictionary<int, GameObject> ConvertMap(GameObject[] prefabs, bool first)
    {
        Dictionary<int, GameObject> map = new Dictionary<int, GameObject>();
        foreach (GameObject fab in prefabs)
        {
            string[] args = fab.name.Split('_');
            string id = args[args.Length - 1];
            if (first)
            {
                id = args[0];
            }
            id = id.Replace("die", "");
            map.Add(Convert.ToInt32(id), fab);
        }
        return map;
    }

    public static Vector3 CloneVector3(Vector3 needClone)
    {
        Vector3 newVector3 = new Vector3(needClone.x, needClone.y, needClone.z);
        return newVector3;
    }

    public static Quaternion CloneQuaternion(Quaternion needClone)
    {
        Quaternion newQ = new Quaternion(needClone.x, needClone.y, needClone.z, needClone.w);
        return newQ;
    }

    public static void AppendTransformToTransform(Transform childTran, Transform parentTran)
    {
        Vector3 childPos = CloneVector3(childTran.localPosition);
        Vector3 childScale = CloneVector3(childTran.localScale);
        //Quaternion rotation = CloneQuaternion(childTran.localRotation);
        childTran.parent = parentTran;
        childTran.localScale = childScale;
        childTran.position = parentTran.position + childPos;
        childTran.Rotate(parentTran.rotation.eulerAngles);
    }

    //public static bool GetRandomBoolean()
    //{
    //    return randomSeek.Next() % 2 == 1;
    //}

    //public static int GetRandomInt(int m, int n)
    //{
    //    return (MoveByte(randomSeek.Next(), 1)) % (n - m) + m;
    //}

    /// <summary>
    ///  simulate java >>>   (c# havn`t >>>)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static int MoveByte(int value, int pos)
    {
        if (value < 0)
        {
            string s = Convert.ToString(value, 2);
            for (int i = 0; i < pos; i++)
            {
                s = "0" + s.Substring(0, 31);
            }
            return Convert.ToInt32(s, 2);
        }
        else
        {
            return value >> pos;
        }
    }


    public static string MD5Hex(string str)
    {
        //		Debug.Log ("md5 before " + str);
        byte[] bytes = Encoding.UTF8.GetBytes(str);

        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("x2"));
        }

        //		Debug.Log ("md5 after " + sb.ToString ());
        return sb.ToString();
    }

    public static string GetMacAddr()
    {
        NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface ni in nis)
        {
            string pa = ni.GetPhysicalAddress().ToString();
            if (!string.IsNullOrEmpty(pa))
                return pa;
        }
        return null;
    }

    public static bool CheckDelegateInstance(Delegate[] dels, Delegate targetDel)
    {
        //		Delegate[] dels = del.GetInvocationList ();
        if (dels == null)
        {
            return false;
        }
        foreach (Delegate eachDel in dels)
        {
            if (eachDel.Equals(targetDel))
            {
                return true;
            }
        }
        return false;
    }

    //public static bool  MakeUpTime (ref long time, int timeDelay)
    //{
    //	long curTime = UserManager.GetCurTime ();
    //	if (time < curTime) {
    //		time = curTime + timeDelay;
    //		return true;
    //	} else {
    //		return false;
    //	}
    //}

    //public static int GetDeviceType ()
    //{
    //	bool ipad = SystemInfo.deviceModel.ToLower ().IndexOf ("ipad") != -1;
    //	return ipad ? Const.IOS_DEVICE_IPAD : Const.IOS_DEVICE_IPHONE;
    //}

    public static string GetHttpUrl(string url, int cmd)
    {
        string reqPath = url;
        if (string.IsNullOrEmpty(reqPath))
        {
            reqPath = (string)ScopeHolder.attr["http_svr_url"];//==============自定义修改=====================
        }
        reqPath += "?cmd=" + cmd;
        reqPath += "&jid=" + ScopeHolder.attr["session_id"];//==============自定义修改=====================
        UnityEngine.Debug.Log(reqPath);
        return reqPath;
    }

    public static string Dic2QueryStr(Dictionary<string, object> paratable)
    {
        return Dic2QueryStr(paratable, true);
    }

    public static string Dic2QueryStr(Dictionary<string, object> paratable, bool escapeURL)
    {
        StringBuilder sb = new StringBuilder();
        if (paratable != null)
        {
            string[] keys = new string[paratable.Keys.Count];
            paratable.Keys.CopyTo(keys, 0);
            Array.Sort(keys);

            for (int i = 0; i < keys.Length; i++)
            {
                //System.UriBuilder ub = new System.UriBuilder();
                object val = null;
                val = paratable.TryGetValue(keys[i], out val) ? val : "";
                if (escapeURL)
                {
                    sb.Append(keys[i]).Append("=").Append(WWW.EscapeURL(Convert.ToString(val)));
                }
                else
                {
                    sb.Append(keys[i]).Append("=").Append(Convert.ToString(val));
                }

                if (i != keys.Length - 1)
                    sb.Append("&");
            }
        }
        return sb.ToString();
    }

    public static void ReadDictionary(string text, Dictionary<string, string> props)
    {
        string[] lines = text.Split(new char[] { '\n' });
        foreach (string line in lines)
        {
            int index = line.IndexOf('=');
            if (index != -1)
            {
                string key = line.Substring(0, index);
                string val = line.Substring(index + 1);
                try
                {
                    props.Add(key.Trim(), val.Trim());
                }
                catch (ArgumentException ex)
                {
                    Debug.Log("key : " + key + " ex : " + ex.ToString());
                }
            }
        }
    }

    public static Dictionary<string, string> Str2Dic(string str)
    {
        Debug.Log("// " + str);
        Dictionary<string, string> dic = new Dictionary<string, string>();
        string[] list = str.Split(new char[] { '&' });
        foreach (string s in list)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] kv = s.Split(new char[] { '=' });
                if (kv.Length > 1)
                {
                    dic[kv[0]] = kv[1];
                }
            }
        }

        foreach (string xx in dic.Keys)
        {
            Debug.Log("== " + xx + "/" + dic[xx]);
        }
        return dic;
    }

    //public static UJObject GetClientInfo ()
    //{
    //	UJObject model = UJObject.NewInstance ();
    //	// get macaddr
    //	object macid;
    //	macid = ScopeHolder.attr.TryGetValue (Const.MACADDR, out macid) ? macid : "";
    //	model.PutUtfString ("macid", macid.ToString ());
    //	// get udid
    //	object udid;
    //	udid = ScopeHolder.attr.TryGetValue (Const.UDID, out udid) ? udid : "";
    //	model.PutUtfString ("udid", udid.ToString ());
    //	// client info
    //	model.PutUtfString ("model", SystemInfo.deviceModel);
    //	model.PutUtfString ("process", SystemInfo.processorType);
    //	model.PutUtfString ("system", SystemInfo.operatingSystem);
    //	model.PutUtfString ("grname", SystemInfo.graphicsDeviceName);
    //	model.PutInt ("memory", SystemInfo.systemMemorySize);
    //	model.PutInt ("grmemory", SystemInfo.graphicsMemorySize);
    //	model.PutUtfString ("appver", (string)ScopeHolder.attr [Const.APP_VERSION]);
    //	model.PutUtfString ("appname", (string)ScopeHolder.attr [Const.APP_NAME]);

    //	Debug.Log ("client info:\n " + model.Dump ());
    //	return model;
    //}

}