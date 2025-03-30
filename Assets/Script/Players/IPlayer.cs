using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    void Init(PieceColor pieceColor);
    void StartTurn(Action<Move> moveCallback);
    void EndGame();
}
