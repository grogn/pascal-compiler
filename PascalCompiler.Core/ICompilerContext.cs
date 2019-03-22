using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core
{
    public interface ICompilerContext
    {
        event Action<TextPosition, int> Error;
        void OnError(TextPosition position, int code);
        void WriteLine(string line);
        string ReadLine();
    }
}
