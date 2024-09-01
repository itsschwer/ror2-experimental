using UnityEngine;

namespace Experimental
{
    internal static class RectTransformExtension
    {
        /// <summary>
        /// Resets the RectTransform's local transform and sets its layer to "UI".
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static RectTransform ResetRectTransform(this RectTransform rect)
        {
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.gameObject.layer = LayerMask.NameToLayer("UI");
            return rect;
        }

        public static RectTransform AnchorStretchStretch(this RectTransform rect) => rect.AnchorStretchStretch(Vector2.zero);
        public static RectTransform AnchorStretchStretch(this RectTransform rect, Vector2 uniformMargin)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = uniformMargin;
            return rect;
        }
    }
}
