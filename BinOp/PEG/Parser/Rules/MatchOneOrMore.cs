namespace BinOp.PEG.Rules
{
    public class MatchOneOrMore : Rule
    {
        public MatchOneOrMore( Rule _rule ) : base( _rule )
        {
            MatchFn = _state =>
            {
                var match = _rule.Apply( _state );
                if( match )
                    while( _rule.Apply( _state ) )
                        {}
                return match;
            };
        }
    }
}