using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardStarter boardStarter;
    [SerializeField] private PiecesSetup setup;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private string fen;
    [SerializeField] private bool startWithFen;

    public static Environment environment { get; private set; } = new();
    public static GameManager instance { get; private set; }

    public static Board Board => environment.board;
    public static BoardManager BoardManager => environment.boardManager;
    public static MoveMaker MoveMaker => environment.moveMaker;
    public static TurnManager TurnManager => environment.turnManager;
    public static EnvironmentEvents Events => environment.events;
    public static EspecialRules Rules => environment.rules;

    public static UIManager UIManager => instance.uiManager;

    private EndGameChecker endGameChecker = new();
    private FENManager FENManager = new FENManager();

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

        ChooseSetup();
    }

    private void SetupEnvironment(Board board) 
    {
        environment.StartRealEnvironment(board);

        environment.events.onTurnDone += endGameChecker.DoCheck;
        environment.events.onPromotionMade += HandlePromotionMove;
    }

    private void HandlePromotionMove(PromotionMove move)
    {
        setup.AddVisual(move.promoteTo, move.piece.visualPiece.name);

        Destroy(move.piece.visualPiece.gameObject);
    }

    private void ChooseSetup() 
    {
        if (string.IsNullOrEmpty(fen) || startWithFen is false)
            setup.SetInitialPieces();
        else
        {
            FENManager.SetupByFEN(new FEN(fen), setup.InstantiatePiece);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) 
        {
            FEN fen = FENManager.GetFENFrom(environment);
            Debug.Log("FEN  -  " + fen);
        }

        if (Input.GetKeyDown(KeyCode.T)) 
        {
            TurnManager.DebugAllTurns();
        }
    }
}
