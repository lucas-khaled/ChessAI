using System;
using System.Collections.Generic;

public enum PieceColor
{
    White,
    Black
}

public abstract class Piece
{
    public PieceColor pieceColor;

    public VisualPiece visualPiece { get; set; }
    public string name { get; set; }

    protected Tile actualTile;
    public TileCoordinates Coordinates => actualTile.TilePosition;
    protected int Row => Coordinates.row;
    protected int Column => Coordinates.column;
    protected bool IsWhite => pieceColor == PieceColor.White;

    public Board Board { get; set; }
    public PinnerPiece PinnedBy { get; set; }

    public Bitboard MovingSquares { get; set; } = new Bitboard();
    public Bitboard AttackingSquares { get; set; } = new Bitboard();

    protected abstract void GenerateBitBoardMethod();
    public abstract Move[] GetMoves();

    public Piece(Board board) 
    {
        Board = board;
    }

    public virtual void GenerateBitBoard() 
    {
        PinnedBy = null;
        MovingSquares.Clear();
        AttackingSquares.Clear();

        GenerateBitBoardMethod();
    }

    public void SetTile(Tile tile) 
    {
        actualTile = tile;

        if (tile == null) return;

        name = $"{pieceColor} - {GetType().Name} ({tile.TilePosition.row},{tile.TilePosition.column})";
        
        if (visualPiece == null || tile == null) return;

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
            moves[i] = new Move(actualTile, segments[i], this, segments[i].OccupiedBy);

        return moves;
    }

    protected List<Tile> CheckForBlockingSquares(List<TileCoordinates> segment, bool capturesIfEnemy = true, bool includeBlockingPieceSquare = false)
    {
        List<Tile> finalTiles = new();
        foreach (var tileCoord in segment)
        {
            Tile tile = Board.tiles[tileCoord.row][tileCoord.column];
            if (tile.IsOccupied)
            {
                if (IsEnemyPiece(tile.OccupiedBy) && capturesIfEnemy || includeBlockingPieceSquare)
                    finalTiles.Add(tile);

                break;
            }

            finalTiles.Add(tile);
        }

        return finalTiles;
    }

    protected List<Tile> GetTilesFromCoordinates(List<TileCoordinates> segment, bool friendlyBlock = true) 
    {
        List<Tile> finalTiles = new();
        foreach (var tileCoord in segment)
        {
            Tile tile = Board.tiles[tileCoord.row][tileCoord.column];
            if (friendlyBlock && tile.IsOccupied && tile.OccupiedBy.pieceColor == pieceColor) break;

            finalTiles.Add(tile);
        }

        return finalTiles;
    }

    protected Bitboard AddTilesBitBoards(List<Tile> tiles)
    {
        Bitboard bitBoard = new Bitboard();
        foreach (Tile tile in tiles)
            bitBoard.Add(tile.Bitboard);

        return bitBoard;
    }

    public Piece Copy(Tile tile) 
    {
        var type = this.GetType();
        Piece piece = Activator.CreateInstance(type, tile.Board) as Piece;

        piece.SetTile(tile);
        piece.pieceColor = pieceColor;
        piece.visualPiece = null;
        piece.name = name;

        return piece;
    }

    public Piece Copy()
    {
        return Copy(null);
    }

    public override string ToString()
    {
        string coord = (actualTile == null) ? "null" : Coordinates.ToString();
        return $"{this.GetType().Name} {pieceColor} ({coord})";
    }

    public override bool Equals(object obj)
    {
        return obj is Piece otherPiece && otherPiece.name == name;
    }
}
