using BepInEx;
using EntityStates.Missions.GeodeSecretMission;
using HarmonyLib;
using RoR2;
using UnityEngine;

namespace GeodeSecretMissionRevives
{
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "GeodeSecretMissionRevives";
        public const string Version = "0.0.0";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            Harmony harmony = new Harmony(Info.Metadata.GUID);
            harmony.Patch(typeof(GeodeSecretMissionRewardState).GetMethod(nameof(GeodeSecretMissionRewardState.DropRewards)), postfix: new HarmonyMethod(typeof(Plugin).GetMethod(nameof(RewardRevive))));

            Logger.LogMessage("~awake.");
        }


        private static void RewardRevive(GeodeSecretMissionRewardState __instance)
        {
            Vector3 position = __instance.geodeSecretMissionController.rewardSpawnLocation.transform.position;
            Quaternion rotation = Quaternion.identity;

            foreach (NetworkUser user in NetworkUser.readOnlyInstancesList) {
                if (user.master.IsDeadAndOutOfLivesServer()) {
                    user.master.Respawn(position, rotation);
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
