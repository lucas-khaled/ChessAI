using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaker
{
    public static Action<Piece> onPieceCaptured;

    public static Move LastMove { get; private set; }
    List<Move> movesDone = new List<Move>();

    public void DoMove(Move move, bool isVirtual = false) 
    {
        if(move.from.IsOccupied is false && isVirtual is false) 
        {
            Debug.LogError($"The move is not valid [{nameof(MoveMaker)}]");
            return;
        }

        Piece movingPiece = move.from.OccupiedBy;
        move.from.DeOccupy();

        if (move.capture != null && isVirtual is false)
            onPieceCaptured?.Invoke(move.capture);

        move.to.Occupy(movingPiece);

        if (isVirtual is false)
        {
            movingPiece.SetTile(move.to);
            movesDone.Add(move);
            LastMove = move;
        }
    }
}
