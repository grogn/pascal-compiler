using PascalCompiler.Core.Constants;
using PascalCompiler.Core.Structures.Types;
using System.Collections.Generic;
using System.Linq;

namespace PascalCompiler.Core.Structures
{
    public class TypeTable
    {
        private List<Type> _types;

        public TypeTable()
        {
            _types = new List<Type>();
        }

        public void Add(Type type)
        {
            _types.Add(type);
        }

        public Type Add(TypeCode typeCode)
        {
            Type type = null;
            switch(typeCode)
            {
                case TypeCode.Limiteds:
                    type = new Limited();
                    break;
                case TypeCode.Scalars:
                    type = new Scalar();
                    break;
                case TypeCode.Arrays:
                    type = new Array();
                    break;
                case TypeCode.Enums:
                    type = new Enum();
                    break;
            }
            _types.Add(type);

            return type;
        }
    }
}
