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

        var enPassant = GetEnPassant();
        if (enPassant != null)
            possibleMoves.Add(enPassant);
        
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

        if(CanMoveToDiagonal(diagonals.topLeftDiagonals))
            moves.AddRange(CreateMovesFromSegment(diagonals.topLeftDiagonals));

        if (CanMoveToDiagonal(diagonals.topRightDiagonals))
            moves.AddRange(CreateMovesFromSegment(diagonals.topRightDiagonals));

        return moves.ToArray();
    }

    private bool CanMoveToDiagonal(List<Tile> diagonal) 
    {
        return diagonal.Count > 0 && IsEnemyPiece(diagonal[0].OccupiedBy);
    }

    private Move GetEnPassant()
    {
        if (IsInEnPassantRow() is false) return null;

        var lastDestiny = Environment.turnManager.LastMove.to;
        if (lastDestiny.OccupiedBy is not Pawn enemyPawn) return null;

        int rowForward = (pieceColor == PieceColor.White) ? 1 : -1;

        bool isSameRow = lastDestiny.TilePosition.row == actualTile.TilePosition.row;
        bool isPassedRow = lastDestiny.TilePosition.row == actualTile.TilePosition.row - rowForward;
        if (isSameRow is false && isPassedRow is false) return null;

        int columnDelta = lastDestiny.TilePosition.column - actualTile.TilePosition.column;
        bool isAdjacentColumn = Math.Abs(columnDelta) == 1;
        if (isAdjacentColumn is false) return null;

        
        var destinyTile = Environment.board.GetTiles()[actualTile.TilePosition.row + rowForward][lastDestiny.TilePosition.column];
        return new Move(actualTile, destinyTile, this, enemyPawn);
    }

    private bool IsInEnPassantRow() 
    {
        var row = actualTile.TilePosition.row;
        return (row == 4 || row == 5 && pieceColor == PieceColor.White)
            || (row == 3 || row == 2 && pieceColor == PieceColor.Black);
    }
}
