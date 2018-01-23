using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberSelectUIManager : MonoBehaviour
{
    const int NumberButtonMax = 11;

    public Button[] numberButtons = new Button[NumberButtonMax];
    AlphaMove[] buttonAlphas = new AlphaMove[NumberButtonMax];
    //AlphaMove[] numberAlphas = new AlphaMove[NumberButtonMax + 1];

    bool endFlag = false;
    int selectedNumber;
    float waitTimer;

    int step;

    void Awake()
    {
        for (int i = 0; i < NumberButtonMax; i++)
        {
            numberButtons[i].gameObject.SetActive(false);
            buttonAlphas[i] = numberButtons[i].GetComponent<AlphaMove>();
            //numberAlphas[i] = numberButtons[i].GetComponent<AlphaMove>();
        }
    }

    void Update()
    {
        // αアニメ更新
        for (int i = 0; i < NumberButtonMax; i++)
        {
            buttonAlphas[i].SelfUpdate();
            //numberAlphas[i].SelfUpdate();
        }

        if (endFlag)
        {
            switch (step)
            {
                case 0:
                    // 選んだ数字だけ、若干後で消えさせるようにする処理
                    if ((waitTimer += Time.deltaTime) > 1.0f)
                    {
                        // エフェクトもあれば良いかもしれない


                        // 選んだ数字を消す
                        buttonAlphas[selectedNumber].StopRoop();
                        //numberAlphas[selectedNumber].StopRoop();
                        step++;
                    }
                    break;
                case 1:
                    // 選んだ数字が完全に消えたら
                    if (buttonAlphas[selectedNumber].IsEndFlag())
                    {
                        // アクション終了
                        gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }

    public void Action(DeckManager deckManager, bool isMyPlayer)
    {
        // アクション開始
        gameObject.SetActive(true);

        step = 0;
        endFlag = false;

        Color notSelectColor = (isMyPlayer) ? new Color(0.5f, 0.5f, 0.5f) : new Color(1.0f, 1.0f, 1.0f);

        // 全部選択不可能
        foreach (Button numberButton in numberButtons)
        {
            numberButton.gameObject.SetActive(true);
            //numberButton.interactable = false;
            numberButton.GetComponent<Image>().color = notSelectColor;
        }

        // αアニメ開始
        for (int i = 0; i < NumberButtonMax; i++)
        {
            buttonAlphas[i].ActionRoop();
            //numberAlphas[i].ActionRoop();
        }

        // 相手プレイヤー視点なら後はいらない
        if (!isMyPlayer) return;

        // 山札にあるカードが選択できる
        foreach (CardData card in deckManager.GetYamahuda())
        {
            // イベントカードはスルー
            if (card.isEventCard()) continue;
            // ボタン有効化
            //numberButtons[card.power].interactable = true;
            float alpha = numberButtons[card.power].GetComponent<Image>().color.a;
            numberButtons[card.power].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
    }

    public void End(int selectNumber)
    {
        // 選んでる番号を保存
        selectedNumber = selectNumber;
        // 終了フラグON
        endFlag = true;
        // 待機時間初期化
        waitTimer = 0;

        for (int i = 0; i < NumberButtonMax; i++)
        {
            // 選んでる番号以外を消す
            if (i != selectNumber)
            {
                numberButtons[i].interactable = false;
                buttonAlphas[i].StopRoop();
                //numberAlphas[i].StopRoop();
            }
        }
    }
}
