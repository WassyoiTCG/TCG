using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public enum CardType
{
    Fighter,        // ファイター
    AbilityFighter, // 効果ありファイター
    Joker,          // ジョーカー
    Support,        // スペル
    Connect,        // オナーズクラスは糞
    Intercept       // 速攻魔法
}

public enum CardEventType
{
    Set,        // セット時
    BeforeOpen, // オープン前
    AfterOpen   // オープン後
}

public enum FighterAbilityConditions
{
    NoneLimit,  // 条件なし
    Otakebi,    // 勝利の雄たけび
    Tsumeato,   // 爪痕
    Sousai,     // 相殺時
}

public enum DifferenceRangeType
{
    In,     // x以内で
    Out,    // x以上で
}


public enum AbilityType
{
    GettingPoint,   // ポイント系
    Hand,           // 手札系
    ChangePower,    // パワーの変化
}

public enum AbilityTarget
{
    Me,     // 自分
    You,    // 相手
    Select, // 選択
    All,    // 全員
}


public enum Arithmetic
{
    Addition,       // 足す
    Subtraction,    // 引く
    Multiplication, // かける
    Division,       // 割る
}

public enum Syuzoku
{
    Warrior     = 0,    // 戦士
    Knight      = 1,    // ナイト
    Sorcerer    = 2,    // ソーサラー
    Beast       = 3,    // 獣
    Plant       = 4,    // 植物
    Insect      = 5,    // 昆虫
    Fish        = 6,    // 魚
    Bird        = 7,    // 鳥
    Undead      = 8,    // アンデッド
    Rock        = 9,    // 岩石
    Machine     = 10,   // マシン
    Monster     = 11,   // 魔物
    Dragon      = 12,   // ドラゴン
    Spirit      = 13,   // 精霊
    Angel       = 14,   // 天使
    Fiend       = 15,   // 悪魔
    Master      = 16,   // マスター
}

public enum SortType
{
    // 0～15は種族のenumをそのまま。なので種族のenumをかえたらここも変える
    Power1      = 17,
    Power2      = 18,
    Power3      = 19,
    Power4      = 20,
    Power5      = 21,
    Power6      = 22,
    Power7      = 23,
    Power8      = 24,
    Power9      = 25,
    Power10     = 26,
    Fighter     = 27,
    AbilityFighter = 28,
    Joker       = 29,
    Event       = 30,
    End         = 31
}


public enum Rarelity
{
    C,  // コモン
    UC, // アンコモン
    R,  // レア
    SR, // Sレア
    LR, // レジェンドレア
}

public class FighterCard
{
    public Syuzoku[] syuzokus;   // 種族(複数)
}
public class AbilityFighterCard : FighterCard
{

}

public class JokerCard
{

}


public class EventCard
{

}

public class SupportCard : EventCard
{

}

public class ConnectCard : EventCard
{
}

public class InterceptCard : EventCard
{

}



public class CardData
{
    public int id;              // カードのID
    public CardType cardType;   // カードのタイプ(ストライカーとかイベントとか)
    public string cardName;     // カード名
    public Rarelity rarelity;   // レアリティ
    public string abilityText;  // 効果テキスト
    public string flavorText;   // フレーバーテキスト
    public Texture image;       // 絵
    public int power;           // パワー

    // ★これのうちのどれか1つを持つ
    public FighterCard fighterCard;                 // ファイターカード
    public AbilityFighterCard abilityFighterCard;   // 効果ファイターカード
    public JokerCard jokerCard;                     // ジョーカーカード
    public SupportCard supportCard;                 // イベントカード
    public ConnectCard connectCard;                 // オナーズクラスは糞。何度でも言う。
    public InterceptCard interceptCard;             // インターセプト

    public FighterCard GetFighterCard() { if (cardType == CardType.Fighter) return fighterCard; else if (cardType == CardType.AbilityFighter) return abilityFighterCard; else return null; }
    public EventCard GetEventCard() { if (cardType == CardType.Support) return supportCard; else if (cardType == CardType.Connect) return connectCard; else if (cardType == CardType.Intercept) return interceptCard; else return null; }
    public bool isStrikerCard() { return (cardType == CardType.Fighter || cardType == CardType.AbilityFighter || cardType == CardType.Joker); }
}

