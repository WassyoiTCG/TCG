using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

// (11/11) UnionでSupport・Connect・InterceptをEventでまとめたい。
public enum CardType
{
    Fighter,        // ファイター
    AbilityFighter, // 効果ありファイター
    Joker,          // ジョーカー
    Support,        // スペル
    Connect,        // オナーズクラスは糞
    Intercept       // 速攻魔法
}

//public enum CardEventType
//{
//    Set,        // セット時
//    BeforeOpen, // オープン前
//    AfterOpen   // オープン後
//}

public enum AbilityTriggerType
{
    BeforeBattle,   // バトル前
    Kousenji,       // 交戦時
    AfterBattle,    // バトル後(勝利の雄たけびとか)
    EventCard       // イベントカード
}

public enum CostType
{
    NoneLimit,  // 無条件
    Otakebi,    // 勝利の雄たけび
    Tsumeato,   // 爪痕
    Sousai,     // 相殺時
    Burst,      // バースト
    Treasure,   // 宝箱
    Nakama,     // 仲間と共に
    Kisikaisei, // 起死回生の一手
    YouStrikerPower,    // 相手ストライカーのパワーがx
    PowerStriker,       // パワーxのストライカーが手札にあるか
    Turn,       // ターン条件
}


public enum DifferenceRangeType
{
    NoneLimit,  // 無条件
    In,         // x以内で
    Out,        // x以上で
}


public enum AbilityType
{
    None,               // 効果なし(ダミー)
    GettingPoint,       // ポイント系
    ChangePower,        // パワーの変化
    CardMove,           // 手札系
    LimitPower,         // 出すストライカーのパワー制限
    Lose = 9,           // 無条件敗北
    Victory = 10,       // 無条件勝利
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
    public CardAbilityData[] abilityDatas;
}

public class JokerCard
{
    public CardAbilityData[] abilityDatas; // [NEW] 追加
}

public class EventCard
{
    public CardAbilityData[] abilityDatas;
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

// (11/14)(TODO)-1はカードが存在しない
public enum IDType
{
    NONE = -1,      //  無し
    RANDOM = -2,    // 無造作フラグ
}


public class CardData
{
    public int id;              // カードのID
    public CardType cardType;   // カードのタイプ(ストライカーとかイベントとか)
    public string cardName;     // カード名
    public Rarelity rarelity;   // レアリティ
    public string abilityText;  // 効果テキスト
    public string flavorText;   // フレーバーテキスト
    public Sprite image;        // 絵
    public int power;           // パワー

    // ★これのうちのどれか1つを持つ
    public FighterCard fighterCard;                 // ファイターカード
    public AbilityFighterCard abilityFighterCard;   // 効果ファイターカード
    public JokerCard jokerCard;                     // ジョーカーカード
    public SupportCard supportCard;                 // イベントカード
    public ConnectCard connectCard;                 // オナーズクラスは糞。何度でも言う。
    public InterceptCard interceptCard;             // インターセプト

    public FighterCard GetFighterCard() { if (cardType == CardType.Fighter) return fighterCard; else if (cardType == CardType.AbilityFighter) return abilityFighterCard; else return null; }
    public EventCard GetEventCard()
    {
        if (cardType == CardType.Support) return supportCard;
        else if (cardType == CardType.Connect) return connectCard;
        else if (cardType == CardType.Intercept) return interceptCard;
        else { Debug.LogError("Eventじゃない"); return null; };

    }
    public JokerCard GetJokerCard()
    {
        if (cardType == CardType.Joker) return jokerCard; // [NEW]JOKERもイベント扱い
        else { Debug.LogError("Jokerじゃない");  return null; };

    }

    public CardAbilityData[] GetAbilityDatas()
    {
        switch (cardType)
        {
            case CardType.AbilityFighter:
                return abilityFighterCard.abilityDatas;
            case CardType.Support:
                return supportCard.abilityDatas;
            case CardType.Connect:
                return connectCard.abilityDatas;
            case CardType.Intercept:
                return interceptCard.abilityDatas;
        }
        return null;
    }
    public bool isStrikerCard() { return (cardType == CardType.Fighter || cardType == CardType.AbilityFighter /*|| cardType == CardType.Joker*/); }
    public bool isEventCard() { return (cardType == CardType.Support || cardType == CardType.Connect || cardType == CardType.Intercept); }
}

public enum CardMoveFlag
{
    Draw,
    ShowDraw,
}

public static class CardDataBase
{
    static CardData[] cardDatas;
    static CardData[] sortedCardDatas;
    static CardData missingCard;    // 欠番用のカード

