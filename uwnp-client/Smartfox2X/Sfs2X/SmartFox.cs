using System;
using System.Collections.Generic;
using System.Threading;
using Sfs2X.Bitswarm;
using Sfs2X.Core;
using Sfs2X.Core.Sockets;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Managers;
using Sfs2X.Exceptions;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Util;
using Sfs2X.Util.LagMonitor;

namespace Sfs2X
{
    public static class PlatformConfig
    {
#if UNITY_WEBGL &&!UNITY_EDITOR
        public static bool UseWebGL = true;
#else
        public static bool UseWebGL = false;
#endif
    }
    /// <summary>
    /// SmartFox is the main class of the SmartFoxServer 2X API.
    /// </summary>
    /// <remarks>
    /// This class is responsible for connecting the client to a SmartFoxServer instance and for dispatching all asynchronous events. Developers always interact with SmartFoxServer through this class.
    /// <para />
    /// <b>NOTE</b>: in the provided examples, <c>sfs</c> always indicates a SmartFox instance.
    /// <para />
    /// Author: The gotoAndPlay() Team<br />
    /// http://www.smartfoxserver.com
    /// </remarks>
    public class SmartFox : IDispatchable
    {
        private const int DEFAULT_HTTP_PORT = 8080;

        private const char CLIENT_TYPE_SEPARATOR = ':';

        private int majVersion = 1;

        private int minVersion = 8;

        private int subVersion = 0;

        private ISocketClient socketClient;

        private string clientDetails = ".Net";

        private ILagMonitor lagMonitor;

        private UseWebSocket? useWebSocket = null;

        private bool isJoining = false;

        private User mySelf;

        private string sessionToken;

        private Room lastJoinedRoom;

        private Logger log;

        private bool inited = false;

        private bool debug = false;

        private bool threadSafeMode = true;

        private bool isConnecting = false;

        private IUserManager userManager;

        private IRoomManager roomManager;

        private IBuddyManager buddyManager;

        private ConfigData config;

        private string currentZone;

        private bool autoConnectOnConfig = false;

        private string lastHost;

        private EventDispatcher dispatcher;

        private object eventsLocker = new object();

        private Queue<BaseEvent> eventsQueue = new Queue<BaseEvent>();

        private bool udpAvailable = true;

        private string nodeId = null;

        /// <exclude />
        public ISocketClient SocketClient
        {
            get
            {
                return socketClient;
            }
        }

        /// <exclude />
        public Logger Log
        {
            get
            {
                return log;
            }
        }

        /// <exclude />
        public bool IsConnecting
        {
            get
            {
                return isConnecting;
            }
        }

        /// <exclude />
        public ILagMonitor LagMonitor
        {
            get
            {
                return lagMonitor;
            }
        }

        /// <summary>
        /// Indicates whether the client is connected to the server or not.
        /// </summary>
        ///
        /// <example>
        /// The following example checks the connection status:
        /// <code>
        /// Console.WriteLine("Am I connected? " + sfs.IsConnected);                        // .Net / Unity
        /// System.Diagnostics.Debug.WriteLine("Am I connected? " + sfs.IsConnected);       // UWP
        /// </code>
        /// </example>
        public bool IsConnected
        {
            get
            {
                bool result = false;
                if (socketClient != null)
                {
                    result = socketClient.Connected;
                }
                return result;
            }
        }

        /// <summary>
        /// Returns the current version of the SmartFoxServer 2X C# API.
        /// </summary>
        public string Version
        {
            get
            {
                return majVersion + "." + minVersion + "." + subVersion;
            }
        }

        /// <summary>
        /// Returns the HTTP URI that can be used to upload files to SmartFoxServer 2X, using regular HTTP POST.
        /// </summary>
        ///
        /// <remarks>
        /// For more details on how to use this functionality, see the <see href="http://docs2x.smartfoxserver.com/AdvancedTopics/file-uploads">Upload File</see> tutorial.
        /// <para />
        /// <b>NOTE</b>: this property returns <c>null</c> if no API configuration has been set or the current user is not already logged in the server.
        /// </remarks>
        public string HttpUploadURI
        {
            get
            {
                if (config == null || mySelf == null)
                {
                    return null;
                }
                return "http://" + config.Host + ":" + config.HttpPort + "/BlueBox/SFS2XFileUpload?sessHashId=" + sessionToken;
            }
        }

        /// <summary>
        /// Returns the client configuration details.
        /// </summary>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" />
        public ConfigData Config
        {
            get
            {
                return config;
            }
        }

        /// <summary>
        /// Returns the current connection mode after a connection has been successfully established.
        /// </summary>
        ///
        /// <remarks>
        /// Possible values are:
        /// <ul>
        /// <li><b>HTTP</b>: a tunnelled http connection (through the BlueBox) was established between the client and the server</li>
        /// <li><b>Socket</b>: a regular socket connection was established between the client and the server</li>
        /// <li><b>WebSocket</b>: a websocket connection was established between the client and the server (not available under Universal Windows Platform)</li>
        /// <li><b>WebSocketSecure</b>: a secure websocket connection was established between the client and the server (not available under Universal Windows Platform)</li>
        /// </ul>
        /// </remarks>
        ///
        /// <example>
        /// The following example shows the current connection mode:
        /// <code>
        /// Console.WriteLine("Connection mode: " + sfs.ConnectionMode);                        // .Net / Unity
        /// System.Diagnostics.Debug.WriteLine("Connection mode: " + sfs.ConnectionMode);       // Universal Windows Platform
        /// </code>
        /// </example>
        public string ConnectionMode
        {
            get
            {
                return socketClient.ConnectionMode;
            }
        }

        /// <summary>
        /// Returns the current compression threshold.
        /// </summary>
        ///
        /// <remarks>
        /// This value represents the maximum message size (in bytes) before the protocol compression is activated and it is determined by the server configuration.<br />
        /// Compression threshold doesn't apply if websocket connection is used.
        /// </remarks>
        public int CompressionThreshold
        {
            get
            {
                return socketClient.CompressionThreshold;
            }
        }

        /// <summary>
        /// Returns the maximum size of messages allowed by the server.
        /// </summary>
        ///
        /// <remarks>
        /// Any request exceeding this size will not be sent. The value is determined by the server-side configuration.
        /// </remarks>
        public int MaxMessageSize
        {
            get
            {
                return socketClient.MaxMessageSize;
            }
        }

        /// <summary>
        /// Indicates whether the client-server messages debug is enabled or not.
        /// </summary>
        ///
        /// <remarks>
        /// If set to <c>true</c>, detailed debugging informations for all the incoming and outgoing messages are provided.<br />
        /// Debugging can be enabled when instantiating the <em>SmartFox</em> class too.
        /// </remarks>
        public bool Debug
        {
            get
            {
                return debug;
            }
            set
            {
                debug = value;
            }
        }

        /// <summary>
        /// Returns the IP address or domain name of the SmartFoxServer 2X instance to which the client is connected.
        /// </summary>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.Connect(System.String,System.Int32)" />
        /// <seealso cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" />
        public string CurrentIp
        {
            get
            {
                return socketClient.ConnectionHost;
            }
        }

        /// <summary>
        /// Returns the TCP port of the SmartFoxServer 2X instance to which the client is connected.
        /// </summary>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.Connect(System.String,System.Int32)" />
        /// <seealso cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" />
        public int CurrentPort
        {
            get
            {
                return socketClient.ConnectionPort;
            }
        }

        /// <summary>
        /// Returns the Zone currently in use, if the user is already logged in.
        /// </summary>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" />
        /// <seealso cref="T:Sfs2X.Requests.LoginRequest" />
        public string CurrentZone
        {
            get
            {
                return currentZone;
            }
        }

        /// <summary>
        /// Returns the <em>User</em> object representing the client when connected to a SmartFoxServer 2X instance.
        /// </summary>
        ///
        /// <remarks>
        /// This object is generated upon successful login only, so it is <c>null</c> if login was not performed yet.
        /// <para />
        /// <b>NOTE</b>: setting this property manually can disrupt the API functioning.
        /// </remarks>
        ///
        /// <seealso cref="P:Sfs2X.Entities.User.IsItMe" />
        /// <seealso cref="T:Sfs2X.Requests.LoginRequest" />
        public User MySelf
        {
            get
            {
                return mySelf;
            }
            set
            {
                mySelf = value;
            }
        }

