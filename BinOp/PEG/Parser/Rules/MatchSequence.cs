namespace BinOp.PEG.Rules
{
    public class MatchSequence : Rule
    {
        public MatchSequence( params Rule[] _rules ) : base( _rules )
        {
            MatchFn = _state => _state.All( _rules );
        }
    }
}