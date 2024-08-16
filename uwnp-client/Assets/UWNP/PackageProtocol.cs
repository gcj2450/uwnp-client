using ProtoBuf;
using System.IO;

namespace UWNP
{
    /// <summary>
    /// 协议ID
    /// </summary>
    public struct ReservedCodes
    {
        // Reserved message codes and their parameters
        // Parameters are each Length Content: use ByteBuilder.GetParameter()
        // Client -> Server: 0000 to 5FFF
        public const uint SignIn = 0x72720000;  // Username, Password
        public const uint PassToMember = 0x72720001;  // Code, Target, (any other args)
        public const uint Broadcast = 0x72720002;  // Code, (any other args)
        public const uint GameBroadcast = 0x72720003;  // Code, Game ID, (any other args)
        public const uint ToGameOwner = 0x72720004;  // Code, Game ID, (any other args)

        public const uint GetGameTypeTokens = 0x72721000;  // Gametype, Key match string
        public const uint RequestGameType = 0x72721001;  // Gametype, Version

        public const uint MemberChangeName = 0x72722000;  // New name
        public const uint AdminCommand = 0x72722001;  // Text

        public const uint CreateGame = 0x72723001;  // Max players, Gametype, Version, Flags, Name, Password
        public const uint SetMyGameFlags = 0x72723002;  // Member ID, Game ID, New flags
        public const uint UpdateGameInfo = 0x72723005;  // Game ID, Max players, Flags, Name, Password
        public const uint JoinRequestResponse = 0x72723006;  // MemberID, Request code, Game ID, Successful, Message
        public const uint KickPlayer = 0x72723007;  // Game ID, Player ID, Message
        public const uint GetGameTokens = 0x72723008;  // GameID, Key match string
        public const uint SetGameFlags = 0x72723009;  // Game ID, Flags

        // Server -> Client: 6000 to AFFF
        public const uint SignInChallenge = 0x72726000;  // Message, FailedAlready

        public const uint DownloadGameType = 0x72727000;  // Gametype, Version, Data (if 0 bytes of data, not found)

        public const uint MemberUpdate = 0x72729000;  // MemberID, Flags, User name, Display name
        public const uint MemberLeft = 0x72729001;  // MemberID, Flags, User name, Display name
        public const uint YouAre = 0x72729002;  // MemberID
        public const uint MemberJoined = 0x72729003;  // Display name
        public const uint PlayerResponse = 0x72729004;  // Sender ID, Request code, Successful, Message

        public const uint GameUpdate = 0x7272A000;  // GameID, Flags, Creator, Max players, Gametype, Version, Name, Players (array of MemberIDs), Player flags
        public const uint GameUpdateUI = 0x7272A001;  // GameID
        public const uint ReadyToStart = 0x7272A002;  // GameID

        public const uint GameMessage = 0x7272AFFD;  // Game ID, Message
        public const uint GameWarning = 0x7272AFFE;  // Game ID, Message
        public const uint GameError = 0x7272AFFF;  // Game ID, Message

        // Either way: B000 to FFFF
        public const uint NoOp = 0x7272B000;  // None
        public const uint Chat = 0x7272B001;  // Scope, MemberID, Message, Extra information
        public const uint Version = 0x7272B002;     // Version string

        public const uint SetGameTypeToken = 0x7272C000;  // Game ID, [Broadcast], (Key, Value)譶
        public const uint GetGameTypeToken = 0x7272C001;  // Game ID, (Key, [Value])譶

        public const uint SetMemberFlags = 0x7272D001;  // New flags

        public const uint CustomPlayerInfo = 0x7272E000;  // [out: MemberID], Info. For within a game
        public const uint GameClosed = 0x7272E001;  // GameID
        public const uint RequestJoinGame = 0x7272E002;  // [out: Requester ID], Request code, Game ID, Password
        public const uint LeaveGame = 0x7272E003;  // GameID, [out: Message]
        public const uint StartGame = 0x7272E004;  // GameID, SetFlags
        public const uint SetGameToken = 0x7272E005;  // GameID, (Key Value)譶
        public const uint GetGameToken = 0x7272E006;  // GameID, (Key [Value])譶

        public const uint Message = 0x7272FFFD;  // Message
        public const uint Warning = 0x7272FFFE;  // Message
        public const uint Error = 0x7272FFFF;  // Message
    }

    /// <summary>
    /// 聊天范围
    /// </summary>
    public struct ChatScope
    {
        public const int Lobby = 1;
        public const int Game = 2;
        public const int Private = 3;
    }

    public enum PackageType
    {
        HEARTBEAT = 1,
        REQUEST = 2,
        PUSH = 3,
        KICK = 4,
        RESPONSE = 5, 
        HANDSHAKE = 6,
        ERROR = 7,
        NOTIFY = 8
    }

    public class PackageProtocol
    {
        public static byte[] Encode(PackageType type)
        {
            Package sr = new Package() {
                packageType = (uint)type
            };
            return Serialize(sr);
        }

        public static byte[] Encode<T>(PackageType type, uint packID, string route, T info, string modelName = null)
        {
            Package sr = new Package(){
                packageType = (uint)type,
                packID = packID,
                route = route,
                buff = Serialize<T>(info),
                modelName = modelName
            };
            return Serialize(sr);
        }

        public static byte[] Serialize<T>(T info) {
            MemoryStream ms = new MemoryStream();
            Serializer.Serialize(ms, info);
            byte[] buff = ms.ToArray();
            ms.Close();
            return buff;
        }

        public static Package Decode(byte[] buff)
        {
            //protobuf反序列化
            MemoryStream mem = new MemoryStream(buff);
            Package rs = Serializer.Deserialize<Package>(mem);
            mem.Close();
            return rs;
        }
    }
}