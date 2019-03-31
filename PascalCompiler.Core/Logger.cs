using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core
{
    public static class Logger
    {
        private static StreamWriter _symbolsWriter;

        static Logger()
        {
            _symbolsWriter = new StreamWriter("../../Test/logs.txt");
        }

        public static void LogSymbol(string symbol)
        {
            _symbolsWriter.Write(symbol);
            _symbolsWriter.Write("|");
        }

        public static void Close()
        {
            _symbolsWriter.Close();
        }
    }
}
