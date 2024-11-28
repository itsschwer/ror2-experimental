using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace Eater.Harmony
{
    [HarmonyPatch]
    internal static class RuntimeSkin
    {
        [HarmonyILManipulator, HarmonyPatch(typeof(SkinDef.RuntimeSkin), nameof(SkinDef.RuntimeSkin.Apply))]
        private static void SkinDef_RuntimeSkin_Apply(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            // Renderer component3 = transform.Find(reference3.path).GetComponent<Renderer>();
            Func<Instruction, bool>[] match = {
                x => x.MatchLdloc(0),
                x => x.MatchLdloc(14),
                x => x.MatchLdfld<SkinDef.MeshReplacementTemplate>(nameof(SkinDef.MeshReplacementTemplate.path)),
                x => x.MatchCallOrCallvirt<Transform>(nameof(Transform.Find)),
                x => x.MatchCallOrCallvirt<Component>(nameof(Component.GetComponent)),
                x => x.MatchStloc(16)
            };

            if (c.TryGotoNext(MoveType.After, match)) {
                ILLabel skip = c.MarkLabel();
                c.Index -= 2; // before GetComponent & after Transform.Find
                ILLabel original = c.MarkLabel();
                c.Emit(OpCodes.Dup);
                // c.Emit(OpCodes.Ldloc, 14);
                // c.Emit(OpCodes.Ldfld, typeof(SkinDef.MeshReplacementTemplate).GetField(nameof(SkinDef.MeshReplacementTemplate.path)));
                // c.EmitDelegate<Func<string, Transform, bool>>((path, transform) => {
                c.EmitDelegate<Func<Transform, bool>>((transform) => {
                    bool nullTransform = transform == null;
                    // if (nullTransform) Plugin.Logger.LogWarning($"{path} is invalid.");
                    return nullTransform;
                });
                c.Emit(OpCodes.Brfalse, original);
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Br, skip);
                c.MarkLabel(original);
#if DEBUG || true
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(RuntimeSkin)}> Cannot hook {nameof(SkinDef_RuntimeSkin_Apply)}: failed to match IL instructions.");
        }
    }
}
