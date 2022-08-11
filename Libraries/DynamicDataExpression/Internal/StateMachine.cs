using System;
using System.Collections.Generic;

namespace SCR.Tools.DynamicDataExpression.Internal
{
    internal abstract class StateMachine
    {
        private const string _legalChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789. -!#|&<>=+-*/%^()";

        public static readonly HashSet<char> LegalChars;

        static StateMachine()
        {
            LegalChars = new HashSet<char>();
            for (int i = 0; i < _legalChars.Length; i++)
                LegalChars.Add(_legalChars[i]);
        }

        public struct MachineState
        {
            public string conditions;
            public int nextState;

            /// <summary> 
            /// null = nothing <br/>
            /// false = add to values <br/>
            /// true = add to operators <br/>
            /// </summary>
            public bool? operation;

            public MachineState(string conditions, int nextState, bool? operation)
            {
                this.conditions = conditions;
                this.nextState = nextState;
                this.operation = operation;
            }
        }

        protected abstract MachineState[][] States { get; }

        protected int _currentState;

        public abstract bool IsExitState { get; }

        /// <summary>
        /// Updates the next state based on the character parameter. <br/>
        /// Return true if the state concluded the value to be a value expression. <br/>
        /// False if its an operation expression. 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool? NextState(char c, int index)
        {
            MachineState[] currentStates = States[_currentState];
            for (int i = 0; i < currentStates.Length; i++)
            {
                MachineState state = currentStates[i];
                if (state.nextState == -1)
                    throw new DynamicDataExpressionException($"Unexpected character: \"{c}\"\n {state.conditions}", index);
                else if (state.conditions.Contains(c))
                {
                    _currentState = state.nextState;
                    return state.operation;
                }
            }

            throw new InvalidOperationException("Error while looking up next state!");
        }

        public string GetCurrentErrorMessage()
            => States[_currentState][^1].conditions;
    }
}
