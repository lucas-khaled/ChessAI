using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterControlHeuristic : Heuristic
{
    PawnCenterControlHandler pawnHandler = new();
    RookCenterControlHandler rookHandler = new();
    QueenCenterControlHandler queenHandler = new();
    BishopCenterControlHandler bishopHandler = new();
    KnightCenterControlHandler knightHandler = new();

    public CenterControlHeuristic(float weight) : base(weight)
    {
    }

    public override float GetHeuristic(Environment environment)
    {
        float blackPoints = 0;
        float whitePoints = 0;

        foreach(var piece in environment.board.pieces) 
        {
            float points = 0;

            if (piece is Queen)
                points = queenHandler.GetControlAmount(piece);
            else if (piece is Bishop)
                points = bishopHandler.GetControlAmount(piece);
            else if (piece is Knight)
                points = knightHandler.GetControlAmount(piece);
            else if (piece is Rook)
                points = rookHandler.GetControlAmount(piece);
            else if (piece is Pawn)
                points = pawnHandler.GetControlAmount(piece);

            if (piece.pieceColor != PieceColor.White)
                blackPoints += points;
            else
                whitePoints += points;

        }

        return (whitePoints - blackPoints) * weight;
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
