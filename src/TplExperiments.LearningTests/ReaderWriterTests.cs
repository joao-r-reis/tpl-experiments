namespace TplExperiments.LearningTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests with ConcurrentExclusiveScheduler.
    /// It can be used to solve the readers writers problem but only with synchronous tasks!
    /// https://stackoverflow.com/a/48351963
    /// https://gist.github.com/paulcbetts/9515910
    /// https://en.wikipedia.org/wiki/Readers%E2%80%93writers_problem
    /// </summary>
    [TestClass]
    public class ReaderWriterTests
    {
        [TestMethod]
        public async Task SynchronousReadersAndWriters_WritersRunExclusivelyAndReadersRunConcurrently()
        {
            //Arrange
            var rand = new Random();
            var list = new List<(int,int)>();

            void Reader()
            {
                Task.Delay(rand.Next() % 50).Wait();
                var listSize = list.Count;
                Task.Delay(100).Wait();
                Assert.AreEqual(listSize, list.Count);
            }

            void Writer()
            {
                Task.Delay(rand.Next() % 50).Wait();
                var listSize = list.Count;
                Task.Delay(100).Wait();
                list.Add((rand.Next(), rand.Next()));
                Assert.AreEqual(listSize + 1, list.Count);

                if (list.Count > 100000)
                {
                    list.Clear();
                }
            }

            var scheduler = new ConcurrentExclusiveSchedulerPair();

            //Act
            var taskReaders = Enumerable.Range(1, 10)
                .Select(_ => 
                    Task.Factory.StartNew(
                        Reader,
                        default(CancellationToken),
                        TaskCreationOptions.LongRunning,
                        scheduler.ConcurrentScheduler));

            var taskWriters = Enumerable.Range(1, 10)
                .Select(_ => 
                    Task.Factory.StartNew(
                        Writer, 
                        default(CancellationToken), 
                        TaskCreationOptions.LongRunning,
                        scheduler.ExclusiveScheduler));

            //Assert
            await Task.WhenAll(taskReaders.Concat(taskWriters));
        }
        
        [TestMethod]
        public async Task AsynchronousReadersAndSynchronousWriters_ReadersAreInterweavedWithWritersBeforeCompletingButWritersRunExclusively()
        {
            //Arrange
            var rand = new Random();
            var list = new List<(int,int)>();

            async Task Reader()
            {
                await Task.Delay(rand.Next() % 50);
                var listSize = list.Count;
                await Task.Delay(100);
                if (listSize != list.Count)
                {
                    throw new Exception("ding ding");
                }
            }

            void Writer()
            {
                Task.Delay(rand.Next() % 50).Wait();
                var listSize = list.Count;
                Task.Delay(100).Wait();
                list.Add((rand.Next(), rand.Next()));
                Assert.AreEqual(listSize + 1, list.Count);

                if (list.Count > 100000)
                {
                    list.Clear();
                }
            }

            var scheduler = new ConcurrentExclusiveSchedulerPair();

            //Act
            var taskReaders = Enumerable.Range(1, 10000)
                .Select(_ => 
                    Task.Factory.StartNew(
                        Reader,
                        default(CancellationToken),
                        TaskCreationOptions.DenyChildAttach,
                        scheduler.ConcurrentScheduler).Unwrap());

            var taskWriters = Enumerable.Range(1, 20)
                .Select(_ => 
                    Task.Factory.StartNew(
                        Writer, 
                        default(CancellationToken), 
                        TaskCreationOptions.LongRunning,
                        scheduler.ExclusiveScheduler));

            //Assert
            var t = Task.WhenAll(taskReaders.Concat(taskWriters));
            try
            {
                await t;
                Assert.Fail("exception wasn't thrown");
            }
            catch (Exception)
            {
                foreach (var exception in t.Exception.InnerExceptions)
                {
                    Assert.AreEqual("ding ding", exception.Message);
                }
            }
        }
        
        [TestMethod]
        public async Task AsynchronousReadersAndWriters_BothReadersAndWritersAreInterweavedBeforeCompleting()
        {
            //Arrange
            var rand = new Random();
            var list = new List<(int,int)>();

            async Task Reader()
            {
                await Task.Delay(rand.Next() % 50);
                var listSize = list.Count;
                await Task.Delay(100);
                if (listSize != list.Count)
                {
                    throw new Exception("ding ding");
                }
            }

            async Task Writer()
            {
                await Task.Delay(rand.Next() % 50);
                var listSize = list.Count;
                await Task.Delay(100);
                list.Add((rand.Next(), rand.Next()));

                var afterListSize = list.Count;

                if (list.Count > 100000)
                {
                    list.Clear();
                }

                if (listSize + 1 != afterListSize)
                {
                    throw new Exception("ding ding ding");
                }
            }

            var scheduler = new ConcurrentExclusiveSchedulerPair();

            //Act
            var taskReaders = Enumerable.Range(1, 10000)
                .Select(_ => 
                    Task.Factory.StartNew(
                        Reader,
                        default(CancellationToken),
                        TaskCreationOptions.DenyChildAttach,
                        scheduler.ConcurrentScheduler).Unwrap());

            var taskWriters = Enumerable.Range(1, 10000)
                .Select(_ => 
                    Task.Factory.StartNew(
                        Writer, 
                        default(CancellationToken), 
                        TaskCreationOptions.DenyChildAttach,
                        scheduler.ExclusiveScheduler).Unwrap());

            //Assert
            var t = Task.WhenAll(taskReaders.Concat(taskWriters));
            try
            {
                await t;
                Assert.Fail("exception wasn't thrown");
            }
            catch (Exception)
            {
                var firstExceptionCount = t.Exception.InnerExceptions.Count(ex => ex.Message == "ding ding");
                
                var twoExceptionCount = t.Exception.InnerExceptions.Count(ex => ex.Message == "ding ding ding");

                Assert.IsTrue(twoExceptionCount > 0);
                Assert.IsTrue(firstExceptionCount > 0);
                Assert.AreEqual(t.Exception.InnerExceptions.Count, firstExceptionCount + twoExceptionCount);
            }
        }
    }
}