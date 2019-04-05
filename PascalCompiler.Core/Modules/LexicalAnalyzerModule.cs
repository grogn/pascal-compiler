﻿using PascalCompiler.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symbols = PascalCompiler.Core.Constants.Symbols;

namespace PascalCompiler.Core.Modules
{
    public class LexicalAnalyzerModule
    {
        private const int MaxInt = 32767;
        private const int MaxString = 20;
        private const int MaxName = 15;
        private readonly IoModule _ioModule;
        private readonly Context _context;

        private char _currentChar;
        public LexicalAnalyzerModule(Context context, IoModule ioModule)
        {
            _context = context;
            _ioModule = ioModule;
        }

        private void ListError(int errorCode)
        {
            var error = new Error(_context.CharNumber, errorCode);
            _context.OnError(error);
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
                // Пропускаем весь комментарий
                var prevChar = _currentChar;
                _currentChar = _ioModule.NextChar();
                while ((prevChar != '*' || _currentChar != ')') && 
                        _currentChar != '\0')
                {
                    prevChar = _currentChar;
                    _currentChar = _ioModule.NextChar();
                }
                if (prevChar == '*' && _currentChar == ')')
                    symbol = NextSymbol();
                else
                {
                    ListError(86);
                    symbol = Symbols.Endoffile;
                }
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
                ListError(85);
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
            var integerPart = _currentChar - '0';
            var listIntegerError = false;
            _currentChar = _ioModule.PeekNextChar();
            while (_currentChar >= '0' && _currentChar <= '9')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                var digit = _currentChar - '0';
                if (!listIntegerError && (integerPart < MaxInt / 10 || (integerPart == MaxInt / 10 && digit <= MaxInt % 10)))
                    integerPart = 10 * integerPart + digit;
                else
                    listIntegerError = true;
                _currentChar = _ioModule.PeekNextChar();
            }

            if (_currentChar != '.')
            {
                if (listIntegerError)
                    ListError(203);
                return Symbols.Intc;
            }

            var nextChar = _ioModule.PeekNextNextChar();
            if (nextChar < '0' || nextChar > '9')
                return Symbols.Intc;
            _ioModule.NextChar();
            _context.Symbol += _currentChar;
            _currentChar = _ioModule.PeekNextChar();

            if (_currentChar < '0' || _currentChar > '9')
            {
                ListError(201);
                return Symbols.Floatc;
            }

            var floatPart = 0;
            var listFloatError = false;
            while (_currentChar >= '0' && _currentChar <= '9')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                var digit = _currentChar - '0';
                if (!listFloatError && (floatPart < MaxInt / 10 || (floatPart == MaxInt / 10 && digit <= MaxInt % 10)))
                    floatPart = 10 * floatPart + digit;
                else
                    listFloatError = true;
                _currentChar = _ioModule.PeekNextChar();
            }

            if (listIntegerError || listFloatError)
                ListError(207);

            return Symbols.Floatc;
        }

        private int ScanString()
        {
            _ioModule.NextChar();
            _context.Symbol += _context.Char;
            if (_context.Char == '\'' || _context.Char == '\n')
            {
                ListError(75);
                return Symbols.Charc;
            }

            _ioModule.NextChar();
            _context.Symbol += _context.Char;

            if (_context.Char == '\'')
            {
                _context.Symbol += _context.Char;
                return Symbols.Charc;
            }

            if (_context.Char == '\n')
            {
                ListError(75);
                return Symbols.Charc;
            }

            _ioModule.NextChar();
            var length = 2;
            var listError = false;
            while (_context.Char != '\'')
            {
                _context.Symbol += _context.Char;
                if (length++ > MaxString)
                    listError = true;
                _ioModule.NextChar();
                if (_context.Char == '\n')
                {
                    ListError(75);
                    return Symbols.Stringc;
                }
            }
            _context.Symbol += _context.Char;
            if (listError)
                ListError(76);

            return Symbols.Stringc;
        }

        private int ScanName()
        {
            var nameLength = 1;
            _currentChar = _ioModule.PeekNextChar();
            while ((_currentChar >= 'a' && _currentChar <= 'z' ||
                    _currentChar >= 'A' && _currentChar <= 'Z' ||
                    _currentChar >= '0' && _currentChar <= '9' ||
                    _currentChar == '_') && nameLength <= MaxName)
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol += _currentChar;
                nameLength++;
                _currentChar = _ioModule.PeekNextChar();
            }

            int symbol;
            if (Keywords.ByName.ContainsKey(_context.Symbol.ToLower()))
            {
                symbol = Keywords.ByName[_context.Symbol.ToLower()];
            }
            else
            {
                symbol = Symbols.Ident;
            }

            return symbol;
        }

        private int ScanFlpar()
        {
            int symbol;
            _currentChar = _ioModule.NextChar();

            while (_currentChar != '}' && _currentChar != '\0')
                _currentChar = _ioModule.NextChar();

            if (_currentChar == '}')
                symbol = NextSymbol();
            else
            {
                ListError(86);
                symbol = Symbols.Endoffile;
            }

            return symbol;
        }

        private int ScanFrpar()
        {
            ListError(85);
            return Symbols.Frpar;
        }

        public int NextSymbol()
        {
            var symbolCode = Symbols.Endoffile;
            _currentChar = _ioModule.NextChar();
            _context.Symbol = _currentChar.ToString();
            // TODO: табы под вопросом - неправильно отображается позиция ошибки
            while (_currentChar == ' ' || _currentChar == '\t')
            {
                _currentChar = _ioModule.NextChar();
                _context.Symbol = _currentChar.ToString();
            }
            _context.SymbolPosition = _context.CharNumber;

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

                case '{':
                    symbolCode = ScanFlpar();
                    break;

                case '}':
                    symbolCode = ScanFrpar();
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

                case '+':
                    symbolCode = Symbols.Plus;
                    break;

                case '-':
                    symbolCode = Symbols.Minus;
                    break;

                case '\n':
                    symbolCode = Symbols.Endofline;
                    break;

                case '\0':
                    symbolCode = Symbols.Endoffile;
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
                        ListError(6);
                    }

                    break;
            }

            _context.SymbolCode = symbolCode;
            Logger.LogSymbol(_context.Symbol);
            if (_context.SymbolCode == Symbols.Endofline)
                return NextSymbol();
            return symbolCode;
        }
    }
}
