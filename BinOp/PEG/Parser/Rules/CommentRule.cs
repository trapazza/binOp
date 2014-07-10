namespace BinOp.PEG.Rules
{
    using System;

    /// <summary>
    /// This rule does nothing, it's just acts as a placeholder
    /// </summary>
    public class DummyRule : Rule
    {
        public DummyRule( string _description )
        {
            MatchFn = _state => true;
        }
    }
}