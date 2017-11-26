using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cost
{
    // 発動チェック関数を作りたかったので自前で作る
    public abstract class Base
    {
        protected bool endFlag;

        public virtual void Enter(CardAbilityData abilityData) { endFlag = false; }
        public abstract void Execute(CardAbilityData abilityData);
        public abstract bool OnMessage(CardAbilityData abilityData, MessageInfo message);
        // 発動条件を満たしているかどうか(主にイベントカードが出せるかどうかに使う)
        public abstract bool HatsudouCheck(CardAbilityData abilityData);
        public bool EndCheck() { return endFlag; }
    }

    // 条件なし
    public class NoneLimit : Base
    {
        static NoneLimit instance;
        public static NoneLimit GetInstance() { if (instance == null) { instance = new NoneLimit(); } return instance; }

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);
        }

        public override void Execute(CardAbilityData abilityData)
        {

        }


        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData) { return true; }
    }

    public class Otakebi : Base
    {
        static Otakebi instance;
        public static Otakebi GetInstance() { if (instance == null) { instance = new Otakebi(); } return instance; }

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            var range = (DifferenceRangeType)abilityData.c_value0;  // 条件タイプ
            var difference = abilityData.c_value1;                  // 条件の差

            var myPower = abilityData.myPlayer.jissainoPower;
            var youPower = abilityData.youPlayer.jissainoPower;
            bool ok = (myPower > youPower);    // 勝ってたらtrue
            if (!ok)
            {
                abilityData.isJoukenOK = false;
                return;
            }

            switch (range)
            {
                case DifferenceRangeType.NoneLimit:
                    abilityData.isJoukenOK = true;
                    break;
                case DifferenceRangeType.In:
                    abilityData.isJoukenOK = (myPower - youPower <= difference);
                    break;
                case DifferenceRangeType.Out:
                    abilityData.isJoukenOK = (myPower - youPower >= difference);
                    break;
                default:
                    Debug.LogError("勝利の雄たけびのswitch文でエラークマ");
                    abilityData.isJoukenOK = false;
                    break;
            }
        }

        public override void Execute(CardAbilityData abilityData)
        {
            if(!abilityData.isJoukenOK)
            {
                endFlag = true;
            }
            // 演出終了
            if(true)
            {
                endFlag = true;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData) { return false; }
    }

    public class Tsumeato : Base
    {
        static Tsumeato instance;
        public static Tsumeato GetInstance() { if (instance == null) { instance = new Tsumeato(); } return instance; }

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            var range = (DifferenceRangeType)abilityData.c_value0;  // 条件タイプ
            var difference = abilityData.c_value1;                  // 条件の差

            var myPower = abilityData.myPlayer.jissainoPower;
            var youPower = abilityData.youPlayer.jissainoPower;
            bool ok = (myPower < youPower);    // 負けてたらtrue
            if (!ok)
            {
                abilityData.isJoukenOK = false;
                return;
            }

            switch (range)
            {
                case DifferenceRangeType.NoneLimit:
                    abilityData.isJoukenOK = true;
                    break;
                case DifferenceRangeType.In:
                    abilityData.isJoukenOK = (youPower - myPower <= difference);
                    break;
                case DifferenceRangeType.Out:
                    abilityData.isJoukenOK = (youPower - myPower >= difference);
                    break;
                default:
                    Debug.LogError("爪痕のswitch文でエラークマ");
                    abilityData.isJoukenOK = false;
                    break;
            }
        }

        public override void Execute(CardAbilityData abilityData)
        {
            if (!abilityData.isJoukenOK)
            {
                endFlag = true;
            }
            // 演出終了
            if (true)
            {
                endFlag = true;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData) { return true; }
    }

    //public class Sousai : BaseEntityState<CardAbilityData>
    //{
    //    public Sousai(CardAbilityData owner) : base(owner) { }

    //    public override bool CheckJouken()
    //    {
    //        int myPower = 0;
    //        int youPower = 0;
    //        return (myPower == youPower);
    //    }
    //}

    public class Burst : Base
    {
        static Burst instance;
        public static Burst GetInstance() { if (instance == null) { instance = new Burst(); } return instance; }


        int step;
        CardData drawCard;

        public override void Enter(CardAbilityData abilityData)
        {
            step = 0;
            drawCard = null;
        }

        public override void Execute(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            switch (step)
            {
                case 0:
                    // ドロー！
                    drawCard = abilityData.myPlayer.deckManager.GetDraw();
                    // モンスターカード！(isStrikerはジョーカーがfalseになるのでこの書き方)
                    if (!drawCard.isEventCard())
                    {
                        step++;
                    }

                    break;

                case 1:
                    if (drawCard != null)
                    {
                        // パワーを足す用の変数に格納
                        abilityData.delvValue0 = drawCard.power;

                        // 引いたカードを墓地に送る
                        abilityData.myPlayer.deckManager.AddBochi(drawCard);
                    }
                    else Debug.LogWarning("バースト: 引いたカードがnullクマ");

                    // コスト処理終了
                    endFlag = true;
                    break;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData)
        {
            // 墓地にジョーカーがいたら発動OK
            return abilityData.myPlayer.deckManager.isExistBochiCardType(CardType.Joker);
        }
    }

    public class Treasure : Base
    {
        static Treasure instance;
        public static Treasure GetInstance() { if (instance == null) { instance = new Treasure(); } return instance; }

        int step;
        int selectNumber;   // プレイヤーの選択した番号
        CardData drawCard;

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            step = 0;
            drawCard = null;

            // 数字選択UI表示
            CardAbilityData.uiManager.DisAppearSelectNumberUI();
        }

        public override void Execute(CardAbilityData abilityData)
        {
            switch(step)
            {
                case 0:
                    // 数字選択中
                    break;
                case 1:
                    // ドロー！モンスターカード！が終わったら
                    
                    if(true)
                    {
                        step++;
                    }
                    break;

                case 2:
                    // プレイヤーの選択した数字と引いたストライカーの数字が同じなら
                    if(true)
                    {
                        // デッキにある枚数分の得点を得る
                        abilityData.delvValue0 = abilityData.myPlayer.deckManager.GetNumYamahuda() * 10;

                        // デッキに加える
                        abilityData.myPlayer.deckManager.AddHand(drawCard);

                        // 成功時の効果に移行
                        abilityData.hatsudouAbilityNumber = abilityData.c_value0;
                    }
                    // 残念
                    else
                    {
                        // 墓地に送る
                        abilityData.myPlayer.deckManager.AddBochi(drawCard);

                        // 失敗時の効果に移行
                        abilityData.hatsudouAbilityNumber = abilityData.c_value1;
                    }

                    // コスト処理終了
                    endFlag = true;
                    break;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            switch (message.messageType)
            {
                case MessageType.SelectNumber:
                    {
                        SelectNumberInfo selectNumberInfo = new SelectNumberInfo();
                        message.GetExtraInfo<SelectNumberInfo>(ref selectNumberInfo);

                        // 選択した番号保存
                        selectNumber = selectNumberInfo.selectNumber;

                        // 次のステップへ
                        step++;
                    }
                    break;
            }

            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData){ return true; }
    }

    public class Kisikaisei : Base
    {
        static Kisikaisei instance;
        public static Kisikaisei GetInstance() { if (instance == null) { instance = new Kisikaisei(); } return instance; }

        int step;
        int selectNumber;   // プレイヤーの選択した番号

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            step = 0;

            // 数字選択UI表示
            CardAbilityData.uiManager.DisAppearSelectNumberUI();
        }

        public override void Execute(CardAbilityData abilityData)
        {
            switch (step)
            {
                case 0:
                    // 数字選択中
                    break;
                case 1:
                    if (true)
                    {
                        step++;
                    }
                    break;

                case 2:
                    // プレイヤーの選択した数字と相手のストライカーのパワーが同じなら
                    if (true)
                    {
                        // 成功時の効果に移行
                        abilityData.hatsudouAbilityNumber = abilityData.c_value0;
                    }
                    // 残念
                    else
                    {
                        // 失敗時の効果に移行
                        abilityData.hatsudouAbilityNumber = abilityData.c_value1;
                    }

                    // コスト処理終了
                    endFlag = true;
                    break;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            switch(message.messageType)
            {
                case MessageType.SelectNumber:
                    {
                        SelectNumberInfo selectNumberInfo = new SelectNumberInfo();
                        message.GetExtraInfo<SelectNumberInfo>(ref selectNumberInfo);

                        // 選択した番号保存
                        selectNumber = selectNumberInfo.selectNumber;

                        // 次のステップへ
                        step++;
                    }
                    break;
            }

            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData){ return true; }
    }

    public class Nakama : Base
    {
        static Nakama instance;
        public static Nakama GetInstance() { if (instance == null) { instance = new Nakama(); } return instance; }

        int step;
        CardData drawCard;

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            step = 0;
            drawCard = null;
        }

        public override void Execute(CardAbilityData abilityData)
        {
            switch (step)
            {
                case 0:
                    // 墓地から無造作にストライカーを引く
                    drawCard = abilityData.myPlayer.deckManager.DrawBochiStriker();

                    Debug.Assert(drawCard != null, "墓地にストライカーがいないのに引こうとしている。事前にその効果を発動しないようにしましょう。");

                    step++;
                    break;
                case 1:
                    // パワーを足す用の変数に格納
                    abilityData.delvValue0 = drawCard.power;

                    // 終了
                    endFlag = true;
                    break;
            }
        }


        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData)
        {
            // 実際のパワーが1の人だったら発動OK
            return (abilityData.myPlayer.jissainoPower == 1);
        }
    }
}


