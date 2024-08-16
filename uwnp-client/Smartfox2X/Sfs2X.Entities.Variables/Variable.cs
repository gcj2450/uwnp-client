using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The Variable interface defines all the default public methods and properties that an object representing a SmartFoxServer Variable exposes.
	/// </summary>
	public interface Variable
	{
		/// <summary>
		/// Indicates the name of this variable.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Indicates the type of this variable.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Entities.Variables.VariableType" />
		VariableType Type { get; }

		/// <summary>
		/// Returns the untyped value of this variable.
		/// </summary>
		object Value { get; }

		/// <summary>
		/// Retrieves the value of a boolean variable.
		/// </summary>
		///
		/// <returns>The variable value as a boolean.</returns>
		bool GetBoolValue();

		/// <summary>
		/// Retrieves the value of an integer variable.
		/// </summary>
		///
		/// <returns>The variable value as an integer.</returns>
		int GetIntValue();

		/// <summary>
		/// Retrieves the value of a double precision variable.
		/// </summary>
		///
		/// <returns>The variable value as a double.</returns>
		double GetDoubleValue();

		/// <summary>
		/// Retrieves the value of a string variable.
		/// </summary>
		///
		/// <returns>The variable value as a string.</returns>
		string GetStringValue();

		/// <summary>
		/// Retrieves the value of a SFSObject variable.
		/// </summary>
		///
		/// <returns>The variable value as an object implementing the <see cref="T:Sfs2X.Entities.Data.ISFSObject" /> interface.</returns>
		///
		/// <seealso cref="T:Sfs2X.Entities.Data.SFSObject" />
		ISFSObject GetSFSObjectValue();

		/// <summary>
		/// Retrieves the value of a SFSArray variable.
		/// </summary>
		///
		/// <returns>The variable value as an object implementing the <see cref="T:Sfs2X.Entities.Data.ISFSArray" /> interface.</returns>
		///
		/// <seealso cref="T:Sfs2X.Entities.Data.SFSArray" />
		ISFSArray GetSFSArrayValue();

		/// <summary>
		/// Indicates if the variable is <c>null</c>.
		/// </summary>
		///
		/// <returns><c>true</c> if the variable has a <c>null</c> value.</returns>
		bool IsNull();

		/// <exclude />
		ISFSArray ToSFSArray();
	}
}
