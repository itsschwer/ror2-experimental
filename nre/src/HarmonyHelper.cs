using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;

namespace itsschwer
{
    internal static class HarmonyHelper
    {
        internal static void TryPatchAll(this Harmony harmony, ManualLogSource logger)
        {
            // https://github.com/BepInEx/HarmonyX/blob/v2.9.0/Harmony/Public/Harmony.cs#L143-L146
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in AccessTools.GetTypesFromAssembly(assembly)) {
                try {
                    harmony.CreateClassProcessor(type).Patch();
                }
                catch (Exception e) {
                    logger.LogError($"Failed to apply patch in {type.FullName}\n{e}");
                }
            }
        }
    }
}
