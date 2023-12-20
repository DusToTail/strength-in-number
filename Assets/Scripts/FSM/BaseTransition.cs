using UnityEngine;

namespace StrengthInNumber.AI
{
    /// <summary>
    /// Abstract transition class
    /// </summary>
    public abstract class BaseTransition : ScriptableObject
    {
        public State to;
        public abstract bool CanTransit();
    }
}
