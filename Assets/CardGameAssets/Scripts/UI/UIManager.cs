﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public int myHP { get; private set; }    // 自分のスコア
    float lerpMyHP;                          // 補間用スコア
    public int cpuHP { get; private set; }   // CPUのスコア
    float lerpCPUHP;                         // 補間用スコア

    public int MaxHP = 250;   // ゲームが決着するスコア

    public GameObject SetStrikerOKButton;
    public GameObject SetStrikerPassButton;
    GameObject activeSetStrikerButton;
    public Sprite spriteSetStrikerON;           // ストライカーセットON
    public Sprite spriteSetStrikerOFF;          // ストライカーセットOFF
    public Sprite spriteSetStrikerPassON;       // パスON
    public Sprite spriteSetStrikerPassOFF;      // パスOFF

    public CemeteryInfomationUI cemeteryInfoUIManager;

    float timer;
    float limitTime;
    bool timerFlag = false;
    public Text limitTimeText;
    public Image eventTimeGauge;    // イベント時間ゲージ

    public Number myHPNumber;
    public Image myScoreGauge;
    public Number cpuHPNumber;
    public Image cpuScoreGauge;

    public Number myHandCountNumber, myYamahudaCountNumber, myBochiCountNumber, myTsuihouCountNumber;
    public Number cpuHandCountNumber, cpuYamahudaCountNumber, cpuBochiCountNumber, cpuTsuihouCountNumber;

    //public GameObject netOrOffline;         // ネットかオフラインかの選択
    public GameObject lobby;                // ロビー
    public GameObject matchingWait;         // マッチング大気中
    public GameObject mainUI;               // メイン画面のUI
    public GameObject firstDrawButtons;     // 最初のドローのボタン
    public GameObject endGameUI;            // ゲーム終了時のUI
    public GameObject waitYouUI;            // 相手待ちUI

    public BattleCardInfomation battleCardInfomation;

    public GameObject connectingUI;

    public GameObject selectNumberUI;   // 宝箱とかの

    public void Restart()
    {
        myHP = MaxHP;
        myHPNumber.SetNumber(myHP);
        cpuHP = MaxHP;
        cpuHPNumber.SetNumber(cpuHP);
        cemeteryInfoUIManager.Restart();
        DisAppearMainUI();
    }

    void Update()
    {
        // 補間処理
        lerpMyHP = Mathf.Lerp(lerpMyHP, (float)myHP, 0.5f);
        lerpCPUHP = Mathf.Lerp(lerpCPUHP, (float)cpuHP, 0.5f);

        // ゲージ処理
        var rate = lerpMyHP / MaxHP;
        myScoreGauge.fillAmount = rate;

        // ゲージ処理
        rate = lerpCPUHP / MaxHP;
        cpuScoreGauge.fillAmount = rate;

        // 制限時間処理
        if (!timerFlag) return;
        if (timer > 0)
        {
            if ((timer -= Time.deltaTime) < 0)
            {
                timer = 0;
            }
        }
        // 時間UI更新
        UpdateLimitTimeText();
    }

    void UpdateLimitTimeText()
    {
        // UIの時間表示
        var minutes = (int)timer / 60;
        var second = (int)timer % 60;
        limitTimeText.text = minutes + ":" + second.ToString("00");

        // UIのゲージ設定
        var timeRate = timer / limitTime;
        eventTimeGauge.fillAmount = timeRate;
    }

    public void SetTimer(float limit)
    {
        timerFlag = true;
        timer = limit;
        limitTime = limit;
        UpdateLimitTimeText();
    }
    public void StopTimer() { timerFlag = false; }

    public bool isTimerEnd() { return (timer == 0); }

    //public void AppearNetOrOffline()
    //{
    //    netOrOffline.SetActive(true);
    //}
    //public void DisAppearNetOrOffline()
    //{
    //    netOrOffline.SetActive(false);
    //}

    public void AppearLobby()
    {
        lobby.SetActive(true);
    }
    public void DisAppearLobby()
    {
        lobby.SetActive(false);
    }

    public void AppearMatchingWait()
    {
        matchingWait.SetActive(true);
    }
    public void DisAppearMatchingWait()
    {
        matchingWait.SetActive(false);
    }

    public void AppearMainUI()
    {
        mainUI.SetActive(true);
        this.enabled = true;
    }

    public void DisAppearMainUI()
    {
        mainUI.SetActive(false);
        this.enabled = false;
    }

    public void AppearFirstDraw()
    {
        firstDrawButtons.SetActive(true);
    }
    public void DisAppearFirstDraw()
    {
        firstDrawButtons.SetActive(false);
    }

    public void AppearBattleCardInfomation(CardData cardData)
    {
        // インフォメーション表示
        battleCardInfomation.Action(cardData);
    }

    public void DisAppearBattleCardInfomation()
    {
        // インフォメーション非表示
        battleCardInfomation.Stop();
    }


    public void AppearStrikerOK(bool isHaveStriker)
    {
        if (isHaveStriker)
        {
            activeSetStrikerButton = SetStrikerOKButton;
            // 画像はOFFの状態
            activeSetStrikerButton.GetComponent<Image>().sprite = spriteSetStrikerOFF;
            // パスボタン消す
            SetStrikerPassButton.SetActive(false);
            // ボタン機能無効
            activeSetStrikerButton.GetComponent<Button>().enabled = false;
        }
        else
        {
            activeSetStrikerButton = SetStrikerPassButton;
            // 画像はOFFの状態
            activeSetStrikerButton.GetComponent<Image>().sprite = spriteSetStrikerPassON;
            // セットOKボタン消す
            SetStrikerOKButton.SetActive(false);
            // ボタン機能有効(パスなので最初から押して)
            activeSetStrikerButton.GetComponent<Button>().enabled = true;
        }

        activeSetStrikerButton.SetActive(true);
           
    }
    public void EnableSetStrikerButton()
    {
        // ゴリA列車
        if (SetStrikerOKButton.gameObject.activeInHierarchy)
        {
            // 画像差し替え
            activeSetStrikerButton.GetComponent<Image>().sprite = spriteSetStrikerON;
        }
        else
        {
            // 画像差し替え
            activeSetStrikerButton.GetComponent<Image>().sprite = spriteSetStrikerPassON;
        }
        // ボタン機能有効
        activeSetStrikerButton.GetComponent<Button>().enabled = true;
    }
    public void DisableSetStrikerButton()
    {
        // ゴリA列車
        if (SetStrikerOKButton.gameObject.activeInHierarchy)
        {
            // 画像差し替え
            activeSetStrikerButton.GetComponent<Image>().sprite = spriteSetStrikerOFF;
        }
        else
        {
            // 画像差し替え
            activeSetStrikerButton.GetComponent<Image>().sprite = spriteSetStrikerPassOFF;
        }
        // ボタン機能有効
        activeSetStrikerButton.GetComponent<Button>().enabled = false;
    }


    // 手札山札墓地追放UIの更新
    public void UpdateDeckUI(DeckManager deckManager, bool isMyPlayer)
    {
        var numHand = deckManager.GetNumHand();
        var numYamahuda = deckManager.GetNumYamahuda();
        var numBochi = deckManager.GetNumBochi();
        var numTsuihou = deckManager.GetNumTsuihou();

        if (isMyPlayer)
        {
            // UIテキストの変更
            myHandCountNumber.SetNumber(numHand);
            myYamahudaCountNumber.SetNumber(numYamahuda);
            myBochiCountNumber.SetNumber(numBochi);
            myTsuihouCountNumber.SetNumber(numTsuihou);
            //myHandCountText.text = "x" + numHand.ToString();
            //myYamahudaCountText.text = "x" + numYamahuda.ToString();
            //myBochiCountText.text = "x" + numBochi.ToString();
            //myTsuihouCountText.text = "x" + numTsuihou.ToString();
        }
        else
        {
            // UIテキストの変更
            //cpuHandCountText.text = "x" + numHand.ToString();
            //cpuYamahudaCountText.text = "x" + numYamahuda.ToString();
            //cpuBochiCountText.text = "x" + numBochi.ToString();
            //cpuTsuihouCountText.text = "x" + numTsuihou.ToString();
            cpuHandCountNumber.SetNumber(numHand);
            cpuYamahudaCountNumber.SetNumber(numYamahuda);
            cpuBochiCountNumber.SetNumber(numBochi);
            cpuTsuihouCountNumber.SetNumber(numTsuihou);
        }
    }

    public bool Damage(bool isMyPlayer, int score)
    {
        if(isMyPlayer)
        {
            myHP = Mathf.Max(myHP - score, 0);
            myHPNumber.SetNumber(myHP);
            if (myHP == 0) return true;
        }
        else
        {
            cpuHP = Mathf.Max(cpuHP - score, 0);
            cpuHPNumber.SetNumber(cpuHP);
            if (cpuHP == 0) return true;
        }

        return false;
    }

    public bool SetHP(bool isMyPlayer, int score)
    {
        if (isMyPlayer)
        {
            myHP = Mathf.Min(Mathf.Max(score, 0), MaxHP);
            myHPNumber.SetNumber(myHP);
            if (myHP == 0) return true;
        }
        else
        {
            cpuHP = Mathf.Min(Mathf.Max(score, 0), cpuHP);
            cpuHPNumber.SetNumber(cpuHP);
            if (cpuHP == 0) return true;
        }

        return false;
    }

    public int GetScore(bool isMyPlayer) { return isMyPlayer ? myHP : cpuHP; }


    public void AppearEndGameUI()
    {
        endGameUI.SetActive(true);
    }

    public void DisAppearEndGameUI()
    {
        endGameUI.SetActive(false);
    }

    public void AppearWaitYouUI()
    {
        waitYouUI.SetActive(true);
    }

    public void DisAppearWaitYouUI()
    {
        waitYouUI.SetActive(false);
    }

    public void AppearConnectingUI()
    {
        connectingUI.SetActive(true);
    }

    public void DisAppearConnectingUI()
    {
        connectingUI.SetActive(false);
    }

    public void AppearSelectNumberUI()
    {
        selectNumberUI.SetActive(true);
    }

    public void DisAppearSelectNumberUI()
    {
        selectNumberUI.SetActive(false);
    }
}
