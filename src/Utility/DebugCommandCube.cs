#if DEBUG
using RoR2;
using System.Linq;

namespace Experimental
{
    public static partial class Debug
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("BepInEx.Analyzers", "Publicizer001")] // Accessing a member that was not originally public
        public static void SpawnCommandCube(UnityEngine.Vector3 position, PickupPickerController.Option[] options)
        {
            UnityEngine.GameObject obj = UnityEngine.Object.Instantiate(RoR2.Artifacts.CommandArtifactManager.commandCubePrefab, position, UnityEngine.Quaternion.identity);
            obj.GetComponent<PickupPickerController>().SetOptionsInternal(options);
            if (options.Length > 0) obj.GetComponent<PickupIndexNetworker>().NetworkpickupIndex = options[0].pickupIndex;
            UnityEngine.Networking.NetworkServer.Spawn(obj);
        }

        public static void SpawnCommandCube(UnityEngine.Vector3 position, System.Func<ItemDef, bool> predicate)
        {
            SpawnCommandCube(position, GetPickupOptions(predicate));
        }

        // RoR2.PickupPickerController.SetTestOptions()
        public static PickupPickerController.Option[] GetPickupOptions(System.Func<ItemDef, bool> predicate)
        {
            return (from index in ItemCatalog.allItems
                    select ItemCatalog.GetItemDef(index) into def
                    where predicate.Invoke(def)
                    select def).Select(delegate(ItemDef def) {
                        PickupPickerController.Option result = default(PickupPickerController.Option);
                        result.pickupIndex = PickupCatalog.FindPickupIndex(def.itemIndex);
                        result.available = true;
                        return result;
                    }).ToArray();
        }

        // RoR2.PickupPickerController.GetOptionsFromPickupIndex()
        public static PickupPickerController.Option[] GetEquipmentPickupOptions()
        {
            PickupIndex[] equip = PickupTransmutationManager.GetGroupFromPickupIndex(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Blackhole.equipmentIndex));
            PickupIndex[] elite = PickupTransmutationManager.GetGroupFromPickupIndex(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixRed.equipmentIndex));
            PickupIndex[] lunar = PickupTransmutationManager.GetGroupFromPickupIndex(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Meteor.equipmentIndex));

            PickupPickerController.Option[] result = new PickupPickerController.Option[equip.Length + elite.Length + lunar.Length];
            for (int i = 0; i < result.Length; i++) {
                PickupIndex index = PickupIndex.none;

                int j = i - equip.Length;
                int k = j - elite.Length;

                if (i < equip.Length) index = equip[i];
                else if (j < elite.Length) index = elite[j];
                else if (k < lunar.Length) index = lunar[k];

                result[i] = new PickupPickerController.Option { available = true, pickupIndex = index };
            }
            return result;
        }
    }
}
#endif
