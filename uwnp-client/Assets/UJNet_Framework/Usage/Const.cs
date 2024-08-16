using UnityEngine;
using System.Collections;

public enum SType
{
	IOS_APPSTORE_IPAD,
	IOS_APPSTORE_IPHONE,
	IOS_91_IPAD,
	IOS_91_IPHONE,
	ANDROID_SAMSUNG,
	ANDROID_JIFENG,
	ANDROID_ANZHI,
	ANDROID_LIANTONG,
	ANDROID_MM,
	ANDROID_91,
	ANDROID_TIANYIKONGJIAN,
	ANDROID_360,
	ANDROID_UC,
    ANDROID_DANGLE,
    ANDROID_CHINATELECOM,
	ANDROID_MI,
	IOS_UC_IPAD,
	IOS_UC_IPHONE,
	IOS_TWM_IPAD,
	IOS_TWM_IPHONE,
	ANDROID_TWM_GOOGLE,
	ANDROID_TWM_MATCH,
	ANDROID_TWM_SMARKET,
	ANDROID_TWM_HAMI,
	IOS_AIMING_IPAD,
	IOS_AIMING_IPHONE,
	ANDROID_AIMING_TSTORE,
	ANDROID_AIMING_OLLEH,
	ANDROID_AIMING_LGU,
	ANDROID_AIMING_GOOGLE,
	ANDROID_WANGXUN,
	ANDROID_SNDA,
	ANDROID_NDUO_MARKET,
	ANDROID_LIQU,
	ANDROID_YOUYOU,
	ANDROID_SHOUYOU,
	ANDROID_GAME_TT,
    ANDROID_GAME_TT1,
    ANDROID_GAME_TT2,
    ANDROID_GAME_FLTX,
    ANDROID_GAME_JUHAOWAN,
    ANDROID_GAME_JIULINGWANG,
    ANDROID_GAME_JINSHAN,    
	ANDROID_WISTONE,
}

public class Const
{
	public const string LANG = "lang";
	public const string SERVER_ID = "svrid";
	public const int SCENES_ID_CITY = 1;
	public static string CAMERA_TAG = "MainCamera";
	public static string UICAMERA_TAG = "uiCamera";
	public static float BUILDING_HIGHLIGHT_TIME = 0.3f;
//	public const string LEVEL_GATE = "login";
//	public const string LEVEL_CITY = "3guo";
//	public const string LEVEL_MAP = "3guo mpad";
//	public const string LEVEL_PVP = "pvp_war";
//	public const string LEVEL_PVE_MAP = "pvemap2";
//	public const string LEVEL_PVE = "war_1";
//	public const string LEVEL_PVE_PRE = "war_";
//	public const string LEVEL_LEAGUE = "zt";
//	public const string LEVEL_FALL_STONE = "3guo fall stone";
//	public const string LEVEL_ROLL_WOOD = "3guo roll wood";
//	public const string LEVEL_PICK_GOLD = "3guo pick gold";
	
	public const int LEVEL_PREPARE_LOAD = 0;
	public const int LEVEL_GATE = LEVEL_PREPARE_LOAD + 1;
	public const int LEVEL_CITY = LEVEL_GATE + 1;
	public const int LEVEL_PVE_MAP = LEVEL_CITY + 1;
	public const int LEVEL_PVE_WAR_1 = LEVEL_PVE_MAP + 1;
	public const int LEVEL_PVE_WAR_2 = LEVEL_PVE_WAR_1 + 1;
	public const int LEVEL_PVE_WAR_3 = LEVEL_PVE_WAR_2 + 1;
	public const int LEVEL_PVE_WAR_4 = LEVEL_PVE_WAR_3 + 1;
	public const int LEVEL_PVE_WAR_5 = LEVEL_PVE_WAR_4 + 1;
	public const int LEVEL_PVE_WAR_6 = LEVEL_PVE_WAR_5 + 1;
	public const int LEVEL_PVE_WAR_7 = LEVEL_PVE_WAR_6 + 1;
	public const int LEVEL_PVP = LEVEL_PVE_WAR_7 + 1;
	public const int LEVEL_FALL_STONE = LEVEL_PVP + 1;
	public const int LEVEL_ROLL_WOOD = LEVEL_FALL_STONE + 1;
	public const int LEVEL_PICK_GOLD = LEVEL_ROLL_WOOD + 1;
	public const int LEVEL_LEAGUE = LEVEL_PICK_GOLD + 1;
	public const int LEVEL_PVP_4 = LEVEL_LEAGUE + 1;
	public const int LEVEL_MAP = 100;
	
	
//	 public const string LEVEL_MAP = "3guo mpad";
	public const string LEVEL_PVE = "war_1";
	public const string LEVEL_PVE_PRE = "war_";
//	public const string LEVEL_LEAGUE = "zt";
	
	
	
