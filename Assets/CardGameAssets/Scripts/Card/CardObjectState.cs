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
        {
            card.animator.Play("Idle");
        }

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

    // 線形補間補間で移動
    public class MoveLerp : BaseEntityState<Card>
    {
        // Singleton.
        static MoveLerp instance;
        public static MoveLerp GetInstance() { if (instance == null) { instance = new MoveLerp(); } return instance; }

        readonly float endTime = 0.5f;

        public override void Enter(Card card)
        {
            card.animator.Play("Idle");

            card.timer = 0;

            card.cacheTransform.localPosition = card.startPosition;
            card.cacheTransform.localEulerAngles = card.startAngle;
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
            card.animator.Play("Idle");

            card.timer = 0;

            //card.nextPosition = card.cacheTransform.localPosition;
            //card.nextAngle = card.cacheTransform.localEulerAngles;
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

            // 半分過ぎたらとりあえずて感じで(相手サイドのときは表にする必要なし)
            if (card.isMyPlayerSide) if (rate > 0.5f) card.SetUraomote(true);
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
            card.animator.Play("Idle");

            card.timer = 0;

            //card.nextPosition = card.cacheTransform.localPosition;
            //card.nextAngle = card.cacheTransform.localEulerAngles;
            card.cacheTransform.localPosition = card.startPosition;
            card.cacheTransform.localEulerAngles = card.startAngle;

            // 大丈夫だと思うが一応裏状態にしとく
            //card.SetUraomote(false);
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
            // 緩急
            rate = Mathf.Pow(rate, 0.15f);

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

    // 見せる用のドローの動き
    public class ShowDraw : BaseEntityState<Card>
    {
        static ShowDraw instance;
        public static ShowDraw GetInstance() { if (instance == null) { instance = new ShowDraw(); } return instance; }

        readonly float endTime = 1.0f;

        public override void Enter(Card card)
        {
            card.animator.Play("Idle");

            card.timer = 0;

            //card.nextPosition = card.cacheTransform.localPosition;
            //card.nextAngle = card.cacheTransform.localEulerAngles;
            card.cacheTransform.localPosition = card.startPosition;
            card.cacheTransform.localEulerAngles = card.startAngle;

            // 大丈夫だと思うが一応裏状態にしとく
            //card.SetUraomote(false);
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
            // 緩急
            rate = Mathf.Pow(rate, 0.15f);

            // 座標
            card.cacheTransform.localPosition = oulMath.LerpVector(card.startPosition, card.nextPosition, rate);

            // アングルも変える
            var angle = Vector3.zero;
            angle.x = Mathf.LerpAngle(card.startAngle.x, card.nextAngle.x, rate);
            angle.y = Mathf.LerpAngle(card.startAngle.y, card.nextAngle.y, rate);
            angle.z = Mathf.LerpAngle(card.startAngle.z, card.nextAngle.z, rate);
            card.cacheTransform.localEulerAngles = angle;

            // 半分過ぎたらとりあえずて感じで(相手サイドのときは表にする必要なし)
            if (card.isMyPlayerSide) if (rate > 0.5f) card.SetUraomote(true);
        }
        public override void Exit(Card card)
        {
        }

        public override bool OnMessage(Card card, MessageInfo message)
        {
            return false;
        }
    }

    // フィールドにセットするときにいるステート
    public class Set : BaseEntityState<Card>
    {
        static Set instance;
        public static Set GetInstance() { if (instance == null) { instance = new Set(); } return instance; }

        //readonly float moveDist = 50;
        readonly float moveTime = 0.5f;

        public override void Enter(Card card)
        {
            card.animator.Play("Idle");

            // 裏にする
            //var angle = card.cacheTransform.localEulerAngles.y * Mathf.Deg2Rad;
            card.cacheTransform.localPosition = card.startPosition;
            card.cacheTransform.localEulerAngles = card.startAngle;

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

    // サポートセット時
    public class SetSupport : BaseEntityState<Card>
    {
        static SetSupport instance;
        public static SetSupport GetInstance() { if (instance == null) { instance = new SetSupport(); } return instance; }

        readonly float endTime = 0.5f;

        public override void Enter(Card card)
        {
            card.timer = 0;

            card.cacheTransform.localPosition = card.startPosition;
            card.cacheTransform.localEulerAngles = card.startAngle;

            Debug.Log("カードくんサポートカードステートEnter");
        }

        public override void Execute(Card card)
        {
            if ((card.timer += Time.deltaTime) > endTime)
            {
                card.timer = endTime;

                // ★移動終わってカードの効果発動
                var abilityManager = GameObject.Find("GameMain/AbilityManager").GetComponent<CardAbilityManager>();

                var abilityes = card.cardData.supportCard.abilityDatas;
                foreach (CardAbilityData ability in abilityes)
                {
                    abilityManager.PushAbility(ability, card.isMyPlayerSide);
                }
                // ステートチェンジ
                card.MoveToCemetery();
                return;
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

            // 半分過ぎたらとりあえずて感じで
            if (rate > 0.5f) card.SetUraomote(true);
        }

        public override void Exit(Card card)
        {
            Debug.Log("カードくんサポートカードステートExit");
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

        readonly float endTime = 0.5f;

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

    // 攻撃モーション
    public class Attack : BaseEntityState<Card>
    {
        // Singleton.
        static Attack instance;
        public static Attack GetInstance() { if (instance == null) { instance = new Attack(); } return instance; }

        public override void Enter(Card card)
        {
            card.timer = 0;

            // アタックモーションセット
            //card.animator.SetBool("AttackFlag", true);
            card.animator.Play("Attack");
        }

        public override void Execute(Card card)
        {
            // モーション終了判定
            if(card.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                card.ChangeState(None.GetInstance());
            }
        }

        public override void Exit(Card card)
        {
            // 攻撃モーション解除
            //card.animator.SetBool("AttackFlag", false);
        }

        public override bool OnMessage(Card card, MessageInfo message)
        {
            return false;
        }
    }

    // 負けるモーション
    public class Lose : BaseEntityState<Card>
    {
        // Singleton.
        static Lose instance;
        public static Lose GetInstance() { if (instance == null) { instance = new Lose(); } return instance; }

        public override void Enter(Card card)
        {
            card.timer = 0;

            // 負けるモーションセット
            //card.animator.SetBool("DamageFlag", true);
            card.animator.Play("Damage");
        }

        public override void Execute(Card card)
        {
            // 負けてるモーション中
            //if(card.animator.GetBool("DamageFlag"))
            {
                // モーション終了判定
                if (card.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    card.ChangeState(None.GetInstance());
                }
                return;
            }

            //// ぶつかった瞬間フレーム
            //if (card.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            //{
            //    // 攻撃フラグ解除
            //    card.animator.SetBool("AttackFlag", false);
            //    // 負けるモーション発動
            //    card.animator.SetBool("DamageFlag", true);
            //}
        }

        public override void Exit(Card card)
        {
            // 負けるモーション解除
            //card.animator.SetBool("DamageFlag", false);
        }

        public override bool OnMessage(Card card, MessageInfo message)
        {
            return false;
        }
    }

    // 墓地に行く
    public class MoveToCemetery : BaseEntityState<Card>
    {
        // Singleton.
        static MoveToCemetery instance;
        public static MoveToCemetery GetInstance() { if (instance == null) { instance = new MoveToCemetery(); } return instance; }

        public override void Enter(Card card)
        {
            card.animator.Play("Idle");

            card.timer = 0;

            card.cacheTransform.localPosition = card.startPosition;
            card.cacheTransform.localEulerAngles = card.startAngle;
        }

        public override void Execute(Card card)
        {
            var position = card.cacheTransform.localPosition;
            position = oulMath.LerpVector(position, card.nextPosition, 0.3f);
            card.cacheTransform.localPosition = position;

            // アングルも変える
            var angle = card.cacheTransform.localEulerAngles;
            angle.z = Mathf.LerpAngle(angle.z, card.nextAngle.z, 0.3f);
            card.cacheTransform.localEulerAngles = angle;

            // ほぼ終点まで移動したら
            if (Vector3.SqrMagnitude(position - card.nextPosition) < 0.1f)
            {
                card.cacheTransform.localPosition = card.nextPosition;
                card.cacheTransform.localEulerAngles = card.nextAngle;

                // ステートチェンジ
                card.ChangeState(None.GetInstance());

                // 墓地に追加する
                //card.cardObjectManager.AddCemetery(card);

                // 非表示
                //card.gameObject.SetActive(false);
            }
        }

        public override void Exit(Card card)
        { }

        public override bool OnMessage(Card card, MessageInfo message)
        {
            return false;
        }
    }

}
