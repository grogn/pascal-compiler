using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PascalCompiler.Core.Constants;
using PascalCompiler.Core.Structures;

namespace PascalCompiler.Core.Modules
{
    public class SyntacticalAnalyzerModule
    {
        private Context _context;
        private LexicalAnalyzerModule _lexicalAnalyzerModule;

        private Structures.Type _booleanType;
        private Structures.Type _integerType;
        private Structures.Type _realType;
        private Structures.Type _charType;
        private Structures.Type _nilType;

        private Structures.ConstValue _const;

        private bool IsNeedConvert;
        private bool WasFirstOperand;

        public SyntacticalAnalyzerModule(Context context, LexicalAnalyzerModule lexicalAnalyzerModule)
        {
            _context = context;
            _lexicalAnalyzerModule = lexicalAnalyzerModule;
            _const = new ConstValue();
        }

        private void ListError(int errorCode)
        {
            GeneratorModule.NoCode = true;
            _context.OnError(new Error(_context.SymbolPosition, errorCode));
        }

        private void ListError(int position, int errorCode)
        {
            GeneratorModule.NoCode = true;
            _context.OnError(new Error(position, errorCode));
        }

        private void Accept(int symbolCode)
        {
            if (_context.SymbolCode == symbolCode)
                _lexicalAnalyzerModule.NextSymbol();
            else
            {
                GeneratorModule.NoCode = true;
                _context.OnError(new Error(_context.SymbolPosition, symbolCode));
            }
        }

        private bool SymbolBelong(IEnumerable<int> starters)
        {
            return starters.Contains(_context.SymbolCode);
        }

        private IEnumerable<int> Union(IEnumerable<int> firstSet, IEnumerable<int> secondSet)
        {
            return firstSet.Union(secondSet);
        }

        private void SkipTo(IEnumerable<int> starters)
        {
            while (!starters.Contains(_context.SymbolCode))
                _lexicalAnalyzerModule.NextSymbol();
        }

        private void SkipTo2(IEnumerable<int> starters, IEnumerable<int> followers)
        {
            while (!_context.IsEnd && !starters.Contains(_context.SymbolCode) && !followers.Contains(_context.SymbolCode))
                _lexicalAnalyzerModule.NextSymbol();
        }

        /// <summary>
        /// Создание элемента стека для текущей области действия
        /// </summary>
        private void OpenScope()
        {
            var scope = new Scope
            {
                EnclosingScope = _context.LocalScope
            };
            _context.LocalScope = scope;
        }

        /// <summary>
        /// Удаление таблиц текущей области действия
        /// </summary>
        private void CloseScope()
        {
            _context.LocalScope = _context.LocalScope.EnclosingScope;
        }

        private void InitFictiousScope()
        {
            var boolType = new Structures.Types.Enum();
            _context.LocalScope.TypeTable.Add(boolType);
            var booleanSymbol = new Symbol("boolean");
            _context.SymbolTable.Add(booleanSymbol);
            var booleanIdentifier = new Identifier
            {
                Symbol = booleanSymbol,
                Class = IdentifierClass.Types,
                Type = boolType
            };
            _context.LocalScope.IdentifierTable.Add(booleanIdentifier);
            _booleanType = boolType;

            var falseSymbol = new Symbol("false");
            _context.SymbolTable.Add(falseSymbol);
            boolType.Symbols.Add(falseSymbol);
            var falseIdentifier = new Identifier
            {
                Type = boolType,
                Class = IdentifierClass.Consts,
                Symbol = falseSymbol
            };
            _context.LocalScope.IdentifierTable.Add(falseIdentifier);

            var trueSymbol = new Symbol("true");
            _context.SymbolTable.Add(trueSymbol);
            boolType.Symbols.Add(trueSymbol);
            var trueIdentifier = new Identifier
            {
                Type = boolType,
                Class = IdentifierClass.Consts,
                Symbol = trueSymbol
            };
            _context.LocalScope.IdentifierTable.Add(trueIdentifier);

            var intType = new Structures.Types.Scalar();
            _context.LocalScope.TypeTable.Add(intType);
            var integerSymbol = new Symbol("integer");
            _context.SymbolTable.Add(integerSymbol);
            var integerIdentifier = new Identifier
            {
                Symbol = integerSymbol,
                Class = IdentifierClass.Types,
                Type = intType
            };
            _context.LocalScope.IdentifierTable.Add(integerIdentifier);
            _integerType = intType;

            var realType = new Structures.Types.Scalar();
            _context.LocalScope.TypeTable.Add(realType);
            var realSymbol = new Symbol("real");
            _context.SymbolTable.Add(realSymbol);
            var realIdentifier = new Identifier
            {
                Symbol = realSymbol,
                Class = IdentifierClass.Types,
                Type = realType
            };
            _context.LocalScope.IdentifierTable.Add(realIdentifier);
            _realType = realType;

            var charType = new Structures.Types.Scalar();
            _context.LocalScope.TypeTable.Add(charType);
            var charSymbol = new Symbol("char");
            _context.SymbolTable.Add(charSymbol);
            var charIdentifier = new Identifier
            {
                Symbol = charSymbol,
                Class = IdentifierClass.Types,
                Type = charType
            };
            _context.LocalScope.IdentifierTable.Add(charIdentifier);
            _charType = charType;
        }

