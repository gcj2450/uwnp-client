using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The BaseVariable object is the base class for all SmartFoxServer Variable entities on the client.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Entities.Variables.SFSUserVariable" />
	/// <seealso cref="T:Sfs2X.Entities.Variables.SFSRoomVariable" />
	/// <seealso cref="T:Sfs2X.Entities.Variables.SFSBuddyVariable" />
	/// <seealso cref="T:Sfs2X.Entities.Variables.MMOItemVariable" />
	public abstract class BaseVariable : Variable
	{
		/// <exclude />
		protected string name;

		/// <exclude />
		protected VariableType type;

		/// <exclude />
		protected object val;

		/// <inheritdoc />
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <inheritdoc />
		public VariableType Type
		{
			get
			{
				return type;
			}
		}

		/// <inheritdoc />
		public object Value
		{
			get
			{
				return val;
			}
		}

		/// <summary>
		/// Creates a new BaseVariable instance.
		/// </summary>
		///
		/// <param name="name">The name of the Variable.</param>
		/// <param name="val">The value of the Variable.</param>
		/// <param name="type">The type of the Variable among those available in the <see cref="T:Sfs2X.Entities.Variables.VariableType" /> class. Usually it is not necessary to pass this parameter, as the type is auto-detected from the value.</param>
		///
		/// <exclude />
		public BaseVariable(string name, object val, int type)
		{
			this.name = name;
			if (type > -1)
			{
				this.val = val;
				this.type = (VariableType)type;
			}
			else
			{
				SetValue(val);
			}
		}

		/// <summary>
		/// See <see cref="M:Sfs2X.Entities.Variables.BaseVariable.#ctor(System.String,System.Object,System.Int32)" />.
		/// </summary>
		///
		/// <exclude />
		public BaseVariable(string name, object val)
			: this(name, val, -1)
		{
		}

		/// <inheritdoc />
		public bool GetBoolValue()
		{
			return (bool)val;
		}

		/// <inheritdoc />
		public int GetIntValue()
		{
			return (int)val;
		}

		/// <inheritdoc />
		public double GetDoubleValue()
		{
			return (double)val;
		}

		/// <inheritdoc />
		public string GetStringValue()
		{
			return val as string;
		}

		/// <inheritdoc />
		public ISFSObject GetSFSObjectValue()
		{
			return val as ISFSObject;
		}

		/// <inheritdoc />
		public ISFSArray GetSFSArrayValue()
		{
			return val as ISFSArray;
		}

		/// <inheritdoc />
		public bool IsNull()
		{
			return type == VariableType.NULL;
		}

		/// <exclude />
		public virtual ISFSArray ToSFSArray()
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			iSFSArray.AddUtfString(name);
			iSFSArray.AddByte((byte)type);
			PopulateArrayWithValue(iSFSArray);
			return iSFSArray;
		}

		private void PopulateArrayWithValue(ISFSArray arr)
		{
			switch (type)
			{
			case VariableType.NULL:
				arr.AddNull();
				break;
			case VariableType.BOOL:
				arr.AddBool(GetBoolValue());
				break;
			case VariableType.INT:
				arr.AddInt(GetIntValue());
				break;
			case VariableType.DOUBLE:
				arr.AddDouble(GetDoubleValue());
				break;
			case VariableType.STRING:
				arr.AddUtfString(GetStringValue());
				break;
			case VariableType.OBJECT:
				arr.AddSFSObject(GetSFSObjectValue());
				break;
			case VariableType.ARRAY:
				arr.AddSFSArray(GetSFSArrayValue());
				break;
			}
		}

		private void SetValue(object val)
		{
			this.val = val;
			if (val == null)
			{
				type = VariableType.NULL;
			}
			else if (val is bool)
			{
				type = VariableType.BOOL;
			}
			else if (val is int)
			{
				type = VariableType.INT;
			}
			else if (val is double)
			{
				type = VariableType.DOUBLE;
			}
			else if (val is string)
			{
				type = VariableType.STRING;
			}
			else
			{
				if (val == null)
				{
					return;
				}
				string text = val.GetType().Name;
				if (text == "SFSObject")
				{
					type = VariableType.OBJECT;
					return;
				}
				if (!(text == "SFSArray"))
				{
					throw new SFSError("Unsupport SFS Variable type: " + text);
				}
				type = VariableType.ARRAY;
			}
		}
	}
}
