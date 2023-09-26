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
    private Piece piece;

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

        piece = new GameObject("Rook", typeof(Rook)).GetComponent<Piece>();

        var primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        primitive.transform.SetParent(piece.transform, false);

        var pawn2 = new GameObject("EnemyPawn", typeof(Pawn)).GetComponent<Piece>();
        pawn2.MoveTo(Board.GetTiles()[3][4]);
        pawn2.pieceColor = PieceColor.Black;

        var primitive2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        primitive2.transform.SetParent(pawn2.transform, false);
        primitive2.GetComponent<Renderer>().material.color = Color.black;

        var pawn3 = new GameObject("MyPawn", typeof(Pawn)).GetComponent<Piece>();
        pawn3.MoveTo(Board.GetTiles()[3][5]);
        pawn3.pieceColor = PieceColor.White;

        var primitive3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        primitive3.transform.SetParent(pawn3.transform, false);
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

            piece.MoveTo(tile);

            var moves = piece.GetPossibleMoves();
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
