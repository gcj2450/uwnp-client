using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Buddylist
{
	/// <summary>
	/// Sets one or more Buddy Variables for the current user.
	/// </summary>
	///
	/// <remarks>
	/// This operation updates the Buddy object representing the user in all the buddies lists in which the user was added as a buddy.
	/// If the operation is successful, a <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_VARIABLES_UPDATE" /> event is dispatched to all the
	/// owners of those buddies lists and to the user who updated his variables too.
	/// <para />
	/// The Buddy Variables can be persisted, which means that their value will be saved even it the user disconnects and it will be restored when he connects again.
	/// In order to make a variable persistent, put the constant <see cref="F:Sfs2X.Entities.Variables.SFSBuddyVariable.OFFLINE_PREFIX">SFSBuddyVariable.OFFLINE_PREFIX</see> before its name.
	/// Read the SmartFoxServer 2X documentaion about the Buddy List API for more informations.
	/// <para />
	/// This request can be sent if the Buddy List system was previously initialized only (see the <see cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" /> request description) and the current user state in the system is "online".
	/// </remarks>
	///
	/// <example>
	/// The following example sets some Buddy Variables for the current user, one of which is persistent; the example also handles changes made by the user or by his buddies:
	/// <code>
	/// void SomeMethod() {
	/// 	// Add event listener for BuddyVariables
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_VARIABLES_UPDATE, OnBuddyVarsUpdate);
	///
	/// 	// Create two Buddy Variables containing the title and artist of the song I'm listening to
	/// 	BuddyVariable songTitle = new SFSBuddyVariable("songTitle", "Ascension");
	/// 	BuddyVariable songAuthor = new SFSBuddyVariable("songAuthor", "Mike Oldfield");
	///
	/// 	// Create a persistent Buddy Variable containing my mood message
	/// 	BuddyVariable mood = new SFSBuddyVariable(SFSBuddyVariable.OFFLINE_PREFIX + "mood", "I Need SmartFoxServer 2X desperately!");
	///
	/// 	// Set my Buddy Variables
	/// 	List&lt;BuddyVariable&gt; myVars = new List&lt;BuddyVariable&gt;();
	/// 	myVars.Add(songTitle);
	/// 	myVars.Add(songAuthor);
	/// 	myVars.Add(mood);
	/// 	sfs.Send(new SetBuddyVariablesRequest(myVars));
	/// }
	///
	/// void OnBuddyVarsUpdate(BaseEvent evt) {
	/// 	// As the update event is dispatched to me too,
	/// 	// I have to check if I am the one who changed his Buddy Variables
	///
	/// 	Buddy buddy = (Buddy)evt.Params["buddy"]);
	/// 	bool isItMe = (bool)evt.Params["isItMe"];
	/// 	List&lt;string&gt; changedVars = (List&lt;string&gt;)evt.Params["changedVars"];
	///
	/// 	if (isItMe)
	/// 	{
	/// 	    Console.WriteLine("I've updated the following Buddy Variables:");                       // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine("I've updated the following Buddy Variables:");      // UWP
	///
	/// 	    for (int i = 0; i &lt; changedVars.Count; i++)
	/// 	    {
	/// 	        string bVarName = changedVars[i];
	/// 	        Console.WriteLine(bVarName + ": " + sfs.BuddyManager.GetMyVariable(bVarName).Value());                      // .Net / Unity
	/// 	        System.Diagnostics.Debug.WriteLine(bVarName + ": " + sfs.BuddyManager.GetMyVariable(bVarName).Value());     // UWP
	/// 	    }
	/// 	}
	/// 	else
	/// 	{
	/// 	    string buddyName = buddy.Name;
	///
	/// 	    Console.WriteLine("My buddy " + buddyName + " updated the following Buddy Variables:");                     // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine("My buddy " + buddyName + " updated the following Buddy Variables:");    // UWP
	///
	/// 	    for (int i = 0; i &lt; changedVars.Count; i++)
	/// 	    {
	/// 	        var bVarName:String = changedVars[i];
	/// 	        Console.WriteLine(bVarName + ": " + sfs.BuddyManager.GetBuddyByName(buddyName).GetVariable(bVarName).Value());                      // .Net / Unity
	/// 	        System.Diagnostics.Debug.WriteLine(bVarName + ": " + sfs.BuddyManager.GetBuddyByName(buddyName).GetVariable(bVarName).Value());     // UWP
	/// 	    }
	/// 	}
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_VARIABLES_UPDATE" />
	/// <seealso cref="T:Sfs2X.Entities.Variables.SFSBuddyVariable" />
	public class SetBuddyVariablesRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_BUDDY_NAME = "bn";

		/// <exclude />
		public static readonly string KEY_BUDDY_VARS = "bv";

		private List<BuddyVariable> buddyVariables;

		/// <summary>
		/// Creates a new SetBuddyVariablesRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="buddyVariables">A list of objects representing the Buddy Variables to be set.</param>
		public SetBuddyVariablesRequest(List<BuddyVariable> buddyVariables)
			: base(RequestType.SetBuddyVariables)
		{
			this.buddyVariables = buddyVariables;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.BuddyManager.Inited)
			{
				list.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (!sfs.BuddyManager.MyOnlineState)
			{
				list.Add("Can't set buddy variables while off-line");
			}
			if (buddyVariables == null || buddyVariables.Count == 0)
			{
				list.Add("No variables were specified");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("SetBuddyVariables request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			foreach (BuddyVariable buddyVariable in buddyVariables)
			{
				iSFSArray.AddSFSArray(buddyVariable.ToSFSArray());
			}
			sfso.PutSFSArray(KEY_BUDDY_VARS, iSFSArray);
		}
	}
}
