using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PascalCompiler.Core.Constants;

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
            _context.OnError(new Error(_context.CharNumber, errorCode));
        }

        private void Accept(int symbolCode)
        {
            if (_context.SymbolCode == symbolCode)
                _lexicalAnalyzerModule.NextSymbol();
            else
                _context.OnError(new Error(_context.CharNumber, symbolCode));
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

        public void Program()
        {
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

        private void Type(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.Type))
            {
                ListError(18);
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
                    SimpleType(followers);
                else if (_context.SymbolCode == Keywords.Arraysy)
                    CompositeType(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }
        
        private void SimpleType(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.SimpleType))
            {
                ListError(18);
                SkipTo2(Starters.SimpleType, followers);
            }
            if (SymbolBelong(Starters.SimpleType))
            {
                if (_context.SymbolCode == Symbols.Leftpar)
                    EnumerationType(followers);
                else if (_context.SymbolCode == Symbols.Ident)
                    Accept(Symbols.Ident);
                else if (_context.SymbolCode == Symbols.Intc ||
                        _context.SymbolCode == Symbols.Floatc ||
                        _context.SymbolCode == Symbols.Charc ||
                        _context.SymbolCode == Symbols.Stringc ||
                        _context.SymbolCode == Symbols.Plus ||
                        _context.SymbolCode == Symbols.Minus ||
                        _context.SymbolCode == Symbols.Ident)
                    LimitedType(followers);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void EnumerationType(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.EnumerationType))
            {
                ListError(18);
                SkipTo2(Starters.EnumerationType, followers);
            }
            if (SymbolBelong(Starters.EnumerationType))
            {
                Accept(Symbols.Leftpar);
                Accept(Symbols.Ident);
                while (_context.SymbolCode == Symbols.Comma)
                {
                    Accept(Symbols.Comma);
                    Accept(Symbols.Ident);
                }
                Accept(Symbols.Rightpar);
                if (!SymbolBelong(followers))
                {
                    ListError(6);
                    SkipTo(followers);
                }
            }
        }

        private void LimitedType(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.LimitedType))
            {
                ListError(18);
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
        }

        private void CompositeType(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.CompositeType))
            {
                ListError(18);
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
        }

        private void ArrayType(IEnumerable<int> followers)
        {
            if (!SymbolBelong(Starters.ArrayType))
            {
                ListError(18);
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
                ListError(18);
                SkipTo2(Starters.VarDeclaration, followers);
            }
            if (SymbolBelong(Starters.VarDeclaration))
            {
                Accept(Symbols.Ident);
                while (_context.SymbolCode == Symbols.Comma)
                {
                    Accept(Symbols.Comma);
                    Accept(Symbols.Ident);
                }
                Accept(Symbols.Colon);
                Type(followers);
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
                ListError(18);
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
                ListError(18);
                SkipTo2(Starters.Statement, followers);
            }
            if (SymbolBelong(Starters.Statement))
            {
                if (_context.SymbolCode == Symbols.Ident)
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
                ListError(18);
                SkipTo2(Starters.SimpleStatement, followers);
            }
            if (SymbolBelong(Starters.SimpleStatement))
            {
                AssignmentStatement(followers);
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
                ListError(18);
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
                ListError(18);
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
                ListError(18);
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
                ListError(18);
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
                ListError(18);
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
                ListError(18);
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
                ListError(18);
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
                ListError(18);
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
                ListError(18);
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
                ListError(18);
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
                ListError(18);
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
                ListError(18);
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
