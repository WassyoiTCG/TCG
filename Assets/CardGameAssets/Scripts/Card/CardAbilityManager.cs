using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cost
{
    // 発動チェック関数を作りたかったので自前で作る
    public abstract class Base
    {
        protected int step;
        protected bool endFlag;

        public virtual void Enter(CardAbilityData abilityData) { step = 0; endFlag = false; }
        public abstract void Execute(CardAbilityData abilityData);
        public abstract bool OnMessage(CardAbilityData abilityData, MessageInfo message);
        // 発動条件を満たしているかどうか(主にイベントカードが出せるかどうかに使う)
        public abstract bool HatsudouCheck(/*CardAbilityData abilityData*/Player player);
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
            // 即終了
            endFlag = true;
            // 条件OK
            abilityData.isJoukenOK = true;
        }


        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(Player player) { return true; }
    }

    // 勝利の雄たけび
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
            bool ok = (myPower > youPower || youPower == Player.noSetStrikerPower);    // 勝ってたらtrue
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

            // 条件満たしていないなら終了
            if (!abilityData.isJoukenOK)
            {
                endFlag = true;
                return;
            }

            // Ability列車 勝利の雄たけびの成功時

        }

        public override void Execute(CardAbilityData abilityData)
        {
            // Ability列車 勝利の雄たけびの演出終了判定
            if (true)
            {
                endFlag = true;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(Player player) { return true; }
    }

    // 爪痕
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


            // 条件満たしていないなら終了
            if (!abilityData.isJoukenOK)
            {
                endFlag = true;
                return;
            }

            // Ability列車 爪痕の成功時

        }

        public override void Execute(CardAbilityData abilityData)
        {
            // Ability列車 爪痕の演出終了判定
            if (true)
            {
                endFlag = true;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(Player player) { return true; }
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

    // バースト
    public class Burst : Base
    {
        static Burst instance;
        public static Burst GetInstance() { if (instance == null) { instance = new Burst(); } return instance; }


        //int step;
        Card drawCard;

        public override void Enter(CardAbilityData abilityData)
        {
            step = 0;
            drawCard = null;

            // Ability列車 バースト発動した瞬間
        }

        public override void Execute(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            switch (step)
            {
                case 0:
                    // ドロー！
                    drawCard = abilityData.myPlayer.deckManager.DequeueYamahuda();
                    drawCard.ShowDraw(0.75f, true);

                    //// データ上で引いたカードを墓地に送る
                    //abilityData.myPlayer.deckManager.AddBochi(drawCard.cardData);

                    step++;
                    break;

                case 1:
                    // カードの動き終わったら
                    if (!drawCard.isInMovingState())
                    {
                        // 引いたカードを墓地に送る動き
                        abilityData.myPlayer.deckManager.AddBochi(drawCard);

                        // モンスターカード！なら次のステート(isStrikerはジョーカーがfalseになるのでこの書き方)
                        if (!drawCard.cardData.isEventCard())
                        {
                            step++;
                        }
                        else
                        {
                            step--;
                        }
                    }
                    break;

                case 2:
                    if (drawCard != null)
                    {
                        // パワーを足す用の変数に格納
                        abilityData.delvValue0 = drawCard.cardData.power;
                    }
                    else Debug.LogWarning("バースト: 引いたカードがnullクマ");

                    // コスト処理終了
                    endFlag = true;
                    abilityData.isJoukenOK = true;
                    break;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(Player player)
        {
            // 山札が1以上のときに発動可能
            // 墓地にジョーカーがいたら発動OK
            return (
                player.deckManager.isExistBochiCardType(CardType.Joker) &&
                player.deckManager.GetNumYamahuda() > 0);
        }
    }

    public class Treasure : Base
    {
        static Treasure instance;
        public static Treasure GetInstance() { if (instance == null) { instance = new Treasure(); } return instance; }

        //int step;
        int selectNumber;   // プレイヤーの選択した番号
        Card drawCard;

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            step = 0;
            drawCard = null;

            // 数字選択UI表示
            CardAbilityData.uiManager.AppearSelectNumberUI(abilityData.myPlayer.deckManager);

            // Ability列車 宝箱発動した瞬間
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
                    // ドロー！
                    drawCard = abilityData.myPlayer.deckManager.DequeueYamahuda();
                    drawCard.ShowDraw(0.75f, true);

                    // データ上で引いたカードを墓地に送る
                    //abilityData.myPlayer.deckManager.AddBochi(drawCard.cardData);

                    step++;
                    break;

                case 2:
                    if (!drawCard.isInMovingState())
                    {
                        // モンスターカード！なら次のステート(isStrikerはジョーカーがfalseになるのでこの書き方)
                        if (!drawCard.cardData.isEventCard())
                        {
                            step++;
                        }
                        else
                        {
                            step--;

                            // 引いたカードを墓地に送る
                            abilityData.myPlayer.deckManager.AddBochi(drawCard);
                        }
                    }
                    break;

                case 3:
                    // プレイヤーの選択した数字と引いたストライカーの数字が同じなら
                    if(selectNumber == drawCard.cardData.power)
                    {
                        // デッキにある枚数分の得点を得る
                        abilityData.delvValue0 = abilityData.myPlayer.deckManager.GetNumYamahuda() * 10;

                        // デッキに加える
                        abilityData.myPlayer.deckManager.AddHand(drawCard);

                        // 成功時の効果に移行
                        abilityData.skillNumber = abilityData.c_value0;
                    }
                    // 残念
                    else
                    {
                        // 引いたカードを墓地に送る
                        abilityData.myPlayer.deckManager.AddBochi(drawCard);

                        // 引いたカードを墓地に送る動き
                        //drawCard.MoveToCemetery();

                        // 失敗時の効果に移行
                        abilityData.skillNumber = abilityData.c_value1;
                    }

                    // コスト処理終了
                    endFlag = true;
                    abilityData.isJoukenOK = true;
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

        public override bool HatsudouCheck(Player player)
        {
            // 山札が1以上のときに発動可能
            return (player.deckManager.GetNumYamahuda() > 0);
        }
    }

    // 起死回生の一手
    public class Kisikaisei : Base
    {
        static Kisikaisei instance;
        public static Kisikaisei GetInstance() { if (instance == null) { instance = new Kisikaisei(); } return instance; }

        //int step;
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
                        abilityData.skillNumber = abilityData.c_value0;
                    }
                    else// 残念
                    {
                        // 失敗時の効果に移行
                        //abilityData.skillNumber = abilityData.c_value1;
                        //[1209] 到達されないコードが検出されましたとか意味不明のエラーが出たため一時コメントアウト
                    }

                    // コスト処理終了
                    endFlag = true;
                    abilityData.isJoukenOK = true;
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

        public override bool HatsudouCheck(Player player) { return true; }
    }

    // 仲間と共に
    public class Nakama : Base
    {
        static Nakama instance;
        public static Nakama GetInstance() { if (instance == null) { instance = new Nakama(); } return instance; }

        float timer;
        //int step;
        Card drawCard;

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            timer = 0;
            step = 0;
            drawCard = null;

            // 墓地からドローする番号を決定(無造作)
            if (abilityData.isMyPlayer || !SelectData.isNetworkBattle)
            {
                SelectCardIndexInfo info = new SelectCardIndexInfo();
                info.index = abilityData.myPlayer.deckManager.GetCemeteryRandomNumberStriker();
                if (info.index == (int)IDType.NONE)
                {
                    Debug.LogWarning("墓地にストライカーがいないのに引こうとしている。事前にその効果を発動しないようにしましょう。");

                    // 終了
                    endFlag = true;
                    return;
                }
                // メッセージ送信
                MessageManager.Dispatch(abilityData.myPlayer.playerID, MessageType.SelectCemetery, info);
            }

            // Ability列車 仲間と共に発動した瞬間
        }

        public override void Execute(CardAbilityData abilityData)
        {
            switch (step)
            {
                case 0:
                    // メッセージ待ち
                    break;
                case 1:
                    // 例外処理
                    if(!drawCard)
                    {
                        Debug.LogWarning("墓地にストライカーがいないのに引こうとしている。事前にその効果を発動しないようにしましょう。");

                        // 終了
                        endFlag = true;

                        return;
                    }

                    // パワーを足す用の変数に格納
                    abilityData.delvValue0 = drawCard.cardData.power;

                    // 見せる用のドローの動きにする
                    drawCard.ShowDraw(0.75f, true);

                    step++;
                    break;
                case 2:
                    // ドローの動きが終わったら
                    if (!drawCard.isInMovingState())
                    {
                        step++;
                    }
                    break;

                case 3:
                    // 若干待ってみる
                    if((timer+= Time.deltaTime) > 2)
                    {
                        step++;
                    }
                    break;
                case 4:

                    // 引いたカードを墓地に送る
                    abilityData.myPlayer.deckManager.AddBochi(drawCard);

                    // 引いたカードを墓地に送る動き
                    drawCard.MoveToCemetery();

                    step++;
                    break;
                case 5:
                    // ドローの動きが終わったら
                    if (!drawCard.isInMovingState())
                    {
                        // 終了
                        endFlag = true;
                        abilityData.isJoukenOK = true;
                    }
                    break;
            }
        }


        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            switch (message.messageType)
            {
                case MessageType.SelectCemetery:
                    {
                        SelectCardIndexInfo selectCardIndexInfo = new SelectCardIndexInfo();
                        message.GetExtraInfo<SelectCardIndexInfo>(ref selectCardIndexInfo);

                        // 墓地から無造作にストライカーを引く
                        drawCard = abilityData.myPlayer.deckManager.DequeCemetery(selectCardIndexInfo.index);

                        // 次のステップへ
                        step++;
                    }
                    break;
            }

            return false;
        }

        public override bool HatsudouCheck(Player player)
        {
            // 墓地にストライカー系がいないと発動できない
            if (player.deckManager.isExistBochiCardType(CardType.Fighter)) { return (player.jissainoPower == 1); }
            if (player.deckManager.isExistBochiCardType(CardType.AbilityFighter)) { return (player.jissainoPower == 1); }
            if (player.deckManager.isExistBochiCardType(CardType.Joker)) { return (player.jissainoPower == 1); }

            return false;

            // 実際のパワーが1の人だったら発動OK
            //return (abilityData.myPlayer.jissainoPower == 1);
        }
    }

    // パワーxのストライカーが
    public class PowerStriker : Base
    {
        static PowerStriker instance;
        public static PowerStriker GetInstance() { if (instance == null) { instance = new PowerStriker(); } return instance; }

        Card suteCard;

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            suteCard = null;

            Func<int, int, bool> check;
            switch (abilityData.c_value1)
            {
                case 0:
                    check = (a, b) => { return (a == b); };
                    break;

                case 1:
                    check = (a, b) => { return (a >= b); };
                    break;

                case 2:
                    check = (a, b) => { return (a <= b); };
                    break;

                default:
                    Debug.LogWarning("おかしいおかしい");
                    endFlag = true;
                    return;
            }

            // (TODO)手札からドローする番号を決定(無造作)
            if (abilityData.isMyPlayer || !SelectData.isNetworkBattle)
            {
                var deckManager = abilityData.myPlayer.deckManager;
                for (int i = 0; i < deckManager.GetNumHand(); i++)
                {
                    var card = deckManager.GetHandCard(i);
                    if (card.isEventCard()) continue;
                    // 条件満たしたら
                    if (check(card.power, abilityData.c_value0))
                    {
                        SelectCardIndexInfo info = new SelectCardIndexInfo();
                        info.index = i;
                        // メッセージ送信
                        MessageManager.Dispatch(abilityData.myPlayer.playerID, MessageType.SelectHand, info);
                        return;
                    }
                }

                Debug.LogWarning("パワーコストのモンスター持ってないのに発動しようとした。事前にその効果を発動しないようにしましょう。");

                // 終了
                endFlag = true;
                return;
            }
        }

        public override void Execute(CardAbilityData abilityData)
        {
            switch(step)
            {
                case 0: // カード選択中
                    break;
                case 1: // 捨てカード移動中
                    if(!suteCard.isInMovingState())
                    {
                        // 墓地に行く
                        suteCard.MoveToCemetery();
                        step++;
                    }
                    break;
                case 2: // 墓地に行くカードの動きが終わったら
                    if(!suteCard.isInMovingState())
                    {
                        // 終了
                        abilityData.isJoukenOK = true;
                        endFlag = true;
                    }
                    break;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            switch(message.messageType)
            {
                case MessageType.SelectHand:
                    {
                        SelectCardIndexInfo info = new SelectCardIndexInfo();
                        message.GetExtraInfo<SelectCardIndexInfo>(ref info);
                        suteCard = abilityData.myPlayer.deckManager.DequeueHand(info.index);
                        suteCard.ShowDraw(0.75f, false);
                        step++;
                    }
                    return true;
            }
            return false;
        }

        public override bool HatsudouCheck(Player player)
        {
            return false;
        }
    }
}