	//only to used distinguish each protocol for each building
	public static string KEY_PORT_ID = "portid";
	//only to used distinguish each protocol for each protocol
	public const string KEY_INSTANCE_ID = "insid"; 
	
	// city style ctrl  to use it when building lv up change style
	public  static readonly int[] CITY_STYLE_LEVEL_CHANGE = {0,1,5,15,25,35};
	
	//xie yi
	public const int CMD_IMPROVE_CONTENT = 2200;
	public const int CMD_IMPROVE_UPGRADE = 2201;
	public const int CMD_LOCK_PAYMENT = -2;
	  
	//protocol
	public const int PROTOCOL_704_PVP = 1;
	public const int PROTOCOL_704_LEAGUE_MEMBER = 2;
	public const int PROTOCOL_1702_LIST_PANEL = 1;
	public const int PROTOCOL_1702_BASE_INFO = 2;
	public const int PROTOCOL_1702_DESC_PANEL = 3;
	public const int PROTOCOL_511_SEARCH_GEN = 1;
	public const int PROTOCOL_511_EXP = 2;
	public const int PROTOCOL_511_FUNCTION = 3;
	
	//512
	public const int PROTOCOL_512_GENERAL = 1;
	
	//201
	public const int PROTOCOL_201_CHAT = 1;
	public const int PROTOCOL_201_PVP = 2;
	public const int PROTOCOL_201_INVITE = 3;
	
	//404
	public const int PROTOCOL_404_EXP = 1; // chuzheng
//	//400
//	public const int PROTOCOL_400_CAMP = 1; // jun ying
//	public const int PROTOCOL_400_WALL = 2; // cheng qiang
	
	//503
	public const int PROTOCOL_503_GENERAL_MY = 1;
	public const int PROTOCOL_503_GENERAL_SEARCH = 2;
	public const int PROTOCOL_503_EXPEDITION = 3;// chu zheng
	public const int PROTOCOL_503_GARRISON = 4;// zhu jun
	public const int PROTOCOL_503_EQUIPMENT = 5;
	public const int PROTOCOL_503_FUNCTION = 6;
	
	//502
	public const int PROTOCOL_502_MY = 1;
	public const int PROTOCOL_502_EQUIPMENT = 2;
	public const int PROTOCOL_1005_ACTIONPOINT = 1;
	public const int PROTOCOL_1005_TRUCE = 2;
	public const int PROTOCOL_1005_SHOP = 3;
	
	
	//1401
	public const int PROTOCOL_1401_CAMP = 1;
	public const int PROTOCOL_1401_WALL = 2;
	public const int PROTOCOL_1401_GENERAL = 3;
	public const int PROTOCOL_1401_CROP = 4;
	
	
	//826
	public const int PROTOCOL_826_BTN_PANEL = 1;
	public const int PROTOCOL_826_LIST_PANEL = 2;
	
	//def setup num
	public const int CITY_DEF_SETUP_MAX_SELECT_ARMY = 3;

	//key to lock panel 
	public const string KEY_LOCK_TIMESTAMP = "timestamp";
	public const int BUILDING_TYPE_MAIN = 1; // main
	public const int BUILDING_TYPE_ECONOMIC = 2; //  jing ji 
	public const int BUILDING_TYPE_MILITARY = 3; // army
	public const int BUILDING_TYPE_RECREATION = 4; //fun

	//----------------- About General --------------------
	//min my gen num
	public const uint GENERAL_MIN_MY_GEN_NUM = 2;
	public const int GENERAL_UV_ATTACK = 0;
	public const int GENERAL_UV_ARMY_ATK = 1;
	public const int GENERAL_UV_DEFENECE = 2;
	public const int GENERAL_UV_ARMY_DEF = 3;
	public const int GENERAL_UV_SPEED = 4;
	public const int GENERAL_UV_ARMY_SPD = 5;
	public const int GENERAL_UV_CAPTAIN = 6;
	public const int GENERAL_DEF_FREE = 0; // is a defence general
	public const int GENERAL_DEF_DEFENCE = 1; // fang shou
	public const int GENERAL_DEF_WORK = 2; // gong zuo
	public const int GENERAL_DEF_SEARCH = 3;// xun fang
	public const int GENERAL_DEF_COMBAT = 4;// zhan dou
//	public const int GENERAL_COMBATSTATE_TRUCE = 0;// is a combat general
//	public const int GENERAL_COMBATSTATE_COMBAT = 1;
	//------------------------------general state-------------------------------------
	public const int GENERAL_STATE_ICON_FREE = 0;
	public const int GENERAL_STATE_ICON_DEFENECE = 1;
	public const int GENERAL_STATE_ICON_BATTLE = 2;
	public const int GENERAL_STATE_ICON_WORK = 3;
	//----------------- ----------------------------------
	public const int GENERAL_SKILL_MAX_LENGTH = 8; // general can hold max skill num
	
