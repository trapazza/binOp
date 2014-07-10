namespace BinOp.PEG.Rules
{
    public class OptionalRule : Rule
    {
        public OptionalRule( Rule _rule )
        {
            MatchFn = _state =>
            {
                _rule.Apply( _state );
                return true;
            };
        }
    }
}