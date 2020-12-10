using System;
using System.Collections.Generic;
using System.Linq;

namespace ZerochSharp.Models.ExtensionLanguage
{
    public class Lexer
    {
        private int _index;
        private readonly string _expression;
        private readonly List<Atomic> _atomics;

        public IEnumerable<Atomic> Atomics => _atomics;

        public Lexer(string expr)
        {

            _atomics = new List<Atomic>();
            _expression = expr;
            _index = 0;
        }
        private char NextChar() => _expression[_index++];
        private char NowChar() => _expression[_index];

        private bool IsEoL() => _index >= _expression.Length;


        private void SkipSpaces()
        {
            while (!IsEoL() && IsSpace())
            {
                _index++;
            }
        }

        private bool IsSpace() => _expression[_index] == ' ';

        private DigitsAtomic GetDigitsAtomic()
        {
            var number = 0L;
            while (!IsEoL() && !IsSpace())
            {
                var c = NextChar();
                var num = c - 48L;
                if (num >= 0 && num < 10)
                {
                    number = number * 10 + num;
                }
                else
                {
                    if (IsOperator(c) || IsBracket(c))
                    {
                        _index--;
                        break;
                    }
                    else
                    {
                        throw new InvalidOperationException("don't use number-first variable or constant");
                    }
                }
            }
            return new DigitsAtomic(number);
        }

        private ConstantsAtomic GetConstantAtomic()
        {
            var str = "";
            while (!IsEoL() && !IsSpace() && !IsOperator())
            {
                var c = NextChar();
                str += c;
            }
            return new ConstantsAtomic(str);
        }
        private OperatorAtomic GetOperatorAtomic()
        {
            var first = NextChar();
            if (first == '>' || first == '<')
            {
                if (IsEoL())
                {
                    return new OperatorAtomic(first == '>' ? OperatorType.Greater : OperatorType.Less);
                }
                var second = NextChar();
                if (second == '=')
                {
                    if (first == '>')
                    {
                        return new OperatorAtomic(OperatorType.GreaterOrEqual);
                    }
                    else
                    {
                        return new OperatorAtomic(OperatorType.LessOrEqual);
                    }
                }
                else
                {
                    _index--;
                    if (first == '>')
                    {
                        return new OperatorAtomic(OperatorType.Greater);
                    }
                    else
                    {
                        return new OperatorAtomic(OperatorType.Less);
                    }
                }
            }
            else
            {
                var type = first switch
                {
                    '=' => OperatorType.Equal,
                    '+' => OperatorType.Addition,
                    '-' => OperatorType.Division,
                    '*' => OperatorType.Multiplication,
                    '/' => OperatorType.Division,
                    '&' => OperatorType.And,
                    '|' => OperatorType.Or,
                    '!' => OperatorType.Not,
                    '^' => OperatorType.Xor,
                    _ => throw new InvalidOperationException("not found such a operator")
                };
                return new OperatorAtomic(type);
            }
        }
        private BracketAtomic GetBracketAtomic(ref int level)
        {
            var c = NextChar();
            if (c == '(' || c == ')')
            {
                if (c == '(')
                {
                    return new BracketAtomic(BracketType.Left, ++level);
                }
                else
                {
                    return new BracketAtomic(BracketType.Right, level--);
                }
            }
            else
            {
                throw new InvalidOperationException("this bracket is not defined");
            }
        }
        private bool IsDigitsAtomic() => IsDigitsAtomic(NowChar());
        private bool IsDigitsAtomic(char c) => c - 48 >= 0 && c - 48 < 10;
        private bool IsOperator() => IsOperator(NowChar());
        private bool IsOperator(char c)
        {
            var targetChars = new[]
            {
                '>','=','<','+','-','*','/','&','|','!','^'
            };
            return targetChars.Contains(c);
        }
        private bool IsBracket(char c) => c == '(' || c == ')';
        private bool IsBracket() => IsBracket(NowChar());
        public void Lex()
        {
            var currentLevel = 0;
            while (!IsEoL())
            {
                if (IsDigitsAtomic())
                {
                    _atomics.Add(GetDigitsAtomic());
                }
                else if (IsOperator())
                {
                    _atomics.Add(GetOperatorAtomic());
                }
                else if (IsBracket())
                {
                    _atomics.Add(GetBracketAtomic(ref currentLevel));
                }
                else
                {
                    _atomics.Add(GetConstantAtomic());
                }
                SkipSpaces();
            }
        }
    }
}
