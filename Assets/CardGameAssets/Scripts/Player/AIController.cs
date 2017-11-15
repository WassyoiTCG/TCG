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
    }

    public void Restart()
    {
        isMarigan = false;
    }

    // Update is called once per frame
    void Update ()
    {
        switch (myPlayer.state)
        {
            case PlayerState.FirstDraw:
                if(!isMarigan)
                {
                    // 絶対マリガンしないマン
                    MessageManager.Dispatch(myPlayer.playerID, MessageType.NoMarigan, null);
                    isMarigan = true;
                }
                break;
            case PlayerState.SetStriker:
                SetStrikerUpdate();
                break;
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
