using UnityEngine;

public class CenterControlHeuristic : Heuristic
{
    private PawnCenterControlHandler pawnHandler = new();
    private RookCenterControlHandler rookHandler = new();
    private QueenCenterControlHandler queenHandler = new();
    private BishopCenterControlHandler bishopHandler = new();
    private KnightCenterControlHandler knightHandler = new();

    private const float QUEEN_WEIGHT = 0.7f;
    private const float ROOK_WEIGHT = 0.75f;
    private const float BISHOP_WEIGHT = 0.85f;
    private const float KNIGHT_WEIGHT = 0.85f;
    private const float PAWN_WEIGHT = 1f;


    public CenterControlHeuristic(GameManager manager, float weight) : base(manager, weight)
    {
    }

    public override float GetHeuristic(Board board)
    {
        float blackPoints = GetBlackPoints(board);
        float whitePoints = GetWhitePoints(board);

        var finalHeuristic = (whitePoints - blackPoints) * weight;
        return finalHeuristic;
    }

    private float GetWhitePoints(Board board) 
    {
        float points = 0;
        foreach (var piece in board.piecesHolder.whiteQueens)
            points = queenHandler.GetControlAmount(piece) * QUEEN_WEIGHT;

        foreach (var piece in board.piecesHolder.whiteBishops)
            points = bishopHandler.GetControlAmount(piece) * BISHOP_WEIGHT;

        foreach (var piece in board.piecesHolder.whiteKnights)
            points = knightHandler.GetControlAmount(piece) * KNIGHT_WEIGHT;

        foreach (var piece in board.piecesHolder.whiteRooks)
            points = rookHandler.GetControlAmount(piece) * ROOK_WEIGHT;

        foreach (var piece in board.piecesHolder.whitePawns)
            points = pawnHandler.GetControlAmount(piece) * PAWN_WEIGHT;

        return points;
    }

    private float GetBlackPoints(Board board)
    {
        float points = 0;
        foreach (var piece in board.piecesHolder.blackQueens)
            points = queenHandler.GetControlAmount(piece) * QUEEN_WEIGHT;

        foreach (var piece in board.piecesHolder.blackBishops)
            points = bishopHandler.GetControlAmount(piece) * BISHOP_WEIGHT;

        foreach (var piece in board.piecesHolder.blackKnights)
            points = knightHandler.GetControlAmount(piece) * KNIGHT_WEIGHT;

        foreach (var piece in board.piecesHolder.blackRooks)
            points = rookHandler.GetControlAmount(piece) * ROOK_WEIGHT;

        foreach (var piece in board.piecesHolder.blackPawns)
            points = pawnHandler.GetControlAmount(piece) * PAWN_WEIGHT;

        return points;
    }

    private interface IPieceCenterControlHandler 
    {
        float GetControlAmount(Piece piece);
    }

    private class PawnCenterControlHandler : IPieceCenterControlHandler
    {
        private float[,] blackPos = new float[8, 8] 
        {
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0.06f, 0.13f, 0.13f, 0.13f, 0.13f, 0.13f, 0.13f, 0.06f },
            { 0.06f, 0.2f, 0.26f, 0.26f, 0.26f, 0.26f, 0.2f, 0.06f },
            { 0.06f, 0.4f, 0.46f, 0.66f, 0.66f, 0.46f, 0.4f, 0.06f },
            { 0.06f, 0.4f, 0.8f, 1f, 1f, 0.8f, 0.4f, 0.06f },
            { 0.06f, 0.4f, 0.8f, 1f, 1f, 0.8f, 0.4f, 0.06f },
            { 0.06f, 0.4f, 0.46f, 0.66f, 0.66f, 0.46f, 0.4f, 0.06f },
            { 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        private float[,] whitePos = new float[8, 8] 
        {
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0.06f, 0.4f, 0.46f, 0.66f, 0.66f, 0.46f, 0.4f, 0.06f },
            { 0.06f, 0.4f, 0.8f, 1f, 1f, 0.8f, 0.4f, 0.06f },
            { 0.06f, 0.4f, 0.8f, 1f, 1f, 0.8f, 0.4f, 0.06f },
            { 0.06f, 0.4f, 0.46f, 0.66f, 0.66f, 0.46f, 0.4f, 0.06f },
            { 0.06f, 0.2f, 0.26f, 0.26f, 0.26f, 0.26f, 0.2f, 0.06f },
            { 0.06f, 0.13f, 0.13f, 0.13f, 0.13f, 0.13f, 0.13f, 0.06f },
            { 1, 1, 1, 1, 1, 1, 1, 1 }  
        };

    public float GetControlAmount(Piece piece)
        {
            int row = piece.GetTile().TilePosition.row;
            int column = piece.GetTile().TilePosition.column;

            if (piece.pieceColor == PieceColor.White)
                return whitePos[row, column];
            else
                return blackPos[row, column];
        }
    }

