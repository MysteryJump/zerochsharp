using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ZerochSharp.Models.ExtensionLanguage;

namespace ZerochSharp.Test.ExtensionLanguage
{
    public class LexerTest
    {
        [Fact]
        public void Test1()
        {
            var lexer = new Lexer("++++++++");
            lexer.Lex();

        }
    }
}
