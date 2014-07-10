using System.Collections.Generic;
using System.Configuration;

namespace BinOp.PEG.Rules
{
    using System;
    using Leaf;

    /// <summary>
    /// Base class for a PEG rule.
    /// Overloads some common operators to ease grammar composition from code
    /// </summary>
    public class Rule : Node<Rule>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public Rule( params Rule[] _children )
        {
            if( _children != null )
                foreach( var child in _children )
                    AddChild( child );

            // set a default name
            Name = GetType().Name;
        }

        /// <summary>
        /// Applies the rule to the provided input state
        /// </summary>
        public bool Apply( ParseState _state )
        {
            return MatchFn( _state );
        }

        /// <summary>
        /// Arbitrary name for the rule
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Returns a rule description
        /// </summary>
        public string Desc( bool _recursive = true )
        {
            return DescFn != null ? DescFn( _recursive ) : GetType().Name;
        }

        /// <summary>
        /// Sequence
        /// </summary>
        public static Rule operator +( Rule _left, Rule _right )
        {
            return Sequence( _left, _right );
        }

        /// <summary>
        /// Ordered choice
        /// </summary>
        public static Rule operator |( Rule _left, Rule _right )
        {
            return Choice( _left, _right );
        }

        /// <summary>
        /// Not operator
        /// </summary>
        public static Rule operator !( Rule _rule )
        {
            return Not( _rule );
        }

        /// <summary>
        /// Any char
        /// </summary>
        public static Rule operator ++( Rule _rule )
        {
            return _rule + AnyChar();
        }

        /// <summary>
        /// Chr to rule
        /// </summary>
        public static implicit operator Rule( char _char )
        {
            return new MatchChar( _char );
        }

        /// <summary>
        /// String to rule
        /// </summary>
        public static implicit operator Rule( string _str )
        {
            return new MatchStringRule( _str );
        }


        // special rules
        public static Rule Dummy( string _desc = "" )
        {
            return new DummyRule( "" );
        }

        public static Rule Brk( Rule _rule )
        {
            return new DebugBreakRule( _rule );
        }

        public static Rule Stop()
        {
            return new DebugStopRule();
        }

        public static Rule SetColor( Rule _rule, ConsoleColor _text, ConsoleColor _back )
        {
            return new SetColorRule( _rule, _text, _back );
        }

        public static Rule Mute( Rule _rule )
        {
            return new MuteRule( _rule );
        }

        public static Rule Comment( string _comment )
        {
            return Dummy( _comment );
        }

        public static Rule Sequence( params Rule[] _rules )
        {
            return new MatchSequence( _rules );
        }

        public static Rule Choice( params Rule[] _rules )
        {
            return new MatchChoice( _rules );
        }

        public static Rule ZeroPlus( Rule _rule )
        {
            return new MatchZeroOrMore( _rule );
        }

        public static Rule OnePlus( Rule _rule )
        {
            return new MatchOneOrMore( _rule );
        }

        //public static Rule OnePlus(Rule _rule) { return _rule + ZeroPlus(_rule); }
        public static Rule Opt( Rule _rule )
        {
            return new MatchOptional( _rule );
        }

        public static Rule And( Rule _rule )
        {
            return new AndRule( _rule );
        }

        public static Rule Not( Rule _rule )
        {
            return new NotRule( _rule );
        }

        public static Rule Str( string _str )
        {
            return new MatchStringRule( _str );
        }

        public static Rule Chr( char _c )
        {
            return new MatchChar( _c );
        }

        public static Rule CharSet( string _set )
        {
            return new MatchCharset( _set );
        }

        public static Rule Range( char _left, char _right )
        {
            return new MatchCharRange( _left, _right );
        }

        public static Rule AnyChar()
        {
            return new MatchAnyChar();
        }

        /// <summary>
        /// Skips input until the specified rule is found
        /// </summary>
        public static Rule WhileNot( Rule _stopRule, Rule _rule )
        {
            return ZeroPlus( !_stopRule + _rule );
        }

        /// <summary>
        /// Moves the input after the specified rule
        /// </summary>
        public static Rule After( Rule _stopRule )
        {
            return ZeroPlus( !_stopRule + AnyChar() ) + _stopRule;
        }

        /// <summary>
        /// Whitespaces
        /// </summary>
        public static Rule WhiteSpaces()
        {
            return ZeroPlus( CharSet( " \r\n\t" ) );
        }

        /// <summary>
        /// Digit rule
        /// </summary>
        public static Rule Digit( char _digit )
        {
            if( !char.IsDigit( _digit ) )
                throw new ArgumentOutOfRangeException( "_digit", "The provided char is not a digit" );

            return new MatchDigitRule();
        }

        /// <summary>
        /// Letter rule
        /// </summary>
        public static Rule Letter( char _letter )
        {
            if( !char.IsLetter( _letter ) )
                throw new ArgumentOutOfRangeException( "_letter", "The provided char is not a letter" );

            return new MatchLetterRule();
        }

        /// <summary>
        /// Character-delimited block
        /// </summary>
        public static Rule CharDelimited( char _char )
        {
            //return _char + ZeroPlus( Not( _char ) + AnyChar() ) + _char;
            return BlockDelimited( new string( _char, 2 ), AnyChar() );
        }

        /// <summary>
        /// Matches all occurences of the specified rule within the block delimited by the _bounds parameter
        /// </summary>
        public static Rule BlockDelimited( string _bounds, Rule _rule )
        {
            if( _bounds.Length != 2 )
                throw new ArgumentOutOfRangeException( "_bounds", "The _bounds parameter must be two characters long" );

            return _bounds[ 0 ] + ZeroPlus( Not( _bounds[ 1 ] ) + _rule ) + _bounds[ 1 ];
        }

        /// <summary>
        /// Double quoted string
        /// </summary>
        public static Rule DoubleQuotedString()
        {
            return CharDelimited( '"' );
        }

        /// <summary>
        /// Double quoted string
        /// </summary>
        public static Rule SingleQuotedString()
        {
            return CharDelimited( '\'' );
        }

        /// <summary>
        /// Rule specified by a regex expression
        /// </summary>
        public static Rule Regex( string _pattern )
        {
            return new RegexRule( _pattern );
        }

        /// <summary>
        /// Creates an AST node if the provided rule matches the input
        /// </summary>
        public static AstNodeRule Ast( string _name, Rule _trackedRule, AstNodeFlags _flags = AstNodeFlags.None )
        {
            return new AstNodeRule( _name, _trackedRule, _flags );
        }

        /// <summary>
        /// Used by derived classes to set the rule's matching function
        /// </summary>
        protected Func<ParseState, bool> MatchFn { private get; set; }

        /// <summary>
        /// Rule description
        /// </summary>
        protected Func<bool, string> DescFn { private get; set; }
    }
}