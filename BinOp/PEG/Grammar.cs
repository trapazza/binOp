using System.Runtime.Remoting.Messaging;

namespace BinOp.PEG
{
    using System.Collections.Generic;
    using Rules;
    using System.Linq;

    public class Grammar
    {
        /// <summary>
        /// Establishes the root rule for this grammar
        /// </summary>
        public Rule Rule
        {
            get { return mRoot; }

            set
            {
                mRoot = value;

                // 
                ScanRuleTree( mRoot, mRules );

                // fix forward references
                var frefs = mRules.OfType<FRefRule>();
                foreach( var fref in frefs )
                    foreach( var rule in mRules )
                        if( rule.Name == fref.Name && !(rule is FRefRule) )
                            fref.Fix( rule );
            }
        }

        /// <summary>
        /// Returns an enumeration of all production rules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Rule> GetProductions()
        {
            return mProductions.Values;
        }

        /// <summary>
        /// Returns the production with the specified name
        /// </summary>
        public Rule this[ string _name ]
        {
            get { return mProductions[_name]; }
        }

        /// <summary>
        /// Parses the specified input and returns the main AST node
        /// </summary>
        public AstNode Parse( string _input )
        {
            var state = new ParseState( _input );
            var result = Rule.Apply( state ) ? state.AstTree : null;
            return result;
        }

        /// <summary>
        /// Creates a forward reference to a yet undeclared *production* rule
        /// </summary>
        protected FRefRule FRef(string _name)
        {
            return new FRefRule( _name );
        }

        /// <summary>
        /// Finds all the rules within the rule tree
        /// </summary>
        private void ScanRuleTree( Rule _rule, HashSet<Rule> _map )
        {
            if( !_map.Contains( _rule ) )
            {
                _map.Add( _rule );
                
                if( !mProductions.ContainsKey( _rule.Name ) && _rule is AstNodeRule ) 
                    mProductions.Add( _rule.Name, _rule.Child );

                foreach( var child in _rule.Children )
                    ScanRuleTree( child, _map );
            }
        }

        /// <summary>
        /// Root rule
        /// </summary>
        private Rule mRoot;

        /// <summary>
        /// A lookup table for all registered rules (this shouldn't be static, it rather belongs to the Grammar itself)
        /// </summary>
        private readonly HashSet<Rule> mRules = new HashSet<Rule>();

        /// <summary>
        /// Production rules
        /// </summary>
        private readonly Dictionary<string, Rule> mProductions = new Dictionary<string, Rule>();
    }
}