public static class CardDataBase
{
    static CardData[] cardDatas;

    public static CardData[] GetCardList() { return cardDatas; }

    public static CardData GetCardData(int id)
    {
        //// 画像データ読み込んでいないなら読み込む
        //if (!m_aCardDatas[id].m_image)
        //{
        //    var texture = PngLoader.LoadPNG(path + "/image.png");
        //    if (texture) m_aCardDatas[id].m_image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //}

        return cardDatas[id];
    }

    /*
      BitVector32 myBV = new BitVector32( 0 );

      // Creates masks to isolate each of the first five bit flags.
      int myBit1 = BitVector32.CreateMask();
      int myBit2 = BitVector32.CreateMask( myBit1 );
      int myBit3 = BitVector32.CreateMask( myBit2 );
      int myBit4 = BitVector32.CreateMask( myBit3 );
      int myBit5 = BitVector32.CreateMask( myBit4 );

      // Sets the alternating bits to TRUE.
      Console.WriteLine( "Setting alternating bits to TRUE:" );
      Console.WriteLine( "   Initial:         {0}", myBV.ToString() );
      myBV[myBit1] = true;
      Console.WriteLine( "   myBit1 = TRUE:   {0}", myBV.ToString() );
      myBV[myBit3] = true;
      Console.WriteLine( "   myBit3 = TRUE:   {0}", myBV.ToString() );
      myBV[myBit5] = true;
      Console.WriteLine( "   myBit5 = TRUE:   {0}", myBV.ToString() );
     */
    static BitVector32 sortBitMask;
    static int[] bitMaskIndices;

    public static string[] SyuzokuString = new string[] { "戦士", "ナイト", "ソーサラー", "獣", "植物", "昆虫", "魚", "鳥", "アンデッド", "岩石", "マシン", "魔物", "ドラゴン", "精霊", "天使", "悪魔", "マスター" };


    public static void Start()
    {
        // ビットマスク初期化
        sortBitMask = new BitVector32(0);
        int numSortType = (int)SortType.End;
        bitMaskIndices = new int[numSortType];
        // ビットマスクのインデックスを
        bitMaskIndices[0] = BitVector32.CreateMask();
        for (int i = 1; i < numSortType; i++)
        {
            bitMaskIndices[i] = BitVector32.CreateMask(bitMaskIndices[i - 1]);
        }

        // カード情報読み込み
        LoadCards();
    }

    public static CardData[] GetSortCards(ushort wSortOption)
    {
        //// ラムダ
        //Func<ushort, SortType, bool> CheckBit = (bit, sortType) =>
        //{
        //    return (((ushort)sortType & bit) == bit);
        //};

        // 格納用
        List<CardData> ret = new List<CardData>();

        // ソートONになってるビットを列挙する
        List<int> sortOnBitIndices = new List<int>();
        for (int i = 0; i < bitMaskIndices.Length; i++)
        {
            if (sortBitMask[bitMaskIndices[i]]) sortOnBitIndices.Add(i);
        }

        // ソート
        foreach (CardData card in cardDatas)
        {
            // ソートなしなら無条件で入れる
            if (sortOnBitIndices.Count == 0)
            {
                ret.Add(card);
                continue;
            }

            // ジョーカーなら
            if (card.cardType == CardType.Joker)
            {
                // ジョーカーがソートONになってるか検索。ONになってたら入れる
                foreach (int i in sortOnBitIndices)
                    if (i == (int)SortType.Joker) ret.Add(card);
                continue;
            }

            // イベントなら
            //if (card.cardType == CardType.Event)
            //{
            //    // イベントがソートONになってるか検索。ONになってたら入れる
            //    foreach (int i in sortOnBitIndices)
            //        if (i == (int)SortType.Event) ret.Add(card);
            //    continue;
            //}

            // それ以外だとファイターしか残っていないはず
            FighterCard fighter = card.GetFighterCard();
            bool isOK = false;

            // ソートオンになってるやつで回す
            foreach (int i in sortOnBitIndices)
            {
                // 種族ソート
                if (i <= Enum.GetValues(typeof(Syuzoku)).Length)
                {
                    foreach (Syuzoku fighterSyuzoku in fighter.syuzokus)
                        if (i == (int)fighterSyuzoku) isOK = true;
                }
            }

                // パワー系ソート
            //    else if (CheckBit(wSortOption, SortType.Power1) && fighter.power == 1) isOK = true;
            //else if (CheckBit(wSortOption, SortType.Power2) && fighter.power == 2) isOK = true;
            //else if (CheckBit(wSortOption, SortType.Power3) && fighter.power == 3) isOK = true;
            //else if (CheckBit(wSortOption, SortType.Power4) && fighter.power == 4) isOK = true;
            //else if (CheckBit(wSortOption, SortType.Power5) && fighter.power == 5) isOK = true;
            //else if (CheckBit(wSortOption, SortType.Power6) && fighter.power == 6) isOK = true;
            //else if (CheckBit(wSortOption, SortType.Power7) && fighter.power == 7) isOK = true;
            //else if (CheckBit(wSortOption, SortType.Power8) && fighter.power == 8) isOK = true;
            //else if (CheckBit(wSortOption, SortType.Power9) && fighter.power == 9) isOK = true;
            //else if (CheckBit(wSortOption, SortType.Power10) && fighter.power == 10) isOK = true;

            if (isOK) ret.Add(card);

        }
        return ret.ToArray();
    }


