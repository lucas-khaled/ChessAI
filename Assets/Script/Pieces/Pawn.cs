using System;
using System.Collections.Generic;

public class Pawn : BlockableMovesPiece
{
    public Pawn(Environment env) : base(env)
    {
    }

    public override Move[] GetMoves()
    {
        List<Move> possibleMoves = new List<Move>();

        if (NextMoveIsPromotion()) 
        {
            possibleMoves.AddRange(GetPromotionMoves());
            return possibleMoves.ToArray();
        }

        possibleMoves.AddRange(GetFowardMoves());
        possibleMoves.AddRange(GetCaptures());
        
        return possibleMoves.ToArray();
    }
    
    private bool NextMoveIsPromotion() 
    {
        return (Row == 6 && IsWhite)
            || (Row == 1 && !IsWhite);
    }

    private PromotionMove[] GetPromotionMoves()
    {
        List<PromotionMove> moves = new();

        var forwardMoves = GetPromotionsForward();
        moves.AddRange(forwardMoves);

        var captureMoves = GetPromotionCaptures();
        moves.AddRange(captureMoves);

        return moves.ToArray();
    }

    private PromotionMove[] GetPromotionsForward() 
    {
        var verticals = Environment.boardManager.GetVerticalsFrom(actualTile.TilePosition, pieceColor, 1);

        var toTile = verticals.frontVerticals[0];
        return toTile.IsOccupied ? new PromotionMove[0] : GetPossiblePromotions(toTile);
    }

    private List<PromotionMove> GetPromotionCaptures() 
    {
        List<PromotionMove> moves = new();

        var diagonals = Environment.boardManager.GetDiagonalsFrom(actualTile.TilePosition, pieceColor, 1);
        
        if (CanMoveToDiagonal(diagonals.topLeftDiagonals))
            moves.AddRange(GetPossiblePromotions(diagonals.topLeftDiagonals[0]));

        if (CanMoveToDiagonal(diagonals.topRightDiagonals))
            moves.AddRange(GetPossiblePromotions(diagonals.topRightDiagonals[0]));

        return moves;
    }

    private PromotionMove[] GetPossiblePromotions(Tile to) 
    {
        PromotionMove[] moves = new PromotionMove[4];
        Piece[] promoteTo = new Piece[4] { new Rook(Environment), new Bishop(Environment), new Knight(Environment), new Queen(Environment) };

        for(int i = 0; i < promoteTo.Length; i++) 
        {
            moves[i] = new PromotionMove(actualTile, to, this, promoteTo[i], to.OccupiedBy);
        }

        return moves;
    }

    private Move[] GetFowardMoves() 
    {
        int range = (IsOnInitialRow()) ? 2 : 1;

        var verticals = Environment.boardManager.GetVerticalsFrom(actualTile.TilePosition, pieceColor, range);
        var checkingBlockVerticals = CheckForBlockingSquares(verticals.frontVerticals, false);
        
        return CreateMovesFromSegment(checkingBlockVerticals);
    }

    private bool IsOnInitialRow()
    {
        return (Row == 1 && IsWhite)
            || (Row == 6 && !IsWhite);
    }

    private Move[] GetCaptures() 
    {
        List<Move> moves = new();

        var diagonals = Environment.boardManager.GetDiagonalsFrom(actualTile.TilePosition, pieceColor, 1);

        if (CanMoveToDiagonal(diagonals.topLeftDiagonals)) 
            moves.Add(CreateDiagonalMove(diagonals.topLeftDiagonals[0]));

        if (CanMoveToDiagonal(diagonals.topRightDiagonals))
            moves.Add(CreateDiagonalMove(diagonals.topRightDiagonals[0]));

        return moves.ToArray();
    }

    private bool CanMoveToDiagonal(List<Tile> diagonal) 
    {
        if (diagonal.Count <= 0) return false;

        return IsEnemyPiece(diagonal[0].OccupiedBy)
            || (Environment.rules.enPassantTile != null && diagonal[0].TilePosition.Equals(Environment.rules.enPassantTile.TilePosition));
    }

    private Move CreateDiagonalMove(Tile diagonalTile) 
    {
        return diagonalTile.IsOccupied ?
            new Move(actualTile, diagonalTile, this, diagonalTile.OccupiedBy) :
            new Move(actualTile, diagonalTile, this, Environment.rules.enPassantPawn);
    }
}
