using BepInEx;

namespace SprintingOnTheScoreboard
{
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "SprintingOnTheScoreboard";
        public const string Version = "1.0.1";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            try {
                new HarmonyLib.Harmony(Info.Metadata.GUID).PatchAll();
#if DEBUG
                Logger.LogMessage("Successfully patched. [config:Debug]");
#else
                Logger.LogMessage("Successfully patched.");
#endif
            }
            catch (HarmonyLib.HarmonyException e) when (e.InnerException?.InnerException?.InnerException != null && e.InnerException.InnerException.InnerException.Message.Contains("Parameter \"onlyAllowMovement\" not found in method static bool RoR2.PlayerCharacterMasterController::CanSendBodyInput")) {
                // HarmonyLib.HarmonyException: IL Compile Error (unknown location) ---> HarmonyLib.HarmonyException: IL Compile Error (unknown location) ---> HarmonyLib.HarmonyException: IL Compile Error (unknown location) ---> System.Exception: Parameter "onlyAllowMovement" not found
                Logger.LogError(e); // Keep the scary big red stack trace for perceptibility but use plugin log source (instead of Unity Log)
                Logger.LogWarning("Failed to patch! This mod has no effect on versions prior to the Seekers Of The Storm patch.");
            }
        }
    }
}
