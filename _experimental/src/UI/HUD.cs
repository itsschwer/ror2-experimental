using Experimental.Helpers;
using UnityEngine;

namespace Experimental.UI
{
    internal sealed class HUD : MonoBehaviour
    {
        private static RoR2.UI.HUD hud;

        public static void Instantiate(RoR2.UI.HUD hud, ref bool _)
        {
            if (HUD.hud != null) return;

            Plugin.Logger.LogDebug("Initialising debugging HUD.");
            hud.gameObject.AddComponent<HUD>();
            HUD.hud = hud;
        }

        private static T AddChild<T>(RectTransform parent, string name) where T : Component
        {
            GameObject obj = new GameObject(name, typeof(T));
            obj.transform.SetParent(parent);
            ((RectTransform)obj.transform).ResetRectTransform().AnchorStretchStretch();
            return obj.GetComponent<T>();
        }




        private new GameObject gameObject;
        private Canvas canvas;
        private bool hide;

        private const string Heading = $"<style=cWorldEvent>{Plugin.GUID} · {Plugin.Version} · DEBUGGING HUD</style>";
        private RoR2.UI.HGTextMeshProUGUI topLeft;
        private RoR2.UI.HGTextMeshProUGUI stageSelect;
        private RoR2.UI.HGTextMeshProUGUI controlKeys;
        private RoR2.UI.HGTextMeshProUGUI controlDescriptions;

        private readonly CommandCubeControls commandCubeControls = new();
        private readonly Action<object> toggleEnemySpawning = new(KeyCode.F5, (_) => Misc.ToggleEnemySpawning(), "<style=cEvent>Toggle Enemy Spawning</style>");
        private readonly Action<object> togglePlayerImmortality = new(KeyCode.F4, (_) => Misc.TogglePlayerImmortality(), "<style=cEvent>Toggle Player Immortality</style>");
        private readonly Action<object> addMountainStack = new(KeyCode.F3, (_) => Misc.AddMountainStack(), "<style=cEvent>Add Mountain Shrine Stack</style>");
        private readonly Action<object> forceChargeTeleporter = new(KeyCode.F2, (_) => Misc.ForceChargeTeleporter(), "<style=cEvent>Force Charge Teleporter</style>");
        private readonly Action<object> forceStage = new(KeyCode.F1, (_) => Stage.ForceStage(Stage.setStage ?? RoR2.Run.instance.nextStageScene), "<style=cEvent>Force Stage</style>");

        private readonly Action<object> clearChat = new(KeyCode.PageUp, (_) => RoR2.Chat.Clear(), "<style=cEvent>Clear Chat</style>");
        private readonly Action<object> addBeamsToPressurePlates = new(KeyCode.PageDown, (_) => Misc.AddBeamsToPressurePlates(), "<style=cEvent>Add Beams To Pressure Plates</style>");
        private readonly Action<object> forceMoonEscapeSequence = new(KeyCode.End, (_) => Misc.ForceMoonEscapeSequence(), "<style=cEvent>Force Moon Escape Sequence</style>");

        private void Start() => CreateUI(hud.mainContainer);

        private void CreateUI(GameObject parent)
        {
            gameObject = new GameObject(Plugin.GUID, typeof(Canvas), typeof(CanvasGroup));
            canvas = gameObject.GetComponent<Canvas>();
            gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent.transform);
            rect.ResetRectTransform().AnchorStretchStretch(new Vector2(-800, -380));

            topLeft = AddChild<RoR2.UI.HGTextMeshProUGUI>(rect, nameof(topLeft));
            topLeft.alignment = TMPro.TextAlignmentOptions.TopLeft;
            topLeft.fontSize = 20;
            topLeft.text = Heading;

            stageSelect = AddChild<RoR2.UI.HGTextMeshProUGUI>(rect, nameof(stageSelect));
            stageSelect.alignment = TMPro.TextAlignmentOptions.TopRight;
            stageSelect.fontSize = 18;

