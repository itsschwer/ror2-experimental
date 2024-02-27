#if DEBUG
using RoR2;
using System.Linq;

namespace Experimental
{
    public static partial class Debug
    {
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
    }
}
#endif
