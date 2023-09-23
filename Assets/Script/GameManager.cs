using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board board;

    private static GameManager instance;

    public static Board Board => instance.board;

    private List<Tile> tiles = new List<Tile>();
    private Tile selectedTile;
    private Piece pawn;

    private void Awake()
    {
        if(instance != null) 
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        if (board != null)
            board.StartBoard();

        pawn = new GameObject("Pawn", typeof(Pawn)).GetComponent<Piece>();

        var pawn2 = new GameObject("EnemyPawn", typeof(Pawn)).GetComponent<Piece>();
        pawn2.MoveTo(Board.GetTiles()[3][4]);
        pawn2.pieceColor = PieceColor.Black;
    }

    private void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask.GetMask("Tile"))) 
        {
            var tile = hit.collider.GetComponent<Tile>();

            if (tile == selectedTile) return;

            ReturnTilesColors();
            if(selectedTile != null)
                selectedTile.DeOccupy();

            selectedTile = tile;
            tiles.Clear();

            pawn.MoveTo(tile);

            var moves = pawn.GetPossibleMoves();
            PaintTiles(moves.Select(x => x.to).ToList());

            //List<List<Tile>> allTiles = board.GetDiagonalsFrom(tile.TilePosition, PieceColor.White);

            //PaintAllDiagonals(allTiles);
            
            selectedTile.Paint(Color.blue);
        }
    }

    private void PaintAllDiagonals(List<List<Tile>> diagonals) 
    {
        foreach (var diagonal in diagonals)
        {
            PaintTiles(diagonal);
        }
    }

    private void PaintTiles(List<Tile> diagonal) 
    {
        foreach (var dTile in diagonal)
            dTile.Paint(Color.yellow);
        
        tiles.AddRange(diagonal);
    }

    private void ReturnTilesColors() 
    {
        if(selectedTile != null)
            selectedTile.Paint(selectedTile.ActualColor);

        foreach(var tile in tiles) 
        {
            tile.Paint(tile.ActualColor);
        }
    }
}
