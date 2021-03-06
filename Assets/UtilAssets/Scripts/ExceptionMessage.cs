﻿using UnityEngine;

public static class ExceptionMessage
{
    public static void Message(string errorStr, System.Exception e)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayDialog(errorStr, System.Environment.StackTrace + "\r\n" + e.GetType().FullName + "\r\nan exception was thrown.", "OK");
#else
        oulFile.OutPutLog(Application.dataPath + "/log.txt", System.Environment.StackTrace + "\r\n" + e.GetType().FullName + "\r\nan exception was thrown.");
#endif
    }

    public static void MessageBox(string errorStr, string Message)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayDialog(errorStr, Message, "OK");
#else

#endif
    }

}