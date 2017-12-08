using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*UVアニメーションクラス
*/

/* 使い方 */
/* ・UVアニメーションをさせたいメッシュを読み込む				*/
/* ・更新にUpdate関数・描画にRender関数をそれぞれ置く			*/
/* ・最後にAction関数を実行するとUVアニメーションが始まります。	*/

public class UVScroll : oul3DAnimBase
{
    [Range(-5.0f, 5.0f)]
    public float moveU = 0, moveV = 0;

    Vector2 UV;
    Vector2 StartUV; // 初期UV

    // bool bMaskUse = false;　// マスクを使うか？
    // Texture MaskTex;

    protected override void Awake()
    {
        // ベース初期化(rendererもここで取得している)
        base.Awake();

        // 最初は描画しないようにする
        gameObject.SetActive(false);
        
        //renderer.sharedMaterial.SetTextureOffset("_MainTex", Vector2.zero);
        UV = Vector2.zero;

        StartUV.x = renderer.materials[0].GetFloat("_ScrollU");
        StartUV.y = renderer.materials[0].GetFloat("_ScrollV");

    }

    protected override void Update()
    {
        if (isAction == false) return;//実行されてないなら出てけ！！

        // ベース更新
        base.Update();

        // UV更新
        UV.x += moveU * Time.deltaTime;
        UV.y += moveV * Time.deltaTime;
        if (UV.x < 0) UV.x = 1;
        if (UV.x > 1) UV.x = 0;
        if (UV.y < 0) UV.y = 1;
        if (UV.y > 1) UV.y = 0;
        //GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", UV);
        renderer.materials[0].SetFloat("_ScrollU", UV.x);
        renderer.materials[0].SetFloat("_ScrollV", UV.y);

        //Debug.Log(UV.y);
    }

    public override void Action(float delay = 0)
    {

        UV = StartUV;       // 初期のUV値をセット 
        // 初期UVセット 
        renderer.materials[0].SetFloat("_ScrollU", UV.x);
        renderer.materials[0].SetFloat("_ScrollV", UV.y);

        base.Action(delay);
        //UV = Vector2.zero;

    }

    // 座標
    public void Action(Vector3 vPos, float delay = 0)
    {

        UV = StartUV;       // 初期のUV値をセット 
        // 初期UVセット 
        renderer.materials[0].SetFloat("_ScrollU", UV.x);
        renderer.materials[0].SetFloat("_ScrollV", UV.y);

        // 座標
        base.SetPos(vPos);

        //UV = Vector2.zero;
        base.Action(delay);

    }

    // 座標とアングル
    public void Action(Vector3 vPos, Vector3 vStartAngle, Vector3 vEndAngle, float delay = 0)
    {

        UV = StartUV;       // 初期のUV値をセット
         
        // 初期UVセット 
        renderer.materials[0].SetFloat("_ScrollU", UV.x);
        renderer.materials[0].SetFloat("_ScrollV", UV.y);

        // 座標
        base.SetPos(vPos);

        // 角度
        base.AngleAnimation(vStartAngle, vEndAngle);

        //UV = Vector2.zero;
        base.Action(delay);

    }
}
