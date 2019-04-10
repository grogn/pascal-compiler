using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core.Structures
{
    public class Symbol
    {
        public string Name { get; }

        public Symbol(string value)
        {
            Name = value;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
