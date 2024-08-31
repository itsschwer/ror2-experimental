# Newt Alternative
*less exploitable bazaar — \[server-side \]*

*[ **!** ]* not thoroughly tested; untested in multiplayer

## effect
1. each lunar bud can only be opened once per loop
    - prevent users with edited lunar coins from amassing large amounts of *(specific)* lunar items *(through rerolling)*
2. newt altars cost 0 lunar coins
    - counterbalance the lunar bud nerf
    - make seeking newt altars a bit more enticing

## setup
mainly made up of Harmony patches, so create a `BaseUnityPlugin` with:
```cs
private void Awake() {
    new Harmony(Info.Metadata.GUID).PatchAll();
}
```
