using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oul3DAnimBase : MonoBehaviour
{
    public float endTime = 10;      // 描画終りのフレーム
    public bool isRoop;        // ループエフェクトか
    public bool actionOnAwake; // しょっぱなからアクションするかどうか

    public bool useAlpha = false;    // 最後透明にするフラグ
    public float alphaNearTime;     // 透明じゃなくなる　始めるフレーム
    public float alphaFarTime;      // 透明じゃなくなる　終わるフレーム　

    public bool useScale = false;
    public float startScale = 1;  // 初期 拡大率
    public float endScale = 1;    // 最大 拡大率

    public bool useAngle = false;
    public Vector3 startAngle;   // 初期 アングル
    public Vector3 endAngle;     // 最大 アングル

    public bool isEnd { get; private set; }     // おわり
    public bool isAction { get; private set; }  // アニメを実行しているか

    protected float currentTime, delayTime;    // 今の時間
    protected new Renderer renderer;

    Transform cashTransform;

    // Use this for initialization
    protected virtual void Awake()
    {
        renderer = GetComponent<Renderer>();
        Debug.Assert(renderer, gameObject.name + ".3DAnimBase:レンダラ―がない！");

        isEnd = isAction = false;

        cashTransform = transform;

        if (actionOnAwake) Action();
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        if (isAction == false) return;//実行されてないなら出てけ！！

        // ディレイ処理
        if (delayTime > 0)
        {
            delayTime = Mathf.Max(delayTime - Time.deltaTime, 0);
            return;
        }

        //Debug.Log(currentTime);

        //if (isRoop == false)//ループじゃなかったら
        {
            // フレーム更新
            currentTime += Time.deltaTime;
            if (isRoop == true)// ループだったらalphaFarの前のとこでループし続ける処理
            {
                //if (currentTime >= (alphaFarTime - 0.016f))
                //{
                //    currentTime = (alphaFarTime - 0.016f);
                //}
                if (currentTime >= endTime) currentTime -= endTime;
            }

            if (currentTime >= endTime)
            {
                isAction = false;

                // 追加
                isEnd = true;

                gameObject.SetActive(false);

                //Debug.Log("キテルグマ");
            }
        }

        // 透明度更新
        if (useAlpha)
        {
            float alpha;

            // α二アーの前か後ろで判定を変える
            if (currentTime >= alphaNearTime)
            {
                // 100-100=0  100-50=50   0/50
                float A = endTime - currentTime;
                float B = endTime - alphaFarTime;
                alpha = A / B;
                //alpha = Clamp(alpha, 0.0f, 1.0f);

            }
            else
            {
                // 最初の
                alpha = currentTime / alphaNearTime;//   0/30=0   60/30=2   1-(0~1)  

            }

            //alpha = (alphaFar - nowFlame) / (alphaFar - alphaNear);
            alpha = Mathf.Clamp(alpha, 0.0f, 1.0f);//指定された値を 0 ～ 1 の範囲にクランプします

            //Debug.Log(alpha);

            // メッシュ透明度変更
            SetAlpha(alpha);

            //Debug.Log(alpha);
        }

        float rate = currentTime / endTime; // 最初にレートを出す

        // スケール更新
        if (useScale)
        {
            SetScale(Mathf.Lerp(startScale, endScale, rate));
        }
        // 回転更新
        if (useAngle)
        {
            Vector3 calcAngle = (startAngle * (1.0f - rate)) + (endAngle * rate);
            cashTransform.localEulerAngles = calcAngle;
        }
    }

    public virtual void Action(float delay = 0)
    {
        gameObject.SetActive(true);

        isAction = true;//起動
        currentTime = 0;
        delayTime = delay;

        isEnd = false;

        // ディレイ考慮で初期設定
        if (useAlpha) SetAlpha((alphaNearTime == 0) ? 1 : 0);
        if (useScale) SetScale(startScale);
        if (useAngle) transform.localEulerAngles = startAngle;

        //Debug.Log("キテルグマ");
    }

    public void Stop()
    {
        isAction = false;//止める
        currentTime = 0;
    }

    void SetAlpha(float alpha)
    {
        Color newColor = renderer.material.color;
        newColor.a = alpha;
        renderer.material.color = newColor;
    }

    void SetScale(float scale)
    {
        cashTransform.localScale = new Vector3(scale, scale, scale);
    }


    // 不透明度アニメ
    public void AlphaAnimation(float AlphaNearTime, float AlphaFarTime)
    {
        useAlpha = true;

        alphaNearTime = AlphaNearTime;
        alphaFarTime = AlphaFarTime;
    }

    // 拡大アニメ
    public void ScaleAnimation(float StartScale, float EndScale)
    {
        useScale = true;

        startScale = StartScale;
        endScale = EndScale;
    }

    // 回転アニメ
    public void AngleAnimation(Vector3 StartAngle, Vector3 EndAngle)
    {
        useAngle = true;

        startAngle = StartAngle;
        endAngle = EndAngle;
    }

    // 座標セット
    public void SetPos(Vector3 vPos)
    {
        transform.localPosition = vPos;
    }

}
