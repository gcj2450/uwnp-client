using System.Collections.Generic;

namespace Sfs2X.Core
{
	/// <summary>
	/// This class represents all the events related to the Buddy List system dispatched by the SmartFoxServer 2X C# API.
	/// </summary>
	///
	/// <seealso cref="M:Sfs2X.SmartFox.AddEventListener(System.String,Sfs2X.Core.EventListenerDelegate)" />
	public class SFSBuddyEvent : BaseEvent
	{
		/// <summary>
		/// Dispatched if the Buddy List system is successfully initialized.
		/// </summary>
		///
		/// <remarks>
		/// This event is fired in response to the <see cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" /> request in case the operation is executed successfully.
		/// <para />
		/// After the Buddy List system initialization, the user returns to his previous custom state (if any - see <see cref="P:Sfs2X.Entities.Managers.IBuddyManager.MyState">IBuddyManager.MyState</see> property).
		/// His online/offline state, his nickname and his persistent Buddy Variables are all loaded and broadcast in the system.
		/// In particular, the online state (see <see cref="P:Sfs2X.Entities.Managers.IBuddyManager.MyOnlineState">IBuddyManager.MyOnlineState</see> property) determines if the user will appear online or not to other users who have him in their buddies list.
		/// <para />
		/// The <see cref="P:Sfs2X.Core.BaseEvent.Params" /> object contains the following parameters:
		/// <list type="table">
		///   <listheader>
		///     <term>Parameter</term>
		///     <description>Description</description>
		///   </listheader>
		///   <item>
		///     <term>buddyList</term>
		///     <description>(<b>List&lt;<see cref="T:Sfs2X.Entities.Buddy" />&gt;</b>) A list of objects representing all the buddies in the current user's buddies list.</description>
		///   </item>
		///   <item>
		///     <term>myVariables</term>
		///     <description>(<b>List&lt;<see cref="T:Sfs2X.Entities.Variables.BuddyVariable" />&gt;</b>) The list of all <see cref="T:Sfs2X.Entities.Variables.BuddyVariable" /> objects associated with the current user.</description>
		///   </item>
		/// </list>
		/// </remarks>
		///
		/// <example>See the <see cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" /> example.</example>
		///
		/// <seealso cref="T:Sfs2X.Entities.Buddy" />
		/// <seealso cref="T:Sfs2X.Entities.Variables.BuddyVariable" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" />
		public static readonly string BUDDY_LIST_INIT = "buddyListInit";

		/// <summary>
		/// Dispatched when a buddy is added successfully to the current user's buddies list.
		/// </summary>
		///
		/// <remarks>
		/// This event is fired in response to the <see cref="T:Sfs2X.Requests.Buddylist.AddBuddyRequest" /> request in case the operation is executed successfully.
		/// <para />
		/// The <see cref="P:Sfs2X.Core.BaseEvent.Params" /> object contains the following parameters:
		/// <list type="table">
		///   <listheader>
		///     <term>Parameter</term>
		///     <description>Description</description>
		///   </listheader>
		///   <item>
		///     <term>buddy</term>
		///     <description>(<b><see cref="T:Sfs2X.Entities.Buddy" /></b>) The object corresponding to the buddy that was added.</description>
		///   </item>
		/// </list>
		/// </remarks>
		///
		/// <example>See the <see cref="T:Sfs2X.Requests.Buddylist.AddBuddyRequest" /> example.</example>
		///
		/// <seealso cref="T:Sfs2X.Entities.SFSBuddy" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.AddBuddyRequest" />
		public static readonly string BUDDY_ADD = "buddyAdd";

		/// <summary>
		/// Dispatched when a buddy is removed successfully from the current user's buddies list.
		/// </summary>
		///
		/// <remarks>
		/// This event is fired in response to the <see cref="T:Sfs2X.Requests.Buddylist.RemoveBuddyRequest" /> request in case the operation is executed successfully.
		/// <para />
		/// The <see cref="P:Sfs2X.Core.BaseEvent.Params" /> object contains the following parameters:
		/// <list type="table">
		///   <listheader>
		///     <term>Parameter</term>
		///     <description>Description</description>
		///   </listheader>
		///   <item>
		///     <term>buddy</term>
		///     <description>(<b><see cref="T:Sfs2X.Entities.Buddy" /></b>) The object corresponding to the buddy that was removed.</description>
		///   </item>
		/// </list>
		/// </remarks>
		///
		/// <example>See the <see cref="T:Sfs2X.Requests.Buddylist.RemoveBuddyRequest" /> example.</example>
		///
		/// <seealso cref="T:Sfs2X.Entities.Buddy" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.RemoveBuddyRequest" />
		public static readonly string BUDDY_REMOVE = "buddyRemove";

