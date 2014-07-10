namespace BinOp.PEG.Rules.Leaf
{
    using System;

    public class MatchLetterRule : Rule
    {
        public MatchLetterRule()
        {
            MatchFn = _state =>
            {
                var match = !_state.IsAtEnd && Char.IsLetter( _state.CurrentChar );
                if( match )
                    _state.Consume( 1 );
                return match;
            };
        }
    }
}