using System.Collections.Generic;
using UnityEngine.Profiling;

public class Knight : Piece
{
    public Knight(Board board) : base(board)
    {
    }

    protected override void GenerateBitBoardMethod()
    {
        Profiler.BeginSample("Move Generation > Generate Bitboard -> Knight");
        int currentIndex = actualTile.Index;
        Bitboard bitboard = new Bitboard();

        //topLeft move
        int topLeftIndex = currentIndex + 15;
        if (currentIndex % 8 > 0 && topLeftIndex < 64)
        {
            var tile = Board.GetTileByIndex(topLeftIndex);
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, tile);
            AttackingSquares.Add(tile.Bitboard);
        }

        //topRight move
        int topRightIndex = currentIndex + 17;
        if (currentIndex % 8 < 7 && topRightIndex < 64)
        {
            var tile = Board.GetTileByIndex(topRightIndex);
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, tile);
            AttackingSquares.Add(tile.Bitboard);
        }

        //leftTop move
        int leftTopIndex = currentIndex + 6;
        if (currentIndex % 8 > 1 && leftTopIndex < 64)
        {
            var tile = Board.GetTileByIndex(leftTopIndex);
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, tile);
            AttackingSquares.Add(tile.Bitboard);
        }

        //leftDown move
        int leftDownIndex = currentIndex - 10;
        if (currentIndex % 8 > 1 && leftDownIndex >= 0)
        {
            var tile = Board.GetTileByIndex(leftDownIndex);
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, tile);
            AttackingSquares.Add(tile.Bitboard);
        }

        //rightTop move
        int rightTopIndex = currentIndex + 10;
        if (currentIndex % 8 < 6 && rightTopIndex < 64)
        {
            var tile = Board.GetTileByIndex(rightTopIndex);
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, tile);
            AttackingSquares.Add(tile.Bitboard);
        }

        //rightDown move
        int rightDownIndex = currentIndex - 6;
        if (currentIndex % 8 < 6 && rightDownIndex >= 0)
        {
            var tile = Board.GetTileByIndex(rightDownIndex);
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, tile);
            AttackingSquares.Add(tile.Bitboard);
        }

        //DownLeft move
        int downLeftIndex = currentIndex - 17;
        if (currentIndex % 8 > 0 && downLeftIndex >= 0)
        {
            var tile = Board.GetTileByIndex(downLeftIndex);
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, tile);
            AttackingSquares.Add(tile.Bitboard);
        }

        //DownLeft move
        int downRightIndex = currentIndex - 15;
        if (currentIndex % 8 < 7 && downRightIndex >= 0)
        {
            var tile = Board.GetTileByIndex(downRightIndex);
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, tile);
            AttackingSquares.Add(tile.Bitboard);
        }

        MovingSquares = bitboard;

        Profiler.EndSample();
    }

    private void AddToBitboardIfNotOccupiedByFriend(ref Bitboard bitboard, Tile tile) 
    {
        if (tile.IsOccupied && tile.OccupiedBy.pieceColor == pieceColor) return;

        bitboard.Add(tile.Bitboard);
    }
}
