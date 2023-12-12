using System.Collections.Generic;

public class MoveChecker : IEnvironmentable
{
    public Environment Environment { get; }

    private CheckChecker checkChecker;

    public MoveChecker(Environment env) 
    {
        Environment = env;
        checkChecker = new();
    }

    public IEnvironmentable Copy(Environment env)
    {
        return new MoveChecker(env);
    }

    public Move[] GetLegalMoves(Move[] moves)
    {
        var returningMoves = FilterCheckMoves(moves);

        return returningMoves.ToArray();
    }

    private List<Move> FilterCheckMoves(Move[] moves)
    {
        List<Move> validMoves = new List<Move>();
        PieceColor turn = Environment.turnManager.ActualTurn;
        foreach (var move in moves)
        {
            var env = Environment.Copy();
            env.turnManager.DoMove(move);

            if (checkChecker.IsCheck(env, turn) is false)
                validMoves.Add(move);
        }

        return validMoves;
    }

    public bool IsCheckMate() 
    {
        return checkChecker.IsCheck(Environment, Environment.turnManager.ActualTurn) && HasAnyMove() is false;
    }

    public bool HasAnyMove()
    {
        return GetAllPossibleMoves().Length > 0;
    }

    public Move[] GetAllPossibleMoves() 
    {
        Piece[] pieces = Environment.boardManager.GetAllPieces(Environment.turnManager.ActualTurn);
        List<Move> moves = new();
        foreach (var piece in pieces)
        {
            var legalMoves = GetLegalMoves(piece.GetMoves());
            moves.AddRange(legalMoves);
        }

        return moves.ToArray();
    }
}
