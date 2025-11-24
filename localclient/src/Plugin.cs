using BepInEx;
using HarmonyLib;
using RoR2;

namespace LocalClient
{
    [HarmonyPatch]
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "localclient";
        public const string Version = "0.0.1";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            new Harmony(Info.Metadata.GUID).PatchAll();

            // Unlock console so it's easier to open and input "connect localhost:7777"
            RoR2.UI.ConsoleWindow.cvConsoleEnabled.defaultValue = "1";
            RoR2.UI.ConsoleWindow.cvConsoleEnabled.SetBool(true);

            Logger.LogMessage("~awake.");
        }


        [HarmonyPrefix, HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.Save))]
        private static bool SaveSystem_Save(ref bool __result)
        {
            __result = true; // pretend save was successful (so that pending save requests are not stuck trying to save every frame [SaveSysem.StaticUpdate])
            return false;    // never run method
        }
    }
}