        /// <summary>
        /// Returns a reference to the internal <em>Logger</em> instance used by SmartFoxServer 2X.
        /// </summary>
        public Logger Logger
        {
            get
            {
                return log;
            }
        }

        /// <summary>
        /// Returns the object representing the last Room joined by the client, if any.
        /// </summary>
        ///
        /// <remarks>
        /// This property is <c>null</c> if no Room was joined.
        /// <para />
        /// <b>NOTE</b>: setting this property manually can disrupt the API functioning.
        /// Use the <see cref="T:Sfs2X.Requests.JoinRoomRequest" /> request to join a new Room instead.
        /// </remarks>
        ///
        /// <seealso cref="T:Sfs2X.Requests.JoinRoomRequest" />
        /// <seealso cref="P:Sfs2X.SmartFox.JoinedRooms" />
        public Room LastJoinedRoom
        {
            get
            {
                return lastJoinedRoom;
            }
            set
            {
                lastJoinedRoom = value;
            }
        }

        /// <summary>
        /// Returns a list of Room objects representing the Rooms currently joined by the client.
        /// </summary>
        ///
        /// <remarks>
        /// The same list is returned by the <b>IRoomManager.getRoomById()</b> method, accessible through the <see cref="P:Sfs2X.SmartFox.RoomManager" /> getter;
        /// this was replicated on the <em>SmartFox</em> class for handy access due to its usually frequent usage.
        /// </remarks>
        ///
        /// <seealso cref="T:Sfs2X.Requests.JoinRoomRequest" />
        /// <seealso cref="P:Sfs2X.SmartFox.LastJoinedRoom" />
        /// <seealso cref="P:Sfs2X.SmartFox.RoomManager" />
        /// <seealso cref="T:Sfs2X.Entities.Room" />
        public List<Room> JoinedRooms
        {
            get
            {
                return roomManager.GetJoinedRooms();
            }
        }

        /// <summary>
        /// Returns a list of Room objects representing the Rooms currently "watched" by the client.
        /// </summary>
        ///
        /// <remarks>
        /// The list contains all the Rooms that are currently joined and all the Rooms belonging to the Room Groups that have been subscribed.<br />
        /// At login time, the client automatically subscribes all the Room Groups specified in the Zone's <b>Default Room Groups</b> setting.
        /// <para />
        /// The same list is returned by the <b>IRoomManager.getRoomById()</b> method, accessible through the <see cref="P:Sfs2X.SmartFox.RoomManager" /> getter;
        /// this was replicated on the <em>SmartFox</em> class for handy access due to its usually frequent usage.
        /// </remarks>
        ///
        /// <seealso cref="P:Sfs2X.SmartFox.RoomManager" />
        /// <seealso cref="T:Sfs2X.Requests.JoinRoomRequest" />
        /// <seealso cref="T:Sfs2X.Requests.SubscribeRoomGroupRequest" />
        /// <seealso cref="T:Sfs2X.Requests.UnsubscribeRoomGroupRequest" />
        public List<Room> RoomList
        {
            get
            {
                return roomManager.GetRoomList();
            }
        }

        /// <summary>
        /// Returns a reference to the Room Manager.
        /// </summary>
        ///
        /// <remarks>
        /// This manager is used internally by the SmartFoxServer 2X API; the reference returned by this property
        /// gives access to the Rooms list and Groups, allowing interaction with <see cref="T:Sfs2X.Entities.Room" /> objects.
        /// </remarks>
        ///
        /// <seealso cref="T:Sfs2X.Entities.Managers.IRoomManager" />
        public IRoomManager RoomManager
        {
            get
            {
                return roomManager;
            }
        }

        /// <summary>
        /// Returns a reference to the User Manager.
        /// </summary>
        ///
        /// <remarks>
        /// This manager is used internally by the SmartFoxServer 2X API; the reference returned by this property
        /// gives access to the users list, allowing interaction with <see cref="T:Sfs2X.Entities.User" /> objects.
        /// </remarks>
        ///
        /// <seealso cref="T:Sfs2X.Entities.Managers.IUserManager" />
        public IUserManager UserManager
        {
            get
            {
                return userManager;
            }
        }

        /// <summary>
        /// Returns a reference to the Buddy Manager.
        /// </summary>
        ///
        /// <remarks>
        /// This manager is used internally by the SmartFoxServer 2X API; the reference returned by this property
        /// gives access to the buddies list, allowing interaction with <see cref="T:Sfs2X.Entities.Buddy" /> and <see cref="T:Sfs2X.Entities.Variables.BuddyVariable" /> objects and access to user properties in the <b>Buddy List</b> system.
        /// </remarks>
        ///
        /// <seealso cref="T:Sfs2X.Entities.Managers.IBuddyManager" />
        public IBuddyManager BuddyManager
        {
            get
            {
                return buddyManager;
            }
        }

        /// <summary>
        /// Indicates whether the UPD protocol is available or not in the current runtime.
        /// </summary>
        ///
        /// <remarks>
        /// UPD protocol is always available, unless websocket connection is used or http fallback (BlueBox) is activated in case a regular socket connection can't be established.
        /// <para />
        /// Using the UDP protocol in an application requires that a handshake is performed between the client and the server.
        /// By default this is NOT done by the SmartFoxServer 2X API, to avoid allocating resources that might never be used.<br />
        /// In order to activate the UDP support, the initUDP() method must be invoked explicitly.
        /// </remarks>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.InitUDP(System.String,System.Int32)" />
        public bool UdpAvailable
        {
            get
            {
                return udpAvailable;
            }
        }

        /// <summary>
        /// Indicates whether the UDP handshake has been performed successfully or not.
        /// </summary>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.InitUDP(System.String,System.Int32)" />
        public bool UdpInited
        {
            get
            {
                if (socketClient.UdpManager != null)
                {
                    return socketClient.UdpManager.Inited;
                }
                return false;
            }
        }

        /// <exclude />
        public bool IsJoining
        {
            get
            {
                return isJoining;
            }
            set
            {
                isJoining = value;
            }
        }

        /// <summary>
        /// Returns the unique session token of the client.
        /// </summary>
        ///
        /// <remarks>
        /// The session token is a string sent by the server to the client after the initial handshake.
        /// It is required as mean of identification when uploading files to the server.
        /// </remarks>
        ///
        /// <seealso cref="P:Sfs2X.SmartFox.HttpUploadURI" />
        public string SessionToken
        {
            get
            {
                return sessionToken;
            }
        }

        /// <exclude />
        public EventDispatcher Dispatcher
        {
            get
            {
                return dispatcher;
            }
        }

        /// <summary>
        /// Sets the API to run with an event queue that needs to be processed by the client.
        /// </summary>
        ///
        /// <remarks>
        /// <b>IMPORTANT</b>: by default this property is set to <c>true</c>. When developing native applications/games for .Net or Universal Windows Platform, always remember to set this property to <c>false</c> explicitly!
        /// </remarks>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.ProcessEvents" />
        public bool ThreadSafeMode
        {
            get
            {
                return threadSafeMode;
            }
            set
            {
                threadSafeMode = value;
            }
        }

        /// <summary>
        /// <b>[CLUSTER]</b> The identifier of the cluster node which the current SmartFox instance is connected to. If the instance is not connected, a null value is returned;
        /// </summary>
        public string NodeId
        {
            get
            {
                return IsConnected ? nodeId : null;
            }
            set
            {
                nodeId = value;
            }
        }

        /// <summary>
        /// Creates a new SmartFox instance.
        /// </summary>
        ///
        /// <example>
        /// The following example instantiates the <b>SmartFox</b> class without enabling the debug messages:
        /// <code>
        /// SmartFox sfs = new SmartFox();
        /// </code>
        /// </example>
        public SmartFox()
        {
            Initialize(false);
        }

        /// <summary>
        /// Creates a new SmartFox instance.
        /// </summary>
        ///
        /// <param name="debug">If <c>true</c>, the SmartFoxServer API debug messages are logged.</param>
        ///
        /// <example>
        /// The following example instantiates the <b>SmartFox</b> class enabling the debug messages:
        /// <code>
        /// SmartFox sfs = new SmartFox(true);
        /// </code>
        /// </example>
        public SmartFox(bool debug)
        {
            Initialize(debug);
        }

