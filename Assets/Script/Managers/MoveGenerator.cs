using System;
using System.Collections.Generic;
using System.Linq;
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

            //calculate Pins
            moves.AddRange(GenerateCapturesAndBlocks());
            

            return moves;
        }
        else 
        {
            // calculate Pins
            // generate other moves

            return moves;
        }
    }

    private List<Move> GenerateCapturesAndBlocks()
    {
        List<Move> moves = new();

        foreach (var piece in board.GetAllPieces(actualColor)) 
        {
            if (piece is King) continue;

            var squareIndex = (piece.MovingSquares.value & (kingAttackersSquaresBitboard.value | inBetweenKingAndAttackersBitboard.value));
            if (squareIndex <= 0) continue;

            var toTile = board.GetTileByIndex(squareIndex.ConvertToIndex());
            Move move = new Move(piece.GetTile(), toTile, piece, kingAttackers.First());
            moves.Add(move);
        }

        return moves;
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
        TileCoordinates coord = piece.Coordinates;
        TileCoordinates kingCoord = kingPiece.Coordinates;

        int pieceIndex = piece.GetTile().Index;
        int kingIndex = kingPiece.GetTile().Index;
        
        /*if(coord.row == kingCoord.row) 
        {
            int difference = pieceIndex - kingIndex;
            if (Mathf.Abs(difference) == 1) return;

            for(int i = 1; Mathf.Abs(i) < Mathf.Abs(difference); i += (int)Mathf.Sign(difference)) 
            {
                int column = kingCoord.column + i;
                Tile tile = board.GetTiles()[kingCoord.row][column];

                inBetweenKingAndAttackersBitboard.Add(tile.Bitboard);
            }
        }
        else if(coord.column == kingCoord.column) 
        {
            int difference = (pieceIndex - kingIndex) / 8;
            if (Mathf.Abs(difference) == 1) return;

            for (int i = 1; Mathf.Abs(i) < Mathf.Abs(difference); i += (int)Mathf.Sign(difference))
            {
                int row = kingCoord.row + i;
                Tile tile = board.GetTiles()[row][kingCoord.column];

                inBetweenKingAndAttackersBitboard.Add(tile.Bitboard);
            }
        }
        else 
        {
            

        }*/

        int delta = pieceIndex - kingIndex;
        int rate = 0;

        if (coord.row == kingCoord.row)
            rate = 1;
        if (coord.column == kingCoord.column)
            rate = 8;
        else
            rate = (delta % 9 == 0) ? 9 : 7;
        
        int difference = delta / rate;

        if (Mathf.Abs(difference) == 1) return;

        for (int i = 1; Mathf.Abs(i) < Mathf.Abs(difference); i += (int)Mathf.Sign(difference))
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

        var impossibleSquares = (enemiesKingDangerSquares.value | enemiesAttackingSquares.value);
        foreach (var tileCoord in possibleTiles) 
        {
            var tile = board.GetTiles()[tileCoord.row][tileCoord.column];
            if ((impossibleSquares & tile.Bitboard.value) > 0) continue;

            if (tile.IsOccupied is false || (tile.OccupiedBy.pieceColor != actualColor))
                moves.Add(new Move(kingTile, tile, kingPiece, tile.OccupiedBy));
        }

        return moves;
    }
}
