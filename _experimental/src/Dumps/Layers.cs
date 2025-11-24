using System.Text;
using UnityEngine;

namespace Experimental.Dumps
{
    internal static class Layers
    {
        public static string Dump()
        {
            const int layers = 32;
            StringBuilder sb = new("Layers:\n");
            for (int i = 0; i < layers; i++) {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName)) {
                    sb.AppendLine($"\t[{i}] {layerName}");
                }
            }
            return sb.ToString();
        }
    }
}
