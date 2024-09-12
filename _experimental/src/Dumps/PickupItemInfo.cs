using RoR2;
using System.Text;

namespace Experimental.Dumps
{
    internal static class PickupItemInfo
    {
        public static string Dump(PickupDef def, out bool hiddenOrCantRemove)
        {
            hiddenOrCantRemove = false;
            StringBuilder sb = new($"itemIndex: {def.itemIndex} | itemTier: {def.itemTier} | equipmentIndex: {def.equipmentIndex} | isLunar: {def.isLunar} | isBoss: {def.isBoss}\n");
            if (def.itemIndex != ItemIndex.None) {
                ItemDef itemDef = ItemCatalog.GetItemDef(def.itemIndex);
                sb.Append($"\t\thidden: {itemDef.hidden} | canRemove: {itemDef.canRemove} | {itemDef.requiredExpansion?.nameToken} | {itemDef.nameToken} | {Language.GetString(itemDef.nameToken)}");
                hiddenOrCantRemove = itemDef.hidden || !itemDef.canRemove;
            }
            return sb.ToString();
        }
    }
}
