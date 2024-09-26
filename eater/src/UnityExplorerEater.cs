namespace Eater
{
    /// <summary>
    /// Error line: <c>((MPEventSystem)EventSystem.current).localUser</c>
    /// </summary>
    /// <remarks>
    /// Maybe try replace with IL hook for scalability.
    /// </remarks>
    internal static class UnityExplorerEater
    {
        internal static void Apply()
        {
            On.RoR2.UI.SurvivorIconController.GetLocalUser += SurvivorIconController_GetLocalUser;
            On.RoR2.UI.RuleChoiceController.FindNetworkUser += RuleChoiceController_FindNetworkUser;
        }

        private static RoR2.LocalUser SurvivorIconController_GetLocalUser(On.RoR2.UI.SurvivorIconController.orig_GetLocalUser orig, RoR2.UI.SurvivorIconController self)
        {
            try {
                return orig(self);
            }
            catch (System.InvalidCastException) {
                return RoR2.LocalUserManager.GetFirstLocalUser();
            }
        }

        private static RoR2.NetworkUser RuleChoiceController_FindNetworkUser(On.RoR2.UI.RuleChoiceController.orig_FindNetworkUser orig, RoR2.UI.RuleChoiceController self)
        {
            try {
                return orig(self);
            }
            catch (System.InvalidCastException) {
                return RoR2.LocalUserManager.GetFirstLocalUser()?.currentNetworkUser;
            }
        }
    }
}
