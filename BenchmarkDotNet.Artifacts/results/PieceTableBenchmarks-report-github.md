```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 11.0.100
  [Host]     : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4
  Job-CNUJVU : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4

InvocationCount=1  UnrollFactor=1  

```
| Method            | DocumentSize | Mean       | Error     | StdDev    | Median     | Allocated |
|------------------ |------------- |-----------:|----------:|----------:|-----------:|----------:|
| **LoadDocument**      | **2000000**      | **1,955.4 ns** | **103.77 ns** | **304.34 ns** | **1,922.0 ns** |     **200 B** |
| AddAtBeginning    | 2000000      | 2,466.4 ns |  80.89 ns | 233.37 ns | 2,474.0 ns |      56 B |
| AddAtMiddle       | 2000000      | 2,500.2 ns |  52.07 ns |  73.00 ns | 2,484.0 ns |      56 B |
| AddAtEnd          | 2000000      | 2,496.3 ns |  75.68 ns | 221.97 ns | 2,468.5 ns |      56 B |
| DeleteAtBeginning | 2000000      |   770.8 ns |  27.83 ns |  78.94 ns |   751.0 ns |         - |
| DeleteAtMiddle    | 2000000      |   933.3 ns |  29.44 ns |  84.47 ns |   896.5 ns |         - |
| DeleteAtEnd       | 2000000      |   788.6 ns |  28.95 ns |  77.78 ns |   771.0 ns |         - |
| **LoadDocument**      | **10000000**     | **2,102.5 ns** |  **82.54 ns** | **243.38 ns** | **2,113.0 ns** |     **200 B** |
| AddAtBeginning    | 10000000     | 2,305.9 ns |  93.21 ns | 271.90 ns | 2,259.0 ns |      56 B |
| AddAtMiddle       | 10000000     | 2,646.6 ns |  72.43 ns | 211.29 ns | 2,594.5 ns |      56 B |
| AddAtEnd          | 10000000     | 2,369.3 ns |  76.47 ns | 221.86 ns | 2,313.5 ns |      56 B |
| DeleteAtBeginning | 10000000     |   806.0 ns |  50.02 ns | 145.12 ns |   771.0 ns |         - |
| DeleteAtMiddle    | 10000000     |   858.4 ns |  27.12 ns |  77.80 ns |   851.0 ns |         - |
| DeleteAtEnd       | 10000000     |   780.2 ns |  33.27 ns |  93.28 ns |   741.0 ns |         - |
| **LoadDocument**      | **100000000**    | **1,910.2 ns** |  **83.16 ns** | **234.57 ns** | **1,858.0 ns** |     **200 B** |
| AddAtBeginning    | 100000000    | 2,404.1 ns |  85.15 ns | 242.94 ns | 2,419.0 ns |      56 B |
| AddAtMiddle       | 100000000    | 2,607.4 ns |  85.36 ns | 244.90 ns | 2,524.0 ns |      56 B |
| AddAtEnd          | 100000000    | 2,454.7 ns |  81.63 ns | 236.83 ns | 2,413.5 ns |      56 B |
| DeleteAtBeginning | 100000000    |   684.4 ns |  25.14 ns |  70.50 ns |   666.0 ns |         - |
| DeleteAtMiddle    | 100000000    |   966.2 ns |  30.04 ns |  86.18 ns |   950.0 ns |         - |
| DeleteAtEnd       | 100000000    |   740.9 ns |  21.16 ns |  58.29 ns |   731.0 ns |         - |
