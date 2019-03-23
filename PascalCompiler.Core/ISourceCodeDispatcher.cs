using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core
{
    public interface ISourceCodeDispatcher
    {
        bool IsEnd { get; }
        void WriteLine(string line);
        string ReadLine();
        void Close();
    }
}
