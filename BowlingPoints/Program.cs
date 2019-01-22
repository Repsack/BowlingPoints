using System;
using System.Collections.Generic;

namespace BowlingPoints
{
    class BowlingProgram
    {
        static void Main(string[] args)
        {
            //As always, main is completely barren,
            //except for the creation of the object that is the entry point for this program.
            BowlingTest BT = new BowlingTest();
            BT.BowlingBogus();

            //BT.ScoreCalculatorTest();
            Console.ReadLine(); //Used to make the program not exit before Console.WriteLines can be read.
        }
    }
}
