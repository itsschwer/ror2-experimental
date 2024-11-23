using HarmonyLib;
using RoR2;

namespace Experimental.Patches
{
    [HarmonyPatch]
    internal static class NoDropPods
    {
        [HarmonyPrefix, HarmonyPatch(typeof(Run), nameof(Run.spawnWithPod), MethodType.Getter)]
        private static bool Run_spawnWithPod(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}
