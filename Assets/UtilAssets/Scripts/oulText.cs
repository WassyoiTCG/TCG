using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oulText : MonoBehaviour
{
    List<string> displayList = new List<string>();

    // FPS
    int fps;
    int fpsCounter;

    // Fixed FPS
    int fixedFps;
    int fixedFpsCounter;

    // Debug Log
    string debugString;
    string debugString2;

    // Timer
    float oneSecondTime;

    void Start()
    {
        oneSecondTime = 0f;
    }

    void Update()
    {
        // FPS
        if (oneSecondTime >= 1f)
        {
            fps = fpsCounter;
            fixedFps = fixedFpsCounter;

            // reset
            fpsCounter = 0;
            fixedFpsCounter = 0;
            oneSecondTime = 0f;
        }
        else
        {
            fpsCounter++;
            oneSecondTime += Time.deltaTime;
        }

        // structure debug string
        debugString = "";
        debugString2 = "";

        for (int i = 0; i < displayList.Count; i++)
        {
            if (i >= 30)
            {
                debugString2 += displayList[i];
                debugString2 += "\n";
            }
            else
            {
                debugString += displayList[i];
                debugString += "\n";
            }
        }
        displayList.Clear();
    }

    void FixedUpdate()
    {
        fixedFpsCounter++;
    }

    void OnGUI()
    {
        GUI.Label(
            new Rect(0.0f, 0.0f, Screen.width, Screen.height),
            "FPS: " + fps + "  FixedUpdate: " + fixedFps + "\n" + debugString);

        GUI.Label(
        new Rect(240.0f, 0.0f, Screen.width, Screen.height),
        debugString2);
    }

    public void Set(string str)
    {
        displayList.Add(str);
    }
}
