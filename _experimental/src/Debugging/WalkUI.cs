#if DEBUG
using RoR2.UI;
using UnityEngine.UI;
using UnityEngine;
using System.Text;

namespace Experimental.Debugging
{
    public static class WalkUI
    {
        internal static void Walk(RoR2.NetworkUser _, string[] args)
        {
            if (args.Length == 2) {
                switch (args[1]) {
                    default: break;
                    case "diff":
                        Walk(GameObject.FindObjectOfType<CurrentDifficultyIconController>().transform, new StringBuilder().AppendLine($"{nameof(WalkUI)}: difficulty icon"));
                        return;
                    case "obj":
                        Walk(GameObject.FindObjectOfType<ObjectivePanelController>().GetComponentInChildren<HGTextMeshProUGUI>().transform, new StringBuilder().AppendLine($"{nameof(WalkUI)}: objective panel"));
                        return;
                    case "dmg":
                        Walk(GameObject.Find("DamageEntry").transform, new StringBuilder().AppendLine($"{nameof(WalkUI)}: damage log entry"));
                        return;
                }
            }

            PressureDrop.ChatCommander.OutputFail(args[0],
                "( <style=cSub>diff</style> | <style=cSub>obj</style> | <style=cSub>dmg</style> )");
        }

        public static void Walk(Transform child, StringBuilder sb, int level = 0)
        {
            string indent = new(' ', level * 4);
            sb.AppendLine($"{indent}{child.name} <{child.gameObject.layer}>");

            var cv = child.GetComponent<Canvas>();
            if (cv != null) sb.AppendLine($"{indent}└─ {nameof(Canvas)}: {nameof(Canvas.overrideSorting)} `{cv.overrideSorting}` | {nameof(Canvas.sortingLayerName)} `{cv.sortingLayerName}` | {nameof(Canvas.renderMode)} `{cv.renderMode}`");

            var cg = child.GetComponent<CanvasGroup>();
            if (cg != null) sb.AppendLine($"{indent}└─ {nameof(CanvasGroup)}: {nameof(CanvasGroup.blocksRaycasts)} `{cg.blocksRaycasts}` | {nameof(CanvasGroup.interactable)} `{cg.interactable}` | {nameof(CanvasGroup.ignoreParentGroups)} `{cg.ignoreParentGroups}`");

            var gr = child.GetComponent<GraphicRaycaster>();
            if (gr != null) sb.AppendLine($"{indent}└─ {nameof(GraphicRaycaster)}: {nameof(GraphicRaycaster.ignoreReversedGraphics)} `{gr.ignoreReversedGraphics}` | {nameof(GraphicRaycaster.blockingObjects)} `{gr.blockingObjects}`");

            var gp = child.GetComponent<Graphic>();
            if (gp != null) sb.AppendLine($"{indent}└─ {nameof(Graphic)}: {nameof(Graphic.raycastTarget)} `{gp.raycastTarget}`");

            var tp = child.GetComponent<TooltipProvider>();
            if (tp != null) sb.AppendLine($"{indent}└─ {nameof(TooltipProvider)}: {nameof(TooltipProvider.titleToken)} {tp.titleToken}");

            if (child.parent != null) Walk(child.parent, sb, level + 1);
            else Plugin.Logger.LogDebug(sb.ToString());
        }
    }
}
#endif
