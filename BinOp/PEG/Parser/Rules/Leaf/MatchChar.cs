namespace BinOp.PEG.Rules.Leaf
{
    public class MatchChar : Rule
    {
        public MatchChar( char _char )
        {
            MatchFn = _state =>
            {
                var match = !_state.IsAtEnd && _char == _state.CurrentChar;
                if( match )
                    _state.Consume( 1 );
                return match;
            };
        }
    }
}

    