using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Animator animator;

    public Canvas canvas;

    //public Image cardFrameImage;
    //public GameObject fighterPowerFrame;
    public SpriteRenderer fighterPowerFrame;
    //public Text powerText;
    public Number powerNumber;
    public Text cardNameText;
    public Text syuzokuText;
    //public Image cardImage;
    public Renderer cardFrameRenderer;
    public Renderer cardSleeveRenderer;
    public Renderer cardImageRenderer;

    public bool isMyPlayerSide;
    public bool isSetField = false;
    public bool uraomoteFlag;                           // 裏表
    public bool notSelectFlag { get; private set; }      // 手札から選択できないフラグ

    //public Vector3 handPosition;    // 手札の座標保存用

    // ステートマシンからのアクセス用
    public float timer;
    public Vector3 startPosition;
    public Vector3 startAngle;
    public Vector3 nextPosition;    // なんか補間とかで使う
    public Vector3 nextAngle;

    public Transform cacheTransform;

    // フレームのテクスチャ取得用
    public CardObjectManager cardObjectManager;

    public CardData cardData { get; private set; }

    public BaseEntityStateMachine<Card> stateMachine { get; private set; }

    public CardObjectState.OpenCardInfo openCardInfo = new CardObjectState.OpenCardInfo();

    public UVScroll cardFrameEffect;
    public UVScroll cardFrameEffect2;
    public bool isCardFrameEffect = false;

    void Awake()
    {
        //canvas = GetComponent<Canvas>();
        cacheTransform = transform;

        // ステート初期化
        stateMachine = new BaseEntityStateMachine<Card>(this);
        stateMachine.ChangeState(CardObjectState.None.GetInstance());

       // this.animator.Play("Idle");
    }

    public void ClearMotion()
    {
        //this.animator.Play("Idle");
    }

    void Update()
    {
        // ステート更新
        stateMachine.Update();
    }

    public void SetOrder(int no)
    {
        fighterPowerFrame.sortingOrder = no + 1;
        powerNumber.SetOrder(no + 2);
        cardFrameRenderer.sortingOrder = no;
        cardSleeveRenderer.sortingOrder = no;
        cardImageRenderer.sortingOrder = no;
        canvas.sortingOrder = no;
        cardFrameEffect.gameObject.GetComponent<MeshRenderer>().sortingOrder = no;
        cardFrameEffect2.gameObject.GetComponent<MeshRenderer>().sortingOrder = no;
    }

    public void SetCardData(CardData data)
    {
        if (!cardObjectManager) cardObjectManager = cacheTransform.parent.GetComponent<CardObjectManager>();

        cardData = data;

        // カード名
        cardNameText.text = data.cardName;
        // カード画像
        //cardSprite.sprite = data.image;
        cardImageRenderer.materials[0].SetTexture("_MainTex", data.image.texture); 

        switch (data.cardType)
        {
            case CardType.Fighter:
                {
                    // フレーム差し替え
                    if (cardObjectManager)
                        if (cardFrameRenderer.materials[0].GetTexture("_MainTex") != cardObjectManager.strikerFrame)
                            cardFrameRenderer.materials[0].SetTexture("_MainTex", cardObjectManager.strikerFrame);

                    fighterPowerFrame.gameObject.SetActive(true);

                    var fighter = data.GetFighterCard();
                    // パワー
                    powerNumber.SetNumber(data.power);
                    // 種族
                    syuzokuText.text = CardDataBase.SyuzokuString[(int)fighter.syuzokus[0]];
                    for (int i = 1; i < fighter.syuzokus.Length; i++)
                        syuzokuText.text += " / " + CardDataBase.SyuzokuString[(int)fighter.syuzokus[i]];
                }
                break;
            case CardType.AbilityFighter:
                {
                    // フレーム差し替え
                    if (cardObjectManager)
                        if (cardFrameRenderer.materials[0].GetTexture("_MainTex") != cardObjectManager.abilityStrikerFrame)
                            cardFrameRenderer.materials[0].SetTexture("_MainTex", cardObjectManager.abilityStrikerFrame);

                    fighterPowerFrame.gameObject.SetActive(true);

                    var fighter = data.GetFighterCard();
                    // パワー
                    powerNumber.SetNumber(data.power);
                    // 種族
                    syuzokuText.text = CardDataBase.SyuzokuString[(int)fighter.syuzokus[0]];
                    for (int i = 1; i < fighter.syuzokus.Length; i++)
                        syuzokuText.text += " / " + CardDataBase.SyuzokuString[(int)fighter.syuzokus[i]];
                }
                break;

            case CardType.Joker:
                // フレーム差し替え
                if (cardObjectManager)
                    if (cardFrameRenderer.materials[0].GetTexture("_MainTex") != cardObjectManager.jokerFrame)
                        cardFrameRenderer.materials[0].SetTexture("_MainTex", cardObjectManager.jokerFrame);
                fighterPowerFrame.gameObject.SetActive(true);
                powerNumber.SetNumber(0);
                break;

            case CardType.Support:
                // フレーム差し替え
                if (cardObjectManager)
                    if (cardFrameRenderer.materials[0].GetTexture("_MainTex") != cardObjectManager.supportFrame)
                        cardFrameRenderer.materials[0].SetTexture("_MainTex", cardObjectManager.supportFrame);
                fighterPowerFrame.gameObject.SetActive(false);
                syuzokuText.text = "サポートカード";
                //powerNumber.SetNumber(data.power);
                break;

            case CardType.Connect:
                // フレーム差し替え
                if (cardObjectManager)
                    if (cardFrameRenderer.materials[0].GetTexture("_MainTex") != cardObjectManager.eventFrame)
                        cardFrameRenderer.materials[0].SetTexture("_MainTex", cardObjectManager.eventFrame);
                fighterPowerFrame.gameObject.SetActive(false);
                syuzokuText.text = "コネクトカード";
                //powerNumber.SetNumber(data.power);
                break;
            case CardType.Intercept:
                // フレーム差し替え
                if (cardObjectManager)
                    if (cardFrameRenderer.materials[0].GetTexture("_MainTex") != cardObjectManager.eventFrame)
                        cardFrameRenderer.materials[0].SetTexture("_MainTex", cardObjectManager.eventFrame);
                fighterPowerFrame.gameObject.SetActive(false);
                syuzokuText.text = "インターセプトカード";
                //powerNumber.SetNumber(data.power);
                break;
        }
    }

    public void SetPower(int power)
    {
        // パワーフレームのパワー設定
        powerNumber.SetNumber(power);
    }

    // 演出込み　出せるか出せないか
    public void SetNotSelectFlag(bool value)
    {
        notSelectFlag = value;

        // 選択できないなら色を若干落とす
        if(notSelectFlag)
        {
            cardImageRenderer.materials[0].color = new Color(0.5f, 0.5f, 0.5f);
            cardFrameRenderer.materials[0].color = new Color(0.5f, 0.5f, 0.5f);
   
        }
        else
        {
 

            cardImageRenderer.materials[0].color = new Color(1, 1, 1);
            cardFrameRenderer.materials[0].color = new Color(1, 1, 1);
        }
    }

    public void SetUraomote(bool value)
    {
        // 表フラグがtrueならパワーフレームとか名前が出る。
        // falseならパワーフレームが非表示になる
        uraomoteFlag = value;
        if (value)
        {
            if(/*cardData.cardType != CardType.Support*/!cardData.isEventCard()) fighterPowerFrame.gameObject.SetActive(true);
            canvas.gameObject.SetActive(true);
        }
        else
        {
            fighterPowerFrame.gameObject.SetActive(false);
            canvas.gameObject.SetActive(false);
        }
    }

    public void Open()
    {
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.Open.GetInstance());
    }

    public void Draw(int handNumber, int numHand)
    {
        // デッキにいる位置を保存
        startPosition = cacheTransform.localPosition;
        startAngle = cacheTransform.localEulerAngles;

        // 手札の位置
        cardObjectManager.MakeHandTransform(handNumber, numHand, this);
        SetOrder(handNumber);
        nextPosition = cacheTransform.localPosition;
        nextAngle = cacheTransform.localEulerAngles;
        //// 時間指定
        //CardObjectState.Draw.GetInstance().endTime = 0.75f;
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.Draw.GetInstance());
    }

    // 宝箱とか見せる用の位置に移動するドロー
    public void ShowDraw(float endTime, bool urakarahajimaru)
    {
        Debug.Log(cardData.cardName + "が見せる用ドロー");

        // 表にする
        SetUraomote(true);

        // 選択不可能になっている手札を考慮して、戻す
        SetNotSelectFlag(false);

        // デッキにいる位置を保存
        startPosition = cacheTransform.localPosition;
        startAngle = cacheTransform.localEulerAngles;
        if(urakarahajimaru)startAngle.z = 180;

        var showDrawTransform = GameObject.Find("Main Camera/ShowDrawTransform").transform;
        Debug.Assert(showDrawTransform, "メインカメラの名前変わってるかも");

        // 最前面
        SetOrder(114);

        // 座標指定
        nextPosition = showDrawTransform.position;
        nextAngle = showDrawTransform.eulerAngles;
        //// 時間指定
        //CardObjectState.Draw.GetInstance().endTime = endTime;
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.Draw.GetInstance());
    }

    public void SetSupport()
    {
        // デッキにいる位置を保存
        startPosition = cacheTransform.localPosition;
        startAngle = cacheTransform.localEulerAngles;

        var showDrawTransform = GameObject.Find("Main Camera/ShowDrawTransform").transform;
        Debug.Assert(showDrawTransform, "メインカメラの名前変わってるかも");

        // 最前面
        SetOrder(114);

        // 座標指定
        nextPosition = showDrawTransform.position;
        nextAngle = showDrawTransform.eulerAngles;
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.SetSupport.GetInstance());
    }

    public void Marigan(Vector3 handPosition, Vector3 handAngle)
    {
        // デッキ→手札への補間用
        startPosition = handPosition;
        startAngle = handAngle;
        nextPosition = cacheTransform.localPosition;
        nextAngle = cacheTransform.localEulerAngles;
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.Marigan.GetInstance());
    }

    public void FieldSet(Vector3 fieldPosition, Vector3 angle)
    {
        // 開始位置設定
        startPosition = nextPosition + new Vector3(Mathf.Sin(angle.y * Mathf.Deg2Rad), 0, Mathf.Cos(angle.y * Mathf.Deg2Rad)) * 50;
        //startPosition = cacheTransform.localPosition;
        nextPosition = fieldPosition;
        startAngle = nextAngle = angle;

        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.Set.GetInstance());
    }

    // 攻撃モーション発動(勝ち)
    public void Attack()
    {
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.Attack.GetInstance());
    }

    public void Lose()
    {
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.Lose.GetInstance());
    }

    // 墓地に移動して消える
    public void MoveToCemetery(/*int numCemetery*/)
    {
        // 描画順
        SetOrder(cardObjectManager.GetNumCemetery());

        startPosition = cacheTransform.localPosition;
        startAngle = cacheTransform.localEulerAngles;
        nextPosition = cardObjectManager.cemeteryPosition;
        nextPosition.y += cardObjectManager.GetNumCemetery() * 0.25f;
        nextAngle = cacheTransform.localEulerAngles;
        // 裏にする
        SetUraomote(false);
        nextAngle.x = 0;
        nextAngle.z = 180;
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.MoveToCemetery.GetInstance());
    }

    public void Expulsion()
    {
        startPosition = cacheTransform.localPosition;
        startAngle = cacheTransform.localEulerAngles;
        nextPosition = cardObjectManager.cemeteryPosition;
        nextPosition.x *= 100;
        nextAngle = cacheTransform.localEulerAngles;
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.MoveToCemetery.GetInstance());
    }

    public void ChangeState(BaseEntityState<Card> newState)
    {
        stateMachine.ChangeState(newState);
    }

    public bool isInMovingState() { return (!stateMachine.isInState(CardObjectState.None.GetInstance())); }

    // カードエフェクト　カード使用可能
    public void ActiveUseCard()
    {
        if (isCardFrameEffect == false)
        {
            isCardFrameEffect = true;

            cardFrameEffect.gameObject.SetActive(true);
            cardFrameEffect.Action();
            cardFrameEffect2.gameObject.SetActive(true);
            cardFrameEffect2.Action(0.75f);
        }

    }

    // カードエフェクト　止める
    public void StopUseCard()
    {
        isCardFrameEffect = false;
            cardFrameEffect.Stop();
            cardFrameEffect.gameObject.SetActive(false);
            cardFrameEffect2.Stop();
            cardFrameEffect2.gameObject.SetActive(false);
        
    }

    // カードを出す
    //public void SendCard()
    //{
    //    // カードの位置をセット位置にする
    //    cashTransform.localPosition = strikerSetPosition;

    //    isSet = true;
    //}
}
