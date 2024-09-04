using PressureDrop;
using RoR2;

namespace Experimental.Debugging
{
    internal static class Misc
    {
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
    }
}
