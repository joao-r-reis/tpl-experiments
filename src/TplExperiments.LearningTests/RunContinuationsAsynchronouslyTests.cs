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

        /*
         * https://referencesource.microsoft.com/#mscorlib/system/threading/Tasks/Task.cs,26835bc3d03bdcbb
        public void Invoke(Task completingTask)
        {
            // Check the current stack guard.  If we're ok to inline,
            // process the task, and reset the guard when we're done.
            var sg = Task.CurrentStackGuard;
            if (sg.TryBeginInliningScope())
            {
                try { InvokeCore(completingTask); }
                finally { sg.EndInliningScope(); }
            }
            // Otherwise, we're too deep on the stack, and
            // we shouldn't run the continuation chain here, so queue a work
            // item to call back here to Invoke asynchronously.
            else InvokeCoreAsync(completingTask);
        }
         */
    }
}