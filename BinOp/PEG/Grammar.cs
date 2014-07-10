using BinOp.PEG.Rules.Leaf;

namespace BinOp.PEG
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
                EnumRules(mRoot, mRules);

                // fix forward references
                var frefs = mRules.OfType<FRefRule>();
                foreach (var fref in frefs)
                    foreach (var rule in mRules)
                        if( rule.Name == fref.Name && !(rule is FRefRule) )
                            fref.Fix( rule );
            }
        }

        /// <summary>
        /// Parses the specified input and returns the main AST node
        /// </summary>
        public AstNode Parse( string _input )
        {
            var state = new ParseState( _input );
            
            var sw = Stopwatch.StartNew();
            var result = Rule.Apply(state) ? state.AstTree : null;
            var time = sw.Elapsed.TotalSeconds;
            
            Console.WriteLine( "Input parsed in {0} s", time );

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
        private void EnumRules(Rule _rule, HashSet<Rule> _map)
        {
            if (!_map.Contains(_rule))
            {
                _map.Add(_rule);
                foreach (var child in _rule.Children)
                    EnumRules(child, _map);
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
    }
}