	//skill max level;
	public const int SKILL_MAX_LEVEL = 10;
	
	// æèœå­Šä¹ äžèŽ­ä¹°ç±»å
	public const int GENERAL_SKILL_LBTYPE_LEARN = 1;// å­Šä¹ æè?
	public const int GENERAL_SKILL_LBTYPE_BUY = 2;// èŽ­ä¹°æè?
	
	//------------------------------GENERAL_MIN_UV_INDEX----------------------------------
	public const int GENERAL_MIN_UV_ATTACK = 0;
	public const int GENERAL_MIN_UV_DEFENECE = 1;
	public const int GENERAL_MIN_UV_SPEED = 2;
	public const int GENERAL_MIN_UV_CAPTAIN = 3;
	//----------------------------------------------------------------

	//client wait the server respone time(one minute)
	public const double CONNECT_WAIT_TIME = 60000d;

	//color
	public const string COLOR_RED = "[#ff0000]";
	public const string COLOR_BLACK = "[#000000]";
	public const string COLOR_WHITE = "[#ffffff]";
	public const string COLOR_BROWN = "[#541b00]";
	public const string COLOR_LIGHT_BROWN = "[#6e2f0c]";
	public const string COLOR_GREEN_BLACK = "[#78ff27]";
	public const string COLOR_GREEN_YELLOW = "[#4a961c]";
	


	//Mail 
	public const int MAIL_INPUT_MAIL_CITYNAME_MAX_LENGTH = 10;
	public const int MAIL_INPUT_MAIL_THEME_MAX_LENGTH = 20;
	public const int MAIL_INPUT_MAIL_CONTENT_MAX_LENGTH = 100;
	public const int MAIL_INPUT_MAIL_CITYNAME_MIN_LENGTH = 1;
	public const int MAIL_INPUT_MAIL_THEME_MIN_LENGTH = 1;
	public const int MAIL_INPUT_MAIL_CONTENT_MIN_LENGTH = 1;

	//Rank
	public const int RANK_INPUT_MIN_LENGTH = 1;
	public const int RANK_INPUT_MAX_LENGTH = 10;
	public const int RANK_ITEM_COUNT = 10;
	//sgame type
	public const int RANK_SG_ROLL_WOOD = 1;
	public const int RANK_SG_PICK_GOLD = 2;
	public const int RANK_SG_FALL_STONE = 3;
	//pvp type
	public const int RANK_PVP_WEEK = 1;
	public const int RANK_PVP_MONTH = 2;
	public const int RANK_PVP_TOTAL = 3;
	
	//Camera const
	public const int LAYER_EVERYTHING = -1;
	public const int LAYER_NOTHING = 0;
	public const int LAYER_GUI = 8;
	public const int CITY_SCENE_LAYER_EVERYTHING = -33025; //11111111111111110111111011111111//0xffff7eff
	public const int LEAGUE_SCENE_LAYER_EVERYTHING = -33025; //11111111111111110111111011111111//0xffff7eff
	//building pid
	public const int PID_BULIDING_GOV = 1;
	public const int PID_BULIDING_HOUSE_1 = 2;
	public const int PID_BULIDING_FOOD_1 = 3;
	public const int PID_BULIDING_STONE_1 = 4;
	public const int PID_BULIDING_IRON_1 = 5;
	public const int PID_BULIDING_WOOD_1 = 6;
	public const int PID_BULIDING_ARMYCAMP = 7;
	public const int PID_BULIDING_GENERAL = 8;
	public const int PID_BULIDING_STORAGE = 9;
	public const int PID_BULIDING_MARKET = 10;
	public const int PID_BULIDING_WALL = 12;
	public const int PID_BULIDING_SGAME = 13;
	public const int PID_BULIDING_HOUSE_2 = 14;
	public const int PID_BULIDING_HOUSE_3 = 15;
	public const int PID_BULIDING_HOUSE_4 = 16;
	public const int PID_BULIDING_FOOD_2 = 17;
	public const int PID_BULIDING_FOOD_3 = 18;
	public const int PID_BULIDING_FOOD_4 = 19;
	public const int PID_BULIDING_WOOD_2 = 20;
	public const int PID_BULIDING_WOOD_3 = 21;
	public const int PID_BULIDING_WOOD_4 = 22;
	
