using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesCapturedController : MonoBehaviour
{
    private void Awake()
    {
        TurnManager.onPieceCaptured += PieceCaptured;
    }

    private void PieceCaptured(Piece piece)
    {
        Destroy(piece.gameObject);
    }
}
