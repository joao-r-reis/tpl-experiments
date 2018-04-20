namespace TplExperiments
{
    using System;
    using System.Security.Cryptography;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Columns;
    using BenchmarkDotNet.Attributes.Exporters;
    using BenchmarkDotNet.Attributes.Jobs;

    [CoreJob,ClrJob]
    [RPlotExporter, RankColumn]
    public class Md5VsSha256
    {
        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();
        private byte[] data;

        [Params(1000, 10000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            this.data = new byte[this.N];
            new Random(42).NextBytes(this.data);
        }

        [Benchmark]
        public byte[] Sha256() => this.sha256.ComputeHash(this.data);

        [Benchmark]
        public byte[] Md5() => this.md5.ComputeHash(this.data);
    }
}