using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private Tile tileprefab;
    [SerializeField]
    private Vector2 tilesOffset = new Vector2(10, 10);

    private Tile[,] tiles = new Tile[8, 8];

    private List<List<Tile>> tilesList = new List<List<Tile>>();
    private List<List<Tile>> tilesListTransposed = new List<List<Tile>>();
    private List<List<Tile>> tilesDiagonals = new List<List<Tile>>();

    private void Start()
    {
        StartBoard();
    }

    private void StartBoard()
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
