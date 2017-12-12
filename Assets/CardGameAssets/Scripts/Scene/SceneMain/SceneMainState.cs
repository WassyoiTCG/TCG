using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
    // テンプレ
    public class A : BaseEntityState<SceneMain>
    {
        // Singleton.
        static A instance;
        public static A GetInstance() { if (instance == null) { instance = new A(); } return instance; }

        public override void Enter(SceneMain pMain)
        {}

        public override void Execute(SceneMain pMain)
        {}

        public override void Exit(SceneMain pMain)
        {}

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }
 */

namespace SceneMainState
{
    public class Global : BaseEntityState<SceneMain>
    {
        // Singleton.
        static Global instance;
        public static Global GetInstance() { if (instance == null) { instance = new Global(); } return instance; }

        public override void Enter(SceneMain pMain)
        { }

        public override void Execute(SceneMain pMain)
        { }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            switch (message.messageType)
            {
                case MessageType.Restart:
                    pMain.Restart();
                    break;

                case MessageType.EndGame:
                    //Application.Quit();
                    // ネットワーク終了
                    if(SelectData.isNetworkBattle)
                    {
                        pMain.networkManager.Stop();
                    }
                    // メニューに戻る
                    SceneManager.LoadScene("Menu");
                    break;

                case MessageType.SyncPoint:
                    {
                        if (message.exInfo == null)
                            return false;

                        // byte[]→構造体
                        PointInfo pointInfo = new PointInfo();
                        //IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(pointInfo));
                        //Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(pointInfo));
                        //pointInfo = (PointInfo)Marshal.PtrToStructure(ptr, pointInfo.GetType());
                        //Marshal.FreeHGlobal(ptr);
                        message.GetExtraInfo<PointInfo>(ref pointInfo);

                        pMain.pointManager.randomIndexArray = pointInfo.points;
                    }
                    return false;
            }
            if (pMain.playerManager.HandleMessage(message)) return true;
            if (pMain.abilityManager.HandleMessage(message)) return true;

