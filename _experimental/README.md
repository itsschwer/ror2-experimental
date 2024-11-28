# experimental

### patches
- no drop pods
- all damage to players is non-lethal

## todo
- refactor:
    - spawning interactables
    - HUD actions
    - HUD toggles — display status
- implement:
    - spawning enemies
- blender-style ~~quick~~ [pie menus](https://docs.blender.org/manual/en/latest/interface/controls/buttons/menus.html#pie-menus)?
    - or use chat → double-tap activation key (right alt?) → hook to read input and auto-display stages list → use string contains to get best match and tab-to-complete?
        - style stage dump matched text with white colour?
        - style stage dump best match as bigger size?


## Suggested syntax for displaying command usage

Entry | Meaning
---   | ---
`plain text` | Enter this literally, exactly as shown
`<argument-name>` | Placeholder, replace `<argument-name>` with an appropriate value
`[optional]` | Optional, `optional` can be omitted
`[choice-1 \| choice-2]` | Optional choice, can omit or pick either `choice-1` or `choice-2`
`(choice-1 \| choice-2)` | Required choice, must pick either `choice-1` or `choice-2`

### Other styles:
- [Minecraft](https://minecraft.wiki/w/Commands#Syntax)
- [GitHub](https://github.com/cli/cli/blob/trunk/docs/command-line-syntax.md)