		/// <summary>
		/// Dispatched when a buddy is blocked or unblocked successfully by the current user.
		/// </summary>
		///
		/// <remarks>
		/// This event is fired in response to the <see cref="T:Sfs2X.Requests.Buddylist.BlockBuddyRequest" /> request in case the operation is executed successfully.
		/// <para />
		/// The <see cref="P:Sfs2X.Core.BaseEvent.Params" /> object contains the following parameters:
		/// <list type="table">
		///   <listheader>
		///     <term>Parameter</term>
		///     <description>Description</description>
		///   </listheader>
		///   <item>
		///     <term>buddy</term>
		///     <description>(<b><see cref="T:Sfs2X.Entities.Buddy" /></b>) The object corresponding to the buddy that was blocked/unblocked.</description>
		///   </item>
		/// </list>
		/// </remarks>
		///
		/// <example>See the <see cref="T:Sfs2X.Requests.Buddylist.BlockBuddyRequest" /> example.</example>
		///
		/// <seealso cref="T:Sfs2X.Entities.Buddy" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.BlockBuddyRequest" />
		public static readonly string BUDDY_BLOCK = "buddyBlock";

		/// <summary>
		/// Dispatched if an error occurs while executing a request related to the Buddy List system.
		/// </summary>
		///
		/// <remarks>
		/// For example, this event is fired in response to the <see cref="T:Sfs2X.Requests.Buddylist.AddBuddyRequest" /> request, the <see cref="T:Sfs2X.Requests.Buddylist.BlockBuddyRequest" />, etc.
		/// <para />
		/// The <see cref="P:Sfs2X.Core.BaseEvent.Params" /> object contains the following parameters:
		/// <list type="table">
		///   <listheader>
		///     <term>Parameter</term>
		///     <description>Description</description>
		///   </listheader>
		///   <item>
		///     <term>errorMessage</term>
		///     <description>(<b>string</b>) The message which describes the error.</description>
		///   </item>
		///   <item>
		///     <term>errorCode</term>
		///     <description>(<b>short</b>) The error code.</description>
		///   </item>
		/// </list>
		/// </remarks>
		///
		/// <example>See the <see cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" /> example.</example>
		public static readonly string BUDDY_ERROR = "buddyError";

		/// <summary>
		/// Dispatched when a buddy in the current user's buddies list changes his online state in the Buddy List system.
		/// </summary>
		///
		/// <remarks>
		/// This event is fired in response to the <see cref="T:Sfs2X.Requests.Buddylist.GoOnlineRequest" /> request to those who have the user as a buddy, but also to the user himself.
		/// As in this case the value of the <em>buddy</em> parameter is <c>null</c> (because the user is not buddy to himself of course),
		/// the <em>isItMe</em> parameter should be used to check if the current user is the one who changed his own online state.
		/// <para />
		/// The <see cref="P:Sfs2X.Core.BaseEvent.Params" /> object contains the following parameters:
		/// <list type="table">
		///   <listheader>
		///     <term>Parameter</term>
		///     <description>Description</description>
		///   </listheader>
		///   <item>
		///     <term>buddy</term>
		///     <description>(<b><see cref="T:Sfs2X.Entities.Buddy" /></b>) The object representing the buddy who changed his own online state. If the <em>isItMe</em> parameter is <c>true</c>, the value of this parameter is <c>null</c> (because a user is not buddy to himself).</description>
		///   </item>
		///   <item>
		///     <term>isItMe</term>
		///     <description>(<b>bool</b>) <c>true</c> if the online state was changed by the current user himself (in this case this event is a sort of state change confirmation).</description>
		///   </item>
		/// </list>
		/// </remarks>
		///
		/// <example>See the <see cref="T:Sfs2X.Requests.Buddylist.GoOnlineRequest" /> example.</example>
		///
		/// <seealso cref="T:Sfs2X.Entities.Buddy" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.GoOnlineRequest" />
		public static readonly string BUDDY_ONLINE_STATE_UPDATE = "buddyOnlineStateChange";

