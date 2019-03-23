using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PascalCompiler.Core;

namespace PascalCompiler.Test
{
    public class TestSourceCodeDispatcher : ISourceCodeDispatcher
    {
        private Queue<string> _code;
        public List<string> Result { get; }

        public bool IsEnd { get; private set; }

        public TestSourceCodeDispatcher(Queue<string> code)
        {
            _code = code;
            Result = new List<string>();
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

        public void Close()
        {

        }
    }
}
