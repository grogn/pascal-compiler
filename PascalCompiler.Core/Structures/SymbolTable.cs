using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core.Structures
{
    public class SymbolTable
    {
        private Dictionary<string, Symbol> _symbols;

        public SymbolTable()
        {
            _symbols = new Dictionary<string, Symbol>();
        }
        public Symbol this[string name]
        {
            get => _symbols[name];
        }

        public Symbol Add(string name)
        {
            if (_symbols.ContainsKey(name))
                return _symbols[name];
            var symbol = new Symbol(name);
            _symbols[name] = symbol;
            return symbol;
        }

        public bool Add(Symbol symbol)
        {
            if (_symbols.ContainsKey(symbol.Name))
                return false;
            _symbols[symbol.Name] = symbol;
            return true;
        }
    }
}
