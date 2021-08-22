using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Utils.Logger
{
    public enum Level
    {
        ERROR = 0,
        WARN,
        INFO,
        DEBUG,
        TRACE
    }
    public interface ISimpleLogger
    {
        void Log(Level level, string text);
        void Log(Level level, Exception ex);
        void Log(Level level, string format, object arg);
        void Log(Level level, string format, params object[] args);
    }
}