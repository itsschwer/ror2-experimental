using BepInEx;
using HarmonyLib;
using RoR2;

namespace NameChanger
{
    [HarmonyPatch]
    [BepInDependency(RiskOfOptions.PluginInfo.PLUGIN_GUID)]
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "NameChanger";
        public const string Version = "0.0.1";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        internal static new Config Config { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            Config = new Config(base.Config);

            new Harmony(Info.Metadata.GUID).PatchAll();

            Logger.LogMessage("~awake.");
        }

        public static void UpdateUserNames() {
            foreach (NetworkUser user in NetworkUser.readOnlyInstancesList) {
                user.UpdateUserName();
            }
        }




        [HarmonyPostfix, HarmonyPatch(typeof(NetworkUser), nameof(NetworkUser.UpdateUserName))]
        private static void NetworkUser_UpdateUserName(NetworkUser __instance)
        {
            bool isClient = __instance.isLocalPlayer;
            if (isClient && !string.IsNullOrWhiteSpace(Plugin.Config.NameReplacement)) {
                __instance.userName = Plugin.Config.NameReplacement;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(LocalUser), nameof(LocalUser.LinkNetworkUser))]
        private static void LocalUser_LinkNetworkUser() => Plugin.UpdateUserNames();
    }
}
