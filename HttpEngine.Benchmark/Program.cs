using BenchmarkDotNet.Running;
using System;

namespace HttpEngine.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var benchmark = new HttpEngineBenchmark();
            while (true)
            {
                benchmark.FullExample();
            }
#else
            BenchmarkRunner.Run<HttpEngineBenchmark>();
#endif
            Console.ReadLine();
        }
    }
}
