using BepInEx;

namespace SprintingOnTheScoreboard
{
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "SprintingOnTheScoreboard";
        public const string Version = "1.0.0";

        private void Awake()
        {
            Log.Init(Logger);
            new HarmonyLib.Harmony(Info.Metadata.GUID).PatchAll();
            Log.Message($"~awake.");
        }
    }
}
