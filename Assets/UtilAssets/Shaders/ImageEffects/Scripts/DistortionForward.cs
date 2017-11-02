using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームを起動しなくても稼働する(つまり値を変えながらのプレビューが可能)
//[ExecuteInEditMode]
public class DistortionForward : MonoBehaviour
{
    // 歪み用オブジェクトがない写真を保存
    public RenderTexture backUp { get; private set; }

    // Use this for initialization
    void Start()
    {

    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // 歪み用オブジェクトがない写真を保存
        backUp = source;
        //Graphics.Blit(source, backUp);

        // 描画しない
        //Graphics.Blit(source, destination);
    }
}
