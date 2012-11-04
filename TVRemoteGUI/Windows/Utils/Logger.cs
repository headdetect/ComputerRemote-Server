/*
	Copyright (c) 2012 Brayden (headdetect)

	Permission is hereby granted, free of charge, 
	to any person obtaining a copy of this software
	and associated documentation files (the "Software"),
	to deal in the Software without restriction, including
	without limitation the rights to use, copy, modify,
	merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom
	the Software is furnished to do so, subject to the
	following conditions:

	The above copyright notice and this permission notice
	shall be included in all copies or substantial portions
	of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF
	ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
	TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT
	SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
	CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
	CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
	CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
	IN THE SOFTWARE.
*/

using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Globalization;
using ComputerRemote.IO;

namespace TVRemoteGUI.Windows.Utils {
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

        internal static Thread _workerThread;
        internal static Queue<LogEventArgs> _flushQueue;

        internal static Thread _errorWorkerThread;
        internal static Queue<ErrorLogEventArgs> _flushErrorQueue;


        private static DateTime _lastTime;


        public static string CurrentLogFile {
            get { return _lastTime.ToString( "yyyy-MM-dd" ) + ".log"; }
        }


        /// <summary>
        /// Initializes Logger object staticly
        /// </summary>
        public static void Init () {

            _flushMessages = true;
            _flushErrorMessages = true;

            _flushQueue = new Queue<LogEventArgs>();
            _flushErrorQueue = new Queue<ErrorLogEventArgs>();
            mem_buffer = new MemoryStream();
            mem_writer = new StreamWriter( mem_buffer );
            FileUtils.CreateDirIfNotExist( FileUtils.LogsPath );
            _lastTime = DateTime.Now;
            FileUtils.CreateFileIfNotExist( FileUtils.LogsPath + CurrentLogFile, "--Remote: Version: " + Assembly.GetExecutingAssembly().GetName().Version + ", OS:" + Environment.OSVersion + ", ARCH:" + ( Environment.Is64BitOperatingSystem ? "x64" : "x86" ) + ", CULTURE: " + CultureInfo.CurrentCulture + Environment.NewLine );

            _workerThread = new Thread( Flush );
            _workerThread.Start();
            _errorWorkerThread = new Thread( FlushErrors );
            _errorWorkerThread.Start();
            _fileFlusher = new Thread( FlushToFile );
            _fileFlusher.Start();

            ObjectOutput.MessageEventStream += ( sender, args ) => {
                if ( args.Object is Exception ) {
                    LogError( args.Object as Exception );
                }
                else {
                    Log( args.Object.ToString() );
                }


            };

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
            ConsoleColor one = ConsoleColor.White;
            switch ( logType ) {
                case LogType.Info:
                    one = ConsoleColor.Blue;
                    break;
                case LogType.Debug:
                    one = ConsoleColor.Gray;
                    break;
                case LogType.Error:
                    one = ConsoleColor.Red;
                    break;
                case LogType.Warning:
                    one = ConsoleColor.Yellow;
                    break;
            }
            Log( message, one, ConsoleColor.Black, logType );
        }

        /// <summary>
        /// Logs a message, to be grabbed by a log event handler
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="textColor">Color of the text</param>
        /// <param name="bgColor">Color of the background</param> 
        /// <param name="logType">The log type</param>
        public static void Log ( string message, ConsoleColor textColor, ConsoleColor bgColor, LogType logType = LogType.Normal ) {
            _flushQueue.Enqueue( new LogEventArgs( message, logType, textColor, bgColor ) );
        }

        /// <summary>
        /// Logs an exception, to be grabbed by a log event handler
        /// </summary>
        /// <param name="e">Exception to be logged</param>
        public static void LogError ( Exception e ) {
            _flushErrorQueue.Enqueue( new ErrorLogEventArgs( e ) );
        }

        private static MemoryStream mem_buffer;
        private static StreamWriter mem_writer;
        private static object mem_lock=new object();
        private static Thread _fileFlusher;

