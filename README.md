# Sprinting on the Scoreboard
> *please don't make me fall out of the sky when looking at the scoreboard as artificer*

> Only for the *Seekers of the Storm* patch!

re-enables sprinting (and jumping, and other body inputs) while the scoreboard is open.

### body inputs?
- skills
- interact
- jump
- sprint
- activate equipment
- ping

## implementation
```cs
[HarmonyPostfix, HarmonyPatch(typeof(RoR2.PlayerCharacterMasterController), nameof(RoR2.PlayerCharacterMasterController.CanSendBodyInput))]
private static void PlayerCharacterMasterController_CanSendBodyInput(bool __result, ref bool onlyAllowMovement)
{
    if (!__result) return;
    onlyAllowMovement = false;
}
```
