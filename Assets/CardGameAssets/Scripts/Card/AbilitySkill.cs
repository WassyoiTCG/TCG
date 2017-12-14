using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public override void Enter(CardAbilityData abilityData) { base.Enter(abilityData); }

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
            // 現在のスコア
            var score = uiManager.GetScore(player.isMyPlayer);
            var isEnd = false;

            if (strikerCard != null)
            {
                // Ability列車 ライフ変化
                Vector3 strikerPosition = strikerCard.cacheTransform.localPosition;
                switch (arithmetic)
                {
                    case Arithmetic.Addition:
                        // 足し算なので回復
                        uiManager.Heal(player.isMyPlayer, value);
                        break;

                    case Arithmetic.Subtraction:
                        // 引き算なのでダメージ
                        isEnd = uiManager.Damage(player.isMyPlayer, value);
                        break;

                    case Arithmetic.Multiplication:
                        // 掛け算(あまり使わない)
                        var mult = score * value;
                        if (mult > score) uiManager.Heal(player.isMyPlayer, mult - score);
                        else if (mult < score) isEnd = uiManager.Damage(player.isMyPlayer, score - mult);
                        break;

                    case Arithmetic.Division:
                        // 割り算(あまり使わない)
                        var div = score / value;
                        if (div > score) uiManager.Heal(player.isMyPlayer, div - score);
                        else if (div < score) isEnd = uiManager.Damage(player.isMyPlayer, score - div);
                        break;
                }
            }


            //// 演算して
            //var newScore = ValueChange.Enzan(arithmetic, score, value);
            //// セット
            //uiManager.SetHP(player.isMyPlayer, newScore);

            //var isEnd = (newScore <= 0 && score > 0);
            return isEnd;
        }

        public override Result Execute(CardAbilityData abilityData)
        {
            switch (step)
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

            switch (abilityTarget)
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
            player.SetPower(ValueChange.Enzan(arithmetic, power, value));
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
    public class CardMove : Base
    {
        static CardMove instance;
        public static CardMove GetInstance() { if (instance == null) { instance = new CardMove(); } return instance; }

        //public class Search
        //{
        //public Search()
        //{

        //}

        //bool Check(MaskData data, CardData card)
        //{
        //    switch (data.maskType)
        //    {
        //        case MaskType.Power:
        //            break;
        //        case MaskType.Syuzoku:
        //            break;
        //        case MaskType.Name:
        //            break;
        //        default:
        //            break;
        //    }

        //    return false;
        //}

        //public enum MaskType
        //{
        //    Power,      // パワー
        //    Syuzoku,    // 種族
        //    Name,       // ボーイと名のつくなど
        //}
        //public struct MaskData
        //{
        //    public MaskType maskType;  // タイプ
        //    public string value;       // マスクに使う値
        //}
        //public MaskData[] maskDatas;

        //public void SearchCards(CardData[] cards)
        //{
        //    //Queue<CardData> queue;

        //    //foreach(CardData card in cards)
        //    //{
        //    //    if()
        //    //}
        //}
        //}

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
        Player fromPlayer;

        public enum To
        {
            Hand,       // 手札に
            Cemetery,   // 墓地に
            Deck,       // 山札に
            Tsuihou,    // 追放
        }
        To toType;
        Player toPlayer;

        public enum SearchType
        {
            None,   // 無造作
            Saati,  // サーチ
        }
        //SearchType searchType;

        public enum SelectType
        {
            PlayerSelect,   // プレイヤー指定
            AutoRandom,     // 無造作(デッキならトップ)
            Highest,        // 最も高い
            Lowest          // 最も低い
        }

        public enum SearchMask
        {
            NoneLimit,          // 指定なし
            AllStriker,         // ストライカー全般
            NoAbilityStriker,   // 効果なしストライカー
            AbilityStriker,     // 効果ありストライカー
            Joker,              // ジョーカー
            Support,            // サポート
            Connect,            // コネクト
            Intercept,          // インターセプト
        }

        //public enum Siborikomi
        //{
        //    Power,      // パワー
        //    Syuzoku,    // 種族
        //    Name,       // ボーイと名のつくなど
        //}

        public struct SearchInfo
        {
            public SelectType selectType;
            public SearchMask searchMask;
            //public Siborikomi siborikomi;
            public string sPower;
            public string sSyuzoku;
            public string sName;
        }

        //float timer;
        SelectCardIndexInfo selectCardIndexInfo;
        MessageType messageType = MessageType.NoMessage;
        Card drawCard;

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            //timer = 0;
            //
            selectCardIndexInfo = new SelectCardIndexInfo();
            selectCardIndexInfo.index = (int)IDType.NONE;
            messageType = MessageType.NoMessage;

            Debug.Log("カードサーチ発動するクマ");

            var skillData = abilityData.GetCurrentSkillData();

            drawCard = null;

            // ※value0=どこから, value1=誰の, value2=どこに, value3=誰の
            var fromPlace = (From)skillData.s_iValue0;
            var fromTarget = (AbilityTarget)skillData.s_iValue1;
            var searchType = (SearchType)skillData.s_iValue2;
            toType = (To)skillData.s_iValue3;
            var toTarget = (AbilityTarget)skillData.s_iValue4;
            toPlayer = abilityData.GetPlayerByAbilitiTarget(toTarget);
            var searchInfo = new SearchInfo();
            searchInfo.selectType = (SelectType)skillData.s_iValue5;
            searchInfo.searchMask = (SearchMask)skillData.s_iValue6;
            //searchInfo.siborikomi = (Siborikomi)skillData.s_iValue7;
            //searchInfo.siborikomiValue = skillData.s_sValue0;
            searchInfo.sPower = skillData.s_sValue0;
            searchInfo.sSyuzoku = skillData.s_sValue1;
            searchInfo.sName = skillData.s_sValue2;

            if (fromPlace == From.NewCreate)
            {
                // もふりとか、生成系の処理
                //drawCard = 
            }
            else
            {
                // Fromの処理
                /*var */
                fromPlayer = abilityData.GetPlayerByAbilitiTarget(fromTarget);

                Debug.Log("from処理するクマ");

                int searchIndex = (int)IDType.NONE;

                switch (fromPlace)
                {
                    case From.Hand:
                        messageType = MessageType.SelectHand;

                        // 手札から無造作に
                        if (searchType == SearchType.None)
                        {
                            Debug.Log("手札から無造作クマ");

                            int numHand = fromPlayer.deckManager.GetNumHand();
                            if (numHand <= 0)
                            {
                                Debug.Log("手札ないクマ");
                                endFlag = true;
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else
                            {
                                int r = UnityEngine.Random.Range(0, numHand - 1);
                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                selectCardIndexInfo.index = r;
                                //info.index = r;

                                // メッセージ送信
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectHand, info);
                            }
                        }
                        else if (searchType == SearchType.Saati)
                        {
                            Debug.Log("手札からサーチクマ");

                            searchIndex = SearchHand(searchInfo);

                            // 見つかった
                            if (searchIndex != (int)IDType.NONE)
                            {
                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                selectCardIndexInfo.index = searchIndex;
                                //info.index = searchIndex;
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectHand, info);
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else endFlag = true;// 見つからなかったので終了
                        }
                        break;
                    case From.Cemetery:
                        messageType = MessageType.SelectCemetery;

                        // 墓地から無造作に
                        if (searchType == SearchType.None)
                        {
                            Debug.Log("墓地から無造作クマ");

                            int numCemetery = fromPlayer.deckManager.GetNumCemetery();
                            if (numCemetery <= 0)
                            {
                                Debug.Log("墓地ないクマ");
                                endFlag = true;
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else
                            {
                                int r = UnityEngine.Random.Range(0, numCemetery - 1);
                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                selectCardIndexInfo.index = r;
                                //info.index = r;

                                // メッセージ送信
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectCemetery, info);
                            }
                        }
                        else if (searchType == SearchType.Saati)
                        {
                            Debug.Log("墓地からサーチクマ");

                            searchIndex = SearchCemetery(searchInfo);

                            // 見つかった
                            if (searchIndex != (int)IDType.NONE)
                            {
                                selectCardIndexInfo.index = searchIndex;

                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                //info.index = searchIndex;
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectCemetery, info);
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else endFlag = true;  // 見つからなかったので終了
                        }
                        break;
                    case From.Deck:
                        messageType = MessageType.SelectYamahuda;

                        // 山札のトップ
                        if (searchType == SearchType.None)
                        {
                            Debug.Log("デッキのトップから引くクマ");

                            int numYamahuda = fromPlayer.deckManager.GetNumYamahuda();
                            if (numYamahuda != 0)
                            {
                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                selectCardIndexInfo.index = numYamahuda - 1;
                                //info.index = fromPlayer.deckManager.GetNumYamahuda() - 1;

                                if (toType == To.Hand && fromPlayer.playerID == toPlayer.playerID)
                                {
                                    //drawCard = fromPlayer.deckManager.DequeueYamahuda();
                                    //step = 2;
                                    selectCardIndexInfo.iMoveFlag = (int)CardMoveFlag.Draw;
                                    //info.iMoveFlag = (int)CardMoveFlag.Draw;
                                }
                                else
                                {
                                    selectCardIndexInfo.iMoveFlag = (int)CardMoveFlag.ShowDraw;
                                    //info.iMoveFlag = (int)CardMoveFlag.Draw;
                                }
                                // メッセージ送信
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectYamahuda, info);
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else
                            {
                                Debug.Log("山札がないのでドローしないクマ");
                                endFlag = true;
                            }

                        }
                        else if (searchType == SearchType.Saati)
                        {
                            Debug.Log("デッキからサーチするクマ");

                            searchIndex = SearchYamahuda(searchInfo);

                            // 見つかった
                            if (searchIndex != (int)IDType.NONE)
                            {
                                selectCardIndexInfo.index = searchIndex;
                                selectCardIndexInfo.iMoveFlag = (int)CardMoveFlag.ShowDraw;

                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                //info.index = searchIndex;
                                //info.iMoveFlag = (int)CardMoveFlag.ShowDraw;
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectYamahuda, info);
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else endFlag = true;// 見つからなかったので終了
                            
                            
                        }
                        break;
                    case From.CemeteryOrDeck:
                        if (searchType != SearchType.Saati)
                        {
                            Debug.LogWarning("墓地または山札なのに無造作になっている");
                            endFlag = true;
                            // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                            //return;
                        }

                        Debug.Log("デッキか墓地からサーチするクマ");

                        // 山札カードから検索をかける
                        searchIndex = SearchYamahuda(searchInfo);
                        // 見つかった
                        if (searchIndex != (int)IDType.NONE)
                        {
                            // もういやだもうやりたくない体を壊すのはもう嫌だ
                            messageType = MessageType.SelectYamahuda;
                            selectCardIndexInfo.index = searchIndex;
                            selectCardIndexInfo.iMoveFlag = (int)CardMoveFlag.ShowDraw;

                            //SelectCardIndexInfo info = new SelectCardIndexInfo();
                            //info.index = searchIndex;
                            //info.iMoveFlag = (int)CardMoveFlag.ShowDraw;
                            //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectYamahuda, info);
                            // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                            //return;
                        }
                        else
                        {

                            // 墓地カードから検索を掛ける
                            searchIndex = SearchCemetery(searchInfo);

                            // 見つかった
                            if (searchIndex != (int)IDType.NONE)
                            {
                                messageType = MessageType.SelectCemetery;
                                selectCardIndexInfo.index = searchIndex;

                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                //info.index = searchIndex;
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectCemetery, info);
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else endFlag = true;// 見つからなかった
                        }

                        break;
                    default:
                        Debug.LogError("FromTypeで想定されない値");
                        endFlag = true;
                        break;
                }

                // ネットプレーかつ自分じゃなかったら送らなくていい
                if (!abilityData.isMyPlayer && SelectData.isNetworkBattle)
                {
                    return;
                }

                // リターンコード書きまくってるので無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理
                // でもだいじょうぶ
                if (selectCardIndexInfo.index != (int)IDType.NONE)
                {
                    MessageManager.Dispatch(fromPlayer.playerID, messageType, selectCardIndexInfo);
                }
            }

        }

        public override Result Execute(CardAbilityData abilityData)
        {
            switch (step)
            {
                //case 0:
                //    step++;
                //    // メッセージおくる
                //    //if (selectCardIndexInfo.index != (int)IDType.NONE)
                //    //{
                //    //    // ゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリ
                //    //    if ((timer += Time.deltaTime) > 10.0f)
                //    //    {
                //    //        MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectYamahuda, selectCardIndexInfo);
                //    //        step++;
                //    //    }
                //    //}
                //    //else step++;
                //    break;
                case 0:// メッセージ待ち
                    break;
                case 1:
                    // カードの動きが終わるまで待つ
                    if (!drawCard.isInMovingState())
                    {
                        step++;
                    }
                    break;

                case 2:
                    // どこに向かうか
                    {
                        //var skillData = abilityData.GetCurrentSkillData();

                        switch (toType)
                        {
                            case To.Hand:
                                toPlayer.deckManager.AddHand(drawCard);
                                break;
                            case To.Cemetery:
                                // データ上でカードを墓地に送る
                                toPlayer.deckManager.AddBochi(drawCard);
                                // カードを墓地に送る動き
                                //drawCard.MoveToCemetery();
                                break;
                            case To.Deck:
                                // (TODO)実装まだ
                                break;
                            case To.Tsuihou:
                                toPlayer.deckManager.AddExpulsion(drawCard);
                                break;
                            default:
                                Debug.LogError("ToTypeで想定されない値");
                                break;
                        }
                    }
                    step++;
                    break;

                case 3:
                    // カードの動きが終わるまで待つ
                    if (!drawCard.isInMovingState())
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
            switch (message.messageType)
            {
                case MessageType.SelectYamahuda:
                    {
                        SelectCardIndexInfo info = new SelectCardIndexInfo();
                        message.GetExtraInfo<SelectCardIndexInfo>(ref info);
                        var index = info.index;
                        Debug.Assert(index != (int)IDType.NONE, "カードサーチのIDが不正");
                        drawCard = fromPlayer.deckManager.DequeueYamahuda(index);
                        if (info.iMoveFlag == (int)CardMoveFlag.ShowDraw)
                            drawCard.ShowDraw(0.75f, false);
                        step++;
                    }
                    break;
                case MessageType.SelectHand:
                    {
                        SelectCardIndexInfo info = new SelectCardIndexInfo();
                        message.GetExtraInfo<SelectCardIndexInfo>(ref info);
                        var index = info.index;
                        Debug.Assert(index != (int)IDType.NONE, "カードサーチのIDが不正");
                        drawCard = fromPlayer.deckManager.DequeueHand(index);
                        drawCard.ShowDraw(0.75f, false);
                        step++;
                    }
                    break;

                case MessageType.SelectCemetery:
                    {
                        SelectCardIndexInfo info = new SelectCardIndexInfo();
                        message.GetExtraInfo<SelectCardIndexInfo>(ref info);
                        var index = info.index;
                        Debug.Assert(index != (int)IDType.NONE, "カードサーチのIDが不正");
                        drawCard = fromPlayer.deckManager.DequeCemetery(index);
                        drawCard.ShowDraw(0.75f, false);
                        step++;
                    }
                    break;
            }
            return false;
        }

        int SearchHand(SearchInfo info)
        {
            var handCards = fromPlayer.deckManager.GetHand();
            return Search(handCards, fromPlayer, info);
        }

        int SearchYamahuda(SearchInfo info)
        {
            var yamahudaCards = fromPlayer.deckManager.GetYamahuda();
            return Search(yamahudaCards, fromPlayer, info);
        }

        int SearchCemetery(SearchInfo info)
        {
            var cemeteryCards = fromPlayer.deckManager.GetCemeteryCards();
            return Search(cemeteryCards, fromPlayer, info);
        }

        int Search(List<CardData> cards, Player player, SearchInfo searchInfo)
        {
            // カードタイプに一致するカードを格納
            Debug.Log("カードタイプ検索するクマ");
            var buf = new List<int>();
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                switch (searchInfo.searchMask)
                {
                    case SearchMask.NoneLimit:
                        buf.Add(i);
                        break;
                    case SearchMask.AllStriker:
                        if (card.isStrikerCard())
                            buf.Add(i);
                        break;
                    case SearchMask.NoAbilityStriker:
                        if (card.cardType == CardType.Fighter)
                            buf.Add(i);
                        break;
                    case SearchMask.AbilityStriker:
                        if (card.cardType == CardType.AbilityFighter)
                            buf.Add(i);
                        break;
                    case SearchMask.Joker:
                        if (card.cardType == CardType.Joker)
                            buf.Add(i);
                        break;
                    case SearchMask.Support:
                        if (card.cardType == CardType.Support)
                            buf.Add(i);
                        break;
                    case SearchMask.Connect:
                        if (card.cardType == CardType.Connect)
                            buf.Add(i);
                        break;
                    case SearchMask.Intercept:
                        if (card.cardType == CardType.Intercept)
                            buf.Add(i);
                        break;
                    default:
                        break;
                }
            }
            // 何もヒットしなかったら終了
            if (buf.Count <= 0)
            {
                Debug.Log("ひっとしなかったクマ…");
                return (int)IDType.NONE;
            }

            Debug.Log(buf.Count + "件ヒットしたクマ");

            var searchIndex = (int)IDType.NONE;

            // 選択指定
            switch (searchInfo.selectType)
            {
                case SelectType.PlayerSelect:
                    searchIndex = PlayerSelectSearch(buf, cards, searchInfo);
                    break;

                case SelectType.AutoRandom:
                    var indices = AutoRandomSearch(buf, cards, searchInfo);
                    if (indices.Length > 0) searchIndex = indices[0];
                    break;

                case SelectType.Highest:
                    searchIndex = HighestSearch(buf, cards, searchInfo);
                    break;

                case SelectType.Lowest:
                    searchIndex = LowestSearch(buf, cards, searchInfo);
                    break;

                default:
                    Debug.LogWarning("SelectTypeおかしいクマ");
                    break;
            }

            buf.Clear();
            return searchIndex;
        }

        public int PlayerSelectSearch(List<int> buf, List<CardData> cards, SearchInfo searchInfo)
        {
            Debug.LogWarning("PlayerSelectSearchは未実装");
            return 0;
        }
        public int[] AutoRandomSearch(List<int> buf, List<CardData> cards, SearchInfo searchInfo)
        {
            List<int> searchIndices = new List<int>();
            foreach (int i in buf)
            {
                var searchOK = true;
                var card = cards[i];

                if (searchInfo.sPower != "")
                {
                    // パワー一致
                    var power = int.Parse(searchInfo.sPower);
                    Debug.Log("パワー一致するかみてみるクマ ");
                    if (card.power != power)
                    {
                        searchOK = false;
                        Debug.Log("してないグマ");
                    }
                    else Debug.Log("しテルクマ");
                }
                if (searchInfo.sSyuzoku != "")
                {
                    // 種族一致
                    var strikerCard = card.GetFighterCard();
                    if (strikerCard != null)
                    {
                        var syuzoku = (Syuzoku)int.Parse(searchInfo.sSyuzoku);
                        Debug.Log("種族一致するかみてみるクマ ");

                        var find = false;
                        foreach (Syuzoku s in strikerCard.syuzokus)
                        {
                            if (s == syuzoku)
                            {
                                find = true;
                                Debug.Log("しテルクマ");
                                break;
                            }
                        }

                        if (!find)
                        {
                            searchOK = false;
                            Debug.Log("してないグマ");
                        }
                    }
                }
                if (searchInfo.sName != "")
                {
                    Debug.Log("名前が含んでるかみてみるクマ ");

                    // 文字列含んでいるかどうか
                    if (!card.cardName.Contains(searchInfo.sName))
                    {
                        searchOK = false;
                        Debug.Log("含んでないクマ ");
                    }
                    else Debug.Log("含んでるクマ ");
                }
                if (searchOK)
                {
                    searchIndices.Add(i);
                    //break;
                }
            }
            return searchIndices.ToArray();
        }

        public int HighestSearch(List<int> buf, List<CardData> cards, SearchInfo searchInfo)
        {
            Debug.Log("パワーもっとも高いやつをサーチするクマ");

            var indices = AutoRandomSearch(buf, cards, searchInfo);

            var max = int.MinValue;
            var searchIndex = (int)IDType.NONE;

            foreach (int i in indices)
            {
                var card = cards[i];
                if (card.power > max)
                {
                    max = card.power;
                    searchIndex = i;
                }
            }

            return searchIndex;
        }

        public int LowestSearch(List<int> buf, List<CardData> cards, SearchInfo searchInfo)
        {
            Debug.Log("パワーもっとも低いやつをサーチするクマ");

            var indices = AutoRandomSearch(buf, cards, searchInfo);

            var min = int.MaxValue;
            var searchIndex = (int)IDType.NONE;

            foreach (int i in indices)
            {
                var card = cards[i];
                if (card.power < min)
                {
                    min = card.power;
                    searchIndex = i;
                }
            }

            return searchIndex;
        }
    }
}