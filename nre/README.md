# nreater

> **NullReferenceException: Object reference not set to an instance of an object**

## targets
- `ElusiveAntlersPickup`
    - prevent NRE spam when the owner of orb pickups spawned by Elusive Antlers dies

## todo
- patch out all `Debug.LogError("Can't PushPickupNotification for " + Util.GetBestMasterName(characterMaster) + " because they aren't local.");` in `CharacterMasterNotificationQueue`?
    - just kind of annoying (and harmless)
