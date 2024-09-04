using PressureDrop;
using RoR2;
using System.Linq;

namespace Experimental.Debugging
{
    internal static class Stage
    {
        internal static SceneDef setStage;

        private static SceneDef[] _stages;
        private static SceneDef[] Stages {
            get {
                return _stages ??= SceneCatalog.allSceneDefs
                    .Where(def =>
                        def.sceneType != SceneType.Menu &&
                        def.sceneType != SceneType.Cutscene)
                    .ToArray();
            }
        }

        public static void ForceStage(SceneDef scene)
        {
            if (Run.instance == null) return;

            Run.instance.GenerateStageRNG();
            UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene(scene.cachedName);
        }

        internal static void SetStage(NetworkUser user, string[] args)
        {
            if (args.Length == 2) {
                SceneDef chosen = null;
                for (int i = 0; i < Stages.Length && !chosen; i++) {
                    if (Stages[i].cachedName == args[1]) {
                        chosen = Stages[i];
                    }
                }
                if (chosen != null) {
                    setStage = chosen;
                }
                else ChatCommander.OutputFail(args[0], "invalid scene name.");
            }
            else ChatCommander.OutputFail(args[0], "<stage-scene-name>");
        }

        public static string GetDisplayName(SceneDef scene)
            => scene ? $"{scene.cachedName} ({Language.GetString(scene.nameToken)})" : "none";

        public static string StyleStageNamesForShow()
        {
            System.Text.StringBuilder sb = new();

            string[] styles = ["cIsHealth", "cIsDamage"];
            for (int i = 0; i < Stages.Length; i++) {
                sb.Append($"<style={styles[i % styles.Length]}>{GetDisplayName(Stages[i])}</style> ");
            }

            return sb.ToString();
        }
    }
}
