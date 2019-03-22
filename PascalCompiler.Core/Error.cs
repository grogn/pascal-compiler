using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core
{
    public class Error
    {
        public int Number { get; }
        public TextPosition Position { get; }
        public int Code { get; }

        public Error(int number, TextPosition position, int code)
        {
            Number = number;
            Position = position;
            Code = code;
        }
    }
}
