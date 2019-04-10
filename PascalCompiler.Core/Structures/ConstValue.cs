using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core.Structures
{
    public class ConstValue
    {
        public int? Integer { get; set; }
        public double? Float { get; set; }
        public char? Symbol { get; set; }
        public Symbol Enum { get; set; }
    }
}
