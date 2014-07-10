namespace BinOp.PEG
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Rules;

    /// <summary>
    /// Analyzes the AST produced by parsing any PEG grammar and produces the corresponding grammar
    /// </summary>
    internal static class PegAnalyzer
    {
        public static Grammar Analyze( AstNode _rootNode )
        {
            var sw = Stopwatch.StartNew();

            // needs the 'start' node
            var startAst = GetDefinition( _rootNode, "start" );
            if( startAst == null )
                return null;

            var grammar = default(Grammar);
            var cache = new AnalysisState( _rootNode );
            var startRule = AnalyzeNode( startAst, cache );
            if( startRule != null )
                grammar = new Grammar { Rule = startRule };
            else
                Console.WriteLine( "An error ocurred while parsing the AST tree" );

            var time = sw.Elapsed.TotalSeconds;
            Console.WriteLine( "AST analyzed in {0} s", time );

            return grammar;
        }

        /// <summary>
        /// Returns the AST node which defines the production with the specified name
        /// </summary>
        private static AstNode GetDefinition( AstNode _root, string _name )
        {
            var def = _root.Children.FirstOrDefault( _node => (_node.Name == "def" || _node.Name == "prd") && _node[ 0 ] == _name );
            if( def == null )
                Console.WriteLine( "The node '{0}' couldn't be found", _name );

            return def;
        }

        private static Rule CreateAstRule( string _astName, Rule _child )
        {
            return Rule.Ast(
                _astName,
                _child,
                _astName == "choice" || _astName == "sequence"
                    ? AstNodeFlags.IgnoreIfSingleChild
                    : AstNodeFlags.None );
        }

        private static Rule[] AnalyzeChildren( AstNode _parent, AnalysisState _cache )
        {
            var rules = new Rule[_parent.ChildCount];
            for( var n = 0; n < rules.Length; ++n )
                rules[ n ] = AnalyzeNode( _parent.ChildByIdx( n ), _cache );
            return rules;
        }

        /// <summary>
        /// Analyzes/processes the specified node and returns the rule it defines.
        /// </summary>
        private static Rule AnalyzeNode( AstNode _node, AnalysisState _cache )
        {
            switch( _node.Name )
            {
                case "mute":        return Rule.Mute( AnalyzeNode( _node[ 0 ], _cache ) );
                case "def":         return _cache.Put( _node[ "ident" ], () => AnalyzeNode( _node[ "expr" ][ 0 ], _cache ) );
                case "prd":         return _cache.Put( _node[ "ident" ], () => CreateAstRule( _node[ "ident" ], AnalyzeNode( _node[ "expr" ][ 0 ], _cache ) ) );
                case "ident":       return _cache.Get( _node ) ?? AnalyzeNode( GetDefinition( _cache.RootNode, _node ), _cache );
                case "ast":         return CreateAstRule( _node[ 0 ].GetInput(), AnalyzeNode( _node[ 0 ], _cache ) );

                // non-terminals    
                case "choice":      return Rule.Choice( AnalyzeChildren( _node, _cache ) );
                case "sequence":    return Rule.Sequence( AnalyzeChildren( _node, _cache ) );
                case "charset":     return Rule.Choice( AnalyzeChildren( _node, _cache ) );

                case "zpl":         return Rule.ZeroPlus( AnalyzeNode( _node[ 0 ], _cache ) );
                case "opl":         return Rule.OnePlus( AnalyzeNode( _node[ 0 ], _cache ) );
                case "opt":         return Rule.Opt( AnalyzeNode( _node[ 0 ], _cache ) );
                case "not":         return Rule.Not( AnalyzeNode( _node[ 0 ], _cache ) );
                case "and":         return Rule.And( AnalyzeNode( _node[ 0 ], _cache ) );

                // terminals
                case "str":         return Rule.Str( _node.GetInput().Replace( "\"", "" ) );
                case "char":        return Rule.Chr( _node );
                case "chargrp":     return Rule.CharSet( _node );
                case "range":       return Rule.Range( _node[ 0 ], _node[ 1 ] );
                case "anyChar":     return Rule.AnyChar();

                // extended grammar

                // special nodes
                case "stop":    return Rule.Stop();

                case "func":
                {
                    var name = _node[ 0 ].GetInput();
                    var expr = AnalyzeNode( _node[ 1 ], _cache );
                    switch( name )
                    {
                        case "brk":
                            return Rule.Brk( expr );

                        case "color":
                        {
                            return Rule.SetColor( expr,
                                (ConsoleColor)Enum.Parse( typeof( ConsoleColor ), _node[ 2 ][ 0 ] ),
                                (ConsoleColor)Enum.Parse( typeof( ConsoleColor ), _node[ 2 ][ 1 ] ) );
                        }
                    }
                    return null;
                }

                case "command":
                    return Rule.Dummy( "pep" );
                    break;

                    //case "lineComment":
                    //case "blockComment":
                    //    return Rule.Comment( _node );

                default:
                    //return Rule.Dummy( "unsupported node" );
                    //return null;
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}