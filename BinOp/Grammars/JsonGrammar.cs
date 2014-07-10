using BinOp.PEG.Rules;

namespace BinOp.PEG
{
    using Rules.Leaf;

    /// <summary>
    /// Hardcoded JSon grammar
    /// </summary>
    public class JsonGrammar : Grammar
    {
        public JsonGrammar()
        {
            // whitespaces (includes also tabs and line feeds)
            var ws = Rule.ZeroPlus(Rule.CharSet(" \t\r\n"));

            var digit = new MatchDigitRule();
            var letter = new MatchLetterRule();
            var literal = Rule.Regex("\"[^\"]*\"");

            var idFirstChar = letter;
            var idNextChars = letter | digit | '_';
            var identifier = idFirstChar + Rule.ZeroPlus(idNextChars);

            var obj = FRef( "obj" );
            var list = FRef( "lst" );

            var number = Rule.Ast("number", digit + Rule.ZeroPlus(digit));
            var boolean = Rule.Ast("bool", Rule.Choice("true", "false"));
            var value = boolean | number | obj | list | Rule.Ast("lit", literal);
            var key = Rule.Ast("key", identifier + ws);
            var keyValue = Rule.Ast("keyVal", key + ':' + ws + value);
            
            //obj  = Ast( "obj", '{' + ws + Operator.ZeroPlus( ws + keyValue ) + ws + '}' );
            //list = Ast( "lst", '[' + ws + Operator.ZeroPlus( ws + value ) + ws + ']' );
            Rule = ws + (obj | list); ;
        }
    }
}