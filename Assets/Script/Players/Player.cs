using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Player : IPlayer
{
    protected bool canPlay = false;
    protected PieceColor actualColor;
    protected GameManager manager;
    protected Action<Move> onMove;

    public Player(GameManager manager) 
    {
        this.manager = manager;
    }

    public virtual void EndGame()
    {
        canPlay = false;
    }

    public virtual void Init(PieceColor pieceColor)
    {
        actualColor = pieceColor;
    }

    public virtual void StartTurn(Action<Move> moveCallback)
    {
        canPlay = true;
        onMove = moveCallback;
    }
}
