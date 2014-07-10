using System;

namespace BinOp.PEG.Rules
{
    /// <summary>
    /// References to the named rule in the specified grammar
    /// </summary>
    public class FRefRule : Rule
    {
        public FRefRule( string _name )
        {
            Name = _name;
            MatchFn = _state =>
            {
                if( mRefRule == null )
                    throw new InvalidOperationException( "The referenced rule has not been assigned" );

                return mRefRule.Apply( _state );
            };
        }

        /// <summary>
        /// Establishes the actual reference
        /// </summary>
        internal void Fix( Rule _rule )
        {
            if( mRefRule == null )
                mRefRule = _rule;
        }

        /// <summary>
        /// Referenced rule
        /// </summary>
        private Rule mRefRule;
    }
}