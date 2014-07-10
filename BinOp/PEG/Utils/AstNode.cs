namespace BinOp.PEG
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Rules;

    /// <summary>
    /// Abstract syntax tree node produced by the parser
    /// </summary>
    public class AstNode : Node<AstNode>
    {
        public AstNode( string _name, string _input )
        {
            Name = _name;
            mInput = _input;
            TextColor = ConsoleColor.White;
            BackColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Node name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Start position within the input stream
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Length of this node within the input stream, in characters
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Returns the source input used to build this node.
        /// </summary>
        public string GetContent()
        {
            return mContentBuilder.ToString();
        }

        public void ClearContent()
        {
            mContentBuilder.Clear();
        }

        /// <summary>
        /// Adds the specified content to the AST
        /// </summary>
        public void AddContent( string _input, int _position, int _length )
        {
            mContentBuilder.Append( _input, _position, _length );
        }

        public void AddContent( string _input )
        {
            mContentBuilder.Append( _input );
        }

        /// <summary>
        /// Indicates whether this is a terminal node or not
        /// </summary>
        public bool IsTerminal
        {
            get { return ChildCount == 0; }
        }

        /// <summary>
        /// Flags
        /// </summary>
        public AstNodeFlags Flags { private get; set; }

        /// <summary>
        /// Returns the only child
        /// </summary>
        private AstNode Child
        {
            get { return EnumChildren().Single(); }
        }

        /// <summary>
        /// Enumerates this node's direct children
        /// </summary>
        public IEnumerable<AstNode> EnumChildren()
        {
            for( var n = 0; n < Children.Count; ++n )
            {
                yield return ChildByIdx( n );
            }
        }

        /// <summary>
        /// Returns the source input used to produce this node
        /// </summary>
        public string GetInput()
        {
            return Utils.ReadString( mInput, Position, Length );
        }

        /// <summary>
        /// Reads the text corresponding to this node as a character
        /// </summary>
        public char GetChr()
        {
            return Utils.ChrAt( mInput, Position );
        }

        /// <summary>
        /// Access a child node by index
        /// </summary>
        public AstNode ChildByIdx( int _idx )
        {
            var child = Children[ _idx ];
            while( (child.Flags & AstNodeFlags.IgnoreIfSingleChild) != 0 && child.ChildCount == 1 )
                child = child.Child;
            return child;
        }

        /// <summary>
        /// Access a child node by index
        /// </summary>
        public AstNode this[ int _index ]
        {
            get { return ChildByIdx( _index ); }
        }

        /// <summary>
        /// Access a child node by name
        /// </summary>
        public AstNode this[ string _nodeName ]
        {
            get { return EnumChildren().Single( _node => _node.Name == _nodeName ); }
        }

        public static implicit operator string( AstNode _node )
        {
            return _node.GetInput();
        }

        public static implicit operator char( AstNode _node )
        {
            return _node.GetChr();
        }

        public ConsoleColor TextColor { get; set; }
        public ConsoleColor BackColor { get; set; }

        public string STR
        {
            get { return GetInput(); }
        }

        public char CHR
        {
            get { return GetChr(); }
        }

        /// <summary>
        /// The string input with which this node was built
        /// </summary>
        private readonly string mInput;

        /// <summary>
        /// Content builder
        /// </summary>
        private readonly StringBuilder mContentBuilder = new StringBuilder();

    }
}