using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStarter : MonoBehaviour
{
    [SerializeField]
    private VisualTile tileprefab;
    [SerializeField]
    private Vector2 tilesOffset = new Vector2(10, 10);

    public Board StartNewBoard()
    {
        Board board = new Board(8, 8);
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
}
