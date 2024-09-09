using BepInEx;
using HarmonyLib;
using System.Collections.ObjectModel;

namespace RestoreScoreboard
{
    [HarmonyPatch]
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "RestoreScoreboard";
        public const string Version = "1.0.0";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            new Harmony(Info.Metadata.GUID).PatchAll();

            Logger.LogMessage("Patched.");
        }

        [HarmonyPrefix, HarmonyPatch(typeof(RoR2.UI.ScoreboardController), nameof(RoR2.UI.ScoreboardController.Rebuild))]
        private static bool ScoreboardController_Rebuild(RoR2.UI.ScoreboardController __instance)
        {
            // Essentially revert to pre-SotS logic (remove Where clause that excludes inactive and dead (no body) elements)
            ReadOnlyCollection<RoR2.PlayerCharacterMasterController> instances = RoR2.PlayerCharacterMasterController.instances;
            int count = instances.Count;
            __instance.SetStripCount(count);
            for (int i = 0; i < count; i++) {
                __instance.stripAllocator.elements[i].SetMaster(instances[i].master);
            }

            return false; // Always skip original
        }
    }
}
