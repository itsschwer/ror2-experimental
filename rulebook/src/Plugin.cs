using BepInEx;
using RoR2;

namespace RulebookItemBlacklist
{
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "RulebookItemBlacklist";
        public const string Version = "0.1.0";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        public static new Config Config { get; private set; }

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            Config = new Config(base.Config);

            IL.RuleDef_FromItem.Apply();
            // IL.RuleCategoryController_SetData.Apply();
            RuleCatalog.availability.onAvailable += EnableRules;
            new HarmonyLib.Harmony(Info.Metadata.GUID).PatchAll();

            Logger.LogMessage("~awake.");
        }

        private static void EnableRules()
        {
            foreach (RuleCategoryDef category in RuleCatalog.allCategoryDefs) {
                if (category.displayToken == Strings.ITEMS_CATEGORY || category.displayToken == Strings.EQUIPMENT_CATEGORY) {
                    category.hiddenTest = DontHide;
                }
                else if (category.displayToken == "RULE_HEADER_MISC") {
                    foreach (RuleDef rule in category.children) {
                        if (rule.displayToken == "RULE_MISC_STAGE_ORDER") {
                            foreach (RuleChoiceDef choice in rule.choices) {
#if DEBUG
                                Logger.LogDebug($"{choice.tooltipNameToken} | {Language.GetString(choice.tooltipNameToken)} | {Language.GetString(choice.tooltipBodyToken)}");
#endif
                                choice.excludeByDefault = false;
                                choice.spritePath = (choice.tooltipNameToken == "RULE_STAGEORDER_CHOICE_RANDOM_NAME" ? "Textures/MiscIcons/texRuleMapIsRandom" : "RoR2/Base/Common/MiscIcons/texTeleporterIcon.png");
                            }
                        }
                    }
                }
            }
            Plugin.Logger.LogDebug("Exposed rules.");
        }

        private static bool DontHide() => false;
    }
}
