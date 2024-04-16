using Achievements.Handlers;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.ServerEvent;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Interfaces;
using Exiled.Events;
using Exiled.Events.EventArgs.Player;
using InventorySystem;
using InventorySystem.Items.Pickups;
using MEC;
using Scp914;
using Scp914.Processors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TestingPlugin
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class sp : ICommand
    {

        public string Command { get; } = "sp914";


        public string[] Aliases { get; } = new string[]
        {
            "sp914"
        };


        public string Description { get; } = "生成914指令";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(arguments.At(0));

            if (player == null || arguments.At(0) == "")
            {
                response = "No text ";
                return false;
            }
            else
            {
                var a = Player.Get(Plugin.M914ID);
                if (a != player)
                {
                    if (a == null)
                    {
                        Plugin.plugin.Spawn(player);
                    }
                    else
                    {
                        a.ClearInventory();
                        a.Kill("管理员强制处死");
                        Plugin.plugin.Spawn(player);
                    }
                }
                else
                {
                    response = "生成失败你已经是914了"; return false;
                }
                response = "生成成功";
                return true;
            }

            //response = "Sent successfully";
            //return true;
        }
    }
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("冷却四件")]
        public int Luck { get; set; } = 60;
        [Description("刷新几率")]
        public int Luck1 { get; set; } = 40;
        [Description("最大血量")]
        public int Health { get; set; } = 150;
        [Description("多少人时会刷新")]
        public int People { get; set; } = 10;
        [Description("m914开局默认给的物品")]
        public List<ItemType> itemTypes { get; set; } = new List<ItemType>() { };

    }
    public class Plugin : Plugin<Config>
    {
        public override string Author => "YF-OFFICE";
        public override Version Version => new Version(1, 0, 0);
        public override string Name => "M-914";
        public static Plugin plugin;
        public static int M914ID = -1;
        public int cool = -1;
        public bool Use = true;
        public CoroutineHandle handle { get; set; }
        public Scp914KnobSetting Scp914Mode { get; set; } = Scp914KnobSetting.OneToOne;
        public override void OnEnabled()
        {
            plugin = this;
            Exiled.Events.Handlers.Server.WaitingForPlayers += this.RoundEnding;
            Exiled.Events.Handlers.Server.RoundStarted += this.RoundStarted;
            Exiled.Events.Handlers.Player.DroppingItem += this.Pick;
            Exiled.Events.Handlers.Player.DroppedItem += this.Pick1;
            //Exiled.Events.Handlers.Player.Hurting += this.Hurt;
            Exiled.Events.Handlers.Player.Died += this.Died;

            Log.Info("加载插件中");
            base.OnEnabled();
        }
        public override void OnDisabled()
        {

            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.RoundEnding;
            Exiled.Events.Handlers.Server.RoundStarted -= this.RoundStarted;
            //Exiled.Events.Handlers.Player.InteractingDoor -= this.Indoor;
            //Exiled.Events.Handlers.Player.Hurting -= this.Hurt;
            Exiled.Events.Handlers.Player.Died -= this.Died;
            plugin = null;
            Log.Info("插件关闭了");
            base.OnDisabled();
        }
        public static List<Scp914KnobSetting> scp914KnobSettings = new List<Scp914KnobSetting>() { Scp914KnobSetting.Rough, Scp914KnobSetting.Coarse, Scp914KnobSetting.OneToOne, Scp914KnobSetting.Fine, Scp914KnobSetting.VeryFine };
        public static string GetChine(Scp914KnobSetting scp)
        {
            string test = "";
            switch (scp)
            {
                case Scp914KnobSetting.Rough:
                    test = "超粗加工";
                    break;
                case Scp914KnobSetting.Coarse:
                    test = "粗加工";
                    break;
                case Scp914KnobSetting.OneToOne:
                    test = "一比一";
                    break;
                case Scp914KnobSetting.Fine:
                    test = "精加工";
                    break;
                case Scp914KnobSetting.VeryFine:
                    test = "超精加工";
                    break;
                default:
                    test = "";
                    break;
            }
            return test;
        }
        public void Spawn(Player player)
        {
            if (player.IsAlive)
            {
                M914ID = player.Id;
                Use = true;
                cool = Config.Luck;
                player.MaxHealth = Config.Health;
                player.Health = player.MaxHealth;
                player.CustomInfo = "M-914";
                player.ClearInventory();
                player.AddItem(Config.itemTypes);
                player.AddItem(ItemType.Flashlight);
                player.ClearBroadcasts();
                //player.Broadcast(8, $"你是M-914 你的技能是: 丢下手电筒开启随机加工模式技能(20s)(60sCD) 捡起的物品将被随机模式加工 祝你玩得愉快");
                player.ShowHint("<align=center><color=red><size=100>SCP-M914</size></color>\n\n\n\n\n<size=35><color=#00FF00>你可以丢下手电筒开启随机加工模式技能(20s)(60sCD) 捡起的物品将被随机模式加工</color></size></align>\n\n\n", 15f);
            }
            else
            {
                player.Role.Set(PlayerRoles.RoleTypeId.ClassD);
                M914ID = player.Id;
                Use = true;
                cool = Config.Luck;
                player.MaxHealth = Config.Health;
                player.Health = player.MaxHealth;
                player.CustomInfo = "M-914";
                player.ClearInventory();
                player.AddItem(Config.itemTypes);
                player.AddItem(ItemType.Flashlight);
                player.ClearBroadcasts();
                //player.Broadcast(8, $"你是M-914 你的技能是: 丢下手电筒开启随机加工模式技能(20s)(60sCD) 捡起的物品将被随机模式加工 祝你玩得愉快");
                player.ShowHint("<align=center><color=red><size=100>SCP-M914</size></color>\n\n\n<size=35><color=#00FF00>你可以丢下手电筒开启随机加工模式技能(20s)(60sCD) 捡起的物品将被随机模式加工</color></size></align>\n\n\n", 15f);
            }
        }
        public void RoundStarted()
        {
            if (Player.List.Count() >= this.Config.People)
            {
                int o = new System.Random().Next(0, 100);
                if (o <= Config.Luck1)
                {
                    Timing.CallDelayed(3f, () =>
                    {
                        var player = Player.Get(PlayerRoles.RoleTypeId.ClassD).GetRandomValue();
                        Spawn(player);

                    });
                }
            }
        }
        public IEnumerator<float> Coll()
        {
            while (true)
            {
                if (Use == false)
                {
                    if (cool > 0)
                    {
                        cool--;
                        if (cool <= Config.Luck - 20)
                        {
                            Player.Get(M914ID).CustomInfo = "SCP-M914-CD中";
                        }
                    }
                    else if (cool == 0)
                    {
                        Player.Get(M914ID).CustomInfo = "SCP-M914-冷却完成";
                        Use = true;
                        cool = Config.Luck;
                    }
                    else if (cool < 0)
                    {
                        cool = Config.Luck;
                    }
                }


                yield return Timing.WaitForSeconds(1);
            }
        }
        public void Pick(DroppingItemEventArgs ev)
        {
            if (ev.Player.Id == M914ID)
            {
                if (ev.Item.Type == ItemType.Flashlight)
                {
                    ev.IsAllowed = false;
                    if (Use)
                    {

                        Scp914Mode = scp914KnobSettings.RandomItem();
                        ev.Player.ShowHint($"你成功使用技能 加工模式为:{GetChine(Scp914Mode)} 20s内你可以扔下任何物品来进行加工", 3f);
                        ev.Player.Broadcast(5, $"你成功使用技能 加工模式为:{GetChine(Scp914Mode)} 20s内你可以扔下任何物品来进行加工");
                        ev.Player.CustomInfo = $"SCP-M914-{GetChine(Scp914Mode)}";
                        cool = Config.Luck;
                        Use = false;
                    }
                    else
                    {

                        ev.Player.ShowHint($"技能冷却中还有{cool}秒", 3f);
                    }
                }
            }

        }
        public void Pick1(DroppedItemEventArgs ev)
        {
            if (ev.Player.Id == M914ID)
            {
                if (Use == false && cool >= Config.Luck - 20)
                {
                    //Scp914ItemProcessor a = new Scp914ItemProcessor()
                    ev.Pickup.Type.GetItemBase().TryGetComponent<Scp914ItemProcessor>(out var component);
                    component.OnPickupUpgraded(Scp914Mode, ev.Pickup.Base, ev.Pickup.Position);
                    //ev.Pickup.Base.TryGetComponent<Scp914ItemProcessor>(out var ao);
                    //ao.OnPickupUpgraded(Scp914Mode, ev.Pickup.Base, ev.Pickup.Position + Vector3.up);
                    /*.ProcessPickup(ev.Pickup.Base,false,Vector3.up,Scp914Mode);*/
                    ev.Player.ShowHint("d你的物品成功升级");
                }
            }

        }
        public void Died(DiedEventArgs ev)
        {
            if (ev.Player.Id == M914ID)
            {
                Map.Broadcast(7, $"[设施消息]\nM-914已被重新收容");
                M914ID = -1;
                Use = true;
                cool = Config.Luck;
            }


        }
        public void RoundEnding()
        {
            M914ID = -1;
            Use = true;
            cool = Config.Luck;
            Log.Info("M914数据已重置");
            if (!handle.IsRunning)
            {
                handle = Timing.RunCoroutine(Coll());
            }
        }

    }
}
