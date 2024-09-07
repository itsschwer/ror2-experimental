using HarmonyLib;

namespace SprintingOnTheScoreboard.Harmony
{
    [HarmonyPatch]
    internal static class HarmonyPatches
    {
        [HarmonyPostfix, HarmonyPatch(typeof(RoR2.PlayerCharacterMasterController), nameof(RoR2.PlayerCharacterMasterController.CanSendBodyInput))]
        private static void PlayerCharacterMasterController_CanSendBodyInput(bool __result, ref bool onlyAllowMovement)
        {
            if (!__result) return;
            onlyAllowMovement = false;
        }
#if false
        [HarmonyPostfix, HarmonyPatch(typeof(RoR2.CharacterBody), nameof(RoR2.CharacterBody.OnSprintStop))]
        private static void CharacterBody_OnSprintStop()
        {
            Plugin.Logger.LogWarning(new System.Diagnostics.StackTrace(true).ToString());
        }
#endif
    }
}
