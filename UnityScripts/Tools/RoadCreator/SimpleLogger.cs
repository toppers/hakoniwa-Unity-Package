using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoniwa.Core.Utils.Logger
{
    public class SimpleLogger : ISimpleLogger
    {
        private static SimpleLogger default_logger = null;
        public static SimpleLogger Get()
        {
            if (default_logger == null)
            {
                string filepath = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "hakoniwa_core.log";

                default_logger = new SimpleLogger(filepath, false);
            }
            return default_logger;
        }
        private static readonly string LOG_FORMAT = "{0} {1} {2}";
        private static readonly string DATETIME_FORMAT = "yyyy/MM/dd HH:mm:ss.fff";
        private StreamWriter stream;

        public SimpleLogger(string logpath, bool append)
        {
            var logFile = new FileInfo(logpath);
            if (!Directory.Exists(logFile.DirectoryName))
            {
                Directory.CreateDirectory(logFile.DirectoryName);
            }
            stream = new StreamWriter(logFile.FullName, append, Encoding.Default);
            stream.AutoFlush = true;
        }
        private void write(Level level, string text)
        {
            string log = string.Format(LOG_FORMAT, DateTime.Now.ToString(DATETIME_FORMAT), level.ToString(), text);
            stream.WriteLine(log);
        }
        public void Log(Level level, string text)
        {
            this.write(level, text);
        }

        public void Log(Level level, Exception ex)
        {
            write(level, ex.Message + Environment.NewLine + ex.StackTrace);
        }

        public void Log(Level level, string format, object arg)
        {
            this.Log(level, string.Format(format, arg));
        }

        public void Log(Level level, string format, params object[] args)
        {
            this.Log(level, string.Format(format, args));
        }
    }
}