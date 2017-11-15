using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

//+---------------------------------------------------
//  メニューステートマシン
//+---------------------------------------------------

namespace SceneDeckState
{
    //+-------------------------------------------
    //  グローバルステート
    //+-------------------------------------------
    public class Global : BaseEntityState<SceneDeck>
    {
        static Global m_pInstance;
        public static Global GetInstance() { if (m_pInstance == null) { m_pInstance = new Global(); } return m_pInstance; }

        public override void Enter(SceneDeck e)
        {

        }

        public override void Execute(SceneDeck e)
        {


        }

        public override void Exit(SceneDeck e)
        {

        }

        public override bool OnMessage(SceneDeck e, MessageInfo message)
        {



            return false;
        }

    }

    //+-------------------------------------------
    //  最初
    //+-------------------------------------------
    public class Intro : BaseEntityState<SceneDeck>
    {
        static Intro m_pInstance;
        public static Intro GetInstance() { if (m_pInstance == null) { m_pInstance = new Intro(); } return m_pInstance; }

        public override void Enter(SceneDeck e)
        {

            // メインへ
            e.m_pStateMachine.ChangeState(SceneDeckState.Main.GetInstance());

            return;
        }

        public override void Execute(SceneDeck e)
        {

             



        }

        public override void Exit(SceneDeck e)
        {

        }

        public override bool OnMessage(SceneDeck e, MessageInfo message)
        {

            return false; // 何も引っかからなかった
        }

    }


    //+-------------------------------------------
    //  メイン
    //+-------------------------------------------
    public class Main : BaseEntityState<SceneDeck>
    {
        static Main m_pInstance;
        public static Main GetInstance() { if (m_pInstance == null) { m_pInstance = new Main(); } return m_pInstance; }

        public override void Enter(SceneDeck e)
        {
        

            return;
        }

