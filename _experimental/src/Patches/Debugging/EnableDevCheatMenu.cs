#if NETSTANDARD2_1_OR_GREATER // DevCheatMenu was introduced in the Seekers of the Storm update
using HarmonyLib;
using RoR2.UI;
using UnityEngine;

namespace Experimental.Patches
{
    [HarmonyPatch]
    internal static class PauseMenu
    {
        [HarmonyPostfix, HarmonyPatch(typeof(PauseScreenController), nameof(PauseScreenController.Awake))]
        private static void PauseScreenControllerAwake(PauseScreenController __instance)
        {
            var copy = Object.Instantiate(__instance.exitGameButton, __instance.exitGameButton.transform.parent);
            var text = copy.GetComponentInChildren<HGTextMeshProUGUI>();
            var button = copy.GetComponent<HGButton>();
            copy.name = "dev cheat menu?";
            text.text = "dev cheat menu?";

#if DEBUG
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++) {
                Plugin.Logger.LogDebug($"[{i}] {button.onClick.GetPersistentMethodName(i)} | {button.onClick.GetPersistentTarget(i)}" +
                    $"\n\t{button.onClick.m_PersistentCalls.GetListener(i).arguments.stringArgument}");
                // [0] SubmitCmd | OptionsPanel(JUICED)(RoR2.ConsoleFunctions)
                //     quit_confirmed_command "quit"
            }
#endif

            // button.onClick.RemoveAllListeners(); // Does not affect persistent (i.e. assigned via Inspector) listeners
            button.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.onClick.AddListener(__instance.OpenDevCheatMenu);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(DevCheatMenu), nameof(DevCheatMenu.Awake))]
        private static bool DevCheatMenu_Awake() => false; // Prevent closing on awake
    }
}
#endif