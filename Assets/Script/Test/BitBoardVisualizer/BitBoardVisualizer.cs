using UnityEngine;

public class BitBoardVisualizer : MonoBehaviour
{
    [SerializeField] private BoardStarter boardStarter;
    [SerializeField] private BitBoardUI boardUI;
    [SerializeField] private Color uiColor = Color.white;

    private Board board;

    void Start()
    {
        board = boardStarter.StartNewBoard();

        int index = 0;
        foreach(var row in board.GetTiles()) 
        {
            foreach(var tile in row) 
            {
                var boardUI = Instantiate(this.boardUI);
                boardUI.Set(tile, index, uiColor);
                index++;
            }
        }
    }
}
