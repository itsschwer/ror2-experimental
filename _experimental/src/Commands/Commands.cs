using RoR2;
using PressureDrop;
using System.Linq;

namespace Experimental.Commands
{
    internal static class Commands
    {
        public static void Register()
        {
            ChatCommander.Register("/show", Cheatsheet.Parse);
            ChatCommander.Register("/setstage", Helpers.Stage.SetStage);

            ChatCommander.Register("/s", Spawn);

            ChatCommander.Register("/walkui", WalkUI.Walk);
        }

        public static void Unregister()
        {
            ChatCommander.Unregister("/show", Cheatsheet.Parse);
            ChatCommander.Unregister("/setstage", Helpers.Stage.SetStage);

            ChatCommander.Unregister("/s", Spawn);

            ChatCommander.Unregister("/walkui", WalkUI.Walk);
        }

        private static bool GetUserBody(NetworkUser user, out CharacterBody body)
        {
            body = null;
            if (Run.instance == null) return false;
            body = user?.GetCurrentBody();
            return (body != null);
        }

        private static void Spawn(NetworkUser user, string[] args)
        {
            if (!GetUserBody(user, out CharacterBody target)) return;

            if (args.Length == 2) {
                switch (args[1].ToLowerInvariant()) {
                    default: break;
                    case "b":
                    case "blue":
                    case "portal":
                    case "blueportal":
                        Debug.SpawnBluePortal(target);
                        return;
                    case "i":
                    case "item":
                    case "itemcost":
                        Debug.SpawnItemCostInteractables(target);
                        return;
                    case "d":
                    case "damage":
                    case "damaging":
                        Debug.SpawnDamagingInteractables(target);
                        return;
                    case "p":
                        Drop.DropStyleChest(target.transform,
                            new ItemDef[] {
                                DLC2Content.Items.BoostAllStats,
                                DLC2Content.Items.DelayedDamage,
                                DLC2Content.Items.ExtraShrineItem,
                                DLC2Content.Items.ExtraStatsOnLevelUp,
                                DLC2Content.Items.GoldOnStageStart,
                                DLC2Content.Items.IncreaseDamageOnMultiKill,
                                DLC2Content.Items.IncreasePrimaryDamage,
                                DLC2Content.Items.KnockBackHitEnemies,
                                DLC2Content.Items.LowerHealthHigherDamage,
                                DLC2Content.Items.LowerPricedChests,
                                DLC2Content.Items.LowerPricedChestsConsumed,
                                DLC2Content.Items.MeteorAttackOnHighDamage,
                                DLC2Content.Items.NegateAttack,
                                DLC2Content.Items.OnLevelUpFreeUnlock,
                                DLC2Content.Items.ResetChests,
                                DLC2Content.Items.StunAndPierce,
                                DLC2Content.Items.TeleportOnLowHealth,
                                DLC2Content.Items.TeleportOnLowHealthConsumed,
                                DLC2Content.Items.TriggerEnemyDebuffs,
                            }.Select(x => PickupCatalog.FindPickupIndex(x.itemIndex)).ToArray(), 1);
                        return;
                }
            }

            ChatCommander.OutputFail(args[0],
                "(<style=cSub>blueportal</style> | <style=cSub>itemcost</style> | <style=cSub>damaging</style>)");
        }
    }
}
