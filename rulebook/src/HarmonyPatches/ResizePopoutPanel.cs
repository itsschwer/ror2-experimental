using HarmonyLib;
using RoR2.UI;
using RoR2;
using UnityEngine.UI;
using UnityEngine;

namespace RulebookItemBlacklist.HarmonyPatches
{
    [HarmonyPatch]
    internal static class ResizePopoutPanel
    {
        [HarmonyPostfix, HarmonyPatch(typeof(RuleCategoryController), nameof(RuleCategoryController.SetData))]
        private static void RuleCategoryController_SetData(RuleCategoryController __instance, RuleCategoryDef categoryDef)
        {
            // Hard-coded, kinda yuck
            if (categoryDef.children.Count > 144) {
                GridLayoutGroup grid = __instance.popoutPanelContentContainer.GetComponent<GridLayoutGroup>();
                grid.childAlignment = TextAnchor.UpperCenter;
                grid.constraintCount = 19;
                RectTransform rect = (RectTransform)__instance.popoutPanelInstance.transform;
                rect.sizeDelta = Vector2.right * 812f;
            }
        }
    }
}
