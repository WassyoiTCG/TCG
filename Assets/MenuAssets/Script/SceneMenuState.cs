﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//+---------------------------------------------------
//  メニューステートマシン
//+---------------------------------------------------

//public class SceneMenuState : MonoBehaviour {

//	// Use this for initialization
//	void Start () {

//	}

//	// Update is called once per frame
//	void Update () {

//	}

//}

namespace SceneMenuState
{
    //+-------------------------------------------
    //  
    //+-------------------------------------------
    //public class A : BaseEntityState<SceneMenu>
    //{
    //    static A m_pInstance;
    //   public  static A GetInstance() { if (m_pInstance == null) { m_pInstance = new A(); } return m_pInstance; }

    //    public override void Enter(SceneMenu e)
    //    {

    //    }

    //    public override void Execute(SceneMenu e)
    //    {

    //    }

    //    public override void Exit(SceneMenu e)
    //    {

    //    }

    //    public override bool OnMessage(SceneMenu e, MessageInfo message)
    //    {

    //        return false;
    //    }

    //}

    //+-------------------------------------------
    //  グローバルステート
    //+-------------------------------------------
    public class Global : BaseEntityState<SceneMenu>
    {
        static Global m_pInstance;
        public static Global GetInstance() { if (m_pInstance == null) { m_pInstance = new Global(); } return m_pInstance; }

        public override void Enter(SceneMenu e)
        {

        }

        public override void Execute(SceneMenu e)
        {
            e.MenuPlate.GetComponent<ScreenOutAppeared>().SelfUpdate();
            // メニューボタンたち
            for (int i = 0; i < (int)MENU_TYPE.END; i++)
            {
                e.MenuButton[i].GetComponent<ScreenOutAppeared>().SelfUpdate();
            }

            for (int i = 0; i < (int)MENU_SPHERE_TYPE.END; i++)
            {
                e.MenuSphere[i].GetComponent<BoyonAppeared>().SelfUpdate();
            }

           // カーソル
           e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().SelfUpdate();

           // その次にインフォメーション
           e.InfoPlate.GetComponent<ScreenOutAppeared>().SelfUpdate();

           // インフォメーションの文字の更新
           e.Info[(int)SelectData.iMenuType].GetComponent<ScreenOutAppeared>().SelfUpdate();

           // コイン更新
           e.CoinPlate.GetComponent<ScreenOutAppeared>().SelfUpdate();

        }

        public override void Exit(SceneMenu e)
        {

        }

        public override bool OnMessage(SceneMenu e, MessageInfo message)
        {
           


            return false;
        }

    }


    //+-------------------------------------------
    //  最初
    //+-------------------------------------------
    public class Intro : BaseEntityState<SceneMenu>
    {
        static Intro m_pInstance;
        public static Intro GetInstance() { if (m_pInstance == null) { m_pInstance = new Intro(); } return m_pInstance; }

        public override void Enter(SceneMenu e)
        {
            //  ↓のプレートが出る
            //e.MenuPlate.GetComponent<ScreenOutAppeared>().SetDelayFrame(0);
            //e.MenuPlate.GetComponent<ScreenOutAppeared>().Action();

            //// 次にアイコン達が出る
            //int iDelayFrame = 6;
            //int iAdd= 12;
            //// メニューボタンたち
            //for (int i = 0; i < (int)MENU_TYPE.END; i++)
            //{
            //
            //    e.MenuButton[i].GetComponent<ScreenOutAppeared>().SetDelayFrame((i * iDelayFrame) + iAdd);
            //    e.MenuButton[i].GetComponent<ScreenOutAppeared>().Action();
            //
            //}

            
            //  選択されているポジションの場所へ配置
            //e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().SetPosReCalcNextPos(e.MenuButton[(int)SelectData.iMenuType].transform.localPosition);
            //e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().SetDelayFrame(((int)SelectData.iMenuType * iDelayFrame) + iAdd);
            //e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().Action();


            // その次にインフォメーション
            //e.InfoPlate.GetComponent<ScreenOutAppeared>().SetDelayFrame(((int)MENU_TYPE.END * iDelayFrame) + iAdd);
            //e.InfoPlate.GetComponent<ScreenOutAppeared>().Action();

            // 説明の文字もアクション
            //e.Info[(int)SelectData.iMenuType].GetComponent<ScreenOutAppeared>().SetDelayFrame(((int)MENU_TYPE.END * iDelayFrame) + iAdd);
            //e.Info[(int)SelectData.iMenuType].GetComponent<ScreenOutAppeared>().Action();

            // コイン
            //e.CoinPlate.GetComponent<ScreenOutAppeared>().SetDelayFrame(((int)MENU_TYPE.END * iDelayFrame) + iAdd);
            //e.CoinPlate.GetComponent<ScreenOutAppeared>().Action();

            // メニュースフィアたちの描画を最初切りたいので
            for (int i = 0; i < (int)MENU_SPHERE_TYPE.END; i++)
            {
                e.MenuSphere[i].GetComponent<BoyonAppeared>().Stop();
            }

            //+-------------------------------------------------------------
            //  セレクトタイプによってステート変更
            // ステートマシンの初期化や切り替えは最後に行う

            // バトルセレクト
            e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());

