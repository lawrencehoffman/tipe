```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.68GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 11.0.100
  [Host]     : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4
  Job-CNUJVU : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4

InvocationCount=1  UnrollFactor=1  

```
| Method               | AddedWordCount | Mean         | Error      | StdDev     | Median       | Allocated |
|--------------------- |--------------- |-------------:|-----------:|-----------:|-------------:|----------:|
| **AddWords**             | **1**              |     **2.577 μs** |  **0.0518 μs** |  **0.1191 μs** |     **2.583 μs** |      **96 B** |
| LookupLine           | 1              |     2.871 μs |  0.1670 μs |  0.4924 μs |     2.700 μs |     744 B |
| GenerateFullDocument | 1              |     2.447 μs |  0.0562 μs |  0.1621 μs |     2.439 μs |    3736 B |
| **AddWords**             | **10**             |     **4.652 μs** |  **0.0941 μs** |  **0.2026 μs** |     **4.652 μs** |     **768 B** |
| LookupLine           | 10             |     4.433 μs |  0.4752 μs |  1.3937 μs |     5.077 μs |     744 B |
| GenerateFullDocument | 10             |    12.661 μs |  2.1906 μs |  5.9596 μs |    15.688 μs |    4096 B |
| **AddWords**             | **100**            |    **78.923 μs** |  **0.5455 μs** |  **0.4259 μs** |    **78.912 μs** |    **6288 B** |
| LookupLine           | 100            |    20.063 μs |  0.4796 μs |  1.3604 μs |    19.735 μs |     744 B |
| GenerateFullDocument | 100            |    15.808 μs |  1.9731 μs |  5.3679 μs |    17.855 μs |    7696 B |
| **AddWords**             | **1000**           | **6,701.605 μs** | **35.7703 μs** | **31.7094 μs** | **6,701.409 μs** |   **49440 B** |
| LookupLine           | 1000           |    27.451 μs |  0.6523 μs |  1.8610 μs |    27.526 μs |     744 B |
| GenerateFullDocument | 1000           |    50.671 μs |  3.3040 μs |  8.8191 μs |    53.169 μs |   43696 B |
