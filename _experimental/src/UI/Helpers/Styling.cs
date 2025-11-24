using UnityEngine;

namespace Experimental.UI.Helpers
{
    public static class Styling
    {

        public static string GenerateColoredString(string str, RoR2.ColorCatalog.ColorIndex colorIndex)
        {
            Color32 color = RoR2.ColorCatalog.GetColor(colorIndex);
            return RoR2.Util.GenerateColoredString(str, color);
        }

        public static string PrettyPrint(this Vector3 vector3) => vector3.PrettyPrint("F3");
        public static string PrettyPrint(this Vector3 vector3, string format)
        {
            Color32 r = RoR2.ColorCatalog.GetColor(RoR2.ColorCatalog.ColorIndex.Tier3Item);
            Color32 g = RoR2.ColorCatalog.GetColor(RoR2.ColorCatalog.ColorIndex.Tier2Item);
            Color32 b = RoR2.ColorCatalog.GetColor(RoR2.ColorCatalog.ColorIndex.LunarItem);
            return $"({RoR2.Util.GenerateColoredString(vector3.x.ToString(format), r)} / {RoR2.Util.GenerateColoredString(vector3.y.ToString(format), g)} / {RoR2.Util.GenerateColoredString(vector3.z.ToString(format), b)})";
        }
    }
}
