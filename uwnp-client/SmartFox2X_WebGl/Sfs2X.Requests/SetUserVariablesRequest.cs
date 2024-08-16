using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Sets one or more custom User Variables for the current user.
	/// </summary>
	///
	/// <remarks>
	/// When a User Variable is set, the <see cref="F:Sfs2X.Core.SFSEvent.USER_VARIABLES_UPDATE" /> event is dispatched to all the users in all the Rooms joined by the current user, including himself.
	/// <para />
	/// <b>NOTE</b>: the <see cref="F:Sfs2X.Core.SFSEvent.USER_VARIABLES_UPDATE" /> event is dispatched to users in a specific Room
	/// only if it is configured to allow this event (see the <see cref="P:Sfs2X.Requests.RoomSettings.Permissions">RoomSettings.Permissions</see> parameter).
	/// </remarks>
	///
	/// <example>
	/// The following example sets a number of User Variables and handles the respective update event:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVarsUpdate);
	///
	/// 	// Create some User Variables
	/// 	List&lt;UserVariable&gt; userVars = new List&lt;UserVariable&gt;();
	/// 	userVars.Add( new SFSUserVariable("avatarType", "SwedishCook") );
	/// 	userVars.Add( new SFSUserVariable("country", "Sweden") );
	/// 	userVars.Add( new SFSUserVariable("x", 10) );
	/// 	userVars.Add( new SFSUserVariable("y", 5) );
	///
	/// 	sfs.Send( new SetUserVariablesRequest(userVars) );
	/// }
	///
	/// void OnUserVarsUpdate(BaseEvent evt) {
	/// 	List&lt;String&gt; changedVars = (List&lt;String&gt;)evt.Params["changedVars"];
	/// 	User user = (User)evt.Params["user"];
	///
	/// 	// Check if the user changed his x and y User Variables
	/// 	if (changedVars.Contains("x") || changedVars.Contains("y"))
	/// 	{
	/// 		// Move the character to a new position...
	/// 		UpdateClientPosition(user);
	/// 	}
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.USER_VARIABLES_UPDATE" />
	/// <seealso cref="T:Sfs2X.Entities.Variables.SFSUserVariable" />
	public class SetUserVariablesRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_USER = "u";

		/// <exclude />
		public static readonly string KEY_VAR_LIST = "vl";

		private ICollection<UserVariable> userVariables;

		/// <summary>
		/// Creates a new SetUserVariablesRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="userVariables">A collection of objects representing the User Variables to be set.</param>
		public SetUserVariablesRequest(ICollection<UserVariable> userVariables)
			: base(RequestType.SetUserVariables)
		{
			this.userVariables = userVariables;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (userVariables == null || userVariables.Count == 0)
			{
				list.Add("No variables were specified");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("SetUserVariables request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			foreach (UserVariable userVariable in userVariables)
			{
				iSFSArray.AddSFSArray(userVariable.ToSFSArray());
			}
			sfso.PutSFSArray(KEY_VAR_LIST, iSFSArray);
		}
	}
}
