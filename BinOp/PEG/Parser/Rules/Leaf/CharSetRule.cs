namespace BinOp.PEG.Rules.Leaf
{
    using System;

    public class CharSetRule : Rule
    {
        public CharSetRule( string _set )
        {
            MatchFn = _state =>
            {
                var match = Array.IndexOf( _set.ToCharArray(), _state.Input[ _state.Position ] ) != -1;
                if( match )
                    _state.Consume( 1 );
                return match;
            };
        }

    }
}