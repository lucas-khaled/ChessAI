using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceColor
{
    White,
    Black
}

public abstract class Piece : MonoBehaviour
{
    public PieceColor pieceColor;

    protected Tile actualTile;
    protected bool isCaptured;
    protected int Row => actualTile.TilePosition.row;
    protected int Column => actualTile.TilePosition.column;
    protected bool IsWhite => pieceColor == PieceColor.White;

    public abstract Move[] GetPossibleMoves();
    public virtual void MoveTo(Tile tile) 
    {
        actualTile = tile;
        actualTile.Occupy(this);
    }

    public void Capture() 
    {
        actualTile.DeOccupy();
        actualTile = null;
        isCaptured = true;
    }

    public bool IsEnemyPiece(Piece piece) 
    {
        return pieceColor != piece.pieceColor;
    }
}
