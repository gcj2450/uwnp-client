namespace Sfs2X.Entities.Match
{
	/// <summary>
	/// The IMatcher interface defines the properties that an object representing a condition to be used in a matching expression exposes.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
	public interface IMatcher
	{
		/// <summary>
		/// Returns the condition symbol of this matcher.
		/// </summary>
		string Symbol { get; }

		/// <summary>
		/// Returns the type id of this matcher.
		/// </summary>
		int Type { get; }
	}
}