namespace Skill
{
    public enum Result
    {
        None,       // 特に問題なし
        EndGame,    // HPが0になったとかでゲーム終了
    }

    public abstract class Base
    {
        protected bool endFlag;
        protected int step;

        public virtual void Enter(CardAbilityData abilityData) { step = 0; endFlag = false; }
        public abstract Result Execute(CardAbilityData abilityData);
        public abstract void Exit(CardAbilityData abilityData);
        public abstract bool OnMessage(CardAbilityData abilityData, MessageInfo message);
        // 終了チェック
        public bool EndCheck() { return endFlag; }
    }

    // 効果なし
    public class None : Base
    {
        static None instance;
        public static None GetInstance() { if (instance == null) { instance = new None(); } return instance; }

        public override void Enter(CardAbilityData abilityData){ base.Enter(abilityData); }

        public override Result Execute(CardAbilityData abilityData)
        {
            // 無条件終了
            endFlag = true;

            return Result.None;
        }

        public override void Exit(CardAbilityData abilityData)
        {

        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }
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

    // ライフ系
    public class Score : Base
    {
        static Score instance;
        public static Score GetInstance() { if (instance == null) { instance = new Score(); } return instance; }

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);
        }

        bool Enzan(Player player, UIManager uiManager, Arithmetic arithmetic, int value)
        {
            Card strikerCard = player.cardObjectManager.fieldStrikerCard;
            if (strikerCard != null)
            {
                // Ability列車 ライフ変化
                Vector3 strikerPosition = strikerCard.cacheTransform.localPosition;
                switch (arithmetic)
                {
                    case Arithmetic.Addition:
                        // 足し算なので回復
                        break;

                    case Arithmetic.Subtraction:
                        // 引き算なのでダメージ
                        break;

                    case Arithmetic.Multiplication:
                        // 掛け算(あまり使わない)
                        break;

                    case Arithmetic.Division:
                        // 割り算(あまり使わない)
                        break;
                }
            }

            // 現在のスコアを取ってきて
            var score = uiManager.GetScore(player.isMyPlayer);
            // 演算して
            var newScore = ValueChange.Enzan(arithmetic, score, value);
            // セット
            uiManager.SetHP(player.isMyPlayer, newScore);

            var isEnd = (newScore <= 0 && score > 0);
            return isEnd;
        }

