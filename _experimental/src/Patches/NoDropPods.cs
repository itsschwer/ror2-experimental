using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Reflection;

namespace Experimental.Patches
{
    internal static class NoDropPods
    {
        internal static void Apply()
        {
            MethodInfo method = typeof(RoR2.Run).GetProperty(nameof(RoR2.Run.spawnWithPod)).GetGetMethod();
            ILHook hook = new(method, (il) => {
                ILCursor c = new(il);
                bool matched = c.TryGotoNext(
                    x => x.MatchCall<RoR2.Run>($"get_{nameof(RoR2.Run.instance)}"),
                    x => x.MatchLdfld<RoR2.Run>(nameof(RoR2.Run.stageClearCount)),
                    x => x.MatchLdcI4(0),
                    x => x.MatchCeq(),
                    x => x.MatchRet()
                );

                if (matched) {
                    c.RemoveRange(4);
                    c.Emit(OpCodes.Ldc_I4_0);

                    Plugin.Logger.LogInfo($"{nameof(NoDropPods)}: Applied ILHook to {method.DeclaringType.FullName}.{method.Name}");
                }
                else Plugin.Logger.LogWarning($"{nameof(NoDropPods)}: Failed to apply ILHook to {method.DeclaringType.FullName}.{method.Name}");
            });
        }
    }
}
