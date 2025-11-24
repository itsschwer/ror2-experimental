using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Vanilla = RoR2.Orbs;
using System;

namespace itsschwer.RoR2.NRE.IL
{
    [HarmonyPatch]
    internal static class VineOrb
    {
        /// <summary>
        /// Uncertain trigger.
        /// </summary>
        /// <remarks>
        /// <code>
        /// [Error  : Unity Log] NullReferenceException
        /// Stack trace:
        /// RoR2.Orbs.VineOrb.OnArrival() (at<f06ee9a3ef5741e1a8136dd7fb5aa0d7>:IL_0067)
        /// RoR2.Orbs.OrbManager.FixedUpdate() (at<f06ee9a3ef5741e1a8136dd7fb5aa0d7>:IL_00A3)
        /// </code>
        /// </remarks>
        [HarmonyILManipulator, HarmonyPatch(typeof(Vanilla.VineOrb), nameof(Vanilla.VineOrb.OnArrival))]
        private static void VineOrb_OnArrival(ILContext il)
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
            else Plugin.Logger.LogError($"{nameof(VineOrb_OnArrival)}> Cannot hook: failed to match IL instructions.");
        }
    }
}
