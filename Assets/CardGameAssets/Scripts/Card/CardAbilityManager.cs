using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CardAbilityData
{
    enum State
    {
        Cost,   // コスト(効果条件)処理
        Ability // 効果処理
    }
    State state;

    public Cost.Base                                    cost;        // コスト(効果の条件)委譲クラス

    public int skillNumber;          // 発動する効果の番号(分岐用に作成)
    public int numSkill;             // 効果の個数

    public bool isJoukenOK; // Costを抜けた後にこれがtrueだったら効果を発動する(主にストライカーの爪痕とかに使う)
    public bool endFlag;    // 効果が終了したフラグ

    public bool isMyPlayer;                    // この効果を発動しようとしているプレイヤーが画面的に自分かどうか

    public static UIManager uiManager;
    public static PlayerManager playerManager;
    public static UVEffectManager uvEffectManager_My;
    public static PanelEffectManager panelEffectManager_My;
    public static UVEffectManager uvEffectManager_Cpu;
    public static PanelEffectManager panelEffectManager_Cpu;

    public Player myPlayer, youPlayer;      // 自分と相手のプレイヤーの実体

    public AbilityTriggerType abilityTriggerType;  // 発動トリガー
    public int lifeCost;                           // 支払うライフの値
    public CostType costType;                      // 条件(コスト)
    public int c_value0;
    public int c_value1;
    public int c_value2;
    public int c_value3;

    public const int RefValueFlag = 256;
    public const int HalfHPCostFlag = 256;
    public const int QuatarHPCostFlag = 257;

    // structにするとvarに格納して処理したときに無駄になってしまう
    public class SkillData
    {
        public Skill.Base skill;        // 効果委譲クラス(複数あり)
        public int nextSkillNumber;     // 次に発動する効果(ないなら終了)
        public AbilityType abilityType;                           // 効果のタイプ
        public int s_iValue0;
        public int s_iValue1;
        public int s_iValue2;
        public int s_iValue3;
        public int s_iValue4;
        public int s_iValue5;
        public int s_iValue6;
        public int s_iValue7;
        public int s_iValue8;
        public int s_iValue9;
        public string s_sValue0;
        public string s_sValue1;
        public string s_sValue2;
    }
    public SkillData[] skillDatas;
    public SkillData GetCurrentSkillData() { return skillDatas[skillNumber]; }

    // スコア系、パワー変化系で使う
    //public int value;                   // 変化の値(パワーだったり、ポイントだったり)
    //public Arithmetic arithmetic;       // 足す引くかける割る

    // カード移動系で使う
    //public int cardFrom;
    //public AbilityTarget fromPlayer;
    //public int cardTo;
    //public AbilityTarget toPlayer;

    // 数字指定系の保存等に使う
    public int delvValue0;             
    public int delvValue1;
    public int delvValue2;
    public int delvValue3;
    //private CardAbilityData ability;

    public Player GetPlayerByAbilitiTarget(AbilityTarget target)
    {
        if (target == AbilityTarget.Me)
        {
            return myPlayer;
        }
        else if (target == AbilityTarget.You)
        {
            return youPlayer;
        }
        else
        {
            Debug.LogError("アビリティターゲットで想定されていない値");
            return null;
        }
    }

    public void Action()
    {
        if (!uiManager) uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (!playerManager) playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        if (isMyPlayer)
        {
            myPlayer = playerManager.GetMyPlayer();
            youPlayer = playerManager.GetCPUPlayer();
        }
        else
        {
            myPlayer = playerManager.GetCPUPlayer();
            youPlayer = playerManager.GetMyPlayer();
        }

        skillNumber = 0;
        endFlag = false;
        state = State.Cost;

        // コスト実行
        cost.Enter(this);
    }

    public void Update(CardAbilityManager abilityManager)
    {
        if (endFlag) return;

        switch(state)
        {
            case State.Cost:
                // コスト処理
                if (cost.EndCheck())
                {
                    // 条件を満たしていない場合処理しない
                    if(!isJoukenOK || skillNumber == (int)IDType.NONE)
                    {
                        endFlag = true;
                        return;
                    }

                    // ライフコストを支払う
                    if (lifeCost > 0)
                    {
                        if (lifeCost == HalfHPCostFlag)
                        {
                            lifeCost = uiManager.GetScore(isMyPlayer) / 2;
                            // 切り捨て処理
                            lifeCost = (lifeCost / 10) * 10;
                        }
                        if(lifeCost == QuatarHPCostFlag)
                        {
                            lifeCost = uiManager.GetScore(isMyPlayer) / 4;
                            // 切り捨て処理
                            lifeCost = (lifeCost / 10) * 10;
                        }
                        uiManager.Damage(isMyPlayer, lifeCost);
                    }

                    state = State.Ability;
                    GetCurrentSkillData().skill.Enter(this);
                    return;
                }
                cost.Execute(this);
                break;

            case State.Ability:
                {
                    var currentSkill = GetCurrentSkillData();
                    // 効果処理
                    if (currentSkill.skill.EndCheck())
                    {
                        // 終了処理
                        currentSkill.skill.Exit(this);

                        // 次に発動するスキルがないなら終了
                        if (currentSkill.nextSkillNumber == (int)IDType.NONE)
                        {
                            endFlag = true;
                        }
                        else
                        {
                            Debug.Assert(skillNumber != currentSkill.nextSkillNumber, "発動スキル番号重複");
                            skillNumber = currentSkill.nextSkillNumber;

                            GetCurrentSkillData().skill.Enter(this);
                        }
                        return;
                    }
                    switch (GetCurrentSkillData().skill.Execute(this))
                    {
                        case Skill.Result.EndGame:
                            // 終了処理
                            currentSkill.skill.Exit(this);
                            abilityManager.sceneMain.Finish();
                            endFlag = true;
                            // 空
                            abilityManager.Restart();
                            break;
                    }
                    
                }
                break;
        }
    }

    public bool HandleMessage(MessageInfo message)
    {
        if (endFlag) return false;

        switch (state)
        {
            case State.Cost:
                return cost.OnMessage(this, message);

            case State.Ability:
                return GetCurrentSkillData().skill.OnMessage(this, message);
        }

        return false;
    }

    // 発動プレイヤー設定
    //public void SetMyPlayerFlag(bool isMyPlayer)
    //{
    //    if (!playerManager) playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

    //    this.isMyPlayer = isMyPlayer;
    //    if(isMyPlayer)
    //    {
    //        myPlayer = playerManager.GetMyPlayer();
    //        youPlayer = playerManager.GetCPUPlayer();
    //    }
    //    else
    //    {
    //        myPlayer = playerManager.GetCPUPlayer();
    //        youPlayer = playerManager.GetMyPlayer();
    //    }
    //}

    // 発動してもいいかチェック(主にイベントカード)
    public bool HatsudouOK(Player player)
    {
        if (!uiManager) uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        // [1210] 宝箱にも設定している？
        // ライフがコスト以下だと発動できない
        var life = uiManager.GetScore(isMyPlayer);
        // 割合ダメージ系のコストでないなら
        if (lifeCost != HalfHPCostFlag && lifeCost != QuatarHPCostFlag)
            // 体力がコスト以下なら発動できない
            if (life <= lifeCost) return false;

        return cost.HatsudouCheck(this, player);
    }

    // 参照型はアウトなのでClone関数を自前で実装する
    public CardAbilityData Clone()
    {
        CardAbilityData work = new CardAbilityData();
        work.state = state;
        work.cost = cost;
        work.skillNumber = skillNumber;          // 発動する効果の番号(分岐用に作成)
        work.numSkill = numSkill;             // 効果の個数

        work.isJoukenOK = isJoukenOK; // Costを抜けた後にこれがtrueだったら効果を発動する(主にストライカーの爪痕とかに使う)
        work.endFlag = endFlag;    // 効果が終了したフラグ

        work.isMyPlayer = isMyPlayer;                    // この効果を発動しようとしているプレイヤーが画面的に自分かどうか
        work.myPlayer = myPlayer;
        work.youPlayer = youPlayer;      // 自分と相手のプレイヤーの実体

        work.abilityTriggerType = abilityTriggerType;  // 発動トリガー
        work.lifeCost = lifeCost;                           // 支払うライフの値
        work.costType = costType;                      // 条件(コスト)
        work.c_value0 = c_value0;
        work.c_value1 = c_value1;
        work.c_value2 = c_value2;
        work.c_value3 = c_value3;

        work.skillDatas = skillDatas;

        work.delvValue0 = delvValue0;
        work.delvValue1 = delvValue1;
        work.delvValue2 = delvValue2;
        work.delvValue3 = delvValue3;

        return work;
    }

    public Vector3 GetFieldStrikerPosition()
    {
        Vector3 ret = Vector3.zero;
        if (myPlayer)
        {
            Card strikerCard = myPlayer.cardObjectManager.fieldStrikerCard;
            if (strikerCard)
            {
                ret = strikerCard.cacheTransform.localPosition;

                // 相手と味方でZ値変える
                if (isMyPlayer == true)
                {
                    ret.z -= strikerCard.rootTransform.localPosition.z;
                }
                else
                {
                    ret.z += strikerCard.rootTransform.localPosition.z;
                }
            }
            else Debug.LogWarning("すとらいかーがぬる");
        }
        else Debug.LogWarning("ぷれいやーがぬる");

        return ret;
    }


    public Vector3 GetFieldEventPosition()
    {
        Vector3 ret = Vector3.zero;
        if (myPlayer)
        {
            Card eventCard = myPlayer.cardObjectManager.fieldEventCard;
            if (eventCard)
            {
                ret = eventCard.cacheTransform.localPosition;
            }
            else Debug.LogWarning("ibenntoganuru");
        }
        else Debug.LogWarning("ぷれいやーがぬる");

        return ret;
    }

}

