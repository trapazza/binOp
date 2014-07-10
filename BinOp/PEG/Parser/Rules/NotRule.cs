namespace BinOp.PEG.Rules
{
    public class NotRule : Rule
    {
        public NotRule( Rule _rule ) : base( _rule )
        {
            MatchFn = _state => !_state.Test( _rule );
        }
    }
}