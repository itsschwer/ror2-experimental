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
            ChatCommander.Register("/s", Spawn);
            ChatCommander.Register("/i", SpawnCommandCube);
        }

        public static void Unregister()
        {
            ChatCommander.Unregister("/?", Help);
            ChatCommander.Unregister("/f", ForceStage);
            ChatCommander.Unregister("/ne", DisableEnemySpawns);
            ChatCommander.Unregister("/s", Spawn);
            ChatCommander.Unregister("/i", SpawnCommandCube);
        }

        private static void Help(NetworkUser user, string[] args)
        {
            ChatCommander.Output($"<style=cWorldEvent>{Plugin.GUID}</style> chat commands:");
            ChatCommander.Output($"  <style=cSub>/f</style>: forces a stage change.");
            ChatCommander.Output($"  <style=cSub>/ne</style>: toggles enemy spawns.");
            ChatCommander.Output($"  <style=cSub>/s</style>: spawns an object.");
            ChatCommander.Output($"  <style=cSub>/i</style>: spawns an Command Cube.");
        }

        private static bool GetUserBody(NetworkUser user, out CharacterBody body)
        {
            body = null;
            if (Run.instance == null) return false;
            body = user?.GetCurrentBody();
            return (body != null);
        }



        private static string[] _stages;
        private static string[] Stages {
            get {
                return _stages ??= SceneCatalog.allStageSceneDefs.Select(def => def.cachedName.ToLowerInvariant()).ToArray();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("BepInEx.Analyzers", "Publicizer001")] // Accessing a member that was not originally public
        private static void ForceStage(NetworkUser user, string[] args)
        {
            if (Run.instance == null) return;

            if (args.Length == 2) {
                args[1] = args[1].ToLowerInvariant();
                if (Stages.Contains(args[1])) {
                    Run.instance.GenerateStageRNG();
                    UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene(args[1]);
                    return;
                }
            }

            ChatCommander.OutputFail(args[0], "invalid scene name.");
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

        private static void Spawn(NetworkUser user, string[] args)
        {
            if (!GetUserBody(user, out CharacterBody target)) return;

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
                    case "e":
                    case "beetle":
                        SpawnCard card = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Beetle/cscBeetle.asset").WaitForCompletion();
                        Debug.SpawnAtBody(card, target, TeamIndex.Monster);
                        return;
                }
            }

            ChatCommander.OutputFail(args[0],
                "{ <style=cSub>scrapper</style> | <style=cSub>printer</style> | <style=cSub>cauldron</style> | <style=cSub>blueportal</style> }");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("BepInEx.Analyzers", "Publicizer001")] // Accessing a member that was not originally public
        private static void SpawnCommandCube(NetworkUser user, string[] args)
        {
            if (!GetUserBody(user, out CharacterBody target)) return;

            if (args.Length == 2) {
                System.Func<ItemDef, bool> predicate = null;
                switch (args[1].ToLowerInvariant()) {
                    default: break;
                    case "w":
                    case "white":
                        predicate = (def => def.tier == ItemTier.Tier1);
                        break;
                    case "g":
                    case "green":
                        predicate = (def => def.tier == ItemTier.Tier2);
                        break;
                    case "r":
                    case "red":
                        predicate = (def => def.tier == ItemTier.Tier3);
                        break;
                    case "y":
                    case "yellow":
                        predicate = (def => def.tier == ItemTier.Boss);
                        break;
                    case "l":
                    case "lunar":
                        predicate = (def => def.tier == ItemTier.Lunar);
                        break;
                    case "v":
                    case "void":
                        predicate = (def => PressureDrop.Drop.IsVoidTier(def.tier));
                        break;
                }
                if (predicate != null) {
                    UnityEngine.GameObject o = UnityEngine.Object.Instantiate(RoR2.Artifacts.CommandArtifactManager.commandCubePrefab, target.footPosition, UnityEngine.Quaternion.identity);
                    o.GetComponent<PickupPickerController>().SetOptionsInternal(Debug.GetPickupOptions(predicate));
                    UnityEngine.Networking.NetworkServer.Spawn(o);
                    return;
                }
            }

            ChatCommander.OutputFail(args[0],
                "{ <style=cSub>white</style> | <style=cSub>green</style> | <style=cSub>red</style> | <style=cSub>yellow</style> | <style=cSub>lunar</style> | <style=cSub>void</style> }");
        }
    }
}
#endif
