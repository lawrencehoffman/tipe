namespace tipe;

public class PieceTable
{
    private enum PieceType
    {
        Original,
        Added,
        Deleted,
    }

    private readonly record struct Piece
    {
        public PieceType Type { get; init; }
        public int Index { get; init; }
        public string Text { get; init; }
        public Piece(PieceType type, int index, string text)
        {
            Type = type;
            Index = index;
            Text = text;
        }
    }

    private readonly List<Piece> _pieces;

    public PieceTable(string initialText)
    {
        _pieces = new List<Piece>
        {
            new Piece(PieceType.Original, 0, initialText)
        };
    }

    public void AddText(int index, string text)
    {
        _pieces.Add(new Piece(PieceType.Added, index, text));
    }

    public void DeleteText(int index, int length)
    {

    }
}