    private class RookCenterControlHandler : MappedCenterControlHandler
    {
        protected override float[,] Map => new float[8, 8]
        {
            { 0.26f, 0.38f, 0.61f, 0.81f, 0.81f, 0.61f, 0.38f, 0.26f },
            { 0.38f, 0.46f, 0.69f, 0.88f, 0.88f, 0.69f, 0.46f, 0.38f },
            { 0.61f, 0.69f, 0.81f, 1, 1, 0.81f, 0.69f, 0.61f },
            { 0.81f, 0.88f, 1, 1, 1, 1, 0.88f, 0.81f},
            { 0.81f, 0.88f, 1, 1, 1, 1, 0.88f, 0.81f},
            { 0.61f, 0.69f, 0.81f, 1, 1, 0.81f, 0.69f, 0.61f },
            { 0.38f, 0.46f, 0.69f, 0.88f, 0.88f, 0.69f, 0.46f, 0.38f },
            { 0.26f, 0.38f, 0.61f, 0.81f, 0.81f, 0.61f, 0.38f, 0.26f }
        };
    }

    private class QueenCenterControlHandler : MappedCenterControlHandler
    {
        protected override float[,] Map => new float[,]
        {
            { 0.52f, 0.49f, 0.53f, 0.6f, 0.6f, 0.53f, 0.49f, 0.52f },
            { 0.49f, 0.63f, 0.68f, 0.73f, 0.73f, 0.68f, 0.63f, 0.49f },
            { 0.53f, 0.68f, 0.84f, 0.89f, 0.89f, 0.84f, 0.68f, 0.53f },
            { 0.6f, 0.73f, 0.89f, 1, 1, 0.89f, 0.73f, 0.6f },
            { 0.6f, 0.73f, 0.89f, 1, 1, 0.89f, 0.73f, 0.6f },
            { 0.53f, 0.68f, 0.84f, 0.89f, 0.89f, 0.84f, 0.68f, 0.53f },
            { 0.49f, 0.63f, 0.68f, 0.73f, 0.73f, 0.68f, 0.63f, 0.49f },
            { 0.52f, 0.49f, 0.53f, 0.6f, 0.6f, 0.53f, 0.49f, 0.52f }
        };
    }

    private class BishopCenterControlHandler : MappedCenterControlHandler
    {
        protected override float[,] Map => new float[,]
        {
            { 0.83f, 0.62f, 0.43f, 0.35f, 0.35f, 0.43f, 0.62f, 0.83f },
            { 0.62f, 0.86f, 0.66f, 0.55f, 0.55f, 0.66f, 0.86f, 0.62f },
            { 0.43f, 0.66f, 0.88f, 0.76f, 0.76f, 0.88f, 0.66f, 0.43f },
            { 0.35f, 0.55f, 0.76f, 1, 1, 0.76f, 0.55f, 0.35f },
            { 0.35f, 0.55f, 0.76f, 1, 1, 0.76f, 0.55f, 0.35f },
            { 0.43f, 0.66f, 0.88f, 0.76f, 0.76f, 0.88f, 0.66f, 0.43f },
            { 0.62f, 0.86f, 0.66f, 0.55f, 0.55f, 0.66f, 0.86f, 0.62f },
            { 0.83f, 0.62f, 0.43f, 0.35f, 0.35f, 0.43f, 0.62f, 0.83f },
        };
    }

    private class KnightCenterControlHandler : MappedCenterControlHandler
    {
        protected override float[,] Map => new float[,]
        {
            { 0.14f, 0.29f, 0.36f, 0.5f, 0.5f, 0.36f, 0.29f, 0.14f },
            { 0.29f, 0.43f, 0.71f, 0.86f, 0.86f, 0.71f, 0.43f, 0.29f },
            { 0.36f, 0.71f, 1f, 1f, 1f, 1f, 0.71f, 0.36f },
            { 0.5f, 0.86f, 1f, 1f, 1f, 1f, 0.86f, 0.5f },
            { 0.5f, 0.86f, 1f, 1f, 1f, 1f, 0.86f, 0.5f },
            { 0.36f, 0.71f, 1f, 1f, 1f, 1f, 0.71f, 0.36f },
            { 0.29f, 0.43f, 0.71f, 0.86f, 0.86f, 0.71f, 0.43f, 0.29f },
            { 0.14f, 0.29f, 0.36f, 0.5f, 0.5f, 0.36f, 0.29f, 0.14f },
        };
    }

    private abstract class MappedCenterControlHandler : IPieceCenterControlHandler
    {
        protected abstract float[,] Map { get; }

        public float GetControlAmount(Piece piece)
        {
            int row = piece.GetTile().TilePosition.row;
            int column = piece.GetTile().TilePosition.column;

            return Map[row, column];
        }
    }
}
