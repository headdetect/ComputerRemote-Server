using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Globalization;
using ComputerRemote.CLI.Utils;

namespace CLI.Utils {
    /// <summary>
    /// Logger Utility
    /// </summary>
    public class Logger {

        private static bool _flushMessages;
        private static bool _flushErrorMessages;

        /// <summary>
        /// This event is called when Logger.Log() is invoked
        /// </summary>
        public static event EventHandler<LogEventArgs> OnRecieveLog;

        /// <summary>
        /// This event is called when Logger.LogError() is invoked
        /// </summary>
        public static event EventHandler<ErrorLogEventArgs> OnRecieveErrorLog;

        internal static Thread WorkerThread;
        internal static Queue<LogEventArgs> FlushQueue;

        internal static Thread ErrorWorkerThread;
        internal static Queue<ErrorLogEventArgs> FlushErrorQueue;


        private static DateTime _lastTime;


        public static string CurrentLogFile => _lastTime.ToString( "yyyy-MM-dd" ) + ".log";


        /// <summary>
        /// Initializes Logger object staticly
        /// </summary>
        public static void Init () {

            _flushMessages = true;
            _flushErrorMessages = true;

            FlushQueue = new Queue<LogEventArgs>();
            FlushErrorQueue = new Queue<ErrorLogEventArgs>();
            _memBuffer = new MemoryStream();
            _memWriter = new StreamWriter( _memBuffer );
            FileUtils.CreateDirIfNotExist( FileUtils.LogsPath );
            _lastTime = DateTime.Now;
            FileUtils.CreateFileIfNotExist( FileUtils.LogsPath + CurrentLogFile, "--Remote: Version: " + Assembly.GetExecutingAssembly().GetName().Version + ", OS:" + Environment.OSVersion + ", ARCH:" + ( Environment.Is64BitOperatingSystem ? "x64" : "x86" ) + ", CULTURE: " + CultureInfo.CurrentCulture + Environment.NewLine );

            WorkerThread = new Thread( Flush );
            WorkerThread.Start();
            ErrorWorkerThread = new Thread( FlushErrors );
            ErrorWorkerThread.Start();
            _fileFlusher = new Thread( FlushToFile );
            _fileFlusher.Start();

        }

        /// <summary>
        /// De-Initializes the logger class
        /// </summary>
        public static void DeInit () {
            _flushMessages = false;
            _flushErrorMessages = false;
        }