    public static CardData[] GetCardList() { return cardDatas; }
    public static CardData[] GetDeckSortedCardData()
    {
        // デッキセレクト用のソートを作成
        if (sortedCardDatas == null)
        {
            CreateDeckSelectSortCard();
        }
        return sortedCardDatas;
    }

    public static CardData GetCardData(int id)
    {
        //// 画像データ読み込んでいないなら読み込む
        //if (!m_aCardDatas[id].m_image)
        //{
        //    var texture = PngLoader.LoadPNG(path + "/image.png");
        //    if (texture) m_aCardDatas[id].m_image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //}

        // けつばん
        if(id < 0 || id >= cardDatas.Length)
        {
            return missingCard;
        }
        return cardDatas[id];
    }

    public static CardData GetSortedCardData(int id)
    {
        // けつばん
        if (id < 0 || id >= cardDatas.Length)
        {
            return missingCard;
        }
        // デッキセレクト用のソートを作成
        if(sortedCardDatas == null)
        {
            CreateDeckSelectSortCard();
        }
        return sortedCardDatas[id];
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

    public static string[] SyuzokuString = new string[] { "戦士", "ナイト", "ソーサラー", "獣", "植物", "虫", "魚", "鳥", "アンデッド", "岩石", "マシン", "魔物", "ドラゴン", "精霊", "天使", "悪魔", "マスター" };

    static bool isInit = false;

    public static void Start()
    {
        if (isInit) return;
        isInit = true;

        missingCard = new CardData();
        missingCard.id = -1;
        missingCard.cardType = CardType.Joker;
        missingCard.power = 334;
        missingCard.abilityText = "効果:このメッセージが表示されたら製作者は消されてしまう。";
        missingCard.flavorText = "宝箱から全てが始まった。";
        missingCard.cardName = "欠番ニキ";
        missingCard.image = Resources.Load<Sprite>("Sprites/MissingImage")/*.texture*/;

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
                string skip;

                // カードタイプ(ファイターor効果ファイターorイベント)
                skip = loader.ReadString();
                Debug.Assert(skip == "[TYPE]", "カードID" + i  +"のテキストずれてるクマ");
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
                Debug.Assert(skip == "[NAME]", "カードID" + i + "のテキストずれてるクマ");
                cardDatas[i].cardName = loader.ReadString();

                // レア度
                skip = loader.ReadString();
                Debug.Assert(skip == "[RARE]", "カードID" + i + "のテキストずれてるクマ");
                cardDatas[i].rarelity = (Rarelity)loader.ReadInt();

                // 効果テキスト
                skip = loader.ReadString();
                Debug.Assert(skip == "[A_TEXT]", "カードID" + i + "のテキストずれてるクマ");
                while (true)
                {
                    var str = loader.ReadLine();
                    if (str == "") continue;
                    if (str.ToCharArray()[0] == '[') break;

                    cardDatas[i].abilityText += str + "\r\n";
                }

                // フレーバーテキスト
                skip = loader.ReadString();
                while (true)
                {
                    var str = loader.ReadLine();
                    if (str == "") continue;
                    if (str.ToCharArray()[0] == '[') break;

                    cardDatas[i].flavorText += str + "\r\n";
                }

                // パワー
                //skip = loader.ReadString();
                cardDatas[i].power = loader.ReadInt();

                // タイプによって分岐
                switch (cardType)
                {
                    case CardType.Fighter:
                        {
                            // 種族
                            skip = loader.ReadLine();
                            // 種族の個数
                            int numSyuzoku = loader.ReadInt();
                            cardDatas[i].fighterCard.syuzokus = new Syuzoku[numSyuzoku];
                            // 個数に応じて読み込み
                            for (int j = 0; j < numSyuzoku; j++)
                                cardDatas[i].fighterCard.syuzokus[j] = (Syuzoku)loader.ReadInt();
                        }
                        break;

                    case CardType.AbilityFighter:
                        {
                            var abilityFighterCard = cardDatas[i].abilityFighterCard;

                            // 種族
                            skip = loader.ReadLine();
                            // 種族の個数
                            int numSyuzoku = loader.ReadInt();
                            abilityFighterCard.syuzokus = new Syuzoku[numSyuzoku];
                            // 個数に応じて読み込み
                            for (int j = 0; j < numSyuzoku; j++)
                                abilityFighterCard.syuzokus[j] = (Syuzoku)loader.ReadInt();

                            // 効果読み込み
                            skip = loader.ReadString();
                            var numAbility = loader.ReadInt();
                            abilityFighterCard.abilityDatas = new CardAbilityData[numAbility];
                            for (int j = 0; j < numAbility; j++)
                            {
                                abilityFighterCard.abilityDatas[j] = new CardAbilityData();
                                LoadAbility(loader, out abilityFighterCard.abilityDatas[j]);
                            }

                        }
                        break;

                    case CardType.Joker:
                        // 効果
                        {
                            var jokerCard = cardDatas[i].GetJokerCard();

                            // [NEW] 今は種族考慮せず
                            //// 種族
                            //skip = loader.ReadLine();
                            //// 種族の個数
                            //int numSyuzoku = loader.ReadInt();
                            //jokerCard.syuzokus = new Syuzoku[numSyuzoku];
                            //// 個数に応じて読み込み
                            //for (int j = 0; j < numSyuzoku; j++)
                            //    abilityFighterCard.syuzokus[j] = (Syuzoku)loader.ReadInt();


                            // 効果
                            var numAbility = loader.ReadInt();
                            jokerCard.abilityDatas = new CardAbilityData[numAbility];
                            for (int j = 0; j < numAbility; j++)
                            {
                                jokerCard.abilityDatas[j] = new CardAbilityData();
                                LoadAbility(loader, out jokerCard.abilityDatas[j]);
                            }
                        }
                        break;

                    case CardType.Support:
                    case CardType.Connect:
                    case CardType.Intercept:
                        {
                            var eventCard = cardDatas[i].GetEventCard();

                            // 効果
                            var numAbility = loader.ReadInt();
                            eventCard.abilityDatas = new CardAbilityData[numAbility];
                            for (int j = 0; j < numAbility; j++)
                            {
                                eventCard.abilityDatas[j] = new CardAbilityData();
                                LoadAbility(loader, out eventCard.abilityDatas[j]);
                            }
                        }
                        break;
                }

            }

            // キャラ画像
            var texture = PngLoader.LoadPNG(path + "/image.png");
            if (texture) cardDatas[i].image = /*texture;*/  Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            //Debug.Log("カードID" + i + "番の読み込みが完了");
        }
    }

