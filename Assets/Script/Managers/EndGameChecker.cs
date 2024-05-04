using System.Linq;

public struct EndGameInfo
{
    public bool hasEnded;
    public bool isCheckMate;
    public DrawType drawType;
}

public class EndGameChecker
{
    private Environment environment;

    public EndGameChecker(Environment environment) 
    {
        SetEnvironment(environment);
    }

    public void SetEnvironment(Environment environment) 
    {
        this.environment = environment;
    }

    public EndGameInfo CheckEnd()
    {
        EndGameInfo info = new EndGameInfo();
        if (IsCheckMate())
            //GameManager.UIManager.ShowCheckmateMessage(lastTurnColor);
            info.hasEnded = info.isCheckMate = true;
        else if (HasDraw(out info.drawType)) 
            info.hasEnded = true;
            //GameManager.UIManager.ShowDrawMessage(drawType);

        return info;
    }

    public bool IsCheckMate() 
    {
        return environment.moveChecker.IsCheckMate();
    }

    public bool HasDraw() 
    {
        DrawType _;
        return HasDraw(out _);
    }

    public bool HasDraw(out DrawType drawType) 
    {
        if (IsStaleMateDraw())
        {
            drawType = DrawType.Stalemate;
            return true;
        }
        if (Is50MoveDraw()) 
        {
            drawType = DrawType.RuleOf50Moves;
            return true;
        }
        if (IsThreefoldDraw()) 
        {
            drawType = DrawType.ThreefoldDraw;
            return true;
        }
        if (IsInsuficcientMaterialDraw()) 
        {
            drawType = DrawType.Deadposition;
            return true;
        }

        drawType = DrawType.None;
        return false;
    }

    private bool Is50MoveDraw()
    {
        return environment.turnManager.halfMoves >= 50;
    }

    private bool IsStaleMateDraw()
    {
        return environment.moveChecker.HasAnyMove() is false;
    }

    private bool IsThreefoldDraw()
    {
        var moves = environment.turnManager.moves;
        FEN fen = moves[moves.Count-1].fen;
        int count = environment.turnManager.moves.Count(x => x.fen.fullPositionsString == fen.fullPositionsString);

        return count >= 3;
    }

    private bool IsInsuficcientMaterialDraw() 
    {
        var pieces = environment.board.pieces;

        if (pieces.Count == 2) return true;

        var notKingPieces = pieces.Where(x => x is not King);

        if (notKingPieces.Any(x => x is Pawn || x is Queen || x is Rook)) return false;

        var notKingWhitePieces = notKingPieces.Where(x => x.pieceColor == PieceColor.White);
        var notKingBlackPieces = notKingPieces.Where(x => x.pieceColor == PieceColor.Black);

        return notKingWhitePieces.Count() < 2 && notKingBlackPieces.Count() < 2;
    }
}
