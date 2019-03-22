using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var context = new TestCompilerContext(text);

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
            var context = new TestCompilerContext(text);

            var ioModule = new IoModule(context);
            var analyzer = new LexicalAnalyzerModule(context, ioModule);

            analyzer.NextSymbol();
            analyzer.NextSymbol();

            Assert.AreEqual("   1  123456789", context.Result[0]);
            Assert.AreEqual("*001* ^ошибка код 203", context.Result[1]);
            Assert.AreEqual("***** целая константа превышает предел", context.Result[2]);
        }
    }
}
