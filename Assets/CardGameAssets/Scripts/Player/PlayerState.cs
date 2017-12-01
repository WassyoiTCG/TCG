﻿using System.Collections;
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
        { }

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
        { }

        public override void Execute(Player player)
        { }

        public override void Exit(Player player)
        {
            player.playerManager.uiManager.DisAppearWaitYouUI();
        }

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
            player.deckManager.TurnEnd();
            // UI更新
            player.playerManager.uiManager.UpdateDeckUI(player.deckManager, player.isMyPlayer);
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