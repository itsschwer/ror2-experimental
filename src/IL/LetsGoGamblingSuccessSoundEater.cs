using LetsGoGambling;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace Eater.IL
{
    internal static class LetsGoGamblingSuccessSoundEater
    {
        private const int OPCODE_COUNT = 9;

        private const int gambling_awdangit = 51628376;
        private const int gambling_awyeahyeah = 853644935;
        private const int gambling_cantstopwinning = 444218535;

        internal static void Apply()
        {
            if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LetsGoGamblingPlugin.MODUID)) return;

            MethodInfo method = typeof(LetsGoGamblingPlugin).GetMethod("ShrineChanceBehavior_AddShrineStack", BindingFlags.Instance | BindingFlags.NonPublic);
            ILHook hook = new(method, (il) => {
                ILCursor c = new(il);

                // NetMessageExtensions.Send((INetMessage)(object)new EmitSoundAtPoint(444218535u, self.gameObject.transform.position), (NetworkDestination)1);
                c.GotoNext(
                    x => x.MatchLdcI4(gambling_cantstopwinning)
                ).RemoveRange(OPCODE_COUNT);

                // NetMessageExtensions.Send((INetMessage)(object)new EmitSoundAtPoint(853644935u, self.gameObject.transform.position), (NetworkDestination)1);
                c.GotoNext(
                    x => x.MatchLdcI4(gambling_awyeahyeah)
                ).RemoveRange(OPCODE_COUNT);

                Log.Info($"Applied ILHook to {method.DeclaringType.FullName}.{method.Name}");
            });
        }
    }
}
