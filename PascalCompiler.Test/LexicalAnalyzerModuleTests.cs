using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PascalCompiler.Core;
using PascalCompiler.Core.Constants;
using PascalCompiler.Core.Modules;

namespace PascalCompiler.Test
{
    [TestClass]
    public class LexicalAnalyzerModuleTests
    {
        private LexicalAnalyzerModule MockAnalyzer(IEnumerable<string> code)
        {
            var text = new Queue<string>(code);
            var sourceCodeDispatcher = new TestSourceCodeDispatcher(text);
            var context = new Context(sourceCodeDispatcher);

            var ioModule = new IoModule(context);
            var analyzer = new LexicalAnalyzerModule(context, ioModule);

            return analyzer;
        }

        [TestMethod]
        public void ShouldScanLaterEqual()
        {
            var analyzer = MockAnalyzer(new[] { "<="});
            
            var symbol = analyzer.NextSymbol();
            
            Assert.AreEqual(Symbols.Laterequal, symbol);
        }

        [TestMethod]
        public void ShouldScanNumber()
        {
            var analyzer = MockAnalyzer(new[] { "123" });

            var symbol = analyzer.NextSymbol();

            Assert.AreEqual(Symbols.Intc, symbol);
        }

        [TestMethod]
        public void ShouldScanNumberPlusNumber()
        {
            var analyzer = MockAnalyzer(new[] { "123+456" });

            var firstNumber = analyzer.NextSymbol();
            var plus = analyzer.NextSymbol();
            var secondNumber = analyzer.NextSymbol();

            Assert.AreEqual(Symbols.Intc, firstNumber);
            Assert.AreEqual(Symbols.Plus, plus);
            Assert.AreEqual(Symbols.Intc, secondNumber);
        }

        [TestMethod]
        public void ShouldScanNumberLaterGreaterNumber()
        {
            var analyzer = MockAnalyzer(new[] { "123<>456" });

            var firstNumber = analyzer.NextSymbol();
            var plus = analyzer.NextSymbol();
            var secondNumber = analyzer.NextSymbol();

            Assert.AreEqual(Symbols.Intc, firstNumber);
            Assert.AreEqual(Symbols.Latergreater, plus);
            Assert.AreEqual(Symbols.Intc, secondNumber);
        }

        [TestMethod]
        public void ShouldSkipSpaces()
        {
            var analyzer = MockAnalyzer(new[] { "123 <> 456" });

            var firstNumber = analyzer.NextSymbol();
            var plus = analyzer.NextSymbol();
            var secondNumber = analyzer.NextSymbol();

            Assert.AreEqual(Symbols.Intc, firstNumber);
            Assert.AreEqual(Symbols.Latergreater, plus);
            Assert.AreEqual(Symbols.Intc, secondNumber);
        }

        [TestMethod]
        public void ShouldErrorWhenNumberIsBig()
        {
            var text = new Queue<string>(new[] { "123456789" });
            var sourceCodeDispatcher = new TestSourceCodeDispatcher(text);
            var context = new Context(sourceCodeDispatcher);

            var ioModule = new IoModule(context);
            var analyzer = new LexicalAnalyzerModule(context, ioModule);

            analyzer.NextSymbol();
            analyzer.NextSymbol();

            Assert.AreEqual("   1  123456789", sourceCodeDispatcher.Result[0]);
            Assert.AreEqual("*001*         ^ошибка код 203", sourceCodeDispatcher.Result[1]);
            Assert.AreEqual("***** целая константа превышает предел", sourceCodeDispatcher.Result[2]);
        }

        [TestMethod]
        public void ShouldErrorWhenSymbolDoesNotExist()
        {
            var text = new Queue<string>(new[] { "123?456$&" });
            var sourceCodeDispatcher = new TestSourceCodeDispatcher(text);
            var context = new Context(sourceCodeDispatcher);

            var ioModule = new IoModule(context);
            var analyzer = new LexicalAnalyzerModule(context, ioModule);

            while (!context.IsEnd)
            {
                analyzer.NextSymbol();
            }

            Assert.AreEqual("   1  123?456$&", sourceCodeDispatcher.Result[0]);
            Assert.AreEqual("*001*    ^ошибка код 6", sourceCodeDispatcher.Result[1]);
            Assert.AreEqual("***** запрещенный символ", sourceCodeDispatcher.Result[2]);
            Assert.AreEqual("*002*        ^ошибка код 6", sourceCodeDispatcher.Result[3]);
            Assert.AreEqual("***** запрещенный символ", sourceCodeDispatcher.Result[4]);
            Assert.AreEqual("*003*         ^ошибка код 6", sourceCodeDispatcher.Result[5]);
            Assert.AreEqual("***** запрещенный символ", sourceCodeDispatcher.Result[6]);
        }

        [TestMethod]
        public void ShouldScanCharConstant()
        {
            var analyzer = MockAnalyzer(new[] { "'a'" });

            var symbol = analyzer.NextSymbol();

            Assert.AreEqual(Symbols.Charc, symbol);
        }

        [TestMethod]
        public void ShouldScanStringConstant()
        {
            var analyzer = MockAnalyzer(new[] { "'abc'" });

            var symbol = analyzer.NextSymbol();

            Assert.AreEqual(Symbols.Stringc, symbol);
        }

        [TestMethod]
        public void ShouldErrorWhenCharIsEmpty()
        {
            var text = new Queue<string>(new[] {"''"});
            var sourceCodeDispatcher = new TestSourceCodeDispatcher(text);
            var context = new Context(sourceCodeDispatcher);

            var ioModule = new IoModule(context);
            var analyzer = new LexicalAnalyzerModule(context, ioModule);

            while (!context.IsEnd)
            {
                analyzer.NextSymbol();
            }

            Assert.AreEqual("   1  ''", sourceCodeDispatcher.Result[0]);
            Assert.AreEqual("*001*  ^ошибка код 75", sourceCodeDispatcher.Result[1]);
            Assert.AreEqual("***** ошибка в символьной константе", sourceCodeDispatcher.Result[2]);
        }


        [TestMethod]
        public void ShouldErrorWhenStringIsTooBig()
        {
            var text = new Queue<string>(new[] { "'1234567891123sadfsadfasdf23asdghfds'" });
            var sourceCodeDispatcher = new TestSourceCodeDispatcher(text);
            var context = new Context(sourceCodeDispatcher);

            var ioModule = new IoModule(context);
            var analyzer = new LexicalAnalyzerModule(context, ioModule);

            while (!context.IsEnd)
            {
                analyzer.NextSymbol();
            }

            Assert.AreEqual("   1  '1234567891123sadfsadfasdf23asdghfds'", sourceCodeDispatcher.Result[0]);
            Assert.AreEqual("*001*                                     ^ошибка код 76", sourceCodeDispatcher.Result[1]);
            Assert.AreEqual("***** слишком длинная строковая константа", sourceCodeDispatcher.Result[2]);
        }
    }
}
