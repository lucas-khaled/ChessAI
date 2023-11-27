using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardStarter boardStarter;
    [SerializeField] private PiecesSetup setup;
    [SerializeField] private UIManager uiManager;

    public static Environment environment { get; private set; } = new();
    public static GameManager instance { get; private set; }

    public static Board Board => environment.board;
    public static BoardManager BoardManager => environment.boardManager;
    public static MoveMaker MoveMaker => environment.moveMaker;
    public static TurnManager TurnManager => environment.turnManager;
    public static EnvironmentEvents Events => environment.events;

    public static UIManager UIManager => instance.uiManager;

    private void Awake()
    {
        if(instance != null) 
        {
            Destroy(this);
            return;
        }

        instance = this;

        InitLogics();
    }

    private void InitLogics()
    {
        var board = boardStarter.StartNewBoard();
        SetupEnvironment(board);

        setup.SetInitialPieces();
    }

    private void SetupEnvironment(Board board) 
    {
        environment.StartRealEnvironment(board);

        environment.events.onTurnDone += CheckForCheckMate;
    }

    private void CheckForCheckMate(PieceColor lastTurnColor) 
    {
        if (environment.moveChecker.IsCheckMate())
        {
            uiManager.ShowCheckmateMessage(lastTurnColor);
            SelectionManager.LockSelection();
        }
    }
}
