using HarmonyLib;
using RoR2;

namespace Experimental.Patches
{
    [HarmonyPatch]
    internal static class TeleportHelperTrace
    {
        [HarmonyPostfix, HarmonyPatch(typeof(TeleportHelper), nameof(TeleportHelper.TeleportGameObject), [typeof(UnityEngine.GameObject), typeof(UnityEngine.Vector3)])]
        private static void TeleportHelper_TeleportGameObject(UnityEngine.Vector3 newPosition)
        {
            string trace = new System.Diagnostics.StackTrace().ToString();
            if (!trace.Contains("RoR2.MapZone.TeleportBody")) {
                Plugin.Logger.LogWarning($"{nameof(TeleportHelper_TeleportGameObject)}> {newPosition}\n{new System.Diagnostics.StackTrace()}");
            }
        }
    }
}