        public override Result Execute(CardAbilityData abilityData)
        {
            switch(step)
            {
                case 0:
                    {
                        var skillData = abilityData.GetCurrentSkillData();

                        // ※s_iValue0=演算タイプ, s_iValue1=演算する値, s_iValue2=効果の対象
                        var arithmetic = (Arithmetic)skillData.s_iValue0;
                        var value = ValueChange.GetCheckRefValue(abilityData, skillData.s_iValue1);
                        var abilityTarget = (AbilityTarget)skillData.s_iValue2;

                        switch (abilityTarget)
                        {
                            case AbilityTarget.Me:
                                if (Enzan(abilityData.myPlayer, CardAbilityData.uiManager, arithmetic, value)) return Result.EndGame;
                                break;

                            case AbilityTarget.You:
                                if (Enzan(abilityData.youPlayer, CardAbilityData.uiManager, arithmetic, value)) return Result.EndGame;
                                break;

                            case AbilityTarget.Select:
                                break;

                            case AbilityTarget.All:
                                if (Enzan(abilityData.myPlayer, CardAbilityData.uiManager, arithmetic, value)) return Result.EndGame;
                                if (Enzan(abilityData.youPlayer, CardAbilityData.uiManager, arithmetic, value)) return Result.EndGame;
                                break;

                            default:
                                Debug.LogWarning("スコア変更で意図しない値:" + (int)abilityTarget);
                                break;
                        }

                        step++;
                    }
                    break;

                case 1:
                    // Ability列車 ライフ変化の演出が終わったら
                    if (true)
                    {
                        endFlag = true;
                    }
                    break;
            }

            return Result.None;
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
    public class Power : Base
    {
        static Power instance;
        public static Power GetInstance() { if (instance == null) { instance = new Power(); } return instance; }

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            var skillData = abilityData.GetCurrentSkillData();

            // ※s_iValue0=演算タイプ, s_iValue1=演算する値, s_iValue2=効果の対象
            var arithmetic = (Arithmetic)skillData.s_iValue0;
            var value = ValueChange.GetCheckRefValue(abilityData, skillData.s_iValue1);
            var abilityTarget = (AbilityTarget)skillData.s_iValue2;

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
            Card strikerCard = player.cardObjectManager.fieldStrikerCard;
            if (strikerCard != null)
            {
                // Ability列車 パワー変化
                Vector3 strikerPosition = strikerCard.cacheTransform.localPosition;
                switch (arithmetic)
                {
                    case Arithmetic.Addition:
                        // パワー加算エフェクト
                        break;

                    case Arithmetic.Subtraction:
                        // パワー減算エフェクト
                        break;

                    case Arithmetic.Multiplication:
                        // 掛け算(パワー0にするだったらvalueが0になってる)
                        break;

                    case Arithmetic.Division:
                        // 割り算(あまり使わない)
                        break;
                }
            }

            // 現在のパワーを取ってきて
            var power = player.jissainoPower;
            // 演算する
            player.jissainoPower = ValueChange.Enzan(arithmetic, power, value);
        }

        public override Result Execute(CardAbilityData abilityData)
        {
            // Ability列車 パワー変化の演出が終わったら
            if (true)
            {
                endFlag = true;
            }
            return Result.None;
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
    //public class CardMove : Base
    //{
    //    static CardMove instance;
    //    public static CardMove GetInstance() { if (instance == null) { instance = new CardMove(); } return instance; }

    //    public class Search
    //    {
    //        public Search()
    //        {

    //        }

    //        //bool Check(MaskData data, CardData card)
    //        //{
    //        //    switch (data.maskType)
    //        //    {
    //        //        case MaskType.Power:
    //        //            break;
    //        //        case MaskType.Syuzoku:
    //        //            break;
    //        //        case MaskType.Name:
    //        //            break;
    //        //        default:
    //        //            break;
    //        //    }

    //        //    return false;
    //        //}

    //        //public enum MaskType
    //        //{
    //        //    Power,      // パワー
    //        //    Syuzoku,    // 種族
    //        //    Name,       // ボーイと名のつくなど
    //        //}
    //        //public struct MaskData
    //        //{
    //        //    public MaskType maskType;  // タイプ
    //        //    public string value;       // マスクに使う値
    //        //}
    //        //public MaskData[] maskDatas;

    //        //public void SearchCards(CardData[] cards)
    //        //{
    //        //    //Queue<CardData> queue;

    //        //    //foreach(CardData card in cards)
    //        //    //{
    //        //    //    if()
    //        //    //}
    //        //}
    //    }

    //    //public AbilityTarget fromTarget;    // 対象
    //    //public AbilityTarget toTarget;      // 対象

    //    public enum From
    //    {
    //        Hand,           // 手札から
    //        Cemetery,       // 墓地から
    //        Deck,           // 山札から
    //        CemeteryOrDeck, // 墓地または山札から
    //        NewCreate,      // 新たに生成
    //    }
    //    //From fromType;

    //    public enum To
    //    {
    //        Hand,       // 手札に
    //        Cemetery,   // 墓地に
    //        Deck,       // 山札に
    //        Tsuihou,    // 追放
    //    }
    //    //To toType;

    //    public enum SearchType
    //    {
    //        None,   // 無造作
    //        Saati,  // サーチ
    //    }
    //    //SearchType searchType;

    //    public enum SearchSitei
    //    {
    //        PlayerSelect,   // プレイヤー指定
    //        Random,         // 無造作
    //        Highest,        // 最も高い
    //        Lowest          // 最も低い
    //    }

    //    public enum SearchMask
    //    {
    //        NoneLimit,          // 指定なし
    //        AllStriker,         // ストライカー全般
    //        NoAbilityStriker,   // 効果なしストライカー
    //        AbilityStriker,     // 効果ありストライカー
    //        Joker,              // ジョーカー
    //        Support,            // サポート
    //        Connect,            // コネクト
    //        Intercept,          // インターセプト
    //    }

    //    public enum Siborikomi
    //    {
    //        Power,      // パワー
    //        Syuzoku,    // 種族
    //        Name,       // ボーイと名のつくなど
    //    }


    //    Card drawCard;

    //    public override void Enter(CardAbilityData abilityData)
    //    {
    //        base.Enter(abilityData);

    //        var skillData = abilityData.GetCurrentSkillData();

    //        drawCard = null;
            
    //        // ※value0=どこから, value1=誰の, value2=どこに, value3=誰の
    //        var fromPlace = (From)skillData.s_iValue0;
    //        var fromTarget = (AbilityTarget)skillData.s_iValue1;
    //        var searchType = (SearchType)skillData.s_iValue2;
    //        var toPlace = (To)skillData.s_iValue3;
    //        var toTarget =  (AbilityTarget)skillData.s_iValue4;
    //        var searchSitei = (SearchSitei)skillData.s_iValue5;
    //        var searchMask = (SearchMask)skillData.s_iValue6;
    //        var siborikomi = (Siborikomi)skillData.s_iValue7;
    //        var siborikomiValue = skillData.s_sValue0;

    //        if (fromPlace != From.NewCreate)
    //        {
    //            // Fromの処理
    //            var fromPlayer = abilityData.GetPlayerByAbilitiTarget(fromTarget);

    //            switch (fromPlace)
    //            {
    //                case From.Hand:
    //                    // 手札から無造作に
    //                    if (searchType == SearchType.None)
    //                    {
    //                        int numHand = abilityData.myPlayer.deckManager.GetNumHand();
    //                        if (numHand <= 0)
    //                        {
    //                            endFlag = true;
    //                            return;
    //                        }

    //                        if (abilityData.isMyPlayer || !SelectData.isNetworkBattle)
    //                        {
    //                            int r = UnityEngine.Random.Range(0, numHand - 1);
    //                            SelectCardIndexInfo info = new SelectCardIndexInfo();
    //                            info.index = r;

    //                            // メッセージ送信
    //                            MessageManager.Dispatch(abilityData.myPlayer.playerID, MessageType.SelectHand, info);
    //                        }

    //                    }
    //                    else if(searchType == SearchType.Saati)
    //                    {
    //                        var cards = fromPlayer.deckManager.GetHand();
    //                        // (TODO)
    //                        Debug.LogWarning("未実装の効果が発動しようとしています");
    //                    }
    //                    break;
    //                case From.Cemetery:
    //                    // 墓地から無造作に
    //                    if (searchType == SearchType.None)
    //                    {
    //                        int numCemetery = abilityData.myPlayer.deckManager.GetNumCemetery();
    //                        if (numCemetery <= 0)
    //                        {
    //                            endFlag = true;
    //                            return;
    //                        }

    //                        if (abilityData.isMyPlayer || !SelectData.isNetworkBattle)
    //                        {
    //                            int r = UnityEngine.Random.Range(0, numCemetery - 1);
    //                            SelectCardIndexInfo info = new SelectCardIndexInfo();
    //                            info.index = r;

    //                            // メッセージ送信
    //                            MessageManager.Dispatch(abilityData.myPlayer.playerID, MessageType.SelectHand, info);
    //                        }
    //                    }
    //                    else if (searchType == SearchType.Saati)
    //                    {
    //                        var cards = fromPlayer.deckManager.GetCemeteryCards();
    //                        // (TODO)
    //                        Debug.LogWarning("未実装の効果が発動しようとしています");
    //                    }
    //                    break;
    //                case From.Deck:
    //                    // 山札から無造作に
    //                    if (searchType == SearchType.None)
    //                    {
    //                        drawCard = fromPlayer.deckManager.DequeueYamahuda();
    //                    }
    //                    else if (searchType == SearchType.Saati)
    //                    {
    //                        var cards = fromPlayer.deckManager.GetHand();
    //                        // (TODO)
    //                        Debug.LogWarning("未実装の効果が発動しようとしています");
    //                    }
    //                    break;
    //                case From.CemeteryOrDeck:
    //                    if(searchType != SearchType.Saati)
    //                    {
    //                        Debug.LogWarning("墓地または山札なのに無造作になっている");

    //                        return;
    //                    }


    //                    break;
    //                default:
    //                    Debug.LogError("FromTypeで想定されない値");
    //                    return;
    //            }

    //        }
    //        else
    //        {
    //            // もふりとか、生成系の処理
    //            //drawCard = 
    //        }

    //        if (drawCard == null) return;

    //        // Toの処理
    //        var toPlayer = abilityData.GetPlayerByAbilitiTarget(toTarget);

    //        switch (toPlace)
    //        {
    //            case To.Hand:
    //                toPlayer.deckManager.AddHand(drawCard);
    //                break;
    //            case To.Cemetery:
    //                // データ上でカードを墓地に送る
    //                abilityData.myPlayer.deckManager.AddBochi(drawCard);
    //                // カードを墓地に送る動き
    //                drawCard.MoveToCemetery();
    //                break;
    //            case To.Deck:
    //                // (TODO)実装まだ
    //                break;
    //            case To.Tsuihou:
    //                // (TODO)実装まだ
    //                break;
    //            default:
    //                Debug.LogError("ToTypeで想定されない値");
    //                return;
    //        }
    //    }

    //    public override Result Execute(CardAbilityData abilityData)
    //    {
    //        switch(step)
    //        {
    //            case 0:// メッセージ待ち
    //                //break;
    //            case 1:
    //                // カードの動きが終わるまで待つ
    //                if (drawCard.isInMovingState())
    //                {
    //                    return Result.None;
    //                }
    //                break;
    //        }
    //        return Result.None;
    //    }

    //    public override void Exit(CardAbilityData abilityData)
    //    {

    //    }

    //    public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
    //    {
    //        switch(message.messageType)
    //        {
    //            case MessageType.SelectHand:
    //                {
    //                    SelectCardIndexInfo info = new SelectCardIndexInfo();
    //                    message.GetExtraInfo<SelectCardIndexInfo>(ref info);
    //                    drawCard = abilityData.myPlayer.deckManager.DequeueHand(info.index);
    //                    drawCard.ShowDraw(0.75f, false);
    //                    step++;
    //                }
    //                break;

    //            case MessageType.SelectCemetery:
    //                {
    //                    SelectCardIndexInfo info = new SelectCardIndexInfo();
    //                    message.GetExtraInfo<SelectCardIndexInfo>(ref info);
    //                    drawCard = abilityData.myPlayer.deckManager.DequeCemetery(info.index);
    //                    drawCard.ShowDraw(0.75f, false);
    //                    step++;
    //                }
    //                break;
    //        }
    //        return false;
    //    }
    //}
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

    public int skillNumber;         // 発動する効果の番号(分岐用に作成)
    public int numSkill;             // 効果の個数

    public bool isJoukenOK; // Costを抜けた後にこれがtrueだったら効果を発動する(主にストライカーの爪痕とかに使う)
    public bool endFlag;    // 効果が終了したフラグ

    public bool isMyPlayer;                    // この効果を発動しようとしているプレイヤーが画面的に自分かどうか
    public static UIManager uiManager;
    public static PlayerManager playerManager;
    public Player myPlayer, youPlayer;      // 自分と相手のプレイヤーの実体

    public AbilityTriggerType abilityTriggerType;  // 発動トリガー
    public int lifeCost;                           // 支払うライフの値
    public CostType costType;                      // 条件(コスト)
    public int c_value0;
    public int c_value1;

    public const int RefValueFlag = 0x100;  // 256

    // structにするとvarに格納して処理したときに無駄になってしまう
    public class SkillData
    {
        public Skill.Base skill;        // 効果委譲クラス(複数あり)
        public int nextSkillNumber;     // 次に発動する効果(ないなら終了)
        public AbilityType abilityType;                           // 効果のタイプ
        public int s_iValue0;
        public int s_iValue1;
        public int s_iValue2;
        public int s_iValue3;
        public int s_iValue4;
        public int s_iValue5;
        public int s_iValue6;
        public int s_iValue7;
        public int s_iValue8;
        public int s_iValue9;
        public string s_sValue0;
        public string s_sValue1;
        public string s_sValue2;
    }
    public SkillData[] skillDatas;
    public SkillData GetCurrentSkillData() { return skillDatas[skillNumber]; }

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
    //private CardAbilityData ability;

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

        if (isMyPlayer)
        {
            myPlayer = playerManager.GetMyPlayer();
            youPlayer = playerManager.GetCPUPlayer();
        }
        else
        {
            myPlayer = playerManager.GetCPUPlayer();
            youPlayer = playerManager.GetMyPlayer();
        }

        // ライフコストを支払う
        if (lifeCost > 0) uiManager.Damage(isMyPlayer, lifeCost);

        // コスト実行
        cost.Enter(this);

        endFlag = false;
        state = State.Cost;
        skillNumber = 0;
    }

    public void Update(CardAbilityManager abilityManager)
    {
        if (endFlag) return;

        switch(state)
        {
            case State.Cost:
                // コスト処理
                if (cost.EndCheck())
                {
                    // 条件を満たしていない場合処理しない
                    if(!isJoukenOK)
                    {
                        endFlag = true;
                        return;
                    }
                    state = State.Ability;
                    GetCurrentSkillData().skill.Enter(this);
                    return;
                }
                cost.Execute(this);
                break;

            case State.Ability:
                {
                    var currentSkill = GetCurrentSkillData();
                    // 効果処理
                    if (currentSkill.skill.EndCheck())
                    {
                        // 終了処理
                        currentSkill.skill.Exit(this);

                        // 次に発動するスキルがないなら終了
                        if (currentSkill.nextSkillNumber == (int)IDType.NONE)
                        {
                            endFlag = true;
                        }
                        else
                        {
                            Debug.Assert(skillNumber != currentSkill.nextSkillNumber, "発動スキル番号重複");
                            skillNumber = currentSkill.nextSkillNumber;

                            GetCurrentSkillData().skill.Enter(this);
                        }
                        return;
                    }
                    switch (GetCurrentSkillData().skill.Execute(this))
                    {
                        case Skill.Result.EndGame:
                            // 終了処理
                            currentSkill.skill.Exit(this);
                            abilityManager.sceneMain.Finish();
                            endFlag = true;
                            // 空
                            abilityManager.Restart();
                            break;
                    }
                    
                }
                break;
        }
    }

    public bool HandleMessage(MessageInfo message)
    {
        if (endFlag) return false;

        switch (state)
        {
            case State.Cost:
                return cost.OnMessage(this, message);

            case State.Ability:
                return GetCurrentSkillData().skill.OnMessage(this, message);
        }

        return false;
    }

    // 発動プレイヤー設定
    //public void SetMyPlayerFlag(bool isMyPlayer)
    //{
    //    if (!playerManager) playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

    //    this.isMyPlayer = isMyPlayer;
    //    if(isMyPlayer)
    //    {
    //        myPlayer = playerManager.GetMyPlayer();
    //        youPlayer = playerManager.GetCPUPlayer();
    //    }
    //    else
    //    {
    //        myPlayer = playerManager.GetCPUPlayer();
    //        youPlayer = playerManager.GetMyPlayer();
    //    }
    //}

    // 発動してもいいかチェック(主にイベントカード)
    public bool HatsudouOK(Player player)
    {
        if (!uiManager) uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        // [1210] 宝箱にも設定している？
        // ライフがコスト以下だと発動できない
        var life = (player.isMyPlayer) ? uiManager.myLP.iLP : uiManager.cpuLP.iLP;
        if (life <= lifeCost) return false;

        return cost.HatsudouCheck(player);
    }
}

public class CardAbilityManager : MonoBehaviour
{
    Queue<CardAbilityData> abilityQueue = new Queue<CardAbilityData>();     // アビリティタスク
    CardAbilityData excutionAbility;                                        // 現在実行しているアビリティ
    public SceneMain sceneMain;

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
            excutionAbility.Update(this);
        }

