using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public int myScore { get; private set; }    // 自分のスコア
    float lerpMyScore;                          // 補間用スコア
    public int cpuScore { get; private set; }   // CPUのスコア
    float lerpCPUScore;                         // 補間用スコア

    public int finishScore = 250;   // ゲームが決着するスコア

    public GameObject SetStrikerOKButton;
    public GameObject SetStrikerPassButton;
    GameObject activeSetStrikerButton;
    public Sprite spriteSetStrikerON;           // ストライカーセットON
    public Sprite spriteSetStrikerOFF;          // ストライカーセットOFF
    public Sprite spriteSetStrikerPassON;       // パスON
    public Sprite spriteSetStrikerPassOFF;      // パスOFF

    public CemeteryInfomationUI cemeteryInfoUIManager;

    public Text limitTimeText;
    public Image eventTimeGauge;    // イベント時間ゲージ

    public Number myScoreNumber;
    public Image myScoreGauge;
    public Number cpuScoreNumber;
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

    public void Restart()
    {
        myScore = 0;
        myScoreNumber.SetNumber(myScore);
        cpuScore = 0;
        cpuScoreNumber.SetNumber(cpuScore);
    }

    void Update()
    {
        // 補間処理
        lerpMyScore = Mathf.Lerp(lerpMyScore, (float)myScore, 0.5f);
        lerpCPUScore = Mathf.Lerp(lerpCPUScore, (float)cpuScore, 0.5f);

        // ゲージ処理
        var rate = lerpMyScore / finishScore;
        myScoreGauge.fillAmount = rate;

        // ゲージ処理
        rate = lerpCPUScore / finishScore;
        cpuScoreGauge.fillAmount = rate;
    }

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
            SetStrikerPassButton.SetActive(false);
        }
        else
        {
            activeSetStrikerButton = SetStrikerPassButton;
            // 画像はOFFの状態
            activeSetStrikerButton.GetComponent<Image>().sprite = spriteSetStrikerPassOFF;
            SetStrikerOKButton.SetActive(false);
        }

        activeSetStrikerButton.SetActive(true);
        // ボタン機能無効
        activeSetStrikerButton.GetComponent<Button>().enabled = false;              
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

    public bool AddScore(bool isMyPlayer, int score)
    {
        if(isMyPlayer)
        {
            myScore = Mathf.Min(myScore + score, finishScore);
            myScoreNumber.SetNumber(myScore);
            if (myScore >= finishScore) return true;
        }
        else
        {
            cpuScore = Mathf.Min(cpuScore + score, finishScore);
            cpuScoreNumber.SetNumber(cpuScore);
            if (cpuScore >= finishScore) return true;
        }

        return false;
    }

    public bool SetScore(bool isMyPlayer, int score)
    {
        if (isMyPlayer)
        {
            myScore = score;
            myScoreNumber.SetNumber(myScore);
            if (myScore >= finishScore) return true;
        }
        else
        {
            cpuScore = score;
            cpuScoreNumber.SetNumber(cpuScore);
            if (cpuScore >= finishScore) return true;
        }

        return false;
    }

    public int GetScore(bool isMyPlayer) { return isMyPlayer ? myScore : cpuScore; }


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
}
