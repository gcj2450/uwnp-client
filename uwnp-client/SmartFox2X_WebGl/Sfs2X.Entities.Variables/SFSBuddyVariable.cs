using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The SFSBuddyVariable object represents a SmartFoxServer Buddy Variable entity on the client.
	/// </summary>
	///
	/// <remarks>
	/// The Buddy Variable is a custom value attached to a Buddy object in a Buddy List that gets automatically synchronized between client and server on every change.
	/// <para />
	/// Buddy Variables work with the same principle of the User and Room Variables. The only difference is the logic by which they get propagated to other users.
	/// While Room and User Variables are usually broadcast to all clients in the same Room, Buddy Variables updates are sent to all users who have the owner of the Buddy Variable in their Buddy Lists.
	/// <para />
	/// Buddy Variables are particularly useful to store custom user data that must be "visible" to the buddies only, such as a profile, a score, a status message, etc.
	/// Buddy Variables can be set by means of the <see cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" /> request; they support the data types listed in the <seealso cref="T:Sfs2X.Entities.Variables.VariableType" /> class (also nested). A Buddy Variable can also be <c>null</c>.
	/// <para />
	/// There is also a special convention that allows Buddy Variables to be set as "offline". Offline Buddy Variables are persistent values which are made available to all users
	/// who have the owner in their Buddy Lists, whether that Buddy is online or not. In order to make a Buddy Variable persistent, its name should start with a dollar sign ($).
	/// This conventional character is contained in the <see cref="F:Sfs2X.Entities.Variables.SFSBuddyVariable.OFFLINE_PREFIX" /> constant.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.SFSBuddy" />
	/// <seealso cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" />
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_VARIABLES_UPDATE" />
	public class SFSBuddyVariable : BaseVariable, BuddyVariable, Variable
	{
		/// <summary>
		/// The prefix to be added to a Buddy Variable name to make it persistent.
		/// </summary>
		///
		/// <remarks>
		/// A persistent Buddy Variable is made available to all users who have the owner in their Buddy Lists, whether that Buddy is online or not.
		/// </remarks>
		public static readonly string OFFLINE_PREFIX = "$";

		/// <inheritdoc />
		public bool IsOffline
		{
			get
			{
				return name.StartsWith("$");
			}
		}

		/// <exclude />
		public static BuddyVariable FromSFSArray(ISFSArray sfsa)
		{
			return new SFSBuddyVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), sfsa.GetByte(1));
		}

		/// <summary>
		/// Creates a new SFSBuddyVariable instance.
		/// </summary>
		///
		/// <param name="name">The name of the Buddy Variable.</param>
		/// <param name="val">The value of the Buddy Variable.</param>
		/// <param name="type">The type of the Buddy Variable among those available in the <see cref="T:Sfs2X.Entities.Variables.VariableType" /> class. Usually it is not necessary to pass this parameter, as the type is auto-detected from the value.</param>
		public SFSBuddyVariable(string name, object val, int type)
			: base(name, val, type)
		{
		}

		/// <summary>
		/// See <see cref="M:Sfs2X.Entities.Variables.SFSBuddyVariable.#ctor(System.String,System.Object,System.Int32)" />.
		/// </summary>
		public SFSBuddyVariable(string name, object val)
			: base(name, val)
		{
		}

		/// <summary>
		/// Returns a string that contains the Buddy Variable name, type and value.
		/// </summary>
		///
		/// <returns>
		/// The string representation of the <see cref="T:Sfs2X.Entities.Variables.SFSBuddyVariable" /> object.
		/// </returns>
		public override string ToString()
		{
			string[] obj = new string[7]
			{
				"[BuddyVar: ",
				name,
				", type: ",
				type.ToString(),
				", value: ",
				null,
				null
			};
			object obj2 = val;
			obj[5] = ((obj2 != null) ? obj2.ToString() : null);
			obj[6] = "]";
			return string.Concat(obj);
		}
	}
}
