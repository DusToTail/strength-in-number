using System.Collections.Generic;
using UnityEngine;

namespace StrengthInNumber.AI
{
    /// <summary>
    /// A state presented as a ScriptableObject
    /// </summary>
    public sealed class State : ScriptableObject
    {
        public List<BaseTransition> transitions = new List<BaseTransition>();
        public BaseAction onEnter;
        public BaseAction onStay;
        public BaseAction onExit;

        // true means need to do the action
        public bool enter = false;
        public bool exit = false;

        public void Execute()
        {
            // Validate exit boolean to prevent transition from this state
            // when already transitioning from any state (handled in FiniteStateMachine) 
            if (!exit && CheckTransit())
            {
                exit = true;
            }

            // Handled during previous state CheckTransit
            if(enter)
            {
                OnEnter();
                enter = false;
            }

            OnStay();

            // Handled during this state CheckTransit
            if (exit)
            {
                OnExit();
                exit = false;
            }
        }

        private void OnEnter()
        {
            onEnter?.Execute();
        }

        private void OnExit()
        {
            onExit?.Execute();
        }

        private void OnStay()
        {
            onStay?.Execute();
        }

        private bool CheckTransit()
        {
            for(int i = 0; i < transitions.Count; i++)
            {
                // Handle normal states
                if (transitions[i].CanTransit())
                {
                    // Handle exit state (transition to null state)
                    if(transitions[i].to != null)
                    {
                        transitions[i].to.enter = true;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
