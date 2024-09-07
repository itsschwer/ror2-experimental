using RoR2;
using System.Linq;

namespace Experimental.Helpers
{
    public static class CommandCube
    {
        public static void Spawn(UnityEngine.Vector3 position, System.Func<ItemDef, bool> predicate) => Spawn(position, GetPickupOptions(predicate));

        public static void Spawn(UnityEngine.Vector3 position, PickupPickerController.Option[] options)
        {
            // Refer to RoR2.PickupDropletController.CreateCommandCube()
            UnityEngine.GameObject obj = UnityEngine.Object.Instantiate(RoR2.Artifacts.CommandArtifactManager.commandCubePrefab, position, UnityEngine.Quaternion.identity);
            obj.GetComponent<PickupPickerController>().SetOptionsInternal(options);
            if (options.Length > 0) obj.GetComponent<PickupIndexNetworker>().NetworkpickupIndex = options[0].pickupIndex;
            UnityEngine.Networking.NetworkServer.Spawn(obj);
        }

        public static PickupPickerController.Option[] GetPickupOptions(System.Func<ItemDef, bool> predicate)
        {
            // Logic from RoR2.PickupPickerController.SetTestOptions()
            return (from index in ItemCatalog.allItems
                    select ItemCatalog.GetItemDef(index) into def
                    where predicate.Invoke(def)
                    select def).Select(delegate(ItemDef def) {
                        PickupPickerController.Option result = default;
                        result.pickupIndex = PickupCatalog.FindPickupIndex(def.itemIndex);
                        result.available = true;
                        return result;
                    }).ToArray();
        }

        public static PickupPickerController.Option[] GetEquipmentPickupOptions()
        {
            PickupIndex[] equip = PickupTransmutationManager.GetGroupFromPickupIndex(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Blackhole.equipmentIndex));
            PickupIndex[] lunar = PickupTransmutationManager.GetGroupFromPickupIndex(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Meteor.equipmentIndex));
            // Refer to RoR2.PickupPickerController.GetOptionsFromPickupIndex()
            PickupPickerController.Option[] result = new PickupPickerController.Option[equip.Length + lunar.Length];
            for (int i = 0; i < result.Length; i++) {
                // Convert and combine the two arrays
                PickupIndex index = (i < equip.Length) ? equip[i] : lunar[i - equip.Length];
                result[i] = new PickupPickerController.Option { available = true, pickupIndex = index };
            }
            return result;
        }
    }
}
