using System;
using System.Collections.Generic;

namespace SCR.Tools.DynamicDataExpression.Internal.Expression
{
    internal enum CheckOperator
    {
        Or,     // |
        And,    // &
        Equals,     // =
        Unequals,   // !=
        Greater,    // >
        GreaterEquals,  // >=
        Smaller,        // <
        SmallerEquals,  // <=
        Add,        // +
        Subtract,   // -
        Multiply,   // *
        Divide,     // /
        Modulo,     // %
        Exponent,   // ^
        Invert,     // !
        Negate,     // -
    }
    internal static class EnumExtensions
    {
        private static readonly Dictionary<CheckOperator, int> OperatorSignificance = new()
        {
            { CheckOperator.Or, 0 },
            { CheckOperator.And, 1 },
            { CheckOperator.Equals, 2 },
            { CheckOperator.Unequals, 2 },
            { CheckOperator.Greater, 3 },
            { CheckOperator.GreaterEquals, 3 },
            { CheckOperator.Smaller, 3 },
            { CheckOperator.SmallerEquals, 3 },
            { CheckOperator.Add, 4 },
            { CheckOperator.Subtract, 4 },
            { CheckOperator.Multiply, 5 },
            { CheckOperator.Divide, 5 },
            { CheckOperator.Modulo, 5 },
            { CheckOperator.Exponent, 6 }
        };

        public static CheckOperator ToOperator(string op)
        {
            return op switch
            {
                "|" => CheckOperator.Or,
                "&" => CheckOperator.And,
                "==" => CheckOperator.Equals,
                "!=" => CheckOperator.Unequals,
                ">" => CheckOperator.Greater,
                ">=" => CheckOperator.GreaterEquals,
                "<" => CheckOperator.Smaller,
                "<=" => CheckOperator.SmallerEquals,
                "+" => CheckOperator.Add,
                "-" => CheckOperator.Subtract,
                "*" => CheckOperator.Multiply,
                "/" => CheckOperator.Divide,
                "%" => CheckOperator.Modulo,
                "^" => CheckOperator.Exponent,
                _ => throw new InvalidCastException($"Invalid operator: {op}"),
            };
        }

        public static string ToStringExtension(this CheckOperator op)
        {
            return op switch
            {
                CheckOperator.Or => "|",
                CheckOperator.And => "&",
                CheckOperator.Equals => "==",
                CheckOperator.Unequals => "!=",
                CheckOperator.Greater => ">",
                CheckOperator.GreaterEquals => ">=",
                CheckOperator.Smaller => "<",
                CheckOperator.SmallerEquals => "<=",
                CheckOperator.Add => "+",
                CheckOperator.Subtract => "-",
                CheckOperator.Multiply => "*",
                CheckOperator.Divide => "/",
                CheckOperator.Modulo => "%",
                CheckOperator.Exponent => "^",
                CheckOperator.Invert => "!",
                CheckOperator.Negate => "-",
                _ => throw new InvalidCastException($"Invalid operator: {op}"),
            };
        }

        public static bool Needs2Operands(this CheckOperator op)
            => op != CheckOperator.Invert && op != CheckOperator.Negate;

        public static KeyType EvalType(this CheckOperator op, KeyType keytype)
        {
            if (keytype == KeyType.Boolean && op == CheckOperator.Invert)
                return KeyType.Boolean;
            else if (keytype == KeyType.Number && op == CheckOperator.Negate)
                return KeyType.Number;

            return KeyType.None;
        }

        public static KeyType EvalType(this CheckOperator op, KeyType left, KeyType right)
        {
            if (left == KeyType.Boolean && right == KeyType.Boolean)
            {
                switch (op)
                {
                    case CheckOperator.Or:
                    case CheckOperator.And:
                    case CheckOperator.Equals:
                    case CheckOperator.Unequals:
                        return KeyType.Boolean;
                }
            }
            else if (left == KeyType.Number && right == KeyType.Number)
            {
                switch (op)
                {
                    case CheckOperator.Equals:
                    case CheckOperator.Unequals:
                    case CheckOperator.Greater:
                    case CheckOperator.GreaterEquals:
                    case CheckOperator.Smaller:
                    case CheckOperator.SmallerEquals:
                        return KeyType.Boolean;
                    case CheckOperator.Add:
                    case CheckOperator.Subtract:
                    case CheckOperator.Multiply:
                    case CheckOperator.Divide:
                    case CheckOperator.Modulo:
                    case CheckOperator.Exponent:
                        return KeyType.Number;
                }
            }
            else if (left == KeyType.NumberList && right == KeyType.Number)
            {
                switch (op)
                {
                    case CheckOperator.Equals:
                    case CheckOperator.Unequals:
                    case CheckOperator.Greater:
                    case CheckOperator.GreaterEquals:
                    case CheckOperator.Smaller:
                    case CheckOperator.SmallerEquals:
                        return KeyType.Boolean;
                }
            }
            return KeyType.None;
        }

        public static bool HasLowerPrecedence(this CheckOperator current, CheckOperator other)
        {
            int lSign = OperatorSignificance[current];
            int rSign = OperatorSignificance[other];

            int result = lSign - rSign;
            if (result == 0 && current == CheckOperator.Exponent)
                return true;

            return result < 0;
        }

    }
}
