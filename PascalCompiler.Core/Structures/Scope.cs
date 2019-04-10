using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core.Structures
{
    public class Scope
    {
        /// <summary>
        /// Таблица идентификаторов области действия
        /// </summary>
        public IdentifierTable IdentifierTable { get; set; }

        /// <summary>
        /// Таблица типов области действия
        /// </summary>
        public TypeTable TypeTable { get; set; }

        /// <summary>
        /// Элемент стека области действия, непосредственно объемлющей данную
        /// </summary>
        public Scope EnclosingScope { get; set; }

        public Scope()
        {
            IdentifierTable = new IdentifierTable();
            TypeTable = new TypeTable();
        }
    }
}
