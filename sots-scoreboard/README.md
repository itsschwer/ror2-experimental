# Restore Scoreboard

reverts the change in Seekers of the Storm where generation of scoreboard strips for dead players is skipped.

## technical
```cs
// SotS
List<PlayerCharacterMasterController> list = PlayerCharacterMasterController.instances.Where((PlayerCharacterMasterController x) => x.gameObject.activeInHierarchy && x.master.GetBody() != null && Util.GetBestMasterName(x.master) != null).ToList();
```
```cs
// pre-SotS
ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
```