        public override void Execute(SceneDeck e)
        {


        // タッチしたときカードだったら
            GameObject TouchObj = oulInput.GetTouchuGUIObject();
            // Nullではない(何かをタッチしていたら)
            if (TouchObj != null)
            {
                if (TouchObj.tag == "Card")
                {
                    //
                    if (oulInput.GetTouchState() == oulInput.TouchState.Began)
                    {

                        Debug.Log("えうれしあ");

                        // ★取得カード（当たり判定の↑の階層。つまりuGUICardや）
                        // ★保存
                        e.TouchCardObj = TouchObj.transform.parent.gameObject;

                        // 掴めないカードなら
                        if (e.TouchCardObj.GetComponent<uGUICard>().GetNotGrasp() == true)
                        {
                            Debug.Log("掴めないカードを掴んだので無効。上のデッキセット中にも当たり判定あるのは気にしなくてよかった。");

                        }
                        else {

                            e.SetCardTapFlag(true);// カードタップしたやで

                            // タップポジション保存
                            e.m_vTapPos = Input.mousePosition;
                            e.m_vTapPos -= new Vector3(Screen.width / 2, Screen.height / 2, 0);
                           
                            // 変更後
                            //DragObj = Instantiate(TouchObj.transform.parent.gameObject, Canvas.transform) as GameObject;

                            /*

                            e.m_uGUICard.SetActive(true);

                            // インスタンスを繰り返すのはダメだと思ったので一つドラッグ用に作って
                            // それを繰り返し使用するようにする。
                            e.m_uGUICard.GetComponent<uGUICard>().SetCardData(e.TouchCardObj.transform.GetComponent<uGUICard>().cardData);

                            */

                            //uGUICard a = DragObj.GetComponent<uGUICard>();
                            // ナンバーなぜか更新されない
                            // DragObj.GetComponent<uGUICard>().SetCardData(PickCard.transform.GetComponent<uGUICard>().cardData);
                            //CardData carddata = new CardData();
                            //carddata.cardName = "aaaa";
                            //carddata.cardType = CardType.Fighter;
                            //
                            //
                            //DragObj.GetComponent<uGUICard>().SetCardData(carddata);

                            // // 掴んでるカードのデータ
                            //int id = a.cardData.id;
                            //CardType type = a.cardData.cardType;

                            // m_aCollectCard[i].GetComponent<uGUICard>().SetCardData(all[i]);

                            //i
                        }
                    }
                }

            }// TouchObj

            //+---------------------------------------------------------------------
            // 離したら描画しない&タップフラグOFF
            if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
            {
                Debug.Log("えうれしあ2");
                e.m_uGUICard.SetActive(false);
                e.SetCardTapFlag(false);// △カードタップフラグOFF
            }

            //+---------------------------------------------------------------------------
            if (e.IsCardTapFlag() == true &&
                e.TouchCardObj != null)
            {

                // 今のポジション保存
                e.m_vCurrentPos = Input.mousePosition;
                e.m_vCurrentPos -= new Vector3(Screen.width / 2, Screen.height / 2, 0);
                float fDis = Vector3.Distance(e.m_vTapPos, e.m_vCurrentPos);
                float fRange = 35;// ↓の基準の距離
                if (fDis >= fRange)
                {
                    Debug.Log("タップした場所からそのまま一定距離移動したので、カードを持ちたいと認識");

                    // カードタップフラグOFF
                    // インスタンスを繰り返すのはダメだと思ったので一つドラッグ用に作って
                    // それを繰り返し使用するようにする。
                    e.m_uGUICard.SetActive(true);
                    e.m_uGUICard.GetComponent<uGUICard>().SetCardData(e.TouchCardObj.transform.GetComponent<uGUICard>().cardData);

                    // 座標マウスに合わす
                    Vector3 pos = Input.mousePosition;
                    pos -= new Vector3(Screen.width / 2, Screen.height / 2, 0);
                    e.m_uGUICard.transform.localPosition = pos;


                    e.SetCardTapFlag(false);// △カードタップフラグOFF
                                            
                    // デッキのカードかコレクションのカードかで分岐
                    if (e.TouchCardObj.transform.GetComponent<uGUICard>().GetMyDeck() == false)
                    {
                        // CollectToDeckへ
                        e.m_pStateMachine.ChangeState(SceneDeckState.CollectToDeck.GetInstance());
                    }
                    else
                    {
                        // DeckToCollectへ
                        e.m_pStateMachine.ChangeState(SceneDeckState.DeckToCollect.GetInstance());
                    }

                    return; //★★★　ステートマシン変えて終り！！
                }


                // あたらしくかーどおしたふらぐつくってここでTrueだったらってやる
                // 押しっぱなし
                float fTime = 1.0f;// ↓の基準の時間
                if (oulInput.GetHoldTime() >= fTime)
                {
                    GameObject CollPanel= oulInput.CollisionuGUI();

                    if (CollPanel != null)
                    {
                        if (CollPanel.tag == "Card")
                        {
                            GameObject CollCard = CollPanel.transform.parent.gameObject;
                            int ID = CollCard.transform.GetComponent<uGUICard>().cardData.id;// ID抜き取り

                            // タップした時のカードと同じIDならば
                            if (ID == e.TouchCardObj.transform.GetComponent<uGUICard>().cardData.id) 
                            {
                                // つまりじっと同じ場所に手を添えていた
                                // 掴もう。。

                                Debug.Log("押しっぱえうれしあ");
                                // インスタンスを繰り返すのはダメだと思ったので一つドラッグ用に作って
                                // それを繰り返し使用するようにする。
                                e.m_uGUICard.SetActive(true);
                                e.m_uGUICard.GetComponent<uGUICard>().SetCardData(e.TouchCardObj.transform.GetComponent<uGUICard>().cardData);
                              
                                 // 座標マウスに合わす
                                Vector3 pos = Input.mousePosition;
                                pos -= new Vector3(Screen.width / 2, Screen.height / 2, 0);
                                e.m_uGUICard.transform.localPosition = pos;

                                e.SetCardTapFlag(false);// △カードタップフラグOFF

                                // デッキのカードかコレクションのカードかで分岐
                                if (e.TouchCardObj.transform.GetComponent<uGUICard>().GetMyDeck() == false)
                                {
                                    // CollectToDeckへ
                                    e.m_pStateMachine.ChangeState(SceneDeckState.CollectToDeck.GetInstance());
                                }
                                else 
                                {
                                    // DeckToCollectへ
                                    e.m_pStateMachine.ChangeState(SceneDeckState.DeckToCollect.GetInstance());
                                }

                                return; //★★★　ステートマシン変えて終り！！
                            }

                        }

                    }// CollPanel != null

                    // ここに来たということは何もおこらなかったやで
                      e.SetCardTapFlag(false);// △カードタップフラグOFF
                    Debug.Log("押しっぱやけど掴みたくない意図を確認。掴まない。");

                }

            }// e.IsCardTapFlag()



 


        }

        public override void Exit(SceneDeck e)
        {

        }

        public override bool OnMessage(SceneDeck e, MessageInfo message)
        {


            // メッセージタイプが一致している箇所に
            switch (message.messageType)
            {
                case MessageType.ClickLineButton:

                    // byte[]→構造体
                    ChangeLine info = new ChangeLine();
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(info));
                    Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(info));
                    info = (ChangeLine)Marshal.PtrToStructure(ptr, info.GetType());
                    Marshal.FreeHGlobal(ptr);

