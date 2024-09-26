namespace Eater
{
    internal static class UnityExplorerEater
    {
        internal static void Apply()
        {
            On.RoR2.UI.SurvivorIconController.GetLocalUser += SurvivorIconController_GetLocalUser;
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
    }
}
