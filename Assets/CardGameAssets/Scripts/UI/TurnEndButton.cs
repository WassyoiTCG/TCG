using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//+---------------------------------
//  ターンエンドUI
//+---------------------------------
public class TurnEndButton : MonoBehaviour {

    //+---------------------------------
    //  メンバ変数
    //+---------------------------------
    public GameObject SetOKButton;   // モンスター
    public GameObject NextButton;    // イベント
    public GameObject PushedButton;  // 押した後
    public GameObject RippleEf;      // 演出用

    private bool bPushOK = false;    // 押せるか？

    public enum TURN_END_TYPE { SET_OK,NEXT }

    // Use this for initialization
    public void Awake () {
    
        Restart();
    }

    public void Restart()
    {
        bPushOK = false;
        SetOKButton.SetActive(false);
        NextButton.SetActive(false);
    }

    // Update is called once per frame
    public void Update () {
        gameObject.GetComponent<ScreenOutAppeared>().SelfUpdate();

        // 押したフラグ
        if (bPushOK == true)
        {
            RippleUpdate();
        }

    }

    public void RippleUpdate()
    {
        RippleEf.GetComponent<Ripple>().SelfUpdate();
    }

    public void Action(TURN_END_TYPE eType)
    {
        // 押してええよフラグ
        bPushOK = true;

        RippleEf.GetComponent<Ripple>().ActionRoop();

        //  まず移動アニメーション発動 左からシャーッ
        gameObject.GetComponent<ScreenOutAppeared>().SetPos(gameObject.GetComponent<ScreenOutAppeared>().GetAwakPos());
        gameObject.GetComponent<ScreenOutAppeared>().SetNextPos(gameObject.GetComponent<ScreenOutAppeared>().GetAwakeNextPos());
        gameObject.GetComponent<ScreenOutAppeared>().Action();

        //  
        switch (eType)
        {
            case TURN_END_TYPE.SET_OK:

                // 
                SetOKButton.SetActive(true);
                SetOKButton.GetComponent<Button>().enabled = true;
                
                NextButton.SetActive(false);
                //NextButton.GetComponent<Button>().enabled = false;
                break;
            case TURN_END_TYPE.NEXT:
                NextButton.SetActive(true);
                NextButton.GetComponent<Button>().enabled = true;

                SetOKButton.SetActive(false);

                break;
            default:
                Debug.Log(" ベロニカ:ちょっとへろし、そのタイプは存在しないわよ。 - TurnEndButton");
                break;
        }

    }

    // ボタン戻す
    public void BackButton()
    {
        // スイッチ消す
        SetOKButton.GetComponent<Button>().enabled = true;
        NextButton.GetComponent<Button>().enabled = true;
        SetOKButton.SetActive(false);
        NextButton.SetActive(false);

        // 波紋止める
        RippleEf.GetComponent<Ripple>().Stop();

        //  まず移動アニメーション発動 戻る
        gameObject.GetComponent<ScreenOutAppeared>().SetNextPos(gameObject.GetComponent<ScreenOutAppeared>().GetAwakPos());
        gameObject.GetComponent<ScreenOutAppeared>().Action();


    }


    //// 準備完了ボタン押したとき
    //public void OnClickEndButton()
    //{
    //    MessageManager.Dispatch(myPlayerID, MessageType.SetStrikerOK, null);
    //    Debug.Log("モンスターセットボタンキテルグマリアン-ファイナルフラッシュ : TurnEndButton");

    //    // 別のところでもこのメッセージを送るので、非表示処理はここでしない

    //}

    //// パスボタン押したとき
    //public void OnClickNextButton()
    //{
    //    //MessageManager.Dispatch(myPlayerID, MessageType.SetStrikerPass, null);

    //    // 別のところでもこのメッセージを送るので、非表示処理はここでしない

    //}

}
