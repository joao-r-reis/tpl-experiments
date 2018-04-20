namespace TplExperiments
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Columns;
    using BenchmarkDotNet.Attributes.Exporters;
    using BenchmarkDotNet.Attributes.Jobs;
    using BenchmarkDotNet.Running;

    [CoreJob, ClrJob]
    [RPlotExporter, RankColumn]
    public class TaskRunVsTaskStartNew
    {
        [Benchmark]
        public async Task TaskRunAsync() => await Task.Run(() => Task.Delay(1));

        [Benchmark]
        public void TaskRunWait() => Task.Run(() => Task.Delay(1)).Wait();

        [Benchmark]
        public void TaskRunGetAwaiter() => Task.Run(() => Task.Delay(1)).GetAwaiter().GetResult();

        [Benchmark]
        public async Task TaskRunAsyncConfigureAwaitFalse() => await Task.Run(() => Task.Delay(1)).ConfigureAwait(false);
    }
}