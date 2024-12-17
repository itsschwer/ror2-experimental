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
        // Consistent NRE on client for each pickup spawn
        // [Error  : Unity Log] NullReferenceException: Object reference not set to an instance of an object
        // Stack trace:
        // RoR2.ElusiveAntlersPickup.Start () (at <a43009bc6a5f4aee99e5521ef176a18d>:IL_0000)

        // Inconsistent NRE on host on stage change
        // [Error  : Unity Log] NullReferenceException
        // Stack trace:
        // UnityEngine.Component.GetComponent[T] () (at <a20b3695b7ce4017b7981f9d06962bd1>:IL_0021)
        // RoR2.CharacterBody.CallRpcOnShardDestroyedClient (RoR2.ElusiveAntlersPickup+ElusiveAntlersDestroyCondition pickupCondition) (at <a43009bc6a5f4aee99e5521ef176a18d>:IL_003A)
        // RoR2.CharacterBody.OnShardDestroyed (RoR2.ElusiveAntlersPickup+ElusiveAntlersDestroyCondition pickupCondition) (at <a43009bc6a5f4aee99e5521ef176a18d>:IL_0023)
        // RoR2.ElusiveAntlersPickup.OnDestroy () (at <a43009bc6a5f4aee99e5521ef176a18d>:IL_0007)


        /// <summary>
        /// Fixes vanilla NullReferenceException spam on host from Elusive Antlers pickups when the associated owner CharacterBody is destroyed (e.g. player death; starts when ragdoll destroyed?).
        /// </summary>
        /// <remarks>
        /// <code>
        /// [Error  : Unity Log] NullReferenceException
        /// Stack trace:
        /// RoR2.ElusiveAntlersPickup.FixedUpdate () (at <a43009bc6a5f4aee99e5521ef176a18d>:IL_003C)
        /// </code>
        /// </remarks>
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
                c.RemoveRange(match.Length-1); // :p (keep stloc.0)
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