	// league building pid
	public const int PID_LEAGUE_CENTER = 101;
	public const int PID_LEAGUE_GOLD = 102;
	public const int PID_LEAGUE_FOOD = 103;
	public const int PID_LEAGUE_STONE = 104;
	public const int PID_LEAGUE_IRON = 105;
	public const int PID_LEAGUE_WOOD = 106;
	public const int PID_LEAGUE_GAIN_RES = 107;
	public const int PID_LEAGUE_GAIN_POINT = 108;
	public const int PID_LEAGUE_GOLD_STORAGE = 109;
	public const int PID_LEAGUE_FOOD_STORAGE = 110;
	public const int PID_LEAGUE_STONE_STORAGE = 111;
	public const int PID_LEAGUE_IRON_STORAGE = 112;
	public const int PID_LEAGUE_WOOD_STORAGE = 113;

	//crop prop type
	public const int CROP_TYPE_GOLD = 2;
	public const int CROP_TYPE_FOOD = 3;
	public const int CROP_TYPE_STONE = 4;
	public const int CROP_TYPE_IRON = 5;
	public const int CROP_TYPE_WOOD = 6;
	public const int CROP_TYPE_LEA_GOLD = 102;
	public const int CROP_TYPE_LEA_FOOD = 103;
	public const int CROP_TYPE_LEA_STONE = 104;
	public const int CROP_TYPE_LEA_IRON = 105;
	public const int CROP_TYPE_LEA_WOOD = 106;
	
	
	// crop type
	public const int CC_TYPE_FOOD1 = 3;
	public const int CC_TYPE_FOOD2 = 17;
	public const int CC_TYPE_FOOD3 = 18;
	public const int CC_TYPE_FOOD4 = 19;
	public const int CC_TYPE_WOOD1 = 6;
	public const int CC_TYPE_WOOD2 = 20;
	public const int CC_TYPE_WOOD3 = 21;
	public const int CC_TYPE_WOOD4 = 22;
	public const int CC_TYPE_GOLD1 = 2;
	public const int CC_TYPE_GOLD2 = 14;
	public const int CC_TYPE_GOLD3 = 15;
	public const int CC_TYPE_GOLD4 = 16;
	
	//player cstate
	public const int BATTLECITY_ONLINESTATE_ONLINE = 1;
	public const int BATTLECITY_ONLINESTATE_OFFLINE = 2;

	//scout type
	public const int SCOUT_TYPE_CITY = 1;
	public const int SCOUT_TYPE_LEAGUE = 2;
	//battle warning battle state
	public const int BT_BATTLESTATE_DEFAULT = 0;
	public const int BT_BATTLESTATE_INIT = 1;
	public const int BT_BATTLESTATE_WAIT = 2;
	public const int BT_BATTLESTATE_BEGIN = 3;
	public const int BT_BATTLESTATE_END = 4;
	public const int BT_BATTLESTATE_FREE = 5;
	public const int BT_BATTLESTATE_DESTROY = 6;
	
	//-------------------------------------------------------------------------
	// bat btn panel state
	public const  int BAT_BTN_NO_INIT = 1; //hai wei chu shi hua
	public const  int BAT_BTN_NO_WAR = 2; // wu zhan dou
	public const  int BAT_BTN_ALL_WAIT = 3;// deng dai
	public const  int BAT_BTN_HAS_INBAT = 4; // you kai zhan
	//-------------------------------------------------------------------------
	// player faction
	public const int BATTLE_PLAYER_FACTION_ATTACKER = 1;
	public const int BATTLE_PLAYER_FACTION_DEFENDER = 2;
	
	
	//garrison city flag
	public const int GARRISON_CITYFLAG_NO_SLAVE = 1;
	public const int GARRISON_CITYFLAG_SLAVE = 2;
	
	//garrison type
	public const int GARRISON_TYPE_SELF_NO_SLAVE = 1;
	public const int GARRISON_TYPE_SELF_SELF_SLAVE = 2;
	public const int GARRISON_TYPE_SELF_OTHER_SLAVE = 3;
	public const int GARRISON_TYPE_OTHER_SLAVE = 4;
	
	// general and army def plan index
	public const int GARRISON_DEF_PLAN_NONE = 0;
	public const int GARRISON_DEF_PLAN_FREEDOM = 1;
	public const int GARRISON_DEF_PLAN_RANGE = 2;
	public const int GARRISON_DEF_PLAN_AWAIT = 3;
	
	
	
	// city garrison each page have five element
	public const int CITY_GARRISON_PAGE_ITEM_COUNT = 5;
	
	
	//league event type
	public const int LEAGUE_EVENT_TYPE_IN_BATTLE = 1;
	public const int LEAGUE_EVENT_TYPE_CAPTAIN = 2;
	
