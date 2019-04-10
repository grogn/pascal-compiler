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

        public SyntacticalAnalyzerModule(Context context, LexicalAnalyzerModule lexicalAnalyzerModule)
        {
            _context = context;
            _lexicalAnalyzerModule = lexicalAnalyzerModule;
        }

        private void ListError(int errorCode)
        {
            _context.OnError(new Error(_context.SymbolPosition, errorCode));
        }

        private void Accept(int symbolCode)
        {
            if (_context.SymbolCode == symbolCode)
                _lexicalAnalyzerModule.NextSymbol();
            else
                _context.OnError(new Error(_context.SymbolPosition, symbolCode));
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
                Accept(Symbols.Ident);
                Accept(Symbols.Equal);
                Const(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        // TODO: вещественное с E
        private void Const(IEnumerable<int> followers)
        {
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
                        Accept(Symbols.Intc);
                        break;
                    case Symbols.Floatc:
                        Accept(Symbols.Floatc);
                        break;
                    case Symbols.Stringc:
                        Accept(Symbols.Stringc);
                        break;
                    case Symbols.Charc:
                        Accept(Symbols.Charc);
                        break;
                    case Symbols.Ident:
                        Accept(Symbols.Ident);
                        break;
                    default:
                        if (_context.SymbolCode == Symbols.Minus ||
                            _context.SymbolCode == Symbols.Plus)
                        {
                            _lexicalAnalyzerModule.NextSymbol();
                            if (_context.SymbolCode == Symbols.Intc ||
                                _context.SymbolCode == Symbols.Floatc ||
                                _context.SymbolCode == Symbols.Ident)
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
                Accept(Symbols.Ident);
                Accept(Symbols.Equal);
                Type(followers);
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
                    Accept(Symbols.Ident);
                    type = new Structures.Types.Scalar();
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
            if (!SymbolBelong(Starters.LimitedType))
            {
                ListError(10);
                SkipTo2(Starters.LimitedType, followers);
            }
            if (SymbolBelong(Starters.LimitedType))
            {
                var symbols = Union(Followers.LimitedTypeFirstConst, followers);
                Const(symbols);
                Accept(Symbols.Twopoints);
                Const(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return new Structures.Types.Limited();
        }

        private Structures.Type CompositeType(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.CompositeType))
            {
                ListError(10);
                SkipTo2(Starters.CompositeType, followers);
            }
            if (SymbolBelong(Starters.CompositeType))
            {
                ArrayType(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
            return new Structures.Types.Array();
        }

        private void ArrayType(IEnumerable<int> followers)
        {
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
                SimpleType(symbols);
                while (_context.SymbolCode == Symbols.Comma)
                {
                    Accept(Symbols.Comma);
                    SimpleType(symbols);
                }
                Accept(Symbols.Rbracket);
                Accept(Keywords.Ofsy);
                Type(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
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
                var variableIdentifier = new Identifier
                {
                    Symbol = _context.Symbol,
                    Class = IdentifierClass.Vars
                };
                variableList.Add(variableIdentifier);
                Accept(Symbols.Ident);
                while (_context.SymbolCode == Symbols.Comma)
                {
                    Accept(Symbols.Comma);
                    variableIdentifier = new Identifier
                    {
                        Symbol = _context.Symbol,
                        Class = IdentifierClass.Vars
                    };
                    variableList.Add(variableIdentifier);
                    Accept(Symbols.Ident);
                }
                Accept(Symbols.Colon);
                var type = Type(followers);
                variableList.ForEach(x => x.Type = type);
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
                Statement(symbols);
                while (_context.SymbolCode == Symbols.Semicolon)
                {
                    Accept(Symbols.Semicolon);
                    Statement(symbols);
                }
                Accept(Keywords.Endsy);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
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
                if (_context.SymbolCode == Symbols.Ident ||
                    _context.SymbolCode == Symbols.Semicolon)
                    SimpleStatement(followers);
                else if (_context.SymbolCode == Keywords.Beginsy ||
                            _context.SymbolCode == Keywords.Ifsy ||
                            _context.SymbolCode == Keywords.Whilesy)
                    ComplexStatement(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void SimpleStatement(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.SimpleStatement))
            {
                ListError(22);
                SkipTo2(Starters.SimpleStatement, followers);
            }
            if (SymbolBelong(Starters.SimpleStatement))
            {
                if (_context.SymbolCode == Symbols.Semicolon)
                {
                    //Accept(Symbols.Semicolon);
                }
                else
                {
                    AssignmentStatement(followers);
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void AssignmentStatement(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.AssignmnetStatement))
            {
                ListError(22);
                SkipTo2(Starters.AssignmnetStatement, followers);
            }
            if (SymbolBelong(Starters.AssignmnetStatement))
            {
                var symbols = Union(Followers.AssignmentStatementVariable, followers);
                Variable(symbols);
                Accept(Symbols.Assign);
                Expression(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void Variable(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.Variable))
            {
                ListError(22);
                SkipTo2(Starters.Variable, followers);
            }
            if (SymbolBelong(Starters.Variable))
            {
                Accept(Symbols.Ident);
                while (_context.SymbolCode == Symbols.Lbracket)
                {
                    Accept(Symbols.Lbracket);
                    var symbols = Union(Followers.VariableExpression, followers);
                    Expression(symbols);
                    while (_context.SymbolCode == Symbols.Comma)
                    {
                        Accept(Symbols.Comma);
                        Expression(symbols);
                    }
                    Accept(Symbols.Rbracket);
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void Expression(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.Expression))
            {
                ListError(23);
                SkipTo2(Starters.Expression, followers);
            }
            if (SymbolBelong(Starters.Expression))
            {
                var symbols = Union(Followers.ExpressionSimpleExpression, followers);
                SimpleExpression(symbols);
                if (_context.SymbolCode == Symbols.Equal ||
                    _context.SymbolCode == Symbols.Latergreater ||
                    _context.SymbolCode == Symbols.Later ||
                    _context.SymbolCode == Symbols.Laterequal ||
                    _context.SymbolCode == Symbols.Greaterequal ||
                    _context.SymbolCode == Symbols.Greater)
                {
                    _lexicalAnalyzerModule.NextSymbol();
                    SimpleExpression(followers);
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void SimpleExpression(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.SimpleExpression))
            {
                ListError(22);
                SkipTo2(Starters.SimpleExpression, followers);
            }
            if (SymbolBelong(Starters.SimpleExpression))
            {
                var symbols = Union(Followers.SimpleExpressionSign, followers);
                if (_context.SymbolCode == Symbols.Plus ||
                    _context.SymbolCode == Symbols.Minus)
                    Sign(symbols);
                symbols = Union(Followers.SimpleExpressionAddend, followers);
                Term(symbols);
                if (_context.SymbolCode == Symbols.Plus ||
                    _context.SymbolCode == Symbols.Minus ||
                    _context.SymbolCode == Keywords.Orsy)
                {
                    symbols = Union(Followers.SimpleExpressionSign, followers);
                    AdditiveOperation(symbols);
                    Term(followers);
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void Sign(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.Sign))
            {
                ListError(22);
                SkipTo2(Starters.Sign, followers);
            }
            if (_context.SymbolCode == Symbols.Plus ||
                _context.SymbolCode == Symbols.Minus)
            {
                _lexicalAnalyzerModule.NextSymbol();
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void Term(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.Term))
            {
                ListError(22);
                SkipTo2(Starters.Term, followers);
            }
            if (SymbolBelong(Starters.Term))
            {
                var symbols = Union(Followers.AddendMultiplier, followers);
                Factor(symbols);
                if (_context.SymbolCode == Symbols.Star ||
                    _context.SymbolCode == Symbols.Slash ||
                    _context.SymbolCode == Keywords.Divsy ||
                    _context.SymbolCode == Keywords.Modsy ||
                    _context.SymbolCode == Keywords.Andsy)
                {
                    symbols = Union(Followers.AddendMultiplicativeOperation, followers);
                    MultiplicativeOperation(symbols);
                    Factor(followers);
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        // TODO: неоднозначность ident
        private void Factor(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.Factor))
            {
                ListError(22);
                SkipTo2(Starters.Factor, followers);
            }
            if (SymbolBelong(Starters.Factor))
            {
                switch (_context.SymbolCode)
                {
                    case Symbols.Ident:
                        Variable(followers);
                        break;
                    case Symbols.Leftpar:
                        Accept(Symbols.Leftpar);
                        var symbols = Union(Followers.FactorExpression, followers);
                        Expression(symbols);
                        Accept(Symbols.Rightpar);
                        break;
                    case Keywords.Notsy:
                        Accept(Keywords.Notsy);
                        Factor(followers);
                        break;
                    default:
                        if (_context.SymbolCode == Symbols.Intc ||
                            _context.SymbolCode == Symbols.Floatc ||
                            _context.SymbolCode == Symbols.Charc ||
                            _context.SymbolCode == Symbols.Stringc ||
                            _context.SymbolCode == Symbols.Ident ||
                            _context.SymbolCode == Keywords.Nilsy)
                            _lexicalAnalyzerModule.NextSymbol();
                        break;
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void MultiplicativeOperation(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.MultiplicativeOperation))
            {
                ListError(22);
                SkipTo2(Starters.MultiplicativeOperation, followers);
            }
            if (SymbolBelong(Starters.MultiplicativeOperation))
            {
                _lexicalAnalyzerModule.NextSymbol();
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void AdditiveOperation(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.AdditiveOperation))
            {
                ListError(22);
                SkipTo2(Starters.AdditiveOperation, followers);
            }
            if (SymbolBelong(Starters.AdditiveOperation))
            {
                _lexicalAnalyzerModule.NextSymbol();
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void ComplexStatement(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.ComplexStatement))
            {
                ListError(22);
                SkipTo2(Starters.ComplexStatement, followers);
            }
            if (SymbolBelong(Starters.ComplexStatement))
            {
                switch (_context.SymbolCode)
                {
                    case Keywords.Beginsy:
                        CompoundStatement(followers);
                        break;
                    case Keywords.Ifsy:
                        ConditionalStatement(followers);
                        break;
                    case Keywords.Whilesy:
                        WhileStatement(followers);
                        break;
                }
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void ConditionalStatement(IEnumerable<int> followers)
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
                Expression(symbols);
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
                Expression(symbols);
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