		/// <summary>
		/// Dispatched when a buddy in the current user's buddies list updates one or more Buddy Variables.
		/// </summary>
		///
		/// <remarks>
		/// This event is fired in response to the <see cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" /> request to those who have the user as a buddy, but also to the user himself.
		/// As in this case the value of the <em>buddy</em> parameter is <c>null</c> (because the user is not buddy to himself of course),
		/// the <em>isItMe</em> parameter should be used to check if the current user is the one who updated his own Buddy Variables.
		/// <para />
		/// The <see cref="P:Sfs2X.Core.BaseEvent.Params" /> object contains the following parameters:
		/// <list type="table">
		///   <listheader>
		///     <term>Parameter</term>
		///     <description>Description</description>
		///   </listheader>
		///   <item>
		///     <term>buddy</term>
		///     <description>(<b><see cref="T:Sfs2X.Entities.Buddy" /></b>) The object representing the buddy who updated his own Buddy Variables. If the <em>isItMe</em> parameter is <c>true</c>, the value of this parameter is <c>null</c> (because a user is not buddy to himself).</description>
		///   </item>
		///   <item>
		///     <term>changedVars</term>
		///     <description>(<b>IList&lt;string&gt;</b>) The list of names of the Buddy Variables that were changed (or created for the first time).</description>
		///   </item>
		///   <item>
		///     <term>isItMe</term>
		///     <description>(<b>bool</b>) <c>true</c> if the Buddy Variables were updated by the current user himself (in this case this event is a sort of update confirmation).</description>
		///   </item>
		/// </list>
		/// </remarks>
		///
		/// <example>See the <see cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" /> example.</example>
		///
		/// <seealso cref="T:Sfs2X.Entities.Buddy" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest" />
		public static readonly string BUDDY_VARIABLES_UPDATE = "buddyVariablesUpdate";

		/// <summary>
		/// Dispatched when a message from a buddy is received by the current user.
		/// </summary>
		///
		/// <remarks>
		/// This event is fired in response to the <see cref="T:Sfs2X.Requests.Buddylist.BuddyMessageRequest" /> request.
		/// <para />
		/// The same event is fired by the sender's client too, so that the user is aware that the message was delivered successfully to the recipient, and it can be displayed in the chat area keeping the correct message ordering.
		/// As in this case the value of the buddy parameter is <c>null</c> (because, being the sender, the user is not buddy to himself of course), there is no default way to know who the message was originally sent to.
		/// As this information can be useful in scenarios where the sender is chatting with more than one buddy at the same time in separate windows or tabs (and we need to write his own message in the proper one),
		/// the data parameter can be used to store, for example, the id of the recipient buddy.
		/// <para />
		/// The <see cref="P:Sfs2X.Core.BaseEvent.Params" /> object contains the following parameters:
		/// <list type="table">
		///   <listheader>
		///     <term>Parameter</term>
		///     <description>Description</description>
		///   </listheader>
		///   <item>
		///     <term>buddy</term>
		///     <description>(<b><see cref="T:Sfs2X.Entities.Buddy" /></b>) The object representing the message sender. If the <em>isItMe</em> parameter is <c>true</c>, the value of this parameter is <c>null</c> (because a user is not buddy to himself).</description>
		///   </item>
		///   <item>
		///     <term>isItMe</term>
		///     <description>(<b>bool</b>) <c>true</c> if the message sender is the current user himself (in this case this event is a sort of message delivery confirmation).</description>
		///   </item>
		///   <item>
		///     <term>message</term>
		///     <description>(<b>string</b>) The message text.</description>
		///   </item>
		///   <item>
		///     <term>data</term>
		///     <description>(<b><see cref="T:Sfs2X.Entities.Data.ISFSObject" /></b>) An object containing additional custom parameters (e.g. the message color, an emoticon id, etc).</description>
		///   </item>
		/// </list>
		/// </remarks>
		///
		/// <example>See the <see cref="T:Sfs2X.Requests.Buddylist.BuddyMessageRequest" /> example.</example>
		///
		/// <seealso cref="T:Sfs2X.Entities.SFSBuddy" />
		/// <seealso cref="T:Sfs2X.Requests.Buddylist.BuddyMessageRequest" />
		public static readonly string BUDDY_MESSAGE = "buddyMessage";

		/// <exclude />
		public SFSBuddyEvent(string type)
			: base(type, null)
		{
		}

		/// <exclude />
		public SFSBuddyEvent(string type, Dictionary<string, object> args)
			: base(type, args)
		{
		}
	}
}
