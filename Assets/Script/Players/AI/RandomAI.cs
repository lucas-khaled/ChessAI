using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RandomAI : AIPlayer
{
    public RandomAI(GameManager manager) : base(manager, 0.1f)
    {
    }

    protected override async Task<Move> CalculateMove()
    {
        await Task.Delay((int)(minimumWaitTime * 1000));
        var allMoves = GetAllMoves(manager.GameBoard, actualColor);
        System.Random rand = new System.Random();
        int index = rand.Next(0, allMoves.Count);

        var move = allMoves[index];
        return move;
    }
}
