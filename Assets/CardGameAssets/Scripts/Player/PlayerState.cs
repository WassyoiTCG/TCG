using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
    public class None : BaseEntityState<Player>
    {
        // Singleton.
        static None instance;
        public static None GetInstance() { if (instance == null) { instance = new None(); } return instance; }

        public override void Enter(Player player)
        {
            player.isStateEnd = true;

            if (player.isMyPlayer)
            {
                // 何も選択できないように
                player.cardObjectManager.ChangeHandNoneSelect();
            }
        }

        public override void Execute(Player player)
        { }

        public override void Exit(Player player)
        { }

        public override bool OnMessage(Player player, MessageInfo message)
        {
            return false;
        }
    }

    public class FirstDraw : BaseEntityState<Player>
    {
        // Singleton.
        static FirstDraw instance;
        public static FirstDraw GetInstance() { if (instance == null) { instance = new FirstDraw(); } return instance; }

        public override void Enter(Player player)
        {
            player.isStateEnd = false;

            player.FirstDraw();
            player.isMarigan = false;

            player.step = 0;
        }

        public override void Execute(Player player)
        {
            if (player.isMarigan)
            {
                switch (player.step)
                {
                    case 0:
                        // 手札→デッキ終了
                        if (!player.cardObjectManager.isInMovingState())
                        {
                            player.step++;

                            // 7毎ドロー
                            player.FirstDraw();
                        }
                        break;

                    case 1:
                        // デッキ→手札終了
                        if (!player.cardObjectManager.isInMovingState())
                        {
                            player.step++;

                            // 最初のドロー終了
                            player.isStateEnd = true;
                        }
                        break;
                }
            }
        }

        public override void Exit(Player player)
        { }

        public override bool OnMessage(Player player, MessageInfo message)
        {
            return false;
        }
    }

    public class Draw : BaseEntityState<Player>
    {
        // Singleton.
        static Draw instance;
        public static Draw GetInstance() { if (instance == null) { instance = new Draw(); } return instance; }

        public override void Enter(Player player)
        {
            player.isStateEnd = false;

            // 1枚ドロー
            player.deckManager.Draw(1);
        }

        public override void Execute(Player player)
        {
            // デッキ→手札終了
            if (!player.cardObjectManager.isInMovingState())
            {
                // 最初のドロー終了
                player.isStateEnd = true;
            }
        }

        public override void Exit(Player player)
        {
            // UI更新
            player.playerManager.uiManager.UpdateDeckUI(player.deckManager, player.isMyPlayer);
        }

        public override bool OnMessage(Player player, MessageInfo message)
        {
            return false;
        }
    }

    public class SetStriker : BaseEntityState<Player>
    {
        // Singleton.
        static SetStriker instance;
        public static SetStriker GetInstance() { if (instance == null) { instance = new SetStriker(); } return instance; }

        public override void Enter(Player player)
        {
            if(player.isMyPlayer)
            {
                // インターセプトを選択不可能に
                player.cardObjectManager.ChangeHandSetStrikerMode(player);
            }
        }

        public override void Execute(Player player)
        {

        }

        public override void Exit(Player player)
        {
            var striker = player.GetFieldStrikerCard();
            if (striker != null)
            {
                player.SetPower(striker.cardData.power);
            }
            else player.SetPower(Player.noSetStrikerPower);

            player.playerManager.uiManager.DisAppearWaitYouUI();

            //// ★リミット処理を解除
            //Player.LimitPowerData data = new Player.LimitPowerData();
            //data.type = Player.LimitPowerType.NoneLimit;
            //player.SetLimitPowerData(data);

            // 情報を閉じる
            if (player.isMyPlayer)
            {
                player.GetComponent<PlayerController>().InfomationOff();
            }
        }

        public override bool OnMessage(Player player, MessageInfo message)
        {
            return false;
        }
    }

    // サポートカードの効果待ちをしているステート
    public  class SupportWait : BaseEntityState<Player>
    {
        // Singleton.
        static SupportWait instance;
        public static SupportWait GetInstance() { if (instance == null) { instance = new SupportWait(); } return instance; }

        public override void Enter(Player player)
        {
            if (player.isMyPlayer)
            {
                // 何も選択できないように
                player.cardObjectManager.ChangeHandNoneSelect();
            }
        }

        public override void Execute(Player player)
        {

        }

        public override void Exit(Player player)
        {

        }

        public override bool OnMessage(Player player, MessageInfo message)
        {
            return false;
        }
    }

    public class SetIntercept : BaseEntityState<Player>
    {
        // Singleton.
        static SetIntercept instance;
        public static SetIntercept GetInstance() { if (instance == null) { instance = new SetIntercept(); } return instance; }

        public override void Enter(Player player)
        {
            player.isStateEnd = false;
            player.isPushedJunbiKanryo = false;
            player.isPushedNextButton= false;

            if (player.isMyPlayer)
            {
                // インターセプトを選択可能に
                player.cardObjectManager.ChangeHandSetInterceptMode(player);
            }
        }

        public override void Execute(Player player)
        { }

        public override void Exit(Player player)
        {
            player.playerManager.uiManager.DisAppearWaitYouUI();

		// 情報を閉じる
            if (player.isMyPlayer)
            {
                player.GetComponent<PlayerController>().InfomationOff();
            }
        }

        public override bool OnMessage(Player player, MessageInfo message)
        {
            return false;
        }
    }

    public class StrikerBeforeBattleAbility : BaseEntityState<Player>
    {
        // Singleton.
        static StrikerBeforeBattleAbility instance;
        public static StrikerBeforeBattleAbility GetInstance() { if (instance == null) { instance = new StrikerBeforeBattleAbility(); } return instance; }

        public override void Enter(Player player)
        {
            // 出したストライカーが効果持ちでなおかつバトル後効果発動なら
            var card = player.GetFieldStrikerCard();
            if (card == null) return;
            if (card.cardData.cardType != CardType.AbilityFighter) return;

            var abilityes = card.cardData.abilityFighterCard.abilityDatas;
            foreach (CardAbilityData ability in abilityes)
            {
                if (ability.abilityTriggerType != AbilityTriggerType.BeforeBattle) continue;

                // 効果の条件を満たしているかどうか(爪痕とかのチェック)
                if (!ability.HatsudouOK(player)) continue;

                // 効果発動!
                GameObject.Find("GameMain/AbilityManager").GetComponent<CardAbilityManager>().PushAbility(ability, player.isMyPlayer);
            }
        }

        public override void Execute(Player player)
        {

        }

        public override void Exit(Player player)
        { }

        public override bool OnMessage(Player player, MessageInfo message)
        {
            return false;
        }
    }


    public class StrikerAfterBattleAbility : BaseEntityState<Player>
    {
        // Singleton.
        static StrikerAfterBattleAbility instance;
        public static StrikerAfterBattleAbility GetInstance() { if (instance == null) { instance = new StrikerAfterBattleAbility(); } return instance; }

        public override void Enter(Player player)
        {
            // 出したストライカーが効果持ちでなおかつバトル後効果発動なら
            var card = player.GetFieldStrikerCard();
            if (card == null) return;
            if (card.cardData.cardType != CardType.AbilityFighter) return;

            var abilityes = card.cardData.abilityFighterCard.abilityDatas;
            foreach (CardAbilityData ability in abilityes)
            {
                if (ability.abilityTriggerType != AbilityTriggerType.AfterBattle) continue;

                // 効果の条件を満たしているかどうか(爪痕とかのチェック)
                if (!ability.HatsudouOK(player)) continue;

                // 効果発動!
                GameObject.Find("GameMain/AbilityManager").GetComponent<CardAbilityManager>().PushAbility(ability, player.isMyPlayer);
            }
        }

        public override void Execute(Player player)
        {
            
        }

        public override void Exit(Player player)
        { }

        public override bool OnMessage(Player player, MessageInfo message)
        {
            return false;
        }
    }

    public class TurnEnd : BaseEntityState<Player>
    {
        // Singleton.
        static TurnEnd instance;
        public static TurnEnd GetInstance() { if (instance == null) { instance = new TurnEnd(); } return instance; }

        public override void Enter(Player player)
        {
            player.isPushedJunbiKanryo = false;
            player.isPushedNextButton　= false;
            player.deckManager.TurnEnd();
            // UI更新
            player.playerManager.uiManager.UpdateDeckUI(player.deckManager, player.isMyPlayer);

            //// ★リミット処理を解除
            //Player.LimitPowerData data = new Player.LimitPowerData();
            //data.type = Player.LimitPowerType.NoneLimit;
            //player.SetLimitPowerData(data);
        }

        public override void Execute(Player player)
        { }

        public override void Exit(Player player)
        { }

        public override bool OnMessage(Player player, MessageInfo message)
        {
            return false;
        }
    }
}