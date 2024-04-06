using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesCapturedController : ManagerHelper
{
    public void PieceCaptured(Piece piece)
    {
        Destroy(piece.visualPiece.gameObject);
    }
}
