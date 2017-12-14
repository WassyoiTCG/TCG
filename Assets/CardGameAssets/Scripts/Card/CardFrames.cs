using System;
using System.Collections;
//using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

//+---------------------------------
//  フレーム用
//+---------------------------------
public static class CardFrames {

    // 
    static Sprite StrikerFrame;
    static Sprite AbilityStrikerFrame;
    static Sprite SupportFrame;
    static Sprite EventFrame;
    static Sprite JOKERFrame;

    // 初期化をゲーム中一回きりにしたいから
    static bool isInit = false;

    // Use this for initialization
    public static void Start () 
    {

    }

    public static void Init()
    {

        if (isInit == true) return;
        isInit = true;

        var texture1 = PngLoader.LoadPNG("Customize/Frame/StrikerFrame.png");
        StrikerFrame = Sprite.Create(texture1, new Rect(0, 0, texture1.width, texture1.height), new Vector2(0.5f, 0.5f));

        var texture2 = PngLoader.LoadPNG("Customize/Frame/AbilityStrikerFrame.png");
        AbilityStrikerFrame = Sprite.Create(texture2, new Rect(0, 0, texture2.width, texture2.height), new Vector2(0.5f, 0.5f));

        var texture3 = PngLoader.LoadPNG("Customize/Frame/SupportFrame.png");
        SupportFrame = Sprite.Create(texture3, new Rect(0, 0, texture3.width, texture3.height), new Vector2(0.5f, 0.5f));

        var texture4 = PngLoader.LoadPNG("Customize/Frame/EventFrame.png");
        EventFrame = Sprite.Create(texture4, new Rect(0, 0, texture4.width, texture4.height), new Vector2(0.5f, 0.5f));

        var texture5 = PngLoader.LoadPNG("Customize/Frame/JOKERFrame.png");
        JOKERFrame = Sprite.Create(texture5, new Rect(0, 0, texture5.width, texture5.height), new Vector2(0.5f, 0.5f));

    }

    // Update is called once per frame
    public　static void Update () {
		
	}

    // アクセサ-
    public static Sprite GetStrikerFrame() { return StrikerFrame; }

    public static Sprite GetAbilityStrikerFrame() { return AbilityStrikerFrame; }

    public static Sprite GetSupportFrame() { return SupportFrame; }

    public static Sprite GetEventFrame() { return EventFrame; }
    
    public static Sprite GetJOKERFrame() { return JOKERFrame; }



}
