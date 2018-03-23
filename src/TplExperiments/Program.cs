namespace TplExperiments
{
    using System;
    using BenchmarkDotNet.Running;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[] {
                typeof(Md5VsSha256),
                typeof(TaskRunVsTaskStartNew)
            });
            switcher.Run(args);
        }
    }
}