        else if(abilityQueue.Count > 0)
        {
            excutionAbility = abilityQueue.Dequeue();
            // 効果実行
            excutionAbility.Action();
            //Debug.Log("効果発動: " + excutionAbility.abilityType.ToString() + ", values: " +
            //    excutionAbility.s_iValue0 + ", " + excutionAbility.s_iValue1 + ", " + excutionAbility.s_iValue2 + ", " + excutionAbility.s_iValue3 + ", " +
            //    excutionAbility.s_iValue4 + ", " + excutionAbility.s_iValue5 + ", " + excutionAbility.s_iValue6 + ", " + excutionAbility.s_iValue7 + ", " +
            //    excutionAbility.s_iValue8 + ", " + excutionAbility.s_iValue9);
        }

        // ここにexcutionAbility.を書いてはいけない
    }

    public void PushAbility(CardAbilityData ability, bool isMyPlayer)
    {
        // ★★★嫌な予感がする…(参照)
        var abilityData = new CardAbilityData();
        abilityData = ability;
        abilityData.isMyPlayer = isMyPlayer;
        abilityQueue.Enqueue(abilityData);
    }

    // 全ての効果の処理が終わったかどうかの判定
    public bool isAbilityEnd()
    {
        return (abilityQueue.Count == 0 && excutionAbility == null);
    }

    // メッセージ受信
    public bool HandleMessage(MessageInfo message)
    {
        if (excutionAbility == null) return false;

        return excutionAbility.HandleMessage(message);
    }
}
