using UnityEngine;

namespace Experimental.Debugging.UI
{
    internal sealed class HUD : MonoBehaviour
    {
        private static RoR2.UI.HUD hud;

        public static void Instantiate(RoR2.UI.HUD hud, ref bool _)
        {
            if (HUD.hud != null) return;

            Plugin.Logger.LogDebug("Adding to HUD.");
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
        private RoR2.UI.HGTextMeshProUGUI bottomRight;

        private CommandCubeControls commandCubeControls = new();
        private Action<object> toggleEnemySpawning= new(KeyCode.F1, (_) => Misc.ToggleEnemySpawning(), "Toggle Enemy Spawning");

        private void Start() => CreateUI(hud.mainContainer);

        private void CreateUI(GameObject parent)
        {
            gameObject = new GameObject(Plugin.GUID, typeof(Canvas), typeof(CanvasGroup));
            canvas = gameObject.GetComponent<Canvas>();
            gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent.transform);
            rect.ResetRectTransform().AnchorStretchStretch(new Vector2(-784, -400));

            bottomRight = AddChild<RoR2.UI.HGTextMeshProUGUI>(rect, "hint");
        }

        private void Update()
        {
            System.Text.StringBuilder sb = new();

            if (hud?.localUserViewer?.cachedBody) {
                for (int i = 0; i < commandCubeControls.controls.Count; i++) {
                    commandCubeControls.controls[i].PerformIfPossible(hud.localUserViewer.cachedBody);
                    sb.AppendLine(commandCubeControls.controls[i].ToString());
                }

                toggleEnemySpawning.PerformIfPossible(null);
                sb.AppendLine().AppendLine(toggleEnemySpawning.ToString());
            }

            bottomRight.text = sb.ToString();
        }
    }
}
