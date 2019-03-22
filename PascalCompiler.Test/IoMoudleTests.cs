using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PascalCompiler.Core;
using PascalCompiler.Core.Modules;

namespace PascalCompiler.Test
{
    [TestClass]
    public class IoMoudleTests
    {
        [TestMethod]
        public void IoModuleLineNumberRightOrder()
        {
            var text = new Queue<string>(new [] {"foo", "bar", "baz"});
            var context = new TestCompilerContext(text);

            var ioModule = new IoModule(context);
            while (!context.IsEnd)
            {
                ioModule.NextChar();
            }

            Assert.AreEqual("   1  foo", context.Result[0]);
            Assert.AreEqual("   2  bar", context.Result[1]);
            Assert.AreEqual("   3  baz", context.Result[2]);
        }

        [TestMethod]
        public void IoModuleErrorRightListing()
        {
            var text = new Queue<string>(new[] { "foo" });
            var context = new TestCompilerContext(text);

            var ioModule = new IoModule(context);
            context.OnError(new TextPosition { CharNumber = 1 }, 111);
            while (!context.IsEnd)
            {
                ioModule.NextChar();
            }

            Assert.AreEqual("   1  foo", context.Result[0]);
            Assert.AreEqual("*001*  ^ошибка код 111", context.Result[1]);
            Assert.AreEqual("***** несовместимость с типом дискриминанта", context.Result[2]);
        }
   
        [TestMethod]
        public void IoModuleErrorRightOrder()
        {
            var text = new Queue<string>(new[] { "foo" });
            var context = new TestCompilerContext(text);

            var ioModule = new IoModule(context);
            var i = 0;
            while (!context.IsEnd)
            {
                ioModule.NextChar();
                context.OnError(new TextPosition { CharNumber = i++ }, 111);
            }

            Assert.AreEqual("   1  foo", context.Result[0]);
            Assert.AreEqual("*001* ^ошибка код 111", context.Result[1]);
            Assert.AreEqual("***** несовместимость с типом дискриминанта", context.Result[2]);
            Assert.AreEqual("*002*  ^ошибка код 111", context.Result[3]);
            Assert.AreEqual("***** несовместимость с типом дискриминанта", context.Result[4]);
            Assert.AreEqual("*003*   ^ошибка код 111", context.Result[5]);
            Assert.AreEqual("***** несовместимость с типом дискриминанта", context.Result[6]);
        }
    }
}
