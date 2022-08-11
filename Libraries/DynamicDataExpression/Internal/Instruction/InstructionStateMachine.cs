using SCR.Tools.DynamicDataExpression.Internal;

namespace SCR.Tools.DynamicDataExpression.Internal.Instruction
{
    internal class InstructionStateMachine : StateMachine
    {
        private static readonly MachineState[][] _states = new MachineState[][]
        {
            new MachineState[] // 0: Key Start
            {
                new("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 1, null),
                new("0123456789", 2, null),

                new("Expected Key or ID expression", -1, null)
            },
            new MachineState[] // 1: Key Continuation
            {
                new("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 1, null),
                new("0123456789", 2, null),
                new("+-*/%^|&", 3, false),
                new("=", 4, false),

                new("Expected Key, ID or operator expression", -1, null)
            },
            new MachineState[] // 2: ID
            {
                new("0123456789", 2, false),
                new("+-*/%^|&", 3, false),
                new("=", 4, false),

                new("Expected ID or operator expression", -1, null)
            },
            new MachineState[] // 3: enforced equals
            {
                new("=", 4, false),

                new("Expected an \"Equals\" operator", -1, null)
            }
        };

        public override bool IsExitState => _currentState == 4;

        protected override MachineState[][] States => _states;
    }
}
