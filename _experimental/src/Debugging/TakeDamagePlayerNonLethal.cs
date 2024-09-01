#if DEBUG
using HarmonyLib;
using RoR2;

namespace Experimental.Debugging
{
    [HarmonyPatch]
    internal static class TakeDamagePlayerNonLethal
    {
        [HarmonyPrefix, HarmonyPatch(typeof(HealthComponent), nameof(HealthComponent.TakeDamage))]
        private static void HealthComponent_TakeDamage_NonLethalToPlayers(HealthComponent __instance, ref DamageInfo damageInfo)
        {
            if (__instance.body.isPlayerControlled) {
                try {
                    MakeNonLethal(damageInfo);
                }
                catch (System.MissingFieldException) {
                    // DamageType value = (DamageType)damageType.GetValue(damageInfo);
                    // damageType.SetValue(damageInfo, value |= DamageType.NonLethal);
                    var combo = damageTypeCombo.GetValue(damageInfo);
                    var value = (DamageType)damageType.GetValue(combo);
                    damageType.SetValue(combo, value |= DamageType.NonLethal);
                    damageTypeCombo.SetValue(damageInfo, combo);
                    Plugin.Logger.LogWarning($"{value} | {(DamageType)damageType.GetValue(combo)} | {(DamageType)damageType.GetValue(damageTypeCombo.GetValue(damageInfo))}");
                }
            }
        }

        // https://stackoverflow.com/questions/3546580/why-is-it-not-possible-to-catch-missingmethodexception/3546611#3546611
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void MakeNonLethal(DamageInfo damageInfo) => damageInfo.damageType |= DamageType.NonLethal;

        private static System.Reflection.FieldInfo _damageType;
        private static System.Reflection.FieldInfo damageType {
            get {
                if (_damageType == null) {
                    Plugin.Logger.LogWarning($"{nameof(System.MissingFieldException)}: Using Seekers of the Storm version of {nameof(DamageInfo)}.{nameof(DamageInfo.damageType)}");
                    // DamageInfo -> DamageTypeCombo -> DamageType
                    _damageType = _damageTypeCombo.FieldType.GetField(nameof(DamageInfo.damageType));
                }
                return _damageType;
            }
        }

        private static System.Reflection.FieldInfo _damageTypeCombo;
        private static System.Reflection.FieldInfo damageTypeCombo {
            get {
                if (_damageTypeCombo == null) {
                    _damageTypeCombo = typeof(DamageInfo).GetField(nameof(DamageInfo.damageType));
                }
                return _damageTypeCombo;
            }
        }
    }
}
#endif
