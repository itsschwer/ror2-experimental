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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void Awake()
        {
            Log.Init(Logger);
            new Harmony(Info.Metadata.GUID).PatchAll();
            Log.Message($"~awake.");
#if DEBUG
            RoR2.Inventory.onInventoryChangedGlobal += Debug.EternalGhost;
#endif
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnEnable()
        {
            Log.Message($"~enabled.");
#if DEBUG
            Commands.Register();
#endif
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnDisable()
        {
            Log.Message($"~disabled.");
#if DEBUG
            Commands.Unregister();
#endif
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
