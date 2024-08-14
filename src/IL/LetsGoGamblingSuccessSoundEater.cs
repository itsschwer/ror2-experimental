using LetsGoGambling;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace Eater.IL
{
    internal static class LetsGoGamblingSuccessSoundEater
    {
        internal static void Apply()
        {
            if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LetsGoGamblingPlugin.MODUID)) return;

            MethodInfo method = typeof(LetsGoGamblingPlugin).GetMethod("ShrineChanceBehavior_AddShrineStack", BindingFlags.Instance | BindingFlags.NonPublic);
            ILHook hook = new(method, (il) => {
                ILCursor c = new(il);
                c.GotoNext(
                    // if (successfulPurchaseCount != successfulPurchaseCount2)
                    //     NetMessageExtensions.Send((INetMessage)(object)new EmitSoundAtPoint(853644935u, self.gameObject.transform.position), (NetworkDestination)1);
                    x => x.MatchLdcI4(853644935) // IL_002e
                );
                c.RemoveRange(9); // Skip to end of if block (past IL_004f)

                Log.Info($"Applied ILHook to {method.DeclaringType.FullName}.{method.Name}");
            });
        }
    }
}
