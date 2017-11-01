using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    int myPlayerID = -1;                    // 自分のプレイヤーのID

    public oulNetwork networkManager;
    public UIManager uiManager;

    //public void OnClickNetPlay()
    //{
    //    // ネットフラグOFF
    //    MessageManager.isNetwork = false;
    //    MessageManager.Dispatch(myPlayerID, MessageType.NetPlay, null);

    //    // ネットフラグON
    //    MessageManager.isNetwork = true;
    //    // ボタン非表示
    //    uiManager.DisAppearNetOrOffline();
    //}

    //public void OnClickOfflinePlay()
    //{
    //    // ネットフラグOFF
    //    MessageManager.isNetwork = false;

    //    MessageManager.Dispatch(myPlayerID, MessageType., null);

    //    // ボタン非表示
    //    uiManager.DisAppearNetOrOffline();
    //}

    public void OnClickHostGame()
    {
        networkManager.StartHost();

        // ボタン非表示
        uiManager.DisAppearLobby();
    }

    public void OnClickClientGame()
    {
        networkManager.StartClient();

        // ボタン非表示
        uiManager.DisAppearLobby();
    }

    // マリガンボタンを押したとき
    public void OnClickMarigan()
    {
        if(myPlayerID == -1)
            myPlayerID = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().GetMyPlayerID();

        MessageManager.Dispatch(myPlayerID, MessageType.Marigan, null);

        // ボタン非表示
        uiManager.DisAppearFirstDraw();
    }

    // マリガンしないボタンを押したとき
    public void OnClickNoMarigan()
    {
        if (myPlayerID == -1)
            myPlayerID = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().GetMyPlayerID();

        MessageManager.Dispatch(myPlayerID, MessageType.NoMarigan, null);

        // ボタン非表示
        uiManager.DisAppearFirstDraw();
    }

    public void OnClickSetStrikerOK()
    {
        MessageManager.Dispatch(myPlayerID, MessageType.SetStrikerOK, null);

        // ボタン非表示
        uiManager.DisAppearStrikerOK();
    }
}
