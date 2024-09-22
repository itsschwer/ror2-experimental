using PressureDrop;
using RoR2;
using Stage = Experimental.Helpers.Stage;

namespace Experimental
{
    internal static class Cheatsheet
    {
        public static string currentDisplay;

        public static void Parse(NetworkUser _, string[] args)
        {
            if (!UnityEngine.Networking.NetworkServer.active) return;

            if (args.Length == 2) {
                string display = Parse(args[1]);
                if (display != null) {
                    currentDisplay = display;
                    return;
                }
            }
            ChatCommander.OutputFail(args[0], "(clear | stages | PressureDrop | DamageLog)");
        }

        private static string Parse(string arg)
        {
            return arg switch {
                "clear" => "",
                "stages" => Stage.DumpStyledDisplayNames(),
                "PressureDrop" => PressureDrop,
                "DamageLog" => DamageLog,
                "ServerSider" => ServerSider,
                _ => null,
            };
        }




        public const string PressureDrop =
            "PressureDrop:\n" +
            "\t- goolake (pressure plates)\n" +
            "\t- moon2 (dropping, no teleporter)";

        public const string DamageLog =
            "DamageLog:\n" +
            "\t- Shrine of Blood attacker portrait\n" +
            "\t- Void Cradle & Void Potential attacker portrait\n" +
            "\t- stages:\n" +
            "\t\t- artifactworld (Artifact Reliquary attacker portrait)\n" +
            "\t\t- goolake (Pot damage)\n" +
            "\t\t- frozenwall (Fusion Cell damage)\n" +
            "\t\t- sulfurpools (Sulfur Pod damage)\n" +
            "\t\t- arena (Void Fog (portrait, tooltip colour, text colour))\n" +
            "\t\t\t- make sure to disable player immortality (since non-lethal flag affects fog damage detection)" +
            "\t- damage types:\n" +
            "\t\t- player damage (tooltip colour, text colour)\n" +
            "\t\t- fall damage (portrait, tooltip colour, text colour)\n" +
            "\t- attacker counts as separate damage source if became Voidtouched\n" +
            "\t- reset Damage Log on revive";

        public const string ServerSider =
            "ServerSider:\n" +
            "\t- moon2 (rescue ship loop portal)" +
            "\t\t- force escape sequence" +
            "\t- arena (void field fog tweak)";
    }
}
