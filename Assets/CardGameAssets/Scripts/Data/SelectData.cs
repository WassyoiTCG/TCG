
//+-------------------------------------------
//  ゲーム起動中だけずっと残っているデータ
//+-------------------------------------------

//  なむりあん
enum MENU_TYPE { TOTORIAL, BATTLE, DECK, COLLECTION, OPTION, END };
enum MENU_SPHERE_TYPE { TOTORIAL, BATTLE,NET_BATTLE, DECK_CREATE, COLLECTION,
                        SHOP, OPTION, END };

public enum AI_CARD_GRADE { LOW,MIDDLE,HIGH, RANDOM };

enum CHANGE_LINE_TYPE { BACK,NEXT, END };

public enum NETWORK_TYPE { HOST,CLIENT };


public static class SelectData
{
    public const int DECK_COLLECTCARD_MAX = 12; // (11/08)(TODO)本来はここにいれるものではない

    public static int iDeckCollectLineNo = 0; // デッキ

    public static bool isNetworkBattle;     // ネットワーク対戦フラグ
    public static NETWORK_TYPE networkType; // ネットワークタイプ

    public static int iMenuType = (int)(MENU_TYPE.TOTORIAL);

    public static string cpuPlayerName;

    public static void Initialize()
    {
        isNetworkBattle = false;
    }
}