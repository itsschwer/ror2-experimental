using MonoMod.Cil;
using Mono.Cecil.Cil;
using HarmonyLib;
using RoR2;

namespace Experimental.Patches
{
    [HarmonyPatch]
    internal static class NoDropPods
    {
        [HarmonyILManipulator, HarmonyPatch(typeof(Run), nameof(Run.spawnWithPod), MethodType.Getter)]
        private static void Run_spawnWithPod(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool matched = c.TryGotoNext(
                x => x.MatchCall<Run>($"get_{nameof(Run.instance)}"),
                x => x.MatchLdfld<Run>(nameof(Run.stageClearCount)),
                x => x.MatchLdcI4(0),
                x => x.MatchCeq(),
                x => x.MatchRet()
            );

            if (matched) {
                c.RemoveRange(4);
                c.Emit(OpCodes.Ldc_I4_0);

#if DEBUG
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(NoDropPods)}> Cannot hook {nameof(Run_spawnWithPod)}: failed to match IL instructions.");
        }
    }
}
