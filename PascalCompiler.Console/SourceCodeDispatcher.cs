using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PascalCompiler.Core;

namespace PascalCompiler.Console
{
    class SourceCodeDispatcher : ISourceCodeDispatcher
    {
        public bool IsEnd { get; private set; }

        private StreamReader _reader;
        private StreamWriter _writer;
        
        public SourceCodeDispatcher(string inputeFile, string outputFile)
        {
            _reader = new StreamReader(inputeFile);
            _writer = new StreamWriter(outputFile);
        }

        public string ReadLine()
        {
            var line = _reader.ReadLine();
            if (line == null)
                IsEnd = true;
            return line;
        }

        public void WriteLine(string line)
        {
            _writer.WriteLine(line);
        }

        public void Close()
        {
            _writer.Close();
        }
    }
}
