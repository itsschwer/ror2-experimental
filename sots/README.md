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
- IL hooking `RoR2.PlayerCharacterMasterController.Update` may be preferrable to limit the set of body inputs that are modified?
```cs
[HarmonyPostfix, HarmonyPatch(typeof(RoR2.PlayerCharacterMasterController), nameof(RoR2.PlayerCharacterMasterController.CanSendBodyInput))]
private static void PlayerCharacterMasterController_CanSendBodyInput(bool __result, ref bool onlyAllowMovement)
{
    if (!__result) return;
    onlyAllowMovement = false;
}
```

## notes
- haven't uploaded the source code yet — please report any issues to https://github.com/itsschwer/ror2-experimental/issues
- not thoroughly tested
    - works in multiplayer
        - appears to be client-side *(i.e. not required by host; host having it does not affect others)*
        - appears to generate a lot of the following log message in the console on the host player (not sure of cause/fix):
            ```
            [Warning: Unity Log] Instance not found when handling Command message [netId=   ]
            ```
