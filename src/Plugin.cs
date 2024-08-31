using BepInEx;

namespace SprintingOnTheScoreboard
{
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "SprintingOnTheScoreboard";
        public const string Version = "1.0.0";

        internal new static BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            try {
                new HarmonyLib.Harmony(Info.Metadata.GUID).PatchAll();
                Logger.LogMessage("Successfully patched.");
            }
            catch (HarmonyLib.HarmonyException e) when (e.InnerException?.InnerException?.InnerException != null && e.InnerException.InnerException.InnerException.Message.Contains("Parameter \"onlyAllowMovement\" not found in method static bool RoR2.PlayerCharacterMasterController::CanSendBodyInput")) {
                // HarmonyLib.HarmonyException: IL Compile Error (unknown location) ---> HarmonyLib.HarmonyException: IL Compile Error (unknown location) ---> HarmonyLib.HarmonyException: IL Compile Error (unknown location) ---> System.Exception: Parameter "onlyAllowMovement" not found
                Logger.LogError(ReplicateUnityLogException(e)); // Keep the big scary red stack trace for perceptibility, but use plugin log source instead of Unity Log to better indicate source
                Logger.LogWarning("Failed to patch! This mod has no effect on versions prior to the Seekers Of The Storm patch.");
            }
        }

        private static string ReplicateUnityLogException(System.Exception exception)
        {
            System.Collections.Generic.Stack<System.Exception> exceptions = new();
            while (exception != null) {
                exceptions.Push(exception);
                exception = exception.InnerException;
            }

            System.Text.StringBuilder sb = new();
            System.Exception top = exceptions.Pop();
            sb.AppendLine($"{top.GetType().FullName}: {top.Message}").AppendLine("Stack trace:").AppendLine(top.StackTrace);
            while (exceptions.Count > 0) {
                System.Exception e = exceptions.Pop();
                sb.AppendLine($"Rethrow as {e.GetType().FullName}: {e.Message}").AppendLine(e.StackTrace);
            }

            return sb.ToString();
        }
    }
}
