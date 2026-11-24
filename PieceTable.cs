namespace tipe;

public class PieceTable
{
    private enum BufferType
    {
        Original,
        Added,
    }

    private readonly record struct Piece
    {
        public BufferType Buffer { get; init; }
        public int BufferIndex { get; init; }
        public int Start { get; init; }
        public int Length { get; init; }

        public Piece(BufferType buffer, int bufferIndex, int start, int length)
        {
            Buffer = buffer;
            BufferIndex = bufferIndex;
            Start = start;
            Length = length;
        }
    }

    private readonly string _originalText;
    private readonly List<string> _addedBuffers = new ();
    private readonly List<Piece> _pieces;
    private int _length;

    public PieceTable(string initialText)
    {
        ArgumentNullException.ThrowIfNull(initialText);

        _originalText = initialText;
        _pieces = new List<Piece>
        {
            new Piece(BufferType.Original, 0, 0, initialText.Length)
        };
        _length = initialText.Length;
    }

    /// <summary>
    /// Gets the current document length in UTF-16 character offsets.
    /// </summary>
    public int Length => _length;

    /// <summary>
    /// Inserts <paramref name="text"/> at <paramref name="start"/>.
    /// Offsets are UTF-16 character offsets and the insertion may occur at <see cref="Length"/>.
    /// </summary>
    public void AddText(int start, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        ValidateOffset(start, nameof(start), allowEnd: true);

        if (text.Length == 0)
        {
            return;
        }

        SplitAt(start);

        int addedBufferIndex = _addedBuffers.Count;
        _addedBuffers.Add(text);
        int insertionIndex = GetBoundaryIndex(start);
        _pieces.Insert(insertionIndex, new Piece(BufferType.Added, addedBufferIndex, 0, text.Length));
        _length += text.Length;
    }

    /// <summary>
    /// Deletes the half-open range [<paramref name="start"/>, <paramref name="end"/>).
    /// </summary>
    public void DeleteText(int start, int end)
    {
        ValidateOffset(start, nameof(start), allowEnd: true);
        ValidateOffset(end, nameof(end), allowEnd: true);

        if (end < start)
        {
            throw new ArgumentOutOfRangeException(nameof(end), "The end offset must not be less than the start offset.");
        }

        if (start == end)
        {
            return;
        }

        SplitAt(start);
        SplitAt(end);

        int firstPiece = GetBoundaryIndex(start);
        int lastPiece = GetBoundaryIndex(end);
        _pieces.RemoveRange(firstPiece, lastPiece - firstPiece);
        _length -= end - start;
    }

    /// <summary>
    /// Gets the text in the half-open range [<paramref name="start"/>, <paramref name="end"/>).
    /// </summary>
    public string GetText(int start, int end)
    {
        ValidateRange(start, end);

        if (start == end)
        {
            return string.Empty;
        }

        System.Text.StringBuilder result = new (end - start);
        AppendText(result, start, end);
        return result.ToString();
    }

    /// <summary>
    /// Gets a zero-based line without its LF or CRLF line terminator.
    /// </summary>
    public string GetLine(int lineNumber)
    {
        if (lineNumber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lineNumber), "Line numbers are zero-based.");
        }

        System.Text.StringBuilder line = new ();
        int currentLine = 0;

        foreach (Piece piece in _pieces)
        {
            string buffer = GetBuffer(piece);
            int pieceEnd = piece.Start + piece.Length;

            for (int bufferIndex = piece.Start; bufferIndex < pieceEnd; bufferIndex++)
            {
                char character = buffer[bufferIndex];

                if (character == '\n')
                {
                    if (currentLine == lineNumber)
                    {
                        RemoveCarriageReturn(line);
                        return line.ToString();
                    }

                    currentLine++;
                    line.Clear();
                    continue;
                }

                if (currentLine == lineNumber)
                {
                    line.Append(character);
                }
            }
        }

        if (currentLine == lineNumber)
        {
            return line.ToString();
        }

        throw new ArgumentOutOfRangeException(nameof(lineNumber), "The requested line does not exist.");
    }

    private void AppendText(System.Text.StringBuilder result, int start, int end)
    {
        int documentPosition = 0;

        foreach (Piece piece in _pieces)
        {
            int pieceEnd = documentPosition + piece.Length;

            if (pieceEnd <= start)
            {
                documentPosition = pieceEnd;
                continue;
            }

            if (documentPosition >= end)
            {
                break;
            }

            int overlapStart = Math.Max(start, documentPosition);
            int overlapEnd = Math.Min(end, pieceEnd);
            string buffer = GetBuffer(piece);
            result.Append(buffer, piece.Start + overlapStart - documentPosition, overlapEnd - overlapStart);
            documentPosition = pieceEnd;
        }
    }

    private void SplitAt(int offset)
    {
        if (offset == 0 || offset == _length)
        {
            return;
        }

        int documentPosition = 0;

        for (int pieceIndex = 0; pieceIndex < _pieces.Count; pieceIndex++)
        {
            Piece piece = _pieces[pieceIndex];
            int pieceEnd = documentPosition + piece.Length;

            if (offset > documentPosition && offset < pieceEnd)
            {
                int firstLength = offset - documentPosition;
                _pieces[pieceIndex] = piece with { Length = firstLength };
                _pieces.Insert(pieceIndex + 1, piece with
                {
                    Start = piece.Start + firstLength,
                    Length = piece.Length - firstLength
                });
                return;
            }

            documentPosition = pieceEnd;
        }
    }

    private int GetBoundaryIndex(int offset)
    {
        int documentPosition = 0;

        for (int pieceIndex = 0; pieceIndex < _pieces.Count; pieceIndex++)
        {
            if (documentPosition == offset)
            {
                return pieceIndex;
            }

            documentPosition += _pieces[pieceIndex].Length;
        }

        return _pieces.Count;
    }

    private string GetBuffer(Piece piece)
    {
        return piece.Buffer == BufferType.Original
            ? _originalText
            : _addedBuffers[piece.BufferIndex];
    }

    private void ValidateRange(int start, int end)
    {
        ValidateOffset(start, nameof(start), allowEnd: true);
        ValidateOffset(end, nameof(end), allowEnd: true);

        if (end < start)
        {
            throw new ArgumentOutOfRangeException(nameof(end), "The end offset must not be less than the start offset.");
        }
    }

    private void ValidateOffset(int offset, string parameterName, bool allowEnd)
    {
        int maximum = allowEnd ? _length : _length - 1;

        if (offset < 0 || offset > maximum)
        {
            throw new ArgumentOutOfRangeException(parameterName, "The offset is outside the document.");
        }
    }

    private static void RemoveCarriageReturn(System.Text.StringBuilder line)
    {
        if (line.Length > 0 && line[^1] == '\r')
        {
            line.Length--;
        }
    }
}
