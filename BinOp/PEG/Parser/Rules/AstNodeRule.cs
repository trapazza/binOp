namespace BinOp.PEG.Rules
{
    public class AstNodeRule : Rule
    {
        public AstNodeRule( string _name, Rule _rule, AstNodeFlags _flags ) : base( _rule )
        {
            Name = _name;
            MatchFn = _state => _state.MatchAstNode( _rule, _name, _flags );
        }
    }
}