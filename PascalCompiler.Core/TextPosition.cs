using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core
{
    public struct TextPosition
    {
        public int LineNumber { get; set; }
        public int CharNumber { get; set; }
    }
}
