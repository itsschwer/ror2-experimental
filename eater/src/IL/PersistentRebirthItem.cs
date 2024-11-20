using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace Eater.IL
{
    [HarmonyPatch]
    internal static class PersistentRebirthItem
    {
        [HarmonyPrefix, HarmonyPatch(typeof(NetworkUser), nameof(NetworkUser.NetworkrebirthItem), MethodType.Setter)]
        private static bool NetworkUser_NetworkrebirthItem(NetworkUser __instance, ItemIndex value) => !(__instance.isLocalPlayer && value == ItemIndex.None);

        [HarmonyILManipulator, HarmonyPatch(typeof(Run), nameof(Run.Start))]
        private static void Run_Start(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            Func<Instruction, bool>[] match = {
                x => x.MatchLdloc(6),
                x => x.MatchLdfld<NetworkUser>(nameof(NetworkUser.localUser)),
                x => x.MatchCallOrCallvirt<LocalUser>($"get_{nameof(LocalUser.userProfile)}"),
                x => x.MatchLdnull(),
                x => x.MatchStfld<UserProfile>(nameof(UserProfile.RebirthItem))
            };

            if (c.TryGotoNext(MoveType.Before, match)) {
                ILLabel skip = c.DefineLabel();
                c.Emit(OpCodes.Br, skip);
                c.GotoNext(MoveType.After, match);
                c.MarkLabel(skip);
                c.Emit(OpCodes.Ldloc, 6);
                c.EmitDelegate<Action<NetworkUser>>((user) => {
                    if (string.IsNullOrWhiteSpace(user.localUser.userProfile.RebirthItem) || user.localUser.userProfile.RebirthItem == "ItemIndex.None") {
                        user.localUser.userProfile.RebirthItem = "ItemIndex.ResetChests";
                        Plugin.Logger.LogMessage($"Set rebirth item for {user.userName}");
                    }
                    Plugin.Logger.LogMessage($"Rebirth item for {user.userName} is {user.localUser.userProfile.RebirthItem}");
                });
#if DEBUG
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(PersistentRebirthItem)}> Cannot hook {nameof(Run_Start)}: failed to match IL instructions.");
        }
    }
}
