using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private Tile tileprefab;
    [SerializeField]
    private Vector2 tilesOffset = new Vector2(10, 10);

    public int BoardRowSize => tiles.GetLength(0);
    public int BoardColumnSize => tiles.GetLength(1);

    private Tile[,] tiles = new Tile[8, 8];

    private List<List<Tile>> tilesList = new List<List<Tile>>();

    public void StartBoard()
    {
        List<Tile> firstDiagonal = new();
        List<Tile> secondDiagonal = new();

        for (int row = 0; row < tiles.GetLength(0); row++)
        {
            List<Tile> tileRow = new();
            for (int column = 0; column < tiles.GetLength(1); column++)
            {
                float x = transform.position.x + tilesOffset.x * column;
                float y = transform.position.y;
                float z = transform.position.z + tilesOffset.y * row;

                Tile tile = Instantiate(tileprefab, new Vector3(x, y, z), Quaternion.identity);
                tile.TilePosition = new TileCoordinates(row, column);
                tile.transform.SetParent(transform);
                tiles[row, column] = tile;

                bool isLightSquare = (row + column) % 2 == 0;
                if (isLightSquare)
                    tile.SetLightColor();
                else
                    tile.SetDarkColor();

                tile.name = $"Tile({row},{column})";

                tileRow.Add(tile);

                if (row == column)
                    firstDiagonal.Add(tile);

                if (column + row == tiles.GetLength(0) - 1)
                    secondDiagonal.Add(tile);
            }

            tilesList.Add(tileRow);
        }
    }

    public List<List<Tile>> GetTiles()
    {
        return tilesList;
    }

    public List<List<Tile>> GetTilesTransposed()
    {
        return tilesListTransposed;
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

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(BoardRowSize, BoardColumnSize));
        int columnLimit = Mathf.Min(BoardColumnSize, origin.column+ clampedRange + 1);
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row-1, column = origin.column+1;
            row >= rowLimit && column < columnLimit; row--, column++)
        {
            diagonal.Add(tiles[row, column]);
        }

        return diagonal;
    }

    private List<Tile> GetDownLeftDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(BoardRowSize, BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange - 1);
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row-1, column = origin.column-1;
            row >= rowLimit && column >= columnLimit; row--, column--)
        {
            diagonal.Add(tiles[row, column]);
        }

        return diagonal;
    }

    private List<Tile> GetTopRightDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(BoardRowSize, BoardColumnSize));
        int columnLimit = Mathf.Min(BoardColumnSize, origin.column + clampedRange + 1);
        int rowLimit = Mathf.Min(BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1, column = origin.column + 1;
            row < rowLimit && column < columnLimit; row++, column++)
        {
            diagonal.Add(tiles[row, column]);
        }

        return diagonal;
    }

    private List<Tile> GetTopLeftDiagonals(TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(BoardRowSize, BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange);
        int rowLimit = Mathf.Min(BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1, column = origin.column - 1;
            row < rowLimit && column >= columnLimit; row++, column--)
        {
            diagonal.Add(tiles[row, column]);
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

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(BoardRowSize));
        int rowLimit = Mathf.Min(BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1; row < rowLimit; row++)
        {
            verticals.Add(tiles[row, origin.column]);
        }

        return verticals;
    }

    private List<Tile> GetBackVerticals(TileCoordinates origin, int range = 8)
    {
        List<Tile> verticals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(BoardRowSize));
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1; row >= rowLimit; row--)
        {
            verticals.Add(tiles[row, origin.column]);
        }

        return verticals;
    }

    public Horizontals GetHorizontalsFrom(TileCoordinates origin, PieceColor pieceColor, int range = 8)
    {
        Horizontals horizontals = new();
        
        horizontals.rightHorizontals = (pieceColor == PieceColor.White) ?  GetRightHorizontals(origin, range) : GetLeftHorizontals(origin, range);
        horizontals.leftHorizontals = (pieceColor == PieceColor.White) ? GetLeftHorizontals(origin, range) : GetRightHorizontals(origin, range);

        return horizontals;
    }

    private List<Tile> GetLeftHorizontals(TileCoordinates origin, int range = 8) 
    {
        List<Tile> horizontals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange);

        for (int column = origin.column - 1; column >= columnLimit; column--)
        {
            horizontals.Add(tiles[origin.row, column]);
        }

        return horizontals;
    }

    private List<Tile> GetRightHorizontals(TileCoordinates origin, int range = 8)
    {
        List<Tile> horizontals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(BoardColumnSize));
        int columnLimit = Mathf.Min(BoardColumnSize, origin.column + clampedRange+1);

        for (int column = origin.column + 1; column < columnLimit; column++)
        {
            horizontals.Add(tiles[origin.row, column]);
        }

        return horizontals;
    }

    public void Clear()
    {
        foreach (var row in tilesList)
        {
            foreach (var tile in row)
            {
                tile.DeOccupy();
            }
        }
    }

#if UNITY_EDITOR
    public void DebugBoard()
    {
        string board = string.Empty;
        for (int row = tiles.GetLength(0) - 1; row >= 0; row--)
        {
            board += "[";
            for (int column = 0; column < tiles.GetLength(1); column++)
            {
                board += " " + tiles[column, row].OccupiedBy;
            }
            board += "]\n";
        }

        Debug.Log("<color=red>" + board + "</color>");
    }
#endif
}

public struct Diagonals 
{
    public List<Tile> topLeftDiagonals;
    public List<Tile> topRightDiagonals;
    public List<Tile> downLeftDiagonals;
    public List<Tile> downRightDiagonals;
}

public struct Verticals
{
    public List<Tile> frontVerticals;
    public List<Tile> backVerticals;
}

public struct Horizontals
{
    public List<Tile> leftHorizontals;
    public List<Tile> rightHorizontals;
}

