using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayTurnManager : ManagerHelper
{
    private IPlayer whitePlayer;
    private IPlayer blackPlayer;

    private object moveLock = new();
    private Move madeMove = null;

    private object currentTurnLock = new();
    private IPlayer currentTurn;

#if !UNITY_WEBGL
    private bool hasInvoked;
#endif

    public void SetPlayers(IPlayer firstPlayer, IPlayer secondPlayer, PieceColor startTurn, bool randomize = true)
    {
        this.whitePlayer = firstPlayer;
        this.blackPlayer = secondPlayer;

        if (randomize)
        {
            int random = UnityEngine.Random.Range(0, 100);

            if (random >= 50)
            {
                this.whitePlayer = secondPlayer;
                this.blackPlayer = firstPlayer;
            }
        }

        this.whitePlayer.Init(PieceColor.White);
        this.blackPlayer.Init(PieceColor.Black);

        StartPlayers(startTurn); 
    }

    private void StartPlayers(PieceColor turn)
    {
        SetCurrentPlayer(turn);

#if UNITY_WEBGL
        StartCoroutine(PlayerMoveRoutine());
#else
        Task.Run(() => PlayerMove());

        if(hasInvoked is false)
        {
            InvokeRepeating("CheckForMove", 0, 0.2f);
            hasInvoked = true;
        }
#endif
    }

#if UNITY_WEBGL

    private IEnumerator PlayerMoveRoutine() 
    {
        bool madeMove = false;
        currentTurn.StartTurn((move) =>
        {
            OnMove(move);
            madeMove = true;
        });

        yield return new WaitUntil(() => madeMove);
        CheckForMove();
    }

#else

    public void PlayerMove() 
    {
        try
        {
            currentTurn.StartTurn(OnMove);
        }
        catch (Exception e) 
        {
            Debug.LogError($"Was not able to do Turn\n{e}");
        }
    }

#endif

    private void SetCurrentPlayer(PieceColor turn)
    {
        lock (currentTurnLock)
        {
            if (turn == PieceColor.White)
                currentTurn = whitePlayer;
            else
                currentTurn = blackPlayer;
        }
    }

    public IPlayer GetCurrentPlayer() 
    {
        lock (currentTurnLock) 
        {
            return currentTurn;
        }
    }

    private void OnMove(Move move) 
    {
        lock (moveLock) 
        {
            madeMove = move;
        }
    }

    private void CheckForMove() 
    {
        lock (moveLock) 
        {
            if (madeMove == null) return;

            var board = manager.GameBoard;
            var color = board.ActualTurn.GetOppositeColor();
            SetCurrentPlayer(color);
            manager.TurnManager.DoMove(madeMove, board);
            madeMove = null;

            if (manager.EndGameChecker.CheckEnd(manager.TestBoard).hasEnded is false)
            {
                StartPlayers(color);
            }
        }
    }
}
