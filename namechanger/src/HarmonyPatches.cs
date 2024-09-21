using HarmonyLib;
using RoR2;

namespace NameChanger.Patches
{
    [HarmonyPatch]
    internal static class HarmonyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NetworkPlayerName), nameof(NetworkPlayerName.GetResolvedName))]
        private static void NetworkPlayerName_GetResolvedName(NetworkPlayerName __instance, ref string __result)
        {
            bool isClient = LocalUserManager.GetFirstLocalUser().currentNetworkUser.GetNetworkPlayerName().playerId == __instance.playerId;
            if (isClient && !string.IsNullOrWhiteSpace(Plugin.Config.NameReplacement)) {
                __result = Plugin.Config.NameReplacement;
            }
        }
    }
}
