using RoR2;
using UnityEngine;

namespace Experimental
{
    public static class SpawnMonster
    {
        public const string Beetle = "RoR2/Base/Beetle/cscBeetle.asset";
        public const string LesserWisp = "RoR2/Base/Wisp/cscLesserWisp.asset";
        public const string Bell = "RoR2/Base/Bell/cscBell.asset";
        public const string Templar = "RoR2/Base/ClayBruiser/cscClayBruiser.asset";
        public const string Golem = "RoR2/Base/Golem/cscGolemNature.asset";
        public const string Titan = "RoR2/Base/Titan/cscTitanGolemPlains.asset";
        public const string ImpOverlord = "RoR2/Base/ImpBoss/cscImpBoss.asset";
        public const string AlloyWorshipUnit = "RoR2/Base/RoboBallBoss/cscSuperRoboBallBoss.asset";
        public const string OverloadingWorm = "RoR2/Base/ElectricWorm/cscElectricWorm.asset";

        public static CharacterSpawnCard Get(string assetPath)
        {
            return UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<CharacterSpawnCard>(assetPath).WaitForCompletion();
        }

        public static GameObject Spawn(SpawnCard spawnCard, CharacterBody target, DirectorPlacementRule.PlacementMode placementMode)
        {
            DirectorPlacementRule placement = new DirectorPlacementRule {
                position = target.footPosition,
                placementMode = placementMode
            };
            DirectorCore.GetMonsterSpawnDistance(
                DirectorCore.MonsterSpawnDistance.Close,
                out placement.minDistance, out placement.maxDistance);
            DirectorSpawnRequest request = new DirectorSpawnRequest(spawnCard, placement, RoR2Application.rng) {
                teamIndexOverride = TeamIndex.Lunar,
                ignoreTeamMemberLimit = true
            };
            GameObject obj = DirectorCore.instance.TrySpawnObject(request);
            return obj;
        }

        public static CharacterMaster RemoveAI(CharacterMaster master)
        {
            // https://github.com/harbingerofme/DebugToolkit/blob/master/Code/DT-Commands/Spawners.cs#L287-L294
            foreach (RoR2.CharacterAI.BaseAI ai in master.aiComponents) {
                Object.Destroy(ai);
            }
            master.aiComponents = System.Array.Empty<RoR2.CharacterAI.BaseAI>();

            return master;
        }



        public static void SpawnVoidTitan(CharacterBody target)
        {
            GameObject obj = Spawn(Get(Titan), target, DirectorPlacementRule.PlacementMode.Approximate);
            CharacterMaster master = RemoveAI(obj.GetComponent<CharacterMaster>());
            master.inventory.SetEquipmentIndex(DLC1Content.Equipment.EliteVoidEquipment.equipmentIndex);
            master.teamIndex = TeamIndex.Void;
        }
    }
}
