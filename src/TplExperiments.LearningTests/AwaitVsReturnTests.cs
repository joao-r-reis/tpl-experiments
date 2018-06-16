namespace TplExperiments.LearningTests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AwaitVsReturnTests
    {
        [TestMethod]
        public void MethodAwaitsTask_CallWithoutAwait_DoesntThrowException()
        {
            // Arrange
            // Act
            var task = InvalidDelayAwaitTask();

            // Assert
            Assert.IsTrue(task.IsFaulted);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void MethodReturnsTask_CallWithoutAwait_ThrowsException()
        {
            // Arrange
            // Act
            // Assert
            var task = InvalidDelayReturnTask();
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public async Task MethodAwaitsTask_CallWithAwait_ThrowsException()
        {
            // Arrange
            // Act
            var task = InvalidDelayAwaitTask();

            // Assert
            await task;
        }

        static async Task InvalidDelayAwaitTask()
        {
            await Task.Delay(-5);
        }

        static Task InvalidDelayReturnTask()
        {
            return Task.Delay(-5);
        }
    }
}