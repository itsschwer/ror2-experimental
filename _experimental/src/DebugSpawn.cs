﻿#if DEBUG
using RoR2;
using RoR2.Navigation;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Experimental
{
    public static class Debug
    {
        public static void SpawnScrapper(CharacterBody body)
            => SpawnAtBody(LoadInteractableSpawnCard("RoR2/Base/Scrapper/iscScrapper.asset"), body);
        public static void SpawnPrinter(CharacterBody body)
            => SpawnAtBody(LoadInteractableSpawnCard("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset"), body);
        public static void SpawnCauldron(CharacterBody body)
            => SpawnAtBody(CreateCauldronSpawnCard(), body);
        public static void SpawnBluePortal(CharacterBody body)
            => SpawnAtBody(Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/PortalShop/iscShopPortal.asset").WaitForCompletion(), body);


        public static GameObject SpawnAtBody(SpawnCard spawnCard, CharacterBody body, TeamIndex? teamIndexOverride = null)
        {
            DirectorPlacementRule placement = new DirectorPlacementRule {
                position = body.footPosition,
                placementMode = DirectorPlacementRule.PlacementMode.Direct
            };
            DirectorCore.GetMonsterSpawnDistance(DirectorCore.MonsterSpawnDistance.Standard,
                out placement.minDistance, out placement.maxDistance);
            GameObject obj = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placement, RoR2Application.rng) {
                teamIndexOverride = teamIndexOverride ?? body.master.teamIndex });
            return obj;
        }

        // https://github.com/Goorakh/RiskOfChaos/blob/149f6e103588a66ae83c5539f6f778fd2d405915/RiskOfChaos/EffectDefinitions/World/Spawn/SpawnRandomInteractable.cs#L23-L35
        private static InteractableSpawnCard LoadInteractableSpawnCard(string assetPath)
        {
            InteractableSpawnCard spawn = Addressables.LoadAssetAsync<InteractableSpawnCard>(assetPath).WaitForCompletion();
            // Ignore Artifact of Sacrifice
            if (spawn.skipSpawnWhenSacrificeArtifactEnabled) {
                // Create modified copy
                spawn = ScriptableObject.Instantiate(spawn);
                spawn.skipSpawnWhenSacrificeArtifactEnabled = false;
            }

            return spawn;
        }

        // https://github.com/Goorakh/RiskOfChaos/blob/149f6e103588a66ae83c5539f6f778fd2d405915/RiskOfChaos/EffectDefinitions/World/Spawn/SpawnRandomInteractable.cs#L47-L63
        private static InteractableSpawnCard CreateCauldronSpawnCard(string assetPath = "RoR2/Base/LunarCauldrons/LunarCauldron, GreenToRed Variant.prefab")
        {
            int lastSlashIndex = assetPath.LastIndexOf('/');
            string cardName = assetPath.Substring(lastSlashIndex + 1, assetPath.LastIndexOf('.') - lastSlashIndex - 1);

            InteractableSpawnCard spawn = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            spawn.prefab = Addressables.LoadAssetAsync<GameObject>(assetPath).WaitForCompletion();
            spawn.name = cardName;
            spawn.orientToFloor = true;
            spawn.hullSize = HullClassification.Golem;
            spawn.requiredFlags = NodeFlags.None;
            spawn.forbiddenFlags = NodeFlags.NoChestSpawn;
            spawn.occupyPosition = true;
            spawn.sendOverNetwork = true;

            return spawn;
        }
    }
}
#endif