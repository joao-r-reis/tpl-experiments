``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2125)
Intel Core i7-4712MQ CPU 2.30GHz (Haswell), 1 CPU, 8 logical cores and 4 physical cores
Frequency=2240909 Hz, Resolution=446.2475 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr    : .NET Framework 4.6.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2117.0
  Core   : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT


```
|                          Method |  Job | Runtime |     Mean |     Error |    StdDev | Rank |
|-------------------------------- |----- |-------- |---------:|----------:|----------:|-----:|
|                    TaskRunAsync |  Clr |     Clr | 15.61 ms | 0.0142 ms | 0.0126 ms |    1 |
|                     TaskRunWait |  Clr |     Clr | 15.60 ms | 0.0294 ms | 0.0261 ms |    1 |
|               TaskRunGetAwaiter |  Clr |     Clr | 15.61 ms | 0.0150 ms | 0.0133 ms |    1 |
| TaskRunAsyncConfigureAwaitFalse |  Clr |     Clr | 15.61 ms | 0.0132 ms | 0.0117 ms |    1 |
|                    TaskRunAsync | Core |    Core | 15.61 ms | 0.0143 ms | 0.0127 ms |    1 |
|                     TaskRunWait | Core |    Core | 15.61 ms | 0.0155 ms | 0.0137 ms |    1 |
|               TaskRunGetAwaiter | Core |    Core | 15.61 ms | 0.0139 ms | 0.0123 ms |    1 |
| TaskRunAsyncConfigureAwaitFalse | Core |    Core | 15.61 ms | 0.0153 ms | 0.0135 ms |    1 |
