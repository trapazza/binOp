namespace BinOp.PEG
{
    using System;
    using Rules;
    using Rules.Leaf;

    /// <summary>
    /// Basic PEG operators
    /// With these rules any grammar can be defined
    /// </summary>
    public static class Operators
    {
        /*
         * Grammar:
         * 
         *   seq = expr + '+' + ZeroOrMore( expr ) 
         
         
         */
        public static Rule Sequence( Rule _left, Rule _right ) { return new SequenceRule( _left, _right ); }
        public static Rule Choice( Rule _left, Rule _right ) { return new ChoiceRule( _left, _right ); }
        public static Rule ZeroOrMore( Rule _rule ) { return new ZeroOrMore( _rule ); }
        public static Rule OneOrMore( Rule _rule ) { return new OneOrMore( _rule ); }
        public static Rule Opt( Rule _rule ) { return new OptionalRule( _rule ); }
        public static Rule IsAt( Rule _rule ) { return new AndRule( _rule ); }
        public static Rule Not( Rule _rule ) { return new NotRule( _rule ); }

        public static Rule DebugBreak( Rule _trackedRule ) { return new DebugBreakRule( _trackedRule ); }
        public static Rule Recursive( Func<Rule> _rule ) { return new RecursiveRule( _rule ); }
        public static Rule Node( string _name, Rule _trackedRule ) { return new AstNodeRule( _name, _trackedRule ); }
    }
}