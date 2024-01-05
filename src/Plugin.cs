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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void Awake()
        {
            Log.Init(Logger);
            new Harmony(Info.Metadata.GUID).PatchAll();
            Log.Message($"~awake.");
#if DEBUG
            RoR2.Inventory.onInventoryChangedGlobal += Debug.EternalGhost;
#if NEWT_ALTERNATIVE
            nAlt.OnEnable();
#endif
#endif
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnEnable()
        {
            Log.Message($"~enabled.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnDisable()
        {
            Log.Message($"~disabled.");
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




#if DEBUG
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void Update()
        {
            if (!UnityEngine.Networking.NetworkServer.active || !RoR2.Run.instance) return;

            bool ctrlKey = UnityEngine.Input.GetKey("left ctrl") || UnityEngine.Input.GetKey("right ctrl");
            if (!ctrlKey) return;

            RoR2.CharacterBody body = RoR2.LocalUserManager.GetFirstLocalUser()?.currentNetworkUser?.master?.GetBody();

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.S)) Debug.SpawnScrapper(body);
            else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.P)) Debug.SpawnPrinter(body);
            else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.C)) Debug.SpawnCauldron(body);
            else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.G)) body?.master?.inventory?.GiveItem(RoR2.RoR2Content.Items.TPHealingNova);
            else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.B)) Debug.SpawnBluePortal(body);

            else if (UnityEngine.Input.GetKeyDown("right alt")) Debug.SpawnJellyfish(body);
        }




#if NEWT_ALTERNATIVE
        private static NewtAlternative _nAlt;
        internal static NewtAlternative nAlt {
            get {
                _nAlt ??= new NewtAlternative();
                return _nAlt;
            }
        }
#endif
#endif
    }
}
