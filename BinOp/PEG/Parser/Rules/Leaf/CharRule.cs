namespace BinOp.PEG.Rules.Leaf
{
    public class CharRule : Rule
    {
        public CharRule( char _char )
        {
            MatchFn = _state =>
            {
                var match = _char == _state.Input[ _state.Position ];
                if( match )
                    _state.Consume( 1 );
                return match;
            };
        }
    }
}

    