using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The SFSUserVariable object represents a SmartFoxServer User Variable entity on the client.
	/// </summary>
	///
	/// <remarks>
	/// This is a custom value attached to a User object that gets automatically synchronized between client and server on every change.
	/// <para />
	/// User Variables are particularly useful to store custom user data that must be "visible" to the other users, such as a profile, a score, a status message, etc.
	/// User Variables can be set by means of the <see cref="T:Sfs2X.Requests.SetUserVariablesRequest" /> request; they support the data types listed in the <seealso cref="T:Sfs2X.Entities.Variables.VariableType" /> class (also nested). A User Variable can also be <c>null</c>.
	/// <para />
	/// User Variables can be 'private' (starting from SFS2X v2.12 with client API v1.7): this flag allows to limit the visibility of variables to their owner only.
	/// In other words User Variables marked as private are not sent to other users, even if located in the same Room.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.User" />
	/// <seealso cref="T:Sfs2X.Requests.SetUserVariablesRequest" />
	public class SFSUserVariable : BaseVariable, UserVariable, Variable
	{
		private bool isPrivate;

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

		/// <exclude />
		public static UserVariable FromSFSArray(ISFSArray sfsa)
		{
			UserVariable userVariable = new SFSUserVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), sfsa.GetByte(1));
			if (sfsa.Count > 3)
			{
				userVariable.IsPrivate = sfsa.GetBool(3);
			}
			return userVariable;
		}

		/// <summary>
		/// Creates a new private User Variable.
		/// </summary>
		///
		/// <remarks>
		/// Private User Variables are not broadcast to other users: they are only visible on the server side and in the owner's client application.
		/// </remarks>
		///
		/// <param name="name">The name of the User Variable.</param>
		/// <param name="val">The value of the User Variable.</param>
		public static SFSUserVariable newPrivateVariable(string name, object val)
		{
			SFSUserVariable sFSUserVariable = new SFSUserVariable(name, val);
			sFSUserVariable.IsPrivate = true;
			return sFSUserVariable;
		}

		/// <summary>
		/// Creates a new SFSUserVariable instance.
		/// </summary>
		///
		/// <param name="name">The name of the User Variable.</param>
		/// <param name="val">The value of the User Variable.</param>
		/// <param name="type">The type of the User Variable among those available in the <see cref="T:Sfs2X.Entities.Variables.VariableType" /> class. Usually it is not necessary to pass this parameter, as the type is auto-detected from the value.</param>
		public SFSUserVariable(string name, object val, int type)
			: base(name, val, type)
		{
			isPrivate = false;
		}

		/// <summary>
		/// See <see cref="M:Sfs2X.Entities.Variables.SFSUserVariable.#ctor(System.String,System.Object,System.Int32)" />.
		/// </summary>
		public SFSUserVariable(string name, object val)
			: base(name, val)
		{
			isPrivate = false;
		}

		/// <exclude />
		public override ISFSArray ToSFSArray()
		{
			ISFSArray iSFSArray = base.ToSFSArray();
			iSFSArray.AddBool(isPrivate);
			return iSFSArray;
		}

		/// <summary>
		/// Returns a string that contains the User Variable name, type, value and <em>IsPrivate</em> flag.
		/// </summary>
		///
		/// <returns>
		/// The string representation of the <see cref="T:Sfs2X.Entities.Variables.SFSUserVariable" /> object.
		/// </returns>
		public override string ToString()
		{
			string[] obj = new string[9]
			{
				"[UserVar: ",
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
