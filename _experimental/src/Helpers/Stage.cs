using PressureDrop;
using RoR2;
using System.Linq;

namespace Experimental.Helpers
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

        internal static void SetStage(NetworkUser _, string[] args)
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
                else if (args[1] == "clear") {
                    setStage = null;
                }
                else ChatCommander.OutputFail(args[0], "invalid scene name.");
            }
            else ChatCommander.OutputFail(args[0], "(<stage-scene-name> | clear)");
        }

        public static string GetDisplayName(SceneDef scene)
            => scene ? $"{scene.cachedName} ({Language.GetString(scene.nameToken)})" : "none";

        public static string GetDisplayName(SceneDef scene, string internalNameStyle, string displayNameStyle)
            => scene ? $"<style={internalNameStyle}>{scene.cachedName} (<style={displayNameStyle}>{Language.GetString(scene.nameToken)}</style>)</style>" : "none";

        public static string DumpStyledDisplayNames()
        {
            System.Text.StringBuilder sb = new();

            for (int i = 0; i < Stages.Length; i++) {
                sb.Append($"{GetDisplayName(Stages[i], "cIsDamage", "cIsHealth")} ");
            }

            return sb.ToString();
        }
    }
}
