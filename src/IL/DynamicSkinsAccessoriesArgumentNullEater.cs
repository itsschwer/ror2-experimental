using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace Eater.IL
{
    /// <summary>
    /// [Warning: ] An error occured while adding accessories to a skin
    /// [Error  : ] System.ArgumentNullException: Value cannot be null.
    /// Parameter name: key
    ///   at System.Collections.Generic.Dictionary`2[TKey,TValue].FindEntry (TKey key) [0x00008] in <44afb4564e9347cf99a1865351ea8f4a>:IL_0008
    ///   at System.Collections.Generic.Dictionary`2[TKey,TValue].TryGetValue (TKey key, TValue& value) [0x00000] in <44afb4564e9347cf99a1865351ea8f4a>:IL_0000
    ///   at RuneFoxMods.DynamicSkins.DynamicSkinManager.SkinDefApply (System.Action`2[T1,T2] orig, RoR2.SkinDef self, UnityEngine.GameObject modelObject) [0x00021] in <8473c696cf1b4a4ab37b53521bae4ebc>:IL_0021
    /// </summary>
    internal static class DynamicSkinsAccessoriesArgumentNullEater
    {
        internal static void Apply()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Type type = assembly.GetType("RuneFoxMods.DynamicSkins.DynamicSkinManager");
                if (type != null) {
                    Log.Debug($"Found dynamic skin in {assembly.FullName}");
                    MethodInfo method = type.GetMethod("SkinDefApply", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (method != null) {
                        Apply(method);
                    }
                }
            }
        }

        private static void Apply(MethodBase method)
        {
            // ArgumentException: opcode
            ILHook hook = new(method, (il) => {
                ILCursor c = new(il);
                c.GotoNext(
                    x => x.MatchLdstr("An error occured while adding accessories to a skin") // IL_0093
                );
                c.Index++; // Right before loading and calling logger
                c.Emit(OpCodes.Ldloc_S, 7); // Load exception to stack
                c.Emit(OpCodes.Isinst, typeof(ArgumentNullException));

                ILLabel catchEnd = c.MarkLabel();
                c.Emit(OpCodes.Brfalse_S, catchEnd);
                c.GotoNext(
                    x => x.MatchNop(), // IL_00ab
                    x => x.MatchNop()  // IL_00ac
                );
                c.MarkLabel(catchEnd); // Skip to end of catch block

                Log.Info($"Applied ILHook to {method.DeclaringType.FullName}.{method.Name}");
            });
        }
    }
}
