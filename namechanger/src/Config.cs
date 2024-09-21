using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using UnityEngine;

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

            ModSettingsManager.SetModDescription("Change your in-game* name without having to change your Steam/Epic profile name." +
                "\n\n*Does not affect the lobby player list or \"user connected\" messages." +
                "\n\n\n\nThe name replacement may be shown to other players if you are the host and have mods that print NetworkUser.userName (e.g. server broadcast chat messages)." +
                "\nSimilarly, the name replacement may not be applied on messages from the host that use NetworkUser.userName.");
            SetModIcon();
        }

        private void NameReplacement_SettingChanged(object sender, System.EventArgs e) => Plugin.UpdateUserNames();

        private static void SetModIcon()
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            string path = System.IO.Path.Combine(info.Parent.FullName, "icon.png");
            Texture2D texture = new Texture2D(256, 256);
            try {
                if (texture.LoadImage(System.IO.File.ReadAllBytes(path))) {
                    ModSettingsManager.SetModIcon(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f));
                }
                else Plugin.Logger.LogError($"Failed to load {path}");
            }
            catch (System.IO.FileNotFoundException e) {
                Plugin.Logger.LogError(e);
            }
        }
    }
}
