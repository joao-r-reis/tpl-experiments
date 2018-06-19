namespace TplExperiments.LearningTests
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class AwaitableFuncTests
    {
        [TestMethod]
        public async Task AwaitFunc()
        {
            //Arrange
            //Act
            var result = await new Func<int>(() => 50);

            //Assert
            Assert.AreEqual(50, result);
        }
    }

    public static class FuncExtensions
    {
        public static IAwaiter<TResult> GetAwaiter<TResult>(this Func<TResult> function)
        {
            return new FuncAwaiter<TResult>(function);
        }

        public struct FuncAwaiter<TResult> : IAwaiter<TResult>
        {
            private readonly Task<TResult> task;

            public FuncAwaiter(Func<TResult> function)
            {
                this.task = new Task<TResult>(function);
                this.task.Start();
            }

            bool IAwaiter<TResult>.IsCompleted => this.task.IsCompleted;

            TResult IAwaiter<TResult>.GetResult()
            {
                return this.task.Result;
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                new Task(continuation).Start();
            }
        }

        public interface IAwaitable<out TResult>
        {
            IAwaiter<TResult> GetAwaiter();
        }

        public interface IAwaiter<out TResult> : INotifyCompletion // or ICriticalNotifyCompletion
        {
            bool IsCompleted { get; }

            TResult GetResult();
        }
    }
}