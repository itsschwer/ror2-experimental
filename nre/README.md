# nreater

> **NullReferenceException: Object reference not set to an instance of an object**

Tries to fix (spammy) vanilla null reference exceptions.

## targets
- `ElusiveAntlersPickup`
    - prevent NRE spam when the owner of orb pickups spawned by Elusive Antlers dies

### todo
- observe other common NREs
- try fix `ElusiveAntlersPickup.Start`
- try fix `ElusiveAntlersPickup.OnDestroy`?
- patch out all `Debug.LogError("Can't PushPickupNotification for " + Util.GetBestMasterName(characterMaster) + " because they aren't local.");` in `CharacterMasterNotificationQueue`?
    - just kind of annoying (and harmless)
    - *but not an NRE**

## see also
- [MeridianPrimePrime](https://thunderstore.io/package/itsschwer/MeridianPrimePrime/) — fixes an NRE when cracking *Aurelionite Geodes* on *Prime Meridian*
    - <mark>fixed in vanilla **RoR2v1.3.6 [Seekers of the Storm Roadmap Phase 1 — Items & Elites]**</mark>
