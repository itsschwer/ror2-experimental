using RoR2;
using PressureDrop;

namespace Experimental.Commands
{
    internal static class Commands
    {
        public static void Register()
        {
            ChatCommander.Register("/show", Cheatsheet.Parse);
            ChatCommander.Register("/setstage", Helpers.Stage.SetStage);

            ChatCommander.Register("/s", Spawn);

            ChatCommander.Register("/walkui", WalkUI.Walk);
        }

        public static void Unregister()
        {
            ChatCommander.Unregister("/show", Cheatsheet.Parse);
            ChatCommander.Unregister("/setstage", Helpers.Stage.SetStage);

            ChatCommander.Unregister("/s", Spawn);

            ChatCommander.Unregister("/walkui", WalkUI.Walk);
        }

        private static bool GetUserBody(NetworkUser user, out CharacterBody body)
        {
            body = null;
            if (Run.instance == null) return false;
            body = user?.GetCurrentBody();
            return (body != null);
        }

        private static void Spawn(NetworkUser user, string[] args)
        {
            if (!GetUserBody(user, out CharacterBody target)) return;

            if (args.Length == 2) {
                switch (args[1].ToLowerInvariant()) {
                    default: break;
                    case "s":
                    case "scrapper":
                        Debug.SpawnScrapper(target);
                        return;
                    case "p":
                    case "printer":
                        Debug.SpawnPrinter(target);
                        return;
                    case "c":
                    case "cauldron":
                    case "forge":
                        Debug.SpawnCauldron(target);
                        return;
                    case "b":
                    case "blue":
                    case "portal":
                    case "blueportal":
                        Debug.SpawnBluePortal(target);
                        return;
                    case "d":
                    case "damage":
                        Debug.SpawnDamagingInteractables(target);
                        return;
                }
            }

            ChatCommander.OutputFail(args[0],
                "(<style=cSub>scrapper</style> | <style=cSub>printer</style> | <style=cSub>cauldron</style> | <style=cSub>blueportal</style> | <style=cSub>damage</style>)");
        }
    }
}
