using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class VisualPiece : MonoBehaviour
{
    public Piece actualPiece { get; private set; }
    private Material baseMaterial;
    private new Renderer renderer;

    private bool isBaseMaterial;

    public void SetTilePosition(Tile tile) 
    {
        transform.position = tile.visualTile.transform.position;
        transform.SetParent(tile.visualTile.transform);
    }

    public void SetPiece(Piece piece, PiecesConfig config) 
    {
        actualPiece = piece;
        name = piece.name;

        renderer = GetComponent<Renderer>();
        baseMaterial = (piece.pieceColor == PieceColor.White) ? config.lightMaterial : config.darkMaterial;
        renderer.sharedMaterial = baseMaterial;
        isBaseMaterial = true;
    }

    public void SetColor(Color color) 
    {
        if(isBaseMaterial)
            renderer.sharedMaterial = new Material(baseMaterial);

        isBaseMaterial = false;
        renderer.sharedMaterial.color = color;
    }

    public void ResetMaterial() 
    {
        renderer.sharedMaterial = baseMaterial;
        isBaseMaterial = true;
    }
}
