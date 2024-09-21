using HarmonyLib;
using RoR2;

namespace NameChanger.Patches
{
    [HarmonyPatch]
    internal static class HarmonyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SteamworksLobbyManager), nameof(SteamworksLobbyManager.GetUserDisplayName))]
        [HarmonyPatch(typeof(EOSLobbyManager), nameof(EOSLobbyManager.GetUserDisplayName))]
        private static void LobbyManager_GetUserDisplayName(ref string __result)
        {
            if (!string.IsNullOrWhiteSpace(Plugin.Config.NameReplacement)) {
                __result = Plugin.Config.NameReplacement;
            }
        }
    }
}
