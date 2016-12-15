using System;
using System.Collections.Generic;

namespace Telerik.Core
{
    /// <summary>
    /// Base class for nodes that may have children.
    /// </summary>
    public abstract class Element : Node
    {
        internal NodeCollection children;
        internal IElementPresenter presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        protected Element()
        {
            this.children = new NodeCollection(this);
        }

        /// <summary>
        /// Gets the <see cref="IElementPresenter"/> instance where this element is presented.
        /// </summary>
        public override IElementPresenter Presenter
        {
            get
            {
                if (this.presenter != null)
                {
                    return this.presenter;
                }

                if (this.parent != null)
                {
                    return this.parent.Presenter;
                }

                return null;
            }
        }

        /// <summary>
        /// Searches up the parent chain and returns the first parent of type T.
        /// </summary>
        /// <typeparam name="T">Must be a <see cref="Element"/>.</typeparam>
        public T FindAncestor<T>() where T : Element
        {
            Element currParent = this.parent;
            while (currParent != null)
            {
                if (currParent is T)
                {
                    return (T)currParent;
                }

                currParent = currParent.parent;
            }

            return null;
        }

        /// <summary>
        /// Gets a boolean value that determines whether a given element
        /// resides in the element hierarchy of this element.
        /// </summary>
        /// <param name="node">An instance of the <see cref="Node"/>
        /// class which is checked.</param>
        public bool IsAncestorOf(Node node)
        {
            if (node != null)
            {
                Element parent = node.parent;

                while (parent != null)
                {
                    if (parent == this)
                    {
                        return true;
                    }

                    parent = parent.parent;
                }
            }

            return false;
        }

        /// <summary>
        /// Searches down the subtree of elements, using breadth-first approach, and returns the first descendant of type T.
        /// </summary>
        /// <typeparam name="T">Must be a <see cref="Node"/>.</typeparam>
        public T FindDescendant<T>() where T : Node
        {
            return this.FindDescendant(node => node is T) as T;
        }

        /// <summary>
        /// Searches down the subtree of elements, using breadth-first approach, and returns the first descendant of type T.
        /// </summary>
        public Node FindDescendant(Predicate<Node> criteria)
        {
            if (criteria == null)
            {
                return null;
            }

            foreach (var node in this.EnumDescendants(TreeTraversalMode.BreadthFirst))
            {
                if (criteria(node))
                {
                    return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Provides flexible routine for traversing all descendants of this instance.
        /// </summary>
        public IEnumerable<Node> EnumDescendants()
        {
            return this.EnumDescendants(null, TreeTraversalMode.DepthFirst);
        }

        /// <summary>
        /// Provides flexible routine for traversing all descendants of this instance.
        /// </summary>
        /// <param name="traverseMode">The mode used to traverse the subtree.</param>
        public IEnumerable<Node> EnumDescendants(TreeTraversalMode traverseMode)
        {
            return this.EnumDescendants(null, traverseMode);
        }

        /// <summary>
        /// Provides flexible routine for traversing all descendants of this instance that match the provided predicate.
        /// </summary>
        /// <param name="predicate">The predicate that defines the match criteria.</param>
        /// <param name="traverseMode">The mode used to traverse the subtree.</param>
        public IEnumerable<Node> EnumDescendants(Predicate<Node> predicate, TreeTraversalMode traverseMode)
        {
            switch (traverseMode)
            {
                case TreeTraversalMode.BreadthFirst:
                    Queue<Element> children = new Queue<Element>();
                    children.Enqueue(this);

                    while (children.Count > 0)
                    {
                        Element childElement = children.Dequeue();
                        foreach (Node nestedChild in childElement.children)
                        {
                            if (predicate == null)
                            {
                                yield return nestedChild;
                            }
                            else if (predicate(nestedChild))
                            {
                                yield return nestedChild;
                            }

                            Element nestedChildElement = nestedChild as Element;
                            if (nestedChildElement != null)
                            {
                                children.Enqueue(nestedChildElement);
                            }
                        }
                    }

                    break;
                default:
                    foreach (Node child in this.children)
                    {
                        if (predicate == null)
                        {
                            yield return child;
                        }
                        else if (predicate(child))
                        {
                            yield return child;
                        }

                        Element childElement = child as Element;
                        if (childElement != null)
                        {
                            childElement.EnumDescendants(predicate, traverseMode);
                        }
                    }
                    
                    break;
            }
        }

        internal virtual void OnChildInserted(int index, Node child)
        {
            child.Parent = this;

            if (this.nodeState == NodeState.Loading || this.nodeState == NodeState.Loaded)
            {
                child.Load(this.root);
            }
        }

        internal virtual void OnChildRemoved(int index, Node child)
        {
            child.Parent = null;
            child.Unload();
        }

        internal virtual ModifyChildrenResult CanAddChild(Node child)
        {
            return ModifyChildrenResult.Refuse;
        }

        internal virtual ModifyChildrenResult CanRemoveChild(Node child)
        {
            return ModifyChildrenResult.Accept;
        }

        internal override void LoadCore()
        {
            base.LoadCore();

            foreach (Node node in this.children)
            {
                node.Load(this.root);
            }
        }

        internal override void UnloadCore()
        {
            foreach (Node child in this.children)
            {
                child.Unload();
            }

            base.UnloadCore();
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            foreach (Node child in this.children)
            {
                child.Arrange(rect);
            }

            return rect;
        }
    }
}