using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    int myScore;        // 自分のスコア
    float lerpMyScore;  // 補間用スコア
    int cpuScore;       // CPUのスコア
    float lerpCPUScore; // 補間用スコア

    public int finishScore = 280;   // ゲームが決着するスコア
    public float setLimitTime = 60; // ストライカーセットする制限時間
    float timer;                    // 時間用変数

    public Image eventTimeGauge;    // イベント時間ゲージ

    public Text myScoreText;
    public Image myScoreGauge;
    public Text cpuScoreText;
    public Image cpuScoreGauge;

    public GameObject netOrOffline;         // ネットかオフラインかの選択
    public GameObject lobby;                // ロビー
    public GameObject matchingWait;         // マッチング大気中
    public GameObject mainUI;               // メイン画面のUI
    public GameObject firstDrawButtons;     // 最初のドローのボタン
    public GameObject strikerOKButton;      // ストライカーセット完了のボタン



    void Update()
    {
        // 時間減算処理
        if(timer > 0)
        {
            if ((timer -= Time.deltaTime) < 0) timer = 0;

            // UIのゲージ設定
            var timeRate = timer / setLimitTime;
        }

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

    public void TimerSet()
    {
        timer = setLimitTime;
    }

    public void AppearNetOrOffline()
    {
        netOrOffline.SetActive(true);
    }
    public void DisAppearNetOrOffline()
    {
        netOrOffline.SetActive(false);
    }

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
    }

    public void AppearFirstDraw()
    {
        firstDrawButtons.SetActive(true);
    }
    public void DisAppearFirstDraw()
    {
        firstDrawButtons.SetActive(false);
    }

    public void AppearStrikerOK()
    {
        strikerOKButton.SetActive(true);
    }
    public void DisAppearStrikerOK()
    {
        strikerOKButton.SetActive(false);
    }

    public void AddScore(bool isMyPlayer, int score)
    {
        if(isMyPlayer)
        {
            myScore += score;
            myScoreText.text = myScore.ToString();

        }
        else
        {
            cpuScore += score;
            cpuScoreText.text = cpuScore.ToString();

        }
    }
}
