public class FEN
{
    public string[] positions;
    public PieceColor pieceColor;
    public string castlingString;
    public string enPassantString;
    public string halfMovesString;
    public string fullMovesString;

    public string fullString;
    public string fullPositionsString;

    public FEN(string fen) 
    {
        string[] fieldsSplited = fen.Split(' ');

        fullPositionsString = fieldsSplited[0];
        positions = fullPositionsString.Split('/');

        SetPieceColor(fieldsSplited[1]);

        castlingString = fieldsSplited[2];
        enPassantString = fieldsSplited[3];
        halfMovesString = fieldsSplited[4];
        fullMovesString = fieldsSplited[5];

        fullString = fen;
    }

    private void SetPieceColor(string colorString) 
    {
        pieceColor = (colorString == "w") ? PieceColor.White : PieceColor.Black;
    }

    public override string ToString()
    {
        return fullString;
    }
}
