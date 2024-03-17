using UnityEngine;

public class EspecialRules : IEnvironmentable
{
    public Environment Environment { get; }

    public bool whiteCanCastleKingSide { get; private set; } = true;
    public bool whiteCanCastleQueenSide { get; private set; } = true;
    public bool blackCanCastleQueenSide { get; private set; } = true;
    public bool blackCanCastleKingSide { get; private set; } = true;

    public Tile enPassantTile { get; private set; }
    public Pawn enPassantPawn { get; private set; }

    public EspecialRules(Environment environment) 
    {
        Environment = environment;
        Environment.events.onMoveMade += OnPieceMoved;
    }

    public IEnvironmentable Copy(Environment env)
    {
        return new EspecialRules(env)
        {
            whiteCanCastleQueenSide = this.whiteCanCastleQueenSide,
            whiteCanCastleKingSide = this.whiteCanCastleKingSide,
            blackCanCastleKingSide = this.blackCanCastleKingSide,
            blackCanCastleQueenSide = this.blackCanCastleKingSide,
            enPassantPawn = this.enPassantPawn,
            enPassantTile = this.enPassantTile
        };
    }

    public bool CanCastle(PieceColor color, Rook rook)
    {
        if (HasCastledAllSides(color)) return false;

        var coord = GetRookCoordinates(true, color);
        var queensideBool = (color == PieceColor.White) ? whiteCanCastleQueenSide : blackCanCastleQueenSide;

        if (rook.GetTile().TilePosition.Equals(coord) && queensideBool)
            return true;

        coord = GetRookCoordinates(false, color);
        var kingsideBool = (color == PieceColor.White) ? whiteCanCastleKingSide : blackCanCastleKingSide;

        if (rook.GetTile().TilePosition.Equals(coord) && kingsideBool)
            return true;

        return false;
    }

    private bool HasCastledAllSides(PieceColor pieceColor) 
    {
        var isWhite = pieceColor == PieceColor.White;

        return (isWhite && whiteCanCastleKingSide is false && whiteCanCastleQueenSide is false) ||
            (isWhite is false && blackCanCastleKingSide is false && blackCanCastleQueenSide is false);
    }

    private TileCoordinates GetRookCoordinates(bool isQueenSide, PieceColor pieceColor) 
    {
        bool isWhite = pieceColor == PieceColor.White;

        if(isQueenSide)
            return isWhite ? new TileCoordinates(0, 0) : new TileCoordinates(7, 0);
        else
            return isWhite ? new TileCoordinates(0, 7) : new TileCoordinates(7, 7);
    }

    public void SetCastle(PieceColor pieceColor)
    {
        switch (pieceColor)
        {
            case PieceColor.White:
                whiteCanCastleKingSide = false;
                whiteCanCastleQueenSide = false;
                break;
            case PieceColor.Black:
                blackCanCastleKingSide = false;
                blackCanCastleQueenSide = false;
                break;
        }
    }

    private void OnPieceMoved(Move move) 
    {
        if (move.piece is Pawn)
        {
            CheckEnPassant(move);
            return;
        }

        if (move is CastleMove || move.piece is King)
            SetCastle(move.piece.pieceColor);
        else if (move.piece is Rook)
            RookMoved(move);

        enPassantTile = null;
        enPassantPawn = null;
    }

    private void CheckEnPassant(Move move)
    {
        TileCoordinates toCoord = move.to.TilePosition;
        var moveRange = Mathf.Abs(move.from.TilePosition.row - move.to.TilePosition.row);
        if (moveRange < 2)
        {
            enPassantTile = null;
            enPassantPawn = null;
            return;
        }

        var row = (move.piece.pieceColor == PieceColor.White) ? toCoord.row - 1 : toCoord.row + 1;
        SetEnPassant(Environment.board.GetTiles()[row][toCoord.column], move.piece as Pawn);
    }

    public void SetEnPassant(Tile tile, Pawn pawn) 
    {
        enPassantTile = tile;
        enPassantPawn = pawn;
    }

    private void RookMoved(Move rookMove)
    {
        var pieceColor = rookMove.piece.pieceColor;
        if (HasCastledAllSides(pieceColor)) return;

        var queensidePosition = GetRookCoordinates(true, pieceColor);
        var kingsidePosition = GetRookCoordinates(false, pieceColor);

        if (rookMove.from.TilePosition.Equals(queensidePosition)) 
        {
            SetCastleQueenSide(pieceColor);
            return;
        }

        if (rookMove.from.TilePosition.Equals(kingsidePosition))
        {
            SetCastleKingSide(pieceColor);
            return;
        }
    }

    public void SetCastleKingSide(PieceColor color) 
    {
        if (color == PieceColor.White)
            whiteCanCastleKingSide = false;
        else
            blackCanCastleKingSide = false;
    }

    public void SetCastleQueenSide(PieceColor color)
    {
        if (color == PieceColor.White)
            whiteCanCastleQueenSide = false;
        else
            blackCanCastleQueenSide = false;
    }
}
