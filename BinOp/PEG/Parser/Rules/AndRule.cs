namespace BinOp.PEG.Rules
{
    using System;

    /// <summary>
    /// Matches if its match matches without changing the input state state.
    /// (which really means to make sure the input position is where we think it is)
    /// </summary>
    public class AndRule : Rule
    {
        public AndRule( Rule _rule ) : base( _rule )
        {
            MatchFn = _state => _state.Test( _rule );
        }
    }
}