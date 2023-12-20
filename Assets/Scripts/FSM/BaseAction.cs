using UnityEngine;

namespace StrengthInNumber.AI
{
    /// <summary>
    /// Abstract action class
    /// </summary>
    public abstract class BaseAction : ScriptableObject
    {
        public abstract void Execute();
    }
}
