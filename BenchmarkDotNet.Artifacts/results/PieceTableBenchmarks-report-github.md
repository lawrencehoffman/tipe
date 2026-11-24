```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 11.0.100
  [Host]     : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4
  Job-CNUJVU : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v4

InvocationCount=1  UnrollFactor=1  

```
| Method               | AddedWordCount | Mean          | Error       | StdDev      | Median        | Allocated |
|--------------------- |--------------- |--------------:|------------:|------------:|--------------:|----------:|
| **AddWords**             | **1**              |      **3.490 μs** |   **0.0719 μs** |   **0.1259 μs** |      **3.485 μs** |      **96 B** |
| LookupLine           | 1              |      2.410 μs |   0.0879 μs |   0.2345 μs |      2.329 μs |     744 B |
| GenerateFullDocument | 1              |      2.330 μs |   0.0493 μs |   0.1093 μs |      2.324 μs |    3736 B |
| **AddWords**             | **10**             |      **9.960 μs** |   **0.2006 μs** |   **0.3461 μs** |      **9.986 μs** |     **768 B** |
| LookupLine           | 10             |      5.130 μs |   0.2679 μs |   0.7901 μs |      5.322 μs |     744 B |
| GenerateFullDocument | 10             |     13.627 μs |   2.1820 μs |   6.1544 μs |     15.973 μs |    4096 B |
| **AddWords**             | **100**            |    **104.826 μs** |   **1.5421 μs** |   **1.3670 μs** |    **104.447 μs** |    **6288 B** |
| LookupLine           | 100            |     15.009 μs |   0.9686 μs |   2.8256 μs |     13.610 μs |     744 B |
| GenerateFullDocument | 100            |     15.329 μs |   2.0963 μs |   5.8781 μs |     17.692 μs |    7696 B |
| **AddWords**             | **1000**           |  **6,960.449 μs** |  **21.8334 μs** |  **19.3547 μs** |  **6,958.615 μs** |   **49440 B** |
| LookupLine           | 1000           |     29.917 μs |   0.5603 μs |   0.4679 μs |     29.960 μs |     744 B |
| GenerateFullDocument | 1000           |     49.982 μs |   3.1250 μs |   8.5547 μs |     52.795 μs |   43696 B |
| **AddWords**             | **10000**          | **30,199.680 μs** | **167.4209 μs** | **148.4143 μs** | **30,140.170 μs** |  **786912 B** |
| LookupLine           | 10000          |     92.233 μs |   1.7662 μs |   1.7347 μs |     91.863 μs |     744 B |
| GenerateFullDocument | 10000          |     94.887 μs |   1.8698 μs |   2.4312 μs |     95.209 μs |  403696 B |
