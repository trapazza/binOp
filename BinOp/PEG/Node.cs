namespace BinOp.PEG
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Generic tree node
    /// </summary>
    public abstract class Node<T> where T : Node<T>
    {
        /// <summary>
        /// Returns the number of children
        /// </summary>
        public int ChildCount
        {
            get { return mChildren.Count; }
        }
        
        /// <summary>
        /// Parent
        /// </summary>
        public T Parent { get; set; }

        /// <summary>
        /// Children
        /// </summary>
        public IReadOnlyList<T> Children
        {
            get { return mChildren; }
        }

        /// <summary>
        /// Descendants
        /// </summary>
        public IEnumerable<T> GetDescendants()
        {
            foreach( var child in Children )
            {
                yield return child;
                foreach( var descendant in child.GetDescendants() )
                    yield return descendant;
            }
        }

        /// <summary>
        /// Adds a new child
        /// </summary>
        public void AddChild( T _child )
        {
            if( _child == null )
                throw new ArgumentNullException( "_child", "Children cannot be null" );
            
            mChildren.Add( _child );
        }

        /// <summary>
        /// Removes the specified child
        /// </summary>
        public void RemoveChild( T _child )
        {
            _child.Parent = null;
            mChildren.Remove( _child );
        }

        /// <summary>
        /// Node children
        /// </summary>
        private readonly List<T> mChildren = new List<T>();
    }
}