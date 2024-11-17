public class Bishop : SlidingPieces
{
    public Bishop(Board board) : base(board)
    {
    }

    public override void GenerateBitBoard()
    {
        throw new System.NotImplementedException();
    }

    public override Move[] GetMoves()
    {
        return GetDiagonalMoves().ToArray();
    }
}
