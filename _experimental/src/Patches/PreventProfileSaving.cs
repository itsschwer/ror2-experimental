using HarmonyLib;
using RoR2;

namespace Experimental.Patches
{
    [HarmonyPatch]
    internal static class PreventProfileSaving
    {
        [HarmonyPrefix, HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.Save))]
        private static bool SaveSystem_Save(ref bool __result)
        {
            const bool dontSave = true;
            __result = !dontSave;
            return !dontSave;
        }
    }
}
