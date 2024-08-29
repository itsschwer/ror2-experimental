using HarmonyLib;

namespace SprintingOnTheScoreboard.Harmony
{
    [HarmonyPatch]
    internal static class HarmonyPatches
    {
#if DEBUG
        [HarmonyPostfix, HarmonyPatch(typeof(RoR2.CharacterBody), nameof(RoR2.CharacterBody.OnSprintStop))]
        private static void CharacterBody_OnSprintStop()
        {
            Log.Warning(new System.Diagnostics.StackTrace(true).ToString());
        }
#endif
        [HarmonyPostfix, HarmonyPatch(typeof(RoR2.PlayerCharacterMasterController), nameof(RoR2.PlayerCharacterMasterController.CanSendBodyInput))]
        private static void PlayerCharacterMasterController_CanSendBodyInput(out bool onlyAllowMovement)
        {
            onlyAllowMovement = false;
        }
    }
}
