using HarmonyLib;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Experimental
{
    [HarmonyPatch]
    internal static class VoidPod
    {
        [HarmonyPostfix, HarmonyPatch(typeof(CharacterBody), nameof(CharacterBody.Awake))]
        private static void ReplacePreferredPodPrefab(CharacterBody __instance)
        {
            GameObject prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBody.prefab").WaitForCompletion();
            CharacterBody body = prefab?.GetComponent<CharacterBody>();
            // __instance.preferredPodPrefab = body.preferredPodPrefab;
            __instance.preferredPodPrefab = null;
        }
    }
}
