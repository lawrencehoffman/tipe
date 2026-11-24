using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using tipe;

BenchmarkRunner.Run<PieceTableBenchmarks>();

internal static class BenchmarkDocument
{
    public const int LineWidth = 90;
    public const int BaseWordCount = 100;

    public static string CreateBaseDocument()
        => CreateWords(BaseWordCount, 0, 0, out _);

    public static IReadOnlyList<string> CreateAdditions(int count, string baseDocument)
    {
        int lineLength = baseDocument[(baseDocument.LastIndexOf('\n') + 1)..].Length;
        List<string> additions = new (count);

        for (int index = 0; index < count; index++)
        {
            string word = $"added{index:D4}";
            string separator = lineLength + word.Length + 1 <= LineWidth ? " " : "\n";
            additions.Add(separator + word);
            lineLength = separator == "\n" ? word.Length : lineLength + word.Length + 1;
        }

        return additions;
    }

    public static PieceTable CreatePreparedTable(string baseDocument, IReadOnlyList<string> additions)
    {
        PieceTable table = new (baseDocument);

        foreach (string addition in additions)
        {
            table.AddText(table.Length, addition);
        }

        return table;
    }

    private static string CreateWords(int count, int start, int initialLineLength, out int lineLength)
    {
        System.Text.StringBuilder document = new ();
        lineLength = initialLineLength;

        for (int index = 0; index < count; index++)
        {
            string word = $"base{start + index:D4}";
            string separator = lineLength == 0 ? string.Empty : lineLength + word.Length + 1 <= LineWidth ? " " : "\n";
            document.Append(separator);
            document.Append(word);
            lineLength = separator == "\n" ? word.Length : lineLength + separator.Length + word.Length;
        }

        return document.ToString();
    }
}

[MemoryDiagnoser]
[SimpleJob]
[InvocationCount(1)]
public class PieceTableBenchmarks
{
    private PieceTableAddMetrics _lastAddMetrics;
    private TimeSpan _locateInsertionPoint;
    private TimeSpan _appendToAddBuffer;
    private TimeSpan _splitAndInsertPieces;
    private int _observedAdds;

    [Benchmark]
    public void AddWords()
    {
        foreach (string addition in _additions)
        {
            _table.AddText(_table.Length, addition);
            PieceTableAddMetrics metrics = _table.LastAddMetrics;
            _locateInsertionPoint += metrics.LocateInsertionPoint;
            _appendToAddBuffer += metrics.AppendToAddBuffer;
            _splitAndInsertPieces += metrics.SplitAndInsertPieces;
            _lastAddMetrics = metrics;
            _observedAdds++;
        }

        if (_observedAdds != AddedWordCount || _table.PieceCount != AddedWordCount + 1)
        {
            throw new InvalidOperationException("AddWords did not start from a fresh document.");
        }
    }

    [Benchmark]
    public string LookupLine() => _table.GetLine(_lineToRead);

    [Benchmark]
    public string GenerateFullDocument() => _table.GetText(0, _table.Length);

    [Params(1, 10, 100, 1_000, 10_000)]
    public int AddedWordCount { get; set; }

    private PieceTable _table = null!;
    private string _baseDocument = string.Empty;
    private IReadOnlyList<string> _additions = null!;
    private int _lineToRead;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _baseDocument = BenchmarkDocument.CreateBaseDocument();
        _additions = BenchmarkDocument.CreateAdditions(AddedWordCount, _baseDocument);
        _lineToRead = AddedWordCount / 10;
    }

    [IterationSetup(Target = nameof(AddWords))]
    public void SetupAdd()
    {
        _table = new PieceTable(_baseDocument);
        _table.InstrumentationEnabled = true;
        _locateInsertionPoint = TimeSpan.Zero;
        _appendToAddBuffer = TimeSpan.Zero;
        _splitAndInsertPieces = TimeSpan.Zero;
        _observedAdds = 0;
    }

    [IterationCleanup(Target = nameof(AddWords))]
    public void ReportAddInstrumentation()
    {
        Console.WriteLine(
            $"AddWords instrumentation ({AddedWordCount} adds): " +
            $"locate={_locateInsertionPoint.TotalNanoseconds:F0} ns, " +
            $"append={_appendToAddBuffer.TotalNanoseconds:F0} ns, " +
            $"split/insert={_splitAndInsertPieces.TotalNanoseconds:F0} ns, " +
            "coalesce=0 ns, line-metadata=0 ns, " +
            $"pieces={_lastAddMetrics.PieceCount}, adds={_observedAdds}");
    }

    [IterationSetup(Target = nameof(LookupLine))]
    public void SetupLookup()
    {
        _table = BenchmarkDocument.CreatePreparedTable(_baseDocument, _additions);
    }

    [IterationSetup(Target = nameof(GenerateFullDocument))]
    public void SetupGenerate()
    {
        _table = BenchmarkDocument.CreatePreparedTable(_baseDocument, _additions);
    }
}
