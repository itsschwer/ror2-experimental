#if DEBUG
using RoR2;
using PressureDrop;
using UnityEngine.AddressableAssets;
using System.Linq;

namespace Experimental
{
    internal static class Commands
    {
        public static void Register()
        {
            ChatCommander.Register("/?", Help);
            ChatCommander.Register("/f", ForceStage);
            ChatCommander.Register("/ne", DisableEnemySpawns);
            ChatCommander.Register("/i", SpawnInteractable);
            ChatCommander.Register("/b", SpawnBeetle);
            ChatCommander.Register("/lepton", SpawnLeptonDrop);
        }

        public static void Unregister()
        {
            ChatCommander.Unregister("/?", Help);
            ChatCommander.Unregister("/f", ForceStage);
            ChatCommander.Unregister("/ne", DisableEnemySpawns);
            ChatCommander.Unregister("/i", SpawnInteractable);
            ChatCommander.Unregister("/b", SpawnBeetle);
            ChatCommander.Unregister("/lepton", SpawnLeptonDrop);
        }

        private static void Help(NetworkUser user, string[] args)
        {
            ChatCommander.Output($"<style=cWorldEvent>{Plugin.GUID}</style> chat commands:");
            ChatCommander.Output($"  <style=cSub>/f</style>: forces a stage change.");
            ChatCommander.Output($"  <style=cSub>/ne</style>: toggles enemy spawns.");
            ChatCommander.Output($"  <style=cSub>/i</style>: spawns an interactable.");
            ChatCommander.Output($"  <style=cSub>/b</style>: spawns an enemy Beetle.");
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("BepInEx.Analyzers", "Publicizer001")] // Accessing a member that was not originally public
        private static void ForceStage(NetworkUser user, string[] args)
        {
            if (Run.instance == null) return;
            if (args.Length == 2) {
                string scene = null;
                args[1] = args[1].ToLowerInvariant();
                if (Stages.Contains(args[1])) scene = args[1];
                else if (args[1] == "aq") scene = "goolake";
                else if (args[1] == "dr") scene = "blackbeach";

                if (!string.IsNullOrWhiteSpace(scene)) {
                    Run.instance.GenerateStageRNG();
                    UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene(scene);
                    return;
                }
            }

            ChatCommander.OutputFail(args[0], "{ <style=cSub>aq</style> | <style=cSub>dr</style> }");
        }

        private static void DisableEnemySpawns(NetworkUser user, string[] args)
        {
            if (args.Length == 1) {
                bool wasDisabled = CombatDirector.cvDirectorCombatDisable.GetString() != "0";
                CombatDirector.cvDirectorCombatDisable.SetBool(!wasDisabled);
                if (wasDisabled) ChatCommander.Output("Enemy spawns enabled.");
                else ChatCommander.Output("Enemy spawns disabled.");
            }
            else ChatCommander.OutputFail(args[0], "expects zero arguments.");
        }

        private static void SpawnBeetle(NetworkUser user, string[] args)
        {
            if (Run.instance == null) return;
            CharacterBody target = user?.GetCurrentBody();
            if (target == null) return;

            SpawnCard card = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Beetle/cscBeetle.asset").WaitForCompletion();
            Debug.SpawnAtBody(card, target, TeamIndex.Monster);
        }

        private static void SpawnLeptonDrop(NetworkUser user, string[] args)
        {
            if (Run.instance == null) return;
            CharacterBody target = user?.GetCurrentBody();
            if (target == null) return;

            int count = 0;
            if (args.Length > 1) int.TryParse(args[1], out count);
            if (count <= 0) count = 1;
            if (count > PressureDrop.Plugin.Config.MaxItemsToDropAtATime) count = PressureDrop.Plugin.Config.MaxItemsToDropAtATime;

            PickupIndex idx = PickupCatalog.FindPickupIndex(RoR2Content.Items.TPHealingNova.itemIndex);
            PressureDrop.Drop.DropStyleChest(target.gameObject.transform, idx, count, forwardOverride: PressureDrop.Drop.GetAimDirection(target));
        }

        private static void SpawnInteractable(NetworkUser user, string[] args)
        {
            if (Run.instance == null) return;
            CharacterBody target = user?.GetCurrentBody();
            if (target == null) return;

            if (args.Length == 2) {
                switch (args[1].ToLowerInvariant()) {
                    default: break;
                    case "s":
                    case "scrapper":
                        Debug.SpawnScrapper(target);
                        return;
                    case "p":
                    case "printer":
                        Debug.SpawnPrinter(target);
                        return;
                    case "c":
                    case "cauldron":
                    case "forge":
                        Debug.SpawnCauldron(target);
                        return;
                    case "b":
                    case "blue":
                    case "portal":
                    case "blueportal":
                        Debug.SpawnBluePortal(target);
                        return;
                    case "j":
                    case "jelly":
                        Debug.SpawnJellyfish(target);
                        return;
                }
            }

            ChatCommander.OutputFail(args[0],
                "{ <style=cSub>scrapper</style> | <style=cSub>printer</style> | <style=cSub>cauldron</style> | <style=cSub>blueportal</style> }");
        }




        private static string[] _stages;
        private static string[] Stages {
            get {
                return _stages ??= SceneCatalog.allStageSceneDefs.Select(def => def.cachedName.ToLowerInvariant()).ToArray();
            }
        }
    }
}
#endif
