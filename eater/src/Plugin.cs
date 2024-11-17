using BepInEx;
using Eater.IL;

namespace Eater
{
    [BepInDependency(LetsGoGambling.LetsGoGamblingPlugin.MODUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(SivsContentPack.SivsContentPack.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(itsschwer.Items.Plugin.GUID, BepInDependency.DependencyFlags.SoftDependency)]
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

            itsschwer.Junk.ShuffleEliteTiers.Init();
            itsschwer.Junk.ShrineHalcyoniteObjective.Enable();
            Compat.SivsContentPackAddendum.Apply();

            LetsGoGamblingSuccessSoundEater.Apply();
            UnityExplorerEater.Apply();

            Logger.LogMessage("~awake.");
        }
    }
}
