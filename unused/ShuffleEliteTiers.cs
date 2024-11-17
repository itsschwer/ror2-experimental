using RoR2;
using System;
using System.Linq;
using Plugin = Eater.Plugin;

namespace itsschwer.Junk
{
    internal static class ShuffleEliteTiers
    {
        internal static void Init() {
            RoR2Application.onLoad += Apply;
        }

        private static void Apply()
        {
            // Based on:
            // https://thunderstore.io/package/acats/EliteCostFix/source/
            // https://thunderstore.io/package/arimah/PerfectedLoop/
            //   https://github.com/arimah/PerfectedLoop/blob/master/PerfectedLoop.cs

            CombatDirector.EliteTierDef baseTier = null;
            CombatDirector.EliteTierDef gildedTier = null;
            CombatDirector.EliteTierDef loopTier = null;

            if (CombatDirector.eliteTiers == null) { Plugin.Logger.LogError($"{nameof(ShuffleEliteTiers)}> No elite tiers!"); return; }
            foreach (CombatDirector.EliteTierDef tier in CombatDirector.eliteTiers) {
                if (baseTier == null) {
                    if (tier.eliteTypes.Contains(RoR2Content.Elites.Fire) && !tier.eliteTypes.Contains(DLC2Content.Elites.Aurelionite)) {
                        baseTier = tier;
                        Plugin.Logger.LogDebug($"{nameof(ShuffleEliteTiers)}> found {nameof(baseTier)}");
                    }
                }

                if (gildedTier == null) {
                    if (tier.eliteTypes.Contains(DLC2Content.Elites.Aurelionite)) {
                        gildedTier = tier;
                        Plugin.Logger.LogDebug($"{nameof(ShuffleEliteTiers)}> found {nameof(gildedTier)}");
                    }
                }

                if (loopTier == null) {
                    if (tier.eliteTypes.Contains(RoR2Content.Elites.Poison) && tier.eliteTypes.Contains(RoR2Content.Elites.Haunted)) {
                        loopTier = tier;
                        Plugin.Logger.LogDebug($"{nameof(ShuffleEliteTiers)}> found {nameof(loopTier)}");
                    }
                }
            }

            // gildedTier.costMultiplier = baseTier.costMultiplier; // Revert elites being cheaper to spawn once Gilded elites become available(?)

            try {
                gildedTier.eliteTypes[Array.IndexOf(gildedTier.eliteTypes, DLC2Content.Elites.Aurelionite)] = RoR2Content.Elites.Lunar; // Replace Gilded Elites with Perfected Elites (pre-loop)
            }
            catch (Exception e) { Plugin.Logger.LogError(e); }

            Plugin.Logger.LogMessage($"~{nameof(ShuffleEliteTiers)}");
            RoR2Application.onLoad -= Apply;
        }
    }
}
