using System.Threading.Tasks;
using UnityEngine;

public class PlayTurnManager : ManagerHelper
{
    private IPlayer whitePlayer;
    private IPlayer blackPlayer;

    private object moveLock = new();
    private Move madeMove = null;

    public void SetPlayers(IPlayer player1, IPlayer player2, PieceColor startTurn) 
    {
        int random = Random.Range(0, 100);

       /* if (random > 50)
        {
            whitePlayer = player1;
            blackPlayer = player2;
        }*/
        /*else 
        {*/
            whitePlayer = player2;
            blackPlayer = player1;
        //}

        whitePlayer.Init(PieceColor.White);
        blackPlayer.Init(PieceColor.Black);

        Task.Run(() => PlayerMove(startTurn));
        InvokeRepeating("CheckForMove", 0, 0.2f);
    }


    public void PlayerMove(PieceColor turn) 
    {
        if (turn == PieceColor.White)
            whitePlayer.StartTurn(OnMove);
        else
            blackPlayer.StartTurn(OnMove);
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
