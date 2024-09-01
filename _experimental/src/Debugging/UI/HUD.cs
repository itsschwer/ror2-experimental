using UnityEngine;

namespace Experimental.Debugging.UI
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
        private RoR2.UI.HGTextMeshProUGUI topLeft;
        private RoR2.UI.HGTextMeshProUGUI bottomRight;

        private CommandCubeControls commandCubeControls = new();
        private Action<object> toggleEnemySpawning= new(KeyCode.F5, (_) => Misc.ToggleEnemySpawning(), "Toggle Enemy Spawning");

        private void Start() => CreateUI(hud.mainContainer);

        private void CreateUI(GameObject parent)
        {
            gameObject = new GameObject(Plugin.GUID, typeof(Canvas), typeof(CanvasGroup));
            canvas = gameObject.GetComponent<Canvas>();
            gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent.transform);
            rect.ResetRectTransform().AnchorStretchStretch(new Vector2(-800, -400));

            topLeft = AddChild<RoR2.UI.HGTextMeshProUGUI>(rect, nameof(topLeft));
            topLeft.alignment = TMPro.TextAlignmentOptions.TopLeft;
            topLeft.fontSize = 20;
            topLeft.text = $"<style=cWorldEvent>{Plugin.GUID} · {Plugin.Version} · DEBUGGING HUD</style>";

            bottomRight = AddChild<RoR2.UI.HGTextMeshProUGUI>(rect, nameof(bottomRight));
            bottomRight.alignment = TMPro.TextAlignmentOptions.BottomRight;
            bottomRight.fontSize = 18;
        }

        private void Update()
        {
            // Scoreboard visibility logic from RoR2.UI.HUD.Update()
            bool scoreboardVisible = (hud.localUserViewer?.inputPlayer != null && hud.localUserViewer.inputPlayer.GetButton("info"));
            if (Input.GetKeyDown(KeyCode.KeypadPlus)) hide = !hide;
            canvas.enabled = !scoreboardVisible && !hide;

            System.Text.StringBuilder sb = new();

            if (hud.cameraRigController?.targetBody) {
                for (int i = 0; i < commandCubeControls.controls.Count; i++) {
                    commandCubeControls.controls[i].PerformIfPossible(hud.cameraRigController.targetBody);
                    sb.AppendLine(commandCubeControls.controls[i].ToString());
                }

                toggleEnemySpawning.PerformIfPossible(null);
                sb.AppendLine().AppendLine(toggleEnemySpawning.ToString());
            }

            bottomRight.text = sb.ToString();
        }
    }
}
