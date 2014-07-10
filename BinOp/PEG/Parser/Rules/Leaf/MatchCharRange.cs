namespace BinOp.PEG.Rules.Leaf
{
    /// <summary>
    /// Matches if the current input is within the specified range
    /// </summary>
    public class MatchCharRange : Rule
    {
        public MatchCharRange( char _left, char _right )
        {
            MatchFn = _state =>
            {
                var match = !_state.IsAtEnd && _state.CurrentChar >= _left && _state.CurrentChar <= _right;
                if( match )
                    _state.Consume( 1 );
                return match;
            };
        }
    }
}