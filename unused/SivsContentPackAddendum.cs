using itsschwer.Items.Helpers;
using System.Runtime.CompilerServices;

using SivsItems = SivsContentPack.Content.Items;

namespace itsschwer.Junk
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
            if (!SivsItems.DropYellowItemOnKillUsed.canRemove) Plugin.Logger.LogWarning("Changing canRemove of Trophy Hunter's Medallion no longer necessary");
            else SivsItems.DropYellowItemOnKillUsed.canRemove = false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void AddReplenishTransformation()
        {
            itsschwer.Items.MendConsumedTransformations.Register(new ReplenishTransformation(SivsItems.DropYellowItemOnKillUsed, SivsItems.DropYellowItemOnKill));
        }
    }
}
