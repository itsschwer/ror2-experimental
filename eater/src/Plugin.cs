using BepInEx;
using Eater.IL;

namespace Eater
{
#if LETSGOGAMBLING
    [BepInDependency(LetsGoGambling.LetsGoGamblingPlugin.MODUID, BepInDependency.DependencyFlags.SoftDependency)]
#endif
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "eater";
        public const string Version = "0.0.0";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            DynamicSkinsAccessoriesArgumentNullEater.Apply();
#if LETSGOGAMBLING
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LetsGoGambling.LetsGoGamblingPlugin.MODUID)) {
                LetsGoGamblingSuccessSoundEater.Apply();
            }
#endif
            UnityExplorerEater.Apply();

            Logger.LogMessage("~awake.");
        }
    }
}
