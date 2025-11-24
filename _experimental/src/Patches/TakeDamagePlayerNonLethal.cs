using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace Experimental.Patches
{
    [HarmonyPatch]
    internal static class TakeDamagePlayerNonLethal
    {
        private static bool _active = true;
        public static bool Active => _active;
        public static void SetActive(bool active)
        {
            _active = active;
        }

#if NETSTANDARD2_1_OR_GREATER
        [HarmonyILManipulator, HarmonyPatch(typeof(HealthComponent), nameof(HealthComponent.TakeDamageProcess))]
#else
        [HarmonyILManipulator, HarmonyPatch(typeof(HealthComponent), nameof(HealthComponent.TakeDamage))]
#endif
        private static void HealthComponent_TakeDamage_NonLethalToPlayers(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int ldargHealthComponent = -1;
            Func<Instruction, bool>[] matchSet = {
                // num_ = 1f;
                // x => x.MatchLdarg(out int _),
                // x => x.MatchLdfld<HealthComponent>(nameof(HealthComponent.health)),
                // x => x.MatchLdcR4(1),
                // -- RoR2BepInExPack.VanillaFixes.FixNonLethalOneHP.FixLethality --
                // x => x.MatchBltUn(out ILLabel _),

                x => x.MatchLdcR4(1f),
                x => x.MatchStloc(out int _),

                // Networkhealth = num_
                x => x.MatchLdarg(out ldargHealthComponent),
                x => x.MatchLdloc(out int _),
                x => x.MatchCallOrCallvirt<HealthComponent>($"set_{nameof(HealthComponent.Networkhealth)}"),
            };

            if (c.TryGotoNext(matchSet)) {
                ILLabel eq1f = c.MarkLabel();

                Func<Instruction, bool>[] matchCheck = {
                    // if (num_ < 1f && (damageInfo.damageType & DamageType.NonLethal) != 0 && health >= 1f)
                    x => x.MatchLdloc(out int _),
                    x => x.MatchLdcR4(1f),
                    x => x.MatchBgeUn(out ILLabel _)
                    // ... (diverge after SotS)
                };

                if (c.TryGotoPrev(MoveType.After, matchCheck)) {
                    c.Emit(OpCodes.Ldarg, ldargHealthComponent);
                    c.EmitDelegate<Func<HealthComponent, bool>>((@this) => {
                        Plugin.Logger.LogWarning($"{@this.body.name} | {@this.body.isPlayerControlled && Active} | {@this.body.isPlayerControlled}");
                        return @this.body.isPlayerControlled && Active;
                    });
                    c.Emit(OpCodes.Brtrue, eq1f);
                }
                else Plugin.Logger.LogError($"{nameof(TakeDamagePlayerNonLethal)}> Cannot hook: failed to match IL instructions (second set)");
            }
            else Plugin.Logger.LogError($"{nameof(TakeDamagePlayerNonLethal)}> Cannot hook: failed to match IL instructions (first set)");
#if DEBUG
            Plugin.Logger.LogDebug(il.ToString());
#endif
        }


#if OLD
        [HarmonyPrefix, HarmonyPatch(typeof(HealthComponent), nameof(HealthComponent.TakeDamage))]
        private static void HealthComponent_TakeDamage_NonLethalToPlayers(HealthComponent __instance, ref DamageInfo damageInfo)
        {
            if (__instance.body.isPlayerControlled && _active) {
                try {
                    MakeNonLethal(damageInfo);
                }
                catch (System.MissingFieldException) {
                    object combo = DamageTypeCombo_Field.GetValue(damageInfo);
                    DamageType value = (DamageType)DamageType_Field.GetValue(combo);
                    DamageType_Field.SetValue(combo, value |= DamageType.NonLethal);
                    DamageTypeCombo_Field.SetValue(damageInfo, combo);
                }
            }
        }

        // https://stackoverflow.com/questions/3546580/why-is-it-not-possible-to-catch-missingmethodexception/3546611#3546611
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void MakeNonLethal(DamageInfo damageInfo) => damageInfo.damageType |= DamageType.NonLethal;

        private static System.Reflection.FieldInfo _DamageType_Field;
        private static System.Reflection.FieldInfo DamageType_Field {
            get {
                if (_DamageType_Field == null) {
                    Plugin.Logger.LogWarning($"{nameof(System.MissingFieldException)}: Using Seekers of the Storm version of {nameof(DamageInfo)}.{nameof(DamageInfo.damageType)}");
                    // DamageInfo -> DamageTypeCombo -> DamageType
                    _DamageType_Field = DamageTypeCombo_Field.FieldType.GetField(nameof(DamageInfo.damageType));
                }
                return _DamageType_Field;
            }
        }

        private static System.Reflection.FieldInfo _DamageTypeCombo_Field;
        private static System.Reflection.FieldInfo DamageTypeCombo_Field {
            get {
                if (_DamageTypeCombo_Field == null) {
                    _DamageTypeCombo_Field = typeof(DamageInfo).GetField(nameof(DamageInfo.damageType));
                }
                return _DamageTypeCombo_Field;
            }
        }
#endif
    }
}