    static void CreateDeckSelectSortCard()
    {
        // ストライカーパワー順→イベント順にソートする
        List<CardData> bufAll = new List<CardData>();
        Queue<CardData> bufPower1 = new Queue<CardData>();
        Queue<CardData> bufPower2 = new Queue<CardData>();
        Queue<CardData> bufPower3 = new Queue<CardData>();
        Queue<CardData> bufPower4 = new Queue<CardData>();
        Queue<CardData> bufPower5 = new Queue<CardData>();
        Queue<CardData> bufPower6 = new Queue<CardData>();
        Queue<CardData> bufPower7 = new Queue<CardData>();
        Queue<CardData> bufPower8 = new Queue<CardData>();
        Queue<CardData> bufPower9 = new Queue<CardData>();
        Queue<CardData> bufPower10 = new Queue<CardData>();
        Queue<CardData> bufJoker = new Queue<CardData>();
        Queue<CardData> bufSupport = new Queue<CardData>();
        Queue<CardData> bufIntercept = new Queue<CardData>();
        //buf.AddRange(cardDatas);
        //buf.Sort(SortFunction);
        for (int i = 0; i < cardDatas.Length; i++)
        {
            // イベントじゃない
            if (!cardDatas[i].isEventCard())
            {
                // ジョーカーカード
                if (cardDatas[i].cardType == CardType.Joker)
                {
                    bufJoker.Enqueue(cardDatas[i]);
                }
                // ストライカーカード
                else
                {
                    // パワーで分岐
                    switch (cardDatas[i].power)
                    {
                        case 1:
                            bufPower1.Enqueue(cardDatas[i]);
                            break;
                        case 2:
                            bufPower2.Enqueue(cardDatas[i]);
                            break;
                        case 3:
                            bufPower3.Enqueue(cardDatas[i]);
                            break;
                        case 4:
                            bufPower4.Enqueue(cardDatas[i]);
                            break;
                        case 5:
                            bufPower5.Enqueue(cardDatas[i]);
                            break;
                        case 6:
                            bufPower6.Enqueue(cardDatas[i]);
                            break;
                        case 7:
                            bufPower7.Enqueue(cardDatas[i]);
                            break;
                        case 8:
                            bufPower8.Enqueue(cardDatas[i]);
                            break;
                        case 9:
                            bufPower9.Enqueue(cardDatas[i]);
                            break;
                        case 10:
                            bufPower10.Enqueue(cardDatas[i]);
                            break;
                    }

                }
            }
            else
            {
                // イベントカード追加
                if (cardDatas[i].cardType == CardType.Support)
                    bufSupport.Enqueue(cardDatas[i]);
                if (cardDatas[i].cardType == CardType.Intercept)
                    bufIntercept.Enqueue(cardDatas[i]);
            }
        }
        // 順番にリストに格納
        bufAll.AddRange(bufPower1);
        bufAll.AddRange(bufPower2);
        bufAll.AddRange(bufPower3);
        bufAll.AddRange(bufPower4);
        bufAll.AddRange(bufPower5);
        bufAll.AddRange(bufPower6);
        bufAll.AddRange(bufPower7);
        bufAll.AddRange(bufPower8);
        bufAll.AddRange(bufPower9);
        bufAll.AddRange(bufPower10);
        bufAll.AddRange(bufJoker);
        bufAll.AddRange(bufSupport);
        bufAll.AddRange(bufIntercept);

        //for (int i = 0; i < buf1.Count - 1; i++)
        //{
        //    for (int j = i + 1; j < buf1.Count; j++)
        //    {
        //        if (buf[i].power >= buf[j].power)
        //        {
        //            CardData tmp = buf[i];
        //            buf[i] = buf[j];
        //            buf[j] = tmp;
        //            //break;
        //        }
        //    }

        //}
        //for (int i = buf.Count - 1; i >= 0; i--)
        //{
        //    if (buf[i].isEventCard())
        //    {
        //        // 終端か、イベントカードに当たるまでまわす
        //        for (int j = i + 1; j < buf.Count -1; j++)
        //        {
        //            if (buf[j].isEventCard()) break;

        //            CardData tmp = buf[i];
        //            buf[i] = buf[j];
        //            buf[j] = tmp;
        //            break;
        //        }
        //    }
        //}

        sortedCardDatas = bufAll.ToArray();
    }

