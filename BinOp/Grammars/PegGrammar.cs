using BinOp.PEG.Rules;

namespace BinOp.PEG
{
    using System;

    /// <summary>
    /// This grammar defines the PEG grammar itself.
    /// </summary>
    public class PegGrammar : Grammar
    {
        public PegGrammar()
        {
            var ascii = Rule.CharSet( "\t\n" ) | Rule.Range( ' ', '~' );
            var escChar = '\\' + Rule.CharSet( "0abfnrtv\\\"\'" );
            var character = Rule.Ast( "char", escChar | ascii );

            // comments
            var lineComment = Rule.Ast( "lineComment", "//" + Rule.After( '\n' ) );
            var blockComment = Rule.Ast( "blockComment", "/*" + Rule.After( "*/" ) );
            var comment = lineComment | blockComment;

            // optional whitespaces
            var s = Rule.ZeroPlus( Rule.CharSet( " \t" ) );

            // any whitespace (spaces, tabs, line breaks)
            var ws = Rule.ZeroPlus( comment | Rule.CharSet( " \t\r\n" ) );

            // end of line
            var eol = (Rule)"\r\n";
            var digit = Rule.Range( '0', '9' );
            var letter = Rule.Range( 'a', 'z' ) | Rule.Range( 'A', 'Z' );

            // literals
            var sq = (Rule)"'";
            var dq = (Rule)'"';
            var number = Rule.Ast( "number", Rule.OnePlus( digit ) + !letter );
            var str = Rule.Ast( "str", Rule.ZeroPlus( !dq + ascii ) );
            var litChar = sq + character + sq;
            var litString = dq + str + dq;
            var literal = number | litString | litChar;

            // identifiers
            var idFirstChar = letter | '_';
            var idNextChar = idFirstChar | digit;
            var identifier = Rule.Ast( "ident", idFirstChar + Rule.ZeroPlus( idNextChar ) );
            //var astNode = Rule.Ast( "ast", Rule.Chr( '^' ) + identifier );

            // basic expressions
            var anyChar = Rule.Ast( "anyChar", '.' );
            var range = Rule.Ast( "range", character + '-' + character );
            var charset = Rule.Ast( "charset", '[' + Rule.OnePlus( Rule.Not( ']' ) + (range | character) ) + ']' );

            // expression
            var subExpr = '(' + s + FRef( "choice" ) + s + ')';
            var primary = comment | subExpr | charset | literal | identifier | anyChar;

            // suffixed expressions
            var opt = Rule.Ast( "opt", primary + '?' );
            var zplus = Rule.Ast( "zpl", primary + '*' );
            var oplus = Rule.Ast( "opl", primary + '+' );
            var suffixExpr = oplus | zplus | opt | primary;

            // prefixed expressions
            var and = Rule.Ast( "and", '&' + suffixExpr );
            var not = Rule.Ast( "not", '!' + suffixExpr );
            var ast = Rule.Ast("ast", '^' + suffixExpr);
            var prefixExpr = ast | and | not | suffixExpr;

            // a production
            var sequence = Rule.Ast( "sequence", prefixExpr + Rule.ZeroPlus( s + prefixExpr ), AstNodeFlags.IgnoreIfSingleChild );
            var choice = Rule.Ast( "choice", sequence + Rule.ZeroPlus( s + '|' + s + sequence ), AstNodeFlags.IgnoreIfSingleChild );

            var expr = choice + s + eol;
            var astExpr = Rule.Ast( "expr", expr );

            var def = Rule.Ast( "def", identifier + s + '=' + s + astExpr );
            var prd = Rule.Ast( "prd", identifier + s + "=>" + s + astExpr );

            // start rule
            Rule = Rule.ZeroPlus( ws + (def|prd) ) + ws;
        }
    }
}


// production using all the grammar
// a = (&(![xyz0-9\n]? a "str" | 'c' '\b')+)*
