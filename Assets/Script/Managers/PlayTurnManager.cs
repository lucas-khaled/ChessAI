using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayTurnManager : ManagerHelper
{
    private IPlayer whitePlayer;
    private IPlayer blackPlayer;

    private object moveLock = new();
    private Move madeMove = null;

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

        Task.Run(() => PlayerMove(startTurn));
        InvokeRepeating("CheckForMove", 0, 0.2f);
    }


    public void PlayerMove(PieceColor turn) 
    {
        try
        {
            if (turn == PieceColor.White)
                whitePlayer.StartTurn(OnMove);
            else
                blackPlayer.StartTurn(OnMove);
        }
        catch(Exception e) 
        {
            Debug.LogError($"Was not able to do Turn\n{e}");
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
            manager.TurnManager.DoMove(madeMove, board);
            madeMove = null;

            if (manager.EndGameChecker.CheckEnd(manager.TestBoard).hasEnded is false)
                Task.Run(() => PlayerMove(board.ActualTurn));
        }
    }
}
