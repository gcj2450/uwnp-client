using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The SFSRoomVariable object represents a SmartFoxServer Room Variable entity on the client.
	/// </summary>
	///
	/// <remarks>
	/// This is a custom value attached to a Room object that gets automatically synchronized between client and server on every change.
	/// <para />
	/// Room Variables are particularly useful to store custom Room data such as a game status and other Room-level informations.
	/// Room Variables can be set by means of the <see cref="T:Sfs2X.Requests.SetRoomVariablesRequest" /> request; they support the data types listed in the <seealso cref="T:Sfs2X.Entities.Variables.VariableType" /> class (also nested). A Room Variable can also be <c>null</c>.
	/// <para />
	/// Room Variables also support a number of specific flags:
	/// <ul>
	/// 	<li><b>Private</b>: a private Room Variable can only be modified by its creator.</li>
	/// 	<li><b>Persistent</b>: a persistent Room Variable will continue to exist even if its creator has left the Room (but will be deleted when the creator will get disconnected).</li>
	/// 	<li><b>Global</b>: a global Room Variable will fire update events not only to all users in the Room, but also to all users in the Group to which the Room belongs (NOTE: this flag is not available on the client-side because clients are not allowed to create global Room Variables).</li>
	/// </ul>
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.Room" />
	/// <seealso cref="T:Sfs2X.Requests.SetRoomVariablesRequest" />
	public class SFSRoomVariable : BaseVariable, RoomVariable, Variable
	{
		private bool isPrivate;

		private bool isPersistent;

		/// <inheritdoc />
		public bool IsPrivate
		{
			get
			{
				return isPrivate;
			}
			set
			{
				isPrivate = value;
			}
		}

		/// <inheritdoc />
		public bool IsPersistent
		{
			get
			{
				return isPersistent;
			}
			set
			{
				isPersistent = value;
			}
		}

		/// <exclude />
		public static RoomVariable FromSFSArray(ISFSArray sfsa)
		{
			RoomVariable roomVariable = new SFSRoomVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), sfsa.GetByte(1));
			roomVariable.IsPrivate = sfsa.GetBool(3);
			roomVariable.IsPersistent = sfsa.GetBool(4);
			return roomVariable;
		}

		/// <summary>
		/// Creates a new SFSRoomVariable instance.
		/// </summary>
		///
		/// <param name="name">The name of the Room Variable.</param>
		/// <param name="val">The value of the Room Variable.</param>
		/// <param name="type">The type of the Room Variable among those available in the <see cref="T:Sfs2X.Entities.Variables.VariableType" /> class. Usually it is not necessary to pass this parameter, as the type is auto-detected from the value.</param>
		public SFSRoomVariable(string name, object val, int type)
			: base(name, val, type)
		{
			isPrivate = false;
			isPersistent = false;
		}

		/// <summary>
		/// See <see cref="M:Sfs2X.Entities.Variables.SFSRoomVariable.#ctor(System.String,System.Object,System.Int32)" />.
		/// </summary>
		public SFSRoomVariable(string name, object val)
			: base(name, val)
		{
			isPrivate = false;
			isPersistent = false;
		}

		/// <exclude />
		public override ISFSArray ToSFSArray()
		{
			ISFSArray iSFSArray = base.ToSFSArray();
			iSFSArray.AddBool(isPrivate);
			iSFSArray.AddBool(isPersistent);
			return iSFSArray;
		}

		/// <summary>
		/// Returns a string that contains the Room Variable name, type, value and <em>isPrivate</em> flag.
		/// </summary>
		///
		/// <returns>
		/// The string representation of the <see cref="T:Sfs2X.Entities.Variables.SFSRoomVariable" /> object.
		/// </returns>
		public override string ToString()
		{
			string[] obj = new string[9]
			{
				"[RoomVar: ",
				name,
				", type: ",
				type.ToString(),
				", value: ",
				null,
				null,
				null,
				null
			};
			object obj2 = val;
			obj[5] = ((obj2 != null) ? obj2.ToString() : null);
			obj[6] = ", private: ";
			obj[7] = isPrivate.ToString();
			obj[8] = "]";
			return string.Concat(obj);
		}
	}
}