        public void Program()
        {
            InitFictiousScope();
            OpenScope();
            _lexicalAnalyzerModule.NextSymbol();
            Accept(Keywords.Programsy);
            Accept(Symbols.Ident);
            Accept(Symbols.Semicolon);
            Block(Followers.Block);
            Accept(Symbols.Point);
        }

        private void Block(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.Block))
            {
                ListError(18);
                SkipTo2(Starters.Block, followers);
            }
            if (SymbolBelong(Starters.Block))
            {
                var symbols = Union(Followers.ConstPart, followers);
                ConstPart(symbols);
                symbols = Union(Followers.TypePart, followers);
                TypePart(symbols);
                symbols = Union(Followers.VarPart, followers);
                VarPart(symbols);
                StatementsPart(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void ConstPart(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.ConstPart))
            {
                ListError(18);
                SkipTo2(Starters.ConstPart, followers);
            }
            if (_context.SymbolCode == Keywords.Constsy)
            {
                Accept(Keywords.Constsy);
                var symbols = Union(Followers.ConstDeclaration, followers);
                do
                {
                    ConstDeclaration(symbols);
                    Accept(Symbols.Semicolon);
                } while (_context.SymbolCode == Symbols.Ident);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void ConstDeclaration(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.ConstDeclaration))
            {
                ListError(18);
                SkipTo2(Starters.ConstDeclaration, followers);
            }
            if (SymbolBelong(Starters.ConstDeclaration))
            {
                var identifier = new Identifier
                {
                    Symbol = _context.Symbol,
                    Class = IdentifierClass.Consts
                };
                Accept(Symbols.Ident);
                Accept(Symbols.Equal);
                var type = Const(followers);
                identifier.Type = type;
                if (!_context.LocalScope.IdentifierTable.ExperimentalAdd(identifier))
                    ListError(identifier.Symbol.Position, 101);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        // TODO: вещественное с E
        private Structures.Type Const(IEnumerable<int> followers)
        {
            Structures.Type type = null;
            if (!SymbolBelong(Starters.Const))
            {
                ListError(18);
                SkipTo2(Starters.Const, followers);
            }
            if (SymbolBelong(Starters.Const))
            {
                switch (_context.SymbolCode)
                {
                    case Symbols.Intc:
                        type = _integerType;
                        _const = new ConstValue
                        {
                            Integer = int.Parse(_context.SymbolName)
                        };
                        Accept(Symbols.Intc);
                        break;
                    case Symbols.Floatc:
                        type = _realType;
                        Accept(Symbols.Floatc);
                        break;
                    case Symbols.Stringc:
                        Accept(Symbols.Stringc);
                        break;
                    case Symbols.Charc:
                        type = _charType;
                        _const = new ConstValue
                        {
                            Symbol = _context.SymbolName[0]
                        };
                        Accept(Symbols.Charc);
                        break;
                    case Symbols.Ident:
                        var identifier = SearchIdentifier(_context.LocalScope, _context.SymbolName);
                        if (identifier == null)
                            ListError(104);
                        else
                        {
                            type = identifier.Type;
                            _const = new ConstValue
                            {
                                Enum = identifier.Symbol
                            };
                        }
                        Accept(Symbols.Ident);
                        break;
                    default:
                        if (_context.SymbolCode == Symbols.Minus ||
                            _context.SymbolCode == Symbols.Plus)
                        {
                            _lexicalAnalyzerModule.NextSymbol();
                            if (_context.SymbolCode == Symbols.Intc)
                                type = _integerType;
                            if (_context.SymbolCode == Symbols.Floatc)
                                type = _realType;
                            if (_context.SymbolCode == Symbols.Ident)
                            {
                                identifier = SearchIdentifier(_context.LocalScope, _context.SymbolName);
                                if (identifier == null)
                                    ListError(104);
                                else
                                    type = identifier.Type;
                            }
                            _lexicalAnalyzerModule.NextSymbol();
                        }
                        break;
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private void TypePart(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.TypePart))
            {
                ListError(18);
                SkipTo2(Starters.TypePart, followers);
            }
            if (_context.SymbolCode == Keywords.Typesy)
            {
                Accept(Keywords.Typesy);
                var symbols = Union(Followers.TypeDeclaration, followers);
                do
                {
                    TypeDeclaration(symbols);
                    Accept(Symbols.Semicolon);
                } while (_context.SymbolCode == Symbols.Ident);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void TypeDeclaration(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.TypeDeclaration))
            {
                ListError(18);
                SkipTo2(Starters.TypeDeclaration, followers);
            }
            if (SymbolBelong(Starters.TypeDeclaration))
            {
                var identifier = new Identifier
                {
                    Class = IdentifierClass.Types,
                    Symbol = _context.Symbol
                };
                Accept(Symbols.Ident);
                Accept(Symbols.Equal);
                var type = Type(followers);
                identifier.Type = type;
                _context.LocalScope.IdentifierTable.Add(identifier);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private Structures.Type Type(IEnumerable<int> followers)
        {
            Structures.Type type = null;
            if (!SymbolBelong(Starters.Type))
            {
                ListError(10);
                SkipTo2(Starters.Type, followers);
            }
            if (SymbolBelong(Starters.Type))
            {
                if (_context.SymbolCode == Symbols.Intc ||
                    _context.SymbolCode == Symbols.Floatc ||
                    _context.SymbolCode == Symbols.Charc ||
                    _context.SymbolCode == Symbols.Stringc ||
                    _context.SymbolCode == Symbols.Plus ||
                    _context.SymbolCode == Symbols.Minus ||
                    _context.SymbolCode == Symbols.Ident ||
                    _context.SymbolCode == Symbols.Leftpar)
                    type = SimpleType(followers);
                else if (_context.SymbolCode == Keywords.Arraysy)
                    type = CompositeType(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }

            return type;
        }

        private Structures.Type SearchType(Scope scope, string name)
        {
            var identifier = scope.IdentifierTable.Search(name);
            if (identifier != null && (identifier.Class == IdentifierClass.Types ||
                identifier.Class == IdentifierClass.Vars))
                return identifier.Type;
            if (scope.EnclosingScope != null)
                return SearchType(scope.EnclosingScope, name);
            return null;
        }
        
        private Structures.Type SimpleType(IEnumerable<int> followers)
        {
            Structures.Type type = null;
            if (!SymbolBelong(Starters.SimpleType))
            {
                ListError(10);
                SkipTo2(Starters.SimpleType, followers);
            }
            if (SymbolBelong(Starters.SimpleType))
            {
                if (_context.SymbolCode == Symbols.Leftpar)
                    type = EnumerationType(followers);
                else if (_context.SymbolCode == Symbols.Ident)
                {
                    type = SearchType(_context.LocalScope, _context.SymbolName);
                    if (type == null)
                    {
                        var identifier = SearchIdentifier(_context.LocalScope, _context.SymbolName);
                        if (identifier == null)
                        {
                            ListError(104);
                            Accept(Symbols.Ident);
                        }
                        else
                            type = LimitedType(followers);
                    }
                    else
                        Accept(Symbols.Ident);
                }
                else if (_context.SymbolCode == Symbols.Intc ||
                        _context.SymbolCode == Symbols.Floatc ||
                        _context.SymbolCode == Symbols.Charc ||
                        _context.SymbolCode == Symbols.Stringc ||
                        _context.SymbolCode == Symbols.Plus ||
                        _context.SymbolCode == Symbols.Minus ||
                        _context.SymbolCode == Symbols.Ident)
                    type = LimitedType(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private Structures.Type EnumerationType(IEnumerable<int> followers)
        {
            var type = new Structures.Types.Enum();
            _context.LocalScope.TypeTable.Add(type);
            if (!SymbolBelong(Starters.EnumerationType))
            {
                ListError(10);
                SkipTo2(Starters.EnumerationType, followers);
            }
            if (SymbolBelong(Starters.EnumerationType))
            {
                do
                {
                    _lexicalAnalyzerModule.NextSymbol();
                    if (_context.SymbolCode == Symbols.Ident)
                    {
                        var idenifier = new Identifier
                        {
                            Symbol = _context.Symbol,
                            Class = IdentifierClass.Consts,
                            Type = type
                        };
                        idenifier.ConstValue.Enum = _context.Symbol;
                        _context.LocalScope.IdentifierTable.Add(idenifier);
                        type.Symbols.Add(_context.Symbol);
                        Accept(Symbols.Ident);
                    }
                } while (_context.SymbolCode == Symbols.Comma);
                Accept(Symbols.Rightpar);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private Structures.Type LimitedType(IEnumerable<int> followers)
        {
            var type = new Structures.Types.Limited();
            _context.LocalScope.TypeTable.Add(type);
            if (!SymbolBelong(Starters.LimitedType))
            {
                ListError(10);
                SkipTo2(Starters.LimitedType, followers);
            }
            if (SymbolBelong(Starters.LimitedType))
            {
                var symbols = Union(Followers.LimitedTypeFirstConst, followers);
                var firstType = Const(symbols);
                var firstConst = _const;
                Accept(Symbols.Twopoints);
                var secondType = Const(followers);
                var secondConst = _const;
                if (firstType != secondType)
                    ListError(112);
                if (firstType == _integerType)
                {
                    if (firstConst.Integer.Value >= secondConst.Integer.Value)
                        ListError(112);
                    else
                    {
                        type.Min = firstConst;
                        type.Max = secondConst;
                        type.BaseType = _integerType;
                    }
                }
                if (firstType == _charType)
                {
                    if (firstConst.Symbol.Value >= secondConst.Symbol.Value)
                        ListError(112);
                    else
                    {
                        type.Min = firstConst;
                        type.Max = secondConst;
                        type.BaseType = _charType;
                    }
                }
                if (firstType.Code == Constants.TypeCode.Enums)
                {
                    var enumType = firstType as Structures.Types.Enum;
                    if (enumType.Symbols.IndexOf(firstConst.Enum) >= enumType.Symbols.IndexOf(secondConst.Enum))
                        ListError(112);
                    else
                    {
                        type.Min = firstConst;
                        type.Max = secondConst;
                        type.BaseType = firstType;
                    }
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private Structures.Type CompositeType(IEnumerable<int> followers)
        {
            Structures.Type type = null;
            if (!SymbolBelong(Starters.CompositeType))
            {
                ListError(10);
                SkipTo2(Starters.CompositeType, followers);
            }
            if (SymbolBelong(Starters.CompositeType))
            {
                type = ArrayType(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private Structures.Type ArrayType(IEnumerable<int> followers)
        {
            var type = new Structures.Types.Array();
            if (!SymbolBelong(Starters.ArrayType))
            {
                ListError(10);
                SkipTo2(Starters.ArrayType, followers);
            }
            if (SymbolBelong(Starters.ArrayType))
            {
                Accept(Keywords.Arraysy);
                Accept(Symbols.Lbracket);
                var symbols = Union(Followers.SimpleType, followers);
                var indexType = SimpleType(symbols);
                type.Indexes.Add(indexType);
                while (_context.SymbolCode == Symbols.Comma)
                {
                    Accept(Symbols.Comma);
                    indexType = SimpleType(symbols);
                    type.Indexes.Add(indexType);
                }
                Accept(Symbols.Rbracket);
                Accept(Keywords.Ofsy);
                var baseType = Type(followers);
                type.BaseType = baseType;
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private void VarPart(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.VarPart))
            {
                ListError(18);
                SkipTo2(Starters.VarPart, followers);
            }
            if (SymbolBelong(Starters.VarPart))
            {
                Accept(Keywords.Varsy);
                var symbols = Union(Followers.VarDeclaration, followers);
                do
                {
                    VarDeclaration(symbols);
                    Accept(Symbols.Semicolon);
                } while (_context.SymbolCode == Symbols.Ident);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void AddVariableTo(List<Identifier> variables)
        {
            if (_context.SymbolCode == Symbols.Ident)
            {
                var variableIdentifier = new Identifier
                {
                    Symbol = _context.Symbol,
                    Class = IdentifierClass.Vars
                };
                variables.Add(variableIdentifier);
            }
        }

        private void VarDeclaration(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.VarDeclaration))
            {
                ListError(2);
                SkipTo2(Starters.VarDeclaration, followers);
            }
            if (SymbolBelong(Starters.VarDeclaration))
            {
                var variableList = new List<Identifier>();
                AddVariableTo(variableList);
                Accept(Symbols.Ident);
                while (_context.SymbolCode == Symbols.Comma)
                {
                    Accept(Symbols.Comma);
                    AddVariableTo(variableList);
                    Accept(Symbols.Ident);
                }
                Accept(Symbols.Colon);
                var type = Type(followers);
                variableList.ForEach(x =>
                {
                    x.Type = type;
                    if (type == _integerType)
                        GeneratorModule.AddLocalInt(x.Symbol.Name);
                    if (type == _realType)
                        GeneratorModule.AddLocalReal(x.Symbol.Name);
                    if (type == _charType)
                        GeneratorModule.AddLocalChar(x.Symbol.Name);
                    if (type == _booleanType)
                        GeneratorModule.AddLocalBoolean(x.Symbol.Name);
                    if (!_context.LocalScope.IdentifierTable.ExperimentalAdd(x))
                        ListError(x.Symbol.Position, 101);
                });
                
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void StatementsPart(IEnumerable<int> followers)
        {
            CompoundStatement(followers);
        }

        private void CompoundStatement(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.CompoundStatement))
            {
                ListError(22);
                SkipTo2(Starters.CompoundStatement, followers);
            }
            if (SymbolBelong(Starters.CompoundStatement))
            {
                Accept(Keywords.Beginsy);
                var symbols = Union(Followers.Statement, followers);
                while (SymbolBelong(Starters.Statement))
                {
                    Statement(symbols);
                    Accept(Symbols.Semicolon);
                }
                Accept(Keywords.Endsy);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        // TODO symbols
        private void WriteLine(IEnumerable<int> followers)
        {
            Accept(Symbols.Ident);
            Accept(Symbols.Leftpar);
            var symbols = Union(new[] { Symbols.Rightpar }, followers);
            var type = Expression(symbols);
            Accept(Symbols.Rightpar);
            if (type == _integerType)
                GeneratorModule.WriteLineInteger();
            if (type == _realType)
                GeneratorModule.WriteLineReal();
            if (type == _booleanType)
                GeneratorModule.WriteLineBool();
        }

        private void Statement(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.Statement))
            {
                ListError(22);
                SkipTo2(Starters.Statement, followers);
            }
            if (SymbolBelong(Starters.Statement))
            {
                switch(_context.SymbolCode)
                {
                    case Symbols.Ident:
                        if (_context.SymbolName == "writeln")
                            WriteLine(followers);
                        else
                            AssignmentStatement(followers);
                        break;
                    case Keywords.Beginsy:
                        CompoundStatement(followers);
                        break;
                    case Keywords.Ifsy:
                        IfStatement(followers);
                        break;
                    case Keywords.Whilesy:
                        WhileStatement(followers);
                        break;
                    case Symbols.Semicolon:
                    case Keywords.Elsesy:
                    case Keywords.Endsy:
                        break;
                }
            }
        }

        private bool CheckAssignmentTypes(Structures.Type variableType, Structures.Type expressionType)
        {
            if (variableType == null || expressionType == null)
                return false;
            if (variableType == _realType &&
                (expressionType == _integerType || expressionType == _realType))
                return true;
            if (variableType.Code == Constants.TypeCode.Limiteds &&
                (variableType as Structures.Types.Limited).BaseType == expressionType)
                return true;
            if (variableType.Code == Constants.TypeCode.Arrays &&
                (variableType as Structures.Types.Array).BaseType == expressionType)
                return true;
            if (variableType == expressionType)
                return true;
            return false;
        }

        private void AssignmentStatement(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.AssignmentStatement))
            {
                ListError(22);
                SkipTo2(Starters.AssignmentStatement, followers);
            }
            if (SymbolBelong(Starters.AssignmentStatement))
            {
                var symbols = Union(Followers.AssignmentStatementVariable, followers);
                var variable = _context.SymbolName;
                var variableType = Variable(symbols);
                var position = _context.SymbolPosition;
                Accept(Symbols.Assign);
                var symbol = _context.Symbol;
                var expressionType = Expression(followers);
                if (!CheckAssignmentTypes(variableType, expressionType))
                    ListError(position, 328);
                else if (variableType.Code == Constants.TypeCode.Limiteds)
                {
                    var limited = variableType as Structures.Types.Limited;
                    int min =0, max = 0, current = 0;
                    if (expressionType.Code == Constants.TypeCode.Enums)
                    {
                        var enumType = expressionType as Structures.Types.Enum;
                        min = enumType.Symbols.Select(x => x.Name).ToList().IndexOf(limited.Min.Enum.Name);
                        max = enumType.Symbols.Select(x => x.Name).ToList().IndexOf(limited.Max.Enum.Name);
                        current = enumType.Symbols.Select(x => x.Name).ToList().IndexOf(symbol.Name);
                    }
                    if (min > current || current > max)
                        ListError(306);
                }
                if (variableType == _realType && expressionType == _integerType)
                    GeneratorModule.ConvertToReal();
                GeneratorModule.Assignment(variable);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private Structures.Type Variable(IEnumerable<int> followers)
        {
            Structures.Type type = null;
            if (!SymbolBelong(Starters.Variable))
            {
                ListError(22);
                SkipTo2(Starters.Variable, followers);
            }
            if (SymbolBelong(Starters.Variable))
            {
                var identifier = _context.LocalScope.IdentifierTable.Search(_context.SymbolName);
                if (identifier == null)
                    ListError(104);
                else
                    type = identifier.Type;
                Accept(Symbols.Ident);
                while (_context.SymbolCode == Symbols.Lbracket)
                {
                    var arrayType = type as Structures.Types.Array;
                    var arrayIndex = 0;
                    Accept(Symbols.Lbracket);
                    var symbols = Union(Followers.VariableExpression, followers);
                    var expressionType = Expression(symbols);
                    if (expressionType != (arrayType.Indexes[arrayIndex++] as Structures.Types.Limited).BaseType)
                        ListError(328);
                    while (_context.SymbolCode == Symbols.Comma)
                    {
                        Accept(Symbols.Comma);
                        expressionType = Expression(symbols);
                        if (expressionType != (arrayType.Indexes[arrayIndex++] as Structures.Types.Limited).BaseType)
                            ListError(328);
                    }
                    Accept(Symbols.Rbracket);
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private Structures.Type CheckRelationOperation(Structures.Type firstType, Structures.Type secondType, int operation)
        {
            if ((firstType == _integerType || firstType == _realType) &&
                (secondType == _integerType || secondType == _realType) ||
                (firstType == _charType && secondType == _charType) || 
                (firstType == _booleanType && secondType == _booleanType))
                return _booleanType;
            return null;
        }

        private Structures.Type Expression(IEnumerable<int> followers)
        {
            Structures.Type type = null;
            if (!SymbolBelong(Starters.Expression))
            {
                ListError(23);
                SkipTo2(Starters.Expression, followers);
            }
            if (SymbolBelong(Starters.Expression))
            {
                var symbols = Union(Followers.ExpressionSimpleExpression, followers);
                type = SimpleExpression(symbols);
                if (_context.SymbolCode == Symbols.Equal ||
                    _context.SymbolCode == Symbols.Latergreater ||
                    _context.SymbolCode == Symbols.Later ||
                    _context.SymbolCode == Symbols.Laterequal ||
                    _context.SymbolCode == Symbols.Greaterequal ||
                    _context.SymbolCode == Symbols.Greater)
                {
                    var operation = _context.SymbolCode;
                    var operationPosition = _context.SymbolPosition;
                    _lexicalAnalyzerModule.NextSymbol();
                    var secondType = SimpleExpression(followers);
                    type = CheckRelationOperation(type, secondType, operation);
                    if (type == null)
                        ListError(operationPosition, 186);
                    else
                    {
                        GeneratorModule.PushOperation(operation);
                    }
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private void CheckRightSign(Structures.Type type)
        {
            if (type == null || type == _integerType ||
                type == _realType || (type.Code == Constants.TypeCode.Limiteds && (type as Structures.Types.Limited).BaseType == _integerType))
                return;
            ListError(211);
        }

        private Structures.Type CheckAdd(Structures.Type firstType, Structures.Type secondType, int operation, int errorPosition)
        {
            if (operation == Symbols.Plus ||
                operation == Symbols.Minus)
            {
                if ((firstType == _integerType || firstType == _realType) &&
                    (secondType == _integerType || secondType == _realType))
                    return firstType == _realType ? _realType : secondType == _realType ? _realType : _integerType;
                ListError(errorPosition, 211);
            }
            if (operation == Keywords.Orsy)
            {
                if (firstType == _booleanType && secondType == _booleanType)
                    return _booleanType;
                ListError(errorPosition, 210);
            }
            return null;
        }

        private Structures.Type SimpleExpression(IEnumerable<int> followers)
        {
            Structures.Type type = null;
            if (!SymbolBelong(Starters.SimpleExpression))
            {
                ListError(22);
                SkipTo2(Starters.SimpleExpression, followers);
            }
            if (SymbolBelong(Starters.SimpleExpression))
            {
                IsNeedConvert = false;
                WasFirstOperand = false;
                var symbols = Union(Followers.SimpleExpressionSign, followers);
                var firstTermHasSign = false;
                if (_context.SymbolCode == Symbols.Plus ||
                    _context.SymbolCode == Symbols.Minus)
                {
                    _lexicalAnalyzerModule.NextSymbol();
                    firstTermHasSign = true;
                }
                symbols = Union(Followers.SimpleExpressionAddend, followers);
                type = Term(symbols);
                if (firstTermHasSign)
                    CheckRightSign(type);
                while (SymbolBelong(Starters.AdditiveOperation))
                {
                    var operation = _context.SymbolCode;
                    var operationPosition = _context.SymbolPosition;
                    _lexicalAnalyzerModule.NextSymbol();
                    var secondType = Term(symbols);
                    type = CheckAdd(type, secondType, operation, operationPosition);
                    GeneratorModule.PushOperation(operation);
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private Structures.Type CheckMultiplicativeOperation(Structures.Type firstType, Structures.Type secondType, int operation, int errorPosition)
        {
            if (operation == Keywords.Divsy ||
                operation == Keywords.Modsy)
            {
                if (firstType == _integerType && secondType == _integerType)
                    return _integerType;
                ListError(errorPosition, 212);
            }
            if (operation == Symbols.Slash ||
                operation == Symbols.Star)
            {
                if ((firstType == _integerType || firstType == _realType) &&
                    (secondType == _integerType || secondType == _realType))
                    return firstType == _realType ? _realType : secondType == _realType ? _realType : _integerType;
                if (operation == Symbols.Slash)
                    ListError(errorPosition, 214);
                if (operation == Symbols.Star)
                    ListError(errorPosition, 213);
            }
            if (operation == Keywords.Andsy)
            {
                if (firstType == _booleanType && secondType == _booleanType)
                    return _booleanType;
                ListError(errorPosition, 210);
            }
            return null;
        }

        private Structures.Type Term(IEnumerable<int> followers)
        {
            Structures.Type type = null;
            if (!SymbolBelong(Starters.Term))
            {
                ListError(22);
                SkipTo2(Starters.Term, followers);
            }
            if (SymbolBelong(Starters.Term))
            {
                var symbols = Union(Followers.AddendMultiplier, followers);
                type = Factor(symbols);
                WasFirstOperand = true;
                while (SymbolBelong(Starters.MultiplicativeOperation))
                {
                    var operation = _context.SymbolCode;
                    var operationPosition = _context.SymbolPosition;
                    _lexicalAnalyzerModule.NextSymbol();
                    var secondType = Factor(symbols);
                    type = CheckMultiplicativeOperation(type, secondType, operation, operationPosition);
                    GeneratorModule.PushOperation(operation);
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private Structures.Type CheckLogical(Structures.Type type)
        {
            if (type == _booleanType)
                return _booleanType;
            return null;
        }

        private Identifier SearchIdentifier(Scope scope, string name)
        {
            var identifier = scope.IdentifierTable.Search(_context.SymbolName);
            if (identifier != null)
                return identifier;
            if (scope.EnclosingScope != null)
                return SearchIdentifier(scope.EnclosingScope, name);
            return null;
        }
        
        private Structures.Type Factor(IEnumerable<int> followers)
        {
            Structures.Type type = null;
            if (!SymbolBelong(Starters.Factor))
            {
                ListError(22);
                SkipTo2(Starters.Factor, followers);
            }
            if (SymbolBelong(Starters.Factor))
            {
                switch (_context.SymbolCode)
                {
                    case Symbols.Leftpar:
                        Accept(Symbols.Leftpar);
                        var symbols = Union(Followers.FactorExpression, followers);
                        type = Expression(symbols);
                        Accept(Symbols.Rightpar);
                        break;
                    case Keywords.Notsy:
                        Accept(Keywords.Notsy);
                        var position = _context.SymbolPosition;
                        type = Factor(followers);
                        type = CheckLogical(type);
                        if (type == null)
                            ListError(position, 210);
                        else
                            GeneratorModule.PushOperation(Keywords.Notsy);
                        break;
                    case Symbols.Intc:
                        type = _integerType;
                        GeneratorModule.PushConst(int.Parse(_context.SymbolName));
                        if (IsNeedConvert)
                            GeneratorModule.ConvertToReal();
                        Accept(Symbols.Intc);
                        break;
                    case Symbols.Floatc:
                        type = _realType;
                        if (!IsNeedConvert && WasFirstOperand)
                            GeneratorModule.ConvertToReal();
                        IsNeedConvert = true;
                        GeneratorModule.PushConst(double.Parse(_context.SymbolName, System.Globalization.CultureInfo.InvariantCulture));
                        Accept(Symbols.Floatc);
                        break;
                    case Symbols.Charc:
                        type = _charType;
                        GeneratorModule.PushConst(_context.SymbolName[0]);
                        Accept(Symbols.Charc);
                        break;
                    case Keywords.Nilsy:
                        type = _nilType;
                        Accept(Keywords.Nilsy);
                        break;
                    case Symbols.Ident:
                        var identifier = SearchIdentifier(_context.LocalScope, _context.SymbolName);
                        if (identifier != null)
                        {
                            switch(identifier.Class)
                            {
                                case IdentifierClass.Vars:
                                    if (identifier.Type == _realType)
                                    {
                                        if (!IsNeedConvert && WasFirstOperand)
                                            GeneratorModule.ConvertToReal();
                                        IsNeedConvert = true;
                                    }
                                    if (identifier.Type == _integerType && IsNeedConvert)
                                        GeneratorModule.ConvertToReal();
                                    GeneratorModule.PushVariable(_context.SymbolName); // not so good
                                    type = Variable(followers);
                                    break;
                                case IdentifierClass.Consts:
                                    type = identifier.Type;
                                    if (type == _booleanType)
                                        GeneratorModule.PushConst(identifier.Symbol.Name == "true" ? 1 : 0);
                                    Accept(Symbols.Ident);
                                    break;
                            }
                        }
                        else
                        {
                            identifier = new Identifier
                            {
                                Symbol = _context.Symbol,
                                Class = IdentifierClass.Vars,
                            };
                            _context.LocalScope.IdentifierTable.ExperimentalAdd(identifier);
                            ListError(104);
                            Accept(Symbols.Ident);
                        }
                        break;
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return type;
        }

        private void IfStatement(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.ConditionalStatement))
            {
                ListError(22);
                SkipTo2(Starters.ConditionalStatement, followers);
            }
            if (SymbolBelong(Starters.ConditionalStatement))
            {
                Accept(Keywords.Ifsy);
                var symbols = Union(Followers.ConditionalStatementExpression, followers);
                var position = _context.SymbolPosition;
                var type = Expression(symbols);
                type = CheckLogical(type);
                if (type == null)
                    ListError(position, 328);
                Accept(Keywords.Thensy);
                symbols = Union(Followers.ConditionalStatementStatement, followers);
                Statement(symbols);
                if (_context.SymbolCode == Keywords.Elsesy)
                {
                    Accept(Keywords.Elsesy);
                    Statement(symbols);
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void WhileStatement(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.WhileStatement))
            {
                ListError(22);
                SkipTo2(Starters.WhileStatement, followers);
            }
            if (SymbolBelong(Starters.WhileStatement))
            {
                Accept(Keywords.Whilesy);
                var symbols = Union(Followers.WhileStatementExpression, followers);
                var position = _context.SymbolPosition;
                var type = Expression(symbols);
                type = CheckLogical(type);
                if (type == null)
                    ListError(position, 328);
                Accept(Keywords.Dosy);
                Statement(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }
    }
}
