using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oulSystem : MonoBehaviour
{
    [Range(10, 60)]
    public int frameRate = 30;

    [Range(0, 1)]
    public float timeScale = 1;

    Camera cam;
    // 画面のサイズ
    public int windowWidth = 1600;
    public int windowHeight = 900;
    // 画像のPixel Per Unit
    public float pixelPerUnit = 100f;

    void Awake()
    {
        // フレームレート設定
        Application.targetFrameRate = frameRate;

        // タイムスケール設定
        Time.timeScale = timeScale;

        // サウンド初期化
        oulAudio.Initialize();

        // ウィンドウサイズサイズ
        Screen.SetResolution(windowWidth, windowHeight, false, 60);

        // カメラコンポーネントを取得します
        cam = Camera.main;
        UpdateAspect();
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

        UpdateAspect();
	}

    void UpdateAspect()
    {
        //float aspect = (float)Screen.height / (float)Screen.width;
        //float bgAcpect = height / width;

        //// カメラのorthographicSizeを設定
        //cam.orthographicSize = (height / 2f / pixelPerUnit);

        //if (bgAcpect > aspect)
        //{
        //    // 倍率
        //    float bgScale = height / Screen.height;
        //    // viewport rectの幅
        //    float camWidth = width / (Screen.width * bgScale);
        //    // viewportRectを設定
        //    cam.rect = new Rect((1f - camWidth) / 2f, 0f, camWidth, 1f);
        //}
        //else
        //{
        //    // 倍率
        //    float bgScale = width / Screen.width;
        //    // viewport rectの幅
        //    float camHeight = height / (Screen.height * bgScale);
        //    // viewportRectを設定
        //    cam.rect = new Rect(0f, (1f - camHeight) / 2f, 1f, camHeight);
        //}
    }

    public void SetFrameRate(int frameRate)
    {
        this.frameRate = frameRate;

        // フレームレート設定
        Application.targetFrameRate = frameRate;
    }
}