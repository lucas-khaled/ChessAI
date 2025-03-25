using System;
using System.Collections;

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

#if UNITY_WEBGL
    public virtual IEnumerator StartTurnRoutine(Action<Move> moveCallback)
    {
        canPlay = true;
        onMove = moveCallback;
        yield return null;
    }
#endif
}
