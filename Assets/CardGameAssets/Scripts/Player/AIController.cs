using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    Player myPlayer;                  // コントロールするプレイヤーの実体
    bool isMarigan;

    void Start()
    {
        myPlayer = GetComponent<Player>();
        myPlayer.playerName = "CPU";
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
        if (myPlayer.isSetStriker()) return;

        // 手札にセットできるやつがいなかったらパス
        if(!myPlayer.isHaveStrikerCard())
        {
            // セット完了
            myPlayer.JunbiKanryoON();
            return;
        }

        // 手札からランダムでセットする
        int r = /*Random.Range(0, myPlayer.deckManager.GetNumHand() - 1)*/ myPlayer.cardObjectManager.GetHandNoRandomStriker();

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
