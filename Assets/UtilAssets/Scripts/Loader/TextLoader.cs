using System;
using System.IO;
using System.Text;
using UnityEngine;

public class TextLoader
{
    char[] fileBuf;      // ファイルバッファ
    int readCursor;      // 現在読む進めている位置

    public void LoadText(string fileName)
    {
        try
        {
            //oulFile.OutPutLog(Application.dataPath + "/log.txt", fileName + "\r\n");

            // これだと何故かexe実行の時だけSystem.ArgumentExceptionがでる。どうやらUnityはshift_jisをサポートしていないようだ
            // http://answers.unity3d.com/questions/42955/codepage-1252-not-supported-works-in-editor-but-no.html
            using (StreamReader reader = new StreamReader(fileName, Encoding.GetEncoding("shift_jis")))

            // テキストオープン
            //using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            //using (StreamReader reader = new StreamReader(file/*, Encoding.GetEncoding("shift_jis")*/))
            {
                // 全読み
                fileBuf = reader.ReadToEnd().ToCharArray();
                //str = "unko\r\n";

                // 値初期化
                readCursor = 0;
            }

            //oulFile.OutPutLog(Application.dataPath + "/log.txt", str);
        }
        catch (Exception e)
        {
            ExceptionMessage.Message("text file error", e);
        }
    }

    // 行読み出し
    public string ReadLine()
    {
        // 改行が来るまで文字を回収する
        char[] goalChars = { '\r' };
        string ret = ToReadGetString(goalChars);
        // \rの次は\nなので、1文字進める
        readCursor++;

        return ret;
    }

    // 文字列読み出し
    public string ReadString()
    {
        // 空白か改行じゃなくなるまで進める
        char[] goalChars = { ' ', '\n', '\r', '\t' };
        if (!ToReadXOR(goalChars)) return "";

        // 改行か空白が来るまで文字を回収して返す
        return ToReadGetString(goalChars);
    }

    // ""まで進め、""で区切られた中身を返す
    public string ReadDoubleQuotation()
    {
        // "まで進める
        char[] goalChars = { '"' };
        if (!ToRead(goalChars, true)) return "";

        // 次の"までの文字を回収して返す
        return ToReadGetString(goalChars);
    }

    // 整数型読み出し
    public int ReadInt()
    {
        // 数字まで進める
        char[] goalChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-' };
        if (!ToRead(goalChars, false)) return 0;
        // 空白か改行かタブか,か)まで
        char[] goalChars2 = { ' ', '\n', '\r', '\t', ',', ')' };
        string intString = ToReadGetString(goalChars2);
        return int.Parse(intString);
    }

    // 浮動小数点型読み出し
    public float ReadFloat()
    {
        // 数字まで進める
        char[] goalChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-' };
        if (!ToRead(goalChars, false)) return 0;
        // 空白か改行かタブか,か)まで
        char[] goalChars2 = { ' ', '\n', '\r', '\t', ',', ')' };
        return float.Parse(ToReadGetString(goalChars2));
        //string temp = ReadString();
        //return (temp != "") ? float.Parse(temp) : 0;
    }

    // 目標の文字が見つかるまで進める(nextは目標の文字の次にカーソルを移動させるか)
    bool ToRead(char[] goalChars, bool nextCursor = true)
    {
        while (true)
        {
            // ファイル終端判定
            if (isEOF()) return false;

            // 2バイト文字判定
            if (IsDBCSLeadByte(readCursor))
            {
                readCursor += 2;
                continue;
            }

            // 今の位置が目標のcharか判定
            foreach (char c in goalChars)
            {
                if (fileBuf[readCursor] == c)
                {
                    // カーソルを進める
                    if (nextCursor) readCursor++;

                    // 見つかったので終了
                    return true;
                }
            }

            // 読み進め
            readCursor++;
        }

        // 到達できないコードって出る。isEOFで必ず返ることを想定しているんだろうけど賢すぎ
        //return false;
    }

    // 引数の文字以外が出るまで読み進める
    bool ToReadXOR(char[] goalChars)
    {
        while (true)
        {
            // ファイル終端判定
            if (isEOF()) return false;

            // 2バイト文字判定
            if (IsDBCSLeadByte(readCursor))
            {
                readCursor += 2;
                continue;
            }

            // 今の位置が引数文字以外か
            bool isDefferent = true;
            foreach (char c in goalChars)
            {
                if (fileBuf[readCursor] == c)
                {
                    isDefferent = false;
                    break;
                }
            }
            // 1文字も引数の文字ではない
            if (isDefferent)
            {
                // 目標以外が出たので終了
                return true;
            }

            // 読み進め
            readCursor++;
        }

        // 到達できないコードって出る。isEOFで必ず返ることを想定しているんだろうけど賢すぎ
        //return false;
    }

    // 目標の文字が見つかるまで進め、その過程の文字を返す(nextは目標の文字の次にカーソルを移動させるか)
    string ToReadGetString(char[] goalChars, bool nextCursor = true)
    {
        string ret = "";

        // 目標の文字が見つかるまで進める
        while (true)
        {
            // ファイル終端判定
            if (isEOF()) return ret;

            // 2バイト文字判定
            if (IsDBCSLeadByte(readCursor))
            {
                ret += fileBuf[readCursor++];
                ret += fileBuf[readCursor++];
                continue;
            }

            // 今の位置が目標のcharか判定
            foreach (char c in goalChars)
            {
                if (fileBuf[readCursor] == c)
                {
                    // カーソルを進める
                    if (nextCursor) readCursor++;

                    // 見つかったので終了
                    return ret;
                }
            }

            // 読み進めつつ文字回収
            ret += fileBuf[readCursor++];
        }

        // 到達できないコードって出る。isEOFで必ず返ることを想定しているんだろうけど賢すぎ
        //return ret;
    }

    // 2バイト文字判定
    bool IsDBCSLeadByte(int index)
    {
        /*
            ＪＩＳでは漢字の始めと終わりに制御コードをつけて１バイト

            文字と区分していましたが
            
            ＳｉｆｔＪＩＳではキーボードの１バイト文字に使われていない
            
            コード「８１～９Ｆ」と「Ｅ０～ＥＦ」を２バイト文字の１桁目
            
            に使い、二桁目は「４０～ＦＣ（７Ｆを除く）」の組み合わせで
            
            決められています。
            
            １桁目のコードが「８１～９Ｆ」と「Ｅ０～ＥＦ」なら続く
            
            もう一桁を結合させ漢字に変換し、そうでなければ１バイト文字
            
            として処理します。
            
            そのため漢字の始めと終わりに制御コードが不要になりました。
        */
        //return (Encoding.GetEncoding("shift_jis").GetByteCount(fileBuf, index, 1) == 2);
        if (index < fileBuf.Length - 1)
        {
            char digit1 = fileBuf[index], digit2 = fileBuf[index + 1];
            if ((digit1 >= 0x81 && digit1 <= 0x9f) || (digit1 >= 0xe0 && digit1 <= 0xef))
            {
                if (digit2 >= 0x40 && digit2 <= 0xfc && digit2 != 0x7f) return true;
            }
        }

        return false;
    }

    public bool isEOF() { return (readCursor >= fileBuf.Length); }
}
