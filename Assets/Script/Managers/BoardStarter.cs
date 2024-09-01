using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStarter : ManagerHelper
{
    [SerializeField]
    private VisualTile tileprefab;
    [SerializeField]
    private Vector2 tilesOffset = new Vector2(10, 10);

    private Board actualBoard;

    public Board StartNewBoard()
    {
        Board board = new Board(8, 8);
        StartBoard(board);
        return board;
    }

    public void StartBoard(Board board)
    {
        actualBoard = board;
        for (int row = 0; row < board.BoardRowSize; row++)
        {
            List<Tile> tileRow = new();
            for (int column = 0; column < board.BoardColumnSize; column++)
            {
                float x = transform.position.x + tilesOffset.x * column;
                float y = transform.position.y;
                float z = transform.position.z + tilesOffset.y * row;

                VisualTile visualTile = Instantiate(tileprefab, new Vector3(x, y, z), Quaternion.identity);
                visualTile.transform.SetParent(transform);

                bool isLightSquare = (row + column) % 2 == 0;
                if (isLightSquare)
                    visualTile.SetLightColor();
                else
                    visualTile.SetDarkColor();

                visualTile.name = $"Tile({row},{column})";


                Tile tile = new(board);
                tile.TilePosition = new TileCoordinates(row, column);
                tile.SetVisual(visualTile);

                tileRow.Add(tile);
            }

            board.tiles.Add(tileRow);
        }

        SetBoardRelations();
    }

    private void SetBoardRelations()
    {
        foreach(var row in actualBoard.tiles) 
        {
            foreach(var tile in row) 
            {
                tile.SetDiagonals(GetDiagonalsFrom(tile.TilePosition));
                tile.SetVerticals(GetVerticalsFrom(tile.TilePosition));
                tile.SetHorizontals(GetHorizontalsFrom(tile.TilePosition));
            }
        }
    }

    public Diagonals GetDiagonalsFrom(TileCoordinates origin)
    {
        Diagonals diagonals = new();

        diagonals.topLeftDiagonals = GetTopLeftDiagonals(origin);
        diagonals.topRightDiagonals = GetTopRightDiagonals(origin);
        diagonals.downLeftDiagonals = GetDownLeftDiagonals(origin);
        diagonals.downRightDiagonals = GetDownRightDiagonals(origin);

        return diagonals;
    }

    public List<TileCoordinates> GetDownRightDiagonals(TileCoordinates origin, int range = 8)
    {
        List<TileCoordinates> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(actualBoard.BoardRowSize, actualBoard.BoardColumnSize));
        int columnLimit = Mathf.Min(actualBoard.BoardColumnSize, origin.column + clampedRange + 1);
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1, column = origin.column + 1;
            row >= rowLimit && column < columnLimit; row--, column++)
        {
            diagonal.Add(new TileCoordinates(row, column));
        }

        return diagonal;
    }

    public List<TileCoordinates> GetDownLeftDiagonals(TileCoordinates origin, int range = 8)
    {
        List<TileCoordinates> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(actualBoard.BoardRowSize, actualBoard.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange - 1);
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1, column = origin.column - 1;
            row >= rowLimit && column >= columnLimit; row--, column--)
        {
            diagonal.Add(new TileCoordinates(row, column));
        }

        return diagonal;
    }

    public List<TileCoordinates> GetTopRightDiagonals(TileCoordinates origin, int range = 8)
    {
        List<TileCoordinates> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(actualBoard.BoardRowSize, actualBoard.BoardColumnSize));
        int columnLimit = Mathf.Min(actualBoard.BoardColumnSize, origin.column + clampedRange + 1);
        int rowLimit = Mathf.Min(actualBoard.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1, column = origin.column + 1;
            row < rowLimit && column < columnLimit; row++, column++)
        {
            diagonal.Add(new TileCoordinates(row, column));
        }

        return diagonal;
    }

    public List<TileCoordinates> GetTopLeftDiagonals(TileCoordinates origin, int range = 8)
    {
        List<TileCoordinates> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(actualBoard.BoardRowSize, actualBoard.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange);
        int rowLimit = Mathf.Min(actualBoard.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1, column = origin.column - 1;
            row < rowLimit && column >= columnLimit; row++, column--)
        {
            diagonal.Add(new TileCoordinates(row, column));
        }

        return diagonal;
    }

    public Verticals GetVerticalsFrom(TileCoordinates origin)
    {
        Verticals verticals = new();

        verticals.frontVerticals = GetFrontVerticals(origin);
        verticals.backVerticals = GetBackVerticals(origin);

        return verticals;
    }

    public List<TileCoordinates> GetFrontVerticals(TileCoordinates origin, int range = 8)
    {
        List<TileCoordinates> verticals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(actualBoard.BoardRowSize));
        int rowLimit = Mathf.Min(actualBoard.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1; row < rowLimit; row++)
        {
            verticals.Add(new TileCoordinates(row, origin.column));
        }

        return verticals;
    }

    public List<TileCoordinates> GetBackVerticals(TileCoordinates origin, int range = 8)
    {
        List<TileCoordinates> verticals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(actualBoard.BoardRowSize));
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1; row >= rowLimit; row--)
        {
            verticals.Add(new TileCoordinates(row, origin.column));
        }

        return verticals;
    }

    public Horizontals GetHorizontalsFrom(TileCoordinates origin)
    {
        Horizontals horizontals = new();

        horizontals.rightHorizontals = GetRightHorizontals(origin);
        horizontals.leftHorizontals = GetLeftHorizontals(origin);

        return horizontals;
    }

    public List<TileCoordinates> GetLeftHorizontals(TileCoordinates origin, int range = 8)
    {
        List<TileCoordinates> horizontals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(actualBoard.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange);

        for (int column = origin.column - 1; column >= columnLimit; column--)
        {
            horizontals.Add(new TileCoordinates(origin.row, column));
        }

        return horizontals;
    }

    public List<TileCoordinates> GetRightHorizontals(TileCoordinates origin, int range = 8)
    {
        List<TileCoordinates> horizontals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(actualBoard.BoardColumnSize));
        int columnLimit = Mathf.Min(actualBoard.BoardColumnSize, origin.column + clampedRange + 1);

        for (int column = origin.column + 1; column < columnLimit; column++)
        {
            horizontals.Add(new TileCoordinates(origin.row, column));
        }

        return horizontals;
    }
}
