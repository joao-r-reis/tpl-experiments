namespace TplExperiments.LearningTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests with TaskSchedulers
    /// </summary>
    [TestClass]
    public class TaskSchedulerTests
    {
        [TestMethod]
        public async Task AsyncMethodWithExclusiveScheduler_NoConfigureAwait_BlocksOnResult()
        {
            var ts = new ConcurrentExclusiveSchedulerPair();

            var actTask = Task.Factory.StartNew(() =>
            {
                //Arrange


                //Act

                var delayTask = DelayAsync();

                var output = delayTask.Result; // deadlock here!!

                //Assert
            }, default(CancellationToken), TaskCreationOptions.DenyChildAttach, ts.ExclusiveScheduler);

            var finishedTask = await Task.WhenAny(actTask, Task.Delay(5000));
            Assert.AreNotSame(actTask, finishedTask);
        }

        public static async Task<int> DelayAsync()
        {
            await Task.Delay(2000);
            return 0;
        }

        [TestMethod]
        public async Task AsyncMethodWithExclusiveScheduler_ConfigureAwaitFalse_DoesntBlockOnResult()
        {
            var ts = new ConcurrentExclusiveSchedulerPair();

            var actTask = Task.Factory.StartNew(() =>
            {
                //Arrange


                //Act

                var delayTask = DelayAsyncWithConfigureAwaitFalse();

                var output = delayTask.Result;

                //Assert
            }, default(CancellationToken), TaskCreationOptions.DenyChildAttach, ts.ExclusiveScheduler);

            var finishedTask = await Task.WhenAny(actTask, Task.Delay(5000));
            Assert.AreSame(actTask, finishedTask);
        }

        public static async Task<int> DelayAsyncWithConfigureAwaitFalse()
        {
            await Task.Delay(2000).ConfigureAwait(false);
            return 0;
        }
    }
}