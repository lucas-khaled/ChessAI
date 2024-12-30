using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PerftFunction : MonoBehaviour
{
    protected Board board;
    protected IGameManager manager;

    public void Initialize(IGameManager manager)
    {
        this.board = manager.TestBoard;
        this.manager = manager;
    }

    public abstract Task<PerftData> Perft(int depth, bool divide = true);
}