        /// <summary>
        /// Creates a new SmartFox instance enabling websocket communication instead of the default socket communication.
        /// </summary>
        ///
        /// <remarks>
        /// <b>IMPORTANT</b>: this constructor should be used in Unity only and when building for the web only (Unity WEBGL).
        /// In fact it is preferable to use the default socket communication, as websocket connection doesn't support BlueBox and HRC systems.<br />
        /// This constructor has no effects if used in native .Net applications and is not available in the Universal Windows Platform DLL (websocket communication not supported, because redundant).
        /// <para />
        /// Starting from API v1.7.3, websocket communication uses the default SFS2X binary protocol (see <c>UseWebSocket</c> class). Legacy text protocol is still available for backward compatibility with SmartFoxserver 2X versions prior to 2.13.
        /// </remarks>
        ///
        /// <param name="useWebSocket">If set to <c>UseWebSocket.WS_BIN</c>, non-secure websocket communication (WS) is used; if set to <c>UseWebSocket.WSS_BIN</c>, secure websocket communication (WSS) is used.</param>
        ///
        /// <example>
        /// The following example instantiates the <b>SmartFox</b> class enabling websocket communication:
        /// <code>
        /// SmartFox sfs = new SmartFox(UseWebSocket.WS_BIN);
        /// </code>
        /// </example>
        public SmartFox(UseWebSocket useWebSocket)
        {
            this.useWebSocket = useWebSocket;
            Initialize(false);
        }

        /// <summary>
        /// Creates a new SmartFox instance enabling websocket communication instead of the default socket communication.
        /// </summary>
        ///
        /// <remarks>
        /// <b>IMPORTANT</b>: this constructor should be used in Unity only and when building for the web only (Unity WEBGL).
        /// In fact it is preferable to use the default socket communication, as websocket connection doesn't support BlueBox and HRC systems.<br />
        /// This constructor has no effects if used in native .Net applications and is not available in the Universal Windows Platform DLL (websocket communication not supported, because redundant).
        /// <para />
        /// Starting from API v1.7.3, websocket communication uses the default SFS2X binary protocol (see <c>UseWebSocket</c> class). Legacy text protocol is still available for backward compatibility with SmartFoxserver 2X versions prior to 2.13.
        /// </remarks>
        ///
        /// <param name="useWebSocket">If set to <c>UseWebSocket.WS_BIN</c>, non-secure websocket communication (WS) is used; if set to <c>UseWebSocket.WSS_BIN</c>, secure websocket communication (WSS) is used.</param>
        /// <param name="debug">If <c>true</c>, the SmartFoxServer API debug messages are logged.</param>
        ///
        /// <example>
        /// The following example instantiates the <b>SmartFox</b> class enabling secure websocket communication and debug messages:
        /// <code>
        /// SmartFox sfs = new SmartFox(UseWebSocket.WSS_BIN, true);
        /// </code>
        /// </example>
        public SmartFox(UseWebSocket useWebSocket, bool debug)
        {
            this.useWebSocket = useWebSocket;
            Initialize(debug);
        }

        private void Initialize(bool debug)
        {
            if (!inited)
            {
                log = new Logger(this);
                this.debug = debug;
                if (dispatcher == null)
                {
                    dispatcher = new EventDispatcher(this);
                }
                if (useWebSocket.HasValue)
                {
                    bool useWSBinary = useWebSocket == UseWebSocket.WS_BIN || useWebSocket == UseWebSocket.WSS_BIN;
                    bool useWSSecure = useWebSocket == UseWebSocket.WSS || useWebSocket == UseWebSocket.WSS_BIN;
                    socketClient = new WebSocketClient(this, useWSBinary, useWSSecure);
                    socketClient.IoHandler = new WSIOHandler(socketClient);
                    udpAvailable = false;
                }
                else
                {
                    socketClient = new BitSwarmClient(this);
                    socketClient.IoHandler = new SFSIOHandler(socketClient);
                }
                socketClient.Init();
                socketClient.Dispatcher.AddEventListener(BitSwarmEvent.CONNECT, OnSocketConnect);
                socketClient.Dispatcher.AddEventListener(BitSwarmEvent.DISCONNECT, OnSocketClose);
                socketClient.Dispatcher.AddEventListener(BitSwarmEvent.RECONNECTION_TRY, OnSocketReconnectionTry);
                socketClient.Dispatcher.AddEventListener(BitSwarmEvent.IO_ERROR, OnSocketIOError);
                socketClient.Dispatcher.AddEventListener(BitSwarmEvent.SECURITY_ERROR, OnSocketSecurityError);
                socketClient.Dispatcher.AddEventListener(BitSwarmEvent.DATA_ERROR, OnSocketDataError);
                inited = true;
                Reset();
            }
        }

        private void Reset()
        {
            userManager = new SFSGlobalUserManager(this);
            roomManager = new SFSRoomManager(this);
            buddyManager = new SFSBuddyManager(this);
            if (lagMonitor != null)
            {
                lagMonitor.Destroy();
            }
            isJoining = false;
            currentZone = null;
            lastJoinedRoom = null;
            sessionToken = null;
            mySelf = null;
        }

        /// <summary>
        /// Allows to set custom client details used to gather statistics about the client platform in the SFS2X Analytics Module.
        /// </summary>
        ///
        /// <remarks>
        /// This method must be called before the connection is started. <br />
        /// The length of the two strings combined must be &lt; 512 characters. 
        /// <para />
        /// By default the generic "Unity / .Net" and "Universal Windows Platform" labels (depending on the used DLL) are set as platform, without specifying the version.
        /// </remarks>
        ///
        /// <param name="platformId">The id of the runtime platform: for example "Unity WebPlayer" or "iOS".</param>
        /// <param name="version">An optional version of the runtime platform: for example "2.0.0".</param>
        public void SetClientDetails(string platformId, string version)
        {
            if (IsConnected)
            {
                log.Warn("SetClientDetails must be called before the connection is started");
            }
            else
            {
                clientDetails = ((platformId != null) ? platformId.Replace(':', ' ') : "");
                clientDetails += ":";
                clientDetails += ((version != null) ? version.Replace(':', ' ') : "");
            }
        }

        /// <summary>
        /// Enables the automatic realtime monitoring of the lag between the client and the server (round robin).
        /// </summary>
        ///
        /// <remarks>
        /// When turned on, the <see cref="F:Sfs2X.Core.SFSEvent.PING_PONG" /> event type is dispatched continuously, providing the average of the last ten measured lag values.
        /// The lag monitoring can be enabled after the login has been performed successfully only and it is automatically halted when the user logs out of a Zone or gets disconnected.
        /// </remarks>
        ///
        /// <param name="enabled">The lag monitoring status: <c>true</c> to start the monitoring, <c>false</c> to stop it.</param>
        /// <param name="interval">(default: 4) An optional amount of seconds to wait between each query (recommended 3-4s).</param>
        /// <param name="queueSize">(default: 10) The amount of values stored temporarily and used to calculate the average lag.</param>
        ///
        /// <seealso cref="F:Sfs2X.Core.SFSEvent.PING_PONG" />
        public void EnableLagMonitor(bool enabled, int interval, int queueSize)
        {
            if (mySelf == null)
            {
                log.Warn("Lag Monitoring requires that you are logged in a Zone!");
            }
            else if (enabled)
            {
                if (PlatformConfig.UseWebGL)
                {
                    lagMonitor = new WSLagMonitor(this, interval, queueSize);
                }
                else
                {
                    lagMonitor = new DefaultLagMonitor(this, interval, queueSize);
                }
                lagMonitor.Start();
            }
            else if (lagMonitor != null)
            {
                lagMonitor.Stop();
            }
        }

        /// <summary>
        /// See <see cref="M:Sfs2X.SmartFox.EnableLagMonitor(System.Boolean,System.Int32,System.Int32)" />.
        /// </summary>
        public void EnableLagMonitor(bool enabled)
        {
            EnableLagMonitor(enabled, 4, 10);
        }

