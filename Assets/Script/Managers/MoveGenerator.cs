using System.Collections.Generic;
using System.Linq;

public class MoveGenerator
{
    private Board board;

    private Bitboard attackingSquares;
    private Bitboard kingDangerSquares;

    private Bitboard enemiesAttackingSquares;
    private Bitboard enemiesKingDangerSquares;

    private List<Piece> kingAttackers;
    private Piece kingPiece;

    public MoveGenerator(Board board) 
    {
        this.board = board;
    }

    public List<Move> GenerateMoves(PieceColor color) 
    {
        List<Move> moves = new List<Move>();

        Initialize(color);
        GenerateBitboards(color);

        if (IsCheck())
        {
            if (IsDoubleCheck() is false)
            {
                moves.AddRange(GenerateKingMoves(color));
                return moves;
            }

            //calculate captures
            //calculate blocks
            //calculate Pins
        }
        else 
        {
            // calculate Pins
            // generate other moves
        }




        return null;
    }

    private void Initialize(PieceColor color)
    {
        kingPiece = board.GetKingTile(color).OccupiedBy;
        kingAttackers = new();
    }

    private void GenerateBitboards(PieceColor color)
    {
        attackingSquares = new Bitboard();
        kingDangerSquares = new Bitboard();

        enemiesAttackingSquares = new Bitboard();
        enemiesKingDangerSquares = new Bitboard();
        GenerateMyBitboards(color);
        GenerateEnemyBitboards(color);
    }

    private void GenerateEnemyBitboards(PieceColor color)
    {
        foreach (var piece in board.GetAllPieces(color.GetOppositeColor()))
        {
            piece.GenerateBitBoard();
            enemiesAttackingSquares.Add(piece.AttackingSquares);
            enemiesKingDangerSquares.Add(piece.KingDangerSquares);

            if ((piece.AttackingSquares.value & kingPiece.GetTile().Bitboard.value) > 0)
                kingAttackers.Add(piece);
        }
    }

    private void GenerateMyBitboards(PieceColor color)
    {
        foreach (var piece in board.GetAllPieces(color))
        {
            piece.GenerateBitBoard();
            attackingSquares.Add(piece.AttackingSquares);
            kingDangerSquares.Add(piece.KingDangerSquares);
        }
    }

    private bool IsCheck()
    {
        return kingAttackers.Count > 0;
    }

    private bool IsDoubleCheck() 
    {
        return kingAttackers.Count > 1;
    }

    private List<Move> GenerateKingMoves(PieceColor color) 
    {
        var moves = new List<Move>();
        var possibleTiles = new List<TileCoordinates>();

        var kingTile = kingPiece.GetTile();
        var verticals = kingTile.GetVerticalsByColor(color);
        var horizontals = kingTile.GetHorizontalsByColor(color);
        var diagonals = kingTile.GetDiagonalsByColor(color);

        if(verticals.frontVerticals.Count>0) possibleTiles.Add(verticals.frontVerticals.First());
        if(verticals.backVerticals.Count > 0) possibleTiles.Add(verticals.backVerticals.First());
        if(horizontals.rightHorizontals.Count > 0) possibleTiles.Add(horizontals.rightHorizontals.First());
        if (horizontals.leftHorizontals.Count > 0) possibleTiles.Add(horizontals.leftHorizontals.First());
        if (diagonals.topRightDiagonals.Count > 0) possibleTiles.Add(diagonals.topRightDiagonals.First());
        if (diagonals.topLeftDiagonals.Count > 0) possibleTiles.Add(diagonals.topLeftDiagonals.First());
        if (diagonals.downLeftDiagonals.Count > 0) possibleTiles.Add(diagonals.downLeftDiagonals.First());
        if (diagonals.downRightDiagonals.Count > 0) possibleTiles.Add(diagonals.downRightDiagonals.First());


        foreach (var tileCoord in possibleTiles) 
        {
            var tile = board.GetTiles()[tileCoord.row][tileCoord.column];
            if ((enemiesKingDangerSquares.value & tile.Bitboard.value) > 0) continue;

            if (tile.IsOccupied is false || (tile.OccupiedBy.pieceColor != color))
                moves.Add(new Move(kingTile, tile, kingPiece, tile.OccupiedBy));
        }

        return moves;
    }
}
