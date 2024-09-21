using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;

namespace NameChanger
{
    internal sealed class Config
    {
        private readonly ConfigEntry<string> nameReplacement;
        public string NameReplacement => nameReplacement.Value;

        public Config(ConfigFile config)
        {
            ModSettingsManager.AddOption(new GenericButtonOption("", "Settings", "Force user names to update, in case the name replacement is not applied.", "Refresh User Names", Plugin.UpdateUserNames));

            nameReplacement = config.Bind<string>("Settings", nameof(nameReplacement), "",
                "Input a name to use as a replacement for the name that appears in-game (excluding the lobby list / user connected messages).\n\nLeave empty to disable user name replacement.");
            ModSettingsManager.AddOption(new StringInputFieldOption(nameReplacement));
            nameReplacement.SettingChanged += NameReplacement_SettingChanged;
        }

        private void NameReplacement_SettingChanged(object sender, System.EventArgs e) => Plugin.UpdateUserNames();
    }
}