        internal static void FlushToFile () {
            _lastTime = DateTime.Now;
            while ( _flushErrorMessages || _flushMessages ) {
                for ( int i = 0; ( _flushErrorMessages || _flushMessages ) && ( i < 10 || mem_buffer.Length == 0 ); i++ ) {
                    Thread.Sleep( 1000 );
                    mem_writer.Flush();
                }
                lock ( mem_lock ) {
                    mem_writer.Flush();
                    if ( _lastTime.Day != DateTime.Now.Day ) {
                        _lastTime = DateTime.Now;
                        FileUtils.CreateFileIfNotExist( FileUtils.LogsPath + CurrentLogFile, "--Remote: Version: " + Assembly.GetExecutingAssembly().GetName().Version + ", OS:" + Environment.OSVersion + ", ARCH:" + ( Environment.Is64BitOperatingSystem ? "x64" : "x86" ) + ", CULTURE: " + CultureInfo.CurrentCulture + Environment.NewLine );
                    }
                    try {
                        FileStream fs = new FileStream( FileUtils.LogsPath + CurrentLogFile, FileMode.Append, FileAccess.Write );
                        BinaryWriter file_writer = new BinaryWriter( fs );
                        file_writer.Write( mem_buffer.ToArray() );
                        file_writer.Flush();
                        fs.Flush();
                        fs.Close();
                        mem_buffer = new MemoryStream();
                        mem_writer = new StreamWriter( mem_buffer );
                    }
                    catch { Console.WriteLine( "Logger failed flushing to file" ); }
                }
            }
        }

        /// <summary>
        /// Writes the log message to the log file
        /// </summary>
        /// <param name="log">Message to log</param>
        public static void WriteLog ( string log ) {
            lock ( mem_lock ) {
                mem_writer.WriteLine( log );
            }
        }

        internal static void Flush () {
            while ( _flushMessages ) {
                while ( _flushQueue.Count == 0 && _flushMessages ) Thread.Sleep( 200 );
                if ( _flushQueue.Count > 0 ) {
                    var arg = _flushQueue.Dequeue();
                    if ( OnRecieveLog != null ) {
                        try {
                            if ( arg.LogType == LogType.Debug && !Paramaters.DebugEnabled )
                                continue;
                            OnRecieveLog( null, arg );
                            WriteLog( arg.Message );
                        }
                        catch { }
                    }
                }

            }
        }

        internal static void FlushErrors () {
            while ( _flushErrorMessages ) {
                while ( _flushErrorQueue.Count == 0 && _flushErrorMessages ) Thread.Sleep( 200 );
                if ( _flushErrorQueue.Count > 0 ) {
                    var arg = _flushErrorQueue.Dequeue();
                    if ( OnRecieveErrorLog != null )
                        OnRecieveErrorLog( null, arg );
                    WriteLog( "-------[Error]-------\n\r " + arg.Message + "\n\r---------------------" );
                }

            }
        }

    }

    /// <summary>
    /// Log type for the specified message
    /// </summary>
    public enum LogType {
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
    public class LogEventArgs : EventArgs {

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
        public ConsoleColor TextColor { get; set; }


        /// <summary>
        /// Get or set the color of the background
        /// </summary>
        public ConsoleColor BackgroundColor { get; set; }

        /// <summary>
        /// Default constructor for creating a log event
        /// </summary>
        /// <param name="log">Message of the log event</param>
        /// <param name="logType">Type of log event</param>
        public LogEventArgs ( string log, LogType logType ) {
            Message = log;
            LogType = logType;
            TextColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Default constructor for creating a log event
        /// </summary>
        /// <param name="log">Message of the log event</param>
        /// <param name="logType">Type of log event</param>
        /// <param name="textColor">Color of the text</param>
        /// <param name="bgColor">Color of the background</param>
        public LogEventArgs ( string log, LogType logType, ConsoleColor textColor, ConsoleColor bgColor ) {
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
    public class ErrorLogEventArgs : LogEventArgs {


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
        public ErrorLogEventArgs ( Exception exception )
            : base( exception.Message + "\n" + exception.StackTrace, LogType.Error, ConsoleColor.Red, ConsoleColor.Black ) {
            if ( exception == null )
                throw new ArgumentNullException( "exception", "exception is null." );

            this.Exception = exception;
        }

    }
}