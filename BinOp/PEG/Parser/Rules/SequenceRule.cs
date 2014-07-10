namespace BinOp.PEG.Rules
{
    using System.Collections.Generic;
    using System.Linq;

    public class SequenceRule : Rule
    {
        public SequenceRule( params Rule[] _rules )
        {
            MatchFn = _state => _state.Parse( () => _rules.All( _rule => _rule.Apply( _state ) ) );
            DescFn = () =>
            {
                string str = null;
                foreach( var rule in _rules )
                {
                    str += str == null ? "" : " + ";
                    str += rule.Desc();
                }
                return str;
            };
        }
    }
}