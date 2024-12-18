﻿using Experimental.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.UI
{
    internal sealed class CommandCubeControls
    {
        public readonly List<Action<RoR2.CharacterBody>> controls = [
            new(KeyCode.F6,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier1)),
                GenerateColoredString("White Command Cube", RoR2.ItemTier.Tier1)),
            new(KeyCode.F7,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier2)),
                GenerateColoredString("Green Command Cube", RoR2.ItemTier.Tier2)),
            new(KeyCode.F8,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier3)),
                GenerateColoredString("Red Command Cube", RoR2.ItemTier.Tier3)),
            new(KeyCode.F9,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Boss)),
                GenerateColoredString("Yellow Command Cube", RoR2.ItemTier.Boss)),
            new(KeyCode.F10,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.VoidTier1
                                                                                                || def.tier == RoR2.ItemTier.VoidTier2
                                                                                                || def.tier == RoR2.ItemTier.VoidTier3
                                                                                                || def.tier == RoR2.ItemTier.VoidBoss)),
                GenerateColoredString("Void Command Cube", RoR2.ItemTier.VoidBoss)),
            new(KeyCode.F11,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Lunar)),
                GenerateColoredString("Lunar Command Cube", RoR2.ItemTier.Lunar)),
            new(KeyCode.F12,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetEquipmentPickupOptions()),
                GenerateColoredString("Equipment Command Cube", RoR2.RoR2Content.Equipment.DeathProjectile.colorIndex)),
            new(KeyCode.KeypadDivide,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.NoTier)),
                "<style=cEvent>NoTier Command Cube</style>"),
            new(KeyCode.KeypadMultiply,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.AssignedAtRuntime)),
                "<style=cEvent>AssignedAtRuntime Command Cube</style>")
        ];

        private static string GenerateColoredString(string str, RoR2.ItemTier tier)
            => GenerateColoredString(str, RoR2.ItemTierCatalog.GetItemTierDef(tier).colorIndex);
        private static string GenerateColoredString(string str, RoR2.ColorCatalog.ColorIndex colorIndex)
        {
            Color32 color = RoR2.ColorCatalog.GetColor(colorIndex);
            return RoR2.Util.GenerateColoredString(str, color);
        }
    }
}
