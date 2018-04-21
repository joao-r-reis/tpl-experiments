namespace TplExperiments.LearningTests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test Unwrap() extension method of Task class
    /// </summary>
    [TestClass]
    public class UnwrapVsResultTests
    {
        [TestMethod]
        public async Task ContinueWith_WithResultInsteadOfUnwrap_BlocksContinueWithChain()
        {
            // Arrange
            const int MillisecondsDelay = 500;

            // Act
            var sw = new Stopwatch();
            sw.Start();
            var task = this.AddOneAsync(1, MillisecondsDelay)
                .ContinueWith(t => this.AddOneAsync(t.Result, MillisecondsDelay))
                .Result
                .ContinueWith(t => this.AddOneAsync(t.Result, MillisecondsDelay))
                .Result
                .ContinueWith(t => this.AddOneAsync(t.Result, MillisecondsDelay));

            // Assert
            sw.Stop();
            var output = await await task;
            Assert.AreEqual(5, output);

            // 2 * millisecondsdelay because of .Result twice in the chain
            Assert.IsTrue(sw.ElapsedMilliseconds >= 2 * MillisecondsDelay, sw.ElapsedMilliseconds.ToString());
        }

        [TestMethod]
        public async Task ContinueWith_WithUnwrapInsteadOfResult_DoesntBlockContinueWithChain()
        {
            // Arrange
            const int MillisecondsDelay = 500;

            // Act
            var sw = new Stopwatch();
            sw.Start();
            var task = this.AddOneAsync(1, MillisecondsDelay)
                .ContinueWith(t => this.AddOneAsync(t.Result, MillisecondsDelay))
                .Unwrap()
                .ContinueWith(t => this.AddOneAsync(t.Result, MillisecondsDelay))
                .Unwrap()
                .ContinueWith(t => this.AddOneAsync(t.Result, MillisecondsDelay));

            // Assert
            sw.Stop();
            var output = await await task;
            Assert.AreEqual(5, output);
            Assert.IsTrue(sw.ElapsedMilliseconds < MillisecondsDelay, sw.ElapsedMilliseconds.ToString());
        }

        private Task<int> AddOneAsync(int t, int millisecondsDelay)
        {
            return Task<int>.Factory.StartNew(
                (obj) =>
                {
                    // Simulate a slow operation
                    Thread.Sleep(millisecondsDelay);
                    return t + 1;
                },
                t);
        }
    }
}