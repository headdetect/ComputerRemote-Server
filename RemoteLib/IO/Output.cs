using System;

namespace RemoteLib.IO {

    /// <summary>
    /// A raw output stream of raw objects (Usually strings). Mainly used for debugging. 
    /// </summary>
    public class ObjectOutput {

        /// <summary>
        /// Occurs when Write(object) is called.
        /// </summary>
        public static event EventHandler<MessageEventArgs> MessageEventStream;

        /// <summary>
        /// Writes the specified object to any stream that is listening.
        /// </summary>
        /// <param name="msg">The object to write.</param>
        public static void Write ( object msg ) {
            if ( MessageEventStream != null ) {
                MessageEventStream( null, new MessageEventArgs( msg ) );
            }
        }

        public class MessageEventArgs : EventArgs {

            /// <summary>
            /// Gets or sets the object to carry in the message.
            /// </summary>
            /// <value>
            /// The object.
            /// </value>
            public object Object { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
            /// </summary>
            /// <param name="obj">The obj.</param>
            public MessageEventArgs ( object obj ) {
                Object = obj;
            }
        }
    }
}
