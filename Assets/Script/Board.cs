using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private Tile tileprefab;
    [SerializeField]
    private Vector2 tilesOffset = new Vector2(10, 10);

    public int BoardRowSize => tiles.GetLength(0);
    public int BoardColumnSize => tiles.GetLength(1);

    private Tile[,] tiles = new Tile[8, 8];

    private List<List<Tile>> tilesList = new List<List<Tile>>();
    private List<List<Tile>> tilesListTransposed = new List<List<Tile>>();
    private List<List<Tile>> tilesDiagonals = new List<List<Tile>>();

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

                if (tilesListTransposed.Count - 1 < column)
                    tilesListTransposed.Add(new List<Tile>());

                tilesListTransposed[column].Add(tile);
            }

            tilesList.Add(tileRow);

            tilesDiagonals.Add(firstDiagonal);
            tilesDiagonals.Add(secondDiagonal);
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

    public List<List<Tile>> GetDiagonals()
    {
        return tilesDiagonals;
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
    public Diagonals GetDiagonalsFrom(TileCoordinates origin, PieceColor color) 
    {
        Diagonals diagonals = new();

        diagonals.topLeftDiagonals = (color == PieceColor.White) ? GetTopLeftDiagonals(origin) : GetDownRightDiagonals(origin);
        diagonals.topRightDiagonals = (color == PieceColor.White) ? GetTopRightDiagonals(origin) : GetDownLeftDiagonals(origin);
        diagonals.downLeftDiagonals = (color == PieceColor.White) ? GetDownLeftDiagonals(origin) : GetTopRightDiagonals(origin);
        diagonals.downRightDiagonals = (color == PieceColor.White) ? GetDownRightDiagonals(origin) : GetTopLeftDiagonals(origin);

        return diagonals;
    }

    private List<Tile> GetDownRightDiagonals(TileCoordinates origin)
    {
        List<Tile> diagonal = new();

        for (int row = origin.row-1, column = origin.column+1;
            row >= 0 && column < BoardColumnSize; row--, column++)
        {
            diagonal.Add(tiles[row, column]);
        }

        return diagonal;
    }

    private List<Tile> GetDownLeftDiagonals(TileCoordinates origin)
    {
        List<Tile> diagonal = new();

        for (int row = origin.row-1, column = origin.column-1;
            row >= 0 && column >= 0; row--, column--)
        {
            diagonal.Add(tiles[row, column]);
        }

        return diagonal;
    }

    private List<Tile> GetTopRightDiagonals(TileCoordinates origin)
    {
        List<Tile> diagonal = new();

        for (int row = origin.row + 1, column = origin.column + 1;
            row < BoardRowSize && column < BoardColumnSize; row++, column++)
        {
            diagonal.Add(tiles[row, column]);
        }

        return diagonal;
    }

    private List<Tile> GetTopLeftDiagonals(TileCoordinates origin)
    {
        List<Tile> diagonal = new();

        for (int row = origin.row + 1, column = origin.column - 1;
            row < BoardRowSize && column >= 0; row++, column--)
        {
            diagonal.Add(tiles[row, column]);
        }

        return diagonal;
    }

    public Verticals GetVerticalTilesFrom(TileCoordinates origin, PieceColor pieceColor)
    {
        Verticals verticals = new();

        verticals.frontVerticals = (pieceColor == PieceColor.White) ? GetFrontVerticals(origin) : GetBackVerticals(origin);
        verticals.backVerticals = (pieceColor == PieceColor.Black) ? GetBackVerticals(origin) : GetFrontVerticals(origin);

        return verticals;
    }

    private List<Tile> GetFrontVerticals(TileCoordinates origin)
    {
        List<Tile> verticals = new();

        for (int row = origin.row + 1; row < BoardRowSize; row++)
        {
            verticals.Add(tiles[row, origin.column]);
        }

        return verticals;
    }

    private List<Tile> GetBackVerticals(TileCoordinates origin)
    {
        List<Tile> verticals = new();

        for (int row = origin.row - 1; row >= 0; row--)
        {
            verticals.Add(tiles[row, origin.column]);
        }

        return verticals;
    }

    public Horizontals GetHorizontalTilesFrom(TileCoordinates origin)
    {
        return null;
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

