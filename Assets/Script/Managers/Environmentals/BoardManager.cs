using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : IEnviromentableManager
{
    private Environment environment;

    public BoardManager(Environment environment) 
    {
        this.environment = environment;
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

    private List<Tile> GetDownRightDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(environment.board.BoardRowSize, environment.board.BoardColumnSize));
        int columnLimit = Mathf.Min(environment.board.BoardColumnSize, origin.column + clampedRange + 1);
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1, column = origin.column + 1;
            row >= rowLimit && column < columnLimit; row--, column++)
        {
            diagonal.Add(environment.board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    private List<Tile> GetDownLeftDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(environment.board.BoardRowSize, environment.board.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange - 1);
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1, column = origin.column - 1;
            row >= rowLimit && column >= columnLimit; row--, column--)
        {
            diagonal.Add(environment.board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    private List<Tile> GetTopRightDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(environment.board.BoardRowSize, environment.board.BoardColumnSize));
        int columnLimit = Mathf.Min(environment.board.BoardColumnSize, origin.column + clampedRange + 1);
        int rowLimit = Mathf.Min(environment.board.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1, column = origin.column + 1;
            row < rowLimit && column < columnLimit; row++, column++)
        {
            diagonal.Add(environment.board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    private List<Tile> GetTopLeftDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(environment.board.BoardRowSize, environment.board.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange);
        int rowLimit = Mathf.Min(environment.board.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1, column = origin.column - 1;
            row < rowLimit && column >= columnLimit; row++, column--)
        {
            diagonal.Add(environment.board.GetTiles()[row][column]);
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

    private List<Tile> GetFrontVerticals(TileCoordinates origin, int range = 8)
    {
        List<Tile> verticals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(environment.board.BoardRowSize));
        int rowLimit = Mathf.Min(environment.board.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1; row < rowLimit; row++)
        {
            verticals.Add(environment.board.GetTiles()[row][origin.column]);
        }

        return verticals;
    }

    private List<Tile> GetBackVerticals(TileCoordinates origin, int range = 8)
    {
        List<Tile> verticals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(environment.board.BoardRowSize));
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1; row >= rowLimit; row--)
        {
            verticals.Add(environment.board.GetTiles()[row][origin.column]);
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

    private List<Tile> GetLeftHorizontals(TileCoordinates origin, int range = 8)
    {
        List<Tile> horizontals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(environment.board.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange);

        for (int column = origin.column - 1; column >= columnLimit; column--)
        {
            horizontals.Add(environment.board.GetTiles()[origin.row][column]);
        }

        return horizontals;
    }

    private List<Tile> GetRightHorizontals(TileCoordinates origin, int range = 8)
    {
        List<Tile> horizontals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(environment.board.BoardColumnSize));
        int columnLimit = Mathf.Min(environment.board.BoardColumnSize, origin.column + clampedRange + 1);

        for (int column = origin.column + 1; column < columnLimit; column++)
        {
            horizontals.Add(environment.board.GetTiles()[origin.row][column]);
        }

        return horizontals;
    }

    public void Clear()
    {
        foreach (var row in environment.board.tiles)
        {
            foreach (var tile in row)
            {
                tile.DeOccupy();
            }
        }
    }

    public IEnviromentableManager Virtualize(Environment environment)
    {
        return new BoardManager(environment);
    }
}

