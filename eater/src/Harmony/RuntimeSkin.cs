using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace Eater.Harmony
{
    /// <summary>
    /// Fixes vanilla <see cref="NullReferenceException"/> in <see cref="RoR2.SkinDef.RuntimeSkin.Apply"/> [chained <c>transform.Find().GetComponent()</c>].
    /// </summary>
    /// <remarks>
    /// e.g. Without this patch, <c>KanadeFan-Kanade_Yoisaki_Artificer-1.0.0</c> will throw an NRE when selected in the lobby and then spam the following error message until the skin is deselected or a run starts:
    /// <code>
    /// [Error  : Unity Log] SkinnedMeshRenderer: Rendering stopped because the data for mesh 'Yoisaki Kanade_mesh.001' on Game Object 'MageMesh' does not match the expected mesh data size and vertex stride.
    /// </code>
    /// </remarks>
    // [Error  : Unity Log] NullReferenceException: Object reference not set to an instance of an object
    // Stack trace:
    // RoR2.SkinDef+RuntimeSkin.Apply(UnityEngine.GameObject modelObject) (at<d40ed1fc32a5436f8b992e36da98a05d>:IL_01BD)
    // (wrapper dynamic-method) RoR2.SkinDef.DMD<RoR2.SkinDef::Apply>(RoR2.SkinDef, UnityEngine.GameObject)
    // (wrapper dynamic-method) MonoMod.Utils.DynamicMethodDefinition.Trampoline<RoR2.SkinDef::Apply>?-603622934(RoR2.SkinDef, UnityEngine.GameObject)
    // RuneFoxMods.DynamicSkins.DynamicSkinManager.SkinDefApply(System.Action`2[T1, T2] orig, RoR2.SkinDef self, UnityEngine.GameObject modelObject) (at<2fd45abb8ff24a8cb848e9fa9f21b8dd>:IL_0001)
    // (wrapper dynamic-method) MonoMod.Utils.DynamicMethodDefinition.Hook<RoR2.SkinDef::Apply>?1887537628(RoR2.SkinDef, UnityEngine.GameObject)
    // RoR2.SurvivorMannequins.SurvivorMannequinSlotController.ApplyLoadoutToMannequinInstance() (at<d40ed1fc32a5436f8b992e36da98a05d>:IL_0062)
    // RoR2.SurvivorMannequins.SurvivorMannequinSlotController.Update() (at<d40ed1fc32a5436f8b992e36da98a05d>:IL_0069)
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
                c.Emit(OpCodes.Ldloc, 14);
                c.Emit(OpCodes.Ldfld, typeof(SkinDef.MeshReplacementTemplate).GetField(nameof(SkinDef.MeshReplacementTemplate.path)));
                c.Emit(OpCodes.Ldloc_0);
                c.EmitDelegate<Func<Transform, string, Transform, bool>>((transform, path, mdlTransform) => {
                    bool nullTransform = (transform == null);
                    if (nullTransform) Debug.LogWarning($"Could not find transform \"{path}\" relative to \"{mdlTransform.name}\".");
                    return nullTransform;
                });
                c.Emit(OpCodes.Brfalse, original);
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Br, skip);
                c.MarkLabel(original);
#if DEBUG
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(RuntimeSkin)}> Cannot hook {nameof(SkinDef_RuntimeSkin_Apply)}: failed to match IL instructions.");
        }
    }
}