        /// <summary>
        /// See <see cref="M:Sfs2X.SmartFox.EnableLagMonitor(System.Boolean,System.Int32,System.Int32)" />.
        /// </summary>
        public void EnableLagMonitor(bool enabled, int interval)
        {
            EnableLagMonitor(enabled, interval, 10);
        }

        /// <exclude />
        public ISocketClient GetSocketEngine()
        {
            return socketClient;
        }

        /// <summary>
        /// Retrieves a Room object from its id.
        /// </summary>
        ///
        /// <remarks>
        /// The same object is returned by the <b>IRoomManager.getRoomById()</b> method, accessible through the <see cref="P:Sfs2X.SmartFox.RoomManager" /> getter;
        /// this was replicated on the <em>SmartFox</em> class for handy access due to its usually frequent usage.
        /// </remarks>
        ///
        /// <param name="id">The id of the Room.</param>
        ///
        /// <returns>An object representing the requested Room; <c>null</c> if no <see cref="T:Sfs2X.Entities.Room" /> object with the passed id exists in the Rooms list.</returns>
        ///
        /// <example>
        /// The following example retrieves a <see cref="T:Sfs2X.Entities.Room" /> object and writes its name:
        /// <code>
        /// int roomId = 3;
        /// Room room = sfs.GetRoomById(roomId);
        /// Console.WriteLine("The name of Room " + roomId + " is " + room.Name);                       // .Net / Unity
        /// System.Diagnostics.Debug.WriteLine("The name of Room " + roomId + " is " + room.Name);      // UWP
        /// </code>
        /// </example>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.GetRoomByName(System.String)" />
        /// <seealso cref="P:Sfs2X.SmartFox.RoomList" />
        /// <seealso cref="T:Sfs2X.Entities.Managers.SFSRoomManager" />
        public Room GetRoomById(int id)
        {
            return roomManager.GetRoomById(id);
        }

        /// <summary>
        /// Retrieves a Room object from its name.
        /// </summary>
        ///
        /// <remarks>
        /// The same object is returned by the <b>IRoomManager.getRoomById()</b> method, accessible through the <see cref="P:Sfs2X.SmartFox.RoomManager" /> getter;
        /// this was replicated on the <em>SmartFox</em> class for handy access due to its usually frequent usage.
        /// </remarks>
        ///
        /// <param name="name">The name of the Room.</param>
        ///
        /// <returns>An object representing the requested Room; <c>null</c> if no <see cref="T:Sfs2X.Entities.Room" /> object with the passed name exists in the Rooms list.</returns>
        ///
        /// <example>
        /// The following example retrieves a <see cref="T:Sfs2X.Entities.Room" /> object and writes its id:
        /// <code>
        /// string roomName = "The Lobby";
        /// Room room = sfs.GetRoomByName(roomName);
        /// Console.WriteLine("The id of Room '" + roomName + "' is " + room.Id);                           // .Net / Unity
        /// System.Diagnostics.Debug.WriteLine("The id of Room '" + roomName + "' is " + room.Id);          // UWP
        /// </code>
        /// </example>
        ///
        /// <seealso cref="T:Sfs2X.Entities.Managers.SFSRoomManager" />
        /// <seealso cref="P:Sfs2X.SmartFox.RoomList" />
        /// <seealso cref="T:Sfs2X.Entities.SFSRoom" />
        public Room GetRoomByName(string name)
        {
            return roomManager.GetRoomByName(name);
        }

        /// <summary>
        /// Retrieves the list of Rooms which are part of the specified Room Group.
        /// </summary>
        ///
        /// <remarks>
        /// The same list is returned by the <b>IRoomManager.getRoomById()</b> method, accessible through the <see cref="P:Sfs2X.SmartFox.RoomManager" /> getter;
        /// this was replicated on the <em>SmartFox</em> class for handy access due to its usually frequent usage.
        /// </remarks>
        ///
        /// <param name="groupId">The name of the Group.</param>
        ///
        /// <returns>The list of <see cref="T:Sfs2X.Entities.Room" /> objects belonging to the passed Group.</returns>
        ///
        /// <seealso cref="T:Sfs2X.Entities.Managers.SFSRoomManager" />
        /// <seealso cref="T:Sfs2X.Entities.Room" />
        public List<Room> GetRoomListFromGroup(string groupId)
        {
            return roomManager.GetRoomListFromGroup(groupId);
        }

        /// <summary>
        /// Simulates an abrupt disconnection from the server.
        /// </summary>
        ///
        /// <remarks>
        /// This method should be used for testing and simulations only, otherwise use the <see cref="M:Sfs2X.SmartFox.Disconnect" /> method.<br />
        /// This method is not supported in case of websocket connection.
        /// </remarks>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.Disconnect" />
        public void KillConnection()
        {
            socketClient.KillConnection();
        }

        /// <summary>
        /// Establishes a connection between the client and a SmartFoxServer 2X instance.
        /// </summary>
        ///
        /// <remarks>
        /// If no argument is passed, the client will use the settings loaded via <see cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" /> method.
        /// <para />
        /// The client usually connects to a SmartFoxServer instance through a socket connection. In case a socket connection can't be established,
        /// and the <b>UseBlueBox</b> property is set to <c>true</c>, a tunnelled http connection through the BlueBox module is attempted as a fail-safe system.
        /// When a successful connection is established, the <b>ConnectionMode</b> property can be used to check the current connection mode.
        /// <para />
        /// Tunnelled http connection is not available in case of websocket connection.
        /// <para />
        /// When using a websocket connection to an IPv6 address, always wrap the <i>host</i> value in square brackets.
        /// </remarks>
        ///
        /// <param name="host">The address of the server to connect to.</param>
        /// <param name="port">The TCP port to connect to.</param>
        ///
        /// <exception cref="T:System.ArgumentException">If an invalid host/address or port is passed, and it can't be found in the loaded settings.</exception>
        ///
        /// <example>
        /// The following example connects to a local SmartFoxServer 2X instance:
        /// <code>
        /// void SomeMethod() {
        /// 	SmartFox sfs = new SmartFox();
        ///
        /// 	sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        /// 	sfs.Connect("127.0.0.1", 9933);
        /// }
        ///
        /// void OnConnection(BaseEvent evt) {
        /// 	if ((bool)evt.Params["success"])
        /// 	{
        /// 		Console.WriteLine("Connection was established");                         // .Net / Unity
        /// 	    System.Diagnostics.Debug.WriteLine("Connection was established");        // UWP
        /// 	}
        /// 	else
        /// 	{
        /// 		Console.WriteLine("Connection failed");                         // .Net / Unity
        /// 	    System.Diagnostics.Debug.WriteLine("Connection failed");        // UWP
        /// 	}
        /// }
        /// </code>
        /// </example>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" />
        /// <seealso cref="P:Sfs2X.SmartFox.ConnectionMode" />
        /// <seealso cref="M:Sfs2X.SmartFox.Disconnect" />
        /// <seealso cref="F:Sfs2X.Core.SFSEvent.CONNECTION" />
        public void Connect(string host, int port)
        {
            if (IsConnected)
            {
                log.Warn("Already connected");
                return;
            }
            if (isConnecting)
            {
                log.Warn("A connection attempt is already in progress");
                return;
            }
            if (config == null)
            {
                config = new ConfigData();
                config.Debug = Debug;
            }
            if (host == null)
            {
                host = config.Host;
            }
            if (port == -1)
            {
                port = config.Port;
            }
            if (host == null || host.Length == 0)
            {
                throw new ArgumentException("Invalid connection host name / IP address");
            }
            if (port < 0 || port > 65535)
            {
                throw new ArgumentException("Invalid connection port");
            }
            config.Host = host;
            config.Port = port;
            lastHost = host;
            isConnecting = true;
            socketClient.Connect(host, port);
        }

        /// <summary>
        /// See <see cref="M:Sfs2X.SmartFox.Connect(System.String,System.Int32)" />.
        /// </summary>
        public void Connect()
        {
            Connect(null, -1);
        }

        /// <summary>
        /// See <see cref="M:Sfs2X.SmartFox.Connect(System.String,System.Int32)" />.
        /// </summary>
        public void Connect(string host)
        {
            Connect(host, -1);
        }

