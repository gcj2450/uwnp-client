namespace Sfs2X.Entities.Match
{
	/// <summary>
	/// The BoolMatch class is used in matching expressions to check boolean conditions.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
	public class BoolMatch : IMatcher
	{
		private static readonly int TYPE_ID = 0;

		/// <summary>
		/// An instance of BoolMatch representing the following condition: <em>bool1 == bool2</em>.
		/// </summary>
		public static readonly BoolMatch EQUALS = new BoolMatch("==");

		/// <summary>
		/// An instance of BoolMatch representing the following condition: <em>bool1 != bool2</em>.
		/// </summary>
		public static readonly BoolMatch NOT_EQUALS = new BoolMatch("!=");

		private string symbol;

		/// <inheritdoc />
		public string Symbol
		{
			get
			{
				return symbol;
			}
		}

		/// <inheritdoc />
		public int Type
		{
			get
			{
				return TYPE_ID;
			}
		}

		/// <exclude />
		public BoolMatch(string symbol)
		{
			this.symbol = symbol;
		}
	}
}
