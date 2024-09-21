# name changer

A client-side mod that allows you to change your in-game\* name without having to change your Steam/Epic profile name.

*\*does not affect the lobby player list or "user connected" messages.*

## why?

I saw a request for it in the modding Discord server.

## notes
- client-side means that this mod will only affect you* — other players will still see your Steam/Epic profile name
    - the name replacement may be shown to other players if you are the host and have mods that print `NetworkUser.userName` *(e.g. server broadcast chat messages)*
        - similarly, the name replacement may not be applied on messages from the host that use `NetworkUser.userName`
