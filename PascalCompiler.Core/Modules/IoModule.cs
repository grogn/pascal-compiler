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
        private readonly List<Error> _errorsTable;
        private readonly Context _context;

        public IoModule(Context context)
        {
            _context = context;
            _context.Error += AddError;
            _errorsTable = new List<Error>();
            _context.LineNumber = 1;
            _context.CharNumber = -1;
            _currentErrorNumber = 1;
            ReadNextLine();
        }
        private void ReadNextLine()
        {
            _line = _context.SourceCodeDispatcher.ReadLine();
            ListCurrentLine();
        }
        private void ListCurrentLine()
        {
            _context.SourceCodeDispatcher.WriteLine($" {_context.LineNumber++.ToString().PadLeft(3)}  {_line}");
        }

        public void AddError(int errorPosition, int errorCode)
        {
            _context.SourceCodeDispatcher.WriteLine($"*{_currentErrorNumber++.ToString().PadLeft(3, '0')}* {"".PadLeft(errorPosition)}^ошибка код {errorCode}");
            _context.SourceCodeDispatcher.WriteLine($"***** {ErrorDescriptions.Get(errorCode)}");
        }

        public char PeekNextChar()
        {
            if (_context.CharNumber + 1 != _line.Length)
                return _line[_context.CharNumber + 1];
            return '\n';
        }

        public char NextChar()
        {
            if (_line != null && _context.CharNumber + 1 != _line.Length)
            {
                _context.CharNumber++;
                return _line[_context.CharNumber];
            }
            else
            {
                ReadNextLine();
                if (_line == null)
                {
                    return '\0';
                }
                _context.CharNumber = -1;
                return '\n';
            }
        }
    }
}
