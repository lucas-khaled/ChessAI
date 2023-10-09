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

    public abstract Move[] GetMoves();
    public virtual void MoveTo(Tile tile) 
    {
        if(actualTile != null)
            actualTile.DeOccupy();

        SetTile(tile);
        actualTile.Occupy(this);

        transform.position = tile.transform.position;
        transform.SetParent(tile.transform);
    }

    public void SetTile(Tile tile) 
    {
        actualTile = tile;
    }

    public Tile GetTile() 
    {
        return actualTile;
    }

    public void Capture() 
    {
        actualTile.DeOccupy();
        actualTile = null;
        isCaptured = true;
    }

    public bool IsEnemyPiece(Piece piece) 
    {
        return piece != null && pieceColor != piece.pieceColor;
    }

    protected Move[] CreateMovesFromSegment(List<Tile> segments)
    {
        Move[] moves = new Move[segments.Count];

        for (int i = 0; i < segments.Count; i++)
        {
            var capture = (IsEnemyPiece(segments[i].OccupiedBy)) ? segments[i].OccupiedBy : null;

            moves[i] = new Move(actualTile, segments[i], capture);
        }

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
