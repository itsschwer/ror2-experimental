using RoR2;
using RoR2.Navigation;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Linq;

namespace Experimental
{
    public static class Debug
    {
        public static void SpawnBluePortal(CharacterBody body)
            => SpawnAtBody(Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/PortalShop/iscShopPortal.asset").WaitForCompletion(), body);

        public static void SpawnEquipmentDrones(CharacterBody body)
        {
            InteractableSpawnCard card = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Drones/iscBrokenEquipmentDrone.asset").WaitForCompletion();
            InteractableSpawnCard[] cards = Enumerable.Repeat(card, 10).ToArray();

            Vector3 forward = body.inputBank.aimDirection;
            forward.y = 0;
            forward.Normalize();
            forward *= 5;

            SpawnInteractables(cards, body.footPosition, forward, 360);
        }

        public static void SpawnItemCostInteractables(CharacterBody body)
        {
            InteractableSpawnCard[] spawnCards = [
                CreateCauldronSpawnCard("RoR2/Base/LunarCauldrons/LunarCauldron, RedToWhite Variant.prefab"),
                CreateCauldronSpawnCard("RoR2/Base/LunarCauldrons/LunarCauldron, WhiteToGreen.prefab"),
                CreateCauldronSpawnCard(),
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineCleanse/iscShrineCleanseSnowy.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Scrapper/iscScrapper.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Duplicator/iscDuplicator.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorMilitary/iscDuplicatorMilitary.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorWild/iscDuplicatorWild.asset").WaitForCompletion()
            ];

            Vector3 forward = body.inputBank.aimDirection;
            forward.y = 0;
            forward.Normalize();
            forward *= 5;

            SpawnInteractables(spawnCards, body.footPosition, forward, 360);
        }

        public static void SpawnDamagingInteractables(CharacterBody body)
        {
            InteractableSpawnCard[] spawnCards = [
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/VoidChest/iscVoidChest.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineBlood/iscShrineBloodSnowy.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/VoidTriple/iscVoidTriple.asset").WaitForCompletion()
            ];

            Vector3 forward = body.inputBank.aimDirection;
            forward.y = 0;
            forward.Normalize();
            forward *= 4;

            SpawnInteractables(spawnCards, body.footPosition, forward, 135);
        }

        public static void SpawnInteractables(InteractableSpawnCard[] spawnCards, Vector3 position, Vector3 forward, float arcDegrees)
        {
            float angle = arcDegrees / spawnCards.Length;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 offset = Quaternion.AngleAxis(-arcDegrees / 2, Vector3.up) * forward;

            for (int i = 0; i < spawnCards.Length; i++) {
                try {
                    Spawn(spawnCards[i], position + offset);
                }
                catch {
                    Plugin.Logger.LogError($"{i} | {spawnCards[i]}");
                }
                offset = rotation * offset;
            }
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