        /// <summary>
        /// Establishes a connection between the client and a SmartFoxServer 2X instance using a configuration object.
        /// </summary>
        ///
        /// <remarks>
        /// The client usually connects to a SmartFoxServer instance through a socket connection. In case a socket connection can't be established,
        /// and the <b>UseBlueBox</b> property is set to <c>true</c>, a tunnelled http connection through the BlueBox module is attempted as a fail-safe system.
        /// When a successful connection is established, the <b>ConnectionMode</b> property can be used to check the current connection mode.
        /// <para />
        /// Tunnelled http connection is not available in case of websocket connection.
        /// </remarks>
        ///
        /// <param name="cfg">The client configuration object.</param>
        ///
        /// <exception cref="T:System.ArgumentException">If an invalid host/address or port is passed, and it can't be found in the loaded settings.</exception>
        ///
        /// <example>
        /// The following example connects to a local SmartFoxServer 2X instance:
        /// <code>
        /// ConfigData cfg = new ConfigData();
        /// cfg.Host = "127.0.0.1";
        /// cfg.Port = 9933;
        /// cfg.Zone = "BasicExamples";
        ///
        /// sfs.Connect(cfg);
        /// </code>
        /// </example>
        ///
        /// <seealso cref="T:Sfs2X.Util.ConfigData" />
        /// <seealso cref="P:Sfs2X.SmartFox.ConnectionMode" />
        /// <seealso cref="M:Sfs2X.SmartFox.Disconnect" />
        /// <seealso cref="F:Sfs2X.Core.SFSEvent.CONNECTION" />
        public void Connect(ConfigData cfg)
        {
            ValidateConfig(cfg);
            Connect(cfg.Host, cfg.Port);
        }

        /// <summary>
        /// Closes the connection between the client and the SmartFoxServer 2X instance.
        /// </summary>
        /// <seealso cref="M:Sfs2X.SmartFox.Connect" />
        /// <seealso cref="F:Sfs2X.Core.SFSEvent.CONNECTION_LOST" />
        public void Disconnect()
        {
            if (IsConnected)
            {
                if (PlatformConfig.UseWebGL)
                {
                    HandleClientDisconnection(ClientDisconnectionReason.MANUAL);
                    return;
                }
                else
                {
                    if (socketClient.ReconnectionSeconds > 0)
                    {
                        Send(new ManualDisconnectionRequest());
                        int millisecondsTimeout = 100;
                        Thread.Sleep(millisecondsTimeout);
                    }
                    HandleClientDisconnection(ClientDisconnectionReason.MANUAL);
                }
            }
            else
            {
                log.Info("You are not connected");
            }
        }

        /// <summary>
        /// Initializes the UDP protocol by performing an handshake with the server.
        /// </summary>
        ///
        /// <remarks>
        /// This method needs to be called only once. It can be executed at any moment provided that a connection to the server has already been established.<br />
        /// After a successful initialization, UDP requests can be sent to the server-side Extension at any moment.
        /// <para />
        /// If <em>udpHost</em> or <em>udpPort</em> arguments are not passed, the client will use the settings loaded via <see cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" /> method.
        /// <para />
        /// UDP protocol is not available in case of websocket connection.
        /// <para />
        /// <b>MTU note</b>
        /// <para />
        /// The <em>Maximum Transmission Unit</em> (MTU), represents the largest amount of bytes that can be sent at once before packet fragmentation occurs.
        /// Since the UDP protocol uses a "nothing-or-all" approach to the transmission, it is important to keep in mind that, on average, a message size of 1100-1200 bytes is probably the maximum you can reach.
        /// If you exceed the MTU size the data will be "lost in hyperspace" (the Internet).
        /// <para />
        /// Another interesting matter is that there's no fixed size for the MTU, each operating system uses a slighlty different size.
        /// Because of this we suggest a conservative data size of 1000-1200 bytes per packet to avoid packet loss.
        /// <para />
        /// The SFS2X protocol compression allows to send 2-3KBytes of uncompressed data which usually is squeezed down to a size of ~1000 bytes.
        /// If you have larger data to send we suggest to organize it in smaller chunks so that they don't exceed the suggested MTU size.
        /// <para />
        /// More details about the MTU can be found here: <see href="http://en.wikipedia.org/wiki/Maximum_transmission_unit" />.
        /// </remarks>
        ///
        /// <param name="udpHost">The IP address of the server to connect to.</param>
        /// <param name="udpPort">he UDP port to connect to.</param>
        ///
        /// <example>
        /// The following example initializes the UDP communication, sends a request to an Extension and handles the related events:
        /// <code>
        /// void SomeMethod() {
        /// 	sfs.AddEventListener(SFSEvent.UPD_INIT, OnUDPInit);
        /// 	sfs.InitUDP();
        /// }
        ///
        /// void OnUDPInit(BaseEvent evt) {
        /// 	if ((bool)evt.Params["success"]) {
        /// 		// Execute an extension call via UDP
        /// 		sfs.Send( new ExtensionRequest("udpTest", new SFSObject(), null, true) ):
        /// 	} else {
        /// 		Console.WriteLine("UDP init failed!");                          // .Net / Unity
        /// 	    System.Diagnostics.Debug.WriteLine("UDP init failed!");         // UWP
        /// 	}
        /// }
        /// </code>
        /// </example>
        ///
        /// <seealso cref="P:Sfs2X.SmartFox.UdpAvailable" />
        /// <seealso cref="F:Sfs2X.Core.SFSEvent.UDP_INIT" />
        /// <seealso cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" />
        public void InitUDP(string udpHost, int udpPort)
        {
            if (!IsConnected)
            {
                Logger.Warn("Cannot initialize UDP protocol until the client is connected to SFS2X");
                return;
            }
            if (MySelf == null)
            {
                Logger.Warn("Cannot initialize UDP protocol until the user is logged-in");
                return;
            }
            if (socketClient.UdpManager == null || !socketClient.UdpManager.Inited)
            {
                if (useWebSocket.HasValue)
                {
                    Logger.Warn("UDP not supported in WebSocket mode");
                    return;
                }
                IUDPManager udpManager = new UDPManager(this);
                socketClient.UdpManager = udpManager;
            }
            if (socketClient.UdpManager == null)
            {
                return;
            }
            if (config != null)
            {
                if (udpHost == null)
                {
                    udpHost = config.UdpHost;
                }
                if (udpPort == -1)
                {
                    udpPort = config.UdpPort;
                }
            }
            if (udpHost == null || udpHost.Length == 0)
            {
                throw new ArgumentException("Invalid UDP host/address");
            }
            if (udpPort < 0 || udpPort > 65535)
            {
                throw new ArgumentException("Invalid UDP port range");
            }
            try
            {
                socketClient.UdpManager.Initialize(udpHost, udpPort);
            }
            catch (Exception ex)
            {
                log.Error("Exception initializing UDP: " + ex.Message);
            }
        }

        /// <summary>
        /// See <see cref="M:Sfs2X.SmartFox.InitUDP(System.String,System.Int32)" />.
        /// </summary>
        public void InitUDP()
        {
            InitUDP(null, -1);
        }

        /// <summary>
        /// See <see cref="M:Sfs2X.SmartFox.InitUDP(System.String,System.Int32)" />.
        /// </summary>
        public void InitUDP(string udpHost)
        {
            InitUDP(udpHost, -1);
        }

