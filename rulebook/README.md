# rulebook item blacklist

- exposes item rules
    - always show the item icon (instead of a padlock icon when disabled)
- exposes equipment rules
- exposes stage order rule (meander)
<!--  -->
- modifies item and equipment rule summaries to only show disabled items and equipment (to reduce scrolling)

## todo
- add scroll bar to vote popout panel when there are too many choices?
- config descriptions
- rename?
- proper readme
- manifest description

## issues
- if an expansion-locked item is blacklisted, that item will appear as a disabled choice (padlock) in Command Cubes, rather than not appearing at all

## see also

### alternatives
- [RulebookUnlocker](https://thunderstore.io/package/Anreol/RulebookUnlocker/) <sup>[*src*](https://github.com/Anreol/RulebookUnlocker)</sup> by [Anreol](https://thunderstore.io/package/Anreol/) — a less "invasive" implementation; prevents blacklisting items in Eclipse
- [ItemBlacklist](https://thunderstore.io/package/Thrayonlosa/ItemBlacklist/) by [Thrayonlosa](https://thunderstore.io/package/Thrayonlosa/) — a more "advanced" implementation:
    - contains a separate Printer blacklist
    - applies blacklist to yellow (boss) items
    - allows changing the blacklist mid-run (via console commands)
    - fixes Halcyon Shrines failing to drop rewards if too many *Seekers of the Storm* items are disabled

### technical
- [R2API Rulebook](https://thunderstore.io/package/RiskofThunder/R2API_Rulebook/) <sup>[*src*](https://github.com/risk-of-thunder/R2API/blob/3d211189e043abaf597491fec93457c5f8a0ca24/R2API.Rules/RuleCatalogExtras.cs#L209)</sup> by [RiskofThunder](https://thunderstore.io/package/RiskofThunder/) — used as a reference for finding an entry point into modifying the `RuleCatalog`
