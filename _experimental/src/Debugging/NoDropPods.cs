#if DEBUG
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace Experimental.Debugging
{
    internal static class NoDropPods
    {
        internal static void Apply()
        {
            System.Reflection.MethodInfo method = HarmonyLib.AccessTools.DeclaredPropertyGetter(typeof(bool), nameof(RoR2.Run.instance.spawnWithPod));
            ILHook hook = new(method, (il) => {
                ILCursor c = new(il);
                bool matched = c.TryGotoNext(
                    x => x.MatchCall<RoR2.Run>(nameof(RoR2.Run.instance)),
                    x => x.MatchLdfld<int>(nameof(RoR2.Run.stageClearCount)),
                    x => x.MatchLdcI4(0),
                    x => x.MatchCeq(),
                    x => x.MatchRet()
                );

                if (matched) {
                    c.RemoveRange(4);
                    c.Emit(OpCodes.Ldc_I4_1);

                    Plugin.Logger.LogInfo($"{nameof(NoDropPods)}: Applied ILHook to {method.DeclaringType.FullName}.{method.Name}");
                }
                else Plugin.Logger.LogWarning($"{nameof(NoDropPods)}: Failed to apply ILHook to {method.DeclaringType.FullName}.{method.Name}");
            });
        }
    }
}
#endif
