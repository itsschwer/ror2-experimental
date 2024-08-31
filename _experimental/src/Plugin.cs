using BepInEx;
using HarmonyLib;

using Input = UnityEngine.Input;
using Key = UnityEngine.KeyCode;

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

#if DEBUG
        private void Update()
        {
            var target = RoR2.LocalUserManager.GetFirstLocalUser()?.cachedBody;
            if (!target) return;

            if (Input.GetKeyDown(Key.F9)) Debugging.CommandCube.Spawn(target.footPosition, Debugging.CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier1));
            if (Input.GetKeyDown(Key.F10)) Debugging.CommandCube.Spawn(target.footPosition, Debugging.CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier2));
            if (Input.GetKeyDown(Key.F11)) Debugging.CommandCube.Spawn(target.footPosition, Debugging.CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier3));
            if (Input.GetKeyDown(Key.F12)) Debugging.CommandCube.Spawn(target.footPosition, Debugging.CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Boss));

            if (Input.GetKeyDown(Key.F8)) Debugging.CommandCube.Spawn(target.footPosition, Debugging.CommandCube.GetEquipmentPickupOptions());
            if (Input.GetKeyDown(Key.F7)) Debugging.CommandCube.Spawn(target.footPosition, Debugging.CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Lunar));
            if (Input.GetKeyDown(Key.F6)) Debugging.CommandCube.Spawn(target.footPosition, Debugging.CommandCube.GetPickupOptions(def => PressureDrop.Drop.IsVoidTier(def.tier)));
        }
#endif

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
