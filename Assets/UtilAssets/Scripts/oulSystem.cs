using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oulSystem : MonoBehaviour
{
    [Range(10, 60)]
    public int frameRate = 30;

    [Range(0, 1)]
    public float timeScale = 1;

    void Awake()
    {
        // フレームレート設定
        //Application.targetFrameRate = frameRate;

        // タイムスケール設定
        Time.timeScale = timeScale;

        // サウンド初期化
        oulAudio.Initialize();
    }

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {
        // 入力更新(staticクラス(MonoBehaviour非継承)なので、手動でUpdateを呼ぶ)
        oulInput.Update();
	}

    public void SetFrameRate(int frameRate)
    {
        this.frameRate = frameRate;

        // フレームレート設定
        Application.targetFrameRate = frameRate;
    }
}