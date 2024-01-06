# [ Risk of Rain 2 ] mod experiments

## server-side
- various debug chat commands

# idea: Newt Alternative
*less exploitable bazaar*

## server-side
> *untested as non-host*

[test implementation](./src/Patches/NewtAlternative.cs)

- buds can only be opened once per loop
    - prevent users with edited lunar coins from amassing absurd amounts of (specific) lunar items (through rerolling)
- newt altars cost 0 lunar coins
    - balance the bud purchase limit
    - config *(default true)*

## client-side bonuses
> *untested as non-host*

- lunar see destination on ping
- show items lost to cauldron in (local) chat (excl. scrap)
    - config:
        - notify client when others do?
        - expand to printers, scrappers
        - include scrap in notify message
