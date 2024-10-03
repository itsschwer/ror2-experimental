using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RoR2;
using System;
using System.Reflection;

namespace RulebookItemBlacklist.IL
{
    /// <summary>
    /// Always show the item icon (vanilla displays a padlock icon when disabled).
    /// </summary>
    internal static class RuleDef_FromItem
    {
        internal static void Apply()
        {
            MethodInfo method = typeof(RuleDef).GetMethod(nameof(RuleDef.FromItem));
            ILHook hook = new ILHook(method, (il) => {
                ILCursor c = new ILCursor(il);

                // Keep preceding `dup`
                Func<Instruction, bool>[] match = {
                    x => x.MatchLdstr("Textures/MiscIcons/texUnlockIcon"),
                    x => x.MatchCallOrCallvirt<RuleChoiceDef>($"set_{nameof(RuleChoiceDef.spritePath)}")
                };

                if (c.TryGotoNext(match)) {
                    c.RemoveRange(match.Length);
                    // ruleChoiceDef.sprite = itemDef.pickupIconSprite;
                    c.Emit(OpCodes.Ldloc_0);
                    c.Emit(OpCodes.Ldfld, typeof(ItemDef).GetField(nameof(ItemDef.pickupIconSprite)));
                    c.Emit(OpCodes.Stfld, typeof(RuleChoiceDef).GetField(nameof(RuleChoiceDef.sprite)));
                }
                else Plugin.Logger.LogError($"{nameof(RuleDef_FromItem)}> Cannot hook: failed to match IL instructions.");
            });
        }
    }
}
