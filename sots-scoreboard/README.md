# Restore Scoreboard

reverts the change in Seekers of the Storm where generation of scoreboard strips for dead players is skipped.

**note:** only very minimal local testing was performed — should be fully client-side *(i.e. not interfere with other players who may not have the mod installed)* and have no side-effects — report any issues to the linked repository if found!

## technical
```cs
// SotS
List<PlayerCharacterMasterController> list = PlayerCharacterMasterController.instances.Where((PlayerCharacterMasterController x) => x.gameObject.activeInHierarchy && x.master.GetBody() != null && Util.GetBestMasterName(x.master) != null).ToList();
```
```cs
// pre-SotS
ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
```
