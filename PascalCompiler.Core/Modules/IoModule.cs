using PascalCompiler.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core.Modules
{
    public class IoModule
    {
        private readonly Context _context;

        public IoModule(Context context)
        {
            _context = context;
            _context.Error += ListError;
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

        private void ListError(Error error)
        {
            _context.SourceCodeDispatcher.WriteLine($"*{_context.ErrorNumber++.ToString().PadLeft(3, '0')}* {"^".PadLeft(error.Position)}ошибка код {error.Code}");
            _context.SourceCodeDispatcher.WriteLine($"***** {ErrorDescriptions.Get(error.Code)}");
        }

        public char PeekNextChar()
        {
            return _context.CharNumber < _context.Line.Length ? _context.Line[_context.CharNumber] : '\n';
        }

        public char NextChar()
        {
            if (_context.CharNumber != _context.Line.Length)
                _context.Char = _context.Line[_context.CharNumber++];
            else
            {
                ReadNextLine();
                _context.CharNumber = 0;
                _context.Char = _context.Line == null ? '\0' : '\n';
            }

            return _context.Char;
        }
    }
}
