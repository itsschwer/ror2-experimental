# [ Risk of Rain 2 ] mod experiments

## client-side
> *untested as non-host*
- lunar seer destination on ping
- show items lost to cauldron in (local) chat (excl. scrap)

## server-side
- debug spawning (`ctrl`)
    - \[`s`\]crapper
    - \[`p`\]rinter
    - \[`c`\]auldron
    - \[`g`\]ive lepton daisy
    - \[`b`\]lue portal
- `ctrl`+`right alt` → eternal jellyfish




# idea: Newt Alternative
*less exploitable bazaar*

## server-side
[test implementation](./src/Patches/NewtAlternative.cs)
- buds can only be opened once per loop
    - prevent users with edited lunar coins from amassing absurd amounts of (specific) lunar items (through rerolling)
- newt altars cost 0 lunar coins
    - balance the bud purchase limit
    - config *(default true)*

## client-side bonuses
- lunar see destination on ping
- show items lost to cauldron in (local) chat
    - config:
        - notify client when others do?
        - expand to printers, scrappers
        - include scrap in notify message
