using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core
{
    public class Context
    {
        public event Action<int, int> Error;

        public bool IsEnd => SourceCodeDispatcher.IsEnd;

        public char Char { get; set; }
        public int CharNumber { get; set; }

        public string Line { get; set; }
        public int LineNumber { get; set; }

        public int ErrorNumber { get; set; }

        public string Symbol { get; set; }
        public int SymbolCode { get; set; }

        public ISourceCodeDispatcher SourceCodeDispatcher;

        public Context(ISourceCodeDispatcher sourceCodeDispatcher)
        {
            SourceCodeDispatcher = sourceCodeDispatcher;
            LineNumber = 1;
            ErrorNumber = 1;
        }

        public void OnError(int position, int code)
        {
            Error?.Invoke(position, code);
        }
    }
}
