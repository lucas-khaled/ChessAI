using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RandomAI : AIPlayer
{
    public RandomAI(GameManager manager) : base(manager, 0.1f)
    {
    }

#if UNITY_WEBGL
    public override IEnumerator CalculateMoveRoutine(Move move)
    {
        move = GetRandomMove();
        yield return new WaitForSeconds(minimumWaitTime);
    }
#endif

    protected override async Task<Move> CalculateMove()
    {
        await Task.Delay((int)(minimumWaitTime * 1000));
        return GetRandomMove();
    }

    private Move GetRandomMove()
    {
        var allMoves = manager.TestBoard.currentTurnMoves;
        System.Random rand = new System.Random();
        int index = rand.Next(0, allMoves.Count);

        var move = allMoves[index];
        return move;
    }
}
