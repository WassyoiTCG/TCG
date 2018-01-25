using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerWindow : MonoBehaviour
{
    // プレイヤーの表示
    public GameObject activePlayer, nonePlayer;

    // UI
    public Text TextPlayerName;     // プレイヤー名
    public Image imageIcon;         // プレイヤーアイコン
    public Text TextOK;             // 準備OKの文字

    // 変数
    bool junbiOKPushed;      // 準備OKボタンを押したか

    void Start()
    {
        junbiOKPushed = false;
    }

    public void SetPlayerName(string name) { TextPlayerName.text = name; }
    public void SetIcon(Image icon) { imageIcon = icon; }
    public void SetJunbiOK(bool ok)
    {
        junbiOKPushed = true;
        TextOK.gameObject.SetActive(ok);
    }
    public bool isPushedJunbiOK() { return junbiOKPushed; }
    public void SetPlayerActive(bool isActive)
    {
        activePlayer.SetActive(isActive);
        nonePlayer.SetActive(!isActive);
    }
}
