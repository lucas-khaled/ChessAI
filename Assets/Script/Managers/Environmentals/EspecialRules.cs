using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EspecialRules : IEnvironmentable
{
    public Environment Environment { get; }

    private HashSet<Rook> movedRooks = new();
    private HashSet<King> movedKings = new();

    private bool whiteHasCastled = false;
    private bool blackHasCastled = false;

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
            movedKings = this.movedKings,
            movedRooks = this.movedRooks,
            enPassantPawn = this.enPassantPawn,
            enPassantTile = this.enPassantTile
        };
    }

    public bool CanCastle(PieceColor color, Rook rook)
    {
        bool checkingBool = (color == PieceColor.White) ? whiteHasCastled : blackHasCastled;
        return checkingBool is false && movedKings.Any(x => x.pieceColor == color) is false && movedRooks.Contains(rook) is false;
    }

    public void SetCastle(PieceColor pieceColor)
    {
        switch (pieceColor)
        {
            case PieceColor.White:
                whiteHasCastled = true;
                break;
            case PieceColor.Black:
                blackHasCastled = true;
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

        if (move is CastleMove)
            SetCastle(move.piece.pieceColor);
        else if (move.piece is Rook rook)
            RookMoved(rook);
        else if (move.piece is King king)
            KingMoved(king);

        enPassantTile = null;
    }

    private void CheckEnPassant(Move move)
    {
        TileCoordinates toCoord = move.to.TilePosition;
        var moveRange = Mathf.Abs(move.from.TilePosition.row - move.to.TilePosition.row);
        if (moveRange < 2) return;

        var row = (move.piece.pieceColor == PieceColor.White) ? toCoord.row - 1 : toCoord.row + 1;

        enPassantTile = Environment.board.GetTiles()[row][toCoord.column];
        enPassantPawn = move.piece as Pawn;
    }

    private void RookMoved(Rook rook)
    {
        if (movedRooks.Contains(rook)) return;

        movedRooks.Add(rook);
    }

    private void KingMoved(King king) 
    {
        if (movedKings.Contains(king)) return;

        movedKings.Add(king);
    }
}