        /// <summary>
        /// Initializes the connection cryptography to protect all client-server communications with standard TLS protocol.
        /// </summary>
        ///
        /// <remarks>
        /// This method must be called right after a successful connection, before the login is performed.<br />
        /// Once the encryption initialization process is successfully completed, all of the server's data will be encrypted using standard AES 128-bit algorithm, with a secure key served over HTTPS.
        /// <para />
        /// <para />
        /// <b>IMPORTANT UNITY REMARKS</b>
        /// <para />
        /// This method is not available when building for WebGL: use WSS connection instead.
        /// <para />
        /// When building for the Web Player, do not use Security.PrefetchSocketPolicy in your code. In fact this method accepts an IP address only, while you should connect to the domain name instead, since the SSL certificate is (typically) bound to that.
        /// Let the Web Player auto-fetch the cross-domain policy from the default TCP port 843. In order to do this, add a listener for such port in the SFS2X AdminTool's Server Configurator module.
        /// </remarks>
        ///
        /// <example>
        /// The following example initializes the encrypted communication:
        /// <code>
        /// void SomeMethod() {
        /// 	SmartFox sfs = new SmartFox();
        ///
        /// 	sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        /// 	sfs.AddEventListener(SFSEvent.CRYPTO_INIT, OnEncryptionInitialized);
        ///
        /// 	sfs.Connect("mysecuredomain.com", 9933);
        /// }
        ///
        /// void OnConnection(BaseEvent evt) {
        /// 	if ((bool)evt.Params["success"])
        /// 	{
        /// 		Console.WriteLine("Connection was established");
        ///
        /// 		// Initialize encrypted connection
        /// 		sfs.InitCrypto();
        /// 	}
        /// 	else
        /// 	{
        /// 		Console.WriteLine("Connection failed");
        /// 	}
        /// }
        ///
        /// void OnEncryptionInitialized(BaseEvent evt) {
        /// 	if ((bool)evt.Params["success"])
        /// 	{
        /// 		// Do login
        /// 		sfs.Send( new LoginRequest("FozzieTheBear", "", "SimpleChat") );
        /// 	}
        /// 	else
        /// 	{
        /// 		Console.WriteLine("Encryption initialization failed. Caused by: " + (string)evt.Params["errorMsg"]);
        /// 	}
        /// }
        /// </code>
        /// </example>
        ///
        /// <seealso cref="F:Sfs2X.Core.SFSEvent.CRYPTO_INIT" />
        public void InitCrypto()
        {
            if (useWebSocket.HasValue)
            {
                Logger.Warn("InitCrypto method not supported in WebSocket mode; use WSS protocol instead");
            }
            ICryptoInitializer cryptoInitializer = new CryptoInitializerV2(this);
            cryptoInitializer.Run();
        }

        /// <exclude />
        public int GetReconnectionSeconds()
        {
            return socketClient.ReconnectionSeconds;
        }

        /// <exclude />
        public void SetReconnectionSeconds(int seconds)
        {
            socketClient.ReconnectionSeconds = seconds;
        }

        /// <summary>
        /// Sends a request to the server.  
        /// </summary>
        ///
        /// <remarks>
        /// All the available request objects can be found under the <see cref="N:Sfs2X.Requests" /> namespace. 
        /// </remarks>
        ///
        /// <param name="request">A request object.</param>
        ///
        /// <example>
        /// The following example sends a login request:
        /// <code>
        /// sfs.Send( new LoginRequest("KermitTheFrog", "KermitPass", "TheMuppetZone") );
        /// </code>
        /// </example>
        ///
        /// <example>
        /// The following example sends a login request:
        /// <code>
        /// sfs.Send( new JoinRoomRequest("Lobby") );
        /// </code>
        /// </example>
        ///
        /// <example>
        /// The following example creates an object containing some parameters and sends it to the server-side Extension.
        /// <code>
        /// ISFSObject parameters = SFSObject.NewInstance();
        /// parameters.SetInt("x", 10);
        /// parameters.SetInt("y", 37);
        /// sfs.Send( new ExtensionRequest("setPosition", parameters) );
        /// </code>
        /// </example>
        public void Send(IRequest request)
        {
            if (!IsConnected)
            {
                Logger logger = log;
                string[] array = new string[1];
                array[0] = "You are not connected. Request cannot be sent: " + ((request != null) ? request.ToString() : null);
                logger.Warn(array);
                return;
            }
            try
            {
                if (request is JoinRoomRequest)
                {
                    if (isJoining)
                    {
                        return;
                    }
                    isJoining = true;
                }
                request.Validate(this);
                request.Execute(this);
                socketClient.Send(request.Message);
            }
            catch (SFSValidationError sFSValidationError)
            {
                string text = sFSValidationError.Message;
                foreach (string error in sFSValidationError.Errors)
                {
                    text = text + "\t" + error + "\n";
                }
                log.Warn(text);
            }
            catch (SFSCodecError sFSCodecError)
            {
                log.Warn(sFSCodecError.Message);
            }
        }

        /// <summary>
        /// Loads the client configuration file.
        /// </summary>
        ///
        /// <remarks>
        /// The SmartFox instance can be configured through an external xml configuration file loaded at run-time.
        /// By default, this method loads a file named "sfs-config.xml", placed in the same folder of the application file.<br />
        /// If the <i>connectOnSuccess</i> parameter is set to <c>true</c>, on loading completion the <see cref="M:Sfs2X.SmartFox.Connect(System.String,System.Int32)" /> method is automatically called by the API, otherwise the <see cref="F:Sfs2X.Core.SFSEvent.CONFIG_LOAD_SUCCESS" /> event is dispatched.
        /// In case of loading error, the <see cref="F:Sfs2X.Core.SFSEvent.CONFIG_LOAD_FAILURE" /> event id fired.
        /// <para />
        /// The external xml configuration file has the following structure:
        /// 			<code>
        ///                 &lt;SmartFoxConfig&gt;
        ///
        ///                     &lt;!-- required --&gt;
        ///                     &lt;host&gt;localhost&lt;/host&gt;
        ///
        ///                     &lt;!-- required --&gt;
        ///                     &lt;port&gt;9933&lt;/port&gt;
        ///
        ///                     &lt;udpHost&gt;localhost&lt;/udpHost&gt;
        ///                     &lt;udpPort&gt;9933&lt;/udpPort&gt;
        ///
        ///                     &lt;zone&gt;BasicExamples&lt;/zone&gt;
        ///                     &lt;debug&gt;true&lt;/debug&gt;
        ///
        ///                     &lt;httpPort&gt;8080&lt;/httpPort&gt;
        ///                     &lt;httpsPort&gt;8443&lt;/httpsPort&gt;
        ///
        ///                     &lt;blueBox&gt;
        ///                         &lt;isActive&gt;true&lt;/isActive&gt;
        ///                         &lt;useHttps&gt;false&lt;/useHttps&gt;
        ///                         &lt;pollingRate&gt;500&lt;/pollingRate&gt;
        ///
        ///                         &lt;proxy&gt;
        ///                             &lt;host&gt;&lt;/host&gt;
        ///                             &lt;port&gt;&lt;/port&gt;
        ///                             &lt;userName&gt;&lt;/userName&gt;
        ///                             &lt;password&gt;&lt;/password&gt;
        ///                             &lt;bypassLocal&gt;true&lt;/bypassLocal&gt;
        ///                         &lt;/proxy&gt;
        ///                     &lt;/blueBox&gt;    
        ///                 &lt;/SmartFoxConfig&gt;
        /// 			</code>
        /// </remarks>
        ///
        /// <param name="filePath">(default: sfs-config.xml) Filename of the external XML configuration, including its path relative to the folder of the application file.</param>
        /// <param name="connectOnSuccess">(default: true) A flag indicating if the connection to SmartFoxServer must be attempted upon configuration loading completion.</param>
        ///
        /// <example>
        /// The following example shows how to load an external configuration file:
        /// <code>
        /// void SomeMethod() {
        /// 	sfs.AddEventListener(SFSEvent.CONFIG_LOAD_SUCCESS, OnConfigLoadSuccessHandler);
        /// 	sfs.AddEventListener(SFSEvent.CONFIG_LOAD_FAILURE, OnConfigLoadFailureHandler);
        ///
        /// 	sfs.LoadConfig("testEnvironmentConfig.xml", false);
        /// }
        ///
        /// void OnConfigLoadSuccessHandler(BaseEvent evt) {
        /// 	Console.WriteLine("Config file loaded, now connecting...");
        /// 	sfs.Connect(sfs.IpAddress, sfs.Port);
        /// }
        ///
        /// void OnConfigLoadFailureHandler(BaseEvent evt) {
        /// 	Console.WriteLine("Failed loading config file: " + evt.Params["message"]);
        /// }
        /// </code>
        /// </example>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.Connect(System.String,System.Int32)" />
        /// <seealso cref="F:Sfs2X.Core.SFSEvent.CONFIG_LOAD_SUCCESS" />
        /// <seealso cref="F:Sfs2X.Core.SFSEvent.CONFIG_LOAD_FAILURE" />
        public void LoadConfig(string filePath, bool connectOnSuccess)
        {
            ConfigLoader configLoader = new ConfigLoader(this);
            configLoader.Dispatcher.AddEventListener(SFSEvent.CONFIG_LOAD_SUCCESS, OnConfigLoadSuccess);
            configLoader.Dispatcher.AddEventListener(SFSEvent.CONFIG_LOAD_FAILURE, OnConfigLoadFailure);
            autoConnectOnConfig = connectOnSuccess;
            configLoader.LoadConfig(filePath);
        }

