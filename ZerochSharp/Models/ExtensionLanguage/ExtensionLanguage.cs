using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Models.ExtensionLanguage
{
    public class ExtensionLanguage
    {
        public static bool EvaluatePredicate(string expression, Dictionary<string, long> constants)
        {
            var lexer = new Lexer(expression);
            lexer.Lex();
            var parser = new Parser(lexer.Atomics);
            parser.Parse();
            var value = parser.ParsedAtomic;
            if (value is OperatorAtomic opatom)
            {
                opatom.Calc(constants);
                if (opatom.EvaluatedValue is bool bVal)
                {
                    return bVal;
                }
                else
                {
                    throw new InvalidOperationException("final evaluated value must be boolean.");
                }
            }
            else
            {
                throw new InvalidOperationException("final evaluated value must be boolean.");
            }
        }

        public static bool CheckPredicate(string expression)
        {
            var lexer = new Lexer(expression);
            lexer.Lex();
            var parser = new Parser(lexer.Atomics);
            var value = parser.ParsedAtomic;
            if (value is OperatorAtomic opatom)
            {
                try
                {
                    opatom.Calc(new Dictionary<string, long>() { { "CreatedAt", 0 }, { "ModifiedAt", 0 }, { "Count", 0 }, { "Influence", 0 } });
                    if (opatom.EvaluatedValue is bool)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
