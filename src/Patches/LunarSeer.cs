using HarmonyLib;
using RoR2;

namespace AmGoldfish
{
    [HarmonyPatch]
    internal static class LunarSeer
    {
        [HarmonyPostfix, HarmonyPatch(typeof(SeerStationController), nameof(SeerStationController.OnTargetSceneChanged))]
        private static void UpdateDisplayName(SeerStationController __instance, SceneDef targetScene)
        {
            PurchaseInteraction interaction = __instance.GetComponent<PurchaseInteraction>();
            if (interaction == null) return;

            // Wolfo | WolfoQualityOfLife.RandomMisc.LunarSeerStuff()
            // https://thunderstore.io/package/Wolfo/WolfoQualityOfLife/2.5.0/
            // https://github.com/WolfoIsBestWolf/ror2-WolfoQualityoLlife/blob/7151891/WolfoQualityOfLife/code/Misc/RandomMisc.cs#L317-L328
            const string defaultDisplayNameToken = "BAZAAR_SEER_NAME";
            if (targetScene == null) {
                interaction.displayNameToken = defaultDisplayNameToken;
            }
            else {
                string destination = Language.GetString(targetScene.nameToken).Replace("Hidden Realm: ", "");
                interaction.displayNameToken = $"{Language.GetString(defaultDisplayNameToken)} ({destination})";
            }
        }
    }
}
