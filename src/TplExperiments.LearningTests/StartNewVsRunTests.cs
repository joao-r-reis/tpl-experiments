namespace TplExperiments.LearningTests
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test Task.Factory.StartNew vs Task.Run
    /// </summary>
    [TestClass]
    public class StartNewVsRunTests
    {
        // TODO:
        // https://blogs.msdn.microsoft.com/pfxteam/2011/10/24/task-run-vs-task-factory-startnew/
        [TestMethod]
        public async Task StartNew_AsyncLambda_ReturnedTaskCompletesBeforeLambdaTask()
        {
            // Arrange
            // Act
            var act = Task.Factory.StartNew(() => Task.Delay(5000));
            var finishedTask = await Task.WhenAny(act, Task.Delay(1000));

            // Assert
            Assert.AreSame(act, finishedTask);
            Assert.IsTrue(act.IsCompleted);
        }

        [TestMethod]
        public async Task Run_AsyncLambda_ReturnedTaskCompletesWhenLambdaTaskCompletes()
        {
            // Arrange
            // Act
            var act = Task.Run(() => Task.Delay(1000));
            var finishedTask = await Task.WhenAny(act, Task.Delay(50));

            // Assert
            Assert.AreNotSame(act, finishedTask);
            Assert.IsFalse(act.IsCompleted);
            await Task.Delay(1000);
            Assert.IsTrue(act.IsCompleted);
        }

        [TestMethod]
        public void StartNew_AsyncLambda_ReturnedTaskIsNotSameAsLambdaTask()
        {
            // Arrange
            var delayTask = Task.Delay(5000);

            // Act
            var act = Task.Factory.StartNew(() => delayTask);

            // Assert
            Assert.AreNotSame(act, delayTask);
        }

        [TestMethod]
        public void StartNew_AsyncLambda_InnerTaskOfReturnedTaskIsSameAsLambdaTask()
        {
            // Arrange
            var delayTask = Task.Delay(5000);

            // Act
            var act = Task.Factory.StartNew(() => delayTask);

            // Assert
            Assert.AreSame(act.Result, delayTask);
        }
    }
}