#if DEBUG
using HarmonyLib;
using RoR2;

namespace Experimental.Debugging
{
    [HarmonyPatch]
    internal static class HarmonyPatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(HealthComponent), nameof(HealthComponent.TakeDamage))]
        private static void HealthComponent_TakeDamage_NonLethalToPlayers(HealthComponent __instance, ref DamageInfo damageInfo)
        {
            if (__instance.body.isPlayerControlled) {
                damageInfo.damageType |= DamageType.NonLethal;
            }
        }
    }
}
#endif
