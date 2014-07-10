namespace BinOp.PEG
{
    using System;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public Parser( string _grammarFile )
        {
            // read input
            var input = File.OpenText( _grammarFile ).ReadToEnd();

            // parse grammar program with the built-in PEG grammar
            var pegGrammar = new PegGrammar();
            var ast = pegGrammar.Parse( input );
            if( ast == null )
                throw new ArgumentOutOfRangeException( "_grammarFile", "The specified grammar is invalid" );
            PrintAst(ast, input);
            
            // analyze the resulting ast and generate the grammar 
            mGrammar = PegAnalyzer.Analyze( ast );
            //TestGrammar( mGrammar, input, 1 );
        }

        /// <summary>
        /// Parses the specified file
        /// </summary>
        public AstNode ParseFile( string _inputFile )
        {
            return Parse( File.OpenText( _inputFile ).ReadToEnd() );
        }

        /// <summary>
        /// Parses the specified input
        /// </summary>
        public AstNode Parse( string _input )
        {
            var file1 = new StreamWriter( File.Create( "file1.txt" ) );
            var file2 = new StreamWriter( File.Create( "file2.txt" ) );
            //TestGrammar(mGrammar, _input, 120);
            var ast = mGrammar.Parse( _input );
            if( ast != null )
            {
                PrintAst(ast, _input);
                PrintAstToFile(file1, ast, _input);
                Console.WriteLine("Everything is alright");

                for( var n = 0; n < 2; ++n )
                {
                    Console.WriteLine("################################################################################################");
                    Console.WriteLine("################################################################################################");
                    Console.WriteLine("################################################################################################");
                    Console.WriteLine("################################################################################################");
                    Console.WriteLine("################################################################################################");
                    if (ast == null)
                        break;
                    var g = PegAnalyzer.Analyze(ast);
                    if (g != null)
                    {
                        ast = g.Parse(_input);
                        if (ast != null)
                        {
                            PrintAst(ast, _input);
                        }
                    }
                }
                if( ast != null )
                    PrintAstToFile(file2, ast, _input);

            }
            else
                Console.WriteLine( "Something is wrong" );

            file1.Close();
            file2.Close();

            return ast;
        }

        /// <summary>
        /// Returns whether the specified input is parseable
        /// </summary>
        public bool IsValid( string _input )
        {
            return mGrammar.Parse( _input ) != null;
        }

        /// <summary>
        /// Tests the specified grammar against itself a specified number of times.
        /// </summary>
        private static void TestGrammar( Grammar _grammar, string _input, int _times )
        {
            for( var n = 0; n < _times; ++n )
            {
                var ast = _grammar.Parse( _input );
                if( ast != null )
                {
                    PrintAst( ast, _input );
                    Console.WriteLine( "################################################################################################" );
                    Console.WriteLine( "################################################################################################" );
                    Console.WriteLine( "################################################################################################" );
                    Console.WriteLine( "################################################################################################" );
                    Console.WriteLine( "################################################################################################" );
                    _grammar = PegAnalyzer.Analyze(ast);
                }
            }
        }

        private static void PrintAst( AstNode _node, string _input, int _tabs = 0 )
        {
            // remove line breaks
            Console.ForegroundColor = _node.TextColor;
            Console.BackgroundColor = _node.BackColor;
            Console.WriteLine(
                //"{0}[{1}] {5} {4} [{2}:{3}]",
                "{0}[{1}] {2}",
                new String( ' ', _tabs ),
                _node.Name,
                //(_node.IsTerminal ? "= " + _node.GetContent() : "  ->  " + _node.GetInput()).Trim( "\r\n".ToCharArray() ) );
                _node.GetContent() );

            foreach( var child in _node.EnumChildren() )
                PrintAst( child, _input, _tabs + 2 );

            if( _tabs == 2 )
                Console.WriteLine();
        }

        private static void PrintAstToFile( StreamWriter _file, AstNode _node, string _input, int _tabs = 0 )
        {
            // remove line breaks
            var text = _input != null ? _input.Substring( _node.Position, _node.Length ).Trim( "\r\n".ToCharArray() ) : "";

            _file.WriteLine(
                            "{0}[{1}] -> {4} [{2}:{3}]",
                new String( ' ', _tabs ),
                _node.Name,
                _node.Position,
                _node.Length,
                text );

            foreach( var child in _node.EnumChildren() )
                PrintAstToFile( _file, child, _input, _tabs + 2 );

            if( _tabs == 2 )
                _file.WriteLine();
        }

        /// <summary>
        /// The grammar used to parse files
        /// </summary>
        private readonly Grammar mGrammar;
    }
}
