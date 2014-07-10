namespace BinOp.PEG.Rules
{
    using System;

    public class RecursiveRule : Rule
    {
        public RecursiveRule( Func<Rule> _ruleGetter )
        {
            var rule = default(Rule);
            MatchFn = _state =>
            {
                if( rule == null )
                    rule = _ruleGetter();
                return rule.Apply( _state );
            };
        }
    }
}