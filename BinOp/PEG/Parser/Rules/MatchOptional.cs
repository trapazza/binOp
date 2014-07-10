namespace BinOp.PEG.Rules
{
    public class MatchOptional : Rule
    {
        public MatchOptional( Rule _rule ) : base( _rule )
        {
            MatchFn = _state =>
            {
                _rule.Apply( _state );
                return true;
            };
        }
    }
}