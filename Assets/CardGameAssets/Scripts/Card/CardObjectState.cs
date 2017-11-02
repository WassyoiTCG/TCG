using System;
using UnityEngine;

namespace CardObjectState
{
    // 動かない状態
    public class None : BaseEntityState<Card>
    {
        // Singleton.
        static None instance;
        public static None GetInstance() { if (instance == null) { instance = new None(); } return instance; }

        public override void Enter(Card card)
        {}

        public override void Execute(Card card)
        {

        }

        public override void Exit(Card card)
        { }

        public override bool OnMessage(Card card, MessageInfo message)
        {
            return false;
        }
    }

    public struct OpenCardInfo
    {
        public Vector3[] bezierPoints;
        public Vector3 startAngle, endAngle;
        public float timer;
    }

    // セットするときの
    public class Set : BaseEntityState<Card>
    {
        static Set instance;
        public static Set GetInstance() { if (instance == null) { instance = new Set(); } return instance; }

        Vector3 startPosition;
        readonly float moveDist = 50;
        readonly float moveTime = 0.5f;
        float timer;

        public override void Enter(Card card)
        {
            // 裏にする
            card.SetUraomote(false);
            var angle = card.cashTransform.localEulerAngles.y * Mathf.Deg2Rad;
            startPosition = card.nextPosition + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * moveDist;
            card.cashTransform.localPosition = startPosition;

            // まだセットしてない
            card.isSetField = false;

            // 時間初期化
            timer = 0;
        }

        public override void Execute(Card card)
        {
            if((timer += Time.deltaTime) > moveTime)
            {
                timer = moveTime;

                // セットしテルグマ
                card.isSetField = true;

                // ステートチェンジ
                card.stateMachine.ChangeState(None.GetInstance());
            }

            // 補間
            var rate = timer / moveTime;
            rate = Mathf.Pow(rate, 0.01f);
            card.cashTransform.localPosition = oulMath.LerpVector(startPosition, card.nextPosition, rate);
        }
        public override void Exit(Card card)
        {
        }

        public override bool OnMessage(Card card, MessageInfo message)
        {
            return false;
        }
    }

    // 裏から表になる
    public class Open : BaseEntityState<Card>
    {
        // Singleton.
        static Open instance;
        public static Open GetInstance() { if (instance == null) { instance = new Open(); } return instance; }

        readonly float limitTime = 0.75f;

        public override void Enter(Card card)
        {
            // まだセットしてない
            card.isSetField = false;

            card.openCardInfo.timer = 0;

            // ベジエの点設定
            if (card.openCardInfo.bezierPoints == null) card.openCardInfo.bezierPoints = new Vector3[4];
            card.openCardInfo.bezierPoints[0] = card.openCardInfo.bezierPoints[3] = card.cashTransform.localPosition;
            card.openCardInfo.bezierPoints[1] = card.openCardInfo.bezierPoints[0] + new Vector3(-5, 10, 0);
            card.openCardInfo.bezierPoints[2] = card.openCardInfo.bezierPoints[0] + new Vector3(2, 10, 0);

            // 大丈夫だと思うが一応裏状態にしとく
            card.SetUraomote(false);
            var angle = card.cashTransform.localEulerAngles;
            angle.z = 180;
            card.cashTransform.localEulerAngles = angle;
            card.openCardInfo.startAngle = card.openCardInfo.endAngle = angle;
            card.openCardInfo.endAngle.z = 0;
        }

        public override void Execute(Card card)
        {
            if ((card.openCardInfo.timer += Time.deltaTime) > limitTime)
            {
                card.openCardInfo.timer = limitTime;

                // ステートチェンジ
                card.stateMachine.ChangeState(None.GetInstance());
            }

            var rate = card.openCardInfo.timer / limitTime;

            // ベジエで座標制御
            var position = oulMath.Bezier(card.openCardInfo.bezierPoints, rate);
            card.cashTransform.localPosition = position;

            // アングルも変える
            var angle = card.openCardInfo.startAngle;
            angle.z = Mathf.LerpAngle(card.openCardInfo.startAngle.z, card.openCardInfo.endAngle.z, rate);
            card.cashTransform.localEulerAngles = angle;

            if (rate > 0.5f) card.SetUraomote(true);
        }

        public override void Exit(Card card)
        { }

        public override bool OnMessage(Card card, MessageInfo message)
        {
            return false;
        }
    }

}
