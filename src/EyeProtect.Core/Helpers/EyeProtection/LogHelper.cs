using System;
using System.IO;

namespace EyeProtect.Core.Core
{
    public class LogHelper
    {
        public enum Level
        {
            /// <summary>
            /// 调试
            /// </summary>
            DEBUG,
            /// <summary>
            /// 错误
            /// </summary>
            ERROR,
            /// <summary>
            /// 警告
            /// </summary>
            WARNING
        }
        public static void Debug(string text, bool write = false, bool console = true)
        {
            Log(Level.DEBUG, text, write, console);
        }
        public static void Error(string text, bool write = true, bool console = true)
        {
            Log(Level.ERROR, text, write, console);
        }
        public static void Warning(string text, bool write = true, bool console = true)
        {
            Log(Level.WARNING, text, write, console);
        }
        private static void Log(Level level, string text, bool write = false, bool console = true)
        {
            var log = LogFormat(level, text);
            if (write)
            {
                //写入日志文件
                WriteFile(level, log);
            }
            if (console)
            {
                //在debug模式下打印到输出
                System.Diagnostics.Debug.WriteLine(log);
            }
        }
        private static void WriteFile(Level level, string text)
        {
            try
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                         "Log",
                         $"{level}_{DateTime.Now:yyyy_MM_dd}.log");
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.AppendAllText(filePath, text);
            }
            catch
            {
                //...
            }
        }
        private static string LogFormat(Level level, string text)
        {
            var logText = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}]\r\n{text}\r\n------------------------\r\n\r\n";
            return logText;
        }
    }
}
