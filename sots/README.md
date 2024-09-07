## <mark>**Deprecation Notice**</mark>
<mark>***This mod is no longer necessary, as the change appears to be reverted in patch `v1.3.2`.***</mark>

- while deprecated, the latest version of this mod can still be used if playing on `v1.3.1`
    - this mod should have no effect if left installed on `v1.3.2`
        - uninstalling is recommended to clean up mod lists

### technical
- `onlyAllowMovement` appears to always be discarded now

# Sprinting on the Scoreboard
> *please don't make me fall out of the sky when looking at the scoreboard as artificer*

> Only for the *Seekers of the Storm*<sup>*(v1.3.1)*</sup> patch!

re-enables sprinting (and jumping, and other body inputs) while the scoreboard is open.

### body inputs?
- skills
- interact
- jump
- sprint
- activate equipment
- ping

## implementation
- IL hooking `RoR2.PlayerCharacterMasterController.Update` may be preferrable to limit the set of body inputs that are allowed?
```cs
[HarmonyPostfix, HarmonyPatch(typeof(RoR2.PlayerCharacterMasterController), nameof(RoR2.PlayerCharacterMasterController.CanSendBodyInput))]
private static void PlayerCharacterMasterController_CanSendBodyInput(bool __result, ref bool onlyAllowMovement)
{
    if (!__result) return;
    onlyAllowMovement = false;
}
```

## notes
- not thoroughly tested *— please report any issues to https://github.com/itsschwer/ror2-experimental/issues*
    - works in multiplayer
        - appears to be client-side *(i.e. not required by host; host having it does not affect others)*
        - appears to generate a lot of the following log message in the console on the host player (not sure of cause/fix):
            ```
            [Warning: Unity Log] Instance not found when handling Command message [netId=   ]
            ```
            - only if client who has it installed is playing with a host who does not have it installed?
