using HarmonyLib;
using RoR2;
using UnityEngine;

namespace AmGoldfish
{
    [HarmonyPatch]
    internal static class ShrineIndicator
    {
        [HarmonyPostfix, HarmonyPatch(typeof(PurchaseInteraction), nameof(PurchaseInteraction.OnInteractionBegin))]
        private static void OnPurchaseInteraction(PurchaseInteraction __instance, Interactor activator)
        {
            if (!__instance.CanBeAffordedByInteractor(activator)) return;

            ShrineBossBehavior mountain = __instance.GetComponent<ShrineBossBehavior>();
            if (mountain) OnMountainShrineInteracted(mountain);
            ShrineChanceBehavior chance = __instance.GetComponent<ShrineChanceBehavior>();
            if (chance) OnChanceShrineInteracted(chance);
        }

        private static void OnMountainShrineInteracted(ShrineBossBehavior shrine)
        {
            TeleporterInteraction tp = TeleporterInteraction.instance;
            if (tp == null || tp.shrineBonusStacks <= 1) return;

            // Wolfo | WolfoQualityOfLife.RandomMisc.LunarSeerStuff()
            // https://thunderstore.io/package/Wolfo/WolfoQualityOfLife/2.5.0/
            // https://github.com/WolfoIsBestWolf/ror2-WolfoQualityoLlife/blob/7151891/WolfoQualityOfLife/code/Misc/RandomMiscWithConfig.cs#L166-L171
            GameObject container = tp.bossShrineIndicator;
            GameObject target = container.transform.GetChild(0).gameObject;
            GameObject clone = Object.Instantiate(target, container.transform);
            clone.transform.localPosition = new Vector3(0, (tp.shrineBonusStacks - 1), 0);
        }

        private static void OnChanceShrineInteracted(ShrineChanceBehavior chance)
        {
            if (chance.successfulPurchaseCount <= 1) return;

            // todo
        }
    }
}
