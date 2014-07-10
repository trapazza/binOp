namespace BinOp.PEG.Rules
{
    using System;

    /// <summary>
    /// Brk rule
    /// </summary>
    public class DebugBreakRule : Rule
    {
        public DebugBreakRule( Rule _rule ) : base( _rule )
        {
            MatchFn = _state =>
            {
                System.Diagnostics.Debugger.Break();
                return _rule.Apply( _state );
            };
        }
    }

    public class DebugStopRule : Rule
    {
        public DebugStopRule() : base( null )
        {
            MatchFn = _state =>
            {
                System.Diagnostics.Debugger.Break();
                return true;
            };
        }
    }

    /// <summary>
    /// Mutes its child rule
    /// </summary>
    public class MuteRule : Rule
    {
        public MuteRule( Rule _rule ) : base( _rule )
        {
            MatchFn =
                _state =>
                    _state.Mute( _rule );
        }
    }

    public class SetColorRule : Rule
    {
        public SetColorRule( Rule _rule, ConsoleColor _textColor, ConsoleColor _backColor ) : base( null )
        {
            MatchFn = _state =>
            {
                // save current colors
                var oldTextColor = _state.TextColor;
                var oldBackColor = _state.BackColor;

                // set colors
                _state.TextColor = _textColor;
                _state.BackColor = _backColor;

                // test 'colored' rule
                var result = _rule.Apply( _state );

                // restore colors
                _state.TextColor = oldTextColor;
                _state.BackColor = oldBackColor;

                return result;
            };
        }
    }
}