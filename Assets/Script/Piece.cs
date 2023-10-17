using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceColor
{
    White,
    Black
}

public abstract class Piece : MonoBehaviour
{
    public PieceColor pieceColor;

    protected Tile actualTile;
    protected bool isCaptured;
    public TileCoordinates Coordinates => actualTile.TilePosition;
    protected int Row => Coordinates.row;
    protected int Column => Coordinates.column;
    protected bool IsWhite => pieceColor == PieceColor.White;

    public abstract Move[] GetMoves(Board board);

    public void SetTile(Tile tile, bool isVirtual = false) 
    {
        actualTile = tile;

        if (isVirtual) return;

        transform.position = tile.visualTile.transform.position;
        transform.SetParent(tile.visualTile.transform);
    }

    public Tile GetTile() 
    {
        return actualTile;
    }

    public bool IsEnemyPiece(Piece piece) 
    {
        return piece != null && pieceColor != piece.pieceColor;
    }

    protected Move[] CreateMovesFromSegment(List<Tile> segments)
    {
        Move[] moves = new Move[segments.Count];

        for (int i = 0; i < segments.Count; i++)
            moves[i] = new Move(actualTile, segments[i], segments[i].OccupiedBy);

        return moves;
    }

    protected List<Tile> CheckForBlockingSquares(List<Tile> segment, bool capturesIfEnemy = true)
    {
        List<Tile> finalTiles = new();
        foreach (var tile in segment)
        {
            if (tile.IsOccupied)
            {
                if (IsEnemyPiece(tile.OccupiedBy) && capturesIfEnemy)
                    finalTiles.Add(tile);

                break;
            }

            finalTiles.Add(tile);
        }

        return finalTiles;
    }
}
