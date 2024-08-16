using System.Collections.Generic;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities.Managers
{
	/// <summary>
	/// The SFSBuddyManager class is the entity in charge of managing the current user's Buddy List system.
	/// </summary>
	///
	/// <remarks>
	/// This manager keeps track of all the user's buddies, their state and their Buddy Variables. It also provides utility methods to set the user's properties when he is part of the buddies list of other users.
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.SmartFox.BuddyManager" />
	public class SFSBuddyManager : IBuddyManager
	{
		/// <exclude />
		protected Dictionary<string, Buddy> buddiesByName;

		/// <exclude />
		protected Dictionary<string, BuddyVariable> myVariables;

		/// <exclude />
		protected bool myOnlineState;

		/// <exclude />
		protected bool inited;

		private List<string> buddyStates;

		/// <inheritdoc /> 
		public bool Inited
		{
			get
			{
				return inited;
			}
			set
			{
				inited = value;
			}
		}

		/// <inheritdoc />
		public List<Buddy> OfflineBuddies
		{
			get
			{
				List<Buddy> list = new List<Buddy>();
				lock (buddiesByName)
				{
					foreach (Buddy value in buddiesByName.Values)
					{
						if (!value.IsOnline)
						{
							list.Add(value);
						}
					}
				}
				return list;
			}
		}

		/// <inheritdoc />
		public List<Buddy> OnlineBuddies
		{
			get
			{
				List<Buddy> list = new List<Buddy>();
				lock (buddiesByName)
				{
					foreach (Buddy value in buddiesByName.Values)
					{
						if (value.IsOnline)
						{
							list.Add(value);
						}
					}
				}
				return list;
			}
		}

		/// <inheritdoc />
		public List<Buddy> BuddyList
		{
			get
			{
				lock (buddiesByName)
				{
					return new List<Buddy>(buddiesByName.Values);
				}
			}
		}

		/// <inheritdoc />
		public List<BuddyVariable> MyVariables
		{
			get
			{
				lock (myVariables)
				{
					return new List<BuddyVariable>(myVariables.Values);
				}
			}
			set
			{
				lock (myVariables)
				{
					foreach (BuddyVariable item in value)
					{
						SetMyVariable(item);
					}
				}
			}
		}

		/// <inheritdoc />
		public bool MyOnlineState
		{
			get
			{
				if (!inited)
				{
					return false;
				}
				bool result = true;
				BuddyVariable myVariable = GetMyVariable(ReservedBuddyVariables.BV_ONLINE);
				if (myVariable != null)
				{
					result = myVariable.GetBoolValue();
				}
				return result;
			}
			set
			{
				SetMyVariable(new SFSBuddyVariable(ReservedBuddyVariables.BV_ONLINE, value));
			}
		}

		/// <inheritdoc />
		public string MyNickName
		{
			get
			{
				BuddyVariable myVariable = GetMyVariable(ReservedBuddyVariables.BV_NICKNAME);
				return (myVariable != null) ? myVariable.GetStringValue() : null;
			}
			set
			{
				SetMyVariable(new SFSBuddyVariable(ReservedBuddyVariables.BV_NICKNAME, value));
			}
		}

		/// <inheritdoc />
		public string MyState
		{
			get
			{
				BuddyVariable myVariable = GetMyVariable(ReservedBuddyVariables.BV_STATE);
				return (myVariable != null) ? myVariable.GetStringValue() : null;
			}
			set
			{
				SetMyVariable(new SFSBuddyVariable(ReservedBuddyVariables.BV_STATE, value));
			}
		}

		/// <inheritdoc />
		public List<string> BuddyStates
		{
			get
			{
				return buddyStates;
			}
			set
			{
				buddyStates = value;
			}
		}

		/// <exclude />
		public SFSBuddyManager(SmartFox sfs)
		{
			buddiesByName = new Dictionary<string, Buddy>();
			myVariables = new Dictionary<string, BuddyVariable>();
			inited = false;
		}

		/// <exclude />
		public void AddBuddy(Buddy buddy)
		{
			lock (buddiesByName)
			{
				buddiesByName[buddy.Name] = buddy;
			}
		}

		/// <exclude />
		public void ClearAll()
		{
			lock (buddiesByName)
			{
				buddiesByName.Clear();
			}
		}

		/// <exclude />
		public Buddy RemoveBuddyById(int id)
		{
			Buddy buddyById = GetBuddyById(id);
			if (buddyById != null)
			{
				lock (buddiesByName)
				{
					buddiesByName.Remove(buddyById.Name);
				}
			}
			return buddyById;
		}

		/// <exclude />
		public Buddy RemoveBuddyByName(string name)
		{
			Buddy buddyByName = GetBuddyByName(name);
			if (buddyByName != null)
			{
				lock (buddiesByName)
				{
					buddiesByName.Remove(name);
				}
			}
			return buddyByName;
		}

		/// <inheritdoc />
		public Buddy GetBuddyById(int id)
		{
			if (id > -1)
			{
				lock (buddiesByName)
				{
					foreach (Buddy value in buddiesByName.Values)
					{
						if (value.Id == id)
						{
							return value;
						}
					}
				}
			}
			return null;
		}

		/// <inheritdoc />		
		public bool ContainsBuddy(string name)
		{
			lock (buddiesByName)
			{
				return buddiesByName.ContainsKey(name);
			}
		}

		/// <inheritdoc />
		public Buddy GetBuddyByName(string name)
		{
			lock (buddiesByName)
			{
				Buddy value = null;
				buddiesByName.TryGetValue(name, out value);
				return value;
			}
		}

		/// <inheritdoc />
		public Buddy GetBuddyByNickName(string nickName)
		{
			lock (buddiesByName)
			{
				foreach (Buddy value in buddiesByName.Values)
				{
					if (value.NickName == nickName)
					{
						return value;
					}
				}
			}
			return null;
		}

		/// <inheritdoc />
		public BuddyVariable GetMyVariable(string varName)
		{
			lock (myVariables)
			{
				BuddyVariable value = null;
				myVariables.TryGetValue(varName, out value);
				return value;
			}
		}

		/// <exclude />
		public void SetMyVariable(BuddyVariable bVar)
		{
			lock (myVariables)
			{
				myVariables[bVar.Name] = bVar;
			}
		}
	}
}
