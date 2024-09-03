using BepInEx;
using HarmonyLib;

namespace Experimental
{
    [BepInDependency(DamageLog.Plugin.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(PressureDrop.Plugin.GUID)]
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

#if DEBUG
            Debugging.NoDropPods.Apply();
            RoR2.UI.HUD.shouldHudDisplay += Debugging.UI.HUD.Instantiate;
            Commands.Register();
#endif
            new Harmony(Info.Metadata.GUID).PatchAll();

            Logger.LogMessage("~awake.");
        }

        public static int GetClientPingMilliseconds()
        {
            // Thrayonlosa | QolElements.PingHud.GetPing()
            // https://thunderstore.io/package/Thrayonlosa/QolElements/2.1.0/
            if (UnityEngine.Networking.NetworkClient.active) {
                UnityEngine.Networking.NetworkConnection connection = RoR2.Networking.NetworkManagerSystem.singleton?.client?.connection;
                if (connection != null) {
                    return (int)RoR2.Networking.RttManager.GetConnectionRTTInMilliseconds(connection);
                }
            }

            return -1;
        }
    }
}
