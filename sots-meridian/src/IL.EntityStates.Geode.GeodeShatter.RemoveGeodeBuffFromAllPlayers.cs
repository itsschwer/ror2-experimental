using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RoR2;
using System;
using System.Reflection;

namespace MeridianPrimePrime.IL
{
    /// <summary>
    /// NullReferenceException: Object reference not set to an instance of an object
    /// Stack trace:
    /// <see cref="EntityStates.Geode.GeodeShatter.RemoveGeodeBuffFromAllPlayers()"/> (at <c>IL_001E</c>)
    /// </summary>
    internal static class RemoveGeodeBuffFromAllPlayers
    {
        internal static void Apply()
        {
            ILHook hook = new ILHook(typeof(EntityStates.Geode.GeodeShatter).GetMethod(nameof(EntityStates.Geode.GeodeShatter.RemoveGeodeBuffFromAllPlayers), BindingFlags.Instance | BindingFlags.NonPublic), (il) => {
                ILCursor c = new ILCursor(il);

                ILLabel iter = null;
                MethodReference curr = null;
                Func<Instruction, bool>[] match = {
                    x => x.MatchBr(out iter),
                    x => x.MatchLdloc(0),
                    x => x.MatchCallOrCallvirt(out curr), // [netstandard]System.Collections.Generic.IEnumerator`1<class RoR2.PlayerCharacterMasterController>::get_Current()
                    x => x.MatchCallOrCallvirt<PlayerCharacterMasterController>($"get_{nameof(PlayerCharacterMasterController.networkUser)}"),
                    x => x.MatchCallOrCallvirt<NetworkUser>(nameof(NetworkUser.GetCurrentBody)),
                    x => x.MatchStloc(1)
                };

                if (c.TryGotoNext(MoveType.After, match)) {
#if DEBUG
                    c.Emit(OpCodes.Ldloc_0);
                    c.Emit(OpCodes.Callvirt, curr);
                    c.EmitDelegate<Action<PlayerCharacterMasterController>>((player) => {
                        CharacterBody body = player.networkUser.GetCurrentBody();
                        if (body == null) Plugin.Logger.LogWarning($"{nameof(RemoveGeodeBuffFromAllPlayers)}> {player.networkUser.userName} body NULL");
                        else Plugin.Logger.LogDebug($"{nameof(RemoveGeodeBuffFromAllPlayers)}> {player.networkUser.userName} body OK");
                    });
#endif
                    c.Emit(OpCodes.Ldloc_1);
                    c.Emit(OpCodes.Brfalse, iter.Target);
#if DEBUG
                    Plugin.Logger.LogDebug(il.ToString());
#endif
                }
                else Plugin.Logger.LogError($"{nameof(RemoveGeodeBuffFromAllPlayers)}> Cannot hook: failed to match IL instructions.");
            });
        }
    }
}
