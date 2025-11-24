using BepInEx;
using HarmonyLib;

namespace Experimental
{
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "experimental";
        public const string Version = "0.0.0";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            RoR2.UI.HUD.shouldHudDisplay += UI.HUD.Instantiate;
            RoR2.RoR2Application.onLoad += OnLoad;

            Commands.ChatCommandListener.Hook();
            Commands.Commands.Register();

            new Harmony(Info.Metadata.GUID).PatchAll();


            Logger.LogMessage("~awake.");
        }

        private static void OnLoad()
        {
            Logger.LogDebug(Dumps.Layers.Dump());
            RoR2.RoR2Application.onLoad -= OnLoad;
        }
    }
}
