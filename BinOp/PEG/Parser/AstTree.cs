using System;
using System.Collections.Generic;

namespace BinOp.PEG
{
    /// <summary>
    /// Ast tree
    /// </summary>
    public class AstTree
    {
        public AstTree( string _input )
        {
            Input = _input;
        }

        public string Input { get; set; }

        /// <summary>
        /// Spawns a new parsing branch
        /// </summary>
        private void Branch( int _position )
        {
            // save current position and AST
            mBranchStack.Push(new BranchInfo(_position, CurrentNode));

            // create a new root AST
            if (!IsTesting)
                CurrentNode = new AstNode("BRANCH", Input);
        }

        public void TestBranch(int _position)
        {

        }

        /// <summary>
        /// Accepts the current branch
        /// </summary>
        private void Accept()
        {
            var info = mBranchStack.Pop();
            var parent = info.Node;

            if (!IsTesting)
            {
                // copy children from current AST root to last saved node
                foreach (var child in CurrentNode.Children)
                    parent.AddChild(child);

                // make last saved node the current AST root
                CurrentNode = parent;
            }
        }

        /// <summary>
        /// Rejects the current branch
        /// </summary>
        private int Reject()
        {
            // restore last saved position and root ast
            var info = mBranchStack.Pop();
            var position = info.Position;

            if (!IsTesting)
                CurrentNode = info.Node;

            if (mBranchStack.Count == 0)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(Input.Substring(0, mFarthestPos));
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Input.Substring(mFarthestPos));
            }

            return position;
        }
        /// <summary>
        /// Information stored everytime a branch is created while parsing
        /// </summary>
        private class BranchInfo
        {
            public BranchInfo(int _position, AstNode _node)
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
        /// Returns whether the current parsing state is running in test mode
        /// </summary>
        private bool IsTesting
        {
            get { return mTestBranches > 0; }
        }

        /// <summary>
        /// Ast tree
        /// </summary>
        public AstNode CurrentNode { get; private set; }

        /// <summary>
        /// Saved states
        /// </summary>
        private readonly Stack<BranchInfo> mBranchStack = new Stack<BranchInfo>();

        /// <summary>
        /// Memoization table
        /// </summary>
        private readonly MemoTable mMemoTable = new MemoTable();

        /// <summary>
        /// Reverts to the previous parsing branch
        /// </summary>
        private int mFarthestPos = 0;

        /// <summary>
        /// Recursive tests count
        /// </summary>
        private int mTestBranches = 0;

    }
}