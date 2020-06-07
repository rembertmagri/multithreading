namespace Parallel
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var cores = 4;
            var limit = 40000000;
            var useInefficient = false;

            if (args != null && args.Any())
            {
                if (int.TryParse(args[0], out var coresArg)) {
                    cores = coresArg;
                }
                if (args.Length > 1 && int.TryParse(args[1], out var limitArg))
                {
                    limit = limitArg;
                }
                if (args.Length > 2 && bool.TryParse(args[2], out var useInefficientArg))
                {
                    useInefficient = useInefficientArg;
                }
            }

            var msgConfig = $"Configuration: cores: {cores}, limit: {limit}, ";
            msgConfig += useInefficient ? "inefficient" : "efficient";
            msgConfig += " method";
            Console.WriteLine(msgConfig);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            int totalNumberOfPrimeNumberFound;
            if (useInefficient)
            {
                var inefficientPrime = new InefficientPrime();
                totalNumberOfPrimeNumberFound = inefficientPrime.calculateNumberOfPrimes(cores, limit);
            }
            else
            {
                var prime = new Prime();
                totalNumberOfPrimeNumberFound = prime.calculateNumberOfPrimes(cores, limit);
            }

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            var msgResult = $"Found a total of {totalNumberOfPrimeNumberFound} prime numbers in {elapsedTime} using ";
            msgResult += useInefficient ? "inefficient" : "efficient";
            msgResult += " method";
            Console.WriteLine(msgResult);
        }


    }
}
