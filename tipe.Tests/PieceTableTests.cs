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

        Assert.Equal(string.Empty, table.GetText(4, 4));
        Assert.Equal("world", table.GetText(12, 17));
        Assert.Equal("brave world", table.GetText(6, table.Length));

        int lengthBeforeRead = table.Length;
        Assert.Equal("hello brave world", table.GetText(0, table.Length));
        Assert.Equal(lengthBeforeRead, table.Length);
        Assert.Equal("hello brave world", table.GetText(0, table.Length));

        table.AddText(6, "X");
        table.DeleteText(5, 8);
        Assert.Equal("hellorave world", table.GetText(0, table.Length));
        Assert.Equal(15, table.Length);
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
    public void GetLine_PreservesALoneTrailingCarriageReturn()
    {
        PieceTable table = new ("text\r");

        Assert.Equal("text\r", table.GetLine(0));
    }

    [Fact]
    public void GetLine_ReflectsInsertedAndDeletedLineContent()
    {
        PieceTable table = new ("first\nthird");

        table.AddText(6, "second\n");
        Assert.Equal("second", table.GetLine(1));

        table.DeleteText(6, 13);
        Assert.Equal("third", table.GetLine(1));
    }

    [Fact]
    public void EmptyEdits_DoNotChangeTheDocument()
    {
        PieceTable table = new ("content");

        table.AddText(3, string.Empty);
        table.DeleteText(3, 3);

        Assert.Equal("content", table.GetText(0, table.Length));
        Assert.Equal(7, table.Length);
    }

    [Fact]
    public void EmptyDocument_SupportsInsertAndDeleteAllTransitions()
    {
        PieceTable table = new (string.Empty);

        Assert.Equal(0, table.Length);
        Assert.Equal(string.Empty, table.GetText(0, 0));
        Assert.Equal(string.Empty, table.GetLine(0));

        table.AddText(0, "text");
        Assert.Equal("text", table.GetText(0, table.Length));

        table.DeleteText(0, table.Length);
        Assert.Equal(0, table.Length);
        Assert.Equal(string.Empty, table.GetText(0, 0));
        Assert.Equal(string.Empty, table.GetLine(0));
    }

    [Fact]
    public void Undo_ReversesMixedEditsInOrderAcrossPieces()
    {
        PieceTable table = new ("a\r\nbc");
        table.AddText(1, "XY\n");
        table.DeleteText(2, 6);

        Assert.True(table.CanUndo);
        Assert.Equal("aXbc", table.GetText(0, table.Length));
        Assert.True(table.Undo());
        Assert.Equal("aXY\n\r\nbc", table.GetText(0, table.Length));
        Assert.Equal(8, table.Length);
        Assert.Equal("aXY", table.GetLine(0));
        Assert.Equal(string.Empty, table.GetLine(1));
        Assert.Equal("bc", table.GetLine(2));
        Assert.True(table.Undo());
        Assert.Equal("a\r\nbc", table.GetText(0, table.Length));
        Assert.Equal(5, table.Length);
        Assert.False(table.CanUndo);
        Assert.False(table.Undo());
    }

    [Fact]
    public void Undo_HandlesEmptyDocumentAndEditsAfterUndo()
    {
        PieceTable table = new (string.Empty);
        table.AddText(0, "abc");
        table.DeleteText(0, table.Length);

        Assert.True(table.Undo());
        Assert.Equal("abc", table.GetText(0, table.Length));
        table.AddText(3, "!");
        Assert.True(table.Undo());
        Assert.Equal("abc", table.GetText(0, table.Length));
        Assert.True(table.Undo());
        Assert.Equal(string.Empty, table.GetText(0, table.Length));
        Assert.Equal(0, table.Length);
        Assert.False(table.CanUndo);
    }

    [Fact]
    public void Undo_IgnoresEmptyAndInvalidEdits()
    {
        PieceTable table = new ("abc");
        table.AddText(1, "X");

        table.AddText(0, string.Empty);
        table.DeleteText(0, 0);
        Assert.Throws<ArgumentOutOfRangeException>(() => table.DeleteText(3, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.AddText(-1, "x"));
        Assert.Throws<ArgumentNullException>(() => table.AddText(0, null!));

        Assert.True(table.Undo());
        Assert.Equal("abc", table.GetText(0, table.Length));
        Assert.False(table.CanUndo);
        Assert.False(table.Undo());
    }

    [Fact]
    public void Undo_MatchesStringHistoryForInterleavedEdits()
    {
        Random random = new (42);
        PieceTable table = new ("start\n😀");
        string expected = "start\n😀";
        Stack<string> history = new ();

        for (int i = 0; i < 500; i++)
        {
            int operation = random.Next(3);
            if (operation == 0)
            {
                int start = random.Next(expected.Length + 1);
                string text = i % 2 == 0 ? "😀" : "\r\n";
                history.Push(expected);
                expected = expected.Insert(start, text);
                table.AddText(start, text);
            }
            else if (operation == 1 && expected.Length > 0)
            {
                int start = random.Next(expected.Length);
                int end = random.Next(start + 1, expected.Length + 1);
                history.Push(expected);
                expected = expected.Remove(start, end - start);
                table.DeleteText(start, end);
            }
            else
            {
                bool canUndo = history.TryPop(out string? previous);
                Assert.Equal(canUndo, table.Undo());
                if (canUndo)
                {
                    expected = previous!;
                }
            }

            Assert.Equal(history.Count > 0, table.CanUndo);
            Assert.Equal(expected.Length, table.Length);
            Assert.Equal(expected, table.GetText(0, table.Length));
        }
    }

    [Fact]
    public void Operations_RejectInvalidRanges()
    {
        PieceTable table = new ("abc");

        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetText(2, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetText(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetText(0, table.Length + 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetText(table.Length + 1, table.Length + 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.AddText(4, "x"));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.DeleteText(-1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.DeleteText(2, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.DeleteText(0, table.Length + 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.DeleteText(table.Length + 1, table.Length + 1));
        Assert.Throws<ArgumentNullException>(() => table.AddText(0, null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetLine(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetLine(3));

        Assert.Equal("abc", table.GetText(0, table.Length));
        Assert.Equal(3, table.Length);
    }
}
