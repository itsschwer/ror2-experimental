using System.Runtime.CompilerServices;
using itsschwer.Items.Helpers;
using SivsItems = SivsContentPack.Content.Items;

namespace Eater.Compat
{
    internal static class SivsContentPackAddendum
    {
        internal static bool SivsContentPackEnabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(SivsContentPack.SivsContentPack.PluginGUID);
        internal static bool itsschwerItemsEnabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(itsschwer.Items.Plugin.GUID);

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void Apply()
        {
            if (!SivsContentPackEnabled) return;

            FixForPressureDrop();
            if (itsschwerItemsEnabled) AddReplenishTransformation();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void FixForPressureDrop()
        {
            if (!SivsItems.DropYellowItemOnKillUsed.canRemove) Plugin.Logger.LogWarning("SivsItems.DropYellowItemOnKillUsed.canRemove is already false");
            else SivsItems.DropYellowItemOnKillUsed.canRemove = false;

            if (!SivsItems.GlassShieldBroken.canRemove) Plugin.Logger.LogWarning("SivsItems.GlassShieldBroken.canRemove is already false");
            else SivsItems.GlassShieldBroken.canRemove = false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void AddReplenishTransformation()
        {
            itsschwer.Items.MendConsumedTransformations.Register(new ReplenishTransformation(SivsItems.GlassShieldBroken, SivsItems.GlassShield));
            itsschwer.Items.MendConsumedTransformations.Register(new ReplenishTransformation(SivsItems.DropYellowItemOnKillUsed, SivsItems.DropYellowItemOnKill));
        }
    }
}
