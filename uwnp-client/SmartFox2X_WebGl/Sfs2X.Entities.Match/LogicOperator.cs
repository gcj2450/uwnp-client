namespace Sfs2X.Entities.Match
{
	/// <summary>
	/// The LogicOperator class is used to concatenate two matching expressions using the <b>AND</b> or <b>OR</b> logical operator.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
	public class LogicOperator
	{
		/// <summary>
		/// An instance of LogicOperator representing the <em>AND</em> logical operator.
		/// </summary>
		public static readonly LogicOperator AND = new LogicOperator("AND");

		/// <summary>
		/// An instance of LogicOperator representing the <em>OR</em> logical operator.
		/// </summary>
		public static readonly LogicOperator OR = new LogicOperator("OR");

		private string id;

		/// <summary>
		/// Returns the id of the current LogicOperator instance. It can be the string "AND" or "OR".
		/// </summary>
		public string Id
		{
			get
			{
				return id;
			}
		}

		/// <exclude />
		public LogicOperator(string id)
		{
			this.id = id;
		}
	}
}
