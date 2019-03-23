using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core
{
    public class Logger
    {
        private StreamWriter _writer;

        public Logger(string inputFile)
        {
            _writer = new StreamWriter(inputFile);
        }

        public void Log(string line)
        {
            _writer.WriteLine(line);
        }

        public void Close()
        {
            _writer.Close();
        }
    }
}
