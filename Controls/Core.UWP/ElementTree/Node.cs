using System.ComponentModel;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a node in a logical tree.
    /// </summary>
    public abstract class Node : PropertyBagObject, INotifyPropertyChanged
    {
        internal static readonly int PropertyChangingMessage = Message.Register();
        internal static readonly int PropertyChangedMessage = Message.Register();

        internal RootElement root;
        internal Element parent;
        internal RadRect layoutSlot;
        internal NodeState nodeState;
        internal bool invalidateScheduled;
        internal bool isArrangeValid;
        internal bool isVisible;

        private bool trackPropertyChanging;
        private bool trackPropertyChanged;
        private int index;
        private int collectionIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        protected Node()
        {
            this.index = -1;
            this.collectionIndex = -1;
            this.isVisible = true;
        }

        /// <summary>
        /// Occurs when a property of this node has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the <see cref="IElementPresenter"/> instance where this node is visualized.
        /// </summary>
        public virtual IElementPresenter Presenter
        {
            get
            {
                if (this.parent == null)
                {
                    return null;
                }

                return this.parent.Presenter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the node is laid-out on the chart scene.
        /// </summary>
        public bool IsArrangeValid
        {
            get
            {
                return this.isArrangeValid;
            }
        }

        /// <summary>
        /// Gets the current state of the node.
        /// </summary>
        public NodeState NodeState
        {
            get
            {
                return this.nodeState;
            }
        }

        /// <summary>
        /// Gets the index of this node in its parent <see cref="Element"/> nodes collection.
        /// </summary>
        public int Index
        {
            get
            {
                return this.index;
            }
            internal set
            {
                this.index = value;
            }
        }

        /// <summary>
        /// Gets the index of this node in its owning typed collection.
        /// </summary>
        public int CollectionIndex
        {
            get
            {
                return this.collectionIndex;
            }
            internal set
            {
                this.collectionIndex = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the logical tree this node is part of is loaded.
        /// </summary>
        /// <remarks>
        /// This actually checks for a valid <see cref="RootElement"/> reference and asks whether the area itself is loaded.
        /// This value may differ from the current <see cref="NodeState"/>.
        /// </remarks>
        public virtual bool IsTreeLoaded
        {
            get
            {
                if (this.root != null)
                {
                    return this.root.IsTreeLoaded;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the rectangle (in physical coordinates) where this node resides.
        /// </summary>
        public RadRect LayoutSlot
        {
            get
            {
                return this.layoutSlot;
            }
        }

        /// <summary>
        /// Gets the <see cref="Element"/> where this node resides.
        /// </summary>
        public Element Parent
        {
            get
            {
                return this.parent;
            }
            internal set
            {
                if (this.parent == value)
                {
                    return;
                }

                Element oldParent = this.parent;
                this.SetParentCore(value);
                this.OnParentChanged(oldParent);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node will go through the OnPropertyChanging routine when a property is about to be changed.
        /// </summary>
        internal bool TrackPropertyChanging
        {
            get
            {
                return this.trackPropertyChanging;
            }
            set
            {
                this.trackPropertyChanging = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node will go through the OnPropertyChanged routine when a property has changed.
        /// </summary>
        internal bool TrackPropertyChanged
        {
            get
            {
                return this.trackPropertyChanged;
            }
            set
            {
                this.trackPropertyChanged = value;
            }
        }

        /// <summary>
        /// Arranges the node within the specified layout slot.
        /// </summary>
        public RadRect Arrange(RadRect rect, bool shouldRoundLayout = true)
        {
            if (!this.IsTreeLoaded)
            {
                return RadRect.Empty;
            }

            RadRect arrangeRect = this.ArrangeOverride(rect);

            // do not allow negative Width and Height
            if (arrangeRect.Width < 0)
            {
                arrangeRect.Width = 0;
            }

            if (arrangeRect.Height < 0)
            {
                arrangeRect.Height = 0;
            }

            if (shouldRoundLayout)
            {
                arrangeRect = RadRect.Round(arrangeRect);
            }

            this.layoutSlot = arrangeRect;

            this.isArrangeValid = true;
            this.invalidateScheduled = false;

            return this.layoutSlot;
        }

        /// <summary>
        /// Delegates an "Invalidate" request to the owning <see cref="IElementPresenter"/> instance (if any).
        /// </summary>
        public void Invalidate()
        {
            if (!this.IsTreeLoaded)
            {
                return;
            }

            this.layoutSlot = RadRect.Invalid;
            this.invalidateScheduled = true;
            this.isArrangeValid = false;

            this.InvalidateCore();
        }

        /// <summary>
        /// Perform the core logic behind the Invalidate routine.
        /// </summary>
        internal virtual void InvalidateCore()
        {
            if (this.root != null)
            {
                this.root.InvalidateNode(this);
            }
        }

        internal void SetPropertySilently(int key, object value)
        {
            bool trackChanging = this.trackPropertyChanging;
            bool trackChanged = this.trackPropertyChanged;

            this.trackPropertyChanging = false;
            this.trackPropertyChanged = false;

            this.SetValue(key, value);

            this.trackPropertyChanging = trackChanging;
            this.trackPropertyChanged = trackChanged;
        }

        /// <summary>
        /// Performs pixel-snapping and corrects floating-point calculations errors.
        /// </summary>
        internal virtual void ApplyLayoutRounding()
        {
        }

        internal void ReceiveMessage(Message message)
        {
            this.ProcessMessage(message);
        }

        internal void Load(RootElement rootElement)
        {
            if (this.nodeState == NodeState.Loading || this.nodeState == NodeState.Loaded)
            {
                return;
            }

            this.nodeState = NodeState.Loading;

            // keep references to the load context and plot area
            this.root = rootElement;

            // allow inheritors to provide their own custom logic
            this.LoadCore();

            this.nodeState = NodeState.Loaded;
        }

        internal void Unload()
        {
            this.nodeState = NodeState.Unloading;

            // clear the references to the load context and root chart area
            this.root = null;

            // allow inheritors to provide their own custom logic
            this.UnloadCore();

            this.nodeState = NodeState.Unloaded;
        }

        internal virtual MessageDispatchMode GetMessageDispatchMode(int messageId)
        {
            return MessageDispatchMode.Bubble;
        }

        internal virtual void LoadCore()
        {
        }

        internal virtual void UnloadCore()
        {
        }

        internal virtual RadRect ArrangeOverride(RadRect rect)
        {
            return rect;
        }

        internal virtual void ProcessMessage(Message message)
        {
        }

        internal virtual void SetParentCore(Element parentElement)
        {
            this.parent = parentElement;
        }

        internal virtual void OnParentChanged(Element oldParent)
        {
        }

        internal virtual void OnPropertyChanging(RadPropertyEventArgs e)
        {
            if (!this.IsTreeLoaded)
            {
                return;
            }

            Message message = new Message(PropertyChangingMessage, e, this.GetMessageDispatchMode(PropertyChangingMessage));
            this.DispatchMessage(message);
        }

        internal virtual void OnPropertyChanged(RadPropertyEventArgs e)
        {
            if (!this.IsTreeLoaded)
            {
                return;
            }

            Message message = new Message(PropertyChangedMessage, e, this.GetMessageDispatchMode(PropertyChangingMessage));
            this.DispatchMessage(message);

            // raise the system PropertyChanged event
            this.RaisePropertyChanged(e.PropertyName, e.Key);
        }

        internal void RaisePropertyChanged(string propertyName, int propKey)
        {
            // raise the system PropertyChanged event
            PropertyChangedEventHandler eh = this.PropertyChanged;
            if (eh == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                propertyName = PropertyKeys.GetNameByKey(this.GetType(), propKey);
            }

            eh(this, new PropertyChangedEventArgs(propertyName));
        }

        internal override bool SetValueCore(int key, object value)
        {
            if (!this.trackPropertyChanging && !this.trackPropertyChanged)
            {
                return base.SetValueCore(key, value);
            }

            object currentValue = this.GetValue(key);
            if (object.Equals(currentValue, value))
            {
                return false;
            }

            RadPropertyEventArgs args = new RadPropertyEventArgs(key, currentValue, value);

            if (this.trackPropertyChanging)
            {
                this.OnPropertyChanging(args);
                if (args.Cancel)
                {
                    return false;
                }
            }

            this.propertyStore.SetEntry(key, value);

            if (this.trackPropertyChanged)
            {
                this.OnPropertyChanged(args);
            }

            return true;
        }

        internal override bool ClearValueCore(int key)
        {
            if (!this.trackPropertyChanging && !this.trackPropertyChanged)
            {
                return base.ClearValueCore(key);
            }

            object currentValue = this.GetValue(key);
            RadPropertyEventArgs args = new RadPropertyEventArgs(key, currentValue, null);

            if (this.trackPropertyChanging)
            {
                this.OnPropertyChanging(args);
                if (args.Cancel)
                {
                    return false;
                }
            }

            this.propertyStore.RemoveEntry(key);

            if (this.trackPropertyChanged)
            {
                this.OnPropertyChanged(args);
            }

            return true;
        }

        /// <summary>
        /// Dispatches the provided message to the logical tree, starting from the Sender as a leaf.
        /// </summary>
        internal void DispatchMessage(Message message)
        {
            if (!this.CanDispatch(message))
            {
                return;
            }

            DispatchToTree(message, this);
        }

        internal virtual void PreviewMessage(Message msg)
        {
            if (this.root != null && this.root != this)
            {
                this.root.PreviewMessage(msg);
            }
        }

        private static void DispatchToTree(Message message, Node leaf)
        {
            if ((message.DispatchMode & MessageDispatchMode.Bubble) == MessageDispatchMode.Bubble)
            {
                message.DispatchPhase = MessageDispatchPhase.Bubble;
                BubbleMessage(message, leaf);
            }

            if (message.StopDispatch)
            {
                return;
            }

            if ((message.DispatchMode & MessageDispatchMode.Tunnel) == MessageDispatchMode.Tunnel)
            {
                message.DispatchPhase = MessageDispatchPhase.Tunnel;
                TunnelMessage(message, leaf);
            }
        }

        private static void BubbleMessage(Message message, Node directTarget)
        {
            message.DispatchPhase = MessageDispatchPhase.Bubble;
            Element parent = directTarget.parent;

            while (parent != null)
            {
                parent.ReceiveMessage(message);
                if (message.StopDispatch)
                {
                    return;
                }

                parent = parent.parent;
            }
        }

        private static void TunnelMessage(Message message, Node directTarget)
        {
            Element element = directTarget as Element;
            if (element == null)
            {
                return;
            }

            // TODO: What tree traversal approach should be used here?
            foreach (var descendant in element.EnumDescendants(TreeTraversalMode.DepthFirst))
            {
                descendant.ReceiveMessage(message);
                if (message.StopDispatch)
                {
                    return;
                }
            }
        }

        private bool CanDispatch(Message message)
        {
            this.PreviewMessage(message);
            return !message.StopDispatch;
        }
    }
}
