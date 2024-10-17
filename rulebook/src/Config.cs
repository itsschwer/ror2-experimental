using BepInEx.Configuration;

namespace RulebookItemBlacklist
{
    public sealed class Config
    {
        private readonly ConfigFile file;


        // Generic
        private readonly ConfigEntry<bool> loadFromConfig;
        private readonly ConfigEntry<bool> saveToConfig;
        // Accesors
        public bool LoadFromConfig => loadFromConfig.Value;
        public bool SaveToConfig => saveToConfig.Value;

        // Internal
        internal readonly ConfigEntry<string> blacklistedItems;
        internal readonly ConfigEntry<string> blacklistedEquipment;
        // Accessors
        public string BlacklistedItems => blacklistedItems.Value;
        public string BlacklistedEquipment => blacklistedEquipment.Value;

        public Config(ConfigFile config)
        {
            file = config;

            const string Generic = "";
            loadFromConfig = config.Bind<bool>(Generic, nameof(loadFromConfig), true,
                "");
            saveToConfig = config.Bind<bool>(Generic, nameof(saveToConfig), true,
                "");

            const string Internal = "Internal";
            blacklistedItems = config.Bind<string>(Internal, nameof(blacklistedItems),
                "DelayedDamage, KnockBackHitEnemies, LowerHealthHigherDamage, NegateAttack",
                "");
            blacklistedEquipment = config.Bind<string>(Internal, nameof(blacklistedEquipment),
                "",
                "");
        }
    }
}
