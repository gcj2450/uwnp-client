namespace Sfs2X.Entities.Match
{
	/// <summary>
	/// The NumberMatch class is used in matching expressions to check numeric conditions.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
	public class NumberMatch : IMatcher
	{
		private static readonly int TYPE_ID = 1;

		/// <summary>
		/// An instance of NumberMatch representing the following condition: <em>number1 == number2.</em>
		/// </summary>
		public static readonly NumberMatch EQUALS = new NumberMatch("==");

		/// <summary>
		/// An instance of NumberMatch representing the following condition: <em>number1 != number2.</em>
		/// </summary>
		public static readonly NumberMatch NOT_EQUALS = new NumberMatch("!=");

		/// <summary>
		/// An instance of NumberMatch representing the following condition: <em>number1 &gt; number2.</em>
		/// </summary>
		public static readonly NumberMatch GREATER_THAN = new NumberMatch(">");

		/// <summary>
		/// An instance of NumberMatch representing the following condition: <em>number1 &gt;= number2.</em>
		/// </summary>
		public static readonly NumberMatch GREATER_OR_EQUAL_THAN = new NumberMatch(">=");

		/// <summary>
		/// An instance of NumberMatch representing the following condition: <em>number1 &lt; number2.</em>
		/// </summary>
		public static readonly NumberMatch LESS_THAN = new NumberMatch("<");

		/// <summary>
		/// An instance of NumberMatch representing the following condition: <em>number1 &lt;= number2.</em>
		/// </summary>
		public static readonly NumberMatch LESS_OR_EQUAL_THAN = new NumberMatch("<=");

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
		public NumberMatch(string symbol)
		{
			this.symbol = symbol;
		}
	}
}
