using System.Collections.Generic;

namespace StrengthInNumber.AI
{
    /// <summary>
    /// A finite state machine class with state, action and transition based on ScriptableObject
    /// Transition to default state, transitions from any state, transition to exit state logics are implemented directly into the framework
    /// </summary>
    public sealed class FiniteStateMachine
    {
        public State current;
        public BaseTransition transitionToEntry;
        public List<BaseTransition> transitionsFromAnyState = new List<BaseTransition>();

        public bool transitionToEntryFinished = false;
        public bool active;

        public void Execute()
        {
            if(!active)
            {
                return;
            }

            // Ensure entering default state once
            if(!transitionToEntryFinished)
            {
                transitionToEntry.to.enter = true;
                current = transitionToEntry.to;
                transitionToEntryFinished = true;
            }

            // Handle transitions from any state. Takes priority over transitions from current state
            for (int i = 0; i < transitionsFromAnyState.Count; i++)
            {
                if (transitionsFromAnyState[i].CanTransit())
                {
                    transitionsFromAnyState[i].to.enter = true;
                    break;
                }
            }

            // Handle exit state
            if (current == null)
            {
                active = false;
                return;
            }

            // Handle current state transitions and execution
            current.Execute();
        }
    }
}
