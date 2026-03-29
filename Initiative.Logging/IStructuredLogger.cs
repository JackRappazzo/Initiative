using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Logging
{
    public interface IStructuredLogger
    {

        void Verbose(string messageTemplate, params object?[]? propertyValues);
        void Debug(string messageTemplate, params object?[]? propertyValues);
        void Information(string messageTemplate, params object?[]? propertyValues);
        void Warning(Exception ex, string messageTemplate, params object?[]? propertyValues);
        void Warning(string messageTemplate, params object?[]? propertyValues);
        void Error(string messageTemplate, params object?[]? propertyValues);
        void Error(Exception ex, string messageTemplate, params object?[]? propertyValues);

        IStructuredLogger ForContext(string propertyName, object? value);
    }
}
