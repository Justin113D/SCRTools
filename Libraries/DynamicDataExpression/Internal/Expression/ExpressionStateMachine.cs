using SCR.Tools.DynamicDataExpression.Internal;

namespace SCR.Tools.DynamicDataExpression.Internal.Expression
{
    internal class ExpressionStateMachine : StateMachine
    {
        private static readonly MachineState[][] _states = new MachineState[][]
        {
            new MachineState[] // 0: Open Bracket "(", invert operator "!", "-"
            {
                // Common post-operator //
                new("(", 0, true),
                new("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 2, true),
                new("0123456789", 7, true),
                new("!-#", 6, true),

                new("Expected value expression", -1, null)
            },
            new MachineState[] // 1: Closed bracket ")"
            {
                // Common pre-operator //
                new("+-*/%^|&", 0, false),
                new(")", 1, false),
                new("<>", 4, false),
                new("!=", 5, false),

                new("Expected operator expression", -1, null)
            },
            new MachineState[] // 2: Value expression (Key)
            {
                new("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 2, null),
                new("0123456789", 2, null),

                // Common pre-operator //
                new("+-*/%^|&", 0, false),
                new(")", 1, false),
                new("<>", 4, false),
                new("!=", 5, false),

                new("Expected value or operator expression", -1, null)
            },
            new MachineState[] // 3: Value expression (ID)
            {
                new("0123456789", 2, null),

                // Common pre-operator //
                new("+-*/%^|&", 0, false),
                new(")", 1, false),
                new("<>", 4, false),
                new("!=", 5, false),

                new("Expected value or operator expression", -1, null)
            },
            new MachineState[] // 4: Smaller "<", Greater ">"
            {
                // Common post-operator //
                new("(", 0, true),
                new("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 2, true),
                new("0123456789", 7, true),
                new("!-#", 6, true),
                //////////////////////////
                
                new("=", 0, null),

                new("Expected value expression or \"=\"", -1, null)
            },
            new MachineState[] // 5: Unequals and Equals Start "!" "="
            {
                new("=", 0, null),
                new("Expected \"=\"", -1, null)
            },
            new MachineState[] // 6: Inverted "-", "!", "#"
            {
                new("(", 0, null),
                new("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 2, null),
                new("0123456789", 7, null),

                new("Expected value expression", -1, null)
            },
            new MachineState[] // 7: (floating point) Number, pre point
            {
                new("0123456789", 7, null),
                new(".", 8, null),

                // Common pre-operator //
                new("+-*/%^|&", 0, false),
                new(")", 1, false),
                new("<>", 4, false),
                new("!=", 5, false),

                new("Expected value or operator expression", -1, null)
            },
            new MachineState[] // 8: (floating point) Number, present point
            {
                new("0123456789", 9, null),
                new("Expected a digit", -1, null)
            },
            new MachineState[] // 9: (floating point) Number, post point
            {
                new("0123456789", 9, null),

                // Common pre-operator //
                new("+-*/%^|&", 0, false),
                new(")", 1, false),
                new("<>", 4, false),
                new("!=", 5, false),

                new("Expected value or operator expression", -1, null)
            }
        };

        public override bool IsExitState
            => _currentState is >= 1 and <= 3 or 7 or 9;

        protected override MachineState[][] States => _states;

    }
}
