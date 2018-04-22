namespace TplExperiments.LearningTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Shaman.Runtime;

    /// <summary>
    /// Tests with ConfigureAwait()
    /// 
    /// -- ConfigureAwait in Asp NET Core
    /// https://stackoverflow.com/questions/42053135/configureawaitfalse-relevant-in-asp-net-core?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
    /// 
    /// -- Default SyncContext
    /// 
    /// https://www.codeproject.com/Articles/31971/Understanding-SynchronizationContext-Part-I
    /// "At this point, you might try to use SynchronizationContext with any thread. However, you will soon find that your thread does not have
    ///  a SynchronizationContext when using SynchronizationContext.Current, and it always returns null. No big deal you say, and you simply create 
    /// a SynchronizationContext if there isn't one. Simple. But, it does not really work.
    /// 
    /// The default implementation of this class is confusing and useless. It is one of two reasons I decided to write this article."
    /// 
    /// -- SingleThreaded sync contexts
    /// https://github.com/antiufo/Shaman.SingleThreadSynchronizationContext/blob/master/Shaman.SingleThreadSynchronizationContext/Async.SingleThreadSynchronizationContext.cs
    /// https://github.com/aspnet/Hosting/issues/765
    /// 
    /// -- Asp NET Core sync context
    /// https://blog.stephencleary.com/2017/03/aspnetcore-synchronization-context.html
    /// 
    /// -- Four implementations of sync context
    /// https://msdn.microsoft.com/magazine/gg598924.aspx
    /// </summary>
    [TestClass]
    public class SynchronizationContextTests
    {
        [TestMethod]
        public async Task Result_WithContextAndNoConfigureAwait_Deadlocks()
        {
            //Arrange
            var oldContext = SynchronizationContext.Current;
            var syncContext = SingleThreadSynchronizationContext.CreateInNewThread();
            SynchronizationContext.SetSynchronizationContext(syncContext);

            //Act
            var actTask = Task.Factory.StartNew(() =>
                {
                    var delayTask = DelayAsync();

                    var output = delayTask.Result; // deadlock here!!
                }, 
                default(CancellationToken), 
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.FromCurrentSynchronizationContext());

            //Assert
            SynchronizationContext.SetSynchronizationContext(oldContext);
            var finishedTask = await Task.WhenAny(actTask, Task.Delay(5000));
            Assert.AreNotSame(actTask, finishedTask);

        }

        public static async Task<int> DelayAsync()
        {
            await Task.Delay(2000);
            return 0;
        }

        [TestMethod]
        public async Task Result_WithContextAndConfigureAwaitFalse_DoesNotDeadlock()
        {
            //Arrange
            var oldContext = SynchronizationContext.Current;
            var syncContext = SingleThreadSynchronizationContext.CreateInNewThread();
            SynchronizationContext.SetSynchronizationContext(syncContext);

            //Act
            var actTask = Task.Factory.StartNew(() =>
                {
                    var delayTask = DelayAsyncWithConfigureAwaitFalse();

                    var output = delayTask.Result; // no more deadlock here!!
                }, 
                default(CancellationToken), 
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.FromCurrentSynchronizationContext());

            //Assert
            SynchronizationContext.SetSynchronizationContext(oldContext);
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