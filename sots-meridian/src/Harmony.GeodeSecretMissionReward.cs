using EntityStates.Missions.GeodeSecretMission;
using HarmonyLib;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeridianPrimePrime.Harmony
{
    [HarmonyPatch]
    internal static class GeodeSecretMissionReward
    {
        [HarmonyPostfix, HarmonyPatch(typeof(GeodeSecretMissionRewardState), nameof(GeodeSecretMissionRewardState.DropRewards))]
        private static void GeodeSecretMissionRewardState_DropRewards(GeodeSecretMissionRewardState __instance)
        {
            Vector3 position = __instance.geodeSecretMissionController.rewardSpawnLocation.transform.position;
            Quaternion rotation = Quaternion.identity;
            Plugin.Logger.LogDebug($"{nameof(GeodeSecretMissionReward)} @ {position}");

            foreach (NetworkUser user in NetworkUser.readOnlyInstancesList) {
                if (user.master.IsDeadAndOutOfLivesServer()) {
                    user.master.Respawn(position, rotation);
                }

                try {
                    LeashMinions(user.master, position, 10, rotation);
                }
                catch (System.Exception e) {
                    Plugin.Logger.LogError(e);
                }
            }

            const float floorOffset = 2.6f; // Obtained by comparing survivor footPosition and rewardSpawnLocation
            RelocateBrokenDrones(position + Vector3.down * floorOffset, 6);
        }

        private static void LeashMinions(CharacterMaster master, Vector3 position, float withinRadius, Quaternion rotation)
        {
            MinionOwnership.MinionGroup group = MinionOwnership.MinionGroup.FindGroup(master.netId);
            if (group != null && group.members != null) {
                int leashed = 0;
                foreach (MinionOwnership minion in group.members) {
                    CharacterBody body = minion?.GetComponent<CharacterMaster>()?.GetBody();
                    if (body != null) {
                        Vector2 offset = Random.insideUnitCircle * withinRadius;
                        // Logic from RoR2.Items.MinionLeashBodyBehaviour
                        TeleportHelper.TeleportBody(body, position + new Vector3(offset.x, 0, offset.y));
                        GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(body.gameObject);
                        if (teleportEffectPrefab != null) {
                            EffectManager.SimpleEffect(teleportEffectPrefab, position, rotation, true);
                        }
                        leashed++;
                    }
                }
                Plugin.Logger.LogDebug($"Leash-teleported {leashed} minion(s) for {master.name} (out of {group.members.Length})");
            }
        }

        private static void RelocateBrokenDrones(Vector3 position, float radius)
        {
            IList<PurchaseInteraction> brokenDrones = InstanceTracker.GetInstancesList<PurchaseInteraction>().Where((purchaseInteraction) => purchaseInteraction.displayNameToken.Contains("DRONE")).ToList();
            if (brokenDrones.Count <= 0) return;

            float angle = 360f / brokenDrones.Count;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 offset = Vector3.forward * radius;
            foreach (PurchaseInteraction drone in brokenDrones) {
                drone.transform.position = position + offset;
                offset = rotation * offset;
            }
            Plugin.Logger.LogDebug($"Relocated {brokenDrones.Count} broken drone(s)");
        }
    }
}
