using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBatchRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                //  execute sql found in target folder
                Console.WriteLine("Executing SQL found in {0}", args[0]);
                SqlRunner.Run(args[0]);
            }
            else if (args.Length == 0)
            {
                //  execute sql found folder local to execution context
                Console.WriteLine("Executing SQL found in {0}", Environment.CurrentDirectory);
                SqlRunner.Run(Environment.CurrentDirectory);
            }
            else
            {
                Console.WriteLine("Expecting only one argument, the path to sql files.");
            }

            Console.WriteLine("Hit any key to continue...");
            Console.ReadLine();
        }
    }
}
