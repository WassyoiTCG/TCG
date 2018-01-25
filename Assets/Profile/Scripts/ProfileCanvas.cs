using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 最初から非アクティブにしている前提のコードなので、Findはせずアタッチして使う事。
public class ProfileCanvas : MonoBehaviour
{
    // プレイヤー名を描画しているテキスト
    public Text playerNameText;
    // 勝利数を描画
    public Text WinCountText;
    // プレイ時間を描画
    public Text PlayTimeText;

    // 名前変更のオブジェクト
    public NameChange nameChangeObject;
    
    // 表示
    public void Action()
    {
        PlayerData data = PlayerDataManager.GetPlayerData();

        // プレイヤー名を描画テキストに設定
        playerNameText.text = data.playerName;
        // 勝利数を
        WinCountText.text = data.winCount.ToString();
        // プレイ時間を
        uint minutes = data.playTime % 60, hour = data.playTime / 60; ;
        PlayTimeText.text = hour + "時間" + minutes + "分";

        // 自身を表示させる
        gameObject.SetActive(true);
    }

    // 非表示
    public void Stop()
    {
        // 自身を非表示にする
        gameObject.SetActive(false);
    }

    // 名前変更ボタン
    public void ClickNameChangeButton()
    {
        // 名前変更オブジェクト表示
        nameChangeObject.Action(PlayerDataManager.GetPlayerData().playerName);
    }

    // 名前変更完了ボタン
    public void ClickNameChangeEndButton()
    {
        // 名前変更オブジェクト非表示
        nameChangeObject.Stop();
        // 表示されてい名前も変える
        playerNameText.text = PlayerDataManager.GetPlayerData().playerName;
    }

    // 戻るボタン
    public void ClickBackButton()
    {
        // 終了
        Stop();
    }
}
