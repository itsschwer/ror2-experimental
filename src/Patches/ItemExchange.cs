using HarmonyLib;
using RoR2;
using System.Collections.Generic;
using System.Linq;

namespace AmGoldfish
{
    [HarmonyPatch]
    internal static class ItemExchange
    {
        [HarmonyPostfix, HarmonyPatch(typeof(CostTypeDef), nameof(CostTypeDef.PayCost))]
        private static void PayCost(Interactor activator, UnityEngine.GameObject purchasedObject, CostTypeDef.PayCostResults __result)
        {
            PurchaseInteraction interaction = purchasedObject?.GetComponent<PurchaseInteraction>();
            if (interaction == null) return;

            switch (interaction.displayNameToken) {
                default: return;
                case "BAZAAR_CAULDRON_NAME":
                // add other cases as desired (e.g. printers)
                    break;
            }

            NetworkUser user = activator?.GetComponent<CharacterBody>()?.master?.playerCharacterMasterController?.networkUser;
            if (user == null) return;

            Dictionary<PickupDef, int> exchanged = [];

            // RoR2.PurchaseInteraction.OnInteractionBegin()
            foreach (ItemIndex item in __result.itemsTaken) {
                if (IsScrap(item)) continue;

                PickupDef def = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(item));
                if (!exchanged.ContainsKey(def)) exchanged[def] = 0;
                exchanged[def]++;
            }
            foreach (EquipmentIndex item in __result.equipmentTaken) {
                PickupDef def = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(item));
                if (!exchanged.ContainsKey(def)) exchanged[def] = 0;
                exchanged[def]++;
            }

            AnnounceExchangedItems(exchanged, user);
        }

        private static void AnnounceExchangedItems(Dictionary<PickupDef, int> exchanged, NetworkUser user)
        {
            if (exchanged.Count <= 0) return;

            System.Text.StringBuilder items = new();
            var keys = exchanged.Keys.ToList();
            for (int i = 0; i < keys.Count; i++) {
                var item = keys[i];
                items.Append(Util.GenerateColoredString(Language.GetString(item.nameToken), item.baseColor));
                if (exchanged[item] != 1) items.Append($"({exchanged[item]})");

                int remaining = keys.Count - i;
                if (remaining > 2) items.Append(", ");
                else if (remaining > 1) items.Append((keys.Count > 2) ? ", and " : " and ");
            }

            // RoR2.SubjectChatMessage.IsSecondPerson()
            string subject = (LocalUserManager.readOnlyLocalUsersList.Count == 1 && user?.localUser != null) ? "You" : user.masterController.GetDisplayName();
            Chat.AddMessage($"<style=cEvent>{subject} gave up {items}</color>");
        }

        public static bool IsScrap(ItemIndex item) {
            return item == RoR2Content.Items.ScrapWhite.itemIndex
                || item == RoR2Content.Items.ScrapGreen.itemIndex
                || item == RoR2Content.Items.ScrapRed.itemIndex
                || item == RoR2Content.Items.ScrapYellow.itemIndex
                || item == DLC1Content.Items.RegeneratingScrap.itemIndex;
        }
    }
}
