using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

public class TextUtil
{
	public static void ReadDictionary (string text, Dictionary<string, string> props)
	{
		string[] lines = text.Split (new char[] { '\n' });
		foreach (string line  in lines) {
			int index = line.IndexOf ('=');
			if (index != -1) {
				string key = line.Substring (0, index);
				string val = line.Substring (index + 1);
				try {
					props.Add (key.Trim (), val.Trim ());
				} catch (ArgumentException ex) {
//					Debug.LogWarning ("key : " + key + " ex : " + ex.ToString ());
				}
			}
		}		
	}
}