    static void LoadCards()
    {
        // フォルダー列挙
        var directory = System.IO.Directory.GetParent(Application.dataPath).ToString() + "/CardDatas";
        string[] subFolders = oulFile.EnumDirectory(directory);

        // 曲の分の配列確保
        cardDatas = new CardData[subFolders.Length];

        for (int i = 0; i < subFolders.Length; i++)
        {
            cardDatas[i] = new CardData();
            cardDatas[i].id = i;

            var folderName = subFolders[i];

            // フォルダー名=保存
            //m_aCardDatas[i].folderName = folderName;

            var path = directory + "/" + folderName;

            // テキスト情報読み込み
            {
                //Debug.Log(path + "/info.txt");

                //oulFile.OutPutLog(Application.dataPath + "/log.txt", "テキストオープン\r\n");

                //oulFile.OutPutLog(path + "/log.txt", "キテルグマ");

                var loader = new TextLoader();
                loader.LoadText(path + "/info.txt");

                //oulFile.OutPutLog(Application.dataPath + "/log.txt", "テキストロード開始\r\n");

                // 読み飛ばし用
                var skip = "";

                // カードタイプ(ファイターor効果ファイターorイベント)
                skip = loader.ReadString();
                var cardType = (CardType)loader.ReadInt();
                switch (cardType)
                {
                    case CardType.Fighter:
                        cardDatas[i].fighterCard = new FighterCard();
                        break;

                    case CardType.AbilityFighter:
                        cardDatas[i].abilityFighterCard = new AbilityFighterCard();
                        break;

                    case CardType.Joker:
                        cardDatas[i].jokerCard = new JokerCard();
                        break;

                    case CardType.Support:
                        cardDatas[i].supportCard = new SupportCard();
                        break;

                    case CardType.Connect:
                        cardDatas[i].connectCard = new ConnectCard();
                        break;

                    case CardType.Intercept:
                        cardDatas[i].interceptCard = new InterceptCard();
                        break;

                    default:
                        ExceptionMessage.MessageBox("text error.", "card type error." + folderName);
                        break;
                }

                cardDatas[i].cardType = cardType;

                // カード名
                skip = loader.ReadString();
                cardDatas[i].cardName = loader.ReadString();

                // レア度
                skip = loader.ReadString();
                cardDatas[i].rarelity = (Rarelity)loader.ReadInt();

                // 効果テキスト
                skip = loader.ReadString();
                while (true)
                {
                    var str = loader.ReadLine();
                    if (str == "") continue;
                    if (str.ToCharArray()[0] == '[') break;

                    cardDatas[i].abilityText += str;
                }

                // フレーバーテキスト
                skip = loader.ReadString();
                while (true)
                {
                    var str = loader.ReadLine();
                    if (str == "") continue;
                    if (str.ToCharArray()[0] == '[') break;

                    cardDatas[i].flavorText += str;
                }

                // タイプによって分岐
                switch (cardType)
                {
                    case CardType.Fighter:
                        {
                            // 種族
                            //skip = loader.ReadLine();
                            // 種族の個数
                            int numSyuzoku = loader.ReadInt();
                            cardDatas[i].fighterCard.syuzokus = new Syuzoku[numSyuzoku];
                            // 個数に応じて読み込み
                            for (int j = 0; j < numSyuzoku; j++)
                                cardDatas[i].fighterCard.syuzokus[j] = (Syuzoku)loader.ReadInt();
                            // パワー
                            skip = loader.ReadString();
                            cardDatas[i].power = loader.ReadInt();
                        }
                        break;

                    case CardType.AbilityFighter:
                        {
                            // 種族
                            //skip = loader.ReadLine();
                            // 種族の個数
                            int numSyuzoku = loader.ReadInt();
                            cardDatas[i].fighterCard.syuzokus = new Syuzoku[numSyuzoku];
                            // 個数に応じて読み込み
                            for (int j = 0; j < numSyuzoku; j++)
                                cardDatas[i].fighterCard.syuzokus[j] = (Syuzoku)loader.ReadInt();
                            // パワー
                            skip = loader.ReadString();
                            cardDatas[i].power = loader.ReadInt();
                            // 効果
                        }
                        break;

                    case CardType.Joker:
                        // 効果
                        break;

                    case CardType.Support:
                    case CardType.Connect:
                    case CardType.Intercept:
                        {
                            // イベントタイプ
                            //skip = loader.ReadString();
                            // イベントの個数
                            //int numEvent = loader.ReadInt();
                            //cardDatas[i].eventCard.eventTypes = new CardEventType[numEvent];
                            //// 個数に応じて読み込み
                            //for (int j = 0; j < numEvent; j++)
                            //    cardDatas[i].eventCard.eventTypes[j] = (CardEventType)loader.ReadInt();
                            // 効果
                        }
                        break;
                }

            }

            // キャラ画像
            var texture = PngLoader.LoadPNG(path + "/image.png");
            if (texture) cardDatas[i].image = texture;  /*Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));*/ 
        }
    }