	//league event type by gui entry point
	public const int EP_EVENT_TYPE_INTELLIGENCE = 1;
	public const int EP_EVENT_TYPE_EVENT = -1;
	
	
	//league introduction max length
	public const int LEAGUE_CHANGE_INTRODUCTION_MAX_LENGTH = 40;
	
	// league applicant each page have five element
	public const int LEAGUE_APPLICANT_PAGE_ITEM_COUNT = 8;
	
	// league member each page have five element
	public const int LEAGUE_MEMBER_PAGE_ITEM_COUNT = 8;
	
	//league post
	public const int  LEAGUE_POST_ADMIN = 1;
	public const int  LEAGUE_POST_MEMBER = 3;
	
	// exp type
	public const int EXP_TYPE_PVE = 1;
	public const int EXP_TYPE_PVP = 2;
	public const int EXP_TYPE_EXPLORE = 6;
	public const int EXP_TYPE_LEAGUE_PLUNDER = 3;
	public const int EXP_TYPE_LEAGUE_CONQUER = 4;
	public const int EXP_TYPE_LEAGUE_INSTANCE = 5;
	
	// fly tip index
	public const int BAT_GREEN = 0;
	public const int BAT_RED = 1;
	public const int BAT_REDBIG = 2;
	public const int BAT_YELLOW = 3;
	
	
	// func id
	public const int FUN_SWITCH_POS = 1;
	
	//--------------------chat const------------------------
	//chat max Count
	public const int CHAT_MAX_COUNT = 29;
	//chat max Count
	public const int CHAT_MAX_CHAR_COUNT = 300;
	//-tab index
	public const int CHAT_TAB_WORLD = 0;
	public const int CHAT_TAB_LEAGUE = 1;
	public const int CHAT_TAB_PRIVATE = 2;
	//-
	public const string CHAT_COLOR_GREEN = "[#6da54a]";
	public const string CHAT_COLOR_PURPLE = "[#8663b4]";
	public const string CHAT_COLOR_RED = "[#f86324]";
	public const string CHAT_COLOR_ORANGE = "[#ca794c]";
	public const string CHAT_COLOR_BLUE = "[#4d98d2]";
	public const string CHAT_COLOR_GRAY = "[#515050]";
	
	//-------------------chat type------------------------------
	public const int CHAT_TYPE_PLAYER = 1;
	public const int CHAT_TYPE_GM = 2;
	public const int CHAT_TYPE_SYSTEM = 3;
	public const int CHAT_TYPE_ANNOUNCEMENT = 4;
	public const int CHAT_TYPE_PUSH = 5;
//	public const int CHAT_TYPE_I_PRIVATE = 5;
//	public const int CHAT_TYPE_I_LEAGUE = 6;
//	public const int CHAT_TYPE_I_WORLD = 7;
	//-----------------------------------------------------
	//--------------------chat item-----------------
	public const int CHAT_CHANNEL_WORLD = 1;
	public const int CHAT_CHANNEL_LEAGUE = 2;
	public const int CHAT_CHANNEL_PRIVATE = 3;
	public const int CHAT_CHANNEL_NOTICE = 4;
	//---------------------------end--------------------
	public const int CHAT_SAY_WORLD = 0;
	public const int CHAT_SAY_LEAGUE = 1;
	public const int CHAT_SAY_PRIVATE = 2;
	//--------------------chat item icon-----------------
	public const int CHAT_ICON_WORLD = 1;
	public const int CHAT_ICON_SYSTEM = 2;
	public const int CHAT_ICON_ANNOUNCEMENT = 3;
	public const int CHAT_ICON_PRIVATE = 4;
	public const int CHAT_ICON_LEAGUE = 5; 
	//--------------------end-----------------
	
	//----------
	public const string CHAT_CHAR_SAY = "chat.item.char.say";
	public const string CHAT_CHAR_I = "chat.item.char.i";
	public const string CHAT_CHAR_YON = "chat.item.char.you";
	public const string CHAT_CHAR_TREAT = "chat.item.char.treat";
	//-------------------------------------------------------
	
	
	//--------------------------PVP---------------------------------
	
	//under atk list length
	public const int PVP_UNDER_ATK_LIST_LENGTH = 4;
	public const  int PVP_BATTLE_RESULT_WIN = 1;
	public const  int PVP_BATTLE_RESULT_LOSE = 2;
	//hero rank type
	public const int PVP_HERO_RANK_TYPE_WEEK = 1;
	public const int PVP_HERO_RANK_TYPE_MONTH = 2;
	public const int PVP_HERO_RANK_TYPE_TOTAL = 3;
	
