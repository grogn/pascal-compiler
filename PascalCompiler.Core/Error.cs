﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Core
{
    public class Error
    {
        public int Position { get; }
        public int Code { get; }

        public Error(int position, int code)
        {
            Position = position;
            Code = code;
        }
    }
}
