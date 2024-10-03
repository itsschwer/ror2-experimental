using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RoR2;
using RoR2.UI;
using System;
using System.Reflection;

namespace RulebookItemBlacklist.IL
{
    [HarmonyPatch]
    internal static class RuleCategoryController_SetData_Patch
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

#if DEBUG
        internal static void Apply()
        {
            MethodInfo method = typeof(RuleCategoryController).GetMethod(nameof(RuleCategoryController.SetData));
            ILHook hook = new ILHook(method, (il) => {
                ILCursor c = new ILCursor(il);

                Func<Instruction, bool>[] voteResultGrid = {
                    // stripContainer.gameObject.SetActive(value: false);
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<RuleCategoryController>(nameof(RuleCategoryController.stripContainer)),
                    x => x.MatchCallOrCallvirt<UnityEngine.Component>($"get_{nameof(UnityEngine.Component.gameObject)}"),
                    x => x.MatchLdcI4(0),
                    x => x.MatchCallOrCallvirt<UnityEngine.GameObject>(nameof(UnityEngine.GameObject.SetActive)),
                    // voteResultGridContainer.gameObject.SetActive(value: true);
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<RuleCategoryController>(nameof(RuleCategoryController.voteResultGridContainer)),
                    x => x.MatchCallOrCallvirt<UnityEngine.Component>($"get_{nameof(UnityEngine.Component.gameObject)}"),
                    x => x.MatchLdcI4(1),
                    x => x.MatchCallOrCallvirt<UnityEngine.GameObject>(nameof(UnityEngine.GameObject.SetActive)),
                };

                if (c.TryGotoNext(voteResultGrid)) {
                    // Func<Instruction, bool>[] 

                    Plugin.Logger.LogDebug(il.ToString());
                }
                else Plugin.Logger.LogError($"{nameof(RuleCategoryController_SetData)}> Cannot hook: failed to match IL instructions.");
            });
        }

