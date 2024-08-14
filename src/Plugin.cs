using BepInEx;
using Eater.IL;

namespace Eater
{
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "eater";
        public const string Version = "0.0.0";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void Awake()
        {
            Log.Init(Logger);

            LetsGoGamblingSuccessSoundEater.Apply();
            DynamicSkinsAccessoriesArgumentNullEater.Apply();

            Log.Message("~awake.");
        }
    }
}
