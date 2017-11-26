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
        Restart();

        // 仮でデッキデータ作成
        myPlayer.deckData = new PlayerDeckData();
        myPlayer.deckData.strikerCards[0] = 0;
        myPlayer.deckData.strikerCards[1] = 1;
        myPlayer.deckData.strikerCards[2] = 2;
        myPlayer.deckData.strikerCards[3] = 3;
        myPlayer.deckData.strikerCards[4] = 4;
        myPlayer.deckData.strikerCards[5] = 5;
        myPlayer.deckData.strikerCards[6] = 6;
        myPlayer.deckData.strikerCards[7] = 7;
        myPlayer.deckData.strikerCards[8] = 8;
        myPlayer.deckData.strikerCards[9] = 9;
        myPlayer.deckData.jorkerCard = 10;
        myPlayer.deckData.eventCards[0] = (int)IDType.NONE;
        myPlayer.deckData.eventCards[1] = (int)IDType.NONE;
        myPlayer.deckData.eventCards[2] = (int)IDType.NONE;
        myPlayer.deckData.eventCards[3] = (int)IDType.NONE;
        myPlayer.deckManager.SetDeckData(myPlayer.deckData);
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
    }

    void SetStrikerUpdate()
    {
        if (myPlayer.isSetStriker()) return;

        // 手札からランダムでセットする
        int r = Random.Range(0, myPlayer.deckManager.GetNumHand() - 1);

        // ex構造体作成
        SetCardInfo exInfo = new SetCardInfo();
        exInfo.handNo = r;
        // メッセージ送信
        MessageManager.Dispatch(myPlayer.playerID, MessageType.SetCard, exInfo);

        Debug.Log("CPU:" + r + "番目のカードをセットしました");

        // セット完了
        myPlayer.JunbiKanryoON();
    }
}