	// combat state
	public const int PVP_BATTLE_STATE_PEACE = 1;
	public const int PVP_BATTLE_STATE_WAR = 2; 
	// online state
	public const int PVP_LINE_STATE_ONLINE = 1;
	public const int PVP_LINE_STATE_OFFLINE = 2; 
	
	//bat and online show state
	public const int PVP_ICON_SHOW_BAT = 1;
	public const int PVP_ICON_SHOW_ONLINE = 2;
	public const int PVP_ICON_SHOW_OFFLINE = 3;
	
	//------------------------end--------------------------------------
	 
	// -----------------IMPROVE TYPE-------------------------------------------
	public const int IMPROVE_TYPE_STORAGE = 1;
	public const int IMPROVE_TYPE_GENERAL = 2;
	public const int IMPROVE_TYPE_WOUNDEDARMY = 3;
	public const int IMPROVE_TYPE_WALL = 4;
	//------------------------end--------------------------------------
	
	// -----------------CROP BUILDING style changer-------------------------------------------
	public const int RESB_SHOW_STYLE_ERROR = -1;
	public const int RESB_SHOW_STYLE_IDEL = 1;
	public const int RESB_SHOW_STYLE_PRODUCE = 2;
	public const int RESB_SHOW_STYLE_CROP = 3;
	public const int RESB_SHOW_STYLE_INACTIVE = 4;
	//------------------------end--------------------------------------
	
	// -----------------GEN_FUNCTION-------------------------------------------
	public const int 	GEN_FUNCTIONS_COUNT = 3;
	// -----------------
	public const int 	CIA_FUNCTION_WOODAFFAIRS = 1;//zhong shu jian ling
	public const int 	CIA_FUNCTION_GOLDAFFAIRS = 2;//si jin lang zhong jiang
	public const int 	CIA_FUNCTION_UPGRADEAFFAIRS = 3;//si kong
	public const int 	CIA_FUNCTION_FOODAFFAIRS = 4;//da si nong
	
	public const int 	CIA_AFFECT_FOOD = 1;
	public const int 	CIA_AFFECT_GOLD = 2;
	public const int 	CIA_AFFECT_WOOD = 3;
	public const int 	CIA_AFFECT_SEEK = 4; // xun fang ji lv
	public const int 	CIA_AFFECT_UPGRADE = 5;// sheng ji shi jian
	
	// --------------------------------------------------------------------------
	//   ---------------------- user name length;----------------------
	public const int 	USER_NAME_MAX_LENGTH = 8;
	public const int 	USER_NAME_MIN_LENGTH = 0;
	
	//equipment
	public const int EQUIPMENT_TYPE_HELMET = 1;
	public const int EQUIPMENT_TYPE_ARMOUR = 2;
	public const int EQUIPMENT_TYPE_GLOVES = 3;
	public const int EQUIPMENT_TYPE_BOOTS = 4;
	public const int EQUIPMENT_TYPE_WEAPON = 5;
	
	
	// all timer delay time
	public const int TIME_CYCLE_DELAY = 5;
	public const int RES_CASH_ICON = 12;
	public const int RES_GOOD_ICON = 10;
	
	
	//-----------------------GOODS  type-------------------------------------------------
	public const int GOODS_TYPE_BUILDING = 1;
	public const int GOODS_TYPE_CROP = 2;
	public const int GOODS_TYPE_ARMY = 3;
	public const int GOODS_TYPE_GENERAL = 4;
	public const int GOODS_TYPE_LEA_BUILDING = 5;
	public const int GOODS_TYPE_LEA_CROP = 6;
	public const int GOODS_TYPE_SEEK = 7;
	public const int GOODS_TYPE_TECHNOLOGY = 8;
	public const int GOODS_TYPE_RESOURCE = 9;
	public const int GOODS_TYPE_TRUCE = 10;
	public const int GOODS_TYPE_ACTIONPOINT = 11;
	public const int GOODS_TYPE_CASH = 12;
	public const int GOODS_TYPE_91_CASH = 13;
	public const int GOODS_TYPE_SAMSUNG_CASH = 14;
	public const int GOODS_TYPE_TWM_CASH = 15;
	public const int GOODS_TYPE_TWM_AND_CASH=16;
	public const int GOODS_TYPE_TWM_AND_CASH_MATCH=17;
	public const int GOODS_TYPE_TWM_AND_CASH_SMARKET=18;
	public const int GOODS_TYPE_TWM_AND_CASH_HAMI=19;
	public const int GOODS_TYPE_NEW_IOS = 17;
	public const int GOODS_TYPE_AIMING_IOS = 22;
	public const int GOODS_TYPE_AIMING_TSTORE = 23;
	public const int GOODS_TYPE_AIMING_GOOGLE = 24;
    public const int GOODS_TYPE_AIMING_OLLLEH = 25;
    public const int GOODS_TYPE_AIMING_LGU = 26;
    
