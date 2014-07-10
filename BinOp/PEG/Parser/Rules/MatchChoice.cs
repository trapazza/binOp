namespace BinOp.PEG.Rules
{
    public class MatchChoice : Rule
    {
        public MatchChoice( params Rule[] _rules ) : base( _rules )
        {
            MatchFn = _state => _state.Any( _rules );
        }
    }
}