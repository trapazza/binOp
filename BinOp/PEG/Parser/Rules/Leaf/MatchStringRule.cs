namespace BinOp.PEG.Rules.Leaf
{
    public class MatchStringRule : Rule
    {
        public MatchStringRule( string _strToMatch )
        {
            MatchFn = _state =>
            {
                var match = string.Compare( _strToMatch, 0, _state.Input, _state.Position, _strToMatch.Length ) == 0;
                if( match )
                    _state.Consume( _strToMatch.Length );
                return match;
            };
        }
    }
}