        internal static void Apply_OLD()
        {
            MethodInfo method = typeof(RuleCategoryController).GetMethod(nameof(RuleCategoryController.SetData));
            ILHook hook = new ILHook(method, (il) => {
                ILCursor c = new ILCursor(il);

                /*
                // bool flag = false;
                Func<Instruction, bool>[] flagInit = {
                    x => x.MatchLdcI4(0),
                    x => x.MatchStloc(4)
                };

                if (c.TryGotoNext(MoveType.After, flagInit)) {
                    c.Emit(OpCodes.Ldloc_3); // ruleDef
                    c.Emit(OpCodes.Ldarg_3); // ruleBook
                    c.EmitDelegate<Func<RuleDef, RuleBook, bool>>((ruleDef, ruleBook) => {
                        bool matchedCategory = (ruleDef.category.displayToken == "RULE_HEADER_ITEMS" || ruleDef.category.displayToken == "RULE_HEADER_EQUIPMENT");
                        if (ruleDef.choices[ruleDef.defaultChoiceIndex].itemIndex == RoR2Content.Items.ArmorPlate.itemIndex) Plugin.Logger.LogDebug($"{ruleDef.category.displayToken} | {nameof(matchedCategory)}: {matchedCategory}");
                        if (matchedCategory) {
                            // if (ruleDef.choices[ruleDef.defaultChoiceIndex].itemIndex == RoR2Content.Items.ArmorPlate.itemIndex) Plugin.Logger.LogDebug($"{ruleDef.globalName} | {ruleDef.globalIndex} | {ruleDef.choices[ruleDef.defaultChoiceIndex].globalIndex} | {ruleBook.GetRuleChoiceIndex(ruleDef)}");
                            // return ruleBook.GetRuleChoiceIndex(ruleDef) != ruleDef.choices[ruleDef.defaultChoiceIndex].globalIndex;
                            return false;
                            if (ruleDef.choices[ruleDef.defaultChoiceIndex].itemIndex == RoR2Content.Items.ArmorPlate.itemIndex) Plugin.Logger.LogDebug($"{ruleDef.globalName} | {ruleDef.defaultChoiceIndex} | {ruleBook.GetRuleChoiceIndex(ruleDef)}");
                            return ruleBook.GetRuleChoiceIndex(ruleDef) != ruleDef.defaultChoiceIndex;
                        }
                        return false;
                    });
                    c.Emit(OpCodes.Stloc, 4);

                    c.Emit(OpCodes.Ldloc, 4);
                    ILLabel check = c.MarkLabel();
                    c.Emit(OpCodes.Brtrue, check); // custom condition will be overwritten...

                    // if (flag
                    Func<Instruction, bool>[] flagCheck = {
                        x => x.MatchLdloc(4),
                        x => x.MatchBrtrue(out _)
                    };
                    
                    if (c.TryGotoNext(flagCheck)) {
                        c.MarkLabel(check);
                        Plugin.Logger.LogMessage(il.ToString());
                    }
                    else Plugin.Logger.LogError($"{nameof(RuleCategoryController_SetData)}> Incomplete hook: failed to match IL instructions.");
                }

                /*
                // rulesToDisplay.Add(children[i])
                Func<Instruction, bool>[] match = {
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<RuleCategoryController>(nameof(RuleCategoryController.rulesToDisplay)),
                    x => x.MatchLdloc(1),
                    x => x.MatchLdloc(2),
                    x => x.MatchCallOrCallvirt<System.Collections.Generic.List<RuleDef>>("get_Item"),
                    x => x.MatchCallOrCallvirt<System.Collections.Generic.List<RuleDef>>(nameof(System.Collections.Generic.List<RuleDef>.Add))
                };

                if (c.TryGotoNext(match)) {
                    // ruleDef
                    c.Emit(OpCodes.Ldloc_3);
                    c.EmitDelegate<Func<RuleDef, bool>>((ruleDef) => {
                        // Plugin.Logger.LogDebug(ruleDef.category.displayToken);
                        bool matchedCategory = (ruleDef.category.displayToken == "RULE_HEADER_ITEMS" || ruleDef.category.displayToken == "RULE_HEADER_EQUIPMENT");
                        // Plugin.Logger.LogDebug($"{nameof(matchedCategory)}: {matchedCategory}");
                        if (matchedCategory)
                        {
                            Plugin.Logger.LogDebug($"{ruleDef.globalName} | {ruleDef.globalIndex} | {ruleDef.choices[ruleDef.defaultChoiceIndex].globalIndex}");
                            return ruleDef.globalIndex != ruleDef.choices[ruleDef.defaultChoiceIndex].globalIndex;
                        }
                        return false;
                    });
                    c.Emit(OpCodes.Stloc, 4);
                    c.Emit(OpCodes.Brfalse)
                }
                */

                //! removes choices form both the vote result grid (intended) and the popout panel (unintended)
                // rulesToDisplay.Add(children[i])
                Func<Instruction, bool>[] match = {
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<RuleCategoryController>(nameof(RuleCategoryController.rulesToDisplay)),
                    x => x.MatchLdloc(1),
                    x => x.MatchLdloc(2),
                    // x => x.MatchCallOrCallvirt<System.Collections.Generic.List<RuleDef>>("get_Item"),
                    // x => x.MatchCallOrCallvirt<System.Collections.Generic.List<RuleDef>>(nameof(System.Collections.Generic.List<RuleDef>.Add))
                    x => x.MatchCallOrCallvirt(out _),
                    x => x.MatchCallOrCallvirt(out _)
                };

                if (c.TryGotoNext(match)) {
                    c.Emit(OpCodes.Ldloc_3); // ruleDef
                    c.Emit(OpCodes.Ldarg_3); // ruleBook
                    c.EmitDelegate<Func<RuleDef, RuleBook, bool>>((ruleDef, ruleBook) => {
                        bool matchedCategory = (ruleDef.category.displayToken == "RULE_HEADER_ITEMS" || ruleDef.category.displayToken == "RULE_HEADER_EQUIPMENT");
                        if (ruleDef.choices[ruleDef.defaultChoiceIndex].itemIndex == RoR2Content.Items.ArmorPlate.itemIndex) Plugin.Logger.LogDebug($"{ruleDef.category.displayToken} | {nameof(matchedCategory)}: {matchedCategory}");
                        if (matchedCategory) {
                            if (ruleDef.choices[ruleDef.defaultChoiceIndex].itemIndex == RoR2Content.Items.ArmorPlate.itemIndex) Plugin.Logger.LogDebug($"{ruleDef.globalName} | {ruleDef.defaultChoiceIndex} | {ruleBook.GetRuleChoiceIndex(ruleDef)}");
                            return ruleBook.GetRuleChoiceIndex(ruleDef) == ruleDef.defaultChoiceIndex;
                        }
                        return false;
                    });

                    ILLabel next = c.MarkLabel();
                    c.Emit(OpCodes.Brtrue, next);
                    c.GotoNext(MoveType.After, match);
                    c.MarkLabel(next);

                    Plugin.Logger.LogDebug(il.ToString());
                }
                else Plugin.Logger.LogError($"{nameof(RuleCategoryController_SetData)}> Cannot hook: failed to match IL instructions.");
            });
        }
#endif
    }
}
