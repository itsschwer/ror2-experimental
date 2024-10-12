#if DEBUG
using HarmonyLib;
using RoR2;

namespace itsschwer.Junk
{
    /// <remarks>
    /// <see href="https://github.com/Moffein/RiskyFixes/blob/061e5b91013f377c381e36ce99f1a260adef4884/RiskyFixes/Fixes/General/ModdedPrismaticTrials.cs"/>
    /// </remarks>
    [HarmonyPatch]
    internal static class AllowPrismaticTrials
    {
        [HarmonyPrefix, HarmonyPatch(typeof(WeeklyRun), nameof(WeeklyRun.ClientSubmitLeaderboardScore))]
        private static bool WeeklyRun_ClientSubmitLeaderboardScore() => false;

        [HarmonyPrefix, HarmonyPatch(typeof(DisableIfGameModded), nameof(DisableIfGameModded.OnEnable))]
        private static bool DisableIfGameModded_OnEnable() => false;
    }
}
#endif