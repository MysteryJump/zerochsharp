using System;
using System.Collections.Generic;

namespace ZerochSharp.Models.ExtensionLanguage
{
    class OperatorAtomic : Atomic
    {
        public OperatorType OpType { get; }
        public NTerm Term { get; private set; }
        public Atomic LeftAtomic { get; private set; }
        public Atomic RightAtomic { get; private set; }
        public bool IsBinded { get; private set; } = false;
        public object EvaluatedValue { get; private set; }
        public bool IsCalced { get; set; } = false;
        private Dictionary<string, int> constants;
        public OperatorAtomic(OperatorType type)
        {
            atomicString = type.ToString();
            OpType = type;
            Term = type switch
            {
                OperatorType.Not => NTerm.Unary,
                _ => NTerm.Binomial
            };
        }
        public void Bind(Atomic right, Atomic left = null)
        {
            // Unary
            if (left == null)
            {
                if (Term == NTerm.Binomial)
                {
                    throw new InvalidOperationException();
                }
                RightAtomic = right;
            }
            // Binomial
            else
            {
                if (Term == NTerm.Unary)
                {
                    throw new InvalidOperationException();
                }
                RightAtomic = right;
                LeftAtomic = left;
            }
            IsBinded = true;
        }
        public void Calc(Dictionary<string, int> constantTable)
        {
            constants = constantTable;
            if (!IsBinded)
            {
                throw new InvalidOperationException("You need to parse and bind atomic before calc.");
            }
            var right = GetChildValue(RightAtomic);
            if (Term == NTerm.Unary)
            {
                if (OpType == OperatorType.Not)
                {
                    if (right is int)
                    {
                        throw new InvalidOperationException("cannot apply not operator to number.");
                    }
                    EvaluatedValue = !(bool)right;
                }
                else
                {
                    throw new InvalidOperationException("invalid operator: unary");
                }
            }
            else
            {
                var left = GetChildValue(LeftAtomic);
                if (right is bool rBoolVal && left is bool lBoolVal)
                {
                    EvaluatedValue = OpType switch
                    {
                        OperatorType.And => rBoolVal & lBoolVal,
                        OperatorType.Or => rBoolVal | lBoolVal,
                        OperatorType.Xor => rBoolVal ^ lBoolVal,
                        _ => throw new InvalidOperationException("this operator doesn't support boolean type")
                    };
                }
                else if (right is int rNumVal && left is int lNumVal)
                {
                    EvaluatedValue = OpType switch
                    {
                        OperatorType.Addition => rNumVal + lNumVal,
                        OperatorType.Subtraction => lNumVal - rNumVal,
                        OperatorType.Multiplication => lNumVal * rNumVal,
                        OperatorType.Division => rNumVal / lNumVal,
                        OperatorType.Equal => rNumVal == lNumVal,
                        OperatorType.Greater => lNumVal > rNumVal,
                        OperatorType.GreaterOrEqual => lNumVal >= rNumVal,
                        OperatorType.Less => lNumVal < rNumVal,
                        OperatorType.LessOrEqual => lNumVal <= rNumVal,
                        _ => throw new InvalidOperationException("this operator doesn't support number value")
                    };
                }
                else
                {
                    throw new InvalidOperationException("in binomial operator, left and right object must be same type");
                }
            }
        }
        private object GetChildValue(Atomic atomic)
        {
            if (atomic is DigitsAtomic digiAtom)
            {
                return digiAtom.Digits;
            }
            else if (atomic is ConstantsAtomic constAtom)
            {
                if (constants.ContainsKey(constAtom.ConstantName))
                {
                    return constants[constAtom.ConstantName];
                }
                else
                {
                    throw new InvalidOperationException($"constant table doesn't contains {constAtom.ConstantName}");
                }
            }
            else
            {
                if (!(atomic is OperatorAtomic atom))
                {
                    throw new InvalidOperationException("incomplete parse");
                }
                if (!IsCalced)
                {
                    atom.Calc(constants);
                }
                return atom.EvaluatedValue;
            }
        }
        public override string ToString()
        {
            return base.ToString() + $", IsBinded: {IsBinded}";
        }
    }
}
