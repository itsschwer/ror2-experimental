using HarmonyLib;
using RoR2;
using System;

namespace RulebookItemBlacklist.HarmonyPatches
{
    [HarmonyPatch]
    internal static class SaveToConfig
    {
        [HarmonyPostfix, HarmonyPatch(typeof(PreGameRuleVoteController.LocalUserBallotPersistenceManager), nameof(PreGameRuleVoteController.LocalUserBallotPersistenceManager.OnLocalUserSignIn))]
        private static void LocalUserBallotPersistenceManager_OnLocalUserSignIn(LocalUser localUser)
        {
            if (!Plugin.Config.LoadFromConfig) return;

            if (!PreGameRuleVoteController.LocalUserBallotPersistenceManager.votesCache.TryGetValue(localUser, out PreGameRuleVoteController.Vote[] votes)) return;
            votes ??= PreGameRuleVoteController.CreateBallot();

            char[] separators = { ',', ' ' };
            FromString(Strings.ITEMS_PREFIX, Plugin.Config.BlacklistedItems.Split(separators, StringSplitOptions.RemoveEmptyEntries), votes);
            FromString(Strings.EQUIPMENT_PREFIX, Plugin.Config.BlacklistedEquipment.Split(separators, StringSplitOptions.RemoveEmptyEntries), votes);

            PreGameRuleVoteController.LocalUserBallotPersistenceManager.votesCache[localUser] = votes;
            Plugin.Logger.LogDebug("Applied blacklist votes.");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PreGameRuleVoteController.LocalUserBallotPersistenceManager), nameof(PreGameRuleVoteController.LocalUserBallotPersistenceManager.OnVotesUpdated))]
        private static void LocalUserBallotPersistenceManager_OnVotesUpdated()
        {
            if (!Plugin.Config.SaveToConfig) return;

            if (!PreGameRuleVoteController.LocalUserBallotPersistenceManager.votesCache.TryGetValue(LocalUserManager.GetFirstLocalUser(), out PreGameRuleVoteController.Vote[] votes) || votes == null) return;
            foreach (RuleCategoryDef category in RuleCatalog.allCategoryDefs) {
                switch (category.displayToken) {
                    default: continue;
                    case Strings.ITEMS_CATEGORY:
                        Plugin.Config.blacklistedItems.Value = ToString(Strings.ITEMS_PREFIX.Length, category, votes);
                        Plugin.Logger.LogDebug("Saved blacklisted items.");
                        break;
                    case Strings.EQUIPMENT_CATEGORY:
                        Plugin.Config.blacklistedEquipment.Value = ToString(Strings.EQUIPMENT_PREFIX.Length, category, votes);
                        Plugin.Logger.LogDebug("Saved blacklisted equipment.");
                        break;
                }
            }
        }

        private static string ToString(int rulePrefixLength, RuleCategoryDef category, PreGameRuleVoteController.Vote[] votes)
        {
            System.Collections.Generic.IList<string> strings = new System.Collections.Generic.List<string>();
            foreach (RuleDef rule in category.children) {
                if (votes[rule.globalIndex].choiceValue == 1) strings.Add(rule.globalName.Substring(rulePrefixLength));
            }
            return string.Join(", ", strings);
        }

        private static void FromString(string rulePrefix, string[] tokens, PreGameRuleVoteController.Vote[] votes)
        {
            foreach (string token in tokens) {
                RuleDef rule = RuleCatalog.FindRuleDef($"{rulePrefix}{token}");
                if (rule != null) {
                    votes[rule.globalIndex].choiceValue = 1;
                }
                else Plugin.Logger.LogWarning($"Could not apply \"{rulePrefix}{token}\"");
            }
        }
    }
}
