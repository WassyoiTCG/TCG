using System;
using UnityEngine;

public static class PngLoader
{
    public static Texture2D LoadPNG(string path)
    {
        try
        {
            // ファイルバッファ
            byte[] buf = oulFile.ReadAllBytes(path);

            // バイナリオープン
            //using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            //using (BinaryReader reader = new BinaryReader(file))
            //{
            //    buf = reader.ReadBytes((int)reader.BaseStream.Length);

            // 16バイトから開始
            int pos = 16;

            // 画像幅計算
            int width = 0;
            for (int i = 0; i < 4; i++) width = width * 256 + buf[pos++];
            int height = 0;
            for (int i = 0; i < 4; i++) height = height * 256 + buf[pos++];

            // テクスチャ作成
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(buf);

            return texture;
        }
        catch (Exception e)
        {
            ExceptionMessage.Message("png load error!", e);
        }

        return null;
    }
}
