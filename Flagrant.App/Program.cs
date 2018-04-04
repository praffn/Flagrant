using System;

namespace Flagrant.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config();

            var nargs = new[]
            {
                "--name", "phillip",
                "--verbose=0"
            };
            var nargs2 = new[]
            {
                "-age", "22",
            };
            var flagrant = new Flagrant()
                .Parse(nargs)
                .Parse(nargs2)
                .Bind(config);
            Console.WriteLine(config);
        }
    }
}