using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PascalCompiler.Core.Modules;

namespace PascalCompiler.Core
{
    public class Compiler
    {
        private readonly ICompilerContext _context;
        private readonly IoModule _ioModule;
        private readonly LexicalAnalyzerModule _lexicalAnalyzerModule;

        public Compiler(ICompilerContext context)
        {
            _context = context;
            _ioModule = new IoModule(context);
            _lexicalAnalyzerModule = new LexicalAnalyzerModule(context, _ioModule);
        }

        public void Start()
        {
            while (!_context.IsEnd)
            {
                _lexicalAnalyzerModule.NextSymbol();
            }
        }
    }
}
