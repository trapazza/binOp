using System.IO;

namespace BinOp
{
    using System;
    using PEG;

    class Program
    {
        static void Main( string[] args )
        {
            // creates a parser able to parse PEG programs
            var parser = new Parser( @"PegGrammar.txt" );
            
            // parses the peg program which is no other than the peg
            var astTree = parser.ParseFile( @"PegProgram.txt" );

            Console.ReadKey();
        }
    }
}
