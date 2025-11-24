using Experimental.Helpers;
using Experimental.UI.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.UI
{
    internal sealed class CommandCubeControls
    {
        public readonly List<Action<RoR2.CharacterBody>> controls = [
            new(KeyCode.Keypad7,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier1)),
                GenerateColoredString("White Command Cube", RoR2.ItemTier.Tier1)),
            new(KeyCode.Keypad8,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier2)),
                GenerateColoredString("Green Command Cube", RoR2.ItemTier.Tier2)),
            new(KeyCode.Keypad9,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier3)),
                GenerateColoredString("Red Command Cube", RoR2.ItemTier.Tier3)),
            new(KeyCode.Keypad4,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Boss)),
                GenerateColoredString("Yellow Command Cube", RoR2.ItemTier.Boss)),
            new(KeyCode.Keypad5,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.VoidTier1
                                                                                                || def.tier == RoR2.ItemTier.VoidTier2
                                                                                                || def.tier == RoR2.ItemTier.VoidTier3
                                                                                                || def.tier == RoR2.ItemTier.VoidBoss)),
                GenerateColoredString("Void Command Cube", RoR2.ItemTier.VoidBoss)),
            new(KeyCode.Keypad6,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Lunar)),
                GenerateColoredString("Lunar Command Cube", RoR2.ItemTier.Lunar)),
            new(KeyCode.Keypad1,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetEquipmentPickupOptions()),
                Styling.GenerateColoredString("Equipment Command Cube", RoR2.RoR2Content.Equipment.DeathProjectile.colorIndex)),
            new(KeyCode.Keypad2,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.NoTier)),
                "<style=cEvent>NoTier Command Cube</style>"),
            new(KeyCode.Keypad3,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.AssignedAtRuntime)),
                "<s><style=cEvent>AssignedAtRuntime Command Cube</style></s>")
        ];

        private static string GenerateColoredString(string str, RoR2.ItemTier tier)
            => Styling.GenerateColoredString(str, RoR2.ItemTierCatalog.GetItemTierDef(tier).colorIndex);
        
    }
}
