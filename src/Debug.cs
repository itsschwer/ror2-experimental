#if DEBUG
using RoR2;
using RoR2.Navigation;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Experimental
{
    public static class Debug
    {
        internal static void EternalGhost(Inventory inventory)
        {
            if (inventory.GetItemCount(RoR2Content.Items.Ghost) <= 0) return;
            if (inventory.GetItemCount(RoR2Content.Items.ExtraLifeConsumed) > 0
             && inventory.GetItemCount(RoR2Content.Items.ExtraLife) <= 0) {
                inventory.GiveItem(RoR2Content.Items.ExtraLife.itemIndex);
            }
        }


        public static CharacterMaster SpawnJellyfish(CharacterBody body, bool eternal = true)
        {
            CharacterSpawnCard card = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Jellyfish/cscJellyfish.asset").WaitForCompletion();
            return SpawnCharacter(body, card, eternal);
        }

        public static CharacterMaster SpawnCharacter(CharacterBody body, CharacterSpawnCard card, bool eternal = true)
        {
            CharacterMaster master = SpawnNearBody(card, body)?.GetComponent<CharacterMaster>();
            if (master == null) return null;

            if (eternal) {
                master.inventory?.GiveItem(RoR2Content.Items.ExtraLife.itemIndex);
                master.inventory?.GiveItem(RoR2Content.Items.Ghost);
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cWorldEvent>An eternal {master.GetBody()?.GetDisplayName()} joins the fray...</style>" });
            }

            return master;
        }


        public static void SpawnScrapper(CharacterBody body) => SpawnAtBody(LoadInteractableSpawnCard("RoR2/Base/Scrapper/iscScrapper.asset"), body);
        public static void SpawnPrinter(CharacterBody body) => SpawnAtBody(LoadInteractableSpawnCard("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset"), body);
        public static void SpawnCauldron(CharacterBody body) => SpawnAtBody(CreateCauldronSpawnCard(), body);


        public static GameObject SpawnAtBody(SpawnCard spawnCard, CharacterBody body, TeamIndex teamIndexOverride = TeamIndex.Void)
        {
            DirectorPlacementRule placement = new DirectorPlacementRule {
                position = body.footPosition,
                placementMode = DirectorPlacementRule.PlacementMode.Direct
            };
            DirectorCore.GetMonsterSpawnDistance(DirectorCore.MonsterSpawnDistance.Standard, out placement.minDistance, out placement.maxDistance);
            GameObject obj = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placement, RoR2Application.rng) { teamIndexOverride = (teamIndexOverride != TeamIndex.None) ? teamIndexOverride : body.master.teamIndex });
            return obj;
        }

        public static GameObject SpawnNearBody(SpawnCard spawnCard, CharacterBody body, TeamIndex teamIndexOverride = TeamIndex.None)
        {
            DirectorPlacementRule placement = new DirectorPlacementRule {
                position = body.footPosition,
                placementMode = DirectorPlacementRule.PlacementMode.Approximate
            };
            DirectorCore.GetMonsterSpawnDistance(DirectorCore.MonsterSpawnDistance.Close, out placement.minDistance, out placement.maxDistance);
            GameObject obj = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placement, RoR2Application.rng) { teamIndexOverride = (teamIndexOverride != TeamIndex.None) ? teamIndexOverride: body.master.teamIndex });
            return obj;
        }


        // https://github.com/Goorakh/RiskOfChaos/blob/149f6e103588a66ae83c5539f6f778fd2d405915/RiskOfChaos/EffectDefinitions/World/Spawn/SpawnRandomInteractable.cs#L23-L35
        public static InteractableSpawnCard LoadInteractableSpawnCard(string assetPath)
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
        public static InteractableSpawnCard CreateCauldronSpawnCard(string assetPath = "RoR2/Base/LunarCauldrons/LunarCauldron, GreenToRed Variant.prefab")
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
