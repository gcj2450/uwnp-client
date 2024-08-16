using UnityEngine;
using System.Collections;
using UJNet;
using UJNet.Data;
using System;

public class ChatMsgData : IComparable
{


	//pindao
	private int channel;
	private int msgType;
	private long fromUid;
	private string fromUserName;
	private string content;
	private long toUid;
	private string toUserName;
	private long toLid;//used for league chat  
	private long sendTime;

	public void LoadFromUJObject (SFSObject param)
	{
		channel = param.GetInt ("cha");
		msgType = param.GetInt ("mtype");
		fromUid = param.GetLong ("fcid");
		fromUserName = param.GetUtfString ("fname");
		content = param.GetUtfString ("content");
		sendTime = param.GetLong ("stime");
		toLid = param.GetLong ("tlid");
		toUid = param.GetLong ("tcid");
		toUserName = param.GetUtfString ("tname");
	}
	
	public void ChangeFromTo (ChatMsgData otherData)
	{
		channel = otherData.channel;
		msgType = otherData.msgType;
		content = otherData.content;
		toLid = otherData.toLid;
		sendTime = otherData.sendTime;
		
		fromUid = otherData.toUid;
		fromUserName = otherData.toUserName;
		toUid = otherData.fromUid;
		toUserName = otherData.fromUserName;
		
		
		
		
	}

	public int Channel {
		get {
			return this.channel;
		}
		set {
			channel = value;
		}
	}

	public string Content {
		get {
			return this.content;
		}
		set {
			content = value;
		}
	}

	public long FromUid {
		get {
			return this.fromUid;
		}
		set {
			fromUid = value;
		}
	}

	public string FromUserName {
		get {
			return this.fromUserName;
		}
		set {
			fromUserName = value;
		}
	}

	public int MsgType {
		get {
			return this.msgType;
		}
		set {
			msgType = value;
		}
	}

	public long SendTime {
		get {
			return this.sendTime;
		}
		set {
			sendTime = value;
		}
	}

	public long ToLid {
		get {
			return this.toLid;
		}
		set {
			toLid = value;
		}
	}

	public long ToUid {
		get {
			return this.toUid;
		}
		set {
			toUid = value;
		}
	}

	public string ToUserName {
		get {
			return this.toUserName;
		}
		set {
			toUserName = value;
		}
	}

	public int CompareTo (object obj)
	{
		ChatMsgData otherChat = obj as ChatMsgData;
		long overTime = this.sendTime - otherChat.sendTime;
		if (overTime < 0) {
			return -1;
		} else if (overTime == 0) {
			return 0;
		}
		return 1;
	}
	
}
