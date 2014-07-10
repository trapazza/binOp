namespace BinOp.PEG
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Rules;

    /// <summary>
    /// Used by the parser to access the source code input
    /// </summary>
    public class ParseState
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ParseState( string _input )
        {
            Input = _input;
            Position = 0;
            AstTree = new AstNode( "ROOT", Input );

            // default colors will be the same as the default AST node colors
            TextColor = AstTree.TextColor;
            BackColor = AstTree.BackColor;
        }

        /// <summary>
        /// Input stream
        /// </summary>
        public string Input { get; private set; }

        /// <summary>
        /// Current parsing position
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Returns the character at the current position
        /// </summary>
        public char CurrentChar
        {
            get { return Input[ Position ]; }
        }

        /// <summary>
        /// Returns whether the current position is at the end of the stream
        /// </summary>
        public bool IsAtEnd
        {
            get { return Position == Input.Length; }
        }

        /// <summary>
        /// Ast tree
        /// </summary>
        public AstNode AstTree { get; private set; }

        /// <summary>
        /// Matches if all rules are true
        /// </summary>
        public bool All( Rule[] _rules )
        {
            return Parse( () =>
            {
                for( var n = 0; n < _rules.Length; ++n )
                    if( !_rules[ n ].Apply( this ) )
                        return false;
                return true;
            } );
        }

        /// <summary>
        /// Matches if any rule is true
        /// </summary>
        public bool Any( Rule[] _rules )
        {
            return Parse( () =>
            {
                for( var n = 0; n < _rules.Length; n++ )
                    if( _rules[ n ].Apply( this ) )
                        return true;
                return false;
            } );
        }

        /// <summary>
        /// Creates a new branch for the current state then calls the provided match function.
        /// If matching is successful the new branch will become the current one. Other wise the current branch will stay the same.
        /// </summary>
        private bool Parse( Func<bool> _matchFn )
        {
            Branch();

            var match = _matchFn();
            if( match )
                Accept();
            else
                Reject();

            return match;
        }

        /// <summary>
        /// Tests whether the specified rule matches at the current position. No advancement is done.
        /// </summary>
        public bool Test( Rule _rule )
        {
            BeginTestBranch();
            var match = _rule.Apply( this );
            EndTestBranch();
            return match;
        }

        /// <summary>
        /// Tries to match the AST node at the current location
        /// </summary>
        public bool MatchAstNode( Rule _rule, string _name, AstNodeFlags _flags )
        {
            if( IsTesting )
                return _rule.Apply( this );

            if( mMemoTable.Exists( Position, _rule ) )
            {
                var node = mMemoTable.Get( Position, _rule );
                AstTree.AddChild( node );
                Consume( node.Length );
                return true;
            }

            AstTree = new AstNode( _name, Input ) { Parent = AstTree, Position = Position, Flags = _flags };
            var match = _rule.Apply( this );
            if( match )
            {
                AstTree.Length = Position - AstTree.Position;
                AstTree.TextColor = TextColor;
                AstTree.BackColor = BackColor;

                AstTree.Parent.AddChild( AstTree );
                AstTree.Parent.AddContent( AstTree.GetContent() );

                if( !AstTree.IsTerminal )
                {
                    AstTree.ClearContent();
                }

                // memoize
                mMemoTable.Add( AstTree.Position, _rule, AstTree );
            }

            AstTree = AstTree.Parent;

            return match;
        }

        /// <summary>
        /// Ignores writing to the AST node during the execution of the specified function
        /// </summary>
        public bool Mute( Rule _rule )
        {
            mMuted++;
            var result = _rule.Apply( this );
            mMuted--;
            return result;
        }

        /// <summary>
        /// Consumes the specified amount of input characters and updates the read position accordingly
        /// </summary>
        public void Consume( int _amount )
        {
            if( Position + _amount > Input.Length )
                throw new ArgumentOutOfRangeException( "_amount", "Trying to advance past the end of the input stream" );

            if( !IsMuted && !IsTesting )
                AstTree.AddContent( Input, Position, _amount );

            Position += _amount;
        }

        public ConsoleColor TextColor { get; set; }
        public ConsoleColor BackColor { get; set; }

        /// <summary>
        /// Marks the beginning of a test branch
        /// </summary>
        private void BeginTestBranch()
        {
            mTestBranches++;
            Branch();
        }

        /// <summary>
        /// Marks the ending of a test branch
        /// </summary>
        private void EndTestBranch()
        {
            Reject();
            mTestBranches--;
        }

        /// <summary>
        /// Returns whether the current parsing state is running in test mode
        /// </summary>
        private bool IsTesting
        {
            get { return mTestBranches > 0; }
        }

        private bool IsMuted
        {
            get { return mMuted > 0; }
        }

        /// <summary>
        /// Spawns a new parsing branch
        /// </summary>
        private void Branch()
        {
            // save current position and AST
            mBranchStack.Push( new BranchInfo( Position, AstTree ) );

            // create a new root AST
            if( !IsTesting )
                AstTree = new AstNode( "BRANCH", Input );
        }

        /// <summary>
        /// Accepts the current branch
        /// </summary>
        private void Accept()
        {
            var info = mBranchStack.Pop();
            var astNode = info.Node;

            if( !IsTesting )
            {
                // copy children from current AST root to last saved node
                foreach( var child in AstTree.Children )
                    astNode.AddChild( child );

                // append accumulated content
                astNode.AddContent( AstTree.GetContent() );

                // make last saved node the current AST root
                AstTree = astNode;
            }
        }

        /// <summary>
        /// Rejects the current branch
        /// </summary>
        private void Reject()
        {
            if( Position > mFarthestPos )
                mFarthestPos = Position;

            // restore last saved position and root ast
            var info = mBranchStack.Pop();
            Position = info.Position;

            if( !IsTesting )
                AstTree = info.Node;

            if( mBranchStack.Count == 0 )
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write( Input.Substring( 0, mFarthestPos ) );
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine( Input.Substring( mFarthestPos ) );
            }
        }


        /// <summary>
        /// Information stored everytime a branch is created while parsing
        /// </summary>
        private class BranchInfo
        {
            public BranchInfo( int _position, AstNode _node )
            {
                Position = _position;
                Node = _node;
            }

            /// <summary>
            /// The input position at the time the branch was created
            /// </summary>
            public int Position { get; private set; }

            /// <summary>
            /// The AST node at the time the branch was created
            /// </summary>
            public AstNode Node { get; private set; }
        }

        /// <summary>
        /// Saved states
        /// </summary>
        private readonly Stack<BranchInfo> mBranchStack = new Stack<BranchInfo>();

        /// <summary>
        /// Recursive tests count
        /// </summary>
        private int mTestBranches = 0;

        /// <summary>
        /// Recursive mute count
        /// </summary>
        private int mMuted = 0;


        /// <summary>
        /// Memoization table
        /// </summary>
        private readonly MemoTable mMemoTable = new MemoTable();

        /// <summary>
        /// Reverts to the previous parsing branch
        /// </summary>
        private int mFarthestPos = 0;
    }
}
