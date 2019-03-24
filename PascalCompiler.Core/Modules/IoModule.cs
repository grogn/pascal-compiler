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
        private int _currentErrorNumber;
        private readonly Context _context;

        public IoModule(Context context)
        {
            _context = context;
            _context.Error += AddError;
            _context.LineNumber = 1;
            _context.CharNumber = 0;
            _currentErrorNumber = 1;
            ReadNextLine();
        }
        private void ReadNextLine()
        {
            _context.Line = _context.SourceCodeDispatcher.ReadLine();
            if (_context.Line != null)
                ListCurrentLine();
        }
        private void ListCurrentLine()
        {
            _context.SourceCodeDispatcher.WriteLine($" {_context.LineNumber++.ToString().PadLeft(3)}  {_context.Line}");
        }

        public void AddError(int errorPosition, int errorCode)
        {
            _context.SourceCodeDispatcher.WriteLine($"*{_currentErrorNumber++.ToString().PadLeft(3, '0')}* {"^".PadLeft(errorPosition)}ошибка код {errorCode}");
            _context.SourceCodeDispatcher.WriteLine($"***** {ErrorDescriptions.Get(errorCode)}");
        }

        public char PeekNextChar()
        {
            return _context.CharNumber < _context.Line.Length ? _context.Line[_context.CharNumber] : '\n';
        }

        public char NextChar()
        {
            if (_context.CharNumber != _context.Line.Length)
                return _context.Line[_context.CharNumber++];

            ReadNextLine();
            _context.CharNumber = 0;
            return _context.Line == null ? '\0' : '\n';
        }
    }
}
