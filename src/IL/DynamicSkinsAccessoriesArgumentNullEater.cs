using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace Eater.IL
{
    /// <summary>
    /// Each dynamic skin appears to log a warning and an error each time an enemy spawns:
    /// [Warning: ] An error occured while adding accessories to a skin
    /// [Error  : ] System.ArgumentNullException: Value cannot be null.
    /// Parameter name: key
    ///   at System.Collections.Generic.Dictionary`2[TKey,TValue].FindEntry (TKey key) [0x00008] in <44afb4564e9347cf99a1865351ea8f4a>:IL_0008
    ///   at System.Collections.Generic.Dictionary`2[TKey,TValue].TryGetValue (TKey key, TValue& value) [0x00000] in <44afb4564e9347cf99a1865351ea8f4a>:IL_0000
    ///   at RuneFoxMods.DynamicSkins.DynamicSkinManager.SkinDefApply (System.Action`2[T1,T2] orig, RoR2.SkinDef self, UnityEngine.GameObject modelObject) [0x00021] in <8473c696cf1b4a4ab37b53521bae4ebc>:IL_0021
    /// </summary>
    internal static class DynamicSkinsAccessoriesArgumentNullEater
    {
        const string target = "RuneFoxMods.DynamicSkins.DynamicSkinManager";

        internal static void Apply()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Type type = assembly.GetType(target);
                if (type != null) {
                    Log.Info($"Found dynamic skin in {assembly.FullName}");
                    MethodInfo method = type.GetMethod("SkinDefApply", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (method != null) {
                        Apply(method);
                    }
                }
            }
        }

        private static void Apply(MethodBase method)
        {
            ILHook hook = new(method, (il) => {
                ILCursor c = new(il);
                c.GotoNext(
                    // InstanceLogger.LogWarning((object)"An error occured while adding accessories to a skin");
                    x => x.MatchLdstr("An error occured while adding accessories to a skin") // IL_0093
                );
                c.Index -= 2; // To before IL_008d: ldarg.0
                c.Emit(OpCodes.Ldloc, 7);                              // Load exception to stack (Ldloc instead of Ldloc_S since latter expects byte?)
                c.Emit(OpCodes.Isinst, typeof(ArgumentNullException)); // ex is ArgumentNullException

                ILLabel catchEnd = c.MarkLabel();
                c.Emit(OpCodes.Brtrue_S, catchEnd); // Branch if true
                c.GotoNext(            // Find instructions at end of catch block
                    x => x.MatchNop(), // IL_00ab
                    x => x.MatchNop()  // IL_00ac
                );
                c.MarkLabel(catchEnd); // Setup label at end of catch block

                Log.Info($"Applied ILHook to {method.DeclaringType.FullName}.{method.Name}");
            });
        }
    }
}
