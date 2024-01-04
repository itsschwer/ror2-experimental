using HarmonyLib;
using RoR2;
using System.Collections.Generic;

namespace Experimental
{
    [HarmonyPatch]
    internal class NewtAlternative
    {
        private static NewtAlternative _instance;
        private static NewtAlternative instance {
            get {
                if (_instance == null) {
                    _instance = new NewtAlternative();
                }
                return _instance;
            }
        }

        // Instance ----------------------------------------

        private List<int> purchased = [];

        public bool Purchased(int budIndex) => purchased.Contains(budIndex);
        public bool MarkPurchased(int budIndex) {
            if (Purchased(budIndex)) { return false; }
            else { purchased.Add(budIndex); return true; }
        }

        private void OnEnable()
        {
            Stage.onStageStartGlobal += OnStageStart;
        }

        private void OnStageStart(Stage stage)
        {
            // RoR2.Achievements.LoopOnceAchievement.Check()
            if (Run.instance.loopClearCount > 0) {
                purchased.Clear();
            }
        }

        // Patches -----------------------------------------
#if DEBUG
        [HarmonyPostfix, HarmonyPatch(typeof(PurchaseInteraction), nameof(PurchaseInteraction.Awake))]
        private static void Awake(PurchaseInteraction __instance)
        {
            if (__instance.displayNameToken == "LUNAR_TERMINAL_NAME") { LunarBudAwake(__instance); return; }
            if (__instance.GetComponent<PortalStatueBehavior>() != null) { NewtAltarAwake(__instance); return; }
        }

        private static void LunarBudAwake(PurchaseInteraction lunarBud)
        {
            lunarBud.SetAvailable(false);
            lunarBud.onPurchase.Invoke(null);
        }

        private static void NewtAltarAwake(PurchaseInteraction newtAltar)
        {
            newtAltar.Networkcost = 0;
        }
    }
#endif
}
