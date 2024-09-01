#if DEBUG
using RoR2.UI;
using UnityEngine.UI;
using UnityEngine;

namespace Experimental.Debugging
{
    public static class WalkUI
    {
        internal static bool DamageLogLoaded => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(DamageLog.Plugin.GUID);

        internal static void Walk(RoR2.NetworkUser _, string[] args)
        {
            if (args.Length <= 1) return;

            switch (args[1]) {
                default: break;
                case "diff":
                    Walk(GameObject.FindObjectOfType<CurrentDifficultyIconController>().transform, 0);
                    break;
                case "obj":
                    Walk(GameObject.FindObjectOfType<ObjectivePanelController>().GetComponentInChildren<HGTextMeshProUGUI>().transform, 0);
                    break;
                case "dmg":
                    Walk(GameObject.Find("DamageEntry").transform, 0);
                    break;
            }
        }

        public static void Walk(Transform child, int level)
        {
            string indent = new string(' ', level * 4);
            Plugin.Logger.LogDebug($"{indent}{child.name} <{child.gameObject.layer}>");

            var cv = child.GetComponent<Canvas>();
            if (cv != null) Plugin.Logger.LogDebug($"{indent}└ {nameof(Canvas)}: {nameof(Canvas.overrideSorting)} `{cv.overrideSorting}` | {nameof(Canvas.sortingLayerName)} `{cv.sortingLayerName}` | {nameof(Canvas.renderMode)} `{cv.renderMode}`");

            var cg = child.GetComponent<CanvasGroup>();
            if (cg != null) Plugin.Logger.LogDebug($"{indent}└ {nameof(CanvasGroup)}: {nameof(CanvasGroup.blocksRaycasts)} `{cg.blocksRaycasts}` | {nameof(CanvasGroup.interactable)} `{cg.interactable}` | {nameof(CanvasGroup.ignoreParentGroups)} `{cg.ignoreParentGroups}`");

            var gr = child.GetComponent<GraphicRaycaster>();
            if (gr != null) Plugin.Logger.LogDebug($"{indent}└ {nameof(GraphicRaycaster)}: {nameof(GraphicRaycaster.ignoreReversedGraphics)} `{gr.ignoreReversedGraphics}` | {nameof(GraphicRaycaster.blockingObjects)} `{gr.blockingObjects}`");

            var gp = child.GetComponent<Graphic>();
            if (gp != null) Plugin.Logger.LogDebug($"{indent}└ {nameof(Graphic)}: {nameof(Graphic.raycastTarget)} `{gp.raycastTarget}`");

            var tp = child.GetComponent<TooltipProvider>();
            if (tp != null) Plugin.Logger.LogDebug($"{indent}└ {nameof(TooltipProvider)}: {nameof(TooltipProvider.titleToken)} {tp.titleToken}");

            if (child.parent != null) Walk(child.parent, level + 1);
        }
    }
}
#endif
