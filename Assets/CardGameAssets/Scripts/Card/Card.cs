using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Canvas canvas;

    //public Image cardFrameImage;
    //public GameObject fighterPowerFrame;
    public SpriteRenderer fighterPowerFrame;
    //public Text powerText;
    public Number powerNumber;
    public Text cardNameText;
    public Text syuzokuText;
    //public Image cardImage;
    public MeshRenderer cardFrameRenderer;
    public MeshRenderer cardSleeveRenderer;
    public MeshRenderer cardImageRenderer;

    public bool isMyPlayerSide;
    public bool isSetField = false;
    public bool uraomoteFlag;               // 裏表

    //public Vector3 handPosition;    // 手札の座標保存用

    // ステートマシンからのアクセス用
    public float timer;
    public Vector3 startPosition;
    public Vector3 startAngle;
    public Vector3 nextPosition;    // なんか補間とかで使う
    public Vector3 nextAngle;

    public Transform cacheTransform;

    public CardData cardData { get; private set; }

    public BaseEntityStateMachine<Card> stateMachine { get; private set; }

    public CardObjectState.OpenCardInfo openCardInfo = new CardObjectState.OpenCardInfo();

    void Awake()
    {
        //canvas = GetComponent<Canvas>();
        cacheTransform = transform;

        // ステート初期化
        stateMachine = new BaseEntityStateMachine<Card>(this);
        stateMachine.ChangeState(CardObjectState.None.GetInstance());
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
    }

    public void SetCardData(CardData data)
    {
        cardData = data;

        // カード名
        cardNameText.text = data.cardName;
        // カード画像
        //cardSprite.sprite = data.image;
        cardImageRenderer.materials[0].SetTexture("_MainTex", data.image.texture); 

        switch (data.cardType)
        {
            case CardType.Fighter:
            case CardType.AbilityFighter:
                {
                    //fighterPowerFrame.SetActive(true);

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
                //fighterPowerFrame.SetActive(false);
                powerNumber.SetNumber(0);
                break;

            //case CardType.Event:
            //    //fighterPowerFrame.SetActive(false);
            //    powerNumber.SetNumber(data.power);
            //    break;
        }
    }

    public void SetUraomote(bool omote)
    {
        // 表フラグがtrueならパワーフレームとか名前が出る。
        // falseならパワーフレームが非表示になる
        uraomoteFlag = omote;
        if (omote)
        {
            fighterPowerFrame.gameObject.SetActive(true);
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

    public void Draw(Vector3 deckPosition, Vector3 deckAngle)
    {
        // デッキ→手札への補間用
        startPosition = deckPosition;
        startAngle = deckAngle;
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.Draw.GetInstance());
    }

    public void Marigan(Vector3 handPosition, Vector3 handAngle)
    {
        // デッキ→手札への補間用
        startPosition = handPosition;
        startAngle = handAngle;
        // ステートてぇんじ
        stateMachine.ChangeState(CardObjectState.Marigan.GetInstance());
    }

    public void ChangeState(BaseEntityState<Card> newState)
    {
        stateMachine.ChangeState(newState);
    }

    // カードを出す
    //public void SendCard()
    //{
    //    // カードの位置をセット位置にする
    //    cashTransform.localPosition = strikerSetPosition;

    //    isSet = true;
    //}
}