                    //+-------------------------------------------------------------
                   

                    ////  セレクトタイプによってステート変更
                    //switch ((CHANGE_LINE_TYPE)info.iNextLine)
                    //{
                    //    case CHANGE_LINE_TYPE.BACK:
                        
                    //        SelectData.iDeckCollectLineNo -= 1;
                    //        Debug.Log("SceneCollectState: LINE移動↑(マイナス)");

                    //        break;
                    //    case CHANGE_LINE_TYPE.NEXT:
                    //        SelectData.iDeckCollectLineNo += 1;
                    //        Debug.Log("SceneCollectState: LINE移動↓(プラス)");

                    //        break;
                    //    case CHANGE_LINE_TYPE.END:
                    //        Debug.LogWarning("SceneCollectState: それ以上のタイプはない。");
                    //        break;
                    //    default:
                    //        break;
                    //}

                    // コレクションカードのライン変更！
                   e.ChangeLine(info.iNextLine);

                    return true; // trueを返して終り

                default:

                    break;
            }

            return false; // 何も引っかからなかった
        }

    }


    //+-------------------------------------------
    //  コレクションからデッキへ
    //+-------------------------------------------
    public class CollectToDeck : BaseEntityState<SceneDeck>
    {
        static CollectToDeck m_pInstance;
        public static CollectToDeck GetInstance() { if (m_pInstance == null) { m_pInstance = new CollectToDeck(); } return m_pInstance; }

        public override void Enter(SceneDeck e)
        {

            Debug.Log("CollectToDeckに来た列車");
            return;
        }

        public override void Execute(SceneDeck e)
        {

            //+---------------------------------------------------------------------------
            // 見えてたらマウスに追従
            if (e.m_uGUICard.activeSelf == true)
            {

                Vector3 pos = Input.mousePosition;
                pos -= new Vector3(Screen.width / 2, Screen.height / 2, 0);
                //pos -= new Vector3(-200, 0, 0);

                e.m_uGUICard.transform.localPosition = pos;// oulInput.GetPosition(0, false);
            }

            // 離したら描画しない
            if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
            {
                int DeckSideX = 800;
                if (Input.mousePosition.x >= DeckSideX)
                {

                    Debug.Log("念願の構築。おめでとうーCollectToDeck");
                    // ★★デッキ変更
                    e.DeckSet();
                }

                Debug.Log("離した列車ーCollectToDeck");
                e.m_uGUICard.SetActive(false);

                // メインへ戻る
                e.m_pStateMachine.ChangeState(SceneDeckState.Main.GetInstance());
                return;
            }

        }

        public override void Exit(SceneDeck e)
        {

        }

        public override bool OnMessage(SceneDeck e, MessageInfo message)
        {

            return false; // 何も引っかからなかった
        }

    }


    //+-------------------------------------------
    //  デッキからコレクションへ
    //+-------------------------------------------
    public class DeckToCollect : BaseEntityState<SceneDeck>
    {
        static DeckToCollect m_pInstance;
        public static DeckToCollect GetInstance() { if (m_pInstance == null) { m_pInstance = new DeckToCollect(); } return m_pInstance; }

        public override void Enter(SceneDeck e)
        {

            Debug.Log("DeckToCollectに来た列車");
            return;
        }

        public override void Execute(SceneDeck e)
        {
            //+---------------------------------------------------------------------------
            // 見えてたらマウスに追従
            if (e.m_uGUICard.activeSelf == true)
            {

                Vector3 pos = Input.mousePosition;
                pos -= new Vector3(Screen.width / 2, Screen.height / 2, 0);
                //pos -= new Vector3(-200, 0, 0);

                e.m_uGUICard.transform.localPosition = pos;// oulInput.GetPosition(0, false);
            }

            // 離したら描画しない
            if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
            {
                int CollectSideX = 800;
                if (Input.mousePosition.x <= CollectSideX)
                {

                    Debug.Log("デッキからカードを戻す。やったぜーCollectToDeck");
                    // ★★デッキ変更
                    e.DeckOut();
                }
                

                Debug.Log("離した列車ーDeckToCollect");
                e.m_uGUICard.SetActive(false);

                // メインへ戻る
                e.m_pStateMachine.ChangeState(SceneDeckState.Main.GetInstance());
                return;
            }


        }

        public override void Exit(SceneDeck e)
        {

        }

        public override bool OnMessage(SceneDeck e, MessageInfo message)
        {

            return false; // 何も引っかからなかった
        }

    }
}