    //private static int SortFunction(CardData a, CardData b)
    //{
    //    /*
    //      •負の値 ： インスタンス a は、並べ替え順序において b の前になります。
    //      •0　： インスタンス a は、並べ替え順序で、b と同じ位置に出現します。
    //      •正の値 ： インスタンス a は、並べ替え順序において b の後になります。
    //     */

    //    if (a.isEventCard())
    //    {
    //        return (b.isEventCard()) ? 0 : 1;
    //    }
    //    return a.power - b.power;
    //}

    static bool LoadAbility(TextLoader loader, out CardAbilityData abilityData)
    {
        // 効果
        abilityData = new CardAbilityData();

        /* 1行目 */
        // 発動タイミング
        abilityData.abilityTriggerType = (AbilityTriggerType)loader.ReadInt();

        // 必要ライフポイント
        abilityData.lifeCost = loader.ReadInt();
        // 発動条件
        abilityData.costType = (CostType)loader.ReadInt();
        switch (abilityData.costType)
        {
            case CostType.NoneLimit:
                abilityData.cost = new Cost.NoneLimit();
                break;
            case CostType.Otakebi:
                abilityData.cost = new Cost.Otakebi();
                break;
            case CostType.Tsumeato:
                abilityData.cost = new Cost.Tsumeato();
                break;
            case CostType.Sousai:
                //abilityFighterCard.abilityData.cost = new Cost.();
                break;
            case CostType.Burst:
                abilityData.cost = new Cost.Burst();
                break;
            case CostType.Treasure:
                abilityData.cost = new Cost.Treasure();
                break;
            case CostType.Nakama:
                abilityData.cost = new Cost.Nakama();
                break;
            case CostType.Kisikaisei:
                abilityData.cost = new Cost.Kisikaisei();
                break;
            case CostType.YouStrikerPower:
                abilityData.cost = new Cost.YouStrikerPower();
                break;
            case CostType.PowerStriker:
                abilityData.cost = new Cost.PowerStriker();
                break;
            case CostType.Turn:
                abilityData.cost = new Cost.Turn();
                break;
            default:
                Debug.LogWarning("超すとおかしいクマ" + (int)abilityData.costType);
                return false;
        }
        // 汎用数値
        abilityData.c_value0 = loader.ReadInt();
        abilityData.c_value1 = loader.ReadInt();
        abilityData.c_value2 = loader.ReadInt();
        abilityData.c_value3 = loader.ReadInt();

        /* 2行目 */
        // 効果個数
        abilityData.numSkill = loader.ReadInt();
        abilityData.skillDatas = new CardAbilityData.SkillData[abilityData.numSkill];

        //Debug.Log("abilityTriggerType:" + abilityData.abilityTriggerType);
        //Debug.Log("lifeCost:" + abilityData.lifeCost);
        //Debug.Log("costType:" + abilityData.costType);
        //Debug.Log("s_iValues:" + abilityData.c_value0 + "," + abilityData.c_value1);
        //Debug.Log("numSkill:" + abilityData.numSkill);

        /* 3行目 */
        // 効果のタイプ
        for (int i = 0; i < abilityData.numSkill; i++)
        {
            abilityData.skillDatas[i] = new CardAbilityData.SkillData();
            var skillData = abilityData.skillDatas[i];

            skillData.nextSkillNumber = loader.ReadInt();
            skillData.abilityType = (AbilityType)loader.ReadInt();
            switch (skillData.abilityType)
            {
                case AbilityType.None:
                    skillData.skill = new Skill.None();
                    break;
                case AbilityType.GettingPoint:
                    skillData.skill = new Skill.Score();
                    break;
                case AbilityType.ChangePower:
                    skillData.skill = new Skill.Power();
                    break;
                case AbilityType.CardMove:
                    skillData.skill = new Skill.CardMove();
                    break;
                case AbilityType.LimitPower:
                    skillData.skill = new Skill.LimitPower();
                    break;
                case AbilityType.Lose:
                    Debug.LogWarning("未実装: 無条件敗北");
                    break;
                case AbilityType.Victory:
                    Debug.LogWarning("未実装: 無条件勝利");
                    break;
                default:
                    Debug.LogWarning("アビリティ実装されてない");
                    break;
            }
            // 汎用数値
            skillData.s_iValue0 = loader.ReadInt();
            skillData.s_iValue1 = loader.ReadInt();
            skillData.s_iValue2 = loader.ReadInt();
            skillData.s_iValue3 = loader.ReadInt();
            skillData.s_iValue4 = loader.ReadInt();
            skillData.s_iValue5 = loader.ReadInt();
            skillData.s_iValue6 = loader.ReadInt();
            skillData.s_iValue7 = loader.ReadInt();
            skillData.s_iValue8 = loader.ReadInt();
            skillData.s_iValue9 = loader.ReadInt();

            // 汎用文字列値の個数
            int numStringValue = loader.ReadInt();

            // これはひどい
            for (int j = 0; j < numStringValue; j++)
            {
                switch (j)
                {
                    case 0:
                        skillData.s_sValue0 = loader.ReadDoubleQuotation();
                        break;
                    case 1:
                        skillData.s_sValue1 = loader.ReadDoubleQuotation();
                        break;
                    case 2:
                        skillData.s_sValue2 = loader.ReadDoubleQuotation();
                        break;
                    default:
                        Debug.LogWarning("3個以上文字列型の値を読み込もうとしている(現状の上限)");
                        break;
                }

            }

            //Debug.Log("Skill" + i + ".nextSkillNumber:" + skillData.nextSkillNumber);
            //Debug.Log("Skill" + i + ".abilityType:" + skillData.abilityType);
            //Debug.Log("Skill" + i + ".s_iValues:" +
            //     skillData.s_iValue0 + "," +
            //     skillData.s_iValue1 + "," +
            //     skillData.s_iValue2 + "," +
            //     skillData.s_iValue3 + "," +
            //     skillData.s_iValue4 + "," +
            //     skillData.s_iValue5 + "," +
            //     skillData.s_iValue6 + "," +
            //     skillData.s_iValue7 + "," +
            //     skillData.s_iValue8 + "," +
            //     skillData.s_iValue9);
        }

        return true;

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

    //static void LoadAbility(TextLoader loader)
    //{
    //    // 効果
    //    var skip = loader.ReadString();

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
