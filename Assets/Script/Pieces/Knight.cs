using System.Collections.Generic;

public class Knight : Piece
{
    public Knight(Board board) : base(board)
    {
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new List<Move>();
        moves.AddRange(GetMovesFromHorizontals());
        moves.AddRange(GetMovesFromVertical());

        return moves.ToArray();
    }

    private List<Move> GetMovesFromHorizontals() 
    {
        List<Move> moves = new List<Move>();

        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);
        moves.AddRange(GetMovesFromHorizontal(horizontals.rightHorizontals));
        moves.AddRange(GetMovesFromHorizontal(horizontals.leftHorizontals));

        return moves;
    }

    private List<Move> GetMovesFromHorizontal(List<TileCoordinates> horizontal) 
    {
        List<Move> moves = new List<Move>();

        if (horizontal.Count >= 2)
        {
            var edgeCoord = horizontal[1];
            var edge = Board.tiles[edgeCoord.row][edgeCoord.column];
            var edgeVerticals = edge.GetVerticalsByColor(pieceColor);

            if (edgeVerticals.frontVerticals.Count > 0)
            {
                var checkedFront = CheckForBlockingSquares(edgeVerticals.frontVerticals.GetRange(0, 1));
                moves.AddRange(CreateMovesFromSegment(checkedFront));
            }

            if (edgeVerticals.backVerticals.Count > 0)
            {
                var checkedBack = CheckForBlockingSquares(edgeVerticals.backVerticals.GetRange(0, 1));
                moves.AddRange(CreateMovesFromSegment(checkedBack));
            }
        }

        return moves;
    }

    private List<Move> GetMovesFromVertical()
    {
        List<Move> moves = new List<Move>();

        var verticals = actualTile.GetVerticalsByColor(pieceColor);

        moves.AddRange(GetMovesFromVertical(verticals.frontVerticals));
        moves.AddRange(GetMovesFromVertical(verticals.backVerticals));

        return moves;
    }

    private List<Move> GetMovesFromVertical(List<TileCoordinates> vertical)
    {
        List<Move> moves = new List<Move>();

        if (vertical.Count >= 2)
        {
            var edgeCoord = vertical[1];
            var edge = Board.tiles[edgeCoord.row][edgeCoord.column];
            var edgeHorizontals = edge.GetHorizontalsByColor(pieceColor);

            if (edgeHorizontals.leftHorizontals.Count > 0)
            {
                var checkedLeft = CheckForBlockingSquares(edgeHorizontals.leftHorizontals.GetRange(0, 1));
                moves.AddRange(CreateMovesFromSegment(checkedLeft));
            }

            if (edgeHorizontals.rightHorizontals.Count > 0)
            {
                var checkedRight = CheckForBlockingSquares(edgeHorizontals.rightHorizontals.GetRange(0, 1));
                moves.AddRange(CreateMovesFromSegment(checkedRight));
            }
        }

        return moves;
    }

    public override void GenerateBitBoard()
    {
        int currentIndex = actualTile.Index;
        Bitboard bitboard = new Bitboard();

        //topLeft move
        int topLeftIndex = currentIndex + 15;
        if (currentIndex % 8 > 0 && topLeftIndex < 64)
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, Board.GetTileByIndex(topLeftIndex));

        //topRight move
        int topRightIndex = currentIndex + 17;
        if (currentIndex % 8 < 7 && topRightIndex < 64)
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, Board.GetTileByIndex(topRightIndex));

        //leftTop move
        int leftTopIndex = currentIndex + 6;
        if (currentIndex % 8 > 1 && leftTopIndex < 64)
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, Board.GetTileByIndex(leftTopIndex));

        //leftDown move
        int leftDownIndex = currentIndex - 10;
        if (currentIndex % 8 > 1 && leftDownIndex >= 0)
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, Board.GetTileByIndex(leftDownIndex));

        //rightTop move
        int rightTopIndex = currentIndex + 10;
        if (currentIndex % 8 < 6 && rightTopIndex < 64)
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, Board.GetTileByIndex(rightTopIndex));

        //rightDown move
        int rightDownIndex = currentIndex - 6;
        if (currentIndex % 8 < 6 && rightDownIndex >= 0)
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, Board.GetTileByIndex(rightDownIndex));

        //DownLeft move
        int downLeftIndex = currentIndex - 17;
        if (currentIndex % 8 > 0 && downLeftIndex >= 0)
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, Board.GetTileByIndex(downLeftIndex));

        //DownLeft move
        int downRightIndex = currentIndex - 15;
        if (currentIndex % 8 < 7 && downRightIndex >= 0)
            AddToBitboardIfNotOccupiedByFriend(ref bitboard, Board.GetTileByIndex(downRightIndex));

        AttackingSquares = bitboard;
    }

    private void AddToBitboardIfNotOccupiedByFriend(ref Bitboard bitboard, Tile tile) 
    {
        if (tile.IsOccupied && tile.OccupiedBy.pieceColor == pieceColor) return;

        bitboard.Add(tile.Bitboard);
    }
}
