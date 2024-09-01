#if DEBUG
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.Debugging.UI
{
    internal sealed class CommandCubeControls
    {
        public readonly List<Action<RoR2.CharacterBody>> controls = [
            new(KeyCode.F9,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier1)),
                "Spawn White Command Cube"),
            new(KeyCode.F10,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier2)),
                "Spawn Green Command Cube"),
            new(KeyCode.F11,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Tier3)),
                "Spawn Red Command Cube"),
            new(KeyCode.F12,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Boss)),
                "Spawn Yellow Command Cube"),
            new(KeyCode.F8,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetEquipmentPickupOptions()),
                "Spawn Equipment Command Cube"),
            new(KeyCode.F7,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => def.tier == RoR2.ItemTier.Lunar)),
                "Spawn Lunar Command Cube"),
            new(KeyCode.F6,
                (body) => CommandCube.Spawn(body.footPosition, CommandCube.GetPickupOptions(def => PressureDrop.Drop.IsVoidTier(def.tier))),
                "Spawn Void Command Cube")
        ];
    }
}
#endif
