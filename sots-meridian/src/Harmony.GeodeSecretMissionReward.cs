using EntityStates.Missions.GeodeSecretMission;
using HarmonyLib;
using RoR2;
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

            RelocateBrokenDrones(position, 10);
        }

        private static void LeashMinions(CharacterMaster master, Vector3 position, float radius, Quaternion rotation)
        {
            MinionOwnership.MinionGroup group = MinionOwnership.MinionGroup.FindGroup(master.netId);
            if (group != null && group.members != null) {
                int leashed = 0;
                foreach (MinionOwnership minion in group.members) {
                    CharacterBody body = minion?.GetComponent<CharacterMaster>()?.GetBody();
                    if (body != null) {
                        Vector2 offset = Random.insideUnitCircle * radius;
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
            int drones = 0;
            foreach (PurchaseInteraction purchaseInteraction in InstanceTracker.GetInstancesList<PurchaseInteraction>()) {
                if (purchaseInteraction.displayNameToken.Contains("DRONE")) {
                    Vector2 offset = Random.insideUnitCircle * radius;
                    purchaseInteraction.transform.position = position + new Vector3(offset.x, 0, offset.y);
                    drones++;
                }
            }
            Plugin.Logger.LogDebug($"Relocated {drones} broken drone(s)");
        }
    }
}
