namespace BinOp.PEG.Rules.Leaf
{
    /// <summary>
    /// Matches if the current char is any char.
    /// (Or more informally 'eat the character at the current position')
    /// </summary>
    public class MatchAnyChar : Rule
    {
        public MatchAnyChar()
        {
            MatchFn = _state =>
            {
                var atEnd = _state.IsAtEnd;
                if( !atEnd )
                    _state.Consume( 1 );
                return !atEnd;
            };
        }
    }
}