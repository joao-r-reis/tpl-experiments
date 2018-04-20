namespace TplExperiments.LearningTests
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test TaskCreationOptions.RunContinuationsAsynchronously flag
    /// </summary>
    [TestClass]
    public class RunContinuationsAsynchronouslyTests
    {
        [TestMethod]
        public async Task Tcs_RunContinuationsAsync_ContinuationsExecutedOnSingleThread()
        {
            var threadIds = new ConcurrentBag<int>();
            var tcs = new TaskCompletionSource<bool>();
            var tl = new List<Task>();
            for (var i = 0L; i < 100000L; i++)
            {
                tl.Add(this.AwaitTask(threadIds, tcs.Task));
            }
            
            await Task.Delay(1000);
            tcs.TrySetResult(true);

            await Task.WhenAll(tl);
            Assert.AreEqual(1, threadIds.Distinct().Count());
        }

        [TestMethod]
        public async Task Tcs_RunContinuationsAsync_ContinuationsExecutedOnMultipleThreads()
        {
            var threadIds = new ConcurrentBag<int>();
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var tl = new List<Task>();
            for (var i = 0L; i < 100000L; i++)
            {
                tl.Add(this.AwaitTask(threadIds, tcs.Task));
            }
            
            await Task.Delay(1000);
            tcs.TrySetResult(true);

            await Task.WhenAll(tl);
            Assert.AreNotEqual(1, threadIds.Distinct().Count());
        }

        private async Task AwaitTask(ConcurrentBag<int> strings, Task t)
        {
            await t;
            strings.Add(System.Environment.CurrentManagedThreadId);
        }

        // TODO:
        // https://blogs.msdn.microsoft.com/pfxteam/2011/10/24/task-run-vs-task-factory-startnew/
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}