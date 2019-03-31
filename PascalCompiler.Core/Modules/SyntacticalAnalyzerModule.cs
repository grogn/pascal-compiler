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

        private void Accept(int symbolCode)
        {
            if (_context.SymbolCode == symbolCode)
                _lexicalAnalyzerModule.NextSymbol();
            else
                _context.OnError(new Error(_context.CharNumber, symbolCode));
        }

        public void Program()
        {
            _lexicalAnalyzerModule.NextSymbol();
            Accept(Keywords.Programsy);
            Accept(Symbols.Ident);
            Accept(Symbols.Semicolon);
            Block();
            Accept(Symbols.Point);
        }

        private void Block()
        {
            // TODO: labelpart
            ConstPart();
            TypePart();
            VarPart();
            // TODO: procfuncpart
            StatementsPart();
        }

        private void ConstPart()
        {
            if (_context.SymbolCode == Keywords.Constsy)
            {
                Accept(Keywords.Constsy);
                do
                {
                    ConstDeclaration();
                    Accept(Symbols.Semicolon);
                } while (_context.SymbolCode == Symbols.Ident);
            }
        }

        private void ConstDeclaration()
        {
            Accept(Symbols.Ident);
            Accept(Symbols.Equal);
            Const();
        }

        private void Const()
        {
            switch (_context.SymbolCode)
            {
                case Symbols.Intc:
                    _lexicalAnalyzerModule.NextSymbol();
                    break;
                case Symbols.Floatc:
                    _lexicalAnalyzerModule.NextSymbol();
                    break;
                case Symbols.Stringc:
                    _lexicalAnalyzerModule.NextSymbol();
                    break;
                case Symbols.Charc:
                    _lexicalAnalyzerModule.NextSymbol();
                    break;
                case Symbols.Ident:
                    _lexicalAnalyzerModule.NextSymbol();
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
        }

        private void TypePart()
        {
            if (_context.SymbolCode == Keywords.Typesy)
            {
                Accept(Keywords.Typesy);
                do
                {
                    TypeDeclaration();
                    Accept(Symbols.Semicolon);
                } while (_context.SymbolCode == Symbols.Ident);
            }
        }

        private void TypeDeclaration()
        {
            Accept(Symbols.Ident);
            Accept(Symbols.Equal);
            Type();
        }

        private void Type()
        {
            switch (_context.SymbolCode)
            {
                case Symbols.Arrow:
                    ReferenceType();
                    break;
                default:
                    if (_context.SymbolCode == Keywords.Arraysy ||
                        _context.SymbolCode == Keywords.Recordsy ||
                        _context.SymbolCode == Keywords.Setsy ||
                        _context.SymbolCode == Keywords.Filesy ||
                        _context.SymbolCode == Keywords.Packedsy)
                        CompositeType();
                    if (_context.SymbolCode == Symbols.Intc ||
                        _context.SymbolCode == Symbols.Floatc ||
                        _context.SymbolCode == Symbols.Charc ||
                        _context.SymbolCode == Symbols.Stringc ||
                        _context.SymbolCode == Symbols.Plus ||
                        _context.SymbolCode == Symbols.Minus ||
                        _context.SymbolCode == Symbols.Ident ||
                        _context.SymbolCode == Symbols.Leftpar)
                        SimpleType();
                    break;
            }
        }

        // TODO: косяк - два indent, неоднозначность
        private void SimpleType()
        {
            switch (_context.SymbolCode)
            {
                case Symbols.Leftpar:
                    EnumerationType();
                    break;
                case Symbols.Ident:
                    TypeName();
                    break;
                default:
                    if (_context.SymbolCode == Symbols.Intc ||
                        _context.SymbolCode == Symbols.Floatc ||
                        _context.SymbolCode == Symbols.Charc ||
                        _context.SymbolCode == Symbols.Stringc ||
                        _context.SymbolCode == Symbols.Plus ||
                        _context.SymbolCode == Symbols.Minus ||
                        _context.SymbolCode == Symbols.Ident)
                        LimitedType();
                    break;
            }
        }

        private void EnumerationType()
        {
            Accept(Symbols.Leftpar);
            do
            {
                Accept(Symbols.Ident);
                Accept(Symbols.Comma);
            } while (_context.SymbolCode == Symbols.Ident);
            Accept(Symbols.Rightpar);
        }

        private void TypeName()
        {
            Accept(Symbols.Ident);
        }

        private void LimitedType()
        {
            Const();
            Accept(Symbols.Twopoints);
            Const();
        }

        private void ReferenceType()
        {
            Accept(Symbols.Arrow);
            TypeName();
        }

        private void CompositeType()
        {
            if (_context.SymbolCode == Keywords.Packedsy)
                _lexicalAnalyzerModule.NextSymbol();
            UnpackedCompositeType();
        }

        private void UnpackedCompositeType()
        {
            switch (_context.SymbolCode)
            {
                case Keywords.Arraysy:
                    ArrayType();
                    break;
                case Keywords.Recordsy:
                    RecordType();
                    break;
                case Keywords.Setsy:
                    SetType();
                    break;
                case Keywords.Filesy:
                    FileType();
                    break;
            }
        }

        private void ArrayType()
        {
            Accept(Keywords.Arraysy);
            Accept(Symbols.Lbracket);
            SimpleType();
            while (_context.SymbolCode == Symbols.Comma)
            {
                _lexicalAnalyzerModule.NextSymbol();
                SimpleType();
            }
            Accept(Symbols.Rbracket);
            Accept(Keywords.Ofsy);
            ComponentType();
        }

        private void ComponentType()
        {
            Type();
        }

        private void RecordType()
        {
            Accept(Keywords.Recordsy);
            FieldList();
            Accept(Keywords.Endsy);
        }

        private void FieldList()
        {
            switch (_context.SymbolCode)
            {
                case Symbols.Ident:
                    FixedPart();
                    if (_context.SymbolCode == Keywords.Casesy)
                        VariantPart();
                    break;
                case Keywords.Casesy:
                    VariantPart();
                    break;
            }
        }

        private void FixedPart()
        {
            RecordingSection();
            while (_context.SymbolCode == Symbols.Semicolon)
            {
                Accept(Symbols.Semicolon);
                RecordingSection();
            }
        }

        private void RecordingSection()
        {
            if (_context.SymbolCode == Symbols.Ident)
            {
                FieldName();
                while (_context.SymbolCode == Symbols.Comma)
                {
                    Accept(Symbols.Comma);
                    FieldName();
                }
                Accept(Symbols.Colon);
                Type();
            }
        }

        private void FieldName()
        {
            Accept(Symbols.Ident);
        }

        private void VariantPart()
        {
            Accept(Keywords.Casesy);
            FeatureField();
            TypeName();
            Accept(Keywords.Ofsy);
            Variant();
            while (_context.SymbolCode == Symbols.Semicolon)
            {
                Accept(Symbols.Semicolon);
                Variant();
            }
        }

        private void FeatureField()
        {
            if (_context.SymbolCode == Symbols.Ident)
            {
                FieldName();
                Accept(Symbols.Colon);
            }
        }

        private void Variant()
        {
            if (_context.SymbolCode == Symbols.Intc ||
                _context.SymbolCode == Symbols.Floatc ||
                _context.SymbolCode == Symbols.Charc ||
                _context.SymbolCode == Symbols.Stringc ||
                _context.SymbolCode == Symbols.Plus ||
                _context.SymbolCode == Symbols.Minus ||
                _context.SymbolCode == Symbols.Ident)
            {
                VariantTagList();
                Accept(Symbols.Colon);
                Accept(Symbols.Leftpar);
                FieldList();
                Accept(Symbols.Rightpar);
            }
        }

        private void VariantTagList()
        {
            VariantTag();
            while (_context.SymbolCode == Symbols.Comma)
            {
                Accept(Symbols.Comma);
                VariantTag();
            }
        }

        private void VariantTag()
        {
            Const();
        }

        private void SetType()
        {
            Accept(Keywords.Setsy);
            Accept(Keywords.Ofsy);
            BaseType();
        }

        private void BaseType()
        {
            SimpleType();
        }

        private void FileType()
        {
            Accept(Keywords.Filesy);
            Accept(Keywords.Ofsy);
            Type();
        }

        private void VarPart()
        {
            if (_context.SymbolCode == Keywords.Varsy)
            {
                Accept(Keywords.Varsy);
                do
                {
                    VarDeclaration();
                    Accept(Symbols.Semicolon);
                } while (_context.SymbolCode == Symbols.Ident);
            }
        }

        private void VarDeclaration()
        {
            Accept(Symbols.Ident);
            while (_context.SymbolCode == Symbols.Comma)
            {
                _lexicalAnalyzerModule.NextSymbol();
                Accept(Symbols.Ident);
            }
            Accept(Symbols.Colon);
            Type();
        }

        private void StatementsPart()
        {
            CompoundStatement();
        }

        private void CompoundStatement()
        {
            Accept(Keywords.Beginsy);
            Statement();
            while (_context.SymbolCode == Symbols.Semicolon)
            {
                Accept(Symbols.Semicolon);
                Statement();
            }
            Accept(Keywords.Endsy);
        }

        private void Statement()
        {
            if (_context.SymbolCode == Symbols.Intc)
                Tag();
            UntaggedStatement();
        }

        private void Tag()
        {
            Accept(Symbols.Intc);
        }

        private void UntaggedStatement()
        {
            switch (_context.SymbolCode)
            {
                case Symbols.Ident:
                    SimpleStatement();
                    break;
                default:
                    if (_context.SymbolCode == Keywords.Beginsy ||
                        _context.SymbolCode == Keywords.Ifsy)
                        ComplexStatement();
                    break;
            }
        }

        private void SimpleStatement()
        {
            if (_context.SymbolCode == Symbols.Ident)
            {
                AssignmnetStatement();
            }
        }

        private void AssignmnetStatement()
        {
            // TODO: так то еще можно функции присваивать, но делать этого я конечно же не буду
            Variable();
            Accept(Symbols.Assign);
            Expression();
        }

        private void Variable()
        {
            Accept(Symbols.Ident);
            while (_context.SymbolCode == Symbols.Lbracket ||
                   _context.SymbolCode == Symbols.Point ||
                   _context.SymbolCode == Symbols.Arrow)
                switch (_context.SymbolCode)
                {
                    case Symbols.Lbracket:
                        _lexicalAnalyzerModule.NextSymbol();
                        Expression();
                        while (_context.SymbolCode == Symbols.Comma)
                        {
                            _lexicalAnalyzerModule.NextSymbol();
                            Expression();
                        }
                        Accept(Symbols.Lbracket);
                        break;
                    case Symbols.Point:
                        _lexicalAnalyzerModule.NextSymbol();
                        Accept(Symbols.Ident);
                        break;
                    case Symbols.Arrow:
                        _lexicalAnalyzerModule.NextSymbol();
                        break;
                }
        }

        private void Expression()
        {
            SimpleExpression();
            if (_context.SymbolCode == Symbols.Equal ||
                _context.SymbolCode == Symbols.Latergreater ||
                _context.SymbolCode == Symbols.Later ||
                _context.SymbolCode == Symbols.Laterequal ||
                _context.SymbolCode == Symbols.Greaterequal ||
                _context.SymbolCode == Symbols.Greater ||
                _context.SymbolCode == Keywords.Insy)
            {
                _lexicalAnalyzerModule.NextSymbol();
                SimpleExpression();
            }
        }

        private void SimpleExpression()
        {
            Sign();
            Addend();
            if (_context.SymbolCode == Symbols.Plus ||
                _context.SymbolCode == Symbols.Minus ||
                _context.SymbolCode == Keywords.Orsy)
            {
                AdditiveOperation();
                Addend();
            }
        }

        private void Sign()
        {
            if (_context.SymbolCode == Symbols.Plus ||
                _context.SymbolCode == Symbols.Minus)
                _lexicalAnalyzerModule.NextSymbol();
        }

        private void Addend()
        {
            Multiplier();
            if (_context.SymbolCode == Symbols.Star ||
                _context.SymbolCode == Symbols.Slash ||
                _context.SymbolCode == Keywords.Divsy ||
                _context.SymbolCode == Keywords.Modsy ||
                _context.SymbolCode == Keywords.Andsy)
            {
                MultiplicativeOperation();
                Multiplier();
            }
        }

        private void AdditiveOperation()
        {
            if (_context.SymbolCode == Symbols.Plus ||
                _context.SymbolCode == Symbols.Minus ||
                _context.SymbolCode == Keywords.Orsy)
                _lexicalAnalyzerModule.NextSymbol();
        }

        // TODO: неоднозначность ident
        private void Multiplier()
        {
            switch (_context.SymbolCode)
            {
                case Symbols.Ident:
                    Variable();
                    break;
                case Symbols.Leftpar:
                    _lexicalAnalyzerModule.NextSymbol();
                    Expression();
                    Accept(Symbols.Rightpar);
                    break;
                case Keywords.Notsy:
                    _lexicalAnalyzerModule.NextSymbol();
                    Multiplier();
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
        }

        private void MultiplicativeOperation()
        {
            if (_context.SymbolCode == Symbols.Star ||
                _context.SymbolCode == Symbols.Slash ||
                _context.SymbolCode == Keywords.Divsy ||
                _context.SymbolCode == Keywords.Modsy ||
                _context.SymbolCode == Keywords.Andsy)
                _lexicalAnalyzerModule.NextSymbol();
        }

        private void ComplexStatement()
        {
            switch (_context.SymbolCode)
            {
                case Keywords.Beginsy:
                    CompoundStatement();
                    break;
                case Keywords.Ifsy:
                    ConditionalStatement();
                    break;
                case Keywords.Whilesy:
                    WhileStatement();
                    break;
            }
        }

        private void ConditionalStatement()
        {
            Accept(Keywords.Ifsy);
            Expression();
            Accept(Keywords.Thensy);
            Statement();
            if (_context.SymbolCode == Keywords.Elsesy)
            {
                Accept(Keywords.Elsesy);
                Statement();
            }
        }

        private void WhileStatement()
        {
            Accept(Keywords.Whilesy);
            Expression();
            Accept(Keywords.Dosy);
            Statement();
        }
    }
}
