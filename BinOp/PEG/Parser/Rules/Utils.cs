namespace BinOp.PEG.Rules
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Common utilities
    /// </summary>
    public static class Utils
    {
        public static string ReadString( string _input, int _position, int _length )
        {
            var sb = new StringBuilder();
            return ReadString( sb, _input, _position, _length );

        }

        public static string ReadString( StringBuilder _sb, string _input, int _position, int _length )
        {
            var idx = _position;
            var end = _position + _length;
            while( idx < end )
            {
                var length = 0;
                _sb.Append( ChrAt( _input, idx, out length ) );
                idx += length;
            }

            return _sb.ToString();
        }

        public static char ChrAt( string _input, int _position )
        {
            var dummy = 0;
            return ChrAt( _input, _position, out dummy );
        }

        /// <summary>
        /// Reads a single character from the specified input.
        /// Translates escaped characters.
        /// </summary>
        public static char ChrAt( string _input, int _position, out int _length )
        {
            _length = 1;

            if( IsEscaped( _input, _position ) )
            {
                _length = 2;
                return EscapedChars[ _input[ _position + 1 ] ];
            }

            return _input[ _position ];
        }

        public static bool IsEscaped( string _input, int _position )
        {
            return _input[ _position ] == '\\' && EscapedChars.ContainsKey( _input[ _position + 1 ] );

        }

        /// <summary>
        /// Supported escaped chars
        /// </summary>
        private static readonly Dictionary<char, char> EscapedChars = new Dictionary<char, char>
        {
            { '0', '\0' },
            { 'a', '\a' },
            { 'b', '\b' },
            { 'f', '\f' },
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' },
            { 'v', '\v' },
            { '\\', '\\' },
            { '\"', '\"' },
            { '\'', '\'' },
        };
    }
}
