using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class VisualPiece : MonoBehaviour
{
    public Piece actualPiece { get; private set; }

    public void SetTilePosition(Tile tile) 
    {
        transform.position = tile.visualTile.transform.position;
        transform.SetParent(tile.visualTile.transform);

        name = $"{actualPiece.pieceColor} - {actualPiece.GetType().Name} ({tile.TilePosition.row},{tile.TilePosition.column})";
    }

    public void SetPiece(Piece piece, PiecesConfig config) 
    {
        actualPiece = piece;

        GetComponent<Renderer>().material = (piece.pieceColor == PieceColor.White) ? config.lightMaterial : config.darkMaterial;
    }
}
