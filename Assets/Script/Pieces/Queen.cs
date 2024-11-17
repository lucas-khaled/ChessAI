using System.Collections.Generic;

public class Queen : SlidingPieces
{
    public Queen(Board board) : base(board)
    {
    }

    public override void GenerateBitBoard()
    {
        throw new System.NotImplementedException();
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new();

        moves.AddRange(GetDiagonalMoves());
        moves.AddRange(GetVerticalMoves());
        moves.AddRange(GetHorizontalMoves());

        return moves.ToArray();
    }
}