public class CardAbilityManager : MonoBehaviour
{
    Queue<CardAbilityData> abilityQueue = new Queue<CardAbilityData>();     // アビリティタスク
    CardAbilityData excutionAbility;                                        // 現在実行しているアビリティ
    public SceneMain sceneMain;

    Queue<MessageInfo> messageQueue = new Queue<MessageInfo>();

    void Start()
    {
        Restart();
    }

    public void Restart()
    {
        messageQueue.Clear();
        abilityQueue.Clear();
        excutionAbility = null;
    }

    void Update()
    {
        if(excutionAbility != null)
        {
            // 効果が終了するまで処理する
            if(excutionAbility.endFlag)
            {
                excutionAbility = null;
                return;
            }
            excutionAbility.Update(this);
            if(messageQueue.Count > 0)
            {
                excutionAbility.HandleMessage(messageQueue.Dequeue());
            }
        }

        else if(abilityQueue.Count > 0)
        {
            excutionAbility = abilityQueue.Dequeue();
            // 効果実行
            excutionAbility.Action();
            //Debug.Log("効果発動: " + excutionAbility.abilityType.ToString() + ", values: " +
            //    excutionAbility.s_iValue0 + ", " + excutionAbility.s_iValue1 + ", " + excutionAbility.s_iValue2 + ", " + excutionAbility.s_iValue3 + ", " +
            //    excutionAbility.s_iValue4 + ", " + excutionAbility.s_iValue5 + ", " + excutionAbility.s_iValue6 + ", " + excutionAbility.s_iValue7 + ", " +
            //    excutionAbility.s_iValue8 + ", " + excutionAbility.s_iValue9);
        }

        // ここにexcutionAbility.を書いてはいけない
    }

    public void PushAbility(CardAbilityData ability, bool isMyPlayer)
    {
        // ★★★嫌な予感がする…(参照)
        var abilityData = ability.Clone();
        //abilityData = ability;
        abilityData.isMyPlayer = isMyPlayer;
        abilityQueue.Enqueue(abilityData);
    }

    // 全ての効果の処理が終わったかどうかの判定
    public bool isAbilityEnd()
    {
        return (abilityQueue.Count == 0 && excutionAbility == null);
    }

    // メッセージ受信
    public bool HandleMessage(MessageInfo message)
    {
        if (excutionAbility == null)
        {
            messageQueue.Enqueue(message);
            return false;
        }

        return excutionAbility.HandleMessage(message);
    }
}
