using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<PieceTableBenchmarks>();

[MemoryDiagnoser]
public class PieceTableBenchmarks
{
    private const int EditLength = 1_024;
    private PieceTable _pieceTable = null!;
    private string _document = string.Empty;
    private string _insertText = string.Empty;

    [Params(2_000_000, 10_000_000, 100_000_000)]
    public int DocumentSize { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _document = string.Create(DocumentSize, 'x', static (buffer, value) => buffer.Fill(value));
        _insertText = string.Create(EditLength, 'y', static (buffer, value) => buffer.Fill(value));
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _pieceTable = new PieceTable(_document);
    }

    [Benchmark]
    public PieceTable LoadDocument() => new (_document);

    [Benchmark]
    public void AddAtBeginning() => _pieceTable.AddText(0, _insertText);

    [Benchmark]
    public void AddAtMiddle() => _pieceTable.AddText(DocumentSize / 2, _insertText);

    [Benchmark]
    public void AddAtEnd() => _pieceTable.AddText(DocumentSize, _insertText);

    [Benchmark]
    public void DeleteAtBeginning() => _pieceTable.DeleteText(0, EditLength);

    [Benchmark]
    public void DeleteAtMiddle()
        => _pieceTable.DeleteText((DocumentSize - EditLength) / 2, (DocumentSize + EditLength) / 2);

    [Benchmark]
    public void DeleteAtEnd() => _pieceTable.DeleteText(DocumentSize - EditLength, DocumentSize);
}
