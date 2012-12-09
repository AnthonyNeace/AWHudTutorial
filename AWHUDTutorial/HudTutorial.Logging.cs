using System;
using System.Threading;
using System.Runtime.ExceptionServices;

namespace AWHudTutorial
{
    [Flags]
    public enum LogLevels
    {
        /// <summary>
        /// No logging
        /// </summary>
        None = 0,
        /// <summary>
        /// Debugging loops and intricate micro-timed workings
        /// </summary>
        Fine = 1,
        /// <summary>
        /// Debugging minor functions
        /// </summary>
        Debug = 2,
        /// <summary>
        /// Reports at an interval, load messages
        /// </summary>
        Info = 4,
        /// <summary>
        /// Something goes wrong or unexpected data, but not critica
        /// </summary>
        Warning = 8,
        /// <summary>
        /// Critical error, crash or something stopping bMotion or instance
        /// </summary>
        Severe = 16,

        /// <summary>
        /// All logging levels
        /// </summary>
        All = Fine | Debug | Info | Warning | Severe,
        /// <summary>
        /// Quieter debug levels (no fine)
        /// </summary>
        Debugging = Debug | Info | Warning | Severe,
        /// <summary>
        /// Production levels (no debug/fine)
        /// </summary>
        Production = Info | Warning | Severe,
        /// <summary>
        /// Disables chat and info output
        /// </summary>
        NoChat = Warning | Severe,
    }

    public delegate void LogHandler(LogLevels l, string tag, string message, object[] parts);
    public static class Log
    {
        public static LogLevels LogLevel;
        public static event LogHandler Logged;

        static bool log(LogLevels l, string tag, string message, object[] parts)
        {
            if (Logged != null && (l & LogLevel) == l)
                Logged(l, tag, message, parts);

            return true;
        }

        public static bool Fine(string tag, string message, params object[] parts)
        { return log(LogLevels.Fine, tag, message, parts); }

        public static bool Debug(string tag, string message, params object[] parts)
        { return log(LogLevels.Debug, tag, message, parts); }

        public static bool Info(string tag, string message, params object[] parts)
        { return log(LogLevels.Info, tag, message, parts); }

        public static bool Warn(string tag, string message, params object[] parts)
        { return log(LogLevels.Warning, tag, message, parts); }

        public static bool Severe(string tag, string message, params object[] parts)
        { return log(LogLevels.Severe, tag, message, parts); }

        /// <summary>
        /// Recurses through inner exceptions, printing all stack traces
        /// </summary>
        public static bool FullStackTrace(Exception e)
        {
            var ex = e;
            while (true)
            {
                Log.Severe("Exception", ex.Message + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    continue;
                }
                else break;
            }

            return true;
        }
    }

    /// <summary>
    /// Routes logging and exception handling to the console, using colors for types
    /// of log messages
    /// </summary>
    public class ConsoleLogger
    {
        #region Constructors
        public ConsoleLogger()
        {
            Log.Logged += onLog;
            AppDomain.CurrentDomain.FirstChanceException += onFirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += onUnhandledException;
        }

        ~ConsoleLogger()
        {
            Log.Logged -= onLog;
            AppDomain.CurrentDomain.FirstChanceException -= onFirstChanceException;
            AppDomain.CurrentDomain.UnhandledException -= onUnhandledException;
        }
        #endregion

        #region Exception handling
        /// <summary>
        /// Catches first-chance exceptions for easier display
        /// </summary>
        void onFirstChanceException(object s, FirstChanceExceptionEventArgs e)
        {
            if (Log.LogLevel != (Log.LogLevel & LogLevels.Debug))
                return;

            Console.BackgroundColor = ConsoleColor.DarkRed;
            coloredMessage(
                ConsoleColor.Black,
                "INCOMING EXCEPTION: '{0}' ON THREAD {1}",
                e.Exception.Message, Thread.CurrentThread.Name
            );
            Console.BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Catches unhandled exceptions and pauses execution to give user a chance to
        /// diagnose
        /// </summary>
        void onUnhandledException(object s, UnhandledExceptionEventArgs e)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            coloredMessage(
                ConsoleColor.Black,
                "UNHANDLED EXCEPTION ON THREAD {1}:\n{0}",
                e.ExceptionObject, Thread.CurrentThread.Name
            );
            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        #endregion

        /// <summary>
        /// Prints log messages to console with appropriate color or queues messages
        /// to buffer if paused
        /// </summary>
        void onLog(LogLevels l, string tag, string msg, object[] args)
        {
            msg = "[" + tag + "] " + msg;
            switch (l)
            {
                case LogLevels.Fine:
                    coloredMessage(ConsoleColor.DarkGray, msg, args);
                    return;
                case LogLevels.Debug:
                    coloredMessage(ConsoleColor.Gray, msg, args);
                    return;
                case LogLevels.Info:
                    coloredMessage(ConsoleColor.White, msg, args);
                    return;
                case LogLevels.Warning:
                    coloredMessage(ConsoleColor.Yellow, msg, args);
                    return;
                case LogLevels.Severe:
                    coloredMessage(ConsoleColor.Red, msg, args);
                    return;
            }
        }

        void coloredMessage(ConsoleColor col, string msg, params object[] args)
        {
            Console.ForegroundColor = col;
            Console.WriteLine(msg, args);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
