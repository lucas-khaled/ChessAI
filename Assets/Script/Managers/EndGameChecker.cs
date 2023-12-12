public class EndGameChecker
{
    public void DoCheck(PieceColor lastTurnColor) 
    {
        DrawType drawType;
        if (GameManager.environment.moveChecker.IsCheckMate())
        {
            GameManager.UIManager.ShowCheckmateMessage(lastTurnColor);
            SelectionManager.LockSelection();
        }
        else if (HasDraw(out drawType)) 
        {
            GameManager.UIManager.ShowDrawMessage(drawType);
            SelectionManager.LockSelection();
        }
    }

    public bool HasDraw(out DrawType drawType) 
    {
        if (IsStaleMateDraw())
        {
            drawType = DrawType.Stalemate;
            return true;
        }

        drawType = DrawType.None;
        return false;
    }

    private bool IsStaleMateDraw()
    {
        return GameManager.environment.moveChecker.HasAnyMove() is false;
    }
}

public enum DrawType 
{
    None,
    Stalemate,
    Deadposition,
    Repetition,
    Agreement,
    Move50
}
