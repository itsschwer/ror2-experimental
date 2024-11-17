using RoR2;
using PressureDrop;

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
                    case "l":
                    case "loot":
                        Debug.SpawnLootInteractables(target);
                        return;
                    case "e":
                        Debug.SpawnEquipmentDrones(target);
                        return;
                    case "t":
                    case "templar":
                        SpawnMonster.Spawn(SpawnMonster.Get(SpawnMonster.Templar), target, DirectorPlacementRule.PlacementMode.Direct, TeamIndex.Monster);
                        return;
                    case "v":
                        SpawnMonster.SpawnVoidTitan(target);
                        return;
                    case "quick":
                        GiveQuickItems(target.inventory);
                        return;
                }
            }

            ChatCommander.OutputFail(args[0],
                "(<style=cSub>blueportal</style> | <style=cSub>itemcost</style> | <style=cSub>damaging</style>)");
        }

        private static void GiveQuickItems(Inventory inventory)
        {
            inventory.GiveItemString("JumpBoost", 2);
            inventory.GiveItemString("SprintOutOfCombat", 4);
            inventory.GiveItemString("SprintBonus", 4);
            inventory.GiveItemString("AttackSpeedAndMoveSpeed", 12);
            inventory.GiveItemString("HealthBasedDamageBonus", 1);
            inventory.GiveItemString("CritDamage", 1);
            inventory.GiveItemString("CritGlasses", 4);
            inventory.GiveItemString("Crowbar", 8);
            inventory.GiveItemString("IncreaseHealing", 2);
            inventory.GiveItemString("Medkit", 8);
            inventory.GiveItemString("EquipmentMagazine", 4);
            inventory.GiveItemString("FragileDamageBonus", 4);
        }
    }
}