	// -----------------------------truce state -----------------------------
	public const int TRUCE_STATE_NORMAL = 1;
	public const int TRUCE_STATE_TRUCE = 2;
	public const int TRUCE_STATE_COOLDOWN = 3;

	//exp general hp exp min 80%
	public const float EXP_GEN_MIN_PER = 0.6f;
	
//	common timer control
	public const int TIME_UNINIT = 1;
	public const int TIME_UNOPEN = 2;
	public const int TIME_TIMING = 3;
	public const int TIME_END = 4;
	
	
	// PlayerPrefs key
	public const string UNDO_WAR_GUIDE = "uid-{0}.undo_war_guide";
	public const string SOUND_LOCK = "sound_lock_1";
//	public const string UNDO_HELP_SHOW_WAR = "uid-{0}.undo_help_show_war";
	public const string UNDO_HELP_SHOW_CITY = "uid-{0}.help_show_city";
	public const string NOTICE_CITY = "notice_city";
	public const string TO_PLAYERS = "To players";
	public const string PREF_KEY_JID = "uid-{0}.pref_key_jid";
	public const string UNDO_TWM_ACC_TIP = "twm-acc-{0}-tip";
	
	
	//-------------------------------------mission-------------------------------------
	public const   int MISSION_LIST_MAX_COUNT = 5;
	public const int MISSION_TYPE_MAIN = 1;
	public const int MISSION_TYPE_BRANCH = 2;
	public const int MISSION_TYPE_DAILY = 3;
	public const int MISSION_STATE_OPEN = 1;
	public const int MISSION_STATE_COMPLETE = 2;
	//-------------------------------------mission   end-------------------------------------
	//report  mode type 
	public const int BA_BATTLEMODE_PVP = 1;// 1 pvp
	public const int BA_BATTLEMODE_PVE = 2;// 2 pve
	public const int BA_BATTLEMODE_DISCOVERY = 8;//discovery
	
	//
	public const string SCOPE_JID = "session_id";
	public const string SCOPE_ACC_ID = "account_id";
	public const string SCOPE_AUTH_KEY = "req_auth_key";
	public const string SCOPE_HTTP_SVR_URL = "http_svr_url";
	public const string SCOPE_CHAT_SVR_URL = "chat_svr_url";
	
	
	// in app purchase state	
	public const int PR_IAPSTATE_PAY_WAITTING = 0;
	public const int PR_IAPSTATE_PAY_SUCCESS = 1;
	public const int PR_IAPSTATE_PAY_FAILED = 2;
	public const int PR_IAPSTATE_PAY_CANCEL = 3;
	public const int IOS_DEVICE_IPHONE = 1;
	public const int IOS_DEVICE_IPAD = 2;
	
	// vip state time
	public const  int VIP_STATE_NONE = 0;
	public const  int VIP_STATE_VIP = 1;
	public const  int VIP_SEVER_CLOSE = 0;
	public const  int VIP_SEVER_OPEN = 1;
	
	//
	public const string APP_NAME = "app_name";
	public const string APP_VERSION = "app_ver";
	public const string PUSH_DEVICE_TOKEN = "push_device_token";
	public const string APP_CHANNEL = "app_channel";
	
