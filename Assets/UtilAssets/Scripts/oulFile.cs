using System.IO;

public static class oulFile
{
    static public void OutPutLog(string path, string str)
    {
        //using (FileStream file = new System.IO.FileStream(path, FileMode.OpenOrCreate, System.IO.FileAccess.Write)) 
        using (StreamWriter writer = File.AppendText(path))
        {
            writer.Write(str);
        }
    }

    static public string[] EnumDirectory(string sDirectory)
    {
        //string cdir = Directory.GetCurrentDirectory();
        //string[] files = Directory.GetFiles(cdir);
        //string[] dirs = Directory.GetDirectories(cdir);

        // 変数sDirectory以下のサブフォルダをすべて取得する
        // ワイルドカード"*"は、すべてのフォルダを意味する
        string[] ret = Directory.GetDirectories(sDirectory, "*", SearchOption.AllDirectories);

        // ディレクトリパスまで入っているので、フォルダー名だけにする
        for (int i = 0; i < ret.Length; i++) ret[i] = Path.GetFileName(ret[i]);

        return ret;
    }

    static public byte[] ReadAllBytes(string path)
    {
        try
        {
            byte[] buf;
            // バイナリオープン
            using (FileStream file = new System.IO.FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new System.IO.BinaryReader(file))
            {
                buf = reader.ReadBytes((int)reader.BaseStream.Length);
            }

            return buf;
        }

        catch (System.Exception e)
        {
            ExceptionMessage.Message("file error", e);
        }

        return null;
    }
}
