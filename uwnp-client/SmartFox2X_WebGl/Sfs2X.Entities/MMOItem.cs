using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities
{
	/// <summary>
	/// An MMOItem object represents an active non-player entity inside an MMORoom.
	/// </summary>
	///
	/// <remarks>
	/// MMOItems can be used to represent bonuses, triggers, bullets, etc, or any other non-player entity that will be handled using the MMORoom's rules of visibility.
	/// This means that whenever one or more MMOItems fall within the Area of Interest of a user, their presence will be notified to that user by means of the
	/// <see cref="F:Sfs2X.Core.SFSEvent.PROXIMITY_LIST_UPDATE">PROXIMITY_LIST_UPDATE</see> event.
	/// <para />
	/// MMOItems are identified by a unique ID and can have one or more MMOItem Variables associated to store custom data.
	/// <para />
	/// <b>NOTE:</b> MMOItems can be created in a server side Extension only; client side creation is not allowed.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.MMORoom" />
	/// <seealso cref="T:Sfs2X.Entities.Variables.MMOItemVariable" />
	public class MMOItem : IMMOItem
	{
		private int id;

		private Vec3D aoiEntryPoint;

		private Dictionary<string, IMMOItemVariable> variables = new Dictionary<string, IMMOItemVariable>();

		/// <inheritdoc />
		public int Id
		{
			get
			{
				return id;
			}
		}

		/// <inheritdoc />
		public Vec3D AOIEntryPoint
		{
			get
			{
				return aoiEntryPoint;
			}
			set
			{
				aoiEntryPoint = value;
			}
		}

		/// <exclude />
		public static IMMOItem FromSFSArray(ISFSArray encodedItem)
		{
			IMMOItem iMMOItem = new MMOItem(encodedItem.GetInt(0));
			ISFSArray sFSArray = encodedItem.GetSFSArray(1);
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				iMMOItem.SetVariable(MMOItemVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
			}
			return iMMOItem;
		}

		/// <exclude />
		public MMOItem(int id)
		{
			this.id = id;
		}

		/// <inheritdoc />
		public List<IMMOItemVariable> GetVariables()
		{
			lock (variables)
			{
				return new List<IMMOItemVariable>(variables.Values);
			}
		}

		/// <inheritdoc />
		public IMMOItemVariable GetVariable(string name)
		{
			lock (variables)
			{
				return variables[name];
			}
		}

		/// <exclude />
		public void SetVariable(IMMOItemVariable variable)
		{
			lock (variables)
			{
				if (variable.IsNull())
				{
					variables.Remove(variable.Name);
				}
				else
				{
					variables[variable.Name] = variable;
				}
			}
		}

		/// <exclude />
		public void SetVariables(List<IMMOItemVariable> variables)
		{
			lock (variables)
			{
				foreach (IMMOItemVariable variable in variables)
				{
					SetVariable(variable);
				}
			}
		}

		/// <inheritdoc />
		public bool ContainsVariable(string name)
		{
			lock (variables)
			{
				return variables.ContainsKey(name);
			}
		}
	}
}