        /// <summary>
        /// Logs a message, to be grabbed by a log event handler
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="logType">The log type</param>
        public static void Log ( string message, LogType logType = LogType.Normal ) {
            var one = Color.White;
            switch ( logType ) {
                case LogType.Info:
                    one = Color.Blue;
                    break;
                case LogType.Debug:
                    one = Color.Gray;
                    break;
                case LogType.Error:
                    one = Color.Red;
                    break;
                case LogType.Warning:
                    one = Color.Yellow;
                    break;
                case LogType.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
            Log(message, one, Color.Black, logType);
        }

        /// <summary>
        /// Logs a message, to be grabbed by a log event handler
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="textColor">Color of the text</param>
        /// <param name="bgColor">Color of the background</param> 
        /// <param name="logType">The log type</param>
        public static void Log(string message, Color textColor, Color bgColor, LogType logType = LogType.Normal)
        {
            FlushQueue.Enqueue(new LogEventArgs(message, logType, textColor, bgColor));
        }

        /// <summary>
        /// Logs an exception, to be grabbed by a log event handler
        /// </summary>
        /// <param name="e">Exception to be logged</param>
        public static void LogError(Exception e)
        {
            FlushErrorQueue.Enqueue(new ErrorLogEventArgs(e));
        }

        private static MemoryStream _memBuffer;
        private static StreamWriter _memWriter;
        private static readonly object MemLock = new object();
        private static Thread _fileFlusher;

        internal static void FlushToFile()
        {
            _lastTime = DateTime.Now;
            while (_flushErrorMessages || _flushMessages)
            {
                for (var i = 0; (_flushErrorMessages || _flushMessages) && (i < 10 || _memBuffer.Length == 0); i++)
                {
                    Thread.Sleep(1000);
                    _memWriter.Flush();
                }
                lock (MemLock)
                {
                    _memWriter.Flush();
                    if (_lastTime.Day != DateTime.Now.Day)
                    {
                        _lastTime = DateTime.Now;
                        FileUtils.CreateFileIfNotExist(FileUtils.LogsPath + CurrentLogFile, "--Remote: Version: " + Assembly.GetExecutingAssembly().GetName().Version + ", OS:" + Environment.OSVersion + ", ARCH:" + (Environment.Is64BitOperatingSystem ? "x64" : "x86") + ", CULTURE: " + CultureInfo.CurrentCulture + Environment.NewLine);
                    }
                    try
                    {
                        using (var fs = new FileStream(FileUtils.LogsPath + CurrentLogFile, FileMode.Append, FileAccess.Write))
                        {
                            using (var fileWriter = new BinaryWriter(fs))
                            {
                                fileWriter.Write(_memBuffer.ToArray());
                                fileWriter.Flush();
                            }
                        }
                        _memBuffer = new MemoryStream();
                        _memWriter = new StreamWriter(_memBuffer);
                    }
                    catch
                    {
                        Console.WriteLine("Logger failed flushing to file");
                    }
                }
            }
        }

        /// <summary>
        /// Writes the log message to the log file
        /// </summary>
        /// <param name="log">Message to log</param>
        public static void WriteLog(string log)
        {
            lock (MemLock)
            {
                _memWriter.WriteLine(log);
            }
        }

        internal static void Flush()
        {
            while (_flushMessages)
            {
                while (FlushQueue.Count == 0 && _flushMessages) Thread.Sleep(200);

                if (FlushQueue.Count <= 0)
                {
                    continue;
                }

                var arg = FlushQueue.Dequeue();

                if (OnRecieveLog == null)
                {
                    continue;
                }

                if (arg.LogType == LogType.Debug && !Paramaters.DebugEnabled)
                    continue;
                OnRecieveLog(null, arg);
                WriteLog(arg.Message);
            }
        }

        internal static void FlushErrors()
        {
            while (_flushErrorMessages)
            {
                while (FlushErrorQueue.Count == 0 && _flushErrorMessages) Thread.Sleep(200);

                if (FlushErrorQueue.Count <= 0)
                {
                    continue;
                }

                var arg = FlushErrorQueue.Dequeue();
                OnRecieveErrorLog?.Invoke(null, arg);
                WriteLog("-------[Error]-------\n\r " + arg.Message + "\n\r---------------------");
            }
        }
    }

    /// <summary>
    /// Log type for the specified message
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// The normal messages
        /// </summary>
        Normal,

        /// <summary>
        /// Error messages
        /// </summary>
        Error,

        /// <summary>
        /// Debug messages (only appears if the server is in debugging mode)
        /// </summary>
        Debug,

        /// <summary>
        /// Warning messages
        /// </summary>
        Warning,

        /// <summary>
        /// Critical messages
        /// </summary>
        Info,
    }

    /// <summary>
    ///Log event where object holding the event
    ///would get a string (the message)
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// Get or set the message of the log event
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Get or set the type of log
        /// </summary>
        public LogType LogType { get; set; }


        /// <summary>
        /// Get or set the color of the text
        /// </summary>
        public Color TextColor { get; set; }


        /// <summary>
        /// Get or set the color of the background
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Default constructor for creating a log event
        /// </summary>
        /// <param name="log">Message of the log event</param>
        /// <param name="logType">Type of log event</param>
        public LogEventArgs(string log, LogType logType)
        {
            Message = log;
            LogType = logType;
            TextColor = Color.White;
            BackgroundColor = Color.Black;
        }

        /// <summary>
        /// Default constructor for creating a log event
        /// </summary>
        /// <param name="log">Message of the log event</param>
        /// <param name="logType">Type of log event</param>
        /// <param name="textColor">Color of the text</param>
        /// <param name="bgColor">Color of the background</param>
        public LogEventArgs(string log, LogType logType, Color textColor, Color bgColor)
        {
            Message = log;
            LogType = logType;
            TextColor = textColor;
            BackgroundColor = bgColor;
        }
    }


    /// <summary>
    ///Log event where object holding the event
    ///would get a string (the message)
    /// </summary>
    public class ErrorLogEventArgs : LogEventArgs
    {
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogEventArgs"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public ErrorLogEventArgs(Exception exception) : base(exception.Message + "\n" + exception.StackTrace, LogType.Error, Color.Red, Color.White)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception), "exception is null.");

            Exception = exception;
        }
    }
}