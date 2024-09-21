using HarmonyLib;
using RoR2;

namespace NameChanger.Patches
{
    [HarmonyPatch]
    internal static class HarmonyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NetworkUser), nameof(NetworkUser.GetNetworkPlayerName))]
        private static void NetworkUser_GetNetworkPlayerName(NetworkUser __instance, ref NetworkPlayerName __result)
        {
            bool isClient = LocalUserManager.GetFirstLocalUser()?.currentNetworkUser == __instance;
            if (isClient && !string.IsNullOrWhiteSpace(Plugin.Config.NameReplacement)) {
                __result.nameOverride = Plugin.Config.NameReplacement;
                __result = __result;
            }
        }
    }
}
