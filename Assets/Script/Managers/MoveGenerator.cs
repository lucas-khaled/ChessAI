using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MoveGenerator
{
    private Board board;

    private Bitboard attackingSquares;
    private Bitboard kingDangerSquares;

    private Bitboard enemiesAttackingSquares;
    private Bitboard enemiesKingDangerSquares;

    private Bitboard kingAttackersSquaresBitboard;
    public Bitboard inBetweenKingAndAttackersBitboard;

    private List<Piece> kingAttackers;
    private Piece kingPiece;
    private PieceColor actualColor;

    public MoveGenerator(Board board) 
    {
        this.board = board;
    }

    public List<Move> GenerateMoves(PieceColor color) 
    {
        List<Move> moves = new List<Move>();
        
        Initialize(color);
        GenerateBitboards();

        if (IsCheck())
        {
            moves.AddRange(GenerateKingMoves());
            if (IsDoubleCheck())
                return moves;

            moves.AddRange(GenerateCapturesAndBlocks());

            return moves;
        }
        else 
        {
            moves.AddRange(GeneratePiecesMove());

            return moves;
        }
    }

    private List<Move> GenerateCapturesAndBlocks()
    {
        List<Move> moves = new();

        foreach (var piece in board.GetAllPieces(actualColor))
        {
            if (piece is King || IsPinned(piece)) continue;

            var squareIndex = (piece.MovingSquares.value & (kingAttackersSquaresBitboard.value | inBetweenKingAndAttackersBitboard.value));
            if (squareIndex <= 0) continue;
            FillMovesFromPiece(moves, piece, squareIndex);
        }

        return moves;
    }

    private void FillMovesFromPiece(List<Move> moves, Piece piece, ulong movementIndexes)
    {
        foreach (var index in movementIndexes.ConvertToIndexes())
        {
            var toTile = board.GetTileByIndex(index);
            GeneratePieceMove(moves, piece, toTile);
        }
    }

    private void GeneratePieceMove(List<Move> moves, Piece piece, Tile toTile)
    {
        if(piece is King) 
        {
            if (IsImpossibleForKing(toTile)) return;
            //check for castle
        }

        Move move = new Move(piece.GetTile(), toTile, piece, toTile.OccupiedBy);
        moves.Add(move);
    }

    private bool IsPinned(Piece piece) 
    {
        return (piece.GetTile().Bitboard.value & enemiesKingDangerSquares.value) > 0;
    }

    private void Initialize(PieceColor color)
    {
        actualColor = color;
        kingPiece = board.GetKingTile(color).OccupiedBy;
        kingAttackers = new();
    }

    private void GenerateBitboards()
    {
        attackingSquares = new Bitboard();
        kingDangerSquares = new Bitboard();
        inBetweenKingAndAttackersBitboard = new Bitboard();
        enemiesAttackingSquares = new Bitboard();
        enemiesKingDangerSquares = new Bitboard();

        GenerateMyBitboards();
        GenerateEnemyBitboards();
    }

    private void GenerateEnemyBitboards()
    {
        kingAttackersSquaresBitboard = new Bitboard();
        foreach (var piece in board.GetAllPieces(actualColor.GetOppositeColor()))
        {
            piece.GenerateBitBoard();
            enemiesAttackingSquares.Add(piece.AttackingSquares);
            enemiesKingDangerSquares.Add(piece.KingDangerSquares);

            if ((piece.AttackingSquares.value & kingPiece.GetTile().Bitboard.value) > 0)
            {
                kingAttackersSquaresBitboard.Add(piece.GetTile().Bitboard);
                kingAttackers.Add(piece);

                GenerateInBetwwen(piece);
            }
        }
    }

    private void GenerateInBetwwen(Piece piece)
    {
        if (piece is not SlidingPieces || piece is Pawn) return;

        TileCoordinates coord = piece.Coordinates;
        TileCoordinates kingCoord = kingPiece.Coordinates;

        int pieceIndex = piece.GetTile().Index;
        int kingIndex = kingPiece.GetTile().Index;

        int delta = pieceIndex - kingIndex;
        int rate;

        if (coord.row == kingCoord.row)
            rate = 1;
        if (coord.column == kingCoord.column)
            rate = 8;
        else
            rate = (delta % 9 == 0) ? 9 : 7;
        
        int difference = delta / rate;

        if (Mathf.Abs(difference) == 1) return;

        for (int i = (int)Mathf.Sign(difference); Mathf.Abs(i) < Mathf.Abs(difference); i += (int)Mathf.Sign(difference))
        {
            int index = kingIndex + i * rate;
            Tile tile = board.GetTileByIndex(index);

            inBetweenKingAndAttackersBitboard.Add(tile.Bitboard);
        }
    }

    private void GenerateMyBitboards()
    {
        foreach (var piece in board.GetAllPieces(actualColor))
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

    private List<Move> GenerateKingMoves() 
    {
        var moves = new List<Move>();
        var possibleTiles = new List<TileCoordinates>();

        var kingTile = kingPiece.GetTile();
        var verticals = kingTile.GetVerticalsByColor(actualColor);
        var horizontals = kingTile.GetHorizontalsByColor(actualColor);
        var diagonals = kingTile.GetDiagonalsByColor(actualColor);

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
            if (IsImpossibleForKing(tile)) continue;

            if (tile.IsOccupied is false || (tile.OccupiedBy.pieceColor != actualColor))
                moves.Add(new Move(kingTile, tile, kingPiece, tile.OccupiedBy));
        }

        return moves;
    }

    private bool IsImpossibleForKing(Tile tile)
    {
        var impossibleSquares = enemiesKingDangerSquares.value | enemiesAttackingSquares.value;
        List<int> indexes = impossibleSquares.ConvertToIndexes();
        return (impossibleSquares & tile.Bitboard.value) > 0;
    }

    private List<Move> GeneratePiecesMove() 
    {
        List<Move> moves = new();
        foreach (Piece piece in board.GetAllPieces(actualColor)) 
        {
            if (IsPinned(piece)) continue;

            FillMovesFromPiece(moves, piece, piece.MovingSquares.value);
        }

        return moves;
    }
}
