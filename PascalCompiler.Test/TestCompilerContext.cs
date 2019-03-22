using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PascalCompiler.Core;

namespace PascalCompiler.Test
{
    public class TestCompilerContext : ICompilerContext
    {
        public event Action<TextPosition, int> Error;
        private Queue<string> _code;
        public List<string> Result { get; }

        public bool IsEnd { get; private set; }

        public TestCompilerContext(Queue<string> code)
        {
            _code = code;
            Result = new List<string>();
        }

        public void OnError(TextPosition position, int code)
        {
            Error?.Invoke(position, code);
        }

        public string ReadLine()
        {
            if (_code.Count == 0)
            {
                IsEnd = true;
                return null;
            }
            return _code.Dequeue();
        }

        public void WriteLine(string line)
        {
            Result.Add(line);
        }
    }
}
