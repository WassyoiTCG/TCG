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

public enum FighterAbilityConditions
{
    NoneLimit,  // 無条件
    Otakebi,    // 勝利の雄たけび
    Tsumeato,   // 爪痕
    Sousai,     // 相殺時
}

public enum DifferenceRangeType
{
    NoneLimit,  // 無条件
    In,         // x以内で
    Out,        // x以上で
}


public enum AbilityType
{
    GettingPoint,       // ポイント系
    ChangePower,        // パワーの変化
    Hand,               // 手札系
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

namespace Jouken
{
    public abstract class Base
    {
        protected AbilitiManager abilityManager;

        public Base(AbilitiManager owner) { abilityManager = owner; }
        public abstract bool CheckJouken();
    }

    // 条件なし
    public class NoneLimit : Base
    {
        public NoneLimit(AbilitiManager owner) : base(owner) { }
        public override bool CheckJouken() { return true; }
    }

    public class Otakebi : Base
    {
        DifferenceRangeType range;  // 条件タイプ
        int difference;             // 条件の差

        public Otakebi(AbilitiManager owner) : base(owner) { }
        public override bool CheckJouken()
        {
            int myPower = 0 /*abilityManager.playerManager.GetPlayer(abilityManager.playerID).*/;
            int youPower = 0;
            bool ok = (myPower > youPower);    // 勝ってたらtrue
            if (!ok) return false;

            switch (range)
            {
                case DifferenceRangeType.NoneLimit:
                    return true;
                case DifferenceRangeType.In:
                    return (myPower - youPower <= difference);
                case DifferenceRangeType.Out:
                    return (myPower - youPower >= difference);
                default:
                    Debug.LogError("勝利の雄たけびのswitch文でエラークマ");
                    return false;
            }
        }
    }

    public class Tsumeato : Base
    {
        DifferenceRangeType range;  // 条件タイプ
        int difference;             // 条件の差

        public Tsumeato(AbilitiManager owner) : base(owner) { }
        public override bool CheckJouken()
        {
            int myPower = 0;
            int youPower = 0;
            bool ok = (myPower < youPower);    // 負けてたらtrue
            if (!ok) return false;

            switch (range)
            {
                case DifferenceRangeType.NoneLimit:
                    return true;
                case DifferenceRangeType.In:
                    return (youPower - myPower <= difference);
                case DifferenceRangeType.Out:
                    return (youPower - myPower >= difference);
                default:
                    Debug.LogError("爪痕のswitch文でエラークマ");
                    return false;
            }
        }
    }

    public class Sousai : Base
    {
        public Sousai(AbilitiManager owner) : base(owner) { }

        public override bool CheckJouken()
        {
            int myPower = 0;
            int youPower = 0;
            return (myPower == youPower);
        }
    }
}

namespace Kouka
{
    public abstract class Base
    {
        protected AbilitiManager abilityManager;

        public Base(AbilitiManager owner) { abilityManager = owner; }

        public abstract void Action();
    }

    public abstract class ValueChange : Base
    {
        protected int value;                   // 変化の値(パワーだったり、ポイントだったり)
        //protected Arithmetic arithmetic;       // 足す引くかける割る

        protected Func<int, int, int> enzankun;          // 演算ラムダ式

        public ValueChange(AbilitiManager owner, int value, Arithmetic arithmetic) : base(owner)
        {
            this.value = value;
            //this.arithmetic = arithmetic;

            // 演算タイプに応じてラムダ関数登録
            switch (arithmetic)
            {
                case Arithmetic.Addition:
                    enzankun = (a, b) => { return a + b; };
                    break;
                case Arithmetic.Subtraction:
                    enzankun = (a, b) => { return Mathf.Max(a - b, 0); };
                    break;
                case Arithmetic.Multiplication:
                    enzankun = (a, b) => { return a * b; };
                    break;
                case Arithmetic.Division:
                    enzankun = (a, b) => { return (b != 0) ? a / b : 0; };
                    break;
                default:
                    Debug.LogWarning("演算enumでエラークマ");
                    break;
            }
        }

        public override abstract void Action();
    }

    // スコア系
    public class Score : ValueChange
    {
        public Score(AbilitiManager owner, int value, Arithmetic arithmetic) : base(owner, value, arithmetic) { }

        public override void Action()
        {
            var uiManager = abilityManager.playerManager.uiManager;
            foreach(Player player in abilityManager.targetPlayers)
            {
                // 現在のスコアを取ってきて
                var score = uiManager.GetScore(player.isMyPlayer);
                // 演算して
                score = enzankun(score, value);
                // セット
                uiManager.SetScore(player.isMyPlayer, score);
            }
        }
    }

