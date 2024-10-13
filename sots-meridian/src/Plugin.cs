using BepInEx;
using EntityStates.Missions.GeodeSecretMission;
using HarmonyLib;
using RoR2;
using UnityEngine;

namespace MeridianPrimePrime
{
    [HarmonyPatch]
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "MeridianPrimePrime";
        public const string Version = "0.0.0";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            new Harmony(Info.Metadata.GUID).PatchAll();

#if INCLUDE_UNUSED
            itsschwer.Junk.ShrineHalcyoniteObjective.Enable();
            itsschwer.Junk.ShuffleEliteTiers.Init();
#endif

            Logger.LogMessage("~awake.");
        }


        [HarmonyPostfix, HarmonyPatch(typeof(GeodeSecretMissionRewardState), nameof(GeodeSecretMissionRewardState.DropRewards))]
        private static void GeodeSecretMissionRewardState_DropRewards(GeodeSecretMissionRewardState __instance)
        {
            Vector3 position = __instance.geodeSecretMissionController.rewardSpawnLocation.transform.position;
            Quaternion rotation = Quaternion.identity;

            foreach (NetworkUser user in NetworkUser.readOnlyInstancesList) {
                if (user.master.IsDeadAndOutOfLivesServer()) {
                    user.master.Respawn(position, rotation);
                }

                MinionOwnership.MinionGroup group = MinionOwnership.MinionGroup.FindGroup(user.master.netId);
                if (group != null && group.members != null) {
                    foreach (MinionOwnership minion in group.members) {
                        CharacterBody body = minion?.GetComponent<CharacterMaster>()?.GetBody();
                        if (body != null) {
                            Vector2 offset = Random.insideUnitCircle * 10;
                            // Logic from RoR2.Items.MinionLeashBodyBehaviour
                            TeleportHelper.TeleportBody(body, position + new Vector3(offset.x, 0, offset.y));
                            GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(body.gameObject);
                            if (teleportEffectPrefab != null) {
                                EffectManager.SimpleEffect(teleportEffectPrefab, position, rotation, true);
                            }
                        }
                    }
                }
            }

            foreach (PurchaseInteraction purchaseInteraction in InstanceTracker.GetInstancesList<PurchaseInteraction>()) {
                if (purchaseInteraction.displayNameToken.Contains("DRONE")) {
                    Vector2 offset = Random.insideUnitCircle * 10;
                    purchaseInteraction.transform.position = position + new Vector3(offset.x, 0, offset.y);
                }
            }
        }
    }
}
