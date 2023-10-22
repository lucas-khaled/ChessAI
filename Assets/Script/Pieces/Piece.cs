using System;
using System.Collections.Generic;

public enum PieceColor
{
    White,
    Black
}

public abstract class Piece : IEnvironmentable
{
    public PieceColor pieceColor;

    public VisualPiece visualPiece { get; set; }

    protected Tile actualTile;
    public TileCoordinates Coordinates => actualTile.TilePosition;
    protected int Row => Coordinates.row;
    protected int Column => Coordinates.column;
    protected bool IsWhite => pieceColor == PieceColor.White;

    public abstract Move[] GetMoves();

    public void SetTile(Tile tile) 
    {
        actualTile = tile;

        if (visualPiece == null) return;

        visualPiece.SetTilePosition(tile);
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

    public IEnvironmentable Copy() 
    {
        var type = this.GetType();
        Piece piece = Activator.CreateInstance(type) as Piece;

        piece.SetTile(actualTile);
        piece.pieceColor = pieceColor;
        piece.visualPiece = null;

        return piece;
    }
}
