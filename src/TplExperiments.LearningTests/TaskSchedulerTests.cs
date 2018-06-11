namespace TplExperiments.LearningTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests with TaskSchedulers
    /// https://blog.stephencleary.com/2012/08/async-and-scheduled-concurrency.html
    /// </summary>
    [TestClass]
    public class TaskSchedulerTests
    {
        [TestMethod]
        public async Task AsyncMethodWithExclusiveScheduler_NoConfigureAwait_BlocksOnResult()
        {
            //Arrange
            var ts = new ConcurrentExclusiveSchedulerPair();

            //Act
            var actTask = Task.Factory.StartNew(() =>
                {
                    var delayTask = DelayAsync();

                    var output = delayTask.Result; // deadlock here!!

                }, 
                default(CancellationToken), 
                TaskCreationOptions.DenyChildAttach, 
                ts.ExclusiveScheduler);

            //Assert
            var finishedTask = await Task.WhenAny(actTask, Task.Delay(5000));
            Assert.AreNotSame(actTask, finishedTask);
        }

        public static async Task<int> DelayAsync()
        {
            await Task.Delay(1);
            return 0;
        }

        [TestMethod]
        public async Task AsyncMethodWithExclusiveScheduler_ConfigureAwaitFalse_DoesntBlockOnResult()
        {
            //Arrange
            var ts = new ConcurrentExclusiveSchedulerPair();

            //Act
            var actTask = Task.Factory.StartNew(() =>
                {
                    var delayTask = DelayAsyncWithConfigureAwaitFalse();

                    var output = delayTask.Result;
                }, 
                default(CancellationToken), 
                TaskCreationOptions.DenyChildAttach, 
                ts.ExclusiveScheduler);

            //Assert
            var finishedTask = await Task.WhenAny(actTask, Task.Delay(5000));
            Assert.AreSame(actTask, finishedTask);
        }

        public static async Task<int> DelayAsyncWithConfigureAwaitFalse()
        {
            await Task.Delay(1).ConfigureAwait(false);
            return 0;
        }
    }
}