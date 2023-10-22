using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager
{
    public PieceColor ActualTurn { get; private set; } = PieceColor.White;

    public List<Move> moves  { get; private set; } = new List<Move>();

    public Action<PieceColor> onTurnMade;

    public Move LastMove => (moves.Count > 0) ? moves[moves.Count - 1] : null;

    public void SetMove(Move move) 
    {
        onTurnMade?.Invoke(ActualTurn);

        moves.Add(move);
        ActualTurn = (ActualTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
    }
}
