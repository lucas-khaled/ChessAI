using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : IEnvironmentable
{
    public Environment Environment { get; }

    public BoardManager(Environment environment) 
    {
        Environment = environment;
    }

    /// <summary>
    /// Returns Lists of tiles of the diagonals from the origin depending on piece color
    /// </summary>
    /// <param name="origin">The tile that will be compared</param>
    /// <returns>Returns 4 lists:
    /// 0 - Upper Left Diagonal
    /// 1 - Upper Right Diagonal
    /// 2 - Lower Left Diagonal
    /// 3 - Lower Right Diagonal</returns>
    public Diagonals GetDiagonalsFrom(TileCoordinates origin, PieceColor color, int range = 8)
    {
        Diagonals diagonals = new();

        diagonals.topLeftDiagonals = (color == PieceColor.White) ? GetTopLeftDiagonals(origin, range) : GetDownRightDiagonals(origin, range);
        diagonals.topRightDiagonals = (color == PieceColor.White) ? GetTopRightDiagonals(origin, range) : GetDownLeftDiagonals(origin, range);
        diagonals.downLeftDiagonals = (color == PieceColor.White) ? GetDownLeftDiagonals(origin, range) : GetTopRightDiagonals(origin, range);
        diagonals.downRightDiagonals = (color == PieceColor.White) ? GetDownRightDiagonals(origin, range) : GetTopLeftDiagonals(origin, range);

        return diagonals;
    }

    public List<Tile> GetDownRightDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(Environment.board.BoardRowSize, Environment.board.BoardColumnSize));
        int columnLimit = Mathf.Min(Environment.board.BoardColumnSize, origin.column + clampedRange + 1);
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1, column = origin.column + 1;
            row >= rowLimit && column < columnLimit; row--, column++)
        {
            diagonal.Add(Environment.board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    public List<Tile> GetDownLeftDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(Environment.board.BoardRowSize, Environment.board.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange - 1);
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1, column = origin.column - 1;
            row >= rowLimit && column >= columnLimit; row--, column--)
        {
            diagonal.Add(Environment.board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    public List<Tile> GetTopRightDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(Environment.board.BoardRowSize, Environment.board.BoardColumnSize));
        int columnLimit = Mathf.Min(Environment.board.BoardColumnSize, origin.column + clampedRange + 1);
        int rowLimit = Mathf.Min(Environment.board.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1, column = origin.column + 1;
            row < rowLimit && column < columnLimit; row++, column++)
        {
            diagonal.Add(Environment.board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    public List<Tile> GetTopLeftDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(Environment.board.BoardRowSize, Environment.board.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange);
        int rowLimit = Mathf.Min(Environment.board.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1, column = origin.column - 1;
            row < rowLimit && column >= columnLimit; row++, column--)
        {
            diagonal.Add(Environment.board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    public Verticals GetVerticalsFrom(TileCoordinates origin, PieceColor pieceColor, int range = 8)
    {
        Verticals verticals = new();

        verticals.frontVerticals = (pieceColor == PieceColor.White) ? GetFrontVerticals(origin, range) : GetBackVerticals(origin, range);
        verticals.backVerticals = (pieceColor == PieceColor.White) ? GetBackVerticals(origin, range) : GetFrontVerticals(origin, range);

        return verticals;
    }

    public List<Tile> GetFrontVerticals(TileCoordinates origin, int range = 8)
    {
        List<Tile> verticals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(Environment.board.BoardRowSize));
        int rowLimit = Mathf.Min(Environment.board.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1; row < rowLimit; row++)
        {
            verticals.Add(Environment.board.GetTiles()[row][origin.column]);
        }

        return verticals;
    }

    public List<Tile> GetBackVerticals(TileCoordinates origin, int range = 8)
    {
        List<Tile> verticals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(Environment.board.BoardRowSize));
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1; row >= rowLimit; row--)
        {
            verticals.Add(Environment.board.GetTiles()[row][origin.column]);
        }

        return verticals;
    }

    public Horizontals GetHorizontalsFrom(TileCoordinates origin, PieceColor pieceColor, int range = 8)
    {
        Horizontals horizontals = new();

        horizontals.rightHorizontals = (pieceColor == PieceColor.White) ? GetRightHorizontals(origin, range) : GetLeftHorizontals(origin, range);
        horizontals.leftHorizontals = (pieceColor == PieceColor.White) ? GetLeftHorizontals(origin, range) : GetRightHorizontals(origin, range);

        return horizontals;
    }

    public List<Tile> GetLeftHorizontals(TileCoordinates origin, int range = 8)
    {
        List<Tile> horizontals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(Environment.board.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange);

        for (int column = origin.column - 1; column >= columnLimit; column--)
        {
            horizontals.Add(Environment.board.GetTiles()[origin.row][column]);
        }

        return horizontals;
    }

    public List<Tile> GetRightHorizontals(TileCoordinates origin, int range = 8)
    {
        List<Tile> horizontals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(Environment.board.BoardColumnSize));
        int columnLimit = Mathf.Min(Environment.board.BoardColumnSize, origin.column + clampedRange + 1);

        for (int column = origin.column + 1; column < columnLimit; column++)
        {
            horizontals.Add(Environment.board.GetTiles()[origin.row][column]);
        }

        return horizontals;
    }

    public Tile GetKingTile(PieceColor color) 
    {
        foreach (var row in Environment.board.GetTiles())
        {
            var kingTile = row.Find(t => t.OccupiedBy is King king && king.pieceColor == color);
            if (kingTile != null)
                return kingTile;
        }

        return null;
    }

    public Tile[] GetRookTiles(PieceColor color) 
    {
        List<Tile> tiles = new();
        foreach (var row in Environment.board.GetTiles())
        {
            var rookTile = row.Where(t => t.OccupiedBy is Rook rook && rook.pieceColor == color);
            if (rookTile != null && rookTile.ToList().Count > 0)
            {
                tiles.AddRange(rookTile);
                if (tiles.Count >= 2) break;
            }
        }

        return tiles.ToArray();
    }

    public Piece[] GetAllPieces(PieceColor pieceColor) 
    {
        List<Piece> pieces = new();
        foreach(var tileList in Environment.board.GetTiles()) 
        {
            foreach (var tile in tileList)
            {
                if (tile.IsOccupied && tile.OccupiedBy.pieceColor == pieceColor)
                    pieces.Add(tile.OccupiedBy);
            }
        }

        return pieces.ToArray();
    }

    public string GetFEN() 
    {
        return new FEN().GetFENFrom(Environment.board);
    }

    public void Clear()
    {
        foreach (var row in Environment.board.tiles)
        {
            foreach (var tile in row)
            {
                tile.DeOccupy();
            }
        }
    }

    public IEnvironmentable Copy(Environment environment)
    {
        return new BoardManager(environment);
    }
}

