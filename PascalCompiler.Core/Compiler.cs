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
        private Logger _logger;

        public Compiler(ISourceCodeDispatcher sourceCodeDispatcher)
        {
            _context = new Context(sourceCodeDispatcher);
            _sourceCodeDispatcher = sourceCodeDispatcher;
            _ioModule = new IoModule(_context);
            _lexicalAnalyzerModule = new LexicalAnalyzerModule(_context, _ioModule);
            _logger = new Logger("../../Test/logs.txt");
        }

        public void Log(string line)
        {
            _logger.Log(line);
        }

        public void Start()
        {
            var symbols = new List<string>();
            while (!_sourceCodeDispatcher.IsEnd)
            {
                var symbol = _lexicalAnalyzerModule.NextSymbol();
                if (_context.Symbol == "\n")
                {
                    Log(string.Join("|", symbols));
                    symbols.Clear();
                }
                else
                {
                    symbols.Add(_context.Symbol);
                }
            }
            Log(string.Join("|", symbols));
            symbols.Clear();
            _sourceCodeDispatcher.Close();
            _logger.Close();
        }
    }
}
