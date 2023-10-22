using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private VisualTile tileprefab;
    [SerializeField]
    private Vector2 tilesOffset = new Vector2(10, 10);

    public Board StartNewBoard() 
    {
        Board board = new Board(8,8);
        StartBoard(board);
        return board;
    }

    public void StartBoard(Board board)
    {
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


                Tile tile = new();
                tile.TilePosition = new TileCoordinates(row, column);
                tile.SetVisual(visualTile);

                tileRow.Add(tile);
            }

            board.tiles.Add(tileRow);
        }
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
    public Diagonals GetDiagonalsFrom(Board board,TileCoordinates origin, PieceColor color, int range = 8)
    {
        Diagonals diagonals = new();

        diagonals.topLeftDiagonals = (color == PieceColor.White) ? GetTopLeftDiagonals(board, origin, range) : GetDownRightDiagonals(board, origin, range);
        diagonals.topRightDiagonals = (color == PieceColor.White) ? GetTopRightDiagonals(board, origin, range) : GetDownLeftDiagonals(board, origin, range);
        diagonals.downLeftDiagonals = (color == PieceColor.White) ? GetDownLeftDiagonals(board, origin, range) : GetTopRightDiagonals(board, origin, range);
        diagonals.downRightDiagonals = (color == PieceColor.White) ? GetDownRightDiagonals(board, origin, range) : GetTopLeftDiagonals(board, origin, range);

        return diagonals;
    }

    private List<Tile> GetDownRightDiagonals(Board board, TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(board.BoardRowSize, board.BoardColumnSize));
        int columnLimit = Mathf.Min(board.BoardColumnSize, origin.column + clampedRange + 1);
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1, column = origin.column + 1;
            row >= rowLimit && column < columnLimit; row--, column++)
        {
            diagonal.Add(board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    private List<Tile> GetDownLeftDiagonals(Board board, TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(board.BoardRowSize, board.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange - 1);
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1, column = origin.column - 1;
            row >= rowLimit && column >= columnLimit; row--, column--)
        {
            diagonal.Add(board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    private List<Tile> GetTopRightDiagonals(Board board, TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(board.BoardRowSize, board.BoardColumnSize));
        int columnLimit = Mathf.Min(board.BoardColumnSize, origin.column + clampedRange + 1);
        int rowLimit = Mathf.Min(board.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1, column = origin.column + 1;
            row < rowLimit && column < columnLimit; row++, column++)
        {
            diagonal.Add(board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    private List<Tile> GetTopLeftDiagonals(Board board, TileCoordinates origin, int range = 8)
    {
        List<Tile> diagonal = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(board.BoardRowSize, board.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange);
        int rowLimit = Mathf.Min(board.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1, column = origin.column - 1;
            row < rowLimit && column >= columnLimit; row++, column--)
        {
            diagonal.Add(board.GetTiles()[row][column]);
        }

        return diagonal;
    }

    public Verticals GetVerticalsFrom(Board board, TileCoordinates origin, PieceColor pieceColor, int range = 8)
    {
        Verticals verticals = new();

        verticals.frontVerticals = (pieceColor == PieceColor.White) ? GetFrontVerticals(board, origin, range) : GetBackVerticals(board, origin, range);
        verticals.backVerticals = (pieceColor == PieceColor.White) ? GetBackVerticals(board, origin, range) : GetFrontVerticals(board, origin, range);

        return verticals;
    }

    private List<Tile> GetFrontVerticals(Board board, TileCoordinates origin, int range = 8)
    {
        List<Tile> verticals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(board.BoardRowSize));
        int rowLimit = Mathf.Min(board.BoardRowSize, origin.row + 1 + clampedRange);

        for (int row = origin.row + 1; row < rowLimit; row++)
        {
            verticals.Add(board.GetTiles()[row][origin.column]);
        }

        return verticals;
    }

    private List<Tile> GetBackVerticals(Board board, TileCoordinates origin, int range = 8)
    {
        List<Tile> verticals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(board.BoardRowSize));
        int rowLimit = Mathf.Max(0, origin.row - clampedRange);

        for (int row = origin.row - 1; row >= rowLimit; row--)
        {
            verticals.Add(board.GetTiles()[row][origin.column]);
        }

        return verticals;
    }

    public Horizontals GetHorizontalsFrom(Board board, TileCoordinates origin, PieceColor pieceColor, int range = 8)
    {
        Horizontals horizontals = new();

        horizontals.rightHorizontals = (pieceColor == PieceColor.White) ? GetRightHorizontals(board,origin, range) : GetLeftHorizontals(board,origin, range);
        horizontals.leftHorizontals = (pieceColor == PieceColor.White) ? GetLeftHorizontals(board, origin, range) : GetRightHorizontals(board, origin, range);

        return horizontals;
    }

    private List<Tile> GetLeftHorizontals(Board board, TileCoordinates origin, int range = 8)
    {
        List<Tile> horizontals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(board.BoardColumnSize));
        int columnLimit = Mathf.Max(0, origin.column - clampedRange);

        for (int column = origin.column - 1; column >= columnLimit; column--)
        {
            horizontals.Add(board.GetTiles()[origin.row][column]);
        }

        return horizontals;
    }

    private List<Tile> GetRightHorizontals(Board board, TileCoordinates origin, int range = 8)
    {
        List<Tile> horizontals = new();

        int clampedRange = Mathf.Clamp(range, 0, Mathf.Max(board.BoardColumnSize));
        int columnLimit = Mathf.Min(board.BoardColumnSize, origin.column + clampedRange + 1);

        for (int column = origin.column + 1; column < columnLimit; column++)
        {
            horizontals.Add(board.GetTiles()[origin.row][column]);
        }

        return horizontals;
    }

    public void Clear(Board board)
    {
        foreach (var row in board.tiles)
        {
            foreach (var tile in row)
            {
                tile.DeOccupy();
            }
        }
    }
}

