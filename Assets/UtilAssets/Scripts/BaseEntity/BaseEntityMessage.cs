using System;
using System.Runtime.InteropServices;
using UnityEngine;


public enum MessageType
{
    NoMessage,

    ClickBackButton,
    ClickAnyButton,  // なんか適当なボタン
    /* Menu */
    ClickMenuButton,
    ClickSphereButton,

    /* Deck */
    ClickLineButton,
  

    /* Main */
    //NetPlay,        // ネット対戦プレイ選択
    //OfflinePlay,    // オフライン対戦プレイ選択
    SyncDeck,       // 山札・手札・墓地・追放情報の同期
    SyncPoint,      // ポイント情報の同期
    SetCard,        // カードをセット
    BackToHand,     // セットしたカードを手札に戻す動作
    SetStrikerOK,   // ストライカーセット完了
    SetStrikerPass, // ストライカーセットをパス
    Marigan,        // マリガン
    NoMarigan,      // マリガンなし
    SelectNumber,   // 数字選択
    Restart,        // もう一度
    EndGame,        // ゲーム終了

    ReMessage,      // もっかいそのメッセージおくって
    ReceiveOK,
}

// デッキ情報同期する用構造体(int型はカードのID)
public struct SyncDeckInfo
{
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
    //public int[] hand;         // 手札
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    public int[] yamahuda;     // 山札
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    //public int[] bochi;        // 墓地
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    //public int[] tuihou;       // 追放
}

public struct ReceiveOKInfo
{
    public uint iMessageType;
}

public struct ReMessageInfo
{
    public int messageNumber;
}


public struct PointInfo
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public int[] points;
}

public struct SetCardInfo
{
    public int handNo;  // 手札の何番目のカードがセットされたか
}

public struct BackToHandInfo
{
    public int iCardType;
}

public struct SelectNumberInfo
{
    public int selectNumber;
}

public struct MessageInfo
{
    public MessageType messageType;                 // メッセージのID
    public int fromPlayerID;                        // 誰が送ってきたか
    public int number;                              // 何番目のメッセージか(これでとばしてしまったかどうかわかるかもしれない)
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public byte[] exInfo;                           // いつものinfo
    // exInfoのセット
    public void SetExtraInfo(object structure)
    {
        int size = Marshal.SizeOf(structure);
        IntPtr ptr = Marshal.AllocHGlobal(size);
        exInfo = new byte[size];
        Marshal.StructureToPtr(structure, ptr, false);
        Marshal.Copy(ptr, exInfo, 0, size);

        /*
            但しこれではバッファリングのためにメモリを倍食っています。それが嫌ならGCHandleでbyte配列のポインタを取得し、直接そこに書き込むこともできます。
            int size = Marshal.SizeOf(obj);
            byte[] bytes = new byte[size];
            GCHandle gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(obj, gch.AddrOfPinnedObject(), false);
            gch.Free();
         */

        //exInfo = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
        //Marshal.StructureToPtr(structure, exInfo, false);
    }

    public void GetExtraInfo<T>(ref T outStructure)
    {
        Debug.Assert(exInfo != null, "exInfoがnullなのに値を取得しようとしています。\r\nnullチェックで呼ばないようにするか、ex構造体を追加するのを忘れています");

        // byte[]→構造体
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(outStructure));
        Marshal.Copy(exInfo, 0, ptr, Marshal.SizeOf(outStructure));
        outStructure = (T)Marshal.PtrToStructure(ptr, outStructure.GetType());
        Marshal.FreeHGlobal(ptr);
    }
}

//+-----------------------------------------------------------
//  メニュー画面用
//+-----------------------------------------------------------
public struct AnyButton
{
    public int Index;
}
public struct SelectMenuNo
{
    public int selectNo;
}
public struct SelectSphereNo
{
    public int selectNo;
}

//+-----------------------------------------------------------
//  メニュー画面用
//+-----------------------------------------------------------

// ライン変更
public struct ChangeLine
{
    public int iNextLine;
}
