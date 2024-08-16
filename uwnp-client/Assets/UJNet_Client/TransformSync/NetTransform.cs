
using System;
using System.Collections;
using UnityEngine;
using UJNet.Data;

[Serializable]
public class NetTransform
{
	public int id;
	public Vector3 position; 
	public Vector3 angleRotation; 
	public float velocity;
	
	public double timeStamp = 0;
		
	public Vector3 Position {
		get {
			return position;
		}
	}
		
	public Vector3 AngleRotation {
		get {
			return angleRotation;
		}
	}
	
	public Quaternion Rotation {
		get {
			return Quaternion.Euler(angleRotation);
		}
	}
	
	public double TimeStamp {
		get {
			return timeStamp;
		}
		set {
			timeStamp = value;
		}
	}
	
	public SFSObject ToUJObject() {
        SFSObject obj = new SFSObject();
				
		obj.PutInt("id", id);
		obj.PutFloat("vel", this.velocity);
		obj.PutDouble("x", Convert.ToDouble(this.position.x));
		obj.PutDouble("y", Convert.ToDouble(this.position.y));
		obj.PutDouble("z", Convert.ToDouble(this.position.z));
		
		obj.PutDouble("rx", Convert.ToDouble(this.angleRotation.x));
		obj.PutDouble("ry", Convert.ToDouble(this.angleRotation.y));
		obj.PutDouble("rz", Convert.ToDouble(this.angleRotation.z));
		
		obj.PutLong("t", Convert.ToInt64(this.timeStamp));
		return obj;
	}
	
	
	
	public static NetTransform FromUJObject(SFSObject data) {
		int id = data.GetInt("id");
		float vel = data.GetFloat("vel");
		float x = Convert.ToSingle(data.GetDouble("x"));
		float y = Convert.ToSingle(data.GetDouble("y"));
		float z = Convert.ToSingle(data.GetDouble("z"));
		
		float rx = Convert.ToSingle(data.GetDouble("rx"));
		float ry = Convert.ToSingle(data.GetDouble("ry"));
		float rz = Convert.ToSingle(data.GetDouble("rz"));
				
		NetTransform trans = new NetTransform();
		trans.id = id;
		trans.velocity = vel;
		trans.position = new Vector3(x, y, z);
		trans.angleRotation = new Vector3(rx, ry, rz);
		trans.TimeStamp = Convert.ToDouble(data.GetLong("t"));
		
		return trans;
	}
	
	public static NetTransform FromTransform(Transform transform) {
		NetTransform trans = new NetTransform();
				
		trans.position = transform.position;
		trans.angleRotation = transform.localEulerAngles;
				
		return trans;
	}
	
	public void Load(NetTransform ntransform) {
		this.position = ntransform.position;
		this.angleRotation = ntransform.angleRotation;
		this.timeStamp = ntransform.timeStamp;
	}
	
	public void Update(Transform trans) {
		trans.position = this.Position;
		trans.localEulerAngles = this.AngleRotation;
	}
	
	public static NetTransform Clone(NetTransform ntransform) {
		NetTransform trans = new NetTransform();
		trans.Load(ntransform);
		return trans;
	}
	
	public bool IsDifferent(Transform transform, float accuracy) {
		float posDif = Vector3.Distance(this.position, transform.position);
		float angDif = Vector3.Distance(this.AngleRotation, transform.localEulerAngles);
		
		return (posDif>accuracy || angDif > accuracy);
	}
	
}
