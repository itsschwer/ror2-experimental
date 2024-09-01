namespace Experimental.Debugging.UI
{
    internal sealed class Action<T>(UnityEngine.KeyCode key, System.Action<T> action, string description)
    {
        public readonly UnityEngine.KeyCode key = key;
        private readonly System.Action<T> action = action;
        public readonly string description = description;

        public bool PerformIfPossible(T target)
        {
            if (!UnityEngine.Input.GetKeyDown(key)) return false;

            action.Invoke(target);
            return true;
        }
    }
}
