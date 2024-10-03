using HarmonyLib;
using RoR2;
using RoR2.UI;

namespace RulebookItemBlacklist.HarmonyPatches
{
    [HarmonyPatch]
    internal static class OnlyShowDisabledItems
    {
        [HarmonyPostfix, HarmonyPatch(typeof(RuleCategoryController), nameof(RuleCategoryController.SetData))]
        private static void RuleCategoryController_SetData(RuleCategoryController __instance, RuleChoiceMask availability, RuleCategoryDef categoryDef, RuleBook ruleBook)
        {
            if (categoryDef.ruleCategoryType == RuleCatalog.RuleCategoryType.VoteResultGrid &&
                (categoryDef.displayToken == "RULE_HEADER_ITEMS" || categoryDef.displayToken == "RULE_HEADER_EQUIPMENT"))
            {
                foreach (RuleChoiceController choice in __instance.voteResultIconAllocator.elements) {
                    RuleDef rule = choice.choiceDef.ruleDef;
                    // Hide vote result grid choice if not blacklisted and can be blacklisted (i.e. not disabled by missing expansion)
                    choice.gameObject.SetActive(ruleBook.GetRuleChoiceIndex(rule) != rule.defaultChoiceIndex && rule.AvailableChoiceCount(availability) > 1);
                }
            }
        }
    }
}