            //switch ((MENU_TYPE)SelectData.iMenuType)
            //{
            //    case MENU_TYPE.TOTORIAL:
            //        e.m_pStateMachine.ChangeState(SceneMenuState.TutorialSelect.GetInstance());

            //        break;
            //    case MENU_TYPE.BATTLE:
            //        e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());

            //        break;
            //    case MENU_TYPE.DECK:
            //        e.m_pStateMachine.ChangeState(SceneMenuState.DeckSelect.GetInstance());

            //        break;
            //    case MENU_TYPE.COLLECTION:
            //        e.m_pStateMachine.ChangeState(SceneMenuState.CollectSelect.GetInstance());

            //        break;
            //    case MENU_TYPE.OPTION:
            //        e.m_pStateMachine.ChangeState(SceneMenuState.OptionSelect.GetInstance());

            //        break;
            //    case MENU_TYPE.END:
            //        Debug.LogWarning("SceneMenuState: それ以上のタイプはない。");
            //        break;
            //    default:
            //        break;
            //}

            return;


        }

        public override void Execute(SceneMenu e)
        {

        }

        public override void Exit(SceneMenu e)
        {

        }

        public override bool OnMessage(SceneMenu e, MessageInfo message)
        {

            return false; // 何も引っかからなかった
        }

    }

    //+-------------------------------------------
    //  チュートリアル選択中
    //+-------------------------------------------
    public class TutorialSelect : BaseEntityState<SceneMenu>
    {
        static TutorialSelect m_pInstance;
        public static TutorialSelect GetInstance() { if (m_pInstance == null) { m_pInstance = new TutorialSelect(); } return m_pInstance; }

        public override void Enter(SceneMenu e)
        {     
            // メニュー選択カーソル移動
            Vector3 vNextPos = e.MenuButton[(int)SelectData.iMenuType].GetComponent<ScreenOutAppeared>().GetNextPos();
            e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().SetNextPos(vNextPos);
            e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().Action();

            // ステート毎のインフォメーションの文字のアニメ開始 (最初座標から再アクション)
            e.Info[(int)MENU_TYPE.TOTORIAL].transform.localPosition = e.Info[(int)MENU_TYPE.TOTORIAL].GetComponent<ScreenOutAppeared>().GetAwakPos();
            e.Info[(int)MENU_TYPE.TOTORIAL].GetComponent<ScreenOutAppeared>().Action();


            // 球体
            e.MenuSphere[(int)MENU_SPHERE_TYPE.TOTORIAL].GetComponent<BoyonAppeared>().Action();


        }

        public override void Execute(SceneMenu e)
        {

        }

        public override void Exit(SceneMenu e)
        {
            // 演出・描画を止める
            e.Info[(int)MENU_TYPE.TOTORIAL].GetComponent<ScreenOutAppeared>().Stop();


            // 球体
            e.MenuSphere[(int)MENU_SPHERE_TYPE.TOTORIAL].GetComponent<BoyonAppeared>().Stop();

        }

        public override bool OnMessage(SceneMenu e, MessageInfo message)
        {

            // メッセージタイプが一致している箇所に
            switch (message.messageType)
            {
                case MessageType.ClickMenuButton:

                    // byte[]→構造体
                    SelectMenuNo info = new SelectMenuNo();
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(info));
                    Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(info));
                    info = (SelectMenuNo)Marshal.PtrToStructure(ptr, info.GetType());
                    Marshal.FreeHGlobal(ptr);

                    //+-------------------------------------------------------------
                    // ★選択したアイコンのタイプに変更
                    SelectData.iMenuType = info.selectNo;

                    //  セレクトタイプによってステート変更
                    switch ((MENU_TYPE)SelectData.iMenuType)
                    {
                        case MENU_TYPE.TOTORIAL:
                            //e.m_pStateMachine.ChangeState(SceneMenuState.TutorialSelect.GetInstance());

                            break;
                        case MENU_TYPE.BATTLE:
                            e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());

                            break;
                        case MENU_TYPE.DECK:
                            e.m_pStateMachine.ChangeState(SceneMenuState.DeckSelect.GetInstance());

                            break;
                        case MENU_TYPE.COLLECTION:
                            e.m_pStateMachine.ChangeState(SceneMenuState.CollectSelect.GetInstance());

                            break;
                        case MENU_TYPE.OPTION:
                            e.m_pStateMachine.ChangeState(SceneMenuState.OptionSelect.GetInstance());

                            break;
                        case MENU_TYPE.END:
                            Debug.LogWarning("SceneMenuState: それ以上のタイプはない。");
                            break;
                        default:
                            break;
                    }
                    return true; // trueを返して終り

                default:

                    break;
            }

            return false;　// 何も引っかからなかった
        }

    }

    //+-------------------------------------------
    //  対戦選択中
    //+-------------------------------------------
    public class BattleSelect : BaseEntityState<SceneMenu>
    {
        static BattleSelect m_pInstance;
        public static BattleSelect GetInstance() { if (m_pInstance == null) { m_pInstance = new BattleSelect(); } return m_pInstance; }

        public override void Enter(SceneMenu e)
        {
            // メニュー選択カーソル移動
            //Vector3 vNextPos = e.MenuButton[(int)SelectData.iMenuType].GetComponent<ScreenOutAppeared>().GetNextPos();
            //e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().SetNextPos(vNextPos);
            //e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().Action();
            //
            //// ステート毎のインフォメーションの文字のアニメ開始 (最初座標から再アクション)
            //e.Info[(int)MENU_TYPE.BATTLE].transform.localPosition = e.Info[(int)MENU_TYPE.BATTLE].GetComponent<ScreenOutAppeared>().GetAwakPos();
            //e.Info[(int)MENU_TYPE.BATTLE].GetComponent<ScreenOutAppeared>().Action();


            e.BlackPanel.SetActive(false);

            // 球体
            e.MenuSphere[(int)MENU_SPHERE_TYPE.BATTLE].GetComponent<BoyonAppeared>().Action();
            e.MenuSphere[(int)MENU_SPHERE_TYPE.NET_BATTLE].GetComponent<BoyonAppeared>().Action();

            e.MenuSphere[(int)MENU_SPHERE_TYPE.DECK_CREATE].GetComponent<BoyonAppeared>().Action();
        }

        public override void Execute(SceneMenu e)
        {

        }

        public override void Exit(SceneMenu e)
        {

            // 演出・描画を止める
           // e.Info[(int)MENU_TYPE.BATTLE].GetComponent<ScreenOutAppeared>().Stop();

            // 球体
            e.MenuSphere[(int)MENU_SPHERE_TYPE.BATTLE].GetComponent<BoyonAppeared>().Stop();
            e.MenuSphere[(int)MENU_SPHERE_TYPE.NET_BATTLE].GetComponent<BoyonAppeared>().Stop();

            e.MenuSphere[(int)MENU_SPHERE_TYPE.DECK_CREATE].GetComponent<BoyonAppeared>().Stop();
        }

        public override bool OnMessage(SceneMenu e, MessageInfo message)
        {

            // メッセージタイプが一致している箇所に
            switch (message.messageType)
            {
                case MessageType.ClickMenuButton:

                    // byte[]→構造体
                    SelectMenuNo info = new SelectMenuNo();
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(info));
                    Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(info));
                    info = (SelectMenuNo)Marshal.PtrToStructure(ptr, info.GetType());
                    Marshal.FreeHGlobal(ptr);
                    //+-------------------------------------------------------------
                    // ★選択したアイコンのタイプに変更
                    SelectData.iMenuType = info.selectNo;

                    //  セレクトタイプによってステート変更
                    switch ((MENU_TYPE)SelectData.iMenuType)
                    {
                        case MENU_TYPE.TOTORIAL:
                            e.m_pStateMachine.ChangeState(SceneMenuState.TutorialSelect.GetInstance());

                            break;
                        case MENU_TYPE.BATTLE:
                           // e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());

                            break;
                        case MENU_TYPE.DECK:
                            e.m_pStateMachine.ChangeState(SceneMenuState.DeckSelect.GetInstance());

                            break;
                        case MENU_TYPE.COLLECTION:
                            e.m_pStateMachine.ChangeState(SceneMenuState.CollectSelect.GetInstance());

                            break;
                        case MENU_TYPE.OPTION:
                            e.m_pStateMachine.ChangeState(SceneMenuState.OptionSelect.GetInstance());

                            break;
                        case MENU_TYPE.END:
                            Debug.LogWarning("SceneMenuState: それ以上のタイプはない。");
                            break;
                        default:
                            break;
                    }
                    return true; // trueを返して終り

                default:

                    break;
            }

            return false;　// 何も引っかからなかった
        }

    }

    //+-------------------------------------------
    //  デッキ選択中
    //+-------------------------------------------
    public class DeckSelect : BaseEntityState<SceneMenu>
    {
        static DeckSelect m_pInstance;
        public static DeckSelect GetInstance() { if (m_pInstance == null) { m_pInstance = new DeckSelect(); } return m_pInstance; }

        public override void Enter(SceneMenu e)
        {
            // メニュー選択カーソル移動
            Vector3 vNextPos = e.MenuButton[(int)SelectData.iMenuType].GetComponent<ScreenOutAppeared>().GetNextPos();
            e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().SetNextPos(vNextPos);
            e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().Action();

            // ステート毎のインフォメーションの文字のアニメ開始 (最初座標から再アクション)
            e.Info[(int)MENU_TYPE.DECK].transform.localPosition = e.Info[(int)MENU_TYPE.DECK].GetComponent<ScreenOutAppeared>().GetAwakPos();
            e.Info[(int)MENU_TYPE.DECK].GetComponent<ScreenOutAppeared>().Action();


            // 球体
            e.MenuSphere[(int)MENU_SPHERE_TYPE.DECK_CREATE].GetComponent<BoyonAppeared>().Action();

        }

        public override void Execute(SceneMenu e)
        {

        }

        public override void Exit(SceneMenu e)
        {
            // 演出・描画を止める
            e.Info[(int)MENU_TYPE.DECK].GetComponent<ScreenOutAppeared>().Stop();

            // 球体
            e.MenuSphere[(int)MENU_SPHERE_TYPE.DECK_CREATE].GetComponent<BoyonAppeared>().Stop();
        }

        public override bool OnMessage(SceneMenu e, MessageInfo message)
        {

            // メッセージタイプが一致している箇所に
            switch (message.messageType)
            {
                case MessageType.ClickMenuButton:

                    // byte[]→構造体
                    SelectMenuNo info = new SelectMenuNo();
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(info));
                    Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(info));
                    info = (SelectMenuNo)Marshal.PtrToStructure(ptr, info.GetType());
                    Marshal.FreeHGlobal(ptr);
                    //+-------------------------------------------------------------
                    // ★選択したアイコンのタイプに変更
                    SelectData.iMenuType = info.selectNo;

                    //  セレクトタイプによってステート変更
                    switch ((MENU_TYPE)SelectData.iMenuType)
                    {
                        case MENU_TYPE.TOTORIAL:
                            e.m_pStateMachine.ChangeState(SceneMenuState.TutorialSelect.GetInstance());

                            break;
                        case MENU_TYPE.BATTLE:
                            e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());

                            break;
                        case MENU_TYPE.DECK:
                            //e.m_pStateMachine.ChangeState(SceneMenuState.DeckSelect.GetInstance());

                            break;
                        case MENU_TYPE.COLLECTION:
                            e.m_pStateMachine.ChangeState(SceneMenuState.CollectSelect.GetInstance());

                            break;
                        case MENU_TYPE.OPTION:
                            e.m_pStateMachine.ChangeState(SceneMenuState.OptionSelect.GetInstance());

                            break;
                        case MENU_TYPE.END:
                            Debug.LogWarning("SceneMenuState: それ以上のタイプはない。");
                            break;
                        default:
                            break;
                    }
                    return true; // trueを返して終り

                default:

                    break;
            }

            return false;　// 何も引っかからなかった
        }

    }

    //+-------------------------------------------
    //  コレクト選択中
    //+-------------------------------------------
    public class CollectSelect : BaseEntityState<SceneMenu>
    {
        static CollectSelect m_pInstance;
        public static CollectSelect GetInstance() { if (m_pInstance == null) { m_pInstance = new CollectSelect(); } return m_pInstance; }

        public override void Enter(SceneMenu e)
        {
            // メニュー選択カーソル移動
            Vector3 vNextPos = e.MenuButton[(int)SelectData.iMenuType].GetComponent<ScreenOutAppeared>().GetNextPos();
            e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().SetNextPos(vNextPos);
            e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().Action();

            // ステート毎のインフォメーションの文字のアニメ開始 (最初座標から再アクション)
            e.Info[(int)MENU_TYPE.COLLECTION].transform.localPosition = e.Info[(int)MENU_TYPE.COLLECTION].GetComponent<ScreenOutAppeared>().GetAwakPos();
            e.Info[(int)MENU_TYPE.COLLECTION].GetComponent<ScreenOutAppeared>().Action();


            // 球体
            e.MenuSphere[(int)MENU_SPHERE_TYPE.COLLECTION].GetComponent<BoyonAppeared>().Action();
            e.MenuSphere[(int)MENU_SPHERE_TYPE.SHOP].GetComponent<BoyonAppeared>().Action();

        }

        public override void Execute(SceneMenu e)
        {

        }

        public override void Exit(SceneMenu e)
        {

            // 演出・描画を止める
            e.Info[(int)MENU_TYPE.COLLECTION].GetComponent<ScreenOutAppeared>().Stop();

            // 球体
            e.MenuSphere[(int)MENU_SPHERE_TYPE.COLLECTION].GetComponent<BoyonAppeared>().Stop();
            e.MenuSphere[(int)MENU_SPHERE_TYPE.SHOP].GetComponent<BoyonAppeared>().Stop();

        }

        public override bool OnMessage(SceneMenu e, MessageInfo message)
        {

            // メッセージタイプが一致している箇所に
            switch (message.messageType)
            {
                case MessageType.ClickMenuButton:

                    // byte[]→構造体
                    SelectMenuNo info = new SelectMenuNo();
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(info));
                    Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(info));
                    info = (SelectMenuNo)Marshal.PtrToStructure(ptr, info.GetType());
                    Marshal.FreeHGlobal(ptr);
                    //+-------------------------------------------------------------
                    // ★選択したアイコンのタイプに変更
                    SelectData.iMenuType = info.selectNo;

                    //  セレクトタイプによってステート変更
                    switch ((MENU_TYPE)SelectData.iMenuType)
                    {
                        case MENU_TYPE.TOTORIAL:
                            e.m_pStateMachine.ChangeState(SceneMenuState.TutorialSelect.GetInstance());

                            break;
                        case MENU_TYPE.BATTLE:
                            e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());

                            break;
                        case MENU_TYPE.DECK:
                            e.m_pStateMachine.ChangeState(SceneMenuState.DeckSelect.GetInstance());

                            break;
                        case MENU_TYPE.COLLECTION:
                           // e.m_pStateMachine.ChangeState(SceneMenuState.CollectSelect.GetInstance());

                            break;
                        case MENU_TYPE.OPTION:
                            e.m_pStateMachine.ChangeState(SceneMenuState.OptionSelect.GetInstance());

                            break;
                        case MENU_TYPE.END:
                            Debug.LogWarning("SceneMenuState: それ以上のタイプはない。");
                            break;
                        default:
                            break;
                    }
                    return true; // trueを返して終り


                //case MessageType.ClickSphereButton:


                //    break;
                default:

                    break;
            }

            return false;　// 何も引っかからなかった
        }

    }

    //+-------------------------------------------
    //  オプション選択中
    //+-------------------------------------------
    public class OptionSelect : BaseEntityState<SceneMenu>
    {
        static OptionSelect m_pInstance;
        public static OptionSelect GetInstance() { if (m_pInstance == null) { m_pInstance = new OptionSelect(); } return m_pInstance; }

        public override void Enter(SceneMenu e)
        {
            // メニュー選択カーソル移動
            Vector3 vNextPos = e.MenuButton[(int)SelectData.iMenuType].GetComponent<ScreenOutAppeared>().GetNextPos();
            //e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().SetSpeed(0.5f);
            e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().SetNextPos(vNextPos);
            e.MenuSelectCursor.GetComponent<ScreenOutAppeared>().Action();

            // ステート毎のインフォメーションの文字のアニメ開始 (最初座標から再アクション)
            e.Info[(int)MENU_TYPE.OPTION].transform.localPosition = e.Info[(int)MENU_TYPE.OPTION].GetComponent<ScreenOutAppeared>().GetAwakPos();
            e.Info[(int)MENU_TYPE.OPTION].GetComponent<ScreenOutAppeared>().Action();

            // 球体
            e.MenuSphere[(int)MENU_SPHERE_TYPE.OPTION].GetComponent<BoyonAppeared>().Action();


        }

        public override void Execute(SceneMenu e)
        {

        }

        public override void Exit(SceneMenu e)
        {

            // 演出・描画を止める
            e.Info[(int)MENU_TYPE.OPTION].GetComponent<ScreenOutAppeared>().Stop();

            e.MenuSphere[(int)MENU_SPHERE_TYPE.OPTION].GetComponent<BoyonAppeared>().Stop();
        }

        public override bool OnMessage(SceneMenu e, MessageInfo message)
        {

            // メッセージタイプが一致している箇所に
            switch (message.messageType)
            {
                case MessageType.ClickMenuButton:

                    // byte[]→構造体
                    SelectMenuNo info = new SelectMenuNo();
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(info));
                    Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(info));
                    info = (SelectMenuNo)Marshal.PtrToStructure(ptr, info.GetType());
                    Marshal.FreeHGlobal(ptr);
                    //+-------------------------------------------------------------
                    // ★選択したアイコンのタイプに変更
                    SelectData.iMenuType = info.selectNo;

                    //  セレクトタイプによってステート変更
                    switch ((MENU_TYPE)SelectData.iMenuType)
                    {
                        case MENU_TYPE.TOTORIAL:
                            e.m_pStateMachine.ChangeState(SceneMenuState.TutorialSelect.GetInstance());

                            break;
                        case MENU_TYPE.BATTLE:
                            e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());

                            break;
                        case MENU_TYPE.DECK:
                            e.m_pStateMachine.ChangeState(SceneMenuState.DeckSelect.GetInstance());

                            break;
                        case MENU_TYPE.COLLECTION:
                            e.m_pStateMachine.ChangeState(SceneMenuState.CollectSelect.GetInstance());

                            break;
                        case MENU_TYPE.OPTION:
                            //e.m_pStateMachine.ChangeState(SceneMenuState.OptionSelect.GetInstance());

                            break;
                        case MENU_TYPE.END:
                            Debug.LogWarning("SceneMenuState: それ以上のタイプはない。");
                            break;
                        default:
                            break;
                    }
                    return true; // trueを返して終り

                default:

                    break;
            }

            return false;　// 何も引っかからなかった
        }

    }// class



    //+-------------------------------------------
    //  シーン移動
    //+-------------------------------------------
    public class SceneChange : BaseEntityState<SceneMenu>
    {
        static SceneChange m_pInstance;
        public static SceneChange GetInstance() { if (m_pInstance == null) { m_pInstance = new SceneChange(); } return m_pInstance; }

        public override void Enter(SceneMenu e)
        {

            int a = 0;
            a++;


            ////+-------------------------------------------------------------
            ////  セレクトタイプによってステート変更
            //// ステートマシンの初期化や切り替えは最後に行う

            //switch ((MENU_TYPE)SelectData.iMenuType)
            //{
            //    case MENU_TYPE.TOTORIAL:
            //        e.m_pStateMachine.ChangeState(SceneMenuState.TutorialSelect.GetInstance());

            //        break;
            //    case MENU_TYPE.BATTLE:
            //        e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());

            //        break;
            //    case MENU_TYPE.DECK:
            //        e.m_pStateMachine.ChangeState(SceneMenuState.DeckSelect.GetInstance());

            //        break;
            //    case MENU_TYPE.COLLECTION:
            //        e.m_pStateMachine.ChangeState(SceneMenuState.CollectSelect.GetInstance());

            //        break;
            //    case MENU_TYPE.OPTION:
            //        e.m_pStateMachine.ChangeState(SceneMenuState.OptionSelect.GetInstance());

            //        break;
            //    case MENU_TYPE.END:
            //        Debug.LogWarning("SceneMenuState: それ以上のタイプはない。");
            //        break;
            //    default:
            //        break;
            //}
            //return;

        }

        public override void Execute(SceneMenu e)
        {

        }

        public override void Exit(SceneMenu e)
        {

        }

        public override bool OnMessage(SceneMenu e, MessageInfo message)
        {

            return false; // 何も引っかからなかった
        }

    }


    //+-------------------------------------------
    //  ネットバトル時次の質問
    //+-------------------------------------------
    public class NetBattleSecondSelect : BaseEntityState<SceneMenu>
    {
        static NetBattleSecondSelect m_pInstance;
        public static NetBattleSecondSelect GetInstance()
        {
            if (m_pInstance == null) { m_pInstance = new NetBattleSecondSelect(); }
            return m_pInstance;
        }

        public override void Enter(SceneMenu e)
        {
            //+------------------------------------
            // 演出・描画を続ける
            e.Info[(int)MENU_TYPE.BATTLE].GetComponent<ScreenOutAppeared>().KeepUp();
            e.MenuSphere[(int)MENU_SPHERE_TYPE.BATTLE].GetComponent<BoyonAppeared>().KeepUp();
            e.MenuSphere[(int)MENU_SPHERE_TYPE.NET_BATTLE].GetComponent<BoyonAppeared>().KeepUp();
            e.MenuSphere[(int)MENU_SPHERE_TYPE.DECK_CREATE].GetComponent<BoyonAppeared>().KeepUp();

            // メニュー選択カーソル移動
            e.NetBattleGroup.GetComponent<ScreenOutAppeared>().Action();

            e.BlackPanel.SetActive(true);

        }

        public override void Execute(SceneMenu e)
        {
            e.NetBattleGroup.GetComponent<ScreenOutAppeared>().SelfUpdate();
        }

        public override void Exit(SceneMenu e)
        {
            e.NetBattleGroup.GetComponent<ScreenOutAppeared>().Stop();
            //e.BlackPanel.SetActive(false);

        }

        // ↓で使う
        enum NET_BATTLE_TYPE { SERVER, CLIENT, END };
        public override bool OnMessage(SceneMenu e, MessageInfo message)
        {
              
            // メッセージタイプが一致している箇所に
            switch (message.messageType)
            {
                case MessageType.ClickAnyButton:

                    // byte[]→構造体
                    AnyButton info = new AnyButton();
                    message.GetExtraInfo<AnyButton>(ref info);
                    //+-------------------------------------------------------------
                    // ★選択したアイコンのタイプに変更
                    switch ((NET_BATTLE_TYPE)info.Index)
                    {
                        //case NET_BATTLE_TYPE.SERVER:
                        //    e.m_pStateMachine.ChangeState(NetBattleHostSelected.GetInstance());
                        //    break;
                        //case NET_BATTLE_TYPE.CLIENT:
                        //    e.m_pStateMachine.ChangeState(NetBattleClientSelected.GetInstance());
                        //    break;
                        case NET_BATTLE_TYPE.SERVER:
                            // セレクトデータ保存
                            SelectData.networkType = NETWORK_TYPE.HOST;
                            // ネットワークロビーに行く
                            e.m_bSceneChange = true;
                            SceneManager.LoadScene("NetworkLobby");
                            break;
                        case NET_BATTLE_TYPE.CLIENT:
                            // セレクトデータ保存
                            SelectData.networkType = NETWORK_TYPE.CLIENT;
                            // IPアドレス保存
                            PlayerDataManager.GetPlayerData().ip = e.ipInput.text;
                            // ネットワークロビーに行く
                            e.m_bSceneChange = true;
                            SceneManager.LoadScene("NetworkLobby");
                            break;
                        case NET_BATTLE_TYPE.END:
                        default:
                            //+-------------------------------------------------------------------
                            // もどる
                            e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());
                            // 01/22 Exit関数からここに移動
                            e.BlackPanel.SetActive(false);
                            break;
                    }

                    return true; // trueを返して終り

                default:

                    break;
            }
            return false; // 何も引っかからなかった
        }

    }

    ////+-------------------------------------------
    ////  ネットバトル時ホスト選択時
    ////+-------------------------------------------
    //public class NetBattleHostSelected : BaseEntityState<SceneMenu>
    //{
    //    static NetBattleHostSelected m_pInstance;
    //    public static NetBattleHostSelected GetInstance()
    //    {
    //        if (m_pInstance == null) { m_pInstance = new NetBattleHostSelected(); }
    //        return m_pInstance;
    //    }

    //    public override void Enter(SceneMenu e)
    //    {
    //        // ホスト開始
    //        e.networkManager.StartHost();

    //        // 黒背景
    //        //e.BlackPanel.SetActive(true);

    //        // ロビー表示
    //        //e.NetBattleLobby.SetActive(true);
    //    }

    //    public override void Execute(SceneMenu e)
    //    {
    //        // 2人集まったら
    //        if (e.networkManager.GetNumServerConnections() >= 2)
    //        {
    //            // シーンチェンジフラグ
    //            e.m_bSceneChange = true;
    //            // シーンメインにいく
    //            SceneManager.LoadScene("Main");
    //        }
            
    //    }

    //    public override void Exit(SceneMenu e)
    //    {
    //        // ロビー非表示
    //        //e.NetBattleLobby.SetActive(false);
    //    }

    //    public override bool OnMessage(SceneMenu e, MessageInfo message)
    //    {

    //        // メッセージタイプが一致している箇所に
    //        switch (message.messageType)
    //        {
    //            case MessageType.ClickAnyButton:

    //                // byte[]→構造体
    //                AnyButton info = new AnyButton();
    //                message.GetExtraInfo<AnyButton>(ref info);

    //                //+-------------------------------------------------------------
    //                // ★選択したアイコンのタイプに変更
    //                switch (info.Index)
    //                {
    //                    case 0:
    //                        // 戻るボタン
    //                        // ホスト停止
    //                        e.networkManager.StopHost();
    //                        //+-------------------------------------------------------------------
    //                        // もどる
    //                        e.m_pStateMachine.ChangeState(SceneMenuState.NetBattleSecondSelect.GetInstance());
    //                        break;
    //                    default:
    //                        //+-------------------------------------------------------------------
    //                        // もどる
    //                        //e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());
    //                        break;
    //                }

    //                return true; // trueを返して終り

    //            default:

    //                break;
    //        }
    //        return false; // 何も引っかからなかった
    //    }

    //}

    ////+-------------------------------------------
    ////  ネットバトル時クライアント選択時
    ////+-------------------------------------------
    //public class NetBattleClientSelected : BaseEntityState<SceneMenu>
    //{
    //    static NetBattleClientSelected m_pInstance;
    //    public static NetBattleClientSelected GetInstance()
    //    {
    //        if (m_pInstance == null) { m_pInstance = new NetBattleClientSelected(); }
    //        return m_pInstance;
    //    }

    //    public override void Enter(SceneMenu e)
    //    {
    //        // クライアント開始(IPアドレスを引数に渡す)
    //        e.networkManager.StartClient2(e.ipInput.text);

    //        // 黒背景
    //        //e.BlackPanel.SetActive(true);

    //        // ロビー表示
    //        //e.NetBattleLobby.SetActive(true);
    //    }

    //    public override void Execute(SceneMenu e)
    //    { 
    //        // サーバーと繋がったら
    //        if (e.networkManager.GetNumClientConnections() >= 1)
    //        {
    //            // シーンチェンジフラグ
    //            e.m_bSceneChange = true;
    //            // シーンメインにいく
    //            SceneManager.LoadScene("Main");
    //        }
    //    }

    //    public override void Exit(SceneMenu e)
    //    {
    //        // ロビー非表示
    //        //e.NetBattleLobby.SetActive(false);
    //    }

    //    public override bool OnMessage(SceneMenu e, MessageInfo message)
    //    {

    //        // メッセージタイプが一致している箇所に
    //        switch (message.messageType)
    //        {
    //            case MessageType.ClickAnyButton:

    //                // byte[]→構造体
    //                AnyButton info = new AnyButton();
    //                message.GetExtraInfo<AnyButton>(ref info);
    //                //+-------------------------------------------------------------
    //                // ★選択したアイコンのタイプに変更
    //                switch (info.Index)
    //                {
    //                    case 0:
    //                        // 戻るボタン
    //                        // クライアント停止
    //                        e.networkManager.StopClient();
    //                        //+-------------------------------------------------------------------
    //                        // もどる
    //                        e.m_pStateMachine.ChangeState(SceneMenuState.NetBattleSecondSelect.GetInstance());
    //                        break;
    //                    default:
    //                        //+-------------------------------------------------------------------
    //                        // もどる
    //                        Debug.Log("c");
    //                        //e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());
    //                        break;
    //                }

    //                return true; // trueを返して終り

    //            default:

    //                break;
    //        }
    //        return false; // 何も引っかからなかった
    //    }

    //}



    //+-------------------------------------------
    //  説明ボタン
    //+-------------------------------------------
    public class Tips : BaseEntityState<SceneMenu>
    {
        static Tips m_pInstance;
        public static Tips GetInstance()
        {
            if (m_pInstance == null) { m_pInstance = new Tips(); }
            return m_pInstance;
        }

        public override void Enter(SceneMenu e)
        {
          
            // 黒背景
            e.BlackPanel.SetActive(true);

            e.TipsGroup.SetActive(true);
            int a = 0;
            a++;

        }

        public override void Execute(SceneMenu e)
        {



        }

        public override void Exit(SceneMenu e)
        {
            e.TipsGroup.SetActive(false);
        }

        public override bool OnMessage(SceneMenu e, MessageInfo message)
        {

            // メッセージタイプが一致している箇所に
            switch (message.messageType)
            {
                case MessageType.ClickAnyButton:

                    // byte[]→構造体
                    AnyButton info = new AnyButton();
                    message.GetExtraInfo<AnyButton>(ref info);
                    //+-------------------------------------------------------------
                    // ★選択したアイコンのタイプに変更
                    switch (info.Index)
                    {
                        case 1:
                            
                            e.Tips1.SetActive(false);
                            e.Tips2.SetActive(false);
                            e.Tips3.SetActive(false);
                            e.Tips4.SetActive(false);
                            //e.Tips5.SetActive(false);

                            e.Tips1.SetActive(true);
                            break;
                        case 2:
                            e.Tips1.SetActive(false);
                            e.Tips2.SetActive(false);
                            e.Tips3.SetActive(false);
                            e.Tips4.SetActive(false);
                           // e.Tips5.SetActive(false);

                            e.Tips2.SetActive(true);
                            break;
                        case 3:
                            e.Tips1.SetActive(false);
                            e.Tips2.SetActive(false);
                            e.Tips3.SetActive(false);
                            e.Tips4.SetActive(false);
                           // e.Tips5.SetActive(false);

                            e.Tips3.SetActive(true);
                            break;
                        case 4:
                            e.Tips1.SetActive(false);
                            e.Tips2.SetActive(false);
                            e.Tips3.SetActive(false);
                            e.Tips4.SetActive(false);
                            //e.Tips5.SetActive(false);

                            e.Tips4.SetActive(true);
                            break;
                        //case 5:
                        //    e.Tips1.SetActive(false);
                        //    e.Tips2.SetActive(false);
                        //    e.Tips3.SetActive(false);
                        //    e.Tips4.SetActive(false);
                        //    e.Tips5.SetActive(false);

                        //    e.Tips5.SetActive(true);
                        //    break;
                        default:
                            //+-------------------------------------------------------------------
                            // もどる
                            e.m_pStateMachine.ChangeState(SceneMenuState.BattleSelect.GetInstance());
                            break;
                    }

                    return true; // trueを返して終り

                default:

                    break;
            }
            return false; // 何も引っかからなかった
        }

    }


}// namespace



