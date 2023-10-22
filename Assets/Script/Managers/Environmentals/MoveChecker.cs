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
            env.turnManager.SetMove(move);

            if (checkChecker.IsCheck(env, turn) is false)
                validMoves.Add(move);
        }

        return validMoves;
    }
}
