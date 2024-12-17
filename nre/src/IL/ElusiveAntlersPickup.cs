using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;
using Vanilla = RoR2;

namespace itsschwer.RoR2.NRE.IL
{
    [HarmonyPatch]
    internal static class ElusiveAntlersPickup
    {
        /// <summary>
        /// <code>
        /// [Error  : Unity Log] NullReferenceException
        /// Stack trace:
        /// RoR2.ElusiveAntlersPickup.FixedUpdate () (at <a43009bc6a5f4aee99e5521ef176a18d>:IL_003C)
        /// </code>
        /// </summary>
        [HarmonyILManipulator, HarmonyPatch(typeof(Vanilla.ElusiveAntlersPickup), nameof(Vanilla.ElusiveAntlersPickup.FixedUpdate))]
        private static void ElusiveAntlersPickup_FixedUpdate(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            // Vector3 position = ownerBody.gameObject.transform.position;
            Func<Instruction, bool>[] match = {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Vanilla.ElusiveAntlersPickup>(nameof(Vanilla.ElusiveAntlersPickup.ownerBody)),
                x => x.MatchCallOrCallvirt<Component>($"get_{nameof(Component.gameObject)}"),
                x => x.MatchCallOrCallvirt<GameObject>($"get_{nameof(GameObject.transform)}"),
                x => x.MatchCallOrCallvirt<Transform>($"get_{nameof(Transform.position)}"),
                x => x.MatchStloc(0)
            };

            if (c.TryGotoNext(match)) {
                c.RemoveRange(match.Length); // :p
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Vanilla.ElusiveAntlersPickup, Vector3>>((@this) => {
                    if (@this.ownerBody) return @this.ownerBody.gameObject.transform.position;

                    Vector3 outOfRange = @this.gameObject.transform.position;
                    outOfRange.z += @this.maxDistanceFromOwner + 1;
                    return outOfRange;
                });
#if DEBUG
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(ElusiveAntlersPickup_FixedUpdate)}> Cannot hook: failed to match IL instructions.");
        }
    }
}
