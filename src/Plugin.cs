using BepInEx;
using HarmonyLib;

namespace AmGoldfish
{
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "AmGoldfish";
        public const string Version = "0.0.0";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void Awake()
        {
            Log.Init(Logger);
            new Harmony(Info.Metadata.GUID).PatchAll();
            Log.Message($"{Plugin.GUID}> awake.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnEnable()
        {
            Log.Message($"{Plugin.GUID}> enabled.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnDisable()
        {
            Log.Message($"{Plugin.GUID}> disabled.");
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
        private void Update()
        {
            if (!UnityEngine.Networking.NetworkServer.active || !RoR2.Run.instance) return;

            bool ctrlKey = UnityEngine.Input.GetKey("left ctrl") || UnityEngine.Input.GetKey("right ctrl");
            if (!ctrlKey) return;

            RoR2.CharacterBody user = RoR2.LocalUserManager.GetFirstLocalUser()?.currentNetworkUser?.master?.GetBody();
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.S)) Debug.SpawnScrapper(user);
            else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.P)) Debug.SpawnPrinter(user);
            else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.C)) Debug.SpawnCauldron(user);
            else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.M)) Debug.SpawnShrineBoss(user);
        }
#endif
    }
}
