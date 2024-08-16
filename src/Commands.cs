#if DEBUG
using RoR2;
using PressureDrop;
using UnityEngine.AddressableAssets;
using System.Linq;
using Experimental.Debugging;

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

            if (WalkUI.DamageLogLoaded) ChatCommander.Register("/w", WalkUI.Walk);
        }

        public static void Unregister()
        {
            ChatCommander.Unregister("/?", Help);
            ChatCommander.Unregister("/f", ForceStage);
            ChatCommander.Unregister("/ne", DisableEnemySpawns);
            ChatCommander.Unregister("/s", Spawn);

            ChatCommander.Unregister("/w", WalkUI.Walk);
        }

        private static void Help(NetworkUser user, string[] args)
        {
            ChatCommander.Output($"<style=cWorldEvent>{Plugin.GUID}</style> chat commands:");
            ChatCommander.Output($"  <style=cSub>/f</style>: forces a stage change.");
            ChatCommander.Output($"  <style=cSub>/ne</style>: toggles enemy spawns.");
            ChatCommander.Output($"  <style=cSub>/s</style>: spawns an object.");
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
                }
            }

            ChatCommander.OutputFail(args[0],
                "{ <style=cSub>scrapper</style> | <style=cSub>printer</style> | <style=cSub>cauldron</style> | <style=cSub>blueportal</style> }");
        }
    }
}
#endif
