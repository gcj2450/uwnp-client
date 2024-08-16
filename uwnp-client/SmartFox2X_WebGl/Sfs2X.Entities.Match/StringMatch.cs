namespace Sfs2X.Entities.Match
{
	/// <summary>
	/// The StringMatch class is used in matching expressions to check string conditions.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
	public class StringMatch : IMatcher
	{
		private static readonly int TYPE_ID = 2;

		/// <summary>
		/// An instance of StringMatch representing the following condition: <em>string1 == string2</em>.
		/// </summary>
		public static readonly StringMatch EQUALS = new StringMatch("==");

		/// <summary>
		/// An instance of StringMatch representing the following condition: <em>string1 != string2</em>.
		/// </summary>
		public static readonly StringMatch NOT_EQUALS = new StringMatch("!=");

		/// <summary>
		/// An instance of StringMatch representing the following condition: <em>string1.indexOf(string2) != -1</em>.
		/// </summary>
		public static readonly StringMatch CONTAINS = new StringMatch("contains");

		/// <summary>
		/// An instance of StringMatch representing the following condition: <em>string1</em> starts with characters contained in <em>string2</em>.
		/// </summary>
		public static readonly StringMatch STARTS_WITH = new StringMatch("startsWith");

		/// <summary>
		/// An instance of StringMatch representing the following condition: <em>string1</em> ends with characters contained in <em>string2</em>.
		/// </summary>
		public static readonly StringMatch ENDS_WITH = new StringMatch("endsWith");

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
		public StringMatch(string symbol)
		{
			this.symbol = symbol;
		}
	}
}
