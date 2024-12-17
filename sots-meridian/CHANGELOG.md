### 1.0.2
- Downgrade `GeodeShatter` (`RemoveGeodeBuffFromAllPlayers`) IL error severity
    - Game patch v1.3.6 fixes the NRE, so the IL hook will fail to match and will not be applied — the warning can be ignored

### 1.0.1
- Update for game patch v1.3.5
    - Fix `MissingMethodException` when attempting to leash-teleport drones
- Rework broken drone positioning
    - Position flush to the ground *(rather than floating in the air)*
    - Place on circle perimeter *(rather than random positions within circle)*
- Code refactoring

# 1.0.0
- Initial release
