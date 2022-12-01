using BepInEx;
using BepInEx.Logging;
using Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;
using BepInEx.Configuration;
using HarmonyLib;

namespace MoreMoneyForFirewood
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class MoreMoney : BaseUnityPlugin
    {
        public const string pluginGuid = "ForZa.moremoneyforfirewood";
        public const string pluginName = "More Money for Firewood";
        public const string pluginVersion = "0.0.1";

        private ConfigEntry<int> logValueConfig;

        public static int logValue;

        public void Awake()
        {
            logValueConfig = Config.Bind( "Settings",
                                    "logValue",
                                    7,
                                    "The base value of firewood logs. Note that Maple gets a 1.2 and Birch a 0.8 multiplier that gets rounded down. So base value of 14 will get you 14, 16, 11 cash for Pine, Maple, Birch");

            Logger.LogInfo(string.Format("More Money for Firewood version {0} started", pluginVersion));
            Logger.LogInfo(string.Format("Loaded value from config: {0}", logValueConfig.Value));

            logValue = logValueConfig.Value;
        }

        [HarmonyPatch(typeof(BrotherHouse), nameof(BrotherHouse.Receive))]
        public static bool Receive_Patched(Item_FireWood log, BrotherHouse __instance)
        {
            Singleton<ToDoList_Manager>.i.Debut_SellFireWoodToBrother();
            int num = logValue;
            if (log.IntValue1 == 1)
                num = (int)Mathf.Floor((float)num * 0.8f);
            else if (log.IntValue1 == 2)
                num = (int)Mathf.Floor((float)num * 1f);
            else if (log.IntValue1 == 0)
                num = (int)Mathf.Floor((float)num * 1.2f);
            Singleton<Cash_Manager>.i.Event_Received((float)num);
            log.isPlaced = true;
            Singleton<Gameplay>.i.Player.CheckIfHoldingItemToCancel((InteractableItems)log);
            Singleton<SOUND_Manager>.i.PlaySound_PartTempInstalled();
            Object.Destroy((Object)log.rb);
            Object.Destroy((Object)log.itemCollider);
            Object.Destroy((Object)log);

            return false;
        }
    }
}
