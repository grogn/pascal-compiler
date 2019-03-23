using PascalCompiler.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core.Modules
{
    public class LexicalAnalyzerModule
    {
        private const int MaxInt = 32767;
        private const int MaxString = 10;
        private const int MaxName = 15;
        private readonly IoModule _ioModule;
        private readonly Context _context;

        private char _currentChar;
        public LexicalAnalyzerModule(Context context, IoModule ioModule)
        {
            _context = context;
            _ioModule = ioModule;
        }

        private int ScanLater()
        {
            _currentChar = _ioModule.PeekNextChar();
            int symbol;

            if (_currentChar == '=')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                symbol = Symbols.Laterequal;
            }
            else if (_currentChar == '>')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                symbol = Symbols.Latergreater;
            }
            else
            {
                symbol = Symbols.Later;
            }

            return symbol;
        }

        private int ScanGreater()
        {
            _currentChar = _ioModule.PeekNextChar();
            int symbol;

            if (_currentChar == '=')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                symbol = Symbols.Greaterequal;
            }
            else
            {
                symbol = Symbols.Greater;
            }

            return symbol;
        }

        private int ScanColon()
        {
            _currentChar = _ioModule.PeekNextChar();
            int symbol;

            if (_currentChar == '=')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                symbol = Symbols.Assign;
            }
            else
            {
                symbol = Symbols.Colon;
            }

            return symbol;
        }

        private int ScanPoint()
        {
            _currentChar = _ioModule.PeekNextChar();
            int symbol;

            if (_currentChar == '.')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                symbol = Symbols.Twopoints;
            }
            else
            {
                symbol = Symbols.Point;
            }

            return symbol;
        }

        private int ScanLeftpar()
        {
            _currentChar = _ioModule.PeekNextChar();
            int symbol;

            if (_currentChar == '*')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                symbol = Symbols.Lcomment;
            }
            else
            {
                symbol = Symbols.Leftpar;
            }

            return symbol;
        }

        private int ScanStar()
        {
            _currentChar = _ioModule.PeekNextChar();
            int symbol;

            if (_currentChar == ')')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                symbol = Symbols.Rcomment;
            }
            else
            {
                symbol = Symbols.Star;
            }

            return symbol;
        }

        private int ScanNumberConstant()
        {
            var number = _currentChar - '0';
            _currentChar = _ioModule.PeekNextChar();
            while (_currentChar >= '0' && _currentChar <= '9')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                var digit = _currentChar - '0';
                if (number < MaxInt / 10 || (number == MaxInt / 10 && digit <= MaxInt % 10))
                {
                    number = 10 * number + digit;
                }
                else
                {
                    _context.OnError(_context.CharNumber, 203);
                    number = 0;
                }
                _currentChar = _ioModule.PeekNextChar();
            }

            return Symbols.Intc;
        }

        private int ScanString()
        {
            _currentChar = _ioModule.PeekNextChar();
            if (_currentChar == '\'')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;

                _context.OnError(_context.CharNumber, 75);
                return Symbols.Charc;
            }

            if (_currentChar == '\n')
            {
                _context.OnError(_context.CharNumber, 75);
                return Symbols.Charc;
            }

            _currentChar = _ioModule.NextChar();
            _context.Symbol += _currentChar;

            _currentChar = _ioModule.PeekNextChar();
            if (_currentChar == '\'')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                return Symbols.Charc;
            }
            if (_currentChar == '\n')
            {
                _context.OnError(_context.CharNumber, 75);
                return Symbols.Charc;
            }
            var length = 2;
            _currentChar = _ioModule.PeekNextChar();
            while (_currentChar != '\'')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                length++;
                if (length > MaxString)
                {
                    _context.OnError(_context.CharNumber, 76);
                    length = 0;
                }
                _currentChar = _ioModule.PeekNextChar();
                if (_currentChar == '\n')
                {
                    _context.OnError(_context.CharNumber, 75);
                    return Symbols.Stringc;
                }
            }

            return Symbols.Stringc;
        }

        private int ScanName()
        {
            var nameLength = 1;
            var name = string.Empty + _currentChar;
            _currentChar = _ioModule.PeekNextChar();
            while ((_currentChar >= 'a' && _currentChar <= 'z' ||
                    _currentChar >= 'A' && _currentChar <= 'Z' ||
                    _currentChar >= '0' && _currentChar <= '9') && nameLength <= MaxName)
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                name += _currentChar;
                nameLength++;
                _currentChar = _ioModule.PeekNextChar();
            }

            var symbol = 0;

            return symbol;
        }

        public int NextSymbol()
        {
            var symbolCode = Symbols.Endoffile;
            _currentChar = _ioModule.NextChar();
            _context.Symbol = _currentChar.ToString();
            while (_currentChar == ' ')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol = _currentChar.ToString();
            }

            switch (_currentChar)
            {
                case '\'':
                    symbolCode = ScanString();
                    break;

                case '<':
                    symbolCode = ScanLater();
                    break;

                case '>':
                    symbolCode = ScanGreater();
                    break;

                case ':':
                    symbolCode = ScanColon();
                    break;

                case '.':
                    symbolCode = ScanPoint();
                    break;

                case '*':
                    symbolCode = ScanStar();
                    break;

                case '(':
                    symbolCode = ScanLeftpar();
                    break;

                case ')':
                    symbolCode = Symbols.Rightpar;
                    break;

                case ';':
                    symbolCode = Symbols.Semicolon;
                    break;

                case '/':
                    symbolCode = Symbols.Slash;
                    break;

                case '=':
                    symbolCode = Symbols.Equal;
                    break;

                case ',':
                    symbolCode = Symbols.Comma;
                    break;

                case '^':
                    symbolCode = Symbols.Arrow;
                    break;

                case '[':
                    symbolCode = Symbols.Lbracket;
                    break;

                case ']':
                    symbolCode = Symbols.Rbracket;
                    break;

                case '{':
                    symbolCode = Symbols.Flpar;
                    break;

                case '}':
                    symbolCode = Symbols.Frpar;
                    break;

                case '+':
                    symbolCode = Symbols.Plus;
                    break;

                case '-':
                    symbolCode = Symbols.Minus;
                    break;

                case '\n':
                    symbolCode = Symbols.Endofline;
                    break;
                default:
                    if (_currentChar >= '0' && _currentChar <= '9')
                    {
                        symbolCode = ScanNumberConstant();
                    } else if (_currentChar >= 'a' && _currentChar <= 'z' ||
                               _currentChar >= 'A' && _currentChar <= 'Z')
                    {
                        symbolCode = ScanName();
                    }
                    else
                    {
                        _context.OnError(_context.CharNumber, 6);
                    }

                    break;
            }

            return symbolCode;
        }
    }
}
