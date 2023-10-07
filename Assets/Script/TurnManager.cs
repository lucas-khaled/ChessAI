using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TurnManager
{
    public static PieceColor ActualTurn { get; private set; } = PieceColor.White;

    private static List<Move> moves = new List<Move>();

    public static Action<PieceColor> onTurnMade;
    public static Action<Piece> onPieceCaptured;

    public static void SetMove(Move move) 
    {
        move.Do();

        if(move.capture != null)
            onPieceCaptured?.Invoke(move.capture);

        onTurnMade?.Invoke(ActualTurn);

        moves.Add(move);
        ActualTurn = (ActualTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
    }
}
