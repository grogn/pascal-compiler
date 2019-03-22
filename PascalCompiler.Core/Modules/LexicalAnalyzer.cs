﻿using PascalCompiler.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core.Modules
{
    public class LexicalAnalyzer
    {
        private const int maxInt = 32767;
        private IoModule _ioModule;
        private ICompilerContext _compilerContext;
        private TextPosition _tokenPosition;
        private char _currentChar;
        public LexicalAnalyzer(ICompilerContext compiler, IoModule ioModule)
        {
            _compilerContext = compiler;
            _ioModule = ioModule;
        }

        private int ScanLater()
        {
            _currentChar = _ioModule.NextChar();
            int symbol;

            if (_currentChar == '=')
            {
                symbol = Symbols.Laterequal;
            }
            else if (_currentChar == '>')
            {
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
            _currentChar = _ioModule.NextChar();
            int symbol;

            if (_currentChar == '=')
            {
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
            _currentChar = _ioModule.NextChar();
            int symbol;

            if (_currentChar == '=')
            {
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
            _currentChar = _ioModule.NextChar();
            int symbol;

            if (_currentChar == '.')
            {
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
            _currentChar = _ioModule.NextChar();
            int symbol;

            if (_currentChar == '*')
            {
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
            _currentChar = _ioModule.NextChar();
            int symbol;

            if (_currentChar == ')')
            {
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
            var numInt = 0;
            while (_currentChar >= '0' && _currentChar <= '9')
            {
                var digit = _currentChar - '0';
                if (numInt < maxInt / 10 || (numInt == maxInt / 10 && digit <= maxInt % 10))
                {
                    numInt = 10 * numInt + digit;
                }
                else
                {
                    _compilerContext.OnError(_tokenPosition, 203);
                    numInt = 0;
                }
                _currentChar = _ioModule.NextChar();
            }

            return Symbols.Intc;
        }

        public int NextSymbol()
        {
            int symbol = -1;
            while (_currentChar == ' ')
            {
                _currentChar = _ioModule.NextChar();
            }

            switch (_currentChar)
            {
                case '\'':
                    // TODO
                    break;

                case '<':
                    symbol = ScanLater();
                    break;

                case '>':
                    symbol = ScanGreater();
                    break;

                case ':':
                    symbol = ScanColon();
                    break;

                case '.':
                    symbol = ScanPoint();
                    break;

                case '*':
                    symbol = ScanStar();
                    break;

                case '(':
                    symbol = ScanLeftpar();
                    break;

                case ')':
                    symbol = Symbols.Rightpar;
                    break;

                case ';':
                    symbol = Symbols.Semicolon;
                    break;

                case '/':
                    symbol = Symbols.Slash;
                    break;

                case '=':
                    symbol = Symbols.Equal;
                    break;

                case ',':
                    symbol = Symbols.Comma;
                    break;

                case '^':
                    symbol = Symbols.Arrow;
                    break;

                case '[':
                    symbol = Symbols.Lbracket;
                    break;

                case ']':
                    symbol = Symbols.Rbracket;
                    break;

                case '{':
                    symbol = Symbols.Flpar;
                    break;

                case '}':
                    symbol = Symbols.Frpar;
                    break;

                case '+':
                    symbol = Symbols.Plus;
                    break;

                case '-':
                    symbol = Symbols.Minus;
                    break;
                default:
                    if (_currentChar >= '0' && _currentChar <= '9')
                    {
                        ScanNumberConstant();
                    }
                    break;
            }

            return symbol;
        }
    }
}
