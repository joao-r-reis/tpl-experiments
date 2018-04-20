namespace TplExperiments.LearningTests
{
    using System.Diagnostics;
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
            var output = await await task;
            sw.Stop();
            Assert.AreEqual(5, output);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 4 * MillisecondsDelay);
        }

        private Task<int> AddOneAsync(int t, int millisecondsDelay)
        {
            return Task.Delay(millisecondsDelay).ContinueWith(result => t + 1);
        }
    }
}