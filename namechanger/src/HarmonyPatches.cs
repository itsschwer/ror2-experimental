using HarmonyLib;
using RoR2;

namespace NameChanger.Patches
{
    [HarmonyPatch]
    internal static class HarmonyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NetworkUser), nameof(NetworkUser.UpdateUserName))]
        private static void NetworkUser_UpdateUserName(NetworkUser __instance)
        {
            bool isClient = __instance.isLocalPlayer;
            if (isClient && !string.IsNullOrWhiteSpace(Plugin.Config.NameReplacement)) {
                __instance.userName = Plugin.Config.NameReplacement;
            }
        }
    }
}
