namespace BinOp.PEG
{
    using System;
    using System.Collections.Generic;
    using Rules;

    /// <summary>
    /// State used while analyzing an AST
    /// </summary>
    internal class AnalysisState
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public AnalysisState( AstNode _rootNode )
        {
            RootNode = _rootNode;
        }

        /// <summary>
        /// Root node
        /// </summary>
        public AstNode RootNode { get; private set; }

        /// <summary>
        /// Puts the specified rule in the cache
        /// </summary>
        /// <param name="_name">The rule name</param>
        /// <param name="_func">The function that produces the rule</param>
        public Rule Put( string _name, Func<Rule> _func )
        {
            // mark production as 'being analyzed'
            mUnderConstruction.Push( _name );
            
            // cache production
            var rule = _func();
            mProductions.Add( _name, rule );

            // unmark
            mUnderConstruction.Pop();

            // after all productions has been finished, fix all dangling references
            if( mUnderConstruction.Count == 0 )
                foreach( FRefRule alias in mFRefs.Values )
                    alias.Fix( mProductions[ alias.Name ] );

            return rule;
        }

        /// <summary>
        /// Finds the production rule with the specified name
        /// </summary>
        public Rule Get( string _name )
        {
            // find in cache
            var rule = default(Rule);
            if( mProductions.TryGetValue( _name, out rule ) )
                return rule;

            // find in references
            var fref = default (FRefRule);
            if( mFRefs.TryGetValue( _name, out fref ) )
                return fref;

            // not found. is it under construction?
            if( mUnderConstruction.Contains( _name ) )
            {
                // since referred rule is not yet finished, return a forwarded reference
                fref = new FRefRule( _name );
                mFRefs.Add( _name, fref );
                return fref;
            }

            return null;
        }

        /// <summary>
        /// Contains all rule references found
        /// </summary>
        private readonly Dictionary<string, FRefRule> mFRefs = new Dictionary<string, FRefRule>();

        /// <summary>
        /// Productions currently under construction
        /// </summary>
        private readonly Stack<string> mUnderConstruction = new Stack<string>();

        /// <summary>
        /// Finished productions
        /// </summary>
        private readonly Dictionary<string, Rule> mProductions = new Dictionary<string, Rule>();
    }
}