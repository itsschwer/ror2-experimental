using HarmonyLib;
using RoR2;

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
    }
}
