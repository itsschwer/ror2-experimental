using Experimental.Patches;
using PressureDrop;
using RoR2;

namespace Experimental
{
    internal static class Misc
    {
        public static void ToggleEnemySpawning()
        {
            bool wasDisabled = CombatDirector.cvDirectorCombatDisable.GetString() != "0";
            CombatDirector.cvDirectorCombatDisable.SetBool(!wasDisabled);

            if (wasDisabled) ChatCommander.Output("Enemy spawning enabled.");
            else ChatCommander.Output("Enemy spawning disabled.");
        }

        public static void TogglePlayerImmortality()
        {
            TakeDamagePlayerNonLethal.SetActive(!TakeDamagePlayerNonLethal.Active);
            string status = TakeDamagePlayerNonLethal.Active ? "enabled" : "disabled";
            ChatCommander.Output($"Player immortality {status}.");
        }

        public static void ForceChargeTeleporter()
        {
            if (TeleporterInteraction.instance == null) return;

            TeleporterInteraction.instance.holdoutZoneController.charge = 0.99f;
        }

        public static void ForceMoonEscapeSequence()
        {
            EscapeSequenceController esc = UnityEngine.Object.FindObjectOfType<EscapeSequenceController>();
            esc?.onEnterMainEscapeSequence?.Invoke();
        }

        public static void AddMountainStack()
        {
            if (TeleporterInteraction.instance == null) return;

            TeleporterInteraction.instance.AddShrineStack();
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "SHRINE_BOSS_USE_MESSAGE", paramTokens = [Language.GetString("UNIDENTIFIED_KILLER_NAME")] });
            // Effect from RoR2.ShrineBossBehaviour.AddShrineStack()
            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<UnityEngine.GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData {
                origin = TeleporterInteraction.instance.transform.position,
                rotation = UnityEngine.Quaternion.identity,
                scale = 1f,
                color = new UnityEngine.Color(0.7372549f, 77f / 85f, 0.94509804f)
            }, transmit: true);
        }

        public static void AddBeamsToPressurePlates()
        {
            if (TeleporterInteraction.instance == null) return;

            UnityEngine.Transform beam = TeleporterInteraction.instance.transform.Find("TeleporterBaseMesh/BuiltInEffects/ChargingEffect/BetweenProngs/Loop/Beam");

            foreach (PressurePlateController p in UnityEngine.Object.FindObjectsOfType<PressurePlateController>()) {
                UnityEngine.GameObject.Instantiate(beam, p.transform.position, UnityEngine.Quaternion.identity, p.transform);

                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<UnityEngine.GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData {
                    origin = p.transform.position,
                    rotation = p.transform.rotation,
                    scale = 1f,
                    color = new UnityEngine.Color(0.7372549f, 77f / 85f, 0.94509804f)
                }, transmit: true);

                ChestRevealer.RevealedObject.RevealObject(p.gameObject, float.MaxValue);
            }
        }
    }
}