            return false;
        }
    }


    // ネットワークでプレイヤーが2人揃うまで待つステート
    public class MatchingWait : BaseEntityState<SceneMain>
    {
        // Singleton.
        static MatchingWait instance;
        public static MatchingWait GetInstance() { if (instance == null) { instance = new MatchingWait(); } return instance; }

        public override void Enter(SceneMain pMain)
        {

        }

        public override void Execute(SceneMain pMain)
        {
            if (pMain.playerManager.isPlayesStandbyOK())
            {
                pMain.uiManager.DisAppearMatchingWait();
                Debug.Log("マッチング完了");

                // プレイヤー初期化
                pMain.playerManager.Restart();

                // ステートチェンジ
                pMain.stateMachine.ChangeState(BattleStart.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        {
            pMain.uiManager.DisAppearMatchingWait();
        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // バトルスタートって出るステート
    public class BattleStart : BaseEntityState<SceneMain>
    {
        // Singleton.
        static BattleStart instance;
        public static BattleStart GetInstance() { if (instance == null) { instance = new BattleStart(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            // 自分がホストならポイントデータを相手に送信
            if (MessageManager.isServer)
            {
                PointInfo exInfo = new PointInfo();
                exInfo.points = new int[10];
                exInfo.points = pMain.pointManager.randomIndexArray;
                MessageManager.Dispatch(pMain.playerManager.GetMyPlayerID(), MessageType.SyncPoint, exInfo);
            }
        }

        public override void Execute(SceneMain pMain)
        {
            if (pMain.playerManager.isPlayesStandbyOK())
            {
                // ステートチェンジ
                pMain.stateMachine.ChangeState(SyncDeck.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        {
        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // 互いのデッキを同期するステート
    public class SyncDeck : BaseEntityState<SceneMain>
    {
        // Singleton.
        static SyncDeck instance;
        public static SyncDeck GetInstance() { if (instance == null) { instance = new SyncDeck(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            // 自分のデッキデータを送り、送信しあう
            if(!MessageManager.isServer)
            {
                pMain.playerManager.GetCPUPlayer().SendSyncDeckInfo();
            }
            pMain.playerManager.GetMyPlayer().SendSyncDeckInfo();
        }

        public override void Execute(SceneMain pMain)
        {
            // 互いにデッキデータの同期が終わったら
            if (pMain.playerManager.isSyncDeckOK())
            {
                // ステートチェンジ
                pMain.stateMachine.ChangeState(FirstDraw.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        {
            // デッキ同期フラグリセット
            pMain.playerManager.SyncDeckOff();
        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // 最初のドローのステート
    public class FirstDraw : BaseEntityState<SceneMain>
    {
        // Singleton.
        static FirstDraw instance;
        public static FirstDraw GetInstance() { if (instance == null) { instance = new FirstDraw(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            pMain.uiManager.AppearFirstDraw();
            // プレイヤーにドローさせる
            pMain.playerManager.SetState(PlayerState.FirstDraw.GetInstance());
        }

        public override void Execute(SceneMain pMain)
        {
            if (pMain.playerManager.isStateEnd()/* && pMain.playerManager.isSyncDeckOK()*/)
            {
                // ステートチェンジ
                pMain.stateMachine.ChangeState(MainPhase.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        {
            // めいんUI表示
            pMain.uiManager.AppearMainUI();

            // デッキ同期フラグリセット
            pMain.playerManager.SyncDeckOff();
        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    public class MainPhase : BaseEntityState<SceneMain>
    {
        // Singleton.
        static MainPhase instance;
        public static MainPhase GetInstance() { if (instance == null) { instance = new MainPhase(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            
            // メインフェーズエフェクト発動
            pMain.turnEndEffect.Action(PHASE_TYPE.MAIN);
        }

        public override void Execute(SceneMain pMain)
        {
            // 演出終わったら
            if(pMain.turnEndEffect.GetTrunEndType() == PHASE_TYPE.LEST)
            {
                // ポイントドローステートへ
                pMain.stateMachine.ChangeState(PointDraw.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // ポイント(ダメージ)の数値更新のステート
    public class PointDraw : BaseEntityState<SceneMain>
    {
        // Singleton.
        static PointDraw instance;
        public static PointDraw GetInstance() { if (instance == null) { instance = new PointDraw(); } return instance; }

        public override void Enter(SceneMain pMain)
        { }

        public override void Execute(SceneMain pMain)
        {
            // ポイント
            if (pMain.pointManager.Next())
            {
                pMain.Finish();
            }
            else
            {
                pMain.currentPoint = pMain.pointManager.GetCurrentPoint();
                pMain.pointText.text = pMain.currentPoint.ToString();
                pMain.restPointText.text = (pMain.pointManager.step + 1).ToString();
                // あいこ分を足す
                foreach (int add in pMain.aikoPoint)
                {
                    pMain.currentPoint += add;
                    pMain.pointText.text += " + " + add;
                }
                // ステートチェンジ
                pMain.stateMachine.ChangeState(OneDraw.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // 3ターンごとなら1ドローするステート
    public class OneDraw : BaseEntityState<SceneMain>
    {
        // Singleton.
        static OneDraw instance;
        public static OneDraw GetInstance() { if (instance == null) { instance = new OneDraw(); } return instance; }

        public override void Enter(SceneMain pMain)
        {

 
            //// 初回ドロー
            //if (pMain.turn++ == 0)
            //{
            //    pMain.playerManager.SetState(PlayerState.Draw.GetInstance());
            //    return;
            //}
            // 2ターンごとドロー
            if (++pMain.turn % 2 == 1)
            {           
                //  SE
                oulAudio.PlaySE("card_draw1");

                pMain.playerManager.SetState(PlayerState.Draw.GetInstance());
                return;
            }
            // ドローなしなのでステートチェンジ
            pMain.playerManager.SetState(PlayerState.None.GetInstance());
        }

        public override void Execute(SceneMain pMain)
        {
            if (pMain.playerManager.isStateEnd())
            {
                // ステートチェンジ
                pMain.stateMachine.ChangeState(SetStriker.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // ストライカーをセットするステート
    public class SetStriker : BaseEntityState<SceneMain>
    {
        static SetStriker instance;
        public static SetStriker GetInstance() { if (instance == null) { instance = new SetStriker(); } return instance; }

        readonly float setLimitTime = 60 + 1;   // ストライカーセットする制限時間

        public override void Enter(SceneMain pMain)
        {
            pMain.playerManager.SetState(PlayerState.SetStriker.GetInstance());
            
            // まずプレイヤーがストライカーをセットできる状況か判断。してボタンを表示
            // 出せるカードがなかった！
            if (pMain.playerManager.GetMyPlayer().isHaveStrikerCard() == false)
            {
                Debug.Log(" 出せるカードがなかった！ - SetStriker");
                pMain.uiManager.EnableSetStrikerButton();
            }
            // タイマーセット
            pMain.uiManager.SetTimer(setLimitTime);
        }

        public override void Execute(SceneMain pMain)
        {
            // 互いがストライカーセットし終わっていたら
            if (pMain.playerManager.isSetStrikerEnd())
            {
                // ステートチェンジ
                pMain.stateMachine.ChangeState(WaitToBattlePhase.GetInstance());
            }

            //// 時間切れなったら
            //if (pMain.uiManager.isTimerEnd())
            //{
            //    // 時間切れメッセージ
            //    //MessageManager.Dispatch(pMain.playerManager.GetMyPlayerID(), MessageType.SetStrikerTimeOver, null);

            //    // 効果処理中なら待つ
            //    if (!pMain.abilityManager.isAbilityEnd()) return;

            //    // 時間切れ処理
            //    pMain.playerManager.GetMyPlayer().SetStrikerTimeOver();
            //}
        }

        public override void Exit(SceneMain pMain)
        {
            // タイマーストップ
            pMain.uiManager.StopTimer();

            // プレイヤーのステート変更
            pMain.playerManager.SetState(PlayerState.None.GetInstance());

            // インフォメーションUI非表示
            pMain.uiManager.DisAppearBattleCardInfomation();
        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }


    // （仮）ウェイト用
    public class WaitToBattlePhase : BaseEntityState<SceneMain>
    {
        // Singleton.
        static WaitToBattlePhase instance;
        public static WaitToBattlePhase GetInstance() { if (instance == null) { instance = new WaitToBattlePhase(); } return instance; }

        public override void Enter(SceneMain pMain)
        {

            pMain.iWaitFrame = 0; // 初期化

        }

        public override void Execute(SceneMain pMain)
        {
            // (仮)
            pMain.iWaitFrame++; // 
            if (pMain.iWaitFrame >= 24)
            {
                // ステートチェンジ
                pMain.stateMachine.ChangeState(BattlePhase.GetInstance());

            }

        }

        public override void Exit(SceneMain pMain)
        {

        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }


    }

    // バトルフェーズエフェクトを発動するステート
    public class BattlePhase : BaseEntityState<SceneMain>
    {
        // Singleton.
        static BattlePhase instance;
        public static BattlePhase GetInstance() { if (instance == null) { instance = new BattlePhase(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            // バトルフェーズエフェクト発動
            pMain.turnEndEffect.Action(PHASE_TYPE.BATTLE);
        }

        public override void Execute(SceneMain pMain)
        {
            // 演出終わったら
            if (pMain.turnEndEffect.GetTrunEndType() == PHASE_TYPE.LEST)
            {
                // カードオープンステートへ
                pMain.stateMachine.ChangeState(StrikerOpen.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }


    // ストライカーを開けるステート
    public class StrikerOpen : BaseEntityState<SceneMain>
    {
        // Singleton.
        static StrikerOpen instance;
        public static StrikerOpen GetInstance() { if (instance == null) { instance = new StrikerOpen(); } return instance; }

        float timer;

        public override void Enter(SceneMain pMain)
        {
            // 裏のカードを表にする
            pMain.playerManager.OpenStriker();

            // 時間初期化
            timer = 0;

            pMain.iWaitFrame = 0; // 初期化
        }

        public override void Execute(SceneMain pMain)
        {
        // ひっくり返る動きが終わったら
            if (!pMain.playerManager.GetMyPlayer().cardObjectManager.isInMovingState() && 
                !pMain.playerManager.GetCPUPlayer().cardObjectManager.isInMovingState())
            {

                // SE
                oulAudio.PlaySE("Ketudoramu");

                //+-------------- 
                // 演出
                //+--------------
                // nullじゃなかったら
                Card MyFiledCard = pMain.playerManager.GetMyPlayer().cardObjectManager.fieldStrikerCard;
                if (MyFiledCard  != null)
                {
                    Vector3 MyPos = MyFiledCard.cacheTransform.localPosition;
                    Vector3 MyAngle = MyFiledCard.cacheTransform.localEulerAngles;
                    pMain.UvEffectMgr_My.Action(UV_EFFECT_TYPE.SUMMON, MyPos, MyAngle);
                    // 効果持ちファイターなら
                    if (MyFiledCard.cardData.cardType == CardType.AbilityFighter)
                    {
                        pMain.PanelEffectMgr_My.Action(PANEL_EFFECT_TYPE.ABILITY, MyPos, MyAngle);
                        pMain.PanelEffectMgr_My.Action(PANEL_EFFECT_TYPE.STAR, MyPos, MyAngle);
                    }

                    //pMain.UvEffectMgr.GetComponent<UVEffectManager>(). Action(UV_EFFECT_TYPE.DOWN_STATUS, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

                }
                // 相手もやる
                // nullじゃなかったら
                Card CpuFiledCard = pMain.playerManager.GetCPUPlayer().cardObjectManager.fieldStrikerCard;
                if (CpuFiledCard != null)
                {
                    Vector3 CpuPos = CpuFiledCard.cacheTransform.localPosition;
                    Vector3 CpuAngle = CpuFiledCard.cacheTransform.localEulerAngles;
                    pMain.UvEffectMgr_Cpu.Action(UV_EFFECT_TYPE.SUMMON, CpuPos, CpuAngle);
                    // 効果持ちファイターなら
                    if (CpuFiledCard.cardData.cardType == CardType.AbilityFighter)
                    {
                        pMain.PanelEffectMgr_Cpu.Action(PANEL_EFFECT_TYPE.ABILITY, CpuPos, CpuAngle);
                        pMain.PanelEffectMgr_My.Action(PANEL_EFFECT_TYPE.STAR, CpuPos, CpuAngle);
                    }

                    //pMain.UvEffectMgr.GetComponent<UVEffectManager>().Action(UV_EFFECT_TYPE.DOWN_STATUS, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

                }

                    // ステートチェンジ
                    pMain.stateMachine.ChangeState(WaitToSetIntercept.GetInstance());
                

            }
        }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // ウェイト用
    public class WaitToSetIntercept : BaseEntityState<SceneMain>
    {      
        // Singleton.
        static WaitToSetIntercept instance;
        public static WaitToSetIntercept GetInstance() { if (instance == null) { instance = new WaitToSetIntercept(); } return instance; }
        
        public override void Enter(SceneMain pMain)
        {
        
            pMain.iWaitFrame = 0; // 初期化

        }

        public override void Execute(SceneMain pMain)
        {
             // (仮)
              pMain.iWaitFrame++; // 
            if (pMain.iWaitFrame >= 30)
            {
                // ステートチェンジ
                //pMain.stateMachine.ChangeState(CardAttack.GetInstance());
                pMain.stateMachine.ChangeState(SetIntercept.GetInstance());

            }

        }

        public override void Exit(SceneMain pMain)
        {

        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }


    }
    // インターセプトカードをセットするステート
    public class SetIntercept : BaseEntityState<SceneMain>
    {
        // Singleton.
        static SetIntercept instance;
        public static SetIntercept GetInstance() { if (instance == null) { instance = new SetIntercept(); } return instance; }

        //public enum SET_INTERCEPT_STEP
        //{
        //    FIRST,/*PLAY_CARD,*/END
        //};

        //SET_INTERCEPT_STEP eStep;

        readonly float setLimitTime = 10 + 1;   // インターセプトセットする制限時間

        public override void Enter(SceneMain pMain)
        {
            //eStep = SET_INTERCEPT_STEP.FIRST;

            // プレイヤーステート変更(インターセプトセットステート)
            pMain.playerManager.SetState(PlayerState.SetIntercept.GetInstance());

            //pMain.uiManager.AppearStrikerOK(false);  // falseにするとパスが出る
            pMain.uiManager.AppearInterceptOK();// [1211] NextButton

            // UIに時間セット
            pMain.uiManager.SetTimer(setLimitTime);
        }

        public override void Execute(SceneMain pMain)
        {

            // 時間切れなったら
            if (pMain.uiManager.isTimerEnd())
            {
                //eStep = SET_INTERCEPT_STEP.END;
                // インターセプト発動へ
                pMain.stateMachine.ChangeState(WaitToActionIntercept.GetInstance());

                return;
            }
            // 2人準備完了(NextButtonを押したら)
            if (pMain.playerManager.isPushedNextButton())
            {
                // タイマーストップ
                pMain.uiManager.StopTimer();

                // インターセプト発動へ
                pMain.stateMachine.ChangeState(WaitToActionIntercept.GetInstance());

                //eStep = SET_INTERCEPT_STEP.END;
            }

            //switch (eStep)
            //{
            //    case SET_INTERCEPT_STEP.FIRST:
            //        // 時間切れなったら
            //        if (pMain.uiManager.isTimerEnd())
            //        {
            //            eStep = SET_INTERCEPT_STEP.END;
            //            return;
            //        }
            //        // 2人準備完了(NextButtonを押したら)
            //        if (pMain.playerManager.isPushedNextButton())
            //        {
            //            // タイマーストップ
            //            pMain.uiManager.StopTimer();

            //            // インターセプト発動へ
            //             pMain.stateMachine.ChangeState(ActionIntercept.GetInstance());

            //            //eStep = SET_INTERCEPT_STEP.END;
            //        }
            //        break;

            //    case SET_INTERCEPT_STEP.END:

            //        // 効果を発動させる
            //        //pMain.playerManager.ActionIntercept();
            //        // = SET_INTERCEPT_STEP.END;
            //        break;

            //    //case SET_INTERCEPT_STEP.END:
            //    //    // 効果処理が終わったら
            //    //    if (pMain.abilityManager.isAbilityEnd())
            //    //    {
            //    //        // ステートチェンジ
            //    //        //pMain.stateMachine.ChangeState(StrikerBattleResult.GetInstance());

            //    //        // モンスターアタックモーションへ
            //    //        pMain.stateMachine.ChangeState(CardAttack.GetInstance());
            //    //    }
            //    //    break;
            //}
        }

        public override void Exit(SceneMain pMain)
        {
            //  NEXT Buton消す
            pMain.uiManager.DisableSetStrikerButton();// [1211] NextButton

            // タイマーストップ
            pMain.uiManager.StopTimer();
        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {



            return false;
        }
    }// SetIntercept



    // （仮）ウェイト用
    public class WaitToActionIntercept : BaseEntityState<SceneMain>
    {
        // Singleton.
        static WaitToActionIntercept instance;
        public static WaitToActionIntercept GetInstance() { if (instance == null) { instance = new WaitToActionIntercept(); } return instance; }

        public override void Enter(SceneMain pMain)
        {

            pMain.iWaitFrame = 0; // 初期化

        }

        public override void Execute(SceneMain pMain)
        {
            // (仮)
            pMain.iWaitFrame++; // 
            if (pMain.iWaitFrame >= 24)
            {
                // ステートチェンジ
                pMain.stateMachine.ChangeState(ActionInterceptState.GetInstance());

            }

        }

        public override void Exit(SceneMain pMain)
        {

        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }


    }

    // インターセプトカードをセットするステート
    public class ActionInterceptState : BaseEntityState<SceneMain>
    {
        // Singleton.
        static ActionInterceptState instance;
        public static ActionInterceptState GetInstance() { if (instance == null) { instance = new ActionInterceptState(); } return instance; }
        
        public override void Enter(SceneMain pMain)
        {
            // 効果を発動させる!
            pMain.playerManager.ActionIntercept();
        }

        public override void Execute(SceneMain pMain)
        {
            // 効果処理が終わったら
            if (pMain.abilityManager.isAbilityEnd())
            {
                // ステートチェンジ
                //pMain.stateMachine.ChangeState(StrikerBattleResult.GetInstance());

                // モンスターアタックモーションへ
                pMain.stateMachine.ChangeState(CardAttack.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        {

        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {

            return false;
        }
    }// ActionIntercept



    // カードの攻撃モーションが発動するステート
    public class CardAttack : BaseEntityState<SceneMain>
    {
        // Singleton.
        static CardAttack instance;
        public static CardAttack GetInstance() { if (instance == null) { instance = new CardAttack(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            // ストライカー攻撃モーション発動
            pMain.playerManager.AttackStriker();
            
            //pMain.iWaitFrame = 0; // 初期化

        }

        public override void Execute(SceneMain pMain)
        {

                // 今は速攻でインターセプトセットステートに行く
                //pMain.stateMachine.ChangeState(SetIntercept.GetInstance());
                pMain.stateMachine.ChangeState(StrikerBattleResult.GetInstance());

        }

        public override void Exit(SceneMain pMain)
        {
           
        }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    //// インターセプトカードをセットするステート
    //public class SetIntercept : BaseEntityState<SceneMain>
    //{
    //    // Singleton.
    //    static SetIntercept instance;
    //    public static SetIntercept GetInstance() { if (instance == null) { instance = new SetIntercept(); } return instance; }

    //    int step;

    //    readonly float setLimitTime = 5 + 1;   // インターセプトセットする制限時間

    //    public override void Enter(SceneMain pMain)
    //    {
    //        step = 0;

    //        // 誰もインターセプトを持っていなかったら飛ばす
    //        if (!pMain.playerManager.isHaveInterceptCard())
    //        {
    //            // ステートチェンジ
    //            pMain.stateMachine.ChangeState(StrikerBattleResult.GetInstance());

    //            return;
    //        }

    //        // プレイヤーステート変更(インターセプトセットステート)
    //        pMain.playerManager.SetState(PlayerState.SetIntercept.GetInstance());

    //        var myPlayer = pMain.playerManager.GetMyPlayer();
    //        // 自分がインターセプト持ってなかったら最初からボタン押した扱いUIのボタンを表示
    //        if (!myPlayer.isHaveInterceptCard())
    //        {
    //            myPlayer.isPushedJunbiKanryo = true;
    //            return;
    //        }

    //        //pMain.uiManager.AppearStrikerOK(false);  // falseにするとパスが出る
    //        pMain.uiManager.AppearInterceptOK();// [1211] NextButton

    //        // UIに時間セット
    //        pMain.uiManager.SetTimer(setLimitTime);
    //    }

    //    public override void Execute(SceneMain pMain)
    //    {
    //        switch(step)
    //        {
    //            case 0:
    //                // 時間切れなったら
    //                if(pMain.uiManager.isTimerEnd())
    //                {
    //                    step++;
    //                    return;
    //                }
    //                // 2人準備完了(パスだったり出してたり)
    //                if(pMain.playerManager.isPushedJunbiKanryo())
    //                {
    //                    // タイマーストップ
    //                    pMain.uiManager.StopTimer();
    //                    step++;
    //                }
    //                break;

    //            case 1:
    //                // 効果を発動させる
    //                pMain.playerManager.ActionIntercept();
    //                step++;
    //                break;

    //            case 2:
    //                // 効果処理が終わったら
    //                if (pMain.abilityManager.isAbilityEnd())
    //                {
    //                    // ステートチェンジ
    //                    pMain.stateMachine.ChangeState(StrikerBattleResult.GetInstance());
    //                }
    //                break;
    //        }
    //    }

    //    public override void Exit(SceneMain pMain)
    //    {
    //        //  NEXT Buton消す
    //        pMain.uiManager.DisableSetStrikerButton();// [1211] NextButton

    //        // タイマーストップ
    //        pMain.uiManager.StopTimer();
    //    }

    //    public override bool OnMessage(SceneMain pMain, MessageInfo message)
    //    {
    //        return false;
    //    }
    //}// SetIntercept

    // バトルの結果を清算するステート
    public class StrikerBattleResult : BaseEntityState<SceneMain>
    {
        // Singleton.
        static StrikerBattleResult instance;
        public static StrikerBattleResult GetInstance() { if (instance == null) { instance = new StrikerBattleResult(); } return instance; }

        int iLoserNo = 0;
        bool endFlag = false;

        public override void Enter(SceneMain pMain)
        {
            pMain.iWaitFrame = 0; // 初期化

            var winnerPlayerID = pMain.playerManager.StrikerBattle();
            if (winnerPlayerID != -1)
            {
                // 負けてる方のIDを取得
                iLoserNo = (winnerPlayerID == 0) ? 1 : 0;

                pMain.aikoPoint.Clear();
                
                endFlag = pMain.uiManager.Damage(!pMain.playerManager.players[winnerPlayerID].isMyPlayer, pMain.currentPoint);
                //if (pMain.uiManager.Damage(!pMain.playerManager.players[winnerPlayerID].isMyPlayer, pMain.currentPoint))
                //{
                //    pMain.Finish();
                //
                return;
            }

            iLoserNo = -1;   // 相殺

            // ここからあいこの処理
            if (pMain.aikoPoint.Count >= 2)
                pMain.aikoPoint.Clear();
            else
                pMain.aikoPoint.Add(pMain.pointManager.GetCurrentPoint());

        }

        public override void Execute(SceneMain pMain)
        {
            // ゴレイヌが書いたソースコード
            const int ATTACK_FRAME = 22;
            pMain.iWaitFrame++; // 更新
            if (pMain.iWaitFrame == ATTACK_FRAME)
            {
                // どっち勝かってる
                if (iLoserNo != -1)
                {
                    Card FieldCard = pMain.playerManager.GetPlayer(iLoserNo).GetFieldStrikerCard();
                    if (FieldCard != null)
                    {
                        // やられモーションへ
                        FieldCard.Lose();

                        // +----------------------
                        //  演出
                        // +-----------------------

                        // SE
                        oulAudio.PlaySE("Attack_Middle");

                        Vector3 pos = pMain.playerManager.players[iLoserNo].GetFieldStrikerCard().cacheTransform.localPosition;
                        pos.y += 1;
                        Vector3 vec = pMain.playerManager.players[iLoserNo].GetFieldStrikerCard().cacheTransform.localPosition;

                        pMain.PanelEffectMgr_My.Action(PANEL_EFFECT_TYPE.ORANGE_LIGHT, pos, vec);
                        pMain.PanelEffectMgr_My.Action(PANEL_EFFECT_TYPE.DAMAGE, pos, vec);
                        //pMain.playerManager.GetPlayer(iLoserNo).GetFieldStrikerCard().Lose();
                    }
                }else 
                {
                    Card FieldCard1 = pMain.playerManager.GetPlayer(0).GetFieldStrikerCard();
                    Card FieldCard2 = pMain.playerManager.GetPlayer(1).GetFieldStrikerCard();
                    if (FieldCard1 != null && FieldCard2 != null)
                    {
                        // 相子はどっちもダメージモーション
                        pMain.playerManager.GetPlayer(0).GetFieldStrikerCard().Lose();
                        pMain.playerManager.GetPlayer(1).GetFieldStrikerCard().Lose();

                        // SE
                        oulAudio.PlaySE("Attack_Middle");

                        // +----------------------
                        //  演出
                        // +-----------------------
                        Vector3 pos = pMain.playerManager.players[0].GetFieldStrikerCard().cacheTransform.localPosition;
                        pos.y += 1;
                        Vector3 vec = pMain.playerManager.players[0].GetFieldStrikerCard().cacheTransform.localPosition;

                        pMain.PanelEffectMgr_My.Action(PANEL_EFFECT_TYPE.ORANGE_LIGHT, pos, vec);
                        pMain.PanelEffectMgr_My.Action(PANEL_EFFECT_TYPE.DAMAGE, pos, vec);

                    }
                }

            }
            


            if (pMain.iWaitFrame >= 60)
            {
               // この時点で勝っていたら効果は発動しない。
                if (endFlag)
                {
                    pMain.Finish();
                }

                else
                {
                    // ステートチェンジ
                    pMain.stateMachine.ChangeState(StrikerAfterBattleAbility.GetInstance());
                }
            }

         }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // ストライカーのアビリティ(バトル後)が発動するステート
    public class StrikerAfterBattleAbility : BaseEntityState<SceneMain>
    {
        // Singleton.
        static StrikerAfterBattleAbility instance;
        public static StrikerAfterBattleAbility GetInstance() { if (instance == null) { instance = new StrikerAfterBattleAbility(); } return instance; }

        //float timer;

        public override void Enter(SceneMain pMain)
        {
            // プレイヤーステート変更(効果持ちモンスターのバトル後発動効果の処理)
            pMain.playerManager.SetState(PlayerState.StrikerAfterBattleAbility.GetInstance());
        }

        public override void Execute(SceneMain pMain)
        {
            // 効果処理が終わったら
            if (pMain.abilityManager.isAbilityEnd())
            {
                // ステートチェンジ
                pMain.stateMachine.ChangeState(TurnEnd.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }



    // ターンエンド(出したストライカーを墓地に送ったり)
    public class TurnEnd : BaseEntityState<SceneMain>
    {
        // Singleton.
        static TurnEnd instance;
        public static TurnEnd GetInstance() { if (instance == null) { instance = new TurnEnd(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            // ステートチェンジ
            pMain.playerManager.SetState(PlayerState.TurnEnd.GetInstance());

            // メインフェーズ演出に戻る
            pMain.stateMachine.ChangeState(MainPhase.GetInstance());
        }

        public override void Execute(SceneMain pMain)
        { }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // 勝ったぜ
    public class Winner : BaseEntityState<SceneMain>
    {
        // Singleton.
        static Winner instance;
        public static Winner GetInstance() { if (instance == null) { instance = new Winner(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            // Winnerエフェクト発動
            pMain.turnEndEffect.Action(PHASE_TYPE.WINNER);

            //  SE
            oulAudio.PlaySE("Win");

        }

        public override void Execute(SceneMain pMain)
        {
            // 演出終わったら
            if (pMain.turnEndEffect.GetTrunEndType() == PHASE_TYPE.LEST)
            {
                // 終了ステートへ
                pMain.stateMachine.ChangeState(Finish.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // ルナの負けだよ
    public class Loser : BaseEntityState<SceneMain>
    {
        // Singleton.
        static Loser instance;
        public static Loser GetInstance() { if (instance == null) { instance = new Loser(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            // Loserエフェクト発動
            pMain.turnEndEffect.Action(PHASE_TYPE.LOSER);
        }

        public override void Execute(SceneMain pMain)
        {
            // 演出終わったら
            if (pMain.turnEndEffect.GetTrunEndType() == PHASE_TYPE.LEST)
            {
                // 終了ステートへ
                pMain.stateMachine.ChangeState(Finish.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    // ゲーム終了ステート
    public class Finish : BaseEntityState<SceneMain>
    {
        // Singleton.
        static Finish instance;
        public static Finish GetInstance() { if (instance == null) { instance = new Finish(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            // 演出終り 
            pMain.turnEndEffect.Stop();

            if (MessageManager.isServer || !SelectData.isNetworkBattle)
            {
                pMain.uiManager.AppearEndGameUI();
            }
        }

        public override void Execute(SceneMain pMain)
        { }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }
}