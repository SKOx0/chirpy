
using System.Collections.Generic;
using Zippy.Chirp.Engines;

namespace Zippy.Chirp
{
    class EcmaScriptErrorReporter : EcmaScript.NET.ErrorReporter
    {
        public List<ErrorResult> Errors = new List<ErrorResult>();

        public void Error(string message, string sourceName, int line, string lineSource, int lineOffset)
        {
            Errors.Add(new ErrorResult(message, line, lineOffset));
        }

        public void Warning(string message, string sourceName, int line, string lineSource, int lineOffset)
        {
            // _Result.Warnings.Add(new Result.Error { Description = message, Line = line, Column = lineOffset });
        }

        public EcmaScript.NET.EcmaScriptRuntimeException RuntimeError(string message, string sourceName, int line, string lineSource, int lineOffset)
        {
            return new EcmaScript.NET.EcmaScriptRuntimeException(message, sourceName, line, lineSource, lineOffset);
        }

    }
}