            const float controlKeysWidth = 40;
            const float controlFontSize = 18;
            controlKeys = AddChild<RoR2.UI.HGTextMeshProUGUI>(rect, nameof(controlKeys));
            controlKeys.enableWordWrapping = false;
            controlKeys.alignment = TMPro.TextAlignmentOptions.BottomRight;
            controlKeys.fontSize = controlFontSize;
            RectTransform ck = (RectTransform)controlKeys.transform;
            ck.sizeDelta = Vector2.left * (rect.rect.width - controlKeysWidth);
            ck.localPosition = ck.sizeDelta / -2;

            controlDescriptions = AddChild<RoR2.UI.HGTextMeshProUGUI>(rect, nameof(controlDescriptions));
            controlDescriptions.enableWordWrapping = false;
            controlDescriptions.alignment = TMPro.TextAlignmentOptions.BottomRight;
            controlDescriptions.fontSize = controlFontSize;
            RectTransform cd = (RectTransform)controlDescriptions.transform;
            cd.sizeDelta = Vector2.right * -controlKeysWidth;
            cd.localPosition = cd.sizeDelta / 2;
        }

        private void Update()
        {
            if (RoR2.Run.instance == null) return;

            // Scoreboard visibility logic from RoR2.UI.HUD.Update()
            bool scoreboardVisible = hud.localUserViewer?.inputPlayer != null && hud.localUserViewer.inputPlayer.GetButton("info");
            if (Input.GetKeyDown(KeyCode.KeypadPlus)) hide = !hide;
            canvas.enabled = !scoreboardVisible && !hide;

            clearChat.PerformIfPossible(null);
            addBeamsToPressurePlates.PerformIfPossible(null);
            forceMoonEscapeSequence.PerformIfPossible(null);

            System.Text.StringBuilder keyString = new();
            System.Text.StringBuilder descriptionString = new();

            if (hud.cameraRigController?.targetBody)
            {
                for (int i = 0; i < commandCubeControls.controls.Count; i++)
                {
                    commandCubeControls.controls[i].PerformIfPossible(hud.cameraRigController.targetBody);
                    keyString.AppendLine(commandCubeControls.controls[i].key.ToString());
                    descriptionString.AppendLine(commandCubeControls.controls[i].description);
                }
            }

            forceStage.PerformIfPossible(null);
            keyString.AppendLine().AppendLine(forceStage.key.ToString());
            descriptionString.AppendLine().AppendLine(forceStage.description);
            forceChargeTeleporter.PerformIfPossible(null);
            keyString.AppendLine(forceChargeTeleporter.key.ToString());
            descriptionString.AppendLine(forceChargeTeleporter.description);
            addMountainStack.PerformIfPossible(null);
            keyString.AppendLine(addMountainStack.key.ToString());
            descriptionString.AppendLine(addMountainStack.description);
            toggleEnemySpawning.PerformIfPossible(null);
            keyString.AppendLine(toggleEnemySpawning.key.ToString());
            descriptionString.AppendLine(toggleEnemySpawning.description);
            togglePlayerImmortality.PerformIfPossible(null);
            keyString.AppendLine(togglePlayerImmortality.key.ToString());
            descriptionString.AppendLine(togglePlayerImmortality.description);

            controlKeys.text = keyString.ToString();
            controlDescriptions.text = descriptionString.ToString();

            if (!canvas.enabled) return;

            topLeft.text = $"{Heading}\n\n\n{Cheatsheet.currentDisplay}";

            System.Text.StringBuilder sb = new();
            sb.Append("<style=cEvent>");
            sb.AppendLine($"/setstage: {Stage.GetDisplayName(Stage.setStage)}");
            sb.AppendLine($"Run next stage: {Stage.GetDisplayName(RoR2.Run.instance.nextStageScene)}");
            sb.AppendLine($"Stage next stage: {Stage.GetDisplayName(RoR2.Stage.instance.nextStage)}");
            sb.Append("</style>");
            stageSelect.text = sb.ToString();
        }
    }
}
