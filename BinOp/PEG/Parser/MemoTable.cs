namespace BinOp.PEG
{
    using System.Collections.Generic;
    using Rules;

    /// <summary>
    /// Memoization table
    /// </summary>
    internal class MemoTable
    {
        /// <summary>
        /// Tests whether there's a memoized node for the specified rule at the specified location
        /// </summary>
        public bool Exists( int _position, Rule _rule )
        {
            var match = false;
            Dictionary<Rule, AstNode> rules;
            if( mMap.TryGetValue( _position, out rules ) )
                match = rules.ContainsKey( _rule );
            return match;
        }

        /// <summary>
        /// Returns the memoized node for the specified rule at the specified location.
        /// The node *must* exists.
        /// </summary>
        public AstNode Get( int _position, Rule _rule )
        {
            return mMap[ _position ][ _rule ];
        }

        /// <summary>
        /// Memoizes the specified node for the specified rule at the specified location
        /// </summary>
        public void Add( int _position, Rule _rule, AstNode _node )
        {
            Dictionary<Rule, AstNode> rules = null;
            if( !mMap.TryGetValue( _position, out rules ) )
            {
                rules = new Dictionary<Rule, AstNode>();
                mMap.Add( _position, rules );
            }
            rules.Add( _rule, _node );
        }
        
        /// <summary>
        /// Memoized items by position and rule
        /// </summary>
        private readonly Dictionary<int, Dictionary<Rule, AstNode>> mMap = new Dictionary<int, Dictionary<Rule, AstNode>>();
    }
}