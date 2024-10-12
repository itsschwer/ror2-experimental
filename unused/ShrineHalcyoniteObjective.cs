using RoR2;

namespace itsschwer.Junk
{
    /// <summary>
    /// An attempt at a client-side objective for charging Halcyon Shrine.
    /// </summary>
    internal static class ShrineHalcyoniteObjective
    {
        internal static void Enable()
        {
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteActivatedState.OnEnter += ShrineHalcyoniteActivatedState_OnEnter;
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteBaseState.FixedUpdate += ShrineHalcyoniteBaseState_FixedUpdate; ;
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteMaxQuality.OnEnter += ShrineHalcyoniteMaxQuality_OnEnter;
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteFinished.OnEnter += ShrineHalcyoniteFinished_OnEnter;
        }

        internal static void Disable()
        {
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteActivatedState.OnEnter -= ShrineHalcyoniteActivatedState_OnEnter;
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteBaseState.FixedUpdate -= ShrineHalcyoniteBaseState_FixedUpdate; ;
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteMaxQuality.OnEnter -= ShrineHalcyoniteMaxQuality_OnEnter;
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteFinished.OnEnter -= ShrineHalcyoniteFinished_OnEnter;
        }

        private static void ShrineHalcyoniteActivatedState_OnEnter(On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteActivatedState.orig_OnEnter orig, EntityStates.ShrineHalcyonite.ShrineHalcyoniteActivatedState self)
        {
            orig(self);

            var obj = self.gameObject.AddComponent<GenericObjectiveProvider>();
            obj.objectiveToken = $"Charge the <style=cShrine>{Language.GetString(self.parentShrineReference.purchaseInteraction.displayNameToken)}</style>";

            Plugin.Logger.LogDebug("halcyon start");
        }

        private static void ShrineHalcyoniteBaseState_FixedUpdate(On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteBaseState.orig_FixedUpdate orig, EntityStates.ShrineHalcyonite.ShrineHalcyoniteBaseState self)
        {
            orig(self);

            var obj = self.gameObject.GetComponent<GenericObjectiveProvider>();
            if (obj) {
                if (self.parentShrineReference != null) {
                    if (self.parentShrineReference.purchaseInteraction != null) {
                        if (self.parentShrineReference.isDraining) {
                            float estimatedChargePercent = (self.parentShrineReference.goldMaterialModifier + 2.2f) / (9.6f + 2.2f);
                            obj.objectiveToken = $"Charge the <style=cShrine>{Language.GetString(self.parentShrineReference.purchaseInteraction.displayNameToken)}</style> ({UnityEngine.Mathf.Clamp01(estimatedChargePercent):0%})";
                        }
                    }
                    else Plugin.Logger.LogWarning("halcyon no purchase interaction");
                }
                else Plugin.Logger.LogWarning("halcyon no shrine");
            }
        }

        private static void ShrineHalcyoniteMaxQuality_OnEnter(On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteMaxQuality.orig_OnEnter orig, EntityStates.ShrineHalcyonite.ShrineHalcyoniteMaxQuality self)
        {
            orig(self);

            var old = self.gameObject.GetComponent<GenericObjectiveProvider>();
            old.objectiveToken = $"Charge the <style=cShrine>{Language.GetString(self.parentShrineReference.purchaseInteraction.displayNameToken)}</style> ({1:0%})"; // Ensure 100% is displayed
            UnityEngine.Object.Destroy(old); // Mark the charge objective as completed
            var obj = self.gameObject.AddComponent<GenericObjectiveProvider>();
            if (obj) {
                obj.objectiveToken = $"Defeat the guardian of the <style=cShrine>{Language.GetString(self.parentShrineReference.purchaseInteraction.displayNameToken)}</style>";
            }

            Plugin.Logger.LogDebug("halcyon fight");
        }

        private static void ShrineHalcyoniteFinished_OnEnter(On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteFinished.orig_OnEnter orig, EntityStates.ShrineHalcyonite.ShrineHalcyoniteFinished self)
        {
            orig(self);

            UnityEngine.Object.Destroy(self.gameObject.GetComponent<GenericObjectiveProvider>());

            Plugin.Logger.LogDebug("halcyon end");
        }
    }
}