        /// <summary>
        /// See <see cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" />.
        /// </summary>
        public void LoadConfig(string filePath)
        {
            LoadConfig(filePath, true);
        }

        /// <summary>
        /// See <see cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" />.
        /// </summary>
        public void LoadConfig(bool connectOnSuccess)
        {
            LoadConfig("sfs-config.xml", connectOnSuccess);
        }

        /// <summary>
        /// See <see cref="M:Sfs2X.SmartFox.LoadConfig(System.String,System.Boolean)" />.
        /// </summary>
        public void LoadConfig()
        {
            LoadConfig("sfs-config.xml", true);
        }

        /// <summary>
        /// Registers a delegate method for log messages callbacks.
        /// </summary>
        ///
        /// <remarks>
        /// Calling this method is just like calling the <see cref="M:Sfs2X.Logging.Logger.AddEventListener(Sfs2X.Logging.LogLevel,Sfs2X.Core.EventListenerDelegate)">Logger.AddEventListener</see> method directly,
        /// but in addition the <see cref="P:Sfs2X.Logging.Logger.EnableEventDispatching">Logger.EnableEventDispatching</see> property is automatically set to <c>true</c>. 
        /// </remarks>
        ///
        /// <param name="logLevel">The level of the log events to register a listener for.</param>
        /// <param name="eventListener">The event listener to register.</param>
        ///
        /// <example>
        /// <code>
        /// void SomeMethod() {
        /// 	sfs.AddLogListener(LogLevel.INFO, OnInfoLogMessage);
        /// 	sfs.AddLogListener(LogLevel.WARN, OnWarnLogMessage);
        /// }
        ///
        /// void OnInfoLogMessage(BaseEvent evt) {
        /// 	string message = (string)evt.Params["message"];
        /// 	Console.WriteLine("[SFS2X INFO] " + message);                       // .Net / Unity
        /// 	System.Diagnostics.Debug.WriteLine("[SFS2X INFO] " + message);      // UWP
        /// }
        ///
        /// void OnWarnLogMessage(BaseEvent evt) {
        /// 	string message = (string)evt.Params["message"];
        /// 	Console.WriteLine("[SFS2X WARN] " + message);                       // .Net / Unity
        /// 	System.Diagnostics.Debug.WriteLine("[SFS2X WARN] " + message);      // UWP
        /// }
        /// </code>
        /// </example>
        ///
        /// <seealso cref="M:Sfs2X.Logging.Logger.AddEventListener(Sfs2X.Logging.LogLevel,Sfs2X.Core.EventListenerDelegate)" />
        public void AddLogListener(LogLevel logLevel, EventListenerDelegate eventListener)
        {
            AddEventListener(LoggerEvent.LogEventType(logLevel), eventListener);
            log.EnableEventDispatching = true;
        }

        /// <summary>
        /// Removes a delegate method for log messages callbacks.
        /// </summary>
        ///
        /// <remarks>
        /// Calling this method is just like calling the <see cref="M:Sfs2X.Logging.Logger.RemoveEventListener(Sfs2X.Logging.LogLevel,Sfs2X.Core.EventListenerDelegate)">Logger.RemoveEventListener</see> method directly. 
        /// </remarks>
        ///
        /// <param name="logLevel">The level of the log events to remove the listener for.</param>
        /// <param name="eventListener">The event listener to remove.</param>
        ///
        /// <seealso cref="M:Sfs2X.Logging.Logger.RemoveEventListener(Sfs2X.Logging.LogLevel,Sfs2X.Core.EventListenerDelegate)" />
        public void RemoveLogListener(LogLevel logLevel, EventListenerDelegate eventListener)
        {
            RemoveEventListener(LoggerEvent.LogEventType(logLevel), eventListener);
        }

        /// <exclude />
        public void AddJoinedRoom(Room room)
        {
            if (!roomManager.ContainsRoom(room.Id))
            {
                roomManager.AddRoom(room);
                lastJoinedRoom = room;
                return;
            }
            throw new SFSError("Unexpected: joined room already exists for this User: " + mySelf.Name + ", Room: " + ((room != null) ? room.ToString() : null));
        }

        /// <exclude />
        public void RemoveJoinedRoom(Room room)
        {
            roomManager.RemoveRoom(room);
            if (JoinedRooms.Count > 0)
            {
                lastJoinedRoom = JoinedRooms[JoinedRooms.Count - 1];
            }
        }

        private void OnSocketConnect(BaseEvent e)
        {
            BitSwarmEvent bitSwarmEvent = e as BitSwarmEvent;
            if ((bool)bitSwarmEvent.Params["success"])
            {
                SendHandshakeRequest((bool)bitSwarmEvent.Params["isReconnection"]);
                return;
            }
            log.Warn("Connection attempt failed");
            HandleConnectionProblem(bitSwarmEvent);
        }

