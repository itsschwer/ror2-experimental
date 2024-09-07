using PressureDrop;
using RoR2;

namespace Experimental.Debugging
{
    internal static class Misc
    {
        public static string CurrentHUDShowString;

        public static void ShowParse(NetworkUser user, string[] args)
        {
            if (!UnityEngine.Networking.NetworkServer.active) return;

            if (args.Length == 2) {
                switch (args[1]) {
                    default: break;
                    case "clear":
                        CurrentHUDShowString = "";
                        return;
                    case "stages":
                        CurrentHUDShowString = Stage.StyleStageNamesForShow();
                        return;
                    case "PressureDrop":
                        CurrentHUDShowString = Cheatsheet.PressureDrop;
                        return;
                    case "DamageLog":
                        CurrentHUDShowString = Cheatsheet.DamageLog;
                        return;
                }
            }
            ChatCommander.OutputFail(args[0], "(clear | stages | PressureDrop | DamageLog)");
        }

        public static void ToggleEnemySpawning()
        {
            bool wasDisabled = CombatDirector.cvDirectorCombatDisable.GetString() != "0";
            CombatDirector.cvDirectorCombatDisable.SetBool(!wasDisabled);

            if (wasDisabled) ChatCommander.Output("Enemy spawning enabled.");
            else ChatCommander.Output("Enemy spawning disabled.");
        }

        public static void TogglePlayerImmortality()
        {
            TakeDamagePlayerNonLethal.SetActive(!TakeDamagePlayerNonLethal.Active);
            string status = TakeDamagePlayerNonLethal.Active ? "enabled" : "disabled";
            ChatCommander.Output($"Player immortality {status}.");
        }

        public static void ForceChargeTeleporter()
        {
            if (TeleporterInteraction.instance == null) return;

            TeleporterInteraction.instance.holdoutZoneController.charge = 0.99f;
        }
    }
}
