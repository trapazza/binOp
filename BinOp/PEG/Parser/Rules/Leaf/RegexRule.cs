namespace BinOp.PEG.Rules.Leaf
{
    using System.Text.RegularExpressions;

    public class RegexRule : Rule
    {
        public RegexRule( string _pattern )
        {
            var expr = new Regex( _pattern, RegexOptions.Singleline );

            MatchFn = _state =>
            {
                if( _state.IsAtEnd )
                    return false;
                var result = expr.Match( _state.Input, _state.Position );
                var match = result.Success && result.Index == _state.Position;
                if( match )
                    _state.Consume( result.Length );
                return match;
            };
        }
    }
}
