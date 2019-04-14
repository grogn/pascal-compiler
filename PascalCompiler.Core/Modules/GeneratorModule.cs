using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PascalCompiler.Core.Constants;
using PascalCompiler.Core.Structures;

namespace PascalCompiler.Core.Modules
{
    public static class GeneratorModule
    {
        private static readonly ILGenerator _generator;
        private static readonly TypeBuilder _typeBuilder;
        private static readonly AssemblyBuilder _assemblyBuilder;
        private static readonly MethodBuilder _methodBuilder;
        private static readonly Dictionary<string, LocalBuilder> _variables;
        public static bool NoCode { get; set; }


        static GeneratorModule()
        {
            var appDomain = Thread.GetDomain();
            var assemblyName = new AssemblyName();
            assemblyName.Name = Path.GetFileNameWithoutExtension("build.exe");
            _assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = _assemblyBuilder.DefineDynamicModule("build.exe", "build.exe");
            moduleBuilder.CreateGlobalFunctions();
            _typeBuilder = moduleBuilder.DefineType("Build");
            _methodBuilder = _typeBuilder.DefineMethod("Run", MethodAttributes.Public | MethodAttributes.Static, typeof(void), null);
            _generator = _methodBuilder.GetILGenerator();
            _variables = new Dictionary<string, LocalBuilder>();
        }

        public static void Flush()
        {
            if (NoCode) return;
            _generator.Emit(OpCodes.Ret);
            _typeBuilder.CreateType();
            _assemblyBuilder.SetEntryPoint(_methodBuilder);
            _assemblyBuilder.Save("build.exe");
        }

        public static void AddLocal(string name, System.Type type)
        {
            if (NoCode) return;
            _variables[name] = _generator.DeclareLocal(type);
        }

        public static void AddLocalInt(string name)
        {
            AddLocal(name, typeof(int));
        }

        public static void AddLocalChar(string name)
        {
            AddLocal(name, typeof(char));
        }

        public static void AddLocalBoolean(string name)
        {
            AddLocal(name, typeof(bool));
        }

        public static void AddLocalReal(string name)
        {
            AddLocal(name, typeof(double));
        }

        public static void Assignment(string name)
        {
            if (NoCode) return;
            _generator.Emit(OpCodes.Stloc, _variables[name]);
        }

        public static void Assignment(string name, int value)
        {
            if (NoCode) return;
            _generator.Emit(OpCodes.Ldc_I4, value);
            _generator.Emit(OpCodes.Stloc, _variables[name]);
        }

        public static void Assignment(string name, char value)
        {
            if (NoCode) return;
            _generator.Emit(OpCodes.Ldc_I4, value);
            _generator.Emit(OpCodes.Stloc, _variables[name]);
        }

        public static void Assignment(string name, bool value)
        {
            _generator.Emit(OpCodes.Ldc_I4, value ? 1 : 0);
            _generator.Emit(OpCodes.Stloc, _variables[name]);
        }
        
        public static void Assignment(string name, double value)
        {
            if (NoCode) return;
            _generator.Emit(OpCodes.Ldc_I4, value);
            _generator.Emit(OpCodes.Stloc, _variables[name]);
        }

        public static void WriteLineInteger()
        {
            if (NoCode) return;
            var writeLine = typeof(Console).GetMethod("WriteLine", new[] { typeof(int) });
            _generator.EmitCall(OpCodes.Call, writeLine, null);
        }

        public static void WriteLineReal()
        {
            if (NoCode) return;
            var writeLine = typeof(Console).GetMethod("WriteLine", new[] { typeof(float) });
            _generator.EmitCall(OpCodes.Call, writeLine, null);
        }
        public static void WriteLineBool()
        {
            if (NoCode) return;
            var writeLine = typeof(Console).GetMethod("WriteLine", new[] { typeof(bool) });
            _generator.EmitCall(OpCodes.Call, writeLine, null);
        }

        public static void PushConst(int value)
        {
            if (NoCode) return;
            _generator.Emit(OpCodes.Ldc_I4, value);
        }

        public static void PushConst(double value)
        {
            if (NoCode) return;
            _generator.Emit(OpCodes.Ldc_R8, value);
        }

        public static void PushConst(char value)
        {
            if (NoCode) return;
            _generator.Emit(OpCodes.Ldc_I4, value);
        }

        public static void ConvertToReal()
        {
            if (NoCode) return;
            _generator.Emit(OpCodes.Conv_R8);
        }
        
        public static void PushVariable(string name)
        {
            if (NoCode) return;
            _generator.Emit(OpCodes.Ldloc, _variables[name]);
        }

        public static void PushOperation(int operation)
        {
            if (NoCode) return;
            if (operation == Symbols.Plus)
                _generator.Emit(OpCodes.Add);
            if (operation == Symbols.Minus)
                _generator.Emit(OpCodes.Sub);
            if (operation == Symbols.Slash)
                _generator.Emit(OpCodes.Div);
            if (operation == Symbols.Star)
                _generator.Emit(OpCodes.Mul);
            if (operation == Symbols.Equal)
                _generator.Emit(OpCodes.Ceq);
            if (operation == Symbols.Latergreater)
            {
                _generator.Emit(OpCodes.Ceq);
                _generator.Emit(OpCodes.Ldc_I4_0);
                _generator.Emit(OpCodes.Ceq);
            }
            if (operation == Symbols.Later)
                _generator.Emit(OpCodes.Clt);
            if (operation == Symbols.Greater)
                _generator.Emit(OpCodes.Cgt);
            if (operation == Symbols.Greaterequal)
            {
                _generator.Emit(OpCodes.Clt);
                _generator.Emit(OpCodes.Ldc_I4_0);
                _generator.Emit(OpCodes.Ceq);
            }
            if (operation == Symbols.Laterequal)
            {
                _generator.Emit(OpCodes.Cgt);
                _generator.Emit(OpCodes.Ldc_I4_0);
                _generator.Emit(OpCodes.Ceq);
            }
            if (operation == Keywords.Andsy)
            {
                _generator.Emit(OpCodes.And);
            }
            if (operation == Keywords.Orsy)
            {
                _generator.Emit(OpCodes.Or);
            }
            if (operation == Keywords.Notsy)
            {
                _generator.Emit(OpCodes.Ldc_I4_0);
                _generator.Emit(OpCodes.Ceq);
            }
        }
    }
}
