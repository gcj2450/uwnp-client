namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The VariableType class contains the costants defining the valid types of User and Room Variables to be passed to their constructors.
	/// </summary>
	public enum VariableType
	{
		/// <summary>
		/// The User/Room Variable is <c>null</c>.
		/// </summary>
		NULL,
		/// <summary>
		/// The type of the User/Room Variable is boolean.
		/// </summary>
		BOOL,
		/// <summary>
		/// The type of the User/Room Variable is integer.
		/// </summary>
		INT,
		/// <summary>
		/// The type of the User/Room Variable is double.
		/// </summary>
		DOUBLE,
		/// <summary>
		/// The type of the User/Room Variable is string.
		/// </summary>
		STRING,
		/// <summary>
		/// The type of the User/Room Variable is <em>SFSObject</em>.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Entities.Data.SFSObject" />
		OBJECT,
		/// <summary>
		/// The type of the User/Room Variable is <em>SFSArray</em>.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Entities.Data.SFSArray" />
		ARRAY
	}
}