    //static Syuzoku[] BitToEnumArraySyuzoku(ushort wSyuzoku)
    //{
    //    // 格納用
    //    Queue<Syuzoku> queue = new Queue<Syuzoku>();
    //    // 種族格納
    //    foreach (Syuzoku eSyuzoku in System.Enum.GetValues(typeof(Syuzoku)))
    //    {
    //        if (((byte)eSyuzoku & wSyuzoku) == wSyuzoku) queue.Enqueue(eSyuzoku);
    //    }
    //    return queue.ToArray();
    //}
    //static EventType[] BitToEnumArrayEventType(byte byEventType)
    //{
    //    // 格納用
    //    Queue<EventType> queue = new Queue<EventType>();
    //    // イベント格納
    //    foreach (EventType eEventType in System.Enum.GetValues(typeof(EventType)))
    //    {
    //        if (((byte)eEventType & byEventType) == byEventType) queue.Enqueue(eEventType);
    //    }
    //    return queue.ToArray();
    //}


    static public void SaveRecord()
    {
        //if (!saveFlag) return;
        ////Debug.Log("キテルグマ");

        //for (int i = 0; i < musicDatas.Length; i++)
        //{
        //    // パス
        //    var path = Application.dataPath + "/Musics/" + musicDatas[i].folderName;

        //    // バイナリオープン
        //    using (FileStream file = new FileStream(path + "/record.bin", FileMode.Open, FileAccess.Write))
        //    using (BinaryWriter writer = new BinaryWriter(file))
        //    {
        //        foreach (int dif in System.Enum.GetValues(typeof(Difficulty)))
        //        {
        //            var record = musicDatas[i].difficulDatas[dif].record;
        //            writer.Write(BitConverter.GetBytes(record.hiScore), 0, 4);
        //            writer.Write(BitConverter.GetBytes(record.byClearLamp), 0, 1);
        //            writer.Write(BitConverter.GetBytes(record.maxCombp), 0, 2);
        //            writer.Write(BitConverter.GetBytes(record.playCount), 0, 2);
        //        }
        //    }
        //}
    }
}
