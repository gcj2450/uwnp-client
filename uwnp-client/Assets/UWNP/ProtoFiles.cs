using ProtoBuf;
using System;
using System.Collections.Generic;

public enum PacketID
{
    C_LOGIN = 0,
    S_LOGIN = 1,
    C_CHAT = 2,
    S_CHAT = 3,
    S_ENTER_GAME = 4,
    S_LEAVE_GAME = 5,
    S_SPAWN = 6,
    S_DESPAWN = 7,
    C_MOVE = 8,
    S_MOVE = 9,
    C_SKILL = 10,
    S_SKILL = 11,
    S_CHANGE_HP = 12,
    C_ENTER_GAME = 13,
    S_GAME_ROOM = 14,
    C_ROOM_CREATE = 15,
    S_ROOM_CREATE = 16
}

public enum CreatureState
{
    IDLE = 0,
    MOVING = 1,
    SKILL = 2,
    DEAD = 3
}
public enum MoveDir
{
    UP = 0,
    DOWN = 1,
    LEFT = 2,
    RIGHT = 3
}

public enum GameObjectType
{
    NONE = 0,
    PLAYER = 1,
    MONSTER = 2,
    PROJECTILE = 3
}

public enum SkillType
{
    SKILL_NONE = 0,
    SKILL_AUTO = 1,
    SKILL_PROJECTILE = 2
}

[ProtoContract]
public class C_Login
{
}

[ProtoContract]
public class S_Login
{
    [ProtoMember(1)]
    public ObjectInfo player;
}

[ProtoContract]
public class C_Chat
{
    [ProtoMember(1)]
    public string chat;
}

[ProtoContract]
public class S_Chat
{
    [ProtoMember(1)]
    public int playerId;
    [ProtoMember(1)]
    public string chat;
}

[ProtoContract]
public class S_EnterGame
{
    [ProtoMember(1)]
    public ObjectInfo player;
}

[ProtoContract]
public class C_EnterGame
{
    [ProtoMember(1)]
    public int roomId;
    [ProtoMember(2)]
    public string name;
}

[ProtoContract]
public class S_LeaveGame
{
}

[ProtoContract]
public class S_Spawn
{
    [ProtoMember(1)]
    public List<ObjectInfo> objects;
}

[ProtoContract]
public class S_Despawn
{
    [ProtoMember(1)]
    public List<int> objectIds;
}

[ProtoContract]
public class C_Move
{
    [ProtoMember(1)]
    public PositionInfo posInfo;
}

[ProtoContract]
public class S_Move
{
    [ProtoMember(1)]
    public int objectId = 1;
    [ProtoMember(2)]
    public PositionInfo posInfo;
}

[ProtoContract]
public class C_Skill
{
    [ProtoMember(1)]
    public SkillInfo info;
}

[ProtoContract]
public class S_Skill
{
    [ProtoMember(1)]
    public int objectId;
    [ProtoMember(2)]
    public SkillInfo info;
}

[ProtoContract]
public class S_ChangeHp
{
    [ProtoMember(1)]
    public int objectId;
    [ProtoMember(2)]
    public int hp;
}

[ProtoContract]
public class S_GameRoom
{
    [ProtoMember(1)]
    public List<RoomInfo> roomInfo;
}

[ProtoContract]
public class C_RoomCreate
{
    [ProtoMember(1)]
    public int userCount;
    [ProtoMember(2)]
    public string name;
}

[ProtoContract]
public class S_RoomCreate
{
    [ProtoMember(1)]
    public int roomId;
}

[ProtoContract]
public class RoomInfo
{
    [ProtoMember(1)]
    public ObjectInfo roomMaster;
    [ProtoMember(2)]
    public int roomId;
    [ProtoMember(3)]
    public string name;
    [ProtoMember(4)]
    public int userMax;
}

[ProtoContract]
public class ObjectInfo
{
    [ProtoMember(1)]
    public int objectId = 1;
    [ProtoMember(2)]
    public string name;
    [ProtoMember(3)]
    public PositionInfo posInfo;
    [ProtoMember(4)]
    public StatInfo statInfo;
}

[ProtoContract]
public class PositionInfo
{
    [ProtoMember(1)]
    public CreatureState state;
    [ProtoMember(2)]
    public MoveDir moveDir;
    [ProtoMember(3)]
    public int posX;
    [ProtoMember(4)]
    public int posY;
}

[ProtoContract]
public class SkillInfo
{
    [ProtoMember(1)]
    public int skillId;
}

[ProtoContract]
public class StatInfo
{
    [ProtoMember(1)]
    public int level;
    [ProtoMember(2)]
    public int hp;
    [ProtoMember(3)]
    public int maxHp;
    [ProtoMember(4)]
    public int attack;
    [ProtoMember(5)]
    public float speed;
    [ProtoMember(6)]
    public int totalExp;
}