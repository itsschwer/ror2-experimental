#if DEBUG
using RoR2;
using RoR2.Navigation;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Experimental
{
    public static class Debug
    {
        public static void SpawnScrapper(CharacterBody body)
            => SpawnAtBody(Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Scrapper/iscScrapper.asset").WaitForCompletion(), body);
        public static void SpawnPrinter(CharacterBody body)
            => SpawnAtBody(Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset").WaitForCompletion(), body);
        public static void SpawnCauldron(CharacterBody body)
            => SpawnAtBody(CreateCauldronSpawnCard(), body);
        public static void SpawnBluePortal(CharacterBody body)
            => SpawnAtBody(Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/PortalShop/iscShopPortal.asset").WaitForCompletion(), body);

        public static void SpawnDamagingInteractables(CharacterBody body)
        {
            const string shrineBlood = "RoR2/Base/ShrineBlood/iscShrineBloodSnowy.asset";
            const string voidChest = "RoR2/DLC1/VoidChest/iscVoidChest.asset";
            const string voidPotential = "RoR2/DLC1/VoidTriple/iscVoidTriple.asset";

            const float angle = 135f / 3f;

            Vector3 forward = body.inputBank.aimDirection;
            forward.y = 0;
            forward.Normalize();
            forward *= 4;

            Spawn(Addressables.LoadAssetAsync<InteractableSpawnCard>(shrineBlood).WaitForCompletion(), body.footPosition + forward);
            Spawn(Addressables.LoadAssetAsync<InteractableSpawnCard>(voidChest).WaitForCompletion(), body.footPosition + (Quaternion.AngleAxis(angle, Vector3.up) * forward));
            Spawn(Addressables.LoadAssetAsync<InteractableSpawnCard>(voidPotential).WaitForCompletion(), body.footPosition + (Quaternion.AngleAxis(-angle, Vector3.up) * forward));
        }

        public static GameObject Spawn(SpawnCard spawnCard, Vector3 position)
        {
            DirectorPlacementRule placement = new DirectorPlacementRule {
                position = position,
                placementMode = DirectorPlacementRule.PlacementMode.Direct
            };
            GameObject obj = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placement, RoR2Application.rng));
            return obj;
        }

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
