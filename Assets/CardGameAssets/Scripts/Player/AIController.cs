using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    Player myPlayer;                  // コントロールするプレイヤーの実体
    bool isMarigan;
    PointCardManager pointManager;

    void Start()
    {
        pointManager = GameObject.Find("GameMain").GetComponent<SceneMain>().pointManager;
        myPlayer = GetComponent<Player>();
        //myPlayer.playerName = "CPU";
        Restart();
    }

    public void Restart()
    {
        isMarigan = false;
    }

    // Update is called once per frame
    void Update ()
    {
        if(myPlayer.stateMachine.isInState(PlayerState.FirstDraw.GetInstance()))
        {
            if (!isMarigan)
            {
                // 絶対マリガンしないマン
                MessageManager.Dispatch(myPlayer.playerID, MessageType.NoMarigan, null);
                isMarigan = true;
            }
        }
        else if(myPlayer.stateMachine.isInState(PlayerState.SetStriker.GetInstance()))
        {
            SetStrikerUpdate();
        }
        else if(myPlayer.stateMachine.isInState(PlayerState.SetIntercept.GetInstance()))
        {
            // 絶対イベントカード出さないマン
            myPlayer.isPushedJunbiKanryo = true;
            myPlayer.isPushedNextButton = true; // NextButtonと分けまてみた、はは。
        }
    }

    void SetStrikerUpdate()
    {
        if (myPlayer.isSetStriker())
        {
            // サポートパス復活対策
            if (!myPlayer.isPushedJunbiKanryo)
                myPlayer.JunbiKanryoON();
            return;
        }

        // 手札にセットできるやつがいなかったらパス
        if(!myPlayer.isHaveStrikerCard())
        {
            // セット完了
            myPlayer.JunbiKanryoON();
            return;
        }

        // 出てる点数カードに応じて、出すカードのグレードを設定する
        AI_CARD_GRADE grade;
        int currentPoint = pointManager.GetCurrentPoint();
        if (currentPoint <= 30) grade = AI_CARD_GRADE.LOW;
        else if (currentPoint <= 70) grade = AI_CARD_GRADE.MIDDLE;
        else if (currentPoint <= 100) grade = AI_CARD_GRADE.HIGH;
        else grade = AI_CARD_GRADE.RANDOM;

        // 手札からグレードに対応するカードを取得
        int r = myPlayer.cardObjectManager.GetHandNoRandomStriker(grade);

        // ex構造体作成
        SelectCardIndexInfo exInfo = new SelectCardIndexInfo();
        exInfo.index = r;
        // メッセージ送信
        MessageManager.Dispatch(myPlayer.playerID, MessageType.SetStriker, exInfo);

        Debug.Log("CPU:" + r + "番目のカードをセットしました");

        // セット完了
        myPlayer.JunbiKanryoON();
    }
}
