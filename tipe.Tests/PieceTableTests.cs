namespace tipe.Tests;

public class PieceTableTests
{
    [Fact]
    public void InitializationAndGetText_ReturnsOriginalText()
    {
        PieceTable table = new ("hello");

        Assert.Equal("hello", table.GetText(0, table.Length));
        Assert.Equal(5, table.Length);
    }

    [Fact]
    public void AddText_InsertsTextAtTheRequestedOffset()
    {
        PieceTable table = new ("ac");

        table.AddText(0, ">");
        table.AddText(2, "b");
        table.AddText(table.Length, "<");

        Assert.Equal(">abc<", table.GetText(0, table.Length));
        Assert.Equal(5, table.Length);
    }

    [Fact]
    public void DeleteText_DeletesTheHalfOpenRange()
    {
        PieceTable table = new ("abcdef");

        table.DeleteText(2, 4);

        Assert.Equal("abef", table.GetText(0, table.Length));
    }

    [Fact]
    public void DeleteText_CanDeleteAcrossOriginalAndAddedPieces()
    {
        PieceTable table = new ("abc");
        table.AddText(1, "XYZ");

        table.DeleteText(2, 5);

        Assert.Equal("aXc", table.GetText(0, table.Length));
    }

    [Fact]
    public void GetText_ReturnsOnlyTheRequestedRangeAcrossPieces()
    {
        PieceTable table = new ("hello world");
        table.AddText(6, "brave ");

        Assert.Equal("world", table.GetText(12, 17));
        Assert.Equal("brave world", table.GetText(6, table.Length));
    }

    [Fact]
    public void GetLine_ReturnsZeroBasedLinesWithoutLineTerminators()
    {
        PieceTable table = new ("first\r\nsecond\nthird");

        Assert.Equal("first", table.GetLine(0));
        Assert.Equal("second", table.GetLine(1));
        Assert.Equal("third", table.GetLine(2));
    }

    [Fact]
    public void GetLine_ReturnsAnEmptyLineForConsecutiveOrTrailingNewlines()
    {
        PieceTable table = new ("one\n\nthree\n");

        Assert.Equal(string.Empty, table.GetLine(1));
        Assert.Equal(string.Empty, table.GetLine(3));
    }

    [Fact]
    public void Operations_RejectInvalidRanges()
    {
        PieceTable table = new ("abc");

        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetText(2, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.AddText(4, "x"));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.DeleteText(-1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.DeleteText(2, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.DeleteText(0, table.Length + 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.DeleteText(table.Length + 1, table.Length + 1));
        Assert.Throws<ArgumentNullException>(() => table.AddText(0, null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetLine(3));

        Assert.Equal("abc", table.GetText(0, table.Length));
        Assert.Equal(3, table.Length);
    }
}
