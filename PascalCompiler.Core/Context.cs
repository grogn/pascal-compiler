using PascalCompiler.Core.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core
{
    public class Context
    {
        public event Action<Error> Error;

        public bool IsEnd => SourceCodeDispatcher.IsEnd;

        public char Char { get; set; }
        public int CharNumber { get; set; }

        public string Line { get; set; }
        public int LineNumber { get; set; }

        public int ErrorNumber { get; set; }

        public string SymbolName { get; set; }
        public Symbol Symbol { get; set; }
        public int SymbolCode { get; set; }
        public int SymbolPosition { get; set; }

        /// <summary>
        /// Таблица имен
        /// </summary>
        public SymbolTable SymbolTable { get; }

        public Scope LocalScope { get; set; }

        public ISourceCodeDispatcher SourceCodeDispatcher;

        public Context(ISourceCodeDispatcher sourceCodeDispatcher)
        {
            SourceCodeDispatcher = sourceCodeDispatcher;
            LineNumber = 1;
            ErrorNumber = 1;
            SymbolTable = new SymbolTable();
            LocalScope = new Scope();
        }

        public void OnError(Error error)
        {
            Error?.Invoke(error);
        }
    }
}
