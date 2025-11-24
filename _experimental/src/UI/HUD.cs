using Experimental.Helpers;
using Experimental.UI.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.UI
{
    internal sealed class HUD : MonoBehaviour
    {
        private const KeyCode hideKey = KeyCode.KeypadPlus;
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

        private const string Heading = $"<size=+2><style=cWorldEvent>{Plugin.GUID} · {Plugin.Version} · DEBUGGING HUD</style></size>";
        private RoR2.UI.HGTextMeshProUGUI topLeft;
        private RoR2.UI.HGTextMeshProUGUI topRight;
        private RoR2.UI.HGTextMeshProUGUI botLeft;
        private RoR2.UI.HGTextMeshProUGUI controlKeys;
        private RoR2.UI.HGTextMeshProUGUI controlDescriptions;

        private readonly CommandCubeControls commandCubeControls = new();

        private List<List<Action<object>>> miscControls = [
            [ new(KeyCode.KeypadMinus, (_) => RoR2.Chat.Clear(), "<style=cEvent>Clear Chat</style>") ],
            [
                new(KeyCode.F1, (_) => Stage.ForceStage(Stage.setStage ?? RoR2.Run.instance.nextStageScene), "<style=cEvent>Force Stage</style>"),
                new(KeyCode.F3, (_) => Misc.TogglePlayerImmortality(), "<style=cEvent>Toggle Player Immortality</style>"),
                new(KeyCode.F4, (_) => Misc.ToggleEnemySpawning(), "<style=cEvent>Toggle Enemy Spawning</style>"),
            ],
            [
                new(KeyCode.F5, (_) => Misc.ForceMoonEscapeSequence(), "<style=cEvent>Force Moon Escape Sequence</style>"),
                new(KeyCode.F8, (_) => Misc.AddBeamsToPressurePlates(), "<style=cEvent>Add Beams To Pressure Plates</style>"),
            ],
            [
                new(KeyCode.F9, (_) => Misc.ForceChargeTeleporter(), "<style=cEvent>Force Charge Teleporter</style>"),
                new(KeyCode.F10, (_) => Misc.AddMountainStack(), "<style=cEvent>Add Mountain Shrine Stack</style>"),
            ],


        ];

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
            topLeft.fontSize = 18;
            topLeft.text = Heading;

            topRight = AddChild<RoR2.UI.HGTextMeshProUGUI>(rect, nameof(topRight));
            topRight.alignment = TMPro.TextAlignmentOptions.TopRight;
            topRight.fontSize = 18;

            botLeft = AddChild<RoR2.UI.HGTextMeshProUGUI>(rect, nameof(botLeft));
            botLeft.alignment = TMPro.TextAlignmentOptions.BottomLeft;
            botLeft.fontSize = 18;

            const float controlKeysWidth = 80;
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

            if (Input.GetKeyDown(hideKey)) hide = !hide;
            canvas.enabled = !hud.scoreboardPanel.activeSelf && !hide;

            topLeft.text = $"{Heading}\n\n{GenerateStringPosition()}\n{Cheatsheet.currentDisplay}";
            topRight.text = GenerateStringStageSelect();
            // todo: mention hide debug hud control under heading

            System.Text.StringBuilder keyString = new();
            System.Text.StringBuilder descriptionString = new();

            if (hud.cameraRigController?.targetBody) {
                for (int i = 0; i < commandCubeControls.controls.Count; i++) {
                    commandCubeControls.controls[i].PerformIfPossible(hud.cameraRigController.targetBody);
                    keyString.AppendLine(commandCubeControls.controls[i].key.ToString());
                    descriptionString.AppendLine(commandCubeControls.controls[i].description);
                }
            }
            
            for (int i = 0; i < miscControls.Count; i++) {
                keyString.AppendLine();
                descriptionString.AppendLine();
                for (int j = 0; j < miscControls[i].Count; j++) {
                    miscControls[i][j].PerformIfPossible(null);
                    keyString.AppendLine(miscControls[i][j].key.ToString());
                    descriptionString.AppendLine(miscControls[i][j].description);
                }
            }

            if (!canvas.enabled) return;

            controlKeys.text = keyString.ToString();
            controlDescriptions.text = descriptionString.ToString();
        }

        private string GenerateStringPosition()
        {
            RoR2.CharacterBody body = hud.localUserViewer?.cameraRigController?.targetBody;
            if (body) {
                System.Text.StringBuilder sb = new();
                sb.Append("<style=cEvent>");
                sb.AppendLine($"footPosition: {body.footPosition.PrettyPrint()}");
                if (body.inputBank) {
                    sb.AppendLine($"aimDirection: {body.inputBank.aimDirection.PrettyPrint()}");
                }
                sb.Append("</style>");
                return sb.ToString();
            }
            return "";
        }

        private string GenerateStringStageSelect()
        {
            System.Text.StringBuilder sb = new();
            sb.Append("<style=cEvent>");
            sb.AppendLine($"/setstage: {Stage.GetDisplayName(Stage.setStage)}");
            sb.AppendLine($"Run next stage: {Stage.GetDisplayName(RoR2.Run.instance.nextStageScene)}");
            sb.AppendLine($"Stage next stage: {Stage.GetDisplayName(RoR2.Stage.instance.nextStage)}");
            sb.Append("</style>");
            return sb.ToString();
        }
    }
}
