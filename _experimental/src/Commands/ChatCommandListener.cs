using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace Experimental.Commands
{
    public static class ChatCommandListener
    {
        private static readonly Dictionary<string, Action<NetworkUser, string[]>> Commands = [];

        private static bool _hooked = false;
        internal static void Hook()
        {
            if (_hooked) return;
            _hooked = true;
            On.RoR2.Console.RunCmd += Console_RunCmd;
        }

        private static void Console_RunCmd(On.RoR2.Console.orig_RunCmd orig, RoR2.Console self, RoR2.Console.CmdSender sender, string concommandName, List<string> userArgs)
        {
            string message = userArgs.FirstOrDefault();
            string[] args = message?.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // Not server, not a chat command, or not a registered command
            if (!NetworkServer.active || !concommandName.Equals("say", StringComparison.InvariantCultureIgnoreCase)
                || string.IsNullOrWhiteSpace(message) || args == null || args.Length <= 0
                || !Commands.TryGetValue(args[0], out Action<NetworkUser, string[]> command) || command == null)
            {
                orig.Invoke(self, sender, concommandName, userArgs);
                return;
            }
            // Otherwise, execute and stop propagation

            // Finish say command (add player chat message) | RoR2.Chat.CCSay()
            Chat.SendBroadcastChat(new Chat.UserChatMessage {
                sender = sender.networkUser.gameObject,
                text = message
            });
            // And execute chat command
            command.Invoke(sender.networkUser, args);
        }

        /// <summary>
        /// Registers a chat command.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="action"></param>
        /// <returns>Returns <see langword="true"/> if the chat command was successfully registered, <see langword="false"/> otherwise.</returns>
        public static bool Register(string token, Action<NetworkUser, string[]> action)
        {
            if (Commands.ContainsKey(token)) {
                Plugin.Logger.LogWarning($"{nameof(ChatCommandListener)}> A chat command is already registered under '{token}'");
                return false;
            }

            Commands[token] = action;
            string origin = action.Method.DeclaringType.Assembly.GetName().Name;
            origin = (origin != typeof(ChatCommandListener).Assembly.GetName().Name) ? $" [from {origin}.dll]" : "";
            Plugin.Logger.LogInfo($"{nameof(ChatCommandListener)}> Registering a chat command under '{token}'{origin}");
            return true;
        }

        /// <summary>
        /// Unregisters a chat command.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="action"></param>
        /// <returns>Returns <see langword="true"/> if the chat command was successfully unregistered, <see langword="false"/> otherwise.</returns>
        public static bool Unregister(string token, Action<NetworkUser, string[]> action)
        {
            if (Commands.TryGetValue(token, out Action<NetworkUser, string[]> command)) {
                if (command == action) {
                    Plugin.Logger.LogInfo($"{nameof(ChatCommandListener)}> Unregistered '{token}' chat command.");
                    Commands.Remove(token);
                    return true;
                }
                else Plugin.Logger.LogWarning($"{nameof(ChatCommandListener)}> Could not unregister chat command '{token}' as the action does not match.");
            }
            else Plugin.Logger.LogInfo($"{nameof(ChatCommandListener)}> Could not unregister chat command '{token}' (not registered).");

            return false;
        }

        /// <summary>
        /// Wrapper for sending a chat message styled as an output of a command.
        /// </summary>
        /// <param name="message"></param>
        public static void Output(string message)
            => Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>{message}</style>" });

        /// <summary>
        /// Wrapper for sending a chat message styled as an output of a failed command.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="message"></param>
        public static void OutputFail(string cmd, string message)
            => Output($"<style=cDeath>Failed:</style> <color=#ffffff>{cmd}</color> {message}");
    }
}
