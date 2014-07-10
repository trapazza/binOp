namespace BinOp.PEG.Rules
{
    public class ZeroOrMore : Rule
    {
        public ZeroOrMore( Rule _rule )
        {
            MatchFn = _state =>
            {
                while( _rule.Apply( _state ) ) {}
                return true;
            };
        }
    }
}