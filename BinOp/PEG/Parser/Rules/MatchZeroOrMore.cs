namespace BinOp.PEG.Rules
{
    public class MatchZeroOrMore : Rule
    {
        public MatchZeroOrMore( Rule _rule ) : base( _rule )
        {
            MatchFn = _state =>
            {
                while( _rule.Apply( _state ) )
                    {}

                return true;
            };
        }
    }
}