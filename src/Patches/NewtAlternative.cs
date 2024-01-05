#if NEWT_ALTERNATIVE
using HarmonyLib;
using RoR2;
using System.Collections.Generic;

namespace Experimental
{
    [HarmonyPatch]
    internal class NewtAlternative
    {
        private readonly List<int> purchased = [];

        public bool Purchased(int budIndex) => purchased.Contains(budIndex);
        public bool MarkPurchased(int budIndex) {
            if (Purchased(budIndex)) { return false; }
            else { purchased.Add(budIndex); return true; }
        }

        internal void OnEnable() => Stage.onStageStartGlobal += OnStageStart;
        internal void OnDisable() => Stage.onStageStartGlobal -= OnStageStart;
        private void OnStageStart(Stage stage)
        {
            // new run or looped | RoR2.Achievements.LoopOnceAchievement.Check()
            if (Run.instance.stageClearCount == 0 || Run.instance.loopClearCount > 0) {
                purchased.Clear();
            }
        }

        // Patches -----------------------------------------
#if DEBUG
        [HarmonyPostfix, HarmonyPatch(typeof(PurchaseInteraction), nameof(PurchaseInteraction.Awake))]
        private static void Awake(PurchaseInteraction __instance)
        {
            if (__instance.displayNameToken == "LUNAR_TERMINAL_NAME") { LunarBudAwake(__instance); return; }
            if (__instance.displayNameToken == "NEWT_STATUE_NAME" && __instance.GetComponent<PortalStatueBehavior>() != null) { NewtAltarAwake(__instance); return; }
        }

        private static void LunarBudAwake(PurchaseInteraction lunarBud)
        {
            int idx = (lunarBud.transform.parent.name == "LunarTable") ? lunarBud.transform.GetSiblingIndex() : -1;
            if (idx < 0 || !Plugin.nAlt.Purchased(idx)) return;

            lunarBud.SetAvailable(false);
            lunarBud.onPurchase.Invoke(null); // sets the correct appearance but spawns an invisible but ping-able '???' pickup.
        }

        private static void NewtAltarAwake(PurchaseInteraction newtAltar)
        {
            newtAltar.Networkcost = 0;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PurchaseInteraction), nameof(PurchaseInteraction.OnInteractionBegin))]
        private static void OnInteractionBegin(PurchaseInteraction __instance, Interactor activator)
        {
            if (__instance.displayNameToken != "LUNAR_TERMINAL_NAME") return;
            if (!__instance.CanBeAffordedByInteractor(activator)) return;

            int idx = (__instance.transform.parent.name == "LunarTable") ? __instance.transform.GetSiblingIndex() : -1;
            Plugin.nAlt.MarkPurchased(idx);
        }
    }
#endif
}
#endif
