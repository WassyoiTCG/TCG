using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace OwatasoUtilAssets.ImageEffects
//{
// ゲームを起動しなくても稼働する(つまり値を変えながらのプレビューが可能)
//[ExecuteInEditMode]
public class RadialBlur : MonoBehaviour
{
    [Range(-1, 1)]
    public float centerX = 0, centerY = 0;

    [Range(0.0f, 50.0f)]
    public float power = 1.0f;

    [Range(1, 8)]
    public int numOfSampling = 8;

    public Shader radialBlurShader = null;

    Material material = null;

    void Awake()
    {
        Debug.Assert(radialBlurShader);
        material = new Material(radialBlurShader);
    }

    //protected void OnDisable()
    //{
    //    if (blurMaterial)
    //    {
    //        DestroyImmediate(blurMaterial);
    //    }
    //}

    // --------------------------------------------------------

    void Start()
    {
        // Disable if we don't support image effects
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
        // Disable if the shader can't run on the users graphics card
        if (!radialBlurShader || !material.shader.isSupported)
        {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        power = Mathf.Max(power - (Time.deltaTime * 90), 0);

        // シェーダーに値をセット
        material.SetFloat("_CenterX", centerX);
        material.SetFloat("_CenterY", centerY);
        material.SetFloat("_BlurPower", power);
        material.SetInt("_NumOfSampling", numOfSampling);
    }

    // Called by the camera to apply the image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //int rtW = source.width / 4;
        //int rtH = source.height / 4;
        //RenderTexture buffer = RenderTexture.GetTemporary(source.width, source.height, 0);
        //
        //// Copy source to the 4x4 smaller texture.
        //DownSample4x(source, buffer);

        // Blur the small texture
        //for (int i = 0; i < iterations; i++)
        //{
        //    RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
        //    FourTapCone(buffer, buffer2, i);
        //    RenderTexture.ReleaseTemporary(buffer);
        //    buffer = buffer2;
        //}

        if (power > 0)
        {
            // sourceをシェーダーの_MainTexに設定してdestination(nullなら直接画面に)に描画している。materialに設定したシェーダーで描画する
            Graphics.Blit(source, destination, material);
        }
        else
        {
            // パワーが0ならシェーダー描画しないのと同じなので、シェーダーを使わない描画
            Graphics.Blit(source, destination);
        }
        //RenderTexture.ReleaseTemporary(buffer);
    }

    public void SetBlur(float centerX, float centerY, float power = 50)
    {
        this.centerX = centerX;
        this.centerY = centerY;
        this.power = power;
    }
}
//}
