namespace BinOp.PEG.Rules.Leaf
{
    using System;

    public class MatchDigitRule : Rule
    {
        public MatchDigitRule()
        {
            MatchFn = _state =>
            {
                var match = !_state.IsAtEnd && Char.IsDigit( _state.CurrentChar );
                if( match )
                    _state.Consume( 1 );
                return match;
            };
        }
    }
}