namespace Ability
{
    // 終了判定用
    public class End : BaseEntityState<CardAbilityData>
    {
        static End instance;
        public static End GetInstance() { if (instance == null) { instance = new End(); } return instance; }

        public override void Enter(CardAbilityData abilityData) { }
        public override void Execute(CardAbilityData abilityData) { }
        public override void Exit(CardAbilityData abilityData) { }
        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message) { return false; }
    }

    public static class ValueChange
    {
        public static int Enzan(Arithmetic arithmetic, int value0, int value1)
        {
            // 演算タイプに応じてラムダ関数登録
            switch (arithmetic)
            {
                case Arithmetic.Addition:
                    //enzankun = (a, b) => { return a + b; };
                    return value0 + value1;
                case Arithmetic.Subtraction:
                    //enzankun = (a, b) => { return Mathf.Max(a - b, 0); };
                    return value0 - value1;
                case Arithmetic.Multiplication:
                    //enzankun = (a, b) => { return a * b; };
                    return value0 * value1;
                case Arithmetic.Division:
                    //enzankun = (a, b) => { return (b != 0) ? a / b : 0; };
                    return (value1 != 0) ? value0 / value1 : 0;
                default:
                    Debug.LogWarning("演算enumでエラークマ");
                    return 0;
            }
        }

        public static int GetCheckRefValue(CardAbilityData abilityData, int value)
        {
            // ★参照かどうかチェック
            var bit = (value ^ CardAbilityData.RefValueFlag);
            if (bit < CardAbilityData.RefValueFlag)
            {
                switch (bit)
                {
                    case 0:
                        value = abilityData.delvValue0;
                        break;

                    case 1:
                        value = abilityData.delvValue1;
                        break;

                    case 2:
                        value = abilityData.delvValue2;
                        break;

                    case 3:
                        value = abilityData.delvValue3;
                        break;

                    default:
                        Debug.LogWarning("参照関係のビット演算でエラークマ");
                        break;
                }
            }

            return value;
        }
    }

    // スコア系
    public class Score : BaseEntityState<CardAbilityData>
    {
        static Score instance;
        public static Score GetInstance() { if (instance == null) { instance = new Score(); } return instance; }

        public override void Enter(CardAbilityData abilityData)
        {
            // ※a_value0=演算タイプ, a_value1=演算する値, a_value2=効果の対象
            var arithmetic = (Arithmetic)abilityData.a_value0;
            var value = ValueChange.GetCheckRefValue(abilityData, abilityData.a_value1);
            var abilityTarget = (AbilityTarget)abilityData.a_value2;

            switch (abilityTarget)
            {
                case AbilityTarget.Me:
                    Enzan(abilityData.myPlayer, CardAbilityData.uiManager, arithmetic, value);
                    break;

                case AbilityTarget.You:
                    Enzan(abilityData.youPlayer, CardAbilityData.uiManager, arithmetic, value);
                    break;

                case AbilityTarget.Select:
                    break;

                case AbilityTarget.All:
                    Enzan(abilityData.myPlayer, CardAbilityData.uiManager, arithmetic, value);
                    Enzan(abilityData.youPlayer, CardAbilityData.uiManager, arithmetic, value);
                    break;

                default:
                    Debug.LogWarning("スコア変更で意図しない値:" + (int)abilityTarget);
                    break;
            }
        }

        void Enzan(Player player, UIManager uiManager, Arithmetic arithmetic, int value)
        {
            // 現在のスコアを取ってきて
            var score = uiManager.GetScore(player.isMyPlayer);
            // 演算して
            score = ValueChange.Enzan(arithmetic, score, value);
            // セット
            uiManager.SetScore(player.isMyPlayer, score);
        }

        public override void Execute(CardAbilityData abilityData)
        {
            // 効果のエフェクトが終了したら
            if(true)
            {
                abilityData.abilities[abilityData.hatsudouAbilityNumber].ChangeState(End.GetInstance());
            }
        }

        public override void Exit(CardAbilityData abilityData)
        {

        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }
    }

    // パワー変化系
    public class Power : BaseEntityState<CardAbilityData>
    {
        static Power instance;
        public static Power GetInstance() { if (instance == null) { instance = new Power(); } return instance; }

        public override void Enter(CardAbilityData abilityData)
        {
            // ※a_value0=演算タイプ, a_value1=演算する値, a_value2=効果の対象
            var arithmetic = (Arithmetic)abilityData.a_value0;
            var value = ValueChange.GetCheckRefValue(abilityData, abilityData.a_value1);
            var abilityTarget = (AbilityTarget)abilityData.a_value2;

            switch(abilityTarget)
            {
                case AbilityTarget.Me:
                    Enzan(abilityData.myPlayer, arithmetic, value);
                    break;

                case AbilityTarget.You:
                    Enzan(abilityData.youPlayer, arithmetic, value);
                    break;

                case AbilityTarget.Select:
                    break;

                case AbilityTarget.All:
                    Enzan(abilityData.myPlayer, arithmetic, value);
                    Enzan(abilityData.youPlayer, arithmetic, value);
                    break;

                default:
                    Debug.LogWarning("パワー変更で意図しない値:" + (int)abilityTarget);
                    break;
            }
        }

        void Enzan(Player player, Arithmetic arithmetic, int value)
        {
            // 現在のパワーを取ってきて
            var power = player.jissainoPower;
            // 演算する
            player.jissainoPower = ValueChange.Enzan(arithmetic, power, value);
        }

        public override void Execute(CardAbilityData abilityData)
        {
            // 効果のエフェクトが終了したら
            if (true)
            {
                abilityData.abilities[abilityData.hatsudouAbilityNumber].ChangeState(End.GetInstance());
            }
        }

        public override void Exit(CardAbilityData abilityData)
        {

        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }
    }

    // カード移動系系
    public class Card : BaseEntityState<CardAbilityData>
    {
        static Card instance;
        public static Card GetInstance() { if (instance == null) { instance = new Card(); } return instance; }

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

        //public AbilityTarget fromTarget;    // 対象
        //public AbilityTarget toTarget;      // 対象

        public enum From
        {
            Hand,           // 手札から
            Cemetery,       // 墓地から
            Deck,           // 山札から
            CemeteryOrDeck, // 墓地または山札から
            NewCreate,      // 新たに生成
        }
        //From fromType;

        public enum To
        {
            Hand,       // 手札に
            Cemetery,   // 墓地に
            Deck,       // 山札に
            Tsuihou,    // 追放
        }
        //To toType;

        public enum SearchType
        {
            None,   // 無造作
            Saati,  // サーチ
        }
        //SearchType searchType;

        public override void Enter(CardAbilityData abilityData)
        {
            // ※value0=どこから, value1=誰の, value2=どこに, value3=誰の
            var fromPlace = (From)abilityData.a_value0;
            var fromTarget = (AbilityTarget)abilityData.a_value1;
            var searchType = (SearchType)abilityData.a_value2;
            var toPlace = (To)abilityData.a_value3;
            var toTarget =  (AbilityTarget)abilityData.a_value4;

            var toPlayer = abilityData.GetPlayerByAbilitiTarget(toTarget);

            if (fromPlace != From.NewCreate)
            {
                var fromPlayer = abilityData.GetPlayerByAbilitiTarget(fromTarget);

                CardData card = null;

                switch (fromPlace)
                {
                    case From.Hand:
                        // 手札から無造作に
                        if (searchType == SearchType.None)
                        {
                            card = fromPlayer.deckManager.DequeueHand();
                        }
                        else if(searchType == SearchType.Saati)
                        {
                            var cards = fromPlayer.deckManager.GetHand();

                        }
                        break;
                    case From.Cemetery:
                        break;
                    case From.Deck:
                        // 山札から無造作に
                        if (searchType == SearchType.None)
                        {
                            card = fromPlayer.deckManager.DequeueYamahuda();
                        }
                        break;
                    case From.CemeteryOrDeck:
                        break;
                    default:
                        Debug.LogError("FromTypeで想定されない値");
                        return;
                }
            }


        }

        public override void Execute(CardAbilityData abilityData)
        {
            // 効果のエフェクトが終了したら
            if (true)
            {
                abilityData.abilities[abilityData.hatsudouAbilityNumber].ChangeState(End.GetInstance());
            }
        }

        public override void Exit(CardAbilityData abilityData)
        {

        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }
    }
}

