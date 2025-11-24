using System.Text;
using UnityEngine.Events;

namespace Experimental.Dumps
{
    public static class UnityEventPersistentCalls
    {
        public static string Dump(UnityEventBase unityEvent)
        {
            StringBuilder sb = new();
            sb.AppendLine($"{unityEvent} ({unityEvent.m_Calls.m_RuntimeCalls.Count} non-persistent)");
            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++) {
                sb.AppendLine($"\t[{i}] {unityEvent.GetPersistentMethodName(i)} | {unityEvent.GetPersistentTarget(i)}");
                sb.AppendLine($"\t\t{unityEvent.m_PersistentCalls.GetListener(i).arguments.stringArgument}");
            }
            return sb.ToString();
        }
    }
}
