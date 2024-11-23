using BepInEx;
using HarmonyLib;

namespace Experimental
{
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

            RoR2.UI.HUD.shouldHudDisplay += UI.HUD.Instantiate;
            Commands.Commands.Register();

            new Harmony(Info.Metadata.GUID).PatchAll();

            RoR2.RoR2Application.onLoad += Dump;

            Logger.LogMessage("~awake.");
        }

        private static void Dump()
        {
            foreach (RoR2.ItemDef item in RoR2.ItemCatalog.allItemDefs) {
                string line = Dumps.PickupItemInfo.Dump(RoR2.PickupCatalog.GetPickupDef(RoR2.PickupCatalog.FindPickupIndex(item.itemIndex)), out bool hiddenOrCantRemove);
                if (hiddenOrCantRemove) Logger.LogWarning(line);
                else Logger.LogDebug(line);
            }
#if DEBUG
            Logger.LogDebug(Dumps.Layers.Dump());
#endif
            RoR2.RoR2Application.onLoad -= Dump;
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