public class CardAbilityData
{
    enum State
    {
        Cost,   // コスト(効果条件)処理
        Ability // 効果処理
    }
    State state;

    public Cost.Base                                    cost;        // コスト(効果の条件)委譲クラス
    public BaseEntityStateMachine<CardAbilityData>[]    abilities;   // 効果委譲クラス(複数あり)

    public int hatsudouAbilityNumber;  // 発動する効果の番号(分岐用に作成)
    public int numAbility;             // 効果の個数

    public bool isJoukenOK; // Costを抜けた後にこれがtrueだったら効果を発動する(主にストライカーの爪痕とかに使う)
    public bool endFlag;    // 効果が終了したフラグ

    public int playerID;                    // この効果を発動しようとしているプレイヤーのID
    public static UIManager uiManager;
    public static PlayerManager playerManager;
    public Player myPlayer, youPlayer;      // 自分と相手のプレイヤーの実体

    public AbilityTriggerType abilityTriggerType;  // 発動トリガー
    public CostType costType;                      // 条件(コスト)
    public int c_value0;
    public int c_value1;

    public const int RefValueFlag = 0x100;  // 256

    public AbilityType abilityType;            // 効果のタイプ
    public int a_value0;
    public int a_value1;
    public int a_value2;
    public int a_value3;
    public int a_value4;
    public int a_value5;
    public int a_value6;
    public int a_value7;
    public int a_value8;
    public int a_value9;

