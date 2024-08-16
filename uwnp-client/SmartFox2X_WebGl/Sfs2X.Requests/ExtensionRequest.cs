using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Sends a command to the server-side Extension attached to the Zone or to a Room.
	/// </summary>
	///
	/// <remarks>
	/// This request is used to send custom commands from the client to a server-side Extension, be it a Zone-level or Room-level Extension.
	/// Viceversa, the <see cref="F:Sfs2X.Core.SFSEvent.EXTENSION_RESPONSE" /> event is used by the server to send Extension commands/responses to the client.
	/// <para />
	/// Read the SmartFoxServer 2X documentation about server-side Extension for more informations.
	/// <para />
	/// The <em>ExtensionRequest</em> request can be sent using the UDP protocol too: this allows sending fast stream of packets to the server in real-time type games,
	/// typically for position/transformation updates, etc.
	/// </remarks>
	///
	/// <example>
	/// The following example sends a command to the Zone Extension; it also handles responses coming from the Extension by implementing the <see cref="F:Sfs2X.Core.SFSEvent.EXTENSION_RESPONSE" /> event listener (the same command name is used in both the request and the response):
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.addEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
	///
	/// 	// Send two integers to the Zone extension and get their sum in return
	/// 	ISFSObject params = SFSObject.NewInstance();
	/// 	params.PutInt("n1", 26);
	/// 	params.PutInt("n2", 16);
	///
	/// 	sfs.Send( new ExtensionRequest("add", params) );
	/// }
	///
	/// void OnExtensionResponse(BaseEvent evt) {
	/// 	String cmd = (String)evt.Params["cmd"];
	/// 	if (cmd == "add") {
	/// 		ISFSObject responseParams = (SFSObject)evt.Params["params"];
	///
	/// 		// We expect an int parameter called "sum"
	/// 		Console.WriteLine("The sum is: " + responseParams.GetInt("sum"));                       // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine("The sum is: " + responseParams.GetInt("sum"));      // UWP
	/// 	}
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.EXTENSION_RESPONSE" />
	public class ExtensionRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_CMD = "c";

		/// <exclude />
		public static readonly string KEY_PARAMS = "p";

		/// <exclude />
		public static readonly string KEY_ROOM = "r";

		private string extCmd;

		private ISFSObject parameters;

		private Room room;

		private bool useUDP;

		/// <exclude />
		public bool UseUDP
		{
			get
			{
				return useUDP;
			}
		}

		private void Init(string extCmd, ISFSObject parameters, Room room, bool useUDP)
		{
			targetController = 1;
			this.extCmd = extCmd;
			this.parameters = parameters;
			this.room = room;
			this.useUDP = useUDP;
			if (parameters == null)
			{
				parameters = new SFSObject();
			}
		}

		/// <summary>
		/// Creates a new ExtensionRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="extCmd">The name of the command which identifies an action that should be executed by the server-side Extension.</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing custom data to be sent to the Extension (default = <c>null</c>). Can be <c>null</c> if no data needs to be sent.</param>
		/// <param name="room">If <c>null</c>, the specified command is sent to the current Zone server-side Extension; if not <c>null</c>, the command is sent to the server-side Extension attached to the passed Room (default = <c>null</c>).</param>
		/// <param name="useUDP">If true, the UDP protocol is used to send the request to the server (default = <c>false</c>).</param>
		public ExtensionRequest(string extCmd, ISFSObject parameters, Room room, bool useUDP)
			: base(RequestType.CallExtension)
		{
			Init(extCmd, parameters, room, useUDP);
		}

		/// <summary>
		/// See <em>ExtensionRequest(string, ISFSObject, Room, bool)</em> constructor.
		/// </summary>
		public ExtensionRequest(string extCmd, ISFSObject parameters, Room room)
			: base(RequestType.CallExtension)
		{
			Init(extCmd, parameters, room, false);
		}

		/// <summary>
		/// See <em>ExtensionRequest(string, ISFSObject, Room, bool)</em> constructor.
		/// </summary>
		public ExtensionRequest(string extCmd, ISFSObject parameters)
			: base(RequestType.CallExtension)
		{
			Init(extCmd, parameters, null, false);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (extCmd == null || extCmd.Length == 0)
			{
				list.Add("Missing extension command");
			}
			if (parameters == null)
			{
				list.Add("Missing extension parameters");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ExtensionCall request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_CMD, extCmd);
			sfso.PutInt(KEY_ROOM, (room == null) ? (-1) : room.Id);
			sfso.PutSFSObject(KEY_PARAMS, parameters);
		}
	}
}
