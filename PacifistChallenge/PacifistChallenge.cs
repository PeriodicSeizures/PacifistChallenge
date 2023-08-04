using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
//using Jotunn.Utils;

namespace PacifistChallenge
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    //[NetworkCompatibility(CompatibilityLevel.ServerMustHaveMod, VersionStrictness.Minor)]
    internal class PacifistChallenge : BaseUnityPlugin
    {
        // BepInEx' plugin metadata
        public const string PluginGUID = "com.crzi.PacifistChallenge";
        public const string PluginName = "PacifistChallenge";
        public const string PluginVersion = "1.0.0";

        Harmony _harmony;

        //private static int lastMessage = 0;

        private static string[] MESSAGES = new string[] { 
            "nope", 
            "get a job!", 
            "try harder", 
            "just dont do it",
            "just do it (or dont)",
            "why", 
            "too weak",
            "try again! or not...",
            "stop hitting me",
            "stop",
            "just stop",
            "I said.. nevermind",
            "even I hit harder than that...",
        };

        private void Awake()
        {
            Game.isModded = true;

            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
        }

        private void Destroy()
        {
            _harmony?.UnpatchSelf();
        }

        [HarmonyPatch(typeof(Character))]
        class CharacterPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(Character.RPC_Damage))]
            static bool AwakePrefix(ref Character __instance, ref long sender, ref HitData hit)
            {
                // Only allow non-players to attack
                if (!hit.GetAttacker().IsPlayer())
                    return true;

                // Cancel player attack
                Chat.instance.AddInworldText(null, 
                    ZDOMan.GetSessionID(), 
                    hit.m_point, 
                    Talker.Type.Normal, 
                    UserInfo.GetLocalUser(), 
                    MESSAGES[Random.Range(0, MESSAGES.Length)]
                );

                return false;
            }
        }
    }
}