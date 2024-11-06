# Meridian Prime Prime

A  \[ server-side / host-only \] that makes *Prime Meridian* less painful in multiplayer.

## effect
- when *Aurelionite Fragments* are rewarded (after activating all *Aurelionite Geodes* on the path to the arena):
    - revive all dead players
    - leash-teleport all minions
    - teleport all broken drones
- cracking *Aurelionite Geodes* no longer desyncs in multiplayer if a player is dead
    - ***technical**: add missing `CharacterBody` null check when trying to remove the Lunar Ruin debuff*
