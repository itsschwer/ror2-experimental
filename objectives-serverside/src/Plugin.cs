using BepInEx;
using RoR2;

namespace ObjectivesServerside
{
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "ObjectivesServerside";
        public const string Version = "0.0.0";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            Logger.LogMessage("~awake.");
        }

        private void OnEnable()
        {
            On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;
            ShrineHalcyoniteObjective.Enable();

            Logger.LogMessage("~enabled.");
        }

        private void OnDisable()
        {
            On.RoR2.ShrineChanceBehavior.AddShrineStack -= ShrineChanceBehavior_AddShrineStack;
            ShrineHalcyoniteObjective.Disable();

            Logger.LogMessage("~disabled.");
        }

        private static void ShrineChanceBehavior_AddShrineStack(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig, ShrineChanceBehavior self, Interactor activator)
        {
            orig(self, activator);
            string message = $"{activator.GetComponent<CharacterBody>().GetUserName()}'s {Language.GetString(DLC2Content.Items.ExtraShrineItem.nameToken)} smiles.";
            if (self.chanceDollWin) Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cStack>></style> <style=cShrine>{message}</style>" });
        }
    }
}
