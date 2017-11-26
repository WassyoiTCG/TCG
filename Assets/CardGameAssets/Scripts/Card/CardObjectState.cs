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
    }

    // マリガンの動き
    public class Marigan : BaseEntityState<Card>
    {
        static Marigan instance;
        public static Marigan GetInstance() { if (instance == null) { instance = new Marigan(); } return instance; }

        readonly float endTime = 0.75f;

        public override void Enter(Card card)
        {
            card.timer = 0;

            card.nextPosition = card.cacheTransform.localPosition;
            card.nextAngle = card.cacheTransform.localEulerAngles;
            card.cacheTransform.localPosition = card.startPosition;
            card.cacheTransform.localEulerAngles = card.startAngle;

            // 大丈夫だと思うが一応表状態にしとく
            card.SetUraomote(true);
        }

        public override void Execute(Card card)
        {
            if ((card.timer += Time.deltaTime) > endTime)
            {
                card.timer = endTime;

                // ステートチェンジ
                card.ChangeState(None.GetInstance());
            }

            var rate = card.timer / endTime;

            // 座標
            card.cacheTransform.localPosition = oulMath.LerpVector(card.startPosition, card.nextPosition, rate);

            // アングルも変える
            var angle = Vector3.zero;
            angle.x = Mathf.LerpAngle(card.startAngle.x, card.nextAngle.x, rate);
            angle.y = Mathf.LerpAngle(card.startAngle.y, card.nextAngle.y, rate);
            angle.z = Mathf.LerpAngle(card.startAngle.z, card.nextAngle.z, rate);
            card.cacheTransform.localEulerAngles = angle;

            if (rate > 0.5f) card.SetUraomote(false);
        }
        public override void Exit(Card card)
        {
        }

        public override bool OnMessage(Card card, MessageInfo message)
        {
            return false;
        }
    }

    // ドローの動き
    public class Draw : BaseEntityState<Card>
    {
        static Draw instance;
        public static Draw GetInstance() { if (instance == null) { instance = new Draw(); } return instance; }

        readonly float endTime = 0.75f;

        public override void Enter(Card card)
        {
            card.timer = 0;

            card.nextPosition = card.cacheTransform.localPosition;
            card.nextAngle = card.cacheTransform.localEulerAngles;
            card.cacheTransform.localPosition = card.startPosition;
            card.cacheTransform.localEulerAngles = card.startAngle;

            // 大丈夫だと思うが一応裏状態にしとく
            card.SetUraomote(false);
        }

        public override void Execute(Card card)
        {
            if ((card.timer += Time.deltaTime) > endTime)
            {
                card.timer = endTime;

                // ステートチェンジ
                card.ChangeState(None.GetInstance());
            }

            var rate = card.timer / endTime;

            // 座標
            card.cacheTransform.localPosition = oulMath.LerpVector(card.startPosition, card.nextPosition, rate);

            // アングルも変える
            var angle = Vector3.zero;
            angle.x = Mathf.LerpAngle(card.startAngle.x, card.nextAngle.x, rate);
            angle.y = Mathf.LerpAngle(card.startAngle.y, card.nextAngle.y, rate);
            angle.z = Mathf.LerpAngle(card.startAngle.z, card.nextAngle.z, rate);
            card.cacheTransform.localEulerAngles = angle;

            // 半分過ぎたらとりあえずて感じで(相手サイドのときは表にする必要なし)
            if(card.isMyPlayerSide) if(rate > 0.5f)card.SetUraomote(true);
        }
        public override void Exit(Card card)
        {
        }

        public override bool OnMessage(Card card, MessageInfo message)
        {
            return false;
        }
    }

    // セットするときの
    public class Set : BaseEntityState<Card>
    {
        static Set instance;
        public static Set GetInstance() { if (instance == null) { instance = new Set(); } return instance; }

        readonly float moveDist = 50;
        readonly float moveTime = 0.5f;

        public override void Enter(Card card)
        {
            // 裏にする
            card.SetUraomote(false);
            var angle = card.cacheTransform.localEulerAngles.y * Mathf.Deg2Rad;
            card.startPosition = card.nextPosition + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * moveDist;
            card.cacheTransform.localPosition = card.startPosition;

            // まだセットしてない
            card.isSetField = false;

            // 時間初期化
            card.timer = 0;
        }

        public override void Execute(Card card)
        {
            if((card.timer += Time.deltaTime) > moveTime)
            {
                card.timer = moveTime;

                // セットしテルグマ
                card.isSetField = true;

                // ステートチェンジ
                card.ChangeState(None.GetInstance());
            }

            // 補間
            var rate = card.timer / moveTime;
            rate = Mathf.Pow(rate, 0.01f);
            card.cacheTransform.localPosition = oulMath.LerpVector(card.startPosition, card.nextPosition, rate);
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

        readonly float endTime = 0.75f;

        public override void Enter(Card card)
        {
            // まだセットしてない
            card.isSetField = false;

            card.timer = 0;

            // ベジエの点設定
            if (card.openCardInfo.bezierPoints == null) card.openCardInfo.bezierPoints = new Vector3[4];
            card.openCardInfo.bezierPoints[0] = card.openCardInfo.bezierPoints[3] = card.cacheTransform.localPosition;
            card.openCardInfo.bezierPoints[1] = card.openCardInfo.bezierPoints[0] + new Vector3(-5, 10, 0);
            card.openCardInfo.bezierPoints[2] = card.openCardInfo.bezierPoints[0] + new Vector3(2, 10, 0);

            // 大丈夫だと思うが一応裏状態にしとく
            card.SetUraomote(false);
            var angle = card.cacheTransform.localEulerAngles;
            angle.z = 180;
            card.cacheTransform.localEulerAngles = angle;
            card.startAngle = card.nextAngle = angle;
            card.nextAngle.z = 0;
        }

        public override void Execute(Card card)
        {
            if ((card.timer += Time.deltaTime) > endTime)
            {
                card.timer = endTime;

                // ステートチェンジ
                card.ChangeState(None.GetInstance());
            }

            var rate = card.timer / endTime;

            // ベジエで座標制御
            var position = oulMath.Bezier(card.openCardInfo.bezierPoints, rate);
            card.cacheTransform.localPosition = position;

            // アングルも変える
            var angle = card.startAngle;
            angle.z = Mathf.LerpAngle(card.startAngle.z, card.nextAngle.z, rate);
            card.cacheTransform.localEulerAngles = angle;

            // 半分過ぎたらとりあえずて感じで
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
