using System.Threading.Tasks;
using UnityEngine;

public class PlayTurnManager : ManagerHelper
{
    IPlayer whitePlayer;
    IPlayer blackPlayer;

    public void SetPlayers(IPlayer player1, IPlayer player2) 
    {
        int random = Random.Range(0, 100);

        if (random > 50)
        {
            whitePlayer = player1;
            blackPlayer = player2;
        }
        else 
        {
            whitePlayer = player2;
            blackPlayer = player1;
        }

        whitePlayer.Init(PieceColor.White);
        blackPlayer.Init(PieceColor.Black);
    }

    public void PlayerMove(PieceColor turn) 
    {
        if (turn == PieceColor.White)
            Task.Run(() => whitePlayer.StartTurn(OnMove));
        if (turn == PieceColor.Black)
            Task.Run(() => blackPlayer.StartTurn(OnMove)); 
    }

    private void OnMove(Move move) 
    {
        manager.environment.turnManager.DoMove(move);
    }
}
