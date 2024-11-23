using System;
using System.Collections.Generic;

public class Pawn : SlidingPieces
{
    public Pawn(Board board) : base(board)
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
        var verticals = actualTile.GetVerticalsByColor(pieceColor);

        var toTileCoord = verticals.frontVerticals[0];
        var toTile = Board.tiles[toTileCoord.row][toTileCoord.column];
        return toTile.IsOccupied ? new PromotionMove[0] : GetPossiblePromotions(toTileCoord);
    }

    private List<PromotionMove> GetPromotionCaptures() 
    {
        List<PromotionMove> moves = new();

        var diagonals = actualTile.GetDiagonalsByColor(pieceColor);
        
        if (CanMoveToDiagonal(diagonals.topLeftDiagonals))
            moves.AddRange(GetPossiblePromotions(diagonals.topLeftDiagonals[0]));

        if (CanMoveToDiagonal(diagonals.topRightDiagonals))
            moves.AddRange(GetPossiblePromotions(diagonals.topRightDiagonals[0]));

        return moves;
    }

    private PromotionMove[] GetPossiblePromotions(TileCoordinates toCoord) 
    {
        return new PromotionMove[4]
        {
            new PromotionMove(actualTile, Board.tiles[toCoord.row][toCoord.column], this, new Rook(Board), Board.tiles[toCoord.row][toCoord.column].OccupiedBy),
            new PromotionMove(actualTile, Board.tiles[toCoord.row][toCoord.column], this, new Bishop(Board), Board.tiles[toCoord.row][toCoord.column].OccupiedBy),
            new PromotionMove(actualTile, Board.tiles[toCoord.row][toCoord.column], this, new Knight(Board), Board.tiles[toCoord.row][toCoord.column].OccupiedBy),
            new PromotionMove(actualTile, Board.tiles[toCoord.row][toCoord.column], this, new Queen(Board), Board.tiles[toCoord.row][toCoord.column].OccupiedBy)
        };
    }

    private Move[] GetFowardMoves() 
    {
        int range = (IsOnInitialRow()) ? 2 : 1;

        var verticals = actualTile.GetVerticalsByColor(pieceColor);
        var checkingBlockVerticals = CheckForBlockingSquares(verticals.frontVerticals.GetRange(0, range), false);
        
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

        var diagonals = actualTile.GetDiagonalsByColor(pieceColor);

        if (CanMoveToDiagonal(diagonals.topLeftDiagonals)) 
            moves.Add(CreateDiagonalMove(diagonals.topLeftDiagonals[0]));

        if (CanMoveToDiagonal(diagonals.topRightDiagonals))
            moves.Add(CreateDiagonalMove(diagonals.topRightDiagonals[0]));

        return moves.ToArray();
    }

    private bool CanMoveToDiagonal(List<TileCoordinates> diagonal) 
    {
        if (diagonal.Count <= 0) return false;

        var diagonalTile = Board.tiles[diagonal[0].row][diagonal[0].column];
        return IsEnemyPiece(diagonalTile.OccupiedBy)
            || (Board.rules.enPassantTile != null && diagonal[0].Equals(Board.rules.enPassantTile.TilePosition));
    }

    private Move CreateDiagonalMove(TileCoordinates diagonalTileCoord) 
    {
        Tile diagonalTile = Board.tiles[diagonalTileCoord.row][diagonalTileCoord.column];
        return diagonalTile.IsOccupied ?
            new Move(actualTile, diagonalTile, this, diagonalTile.OccupiedBy) :
            new Move(actualTile, diagonalTile, this, Board.rules.enPassantPawn);
    }

    public override void GenerateBitBoard()
    {
        List<Tile> tiles = new List<Tile>();
        int range = IsOnInitialRow() ? 2 : 1;

        var verticals = actualTile.GetVerticalsByColor(pieceColor);
        var checkingBlockVerticals = CheckForBlockingSquares(verticals.frontVerticals.GetRange(0, range), false);

        tiles.AddRange(checkingBlockVerticals);

        Bitboard bitboard = AddTilesBitBoards(tiles);

        Diagonals diagonals = actualTile.GetDiagonalsByColor(pieceColor);

        if (diagonals.topLeftDiagonals.Count > 0)
        {
            var topLeftCoord = diagonals.topLeftDiagonals[0];
            if (CanMoveToDiagonal(diagonals.topLeftDiagonals))
                bitboard.Add(Board.GetTiles()[topLeftCoord.row][topLeftCoord.column].Bitboard);
        }

        if (diagonals.topRightDiagonals.Count > 0)
        {
            var topRightCoord = diagonals.topRightDiagonals[0];
            if (CanMoveToDiagonal(diagonals.topRightDiagonals))
                bitboard.Add(Board.GetTiles()[topRightCoord.row][topRightCoord.column].Bitboard);
        }

        AttackingSquares = bitboard;
    }
}
