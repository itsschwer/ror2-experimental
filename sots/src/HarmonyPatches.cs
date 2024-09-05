using HarmonyLib;

namespace SprintingOnTheScoreboard.Harmony
{
    [HarmonyPatch]
    internal static class HarmonyPatches
    {
#if !DEBUG
        [HarmonyPostfix, HarmonyPatch(typeof(RoR2.PlayerCharacterMasterController), nameof(RoR2.PlayerCharacterMasterController.CanSendBodyInput))]
        private static void PlayerCharacterMasterController_CanSendBodyInput(bool __result, ref bool onlyAllowMovement)
        {
            if (!__result) return;
            onlyAllowMovement = false;
        }
#endif
#if false
        [HarmonyPostfix, HarmonyPatch(typeof(RoR2.CharacterBody), nameof(RoR2.CharacterBody.OnSprintStop))]
        private static void CharacterBody_OnSprintStop()
        {
            Plugin.Logger.LogWarning(new System.Diagnostics.StackTrace(true).ToString());
        }
#endif
#if DEBUG
        [HarmonyPostfix, HarmonyPatch(typeof(UnityEngine.Debug), nameof(UnityEngine.Debug.LogWarning), typeof(object))]
        private static void Debug_LogWarning(object message)
        {
            if (message is string msg) {
                if (msg.Contains("Instance not found when handling Command message")) {
                    Plugin.Logger.LogWarning(new System.Diagnostics.StackTrace(true).ToString());
                }
            }
        }
#endif
    }
}
