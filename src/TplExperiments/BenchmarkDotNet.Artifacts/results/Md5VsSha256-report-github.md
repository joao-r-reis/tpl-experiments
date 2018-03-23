``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2125)
Intel Core i7-4712MQ CPU 2.30GHz (Haswell), 1 CPU, 8 logical cores and 4 physical cores
Frequency=2240909 Hz, Resolution=446.2475 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr    : .NET Framework 4.6.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2117.0
  Core   : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT


```
| Method |  Job | Runtime |     N |       Mean |     Error |    StdDev | Rank |
|------- |----- |-------- |------ |-----------:|----------:|----------:|-----:|
| **Sha256** |  **Clr** |     **Clr** |  **1000** |  **14.413 us** | **0.0686 us** | **0.0608 us** |    **4** |
|    Md5 |  Clr |     Clr |  1000 |   5.134 us | 0.0329 us | 0.0308 us |    2 |
| Sha256 | Core |    Core |  1000 |   7.730 us | 0.0157 us | 0.0147 us |    3 |
|    Md5 | Core |    Core |  1000 |   3.405 us | 0.0086 us | 0.0076 us |    1 |
| **Sha256** |  **Clr** |     **Clr** | **10000** | **139.352 us** | **1.1182 us** | **0.9912 us** |    **8** |
|    Md5 |  Clr |     Clr | 10000 |  31.935 us | 0.0798 us | 0.0747 us |    6 |
| Sha256 | Core |    Core | 10000 |  72.116 us | 0.1719 us | 0.1436 us |    7 |
|    Md5 | Core |    Core | 10000 |  30.187 us | 0.0471 us | 0.0418 us |    5 |
