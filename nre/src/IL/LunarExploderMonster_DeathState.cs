using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace itsschwer.RoR2.NRE.IL
{
    [HarmonyPatch]
    internal static class LunarExploderMonster_DeathState
    {
        /// <summary>
        /// Uncertain trigger, but will spam heavily until stage change.
        /// </summary>
        /// <remarks>
        /// <code>
        /// [Error: Unity Log] NullReferenceException
        /// Stack trace:
        /// EntityStates.LunarExploderMonster.DeathState.FixedUpdate() (at<f06ee9a3ef5741e1a8136dd7fb5aa0d7>:IL_0061)
        /// RoR2.EntityStateMachine.ManagedFixedUpdate(System.Single deltaTime) (at<f06ee9a3ef5741e1a8136dd7fb5aa0d7>:IL_001D)
        /// RoR2.EntityStateMachine.FixedUpdate() (at<f06ee9a3ef5741e1a8136dd7fb5aa0d7>:IL_0006)
        /// </code>
        /// </remarks>
        [HarmonyILManipulator, HarmonyPatch(typeof(EntityStates.LunarExploderMonster.DeathState), nameof(EntityStates.LunarExploderMonster.DeathState.FixedUpdate))]
        private static void LunarExploderMonster_DeathState_FixedUpdate(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            Func<Instruction, bool>[] match = {

            };

            if (c.TryGotoNext(match)) {
                //todo
#if DEBUG
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(LunarExploderMonster_DeathState_FixedUpdate)}> Cannot hook: failed to match IL instructions.");
        }
    }
}
