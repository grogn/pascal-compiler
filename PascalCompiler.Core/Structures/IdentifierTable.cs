using PascalCompiler.Core.Constants;
using System.Collections.Generic;
using System.Linq;

namespace PascalCompiler.Core.Structures
{
    public class IdentifierTable
    {
        private HashSet<Identifier> _identifiers;

        public IdentifierTable()
        {
            _identifiers = new HashSet<Identifier>();
        }

        public Identifier Add(Symbol symbol, IdentifierClass classUsed)
        {
            var identifier = new Identifier
            {
                Symbol = symbol,
                Class = classUsed
            };

            return _identifiers.Add(identifier) ? identifier : null;
        }

        public bool Add(Identifier identifier)
        {
            return _identifiers.Add(identifier);
        }

        public Identifier Search(Symbol symbol, List<IdentifierClass> classes)
        {
            return _identifiers.FirstOrDefault(x => x.Symbol == symbol);
        }
    }
}