    // パワー変化系
    public class Power : ValueChange
    {
        public Power(AbilitiManager owner, int value, Arithmetic arithmetic) : base(owner, value, arithmetic) { }


        public override void Action()
        {
            foreach (Player player in abilityManager.targetPlayers)
            {
                // 現在のパワーを取ってきて
                var power = player.jissainoPower;
                // 演算する
                player.jissainoPower = enzankun(power, value);
            }
        }
    }

    // カード移動系系
    public class Card : Base
    {
        public class Search
        {
            public Search()
            {

            }

            bool Check(MaskData data, CardData card)
            {
                switch (data.maskType)
                {
                    case MaskType.Power:
                        break;
                    case MaskType.Syuzoku:
                        break;
                    case MaskType.Name:
                        break;
                    default:
                        break;
                }

                return false;
            }

            public enum MaskType
            {
                Power,      // パワー
                Syuzoku,    // 種族
                Name,       // ボーイと名のつくなど
            }
            public struct MaskData
            {
                public MaskType maskType;  // タイプ
                public string value;       // マスクに使う値
            }
            public MaskData[] maskDatas;

            public void SearchCards(CardData[] cards)
            {
                //Queue<CardData> queue;

                //foreach(CardData card in cards)
                //{
                //    if()
                //}
            }
        }



        public Card(AbilitiManager owner, From from, To to, AbilityTarget fromTarget, AbilityTarget toTarget) : base(owner)
        {
            this.fromTarget = fromTarget;
            this.toTarget = toTarget;
            fromType = from;
            toType = to;
        }

        public AbilityTarget fromTarget;    // 対象
        public AbilityTarget toTarget;      // 対象

        public enum From
        {
            Hand,           // 手札から
            Cemetery,       // 墓地から
            Deck,           // 山札から
            CemeteryOrDeck, // 墓地または山札から
            NewCreate,      // 新たに生成
        }
        From fromType;

        public enum To
        {
            Hand,       // 手札に
            Cemetery,   // 墓地に
            Deck,       // 山札に
            Tsuihou,    // 追放
        }
        To toType;

        public enum SearchType
        {
            None,   // 無造作
            Saati,  // サーチ
        }
        SearchType searchType;


        public override void Action()
        {
            Player fromPlayer;
            Player toPlayer = abilityManager.GetPlayerByAbilitiTarget(toTarget);

            if(fromType != From.NewCreate)
                fromPlayer = abilityManager.GetPlayerByAbilitiTarget(fromTarget);

            switch (fromType)
            {
                case From.Hand:
                    break;
                case From.Cemetery:
                    break;
                case From.Deck:
                    break;
                case From.CemeteryOrDeck:
                    break;
                case From.NewCreate:
                    break;
                default:
                    Debug.LogError("FromTypeで想定されない値");
                    return;
            }
        }
    }
}

public class AbilitiManager
{
    // モンスターだけ
    //public Jouken.Base jouken;             // 効果の条件委譲クラス
    public Kouka.Base kouka;               // 効果委譲クラス

    public int playerID;                    // この効果を発動しようとしているプレイヤーのID
    public PlayerManager playerManager;
    public Player myPlayer, youPlayer;      // 自分と相手のプレイヤーの実体
    public Player[] targetPlayers;

    public int value;                   // 変化の値(パワーだったり、ポイントだったり)
    public Arithmetic arithmetic;       // 足す引くかける割る

    public int siteiNumber;             // 数字指定系の保存用

    public Player GetPlayerByAbilitiTarget(AbilityTarget target)
    {
        if (target == AbilityTarget.Me)
        {
            return myPlayer;
        }
        else if (target == AbilityTarget.You)
        {
            return youPlayer;
        }
        else
        {
            Debug.LogError("アビリティターゲットで想定されていない値");
            return null;
        }
    }
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

// (11/14)(TODO)-1はカードが存在しない
public enum IDType
{
    NONE = -1//  無し
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
    public bool isStrikerCard() { return (cardType == CardType.Fighter || cardType == CardType.AbilityFighter /*|| cardType == CardType.Joker*/); }
}

public static class CardDataBase
{
    static CardData[] cardDatas;
    static CardData missingCard;    // 欠番用のカード

    public static CardData[] GetCardList() { return cardDatas; }

    public static CardData GetCardData(int id)
    {
        //// 画像データ読み込んでいないなら読み込む
        //if (!m_aCardDatas[id].m_image)
        //{
        //    var texture = PngLoader.LoadPNG(path + "/image.png");
        //    if (texture) m_aCardDatas[id].m_image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //}

        // けつばん
        if(id == (int)IDType.NONE)
        {
            return missingCard;
        }
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
        missingCard.image = Resources.Load<Sprite>("Sprites/MissingImage").texture;

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