    // スコア系、パワー変化系で使う
    //public int value;                   // 変化の値(パワーだったり、ポイントだったり)
    //public Arithmetic arithmetic;       // 足す引くかける割る

    // カード移動系で使う
    //public int cardFrom;
    //public AbilityTarget fromPlayer;
    //public int cardTo;
    //public AbilityTarget toPlayer;

    // 数字指定系の保存等に使う
    public int delvValue0;             
    public int delvValue1;
    public int delvValue2;
    public int delvValue3;

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

    public void Action()
    {
        if (!uiManager) uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (!playerManager) playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        myPlayer = playerManager.GetPlayer(playerID);
        youPlayer = playerManager.GetAitePlayer(playerID);

        // コスト実行
        cost.Enter(this);

        endFlag = false;
        state = State.Cost;
        hatsudouAbilityNumber = 0;
    }

    public void Update()
    {
        if (endFlag) return;

        switch(state)
        {
            case State.Cost:
                // コスト処理
                if (cost.EndCheck())
                {
                    state = State.Ability;
                    return;
                }
                cost.Execute(this);
                break;

            case State.Ability:
                // 効果処理
                if(abilities[hatsudouAbilityNumber].isInState(Ability.End.GetInstance()))
                {
                    endFlag = true;
                    return;
                }
                abilities[hatsudouAbilityNumber].Update();
                break;
        }
    }

    // 発動してもいいかチェック(主にイベントカード)
    public bool HatsudouOK() { return cost.HatsudouCheck(this); }
}

public class CardAbilityManager : MonoBehaviour
{
    Queue<CardAbilityData> abilityQueue = new Queue<CardAbilityData>();     // アビリティタスク
    CardAbilityData excutionAbility;                                        // 現在実行しているアビリティ

    void Start()
    {
        Restart();
    }

    public void Restart()
    {
        abilityQueue.Clear();
        excutionAbility = null;
    }

    void Update()
    {
        if(excutionAbility != null)
        {
            // 効果が終了するまで処理する
            if(excutionAbility.endFlag)
            {
                excutionAbility = null;
                return;
            }
            excutionAbility.Update();
        }

        else if(abilityQueue.Count > 0)
        {
            excutionAbility = abilityQueue.Dequeue();
            // 効果実行
            excutionAbility.Action();
        }
    }

    public void PushAbility(CardAbilityData ability, int playerID)
    {
        ability.playerID = playerID;
        abilityQueue.Enqueue(ability);
    }
}