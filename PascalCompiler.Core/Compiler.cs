using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PascalCompiler.Core.Modules;

namespace PascalCompiler.Core
{
    public class Compiler
    {
        private readonly ISourceCodeDispatcher _sourceCodeDispatcher;
        private readonly Context _context;
        private readonly IoModule _ioModule;
        private readonly LexicalAnalyzerModule _lexicalAnalyzerModule;
        private readonly SyntacticalAnalyzerModule _syntacticalAnalyzerModule;

        public Compiler(ISourceCodeDispatcher sourceCodeDispatcher)
        {
            _context = new Context(sourceCodeDispatcher);
            _sourceCodeDispatcher = sourceCodeDispatcher;
            _ioModule = new IoModule(_context);
            _lexicalAnalyzerModule = new LexicalAnalyzerModule(_context, _ioModule);
            _syntacticalAnalyzerModule = new SyntacticalAnalyzerModule(_context, _lexicalAnalyzerModule);
        }

        public void Start()
        {
            _syntacticalAnalyzerModule.Program();
            GeneratorModule.Flush();
            _sourceCodeDispatcher.Close();
            Logger.Close();
        }
    }
}
