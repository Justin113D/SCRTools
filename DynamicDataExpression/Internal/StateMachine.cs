using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.DynamicDataExpression.Internal
{
    internal struct State
    {
        public string conditions;
        public int nextState;

        // null = nothing
        // false = add to values
        // true = add to operators
        // string = error message
        public object operation;

        public State(string conditions, int nextState, object operation)
        {
            this.conditions = conditions;
            this.nextState = nextState;
            this.operation = operation;
        }
    }

    internal class StateMachine
    {
        private static readonly State[][] _states = new State[][]
        {
            new State[] // 0: Open Bracket "(", invert operator "!", "-"
            {
                // Common post-operator //
                new("(", 0, true),
                new("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 2, true),
                new("0123456789", 7, true),
                new("!-#", 6, true),

                new(null, -1, "Expected value expression")
            },
            new State[] // 1: Closed bracket ")"
            {
                // Common pre-operator //
                new("=+-*/%^|&", 0, false),
                new(")", 1, false),
                new("<>", 4, false),
                new("!", 5, false),

                new(null, -1, "Expected operator expression")
            },
            new State[] // 2: Value expression (Key)
            {
                new("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 1, null),
                new("0123456789", 2, null),

                // Common pre-operator //
                new("=+-*/%^|&", 0, false),
                new(")", 1, false),
                new("<>", 4, false),
                new("!", 5, false),

                new(null, -1, "Expected value or operator expression")
            },
            new State[] // 3: Value expression (ID)
            {
                new("0123456789", 2, null),

                // Common pre-operator //
                new("=+-*/%^|&", 0, false),
                new(")", 1, false),
                new("<>", 4, false),
                new("!", 5, false),

                new(null, -1, "Expected value or operator expression")
            },
            new State[] // 4: Smaller "<", Greater ">"
            {
                // Common post-operator //
                new("(", 0, true),
                new("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 2, true),
                new("0123456789", 7, true),
                new("!-#", 6, true),
                //////////////////////////
                
                new("=", 0, null),

                new(null, -1, "Expected value expression or \"=\"")
            },
            new State[] // 5: Unequals Start "!"
            {
                new("=", 0, null),
                new(null, -1, "Expected \"=\"")
            },
            new State[] // 6: Inverted "-", "!", "#"
            {
                new("(", 0, null),
                new("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 2, null),
                new("0123456789", 7, null),

                new(null, -1, "Expected value expression")
            },
            new State[] // 7: (floating point) Number, pre point
            {
                new("0123456789", 7, null),
                new(".", 8, null),

                // Common pre-operator //
                new("=+-*/%^|&", 0, false),
                new(")", 1, false),
                new("<>", 4, false),
                new("!", 5, false),

                new(null, -1, "Expected value or operator expression")
            },
            new State[] // 8: (floating point) Number, present point
            {
                new("0123456789", 9, null),
                new(null, -1, "Expected a digit")
            },
            new State[] // 9: (floating point) Number, post point
            {
                new("0123456789", 9, null),

                // Common pre-operator //
                new("=+-*/%^|&", 0, false),
                new(")", 1, false),
                new("<>", 4, false),
                new("!", 5, false),

                new(null, -1, "Expected value or operator expression")
            }
        };

        private int _currentState;

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
            State[] currentStates = _states[_currentState];
            for(int i = 0; i < currentStates.Length; i++)
            {
                State state = currentStates[i];
                if(state.conditions == null)
                    throw new DynamicDataExpressionException($"Unexpected character: \"{c}\"\n {state.operation}", index);
                else if(state.conditions.Contains(c))
                {
                    _currentState = state.nextState;
                    if(state.operation == null)
                        return null;
                    else
                        return (bool)state.operation;
                }
            }

            throw new InvalidOperationException("Error while looking up next state!");
        }

        /// <summary>
        /// Checks whether the machine is in a finished state
        /// </summary>
        /// <returns></returns>
        public bool IsExitState()
            => _currentState is >= 1 and <= 3 or 7 or 9;

        public string GetCurrentErrorMessage()
            => (string)_states[_currentState][^1].operation;
    }
}
