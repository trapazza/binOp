namespace BinOp.PEG.Rules.Leaf
{
    using System;

    /// <summary>
    /// Matches if the current input is contained within the specified set of characters
    /// </summary>
    public class MatchCharset : Rule
    {
        public MatchCharset( string _set )
        {
            var set = _set.ToCharArray();

            MatchFn = _state =>
            {
                var match = !_state.IsAtEnd && Array.IndexOf( set, _state.CurrentChar ) != -1;
                if( match )
                    _state.Consume( 1 );
                return match;
            };
        }
    }
}