	// channel account
	public const string ACC_FMT_91 = "{0}";
	public const string ACC_FMT_WS = "ACC-WS-{0}";
	public const string ACC_FMT_WS_4APPSTORE = "{0}|ACC-WS-{1}";
	public const string ACC_UNBIND_TIP = "ACC-UNBIND-TIP";
	public const string ACC_FMT_UC = "ACC-UC-{0}";
	public const string ACC_FMT_TWM = "{0}|{1}";
	
	
	// channel build dir
	public const string STYPE_WS_ACC = "ws acc";
	public const string STYPE_SAMSUNG = "samsung";
	public const string STYPE_APPSTORE = "appstore";
	public const string STYPE_IOS91 = "ios 91";
	public const string STYPE_JIFENG = "jifeng";
	public const string STYPE_ANZHI = "anzhi";
	public const string STYPE_MM = "mm";
	public const string STYPE_LIANTONG = "liantong";
	public const string STYPE_ANDROID91 = "android 91";
	public const string STYPE_TIANYIKONGJIAN = "tianyikongjian";
	public const string STYPE_ANDROID360 = "android 360";
	public const string STYPE_ANDROIDUC = "android uc";
    public const string STYPE_ANDROID_DANGLE = "android dangle";
    public const string STYPE_ANDROID_CHINATELECOM = "android china telecom";
	public const string STYPE_IOSUC = "ios uc";
	public const string STYPE_IOS_TWM = "ios twm";
	public const string STYPE_ANDROID_TWM = "android twm";
	public const string STYPE_ANDROID_TWM_GOOGLE = "android twm google";
	public const string STYPE_ANDROID_TWM_MATCH = "android twm match";
	public const string STYPE_ANDROID_TWM_SMARKET = "android twm smarket";
	public const string STYPE_ANDROID_TWM_HAMI = "android twm hami";
	public const string STYPE_ANDROID_MI = "android mi";
	public const string STYPE_ANDROID_TT = "android tt";
	public const string STYPE_ANDROID_WANGXUN = "android wangxun";
	public const string STYPE_IOS_AIMING = "ios aiming";
	public const string STYPE_ANDROID_AIMING_TSTORE = "android aiming tstore";
 	public const string STYPE_ANDROID_AIMING_OLLEH = "android aiming olleh";
	public const string STYPE_ANDROID_AIMING_LGU = "android aiming lgu";
	public const string STYPE_ANDROID_AIMING_GOOGLE = "android aiming google";
	public const string STYPE_ANDROID_SNDA = "android snda";
	public const string STYPE_ANDROID_NDUO_MARKET = "android nduo market";
	public const string STYPE_ANDROID_LIQU = "android liqu";
	public const string STYPE_ANDROID_YOUYOU = "android youyou";
	public const string STYPE_ANDROID_SHOUYOU = "android shouyou";
	public const string STYPE_ANDROID_GAME_TT = "android game tt";
    public const string STYPE_ANDROID_GAME_TT1 = "android game tt1";
    public const string STYPE_ANDROID_GAME_TT2 = "android game tt2";
    public const string STYPE_ANDROID_GAME_FLTX = "android game fltx";
    public const string STYPE_ANDROID_GAME_JUHAOWAN = "android game juhaowan";
    public const string STYPE_ANDROID_GAME_JIULINGWANG = "android game jiulingwang";
    public const string STYPE_ANDROID_GAME_JINSHAN = "android game jinshan";
	public const string STYPE_ANDROID_WISTONE = "android wistone";
	

	public const string STYPE_CURRENT = "current_channel";
	public const string STYPE_CUR_NAME = "current_stype";
	public const string PATCH_SERVER_URL = "patch_server_url";
	public const string PKG_TYPE = "pkg_type";
	public const int PLAYER_GOV_PROTECT_LEVEL = 6;
	public const string PROGRESS_BLACKLIST = "blacklist.progress.filter";
	
	
	//   wistone  zhi fu  fang shi
	public const int ANDROID_WISTONE_ALIPAY = 1;
	public const int ANDROID_WISTONE_SHENZHOUFU = 2;
	public const int ANDROID_WISTONE_CANCEL = 3;
	public const int ANDROID_CHANNEL_TYPE_360 = 6;
	public const int ANDROID_CHANNEL_TYPE_UC = 7;
	public const int ANDROID_CHANNEL_TYPE_MI_COIN = 8;
	public const int ANDROID_CHANNEL_TYPE_MI_CARD = 9;
	
	// 
	public const string DebugModel = "debug";
	public const string TestUID = "test_uid";
	public const string TestGateUrl = "test_url";
	public const string MACADDR = "macaddr";
	public const string UDID = "udid";
	public const string ADD_SDCARD_PATH = "android_sdcard_path";
	public const int GOV_SERVER_STATE_FREE = 1;
	public const int GOV_SERVER_STATE_BUSY = 2;
	public const int GOV_SERVER_STATE_FULL = 3;
	
	// twm
	public const string TWM_LOGIN4PAY_STAGE = "login 4 pay stage";
	public const string TWM_LOGIN4PAY_STAGE_ITEM = "pay item"; 
		
// league style ctrl  to use it when building lv up change style
	public  static readonly int[] LEAGUE_STYLE_LEVEL_CHANGE = {0,15,25,35};
	public const int LEAGUE_CREATE_NAME_LEANGTH = 7;
	public const int LEAGUE_CREATE_NAME_MIN = 2;
	public const int LEAGUE_CREATE_NOTICE_LEANGTH = 170;
	public const int LEAGUE_OPPOSE_RANK_ITEM_COUNT = 10;
	// league need back league sence  league id
	public const string LEAGUE_PVP_RETURN_LEAGUE_ID = "league.pvp.need.return.league.id";
}


