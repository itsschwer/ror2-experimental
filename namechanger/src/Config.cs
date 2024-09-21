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
            nameReplacement = config.Bind<string>("Settings", nameof(nameReplacement), "",
                "empty for vanilla");
            ModSettingsManager.AddOption(new StringInputFieldOption(nameReplacement));
            nameReplacement.SettingChanged += NameReplacement_SettingChanged;
        }

        private void NameReplacement_SettingChanged(object sender, System.EventArgs e)
        {
            RoR2.NetworkUser user = RoR2.LocalUserManager.GetFirstLocalUser()?.currentNetworkUser;
            if (user != null) {
                user.UpdateUserName();
            }
        }
    }
}
