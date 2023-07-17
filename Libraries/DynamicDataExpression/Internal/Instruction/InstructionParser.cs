using SCR.Tools.DynamicDataExpression.Internal;
using SCR.Tools.DynamicDataExpression.Internal.Expression;
using System.Collections.Generic;

namespace SCR.Tools.DynamicDataExpression.Internal.Instruction
{
    internal static class InstructionParser
    {
        public static DataInstruction ParseInstruction(
            string instruction,
            IReadOnlyDictionary<string, DataKey> setterKeys,
            IReadOnlyDictionary<string, DataKey> accessorKeys)
        {
            for (int i = 0; i < instruction.Length; i++)
            {
                if (!StateMachine.LegalChars.Contains(instruction[i]))
                {
                    throw new DynamicDataExpressionException($"Illegal character \"{instruction[i]}\"", i);
                }
            }

            int instructionLength = GetInstructionPart(instruction);

            ParseInstructionPart(
                instruction[..instructionLength],
                out string key,
                out long? id,
                out InstructionType type);

            DataKey dataSetter = setterKeys[key];
            KeyType dataType = id == null
                ? dataSetter.NoIDType
                : dataSetter.IDType;

            string dataExpressionString = instruction[instructionLength..].PadLeft(instruction.Length);
            DataExpression dataExpression = ExpressionParser.ParseExpression(dataExpressionString, accessorKeys, dataType, false);

            return new(key, id, type, dataExpression);
        }

        public static void Verify(string instruction)
        {
            for (int i = 0; i < instruction.Length; i++)
            {
                if (!StateMachine.LegalChars.Contains(instruction[i]))
                {
                    throw new DynamicDataExpressionException($"Illegal character \"{instruction[i]}\"", i);
                }
            }

            int instructionLength = GetInstructionPart(instruction);

            ParseInstructionPart(instruction[..instructionLength], out _, out _, out _);

            string dataExpressionString = instruction[instructionLength..].PadLeft(instruction.Length);
            ExpressionParser.Verify(dataExpressionString, false);
        }

        public static int GetInstructionPart(string instruction)
        {
            InstructionStateMachine sm = new();

            int instructionLength = 0;

            for (int i = 0; i < instruction.Length; i++)
            {
                char c = instruction[i];

                if (char.IsWhiteSpace(c))
                    continue;

                sm.NextState(c, i);

                if (sm.IsExitState)
                {
                    instructionLength = i + 1;
                    break;
                }
            }

            if (instructionLength == 0)
            {
                throw new DynamicDataExpressionException("Invalid Instruction! Instruction must end with a \"=\", followed up by a data expression", -1);
            }

            return instructionLength;
        }

        public static void ParseInstructionPart(string instructionPart, out string key, out long? id, out InstructionType type)
        {
            string keyPart = "";
            string idPart = "";
            string typePart = "";
            int mode = 0;

            for (int i = 0; i < instructionPart.Length; i++)
            {
                char c = instructionPart[i];

                if (char.IsWhiteSpace(c))
                    continue;

                switch (mode)
                {
                    case 0:
                        if (char.IsLetter(c))
                        {
                            keyPart += c;
                        }
                        else if (char.IsNumber(c))
                        {
                            idPart += c;
                            mode = 1;
                        }
                        else
                        {
                            typePart += c;
                            mode = 2;
                        }
                        break;
                    case 1:
                        if (char.IsNumber(c))
                        {
                            idPart += c;
                        }
                        else
                        {
                            typePart += c;
                            mode = 2;
                        }
                        break;
                    case 2:
                        typePart += c;
                        break;
                }
            }

            key = keyPart;

            id = idPart.Length == 0 ? null : long.Parse(idPart);

            type = typePart switch
            {
                "+=" => InstructionType.add,
                "-=" => InstructionType.subtract,
                "*=" => InstructionType.multiply,
                "/=" => InstructionType.divide,
                "%=" => InstructionType.modulo,
                "^=" => InstructionType.exponent,
                "&=" => InstructionType.and,
                "|=" => InstructionType.or,
                _ => InstructionType.set,
            };
        }


    }
}
