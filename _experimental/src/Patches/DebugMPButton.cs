using Experimental.Dumps;
using HarmonyLib;
using RoR2.UI;

namespace Experimental.Patches
{
    [HarmonyPatch]
    public static class UnityEvent
    {
        [HarmonyPostfix, HarmonyPatch(typeof(MPButton), nameof(MPButton.Awake))]
        private static void MPButton_Awake(MPButton __instance)
        {
            __instance.onClick.AddListener(() => {
                Plugin.Logger.LogDebug($"{__instance.name}\n{UnityEventPersistentCalls.Dump(__instance.onClick)}");
            });
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RoR2.VoteController), nameof(RoR2.VoteController.Awake))]
        private static void VoteController_Awake(RoR2.VoteController __instance)
        {
            for (int i = 0; i < __instance.choices.Length; i++) {
                Plugin.Logger.LogDebug($"choice [{i}]\n{UnityEventPersistentCalls.Dump(__instance.choices[i])}");
            }
        }
    }
}
