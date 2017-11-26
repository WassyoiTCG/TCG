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

            return pMain.playerManager.HandleMessage(message);
        }
    }


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
                pMain.stateMachine.ChangeState(PointDraw.GetInstance());
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

    public class OneDraw : BaseEntityState<SceneMain>
    {
        // Singleton.
        static OneDraw instance;
        public static OneDraw GetInstance() { if (instance == null) { instance = new OneDraw(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            if (pMain.turn == 0)
            {
                pMain.playerManager.Draw();
            }
            pMain.turn++;
            if (pMain.turn % 3 == 0)
            {
                pMain.playerManager.Draw();
            }
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

    public class SetStriker : BaseEntityState<SceneMain>
    {
        static SetStriker instance;
        public static SetStriker GetInstance() { if (instance == null) { instance = new SetStriker(); } return instance; }

        readonly float setLimitTime = 60 + 1;   // ストライカーセットする制限時間
        float timer;                            // 時間用変数

        Text limitTimeText;
        Image eventTimeGauge;    // イベント時間ゲージ

        public override void Enter(SceneMain pMain)
        {
            pMain.playerManager.SetState(PlayerState.SetStriker.GetInstance());

            timer = setLimitTime;

            // まずプレイヤーがストライカーをセットできる状況か判断。してボタンを表示
            pMain.uiManager.AppearStrikerOK(pMain.playerManager.GetMyPlayer().isHaveStrikerCard());

            if (limitTimeText == null) limitTimeText = pMain.uiManager.limitTimeText;
            if (eventTimeGauge == null) eventTimeGauge = pMain.uiManager.eventTimeGauge;
        }

        public override void Execute(SceneMain pMain)
        {
            // 互いがストライカーセットし終わっていたら
            if (pMain.playerManager.isSetStrikerEnd())
            {
                timer = 0;

                // ステートチェンジ
                pMain.stateMachine.ChangeState(BeforeStrikerOpen.GetInstance());
            }

            // 時間減算処理
            if (timer > 0)
            {
                if ((timer -= Time.deltaTime) < 0)
                {
                    timer = 0;

                    // 時間切れメッセージ
                    //MessageManager.Dispatch(pMain.playerManager.GetMyPlayerID(), MessageType.SetStrikerTimeOver, null);

                    // 時間切れ処理
                    pMain.playerManager.GetMyPlayer().SetStrikerTimeOver();
                }
            }

            // UIの時間表示
            var minutes = (int)timer / 60;
            var second = (int)timer % 60;
            limitTimeText.text = minutes + ":" + second.ToString("00");

            // UIのゲージ設定
            var timeRate = timer / setLimitTime;
            eventTimeGauge.fillAmount = timeRate;
        }

        public override void Exit(SceneMain pMain)
        {
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


    public class BeforeStrikerOpen : BaseEntityState<SceneMain>
    {
        // Singleton.
        static BeforeStrikerOpen instance;
        public static BeforeStrikerOpen GetInstance() { if (instance == null) { instance = new BeforeStrikerOpen(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            // ステートチェンジ
            pMain.stateMachine.ChangeState(StrikerOpen.GetInstance());
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
        }

        public override void Execute(SceneMain pMain)
        {
            if ((timer += Time.deltaTime) > 2)
            {
                // ステートチェンジ
                pMain.stateMachine.ChangeState(AfterStrikeOpen.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    public class AfterStrikeOpen : BaseEntityState<SceneMain>
    {
        // Singleton.
        static AfterStrikeOpen instance;
        public static AfterStrikeOpen GetInstance() { if (instance == null) { instance = new AfterStrikeOpen(); } return instance; }

        //float timer;

        public override void Enter(SceneMain pMain)
        {
            // 時間初期化
            //timer = 0;
        }

        public override void Execute(SceneMain pMain)
        {
            //if ((timer += Time.deltaTime) > 3)
            {
                // ステートチェンジ
                pMain.stateMachine.ChangeState(StrikerBattleResult.GetInstance());
            }
        }

        public override void Exit(SceneMain pMain)
        { }

        public override bool OnMessage(SceneMain pMain, MessageInfo message)
        {
            return false;
        }
    }

    public class StrikerBattleResult : BaseEntityState<SceneMain>
    {
        // Singleton.
        static StrikerBattleResult instance;
        public static StrikerBattleResult GetInstance() { if (instance == null) { instance = new StrikerBattleResult(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            var winnerPlayerID = pMain.playerManager.StrikerBattle();
            if (winnerPlayerID != -1)
            {
                pMain.aikoPoint.Clear();
                if (pMain.uiManager.AddScore(pMain.playerManager.players[winnerPlayerID].isMyPlayer, pMain.currentPoint))
                {
                    pMain.Finish();
                    return;
                }
            }
            else if (pMain.aikoPoint.Count >= 2)
                pMain.aikoPoint.Clear();
            else
                pMain.aikoPoint.Add(pMain.pointManager.GetCurrentPoint());

            // ステートチェンジ
            pMain.stateMachine.ChangeState(TurnEnd.GetInstance());
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

    public class TurnEnd : BaseEntityState<SceneMain>
    {
        // Singleton.
        static TurnEnd instance;
        public static TurnEnd GetInstance() { if (instance == null) { instance = new TurnEnd(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            // ステートチェンジ
            pMain.playerManager.SetState(PlayerState.TurnEnd.GetInstance());
            pMain.stateMachine.ChangeState(PointDraw.GetInstance());
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

    public class Finish : BaseEntityState<SceneMain>
    {
        // Singleton.
        static Finish instance;
        public static Finish GetInstance() { if (instance == null) { instance = new Finish(); } return instance; }

        public override void Enter(SceneMain pMain)
        {
            if(MessageManager.isServer || !SelectData.isNetworkBattle)
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