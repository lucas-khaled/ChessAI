using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesCapturedController : MonoBehaviour
{
    private void Awake()
    {
        //MoveMaker.onPieceCaptured += PieceCaptured;
    }

    private void PieceCaptured(Piece piece)
    {
        Destroy(piece.visualPiece.gameObject);
    }
}
