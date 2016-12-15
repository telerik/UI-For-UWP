namespace Telerik.Core
{
    internal class Message
    {
        private static int counter;

        private MessageDispatchMode dispatchMode;
        private MessageDispatchPhase dispatchPhase;
        private bool handled;
        private bool stopDispatch;
        private int id;
        private object data;

        public Message(int id, object data) : this(id, data, MessageDispatchMode.Bubble)
        {
        }

        public Message(int id, object data, MessageDispatchMode dispatchMode)
        {
            this.id = id;
            this.data = data;
            this.dispatchMode = dispatchMode;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the message is handled (processed) by some receiver.
        /// </summary>
        public bool Handled
        {
            get
            {
                return this.handled;
            }
            set
            {
                this.handled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether message may continue being dispatched or not.
        /// </summary>
        public bool StopDispatch
        {
            get
            {
                return this.stopDispatch;
            }
            set
            {
                this.stopDispatch = value;
            }
        }

        /// <summary>
        /// Gets or sets the current phase of the dispatch process.
        /// </summary>
        public MessageDispatchPhase DispatchPhase
        {
            get
            {
                return this.dispatchPhase;
            }
            internal set
            {
                this.dispatchPhase = value;
            }
        }

        /// <summary>
        /// Gets or sets the mode which determines how this message is dispatched.
        /// </summary>
        public MessageDispatchMode DispatchMode
        {
            get
            {
                return this.dispatchMode;
            }
            internal set
            {
                this.dispatchMode = value;
            }
        }

        /// <summary>
        /// Gets the unique id for this message.
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// Gets or sets the raw data associated with the message.
        /// </summary>
        public object Data
        {
            get
            {
                return this.data;
            }
            internal set
            {
                this.data = value;
            }
        }

        public static int Register()
        {
            return counter++;
        }
    }
}