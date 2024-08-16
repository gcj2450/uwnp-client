using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities
{
	/// <summary>
	/// The SFSBuddy object represents a buddy in the current user's buddies list.
	/// </summary>
	///
	/// <remarks>
	/// A buddy is marked out by the following characteristics.
	/// <ul>
	/// 	<li><b>Nickname</b>: a buddy can have an optional nickname, which differs from the username used during the login process.</li>
	/// 	<li><b>Online/offline state</b>: users can be online or offline as buddies in the Buddy List system. By default a buddy is online every time he joins a Zone, but the user can also switch the state to offline at runtime, and disappear from other user's buddies list. This state is persistent and it is based on a reserved Buddy Variable.</li>
	/// 	<li><b>Custom state</b>: each user can have a typical IM state such as "Available", "Away", "Occupied", etc. State can be selected among the custom ones defined in the Zone configuration, which can be changed or enriched at any time. This state is persistent and it is based on a reserved Buddy Variable.</li>
	/// 	<li><b>Blocked buddy</b>: buddies that are blocked in a user's buddies list won't be able to send messages to that user; also they won't be able to see if the user is online or offline in the Buddy List system.</li>
	/// 	<li><b>Temporary buddy</b>: a temporary buddy is added to the current user's buddies list whenever another user adds him to his own buddies list. In this way users can "see" each other and exchange messages. If the current user doesn't add that temporary buddy to his buddies list voluntarily, that buddy won't be persisted and will be lost upon disconnection.</li>
	/// 	<li><b>Variables</b>: Buddy Variables enable each user to show (and send updates on) specific custom informations to each user to whom he is a buddy. For example one could send realtime updates on his last activity, or post the title of the song he's listening right now, or scores, rankings and whatnot.</li>
	/// </ul>
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.SmartFox.BuddyManager" />
	/// <seealso cref="T:Sfs2X.Entities.Variables.BuddyVariable" />
	public class SFSBuddy : Buddy
	{
		/// <exclude />
		protected string name;

		/// <exclude />
		protected int id;

		/// <exclude />
		protected bool isBlocked;

		/// <exclude />
		protected Dictionary<string, BuddyVariable> variables = new Dictionary<string, BuddyVariable>();

		/// <exclude />
		protected bool isTemp;

		/// <inheritdoc />
		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		/// <inheritdoc />
		public bool IsBlocked
		{
			get
			{
				return isBlocked;
			}
			set
			{
				isBlocked = value;
			}
		}

		/// <inheritdoc />
		public bool IsTemp
		{
			get
			{
				return isTemp;
			}
		}

		/// <inheritdoc />
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <inheritdoc />
		public bool IsOnline
		{
			get
			{
				BuddyVariable variable = GetVariable(ReservedBuddyVariables.BV_ONLINE);
				return (variable == null || variable.GetBoolValue()) && id > -1;
			}
		}

		/// <inheritdoc />
		public string State
		{
			get
			{
				BuddyVariable variable = GetVariable(ReservedBuddyVariables.BV_STATE);
				return (variable == null) ? null : variable.GetStringValue();
			}
		}

		/// <inheritdoc />
		public string NickName
		{
			get
			{
				BuddyVariable variable = GetVariable(ReservedBuddyVariables.BV_NICKNAME);
				return (variable == null) ? null : variable.GetStringValue();
			}
		}

		/// <inheritdoc />
		public List<BuddyVariable> Variables
		{
			get
			{
				lock (variables)
				{
					return new List<BuddyVariable>(variables.Values);
				}
			}
		}

		/// <exclude />
		public static Buddy FromSFSArray(ISFSArray arr)
		{
			Buddy buddy = new SFSBuddy(arr.GetInt(0), arr.GetUtfString(1), arr.GetBool(2), arr.Size() > 4 && arr.GetBool(4));
			ISFSArray sFSArray = arr.GetSFSArray(3);
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				BuddyVariable variable = SFSBuddyVariable.FromSFSArray(sFSArray.GetSFSArray(i));
				buddy.SetVariable(variable);
			}
			return buddy;
		}

		/// <exclude />
		public SFSBuddy(int id, string name)
			: this(id, name, false, false)
		{
		}

		/// <exclude />
		public SFSBuddy(int id, string name, bool isBlocked)
			: this(id, name, isBlocked, false)
		{
		}

		/// <exclude />
		public SFSBuddy(int id, string name, bool isBlocked, bool isTemp)
		{
			this.id = id;
			this.name = name;
			this.isBlocked = isBlocked;
			variables = new Dictionary<string, BuddyVariable>();
			this.isTemp = isTemp;
		}

		/// <inheritdoc />
		public BuddyVariable GetVariable(string varName)
		{
			lock (variables)
			{
				if (variables.ContainsKey(varName))
				{
					return variables[varName];
				}
				return null;
			}
		}

		/// <inheritdoc />
		public List<BuddyVariable> GetOfflineVariables()
		{
			List<BuddyVariable> list = new List<BuddyVariable>();
			lock (variables)
			{
				foreach (BuddyVariable value in variables.Values)
				{
					if (value.Name[0] == Convert.ToChar(SFSBuddyVariable.OFFLINE_PREFIX))
					{
						list.Add(value);
					}
				}
			}
			return list;
		}

		/// <inheritdoc />
		public List<BuddyVariable> GetOnlineVariables()
		{
			List<BuddyVariable> list = new List<BuddyVariable>();
			lock (variables)
			{
				foreach (BuddyVariable value in variables.Values)
				{
					if (value.Name[0] != Convert.ToChar(SFSBuddyVariable.OFFLINE_PREFIX))
					{
						list.Add(value);
					}
				}
			}
			return list;
		}

		/// <inheritdoc />
		public bool ContainsVariable(string varName)
		{
			lock (variables)
			{
				return variables.ContainsKey(varName);
			}
		}

		/// <exclude />
		public void SetVariable(BuddyVariable bVar)
		{
			lock (variables)
			{
				variables[bVar.Name] = bVar;
			}
		}

		/// <exclude />
		public void SetVariables(ICollection<BuddyVariable> variables)
		{
			lock (variables)
			{
				foreach (BuddyVariable variable in variables)
				{
					SetVariable(variable);
				}
			}
		}

		/// <exclude />
		public void RemoveVariable(string varName)
		{
			lock (variables)
			{
				variables.Remove(varName);
			}
		}

		/// <exclude />
		public void ClearVolatileVariables()
		{
			List<string> list = new List<string>();
			lock (variables)
			{
				foreach (BuddyVariable value in variables.Values)
				{
					if (value.Name[0] != Convert.ToChar(SFSBuddyVariable.OFFLINE_PREFIX))
					{
						list.Add(value.Name);
					}
				}
				foreach (string item in list)
				{
					RemoveVariable(item);
				}
			}
		}

		/// <summary>
		/// Returns a string that contains the buddy name and id.
		/// </summary>
		///
		/// <returns>
		/// The string representation of the <see cref="T:Sfs2X.Entities.SFSBuddy" /> object.
		/// </returns>
		public override string ToString()
		{
			return "[Buddy: " + name + ", id: " + id + "]";
		}
	}
}
