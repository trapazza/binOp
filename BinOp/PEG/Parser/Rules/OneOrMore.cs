namespace BinOp.PEG.Rules
{
    public class OneOrMore : Rule
    {
        public OneOrMore( Rule _rule )
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