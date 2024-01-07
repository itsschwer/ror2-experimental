#define NEWT_ALTERNATIVE
#if NEWT_ALTERNATIVE
using HarmonyLib;
using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Experimental
{
    [HarmonyPatch]
    internal class NewtAlternative
    {
        private static NewtAlternative _instance;
        private static NewtAlternative Self => _instance ??= new NewtAlternative();

        private readonly List<int> purchased = [];

        public bool Purchased(int budIndex) => purchased.Contains(budIndex);
        public bool MarkPurchased(int budIndex) {
            if (Purchased(budIndex)) { return false; }
            else { purchased.Add(budIndex); return true; }
        }

        // Patches -----------------------------------------

        [HarmonyPostfix, HarmonyPatch(typeof(Stage), nameof(Stage.Start))]
        private static void OnStageStart()
        {
            if (!NetworkServer.active) return;

            // new run or looped | RoR2.Achievements.LoopOnceAchievement.Check()
            if (Run.instance.stageClearCount == 0 || Run.instance.loopClearCount > 0) {
                Self.purchased.Clear();
#if DEBUG
                Log.Info($"{nameof(NewtAlternative)}> Resetting lunar bud availability.");
#endif
            }
#if DEBUG
            else Log.Info($"{nameof(NewtAlternative)}> {Self.purchased.Count} lunar buds locked | {Run.instance.stageClearCount} stages cleared"); ;
#endif
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PurchaseInteraction), nameof(PurchaseInteraction.Awake))]
        private static void Awake(PurchaseInteraction __instance)
        {
            if (!NetworkServer.active) return;
            if (__instance.displayNameToken == "LUNAR_TERMINAL_NAME") { LunarBudAwake(__instance); return; }
            if (__instance.displayNameToken == "NEWT_STATUE_NAME" && __instance.GetComponent<PortalStatueBehavior>() != null) { __instance.Networkcost = 0; return; }
        }

        private static void LunarBudAwake(PurchaseInteraction lunarBud)
        {
            if (!NetworkServer.active) return;

            int idx = (lunarBud.transform.parent.name == "LunarTable") ? lunarBud.transform.GetSiblingIndex() : -1;
            if (idx < 0) return;
#if DEBUG
            for (int i = 0; i < lunarBud.onPurchase.GetPersistentEventCount(); i++) {
                Log.Info($"[{i}] {lunarBud.onPurchase.GetPersistentMethodName(i)} | {lunarBud.onPurchase.GetPersistentTarget(i)}");
                /* PurchaseInteraction.SetAvailable()
                 * ShopTerminalBehaviour.DropPickup() -> ShopTerminalBehaviour.SetHasBeenPurchased(true)
                 * ShopTerminalBehaviour.SetNoPickup()
                 * BazaarController.CommentOnLunarPurchase()
                 * EntityLogic.Counter.Add()
                 */
            }
#endif
            if (!Self.Purchased(idx)) return;

            lunarBud.SetAvailable(false); // Interactable state
            ShopTerminalBehavior terminal = lunarBud.GetComponent<ShopTerminalBehavior>();
            terminal?.SetHasBeenPurchased(true); // Visual state
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PurchaseInteraction), nameof(PurchaseInteraction.OnInteractionBegin))]
        private static void OnInteractionBegin(PurchaseInteraction __instance, Interactor activator)
        {
            if (!NetworkServer.active) return;
            if (__instance.displayNameToken != "LUNAR_TERMINAL_NAME") return;
            if (!__instance.CanBeAffordedByInteractor(activator)) return;

            int idx = (__instance.transform.parent.name == "LunarTable") ? __instance.transform.GetSiblingIndex() : -1;
            Self.MarkPurchased(idx);
        }
    }
}
#endif
