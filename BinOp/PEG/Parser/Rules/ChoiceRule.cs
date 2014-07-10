namespace BinOp.PEG.Rules
{
    using System.Linq;

    public class ChoiceRule : Rule
    {
        public ChoiceRule( params Rule[] _rules )
        {
            MatchFn = _state => _state.Parse( () => _rules.Any( _rule => _rule.Apply( _state ) ) );
            DescFn = () =>
            {
                string str = null;
                foreach( var rule in _rules )
                {
                    str += str == null ? "" : " | ";
                    str += rule.Desc();
                }
                return str;
            };
        }
    }
}