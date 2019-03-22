using PascalCompiler.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core.Modules
{
    public class IoModule
    {
        private string _line;
        private int _currentErrorNumber;
        private List<Error> _errorsTable;
        private TextPosition _currentPosition;
        private readonly ICompilerContext _compilerContext;

        public IoModule(ICompilerContext compiler)
        {
            _compilerContext = compiler;
            _compilerContext.Error += AddError;
            _errorsTable = new List<Error>();
            _currentPosition.LineNumber = 1;
            _currentPosition.CharNumber = -1;
            _currentErrorNumber = 1;
            ReadNextLine();
        }
        private void ReadNextLine()
        {
            _line = _compilerContext.ReadLine();
        }
        private void ListCurrentLine()
        {
            _compilerContext.WriteLine($" {_currentPosition.LineNumber.ToString().PadLeft(3)}  {_line}");
        }
        private void ListErrors()
        {
            foreach (var error in _errorsTable)
            {
                _compilerContext.WriteLine($"*{error.Number.ToString().PadLeft(3, '0')}* {"".PadLeft(error.Position.CharNumber)}^ошибка код {error.Code}");
                _compilerContext.WriteLine($"***** {ErrorDescriptions.Get(error.Code)}");
            }
        }

        public void AddError(TextPosition errorPosition, int errorCode)
        {
            _errorsTable.Add(new Error(_currentErrorNumber++, errorPosition, errorCode));
        }

        public char PeekNextChar()
        {
            if (_currentPosition.CharNumber + 1 != _line.Length)
                return _line[_currentPosition.CharNumber + 1];
            return '\n';
        }

        public char NextChar()
        {
            if (_currentPosition.CharNumber + 1 != _line.Length)
            {
                _currentPosition.CharNumber++;
                return _line[_currentPosition.CharNumber];
            }
            else
            {
                ListCurrentLine();
                ListErrors();
                _errorsTable.Clear();
                ReadNextLine();
                if (_line == null)
                {
                    return '\0';
                }
                _currentPosition.LineNumber++;
                _currentPosition.CharNumber = -1;
                return '\n';
            }
        }
    }
}