        private void OnSocketClose(BaseEvent e)
        {
            BitSwarmEvent bitSwarmEvent = e as BitSwarmEvent;
            Reset();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["reason"] = bitSwarmEvent.Params["reason"];
            DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_LOST, dictionary));
        }

        private void OnSocketReconnectionTry(BaseEvent e)
        {
            DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_RETRY));
        }

        private void OnSocketDataError(BaseEvent e)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["errorMessage"] = e.Params["message"];
            DispatchEvent(new SFSEvent(SFSEvent.SOCKET_ERROR, dictionary));
        }

        private void OnSocketIOError(BaseEvent e)
        {
            BitSwarmEvent e2 = e as BitSwarmEvent;
            if (isConnecting)
            {
                HandleConnectionProblem(e2);
            }
        }

        private void OnSocketSecurityError(BaseEvent e)
        {
            BitSwarmEvent e2 = e as BitSwarmEvent;
            if (isConnecting)
            {
                HandleConnectionProblem(e2);
            }
        }

        private void OnConfigLoadSuccess(BaseEvent e)
        {
            SFSEvent sFSEvent = e as SFSEvent;
            ConfigLoader configLoader = sFSEvent.Target as ConfigLoader;
            ConfigData configData = sFSEvent.Params["cfg"] as ConfigData;
            configLoader.Dispatcher.RemoveEventListener(SFSEvent.CONFIG_LOAD_SUCCESS, OnConfigLoadSuccess);
            configLoader.Dispatcher.RemoveEventListener(SFSEvent.CONFIG_LOAD_FAILURE, OnConfigLoadFailure);
            ValidateConfig(configData);
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["config"] = configData;
            BaseEvent evt = new SFSEvent(SFSEvent.CONFIG_LOAD_SUCCESS, dictionary);
            DispatchEvent(evt);
            if (autoConnectOnConfig)
            {
                Connect(config.Host, config.Port);
            }
        }

        private void OnConfigLoadFailure(BaseEvent e)
        {
            SFSEvent sFSEvent = e as SFSEvent;
            log.Error("Failed to load config: " + (string)sFSEvent.Params["message"]);
            ConfigLoader configLoader = sFSEvent.Target as ConfigLoader;
            configLoader.Dispatcher.RemoveEventListener(SFSEvent.CONFIG_LOAD_SUCCESS, OnConfigLoadSuccess);
            configLoader.Dispatcher.RemoveEventListener(SFSEvent.CONFIG_LOAD_FAILURE, OnConfigLoadFailure);
            BaseEvent evt = new SFSEvent(SFSEvent.CONFIG_LOAD_FAILURE);
            DispatchEvent(evt);
        }

        private void ValidateConfig(ConfigData cfgData)
        {
            if (cfgData.Host == null || cfgData.Host.Length == 0)
            {
                throw new ArgumentException("Invalid host name / IP address in configuration data");
            }
            if (cfgData.Port < 0 || cfgData.Port > 65535)
            {
                throw new ArgumentException("Invalid TCP port in configuration data");
            }
            if (cfgData.Zone == null || cfgData.Zone.Length == 0)
            {
                throw new ArgumentException("Invalid Zone name in configuration data");
            }
            config = cfgData;
            debug = cfgData.Debug;
        }

        /// <exclude />
        public void HandleHandShake(BaseEvent evt)
        {
            ISFSObject iSFSObject = evt.Params["message"] as ISFSObject;
            if (iSFSObject.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                sessionToken = iSFSObject.GetUtfString(HandshakeRequest.KEY_SESSION_TOKEN);
                socketClient.CompressionThreshold = iSFSObject.GetInt(HandshakeRequest.KEY_COMPRESSION_THRESHOLD);
                socketClient.MaxMessageSize = iSFSObject.GetInt(HandshakeRequest.KEY_MAX_MESSAGE_SIZE);
                if (debug)
                {
                    log.Debug(string.Format("Handshake response: tk => {0}, ct => {1}", sessionToken, socketClient.CompressionThreshold));
                }
                if (socketClient.IsReconnecting)
                {
                    socketClient.IsReconnecting = false;
                    DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_RESUME));
                    return;
                }
                isConnecting = false;
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["success"] = true;
                DispatchEvent(new SFSEvent(SFSEvent.CONNECTION, dictionary));
            }
            else
            {
                short @short = iSFSObject.GetShort(BaseRequest.KEY_ERROR_CODE);
                Logger logger = log;
                object[] utfStringArray = iSFSObject.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS);
                string errorMessage = SFSErrorCodes.GetErrorMessage(@short, logger, utfStringArray);
                Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
                dictionary2["success"] = false;
                dictionary2["errorMessage"] = errorMessage;
                dictionary2["errorCode"] = @short;
                DispatchEvent(new SFSEvent(SFSEvent.CONNECTION, dictionary2));
            }
        }

        /// <exclude />
        public void HandleLogin(BaseEvent evt)
        {
            currentZone = evt.Params["zone"] as string;
        }

        /// <exclude />
        public void HandleClientDisconnection(string reason, bool triggerEvent = true)
        {
            socketClient.ReconnectionSeconds = 0;
            socketClient.Disconnect(reason);
            Reset();
        }

        /// <exclude />
        public void HandleLogout()
        {
            if (lagMonitor != null && lagMonitor.IsRunning)
            {
                lagMonitor.Stop();
            }
            userManager = new SFSGlobalUserManager(this);
            roomManager = new SFSRoomManager(this);
            isJoining = false;
            lastJoinedRoom = null;
            currentZone = null;
            mySelf = null;
        }

        private void HandleConnectionProblem(BaseEvent e)
        {
            if (socketClient.ConnectionMode == ConnectionModes.SOCKET && config.BlueBox.IsActive)
            {
                socketClient.ForceBlueBox(true);
                udpAvailable = false;
                int port = 8080;
                if (config != null)
                {
                    port = (config.BlueBox.UseHttps ? config.HttpsPort : config.HttpPort);
                }
                socketClient.Connect(lastHost, port);
                DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_ATTEMPT_HTTP, new Dictionary<string, object>()));
                return;
            }
            if (socketClient.ConnectionMode != ConnectionModes.WEBSOCKET_TEXT && socketClient.ConnectionMode != ConnectionModes.WEBSOCKET_BIN && socketClient.ConnectionMode != ConnectionModes.WEBSOCKET_SECURE_TEXT && socketClient.ConnectionMode != ConnectionModes.WEBSOCKET_SECURE_BIN)
            {
                socketClient.ForceBlueBox(false);
            }
            BitSwarmEvent bitSwarmEvent = e as BitSwarmEvent;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["success"] = false;
            dictionary["errorMessage"] = bitSwarmEvent.Params["message"];
            DispatchEvent(new SFSEvent(SFSEvent.CONNECTION, dictionary));
            isConnecting = false;
            socketClient.Destroy();
        }

        /// <exclude />
        public void HandleReconnectionFailure()
        {
            SetReconnectionSeconds(0);
            socketClient.StopReconnection();
        }

        private void SendHandshakeRequest(bool isReconnection)
        {
            IRequest request = new HandshakeRequest(Version, isReconnection ? sessionToken : null, clientDetails);
            Send(request);
        }

        internal void DispatchEvent(BaseEvent evt)
        {
            if (!threadSafeMode)
            {
                Dispatcher.DispatchEvent(evt);
            }
            else
            {
                EnqueueEvent(evt);
            }
        }

        private void EnqueueEvent(BaseEvent evt)
        {
            lock (eventsLocker)
            {
                eventsQueue.Enqueue(evt);
            }
        }

        /// <summary>
        /// Tells the API to process all event queues and execute the delegate callbacks.
        /// </summary>
        ///
        /// <remarks>
        /// This method must be called by the client application to maintain thread safety, in conjunction with the ThreadSafeMode property being set to <c>true</c>.<br />
        /// Typically this method is called in Unity's MonoBehavior.Update method.
        /// </remarks>
        ///
        /// <example>
        /// <code>
        /// void FixedUpdate() {
        /// 	sfs.ProcessEvents();
        /// }
        /// </code>
        /// </example>
        ///
        /// <seealso cref="P:Sfs2X.SmartFox.ThreadSafeMode" />
        public void ProcessEvents()
        {
            if (!threadSafeMode)
            {
                return;
            }
            if (useWebSocket.HasValue && socketClient != null)
            {
                WebSocketLayer webSocketLayer = socketClient.Socket as WebSocketLayer;
                if (webSocketLayer != null)
                {
                    webSocketLayer.ProcessState();
                }
                if (lagMonitor != null)
                {
                    lagMonitor.Execute();
                }
            }
            if (eventsQueue.Count != 0)
            {
                BaseEvent[] array;
                lock (eventsLocker)
                {
                    array = eventsQueue.ToArray();
                    eventsQueue.Clear();
                }
                for (int i = 0; i < array.Length; i++)
                {
                    Dispatcher.DispatchEvent(array[i]);
                }
            }
        }

        /// <summary>
        /// Adds a delegate to a given API event type that will be used for callbacks.
        /// </summary>
        ///
        /// <param name="eventType">The name of the <see cref="T:Sfs2X.Core.SFSEvent" /> to get callbacks on.</param>
        /// <param name="listener">The delegate method to register.</param>
        ///
        /// <example>
        /// <code>
        /// void SomeMethod() {
        /// 	sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        /// }
        ///
        /// public void OnConnection(BaseEvent evt) {
        /// 	bool success = (bool)evt.Params["success"];
        /// 	string error = (string)evt.Params["error"];
        /// 	Debug.Log("On Connection callback got: " + success + " (error : " + error + ")");
        /// }
        /// </code>
        /// </example>
        ///
        /// <seealso cref="T:Sfs2X.Core.SFSEvent" />
        /// <seealso cref="M:Sfs2X.SmartFox.RemoveEventListener(System.String,Sfs2X.Core.EventListenerDelegate)" />
        public void AddEventListener(string eventType, EventListenerDelegate listener)
        {
            dispatcher.AddEventListener(eventType, listener);
        }

        /// <summary>
        /// Removes a delegate registration for a given API event.
        /// </summary>
        ///
        /// <param name="eventType">The SFSEvent to remove callbacks on.</param>
        /// <param name="listener">The delegate method to unregister.</param>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.RemoveAllEventListeners" />
        /// <seealso cref="M:Sfs2X.SmartFox.AddEventListener(System.String,Sfs2X.Core.EventListenerDelegate)" />
        public void RemoveEventListener(string eventType, EventListenerDelegate listener)
        {
            dispatcher.RemoveEventListener(eventType, listener);
        }

        /// <summary>
        /// Removes all event listeners.
        /// </summary>
        ///
        /// <remarks>
        /// Please note that log delegates need to be removed separately using the <see cref="M:Sfs2X.SmartFox.RemoveEventListener(System.String,Sfs2X.Core.EventListenerDelegate)" /> method.
        /// </remarks>
        ///
        /// <seealso cref="M:Sfs2X.SmartFox.RemoveEventListener(System.String,Sfs2X.Core.EventListenerDelegate)" />
        public void RemoveAllEventListeners()
        {
            dispatcher.RemoveAll();
        }
    }
}
