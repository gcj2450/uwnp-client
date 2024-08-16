using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The MMOItemVariable object represents a SmartFoxServer MMOItem Variable entity on the client.
	/// </summary>
	///
	/// <remarks>
	/// An MMOItem Variable is a custom value attached to an MMOItem object that gets automatically synchronized between client and server on every change, provided that the MMOItem is inside the Area of Interest of the current user in a MMORoom.
	/// <para />
	/// <b>NOTE:</b> MMOItem Variables behave exactly like User Variables and support the same data types, but they can be created, updated and deleted on the server side only.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.MMOItem" />
	/// <seealso cref="T:Sfs2X.Entities.MMORoom" />
	public class MMOItemVariable : BaseVariable, IMMOItemVariable, Variable
	{
		/// <exclude />
		public static IMMOItemVariable FromSFSArray(ISFSArray sfsa)
		{
			return new MMOItemVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), sfsa.GetByte(1));
		}

		/// <exclude />
		public MMOItemVariable(string name, object val, int type)
			: base(name, val, type)
		{
		}

		/// <exclude />
		public MMOItemVariable(string name, object val)
			: base(name, val)
		{
		}

		/// <summary>
		/// Returns a string that contains the MMOItem Variable name, type and value.
		/// </summary>
		///
		/// <returns>
		/// The string representation of the <see cref="T:Sfs2X.Entities.Variables.MMOItemVariable" /> object.
		/// </returns>
		public override string ToString()
		{
			string[] obj = new string[7]
			{
				"[MMOItemVar: ",
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
