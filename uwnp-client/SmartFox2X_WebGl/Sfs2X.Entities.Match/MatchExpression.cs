using System;
using System.Text;
using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Match
{
	/// <summary>
	/// The MatchExpression class represents a matching expression used to compare custom variables or predefined properties when searching for users or Rooms.
	/// </summary>
	///
	/// <remarks>
	/// The matching expressions are built like "if" statements in any common programming language. They work like queries in a database and can be used to search for Rooms
	/// or users using custom criteria: in fact a matching expression can compare predefined properties of the Room and user entities
	/// (see the <see cref="T:Sfs2X.Entities.Match.RoomProperties" /> and <see cref="T:Sfs2X.Entities.Match.UserProperties" /> classes),but also custom Room or User Variables.
	/// <para />
	/// These expressions are easy to create and concatenate, and they can be used for many different filtering operations within the SmartFoxServer 2X framework,
	/// for example to invite players to join a game (see the <see cref="T:Sfs2X.Requests.Game.CreateSFSGameRequest" /> request description),
	/// to look for specific Rooms or users (see the <see cref="T:Sfs2X.Requests.FindRoomsRequest" /> and <see cref="T:Sfs2X.Requests.FindUsersRequest" /> requests descriptions), etc.
	/// <para />
	/// Additionally (see the examples for more informations):
	/// <ul>
	/// <li>any number of expressions can be linked together with the logical <b>AND</b> and <b>OR</b> operators to create complex queries;</li>
	/// <li>searching through nested data structures such as <see cref="T:Sfs2X.Entities.Data.SFSObject" /> and <see cref="T:Sfs2X.Entities.Data.SFSArray" /> can be done via a very simple dot-syntax.</li>
	/// </ul>
	/// </remarks>
	///
	/// <example>
	/// The following example shows how to create a simple matching expression made of two concatenated conditions: it compares the custom "rank" and "country"
	/// User Variables to the passed values. This expression could be used during the creation of a Game Room, to filter the users that the server should take
	/// into account when sending the invitations to join the game (only italian users with a ranking greater than 5 - whatever this number means to our game):
	/// <code>
	/// MatchExpression exp = new MatchExpression('rank', NumberMatch.GREATER_THAN, 5).And('country', StringMatch.EQUALS, 'Italy');
	/// </code>
	///
	/// The following example creates a matching expression made of three concatenated conditions which compare two predefined Room properties and the custom "isGameStarted"
	/// Room Variable to the passed values; this expression could be used to retrieve all the Game Rooms still waiting for players to join them:
	/// <code>
	/// MatchExpression exp = new MatchExpression(RoomProperties.IS_GAME, BoolMatch.EQUALS, true)
	///                                      .And(RoomProperties.HAS_FREE_PLAYER_SLOTS, BoolMatch.EQUALS, true)
	///                                      .And("isGameStarted", BoolMatch.EQUALS, false);
	/// </code>
	///
	/// The following example creates a matching expression which compares a nested property in a complex data structure; an SFSObject called "avatarData" (could be a User Variable for example)
	/// contains the "shield" object (a nested SFSObject) which in turn contains, among others, the "inUse" property which could be used to retrieve all user
	/// whose avatars are currently equipped with a shield:
	/// <code>
	/// MatchExpression exp = new MatchExpression("avatarData.shield.inUse", BoolMatch.EQUALS, true);
	/// </code>
	///
	/// The following example is similar to the previous one, but it involves an SFSArray. The "avatarData" object contains the "weapons" SFSArray, from which the expression retrieves
	/// the third element (that .3 means "give me the element at index == 3") that we know being the weapon the user avatar has in his right hand. Again, this element is an SFSObject containing,
	/// among the others, the "name" property which can be compared to the passed string. This example could be used to retrieve all users whose avatars have the Narsil sword in the right hand:
	/// <code>
	/// MatchExpression exp = new MatchExpression("avatarData.weapons.3.name", StringMatch.EQUALS, "Narsil");
	/// </code>
	/// </example>
	///
	/// <seealso cref="T:Sfs2X.Entities.Match.RoomProperties" />
	/// <seealso cref="T:Sfs2X.Entities.Match.UserProperties" />
	/// <seealso cref="T:Sfs2X.Requests.Game.CreateSFSGameRequest" />
	/// <seealso cref="T:Sfs2X.Requests.FindRoomsRequest" />
	/// <seealso cref="T:Sfs2X.Requests.FindUsersRequest" />
	public class MatchExpression
	{
		private string varName;

		private IMatcher condition;

		private object varValue;

		internal LogicOperator logicOp;

		internal MatchExpression parent;

		internal MatchExpression next;

		/// <summary>
		/// Returns the name of the variable or property against which the comparison is made.
		/// </summary>
		///
		/// <remarks>
		/// Depending what the matching expression is used for (searching a <see cref="T:Sfs2X.Entities.User" /> or a <see cref="T:Sfs2X.Entities.Room" />),
		/// this can be the name of a <see cref="T:Sfs2X.Entities.Variables.UserVariable" /> or a <see cref="T:Sfs2X.Entities.Variables.RoomVariable" />,
		/// or it can be one of the constants contained in the <see cref="T:Sfs2X.Entities.Match.UserProperties" /> or <see cref="T:Sfs2X.Entities.Match.RoomProperties" /> classes,
		/// representing some of the predefined properties of the user and Room entities respectively.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Entities.Match.RoomProperties" />
		/// <seealso cref="T:Sfs2X.Entities.Match.UserProperties" />
		/// <seealso cref="T:Sfs2X.Entities.Variables.RoomVariable" />
		/// <seealso cref="T:Sfs2X.Entities.Variables.UserVariable" />
		/// <seealso cref="T:Sfs2X.Entities.User" />
		/// <seealso cref="T:Sfs2X.Entities.Room" />
		public string VarName
		{
			get
			{
				return varName;
			}
		}

		/// <summary>
		/// Returns the matching criteria used during values comparison.
		/// </summary>
		///
		/// <remarks>
		/// Different objects implementing the <seealso cref="T:Sfs2X.Entities.Match.IMatcher" /> interface can be used, depending on the type of the variable or property to check.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Entities.Match.BoolMatch" />
		/// <seealso cref="T:Sfs2X.Entities.Match.NumberMatch" />
		/// <seealso cref="T:Sfs2X.Entities.Match.StringMatch" />
		public IMatcher Condition
		{
			get
			{
				return condition;
			}
		}

		/// <summary>
		/// Returns the value against which the variable or property corresponding to varName is compared.
		/// </summary>
		public object VarValue
		{
			get
			{
				return varValue;
			}
		}

		/// <summary>
		/// In case of concatenated expressions, returns the current logical operator.
		/// </summary>
		///
		/// <remarks>
		/// The default value is <c>null</c>.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Entities.Match.LogicOperator" />
		public LogicOperator LogicOp
		{
			get
			{
				return logicOp;
			}
		}

		/// <exclude />
		internal static MatchExpression ChainedMatchExpression(string varName, IMatcher condition, object value, LogicOperator logicOp, MatchExpression parent)
		{
			MatchExpression matchExpression = new MatchExpression(varName, condition, value);
			matchExpression.logicOp = logicOp;
			matchExpression.parent = parent;
			return matchExpression;
		}

		/// <summary>
		/// Creates a new MatchExpression instance.
		/// </summary>
		///
		/// <param name="varName">Name of the variable or property to match.</param>
		/// <param name="condition">The matching condition.</param>
		/// <param name="varValue">The value to compare against the variable or property during the matching.</param>
		public MatchExpression(string varName, IMatcher condition, object varValue)
		{
			this.varName = varName;
			this.condition = condition;
			this.varValue = varValue;
		}

		/// <summary>
		/// Concatenates the current expression with a new one using the logical <b>AND</b> operator.
		/// </summary>
		///
		/// <param name="varName">The name of the additional variable or property to match.</param>
		/// <param name="condition">The additional matching condition.</param>
		/// <param name="varValue">The value to compare against the additional variable or property during the matching.</param>
		///
		/// <returns>A new MatchExpression resulting from the concatenation of the current expression with a new one generated from the specified parameters.</returns>
		///
		/// <seealso cref="P:Sfs2X.Entities.Match.MatchExpression.VarName" />
		/// <seealso cref="P:Sfs2X.Entities.Match.MatchExpression.Condition" />
		/// <seealso cref="P:Sfs2X.Entities.Match.MatchExpression.VarValue" />
		/// <seealso cref="F:Sfs2X.Entities.Match.LogicOperator.AND" />
		public MatchExpression And(string varName, IMatcher condition, object varValue)
		{
			next = ChainedMatchExpression(varName, condition, varValue, LogicOperator.AND, this);
			return next;
		}

		/// <summary>
		/// Concatenates the current expression with a new one using the logical <b>OR</b> operator.
		/// </summary>
		///
		/// <param name="varName">The name of the additional variable or property to match.</param>
		/// <param name="condition">The additional matching condition.</param>
		/// <param name="varValue">The value to compare against the additional variable or property during the matching.</param>
		///
		/// <returns>A new MatchExpression resulting from the concatenation of the current expression with a new one generated from the specified parameters.</returns>
		///
		/// <seealso cref="P:Sfs2X.Entities.Match.MatchExpression.VarName" />
		/// <seealso cref="P:Sfs2X.Entities.Match.MatchExpression.Condition" />
		/// <seealso cref="P:Sfs2X.Entities.Match.MatchExpression.VarValue" />
		/// <seealso cref="F:Sfs2X.Entities.Match.LogicOperator.OR" />
		public MatchExpression Or(string varName, IMatcher condition, object varValue)
		{
			next = ChainedMatchExpression(varName, condition, varValue, LogicOperator.OR, this);
			return next;
		}

		/// <summary>
		/// Checks if the current matching expression is concatenated to another one through a logical operator.
		/// </summary>
		///
		/// <returns><c>true</c> if the current matching expression is concatenated to another one.</returns>
		///
		/// <seealso cref="T:Sfs2X.Entities.Match.LogicOperator" />
		public bool HasNext()
		{
			return next != null;
		}

		/// <summary>
		/// Get the next matching expression concatenated to the current one.
		/// </summary>
		///
		/// <returns>The next expression concatenated to the current one.</returns>
		public MatchExpression Next()
		{
			return next;
		}

		/// <summary>
		/// Moves the iterator cursor to the first matching expression in the chain.
		/// </summary>
		///
		/// <returns>The MatchExpression object at the top of the chain of matching expressions.</returns>
		public MatchExpression Rewind()
		{
			MatchExpression matchExpression = this;
			while (matchExpression.parent != null)
			{
				matchExpression = matchExpression.parent;
			}
			return matchExpression;
		}

		/// <exclude />
		public string AsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (logicOp != null)
			{
				stringBuilder.Append(" " + logicOp.Id + " ");
			}
			stringBuilder.Append("(");
			string[] obj = new string[5] { varName, " ", condition.Symbol, " ", null };
			object obj2;
			if (!(varValue is string))
			{
				obj2 = varValue;
			}
			else
			{
				object obj3 = varValue;
				obj2 = "'" + ((obj3 != null) ? obj3.ToString() : null) + "'";
			}
			obj[4] = ((obj2 != null) ? obj2.ToString() : null);
			stringBuilder.Append(string.Concat(obj));
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Returns a string representation of the matching expression.
		/// </summary>
		///
		/// <returns>
		/// A string that represents the current <see cref="T:Sfs2X.Entities.Match.MatchExpression" />.
		/// </returns>
		public override string ToString()
		{
			MatchExpression matchExpression = Rewind();
			StringBuilder stringBuilder = new StringBuilder(matchExpression.AsString());
			while (matchExpression.HasNext())
			{
				matchExpression = matchExpression.next;
				stringBuilder.Append(matchExpression.AsString());
			}
			return stringBuilder.ToString();
		}

		/// <exclude />
		public ISFSArray ToSFSArray()
		{
			MatchExpression matchExpression = Rewind();
			ISFSArray iSFSArray = new SFSArray();
			iSFSArray.AddSFSArray(matchExpression.ExpressionAsSFSArray());
			while (matchExpression.HasNext())
			{
				matchExpression = matchExpression.Next();
				iSFSArray.AddSFSArray(matchExpression.ExpressionAsSFSArray());
			}
			return iSFSArray;
		}

		private ISFSArray ExpressionAsSFSArray()
		{
			ISFSArray iSFSArray = new SFSArray();
			if (logicOp != null)
			{
				iSFSArray.AddUtfString(logicOp.Id);
			}
			else
			{
				iSFSArray.AddNull();
			}
			iSFSArray.AddUtfString(varName);
			iSFSArray.AddByte((byte)condition.Type);
			iSFSArray.AddUtfString(condition.Symbol);
			if (condition.Type == 0)
			{
				iSFSArray.AddBool(Convert.ToBoolean(varValue));
			}
			else if (condition.Type == 1)
			{
				iSFSArray.AddDouble(Convert.ToDouble(varValue));
			}
			else
			{
				iSFSArray.AddUtfString(Convert.ToString(varValue));
			}
			return iSFSArray;
		}
	}
}
