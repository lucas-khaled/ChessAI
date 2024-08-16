using UnityEngine;

[RequireComponent(typeof(PiecesSetup), typeof(PiecesCapturedController), typeof(BoardStarter))]
[RequireComponent(typeof(PlayTurnManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private string fen;
    [SerializeField] private bool startWithFen;

    public Environment environment { get; private set; } = new();
    public EndGameChecker EndGameChecker { get; private set; }

    private PiecesSetup setup;
    private PiecesCapturedController captureController;
    private BoardStarter boardStarter;
    private PlayTurnManager playTurnManager;

    public UIManager UIManager => uiManager;

    private FENManager FENManager;

    private void Awake()
    {
        GetHelperManagers();
        InitLogics();
    }

    private void GetHelperManagers()
    {
        setup = GetComponent<PiecesSetup>();
        setup.SetManager(this);

        captureController = GetComponent<PiecesCapturedController>();
        captureController.SetManager(this);

        boardStarter = GetComponent<BoardStarter>();
        boardStarter.SetManager(this);

        playTurnManager = GetComponent<PlayTurnManager>();
        playTurnManager.SetManager(this);
    }

    private void InitLogics()
    {
        var board = boardStarter.StartNewBoard();
        SetupEnvironment(board);

        FENManager = new FENManager(environment);
        EndGameChecker = new EndGameChecker(environment);

        ChooseSetup();

        playTurnManager.SetPlayers(new HumanPlayer(this), new MinimaxAI(this,2), environment.turnManager.ActualTurn);
    }

    private void SetupEnvironment(Board board) 
    {
        environment.StartRealEnvironment(board);

        environment.events.onTurnDone += OnEndTurn;
        environment.events.onPromotionMade += HandlePromotionMove;
        environment.events.onPieceCaptured += captureController.PieceCaptured;
    }

    private void OnEndTurn(PieceColor color)
    {
        var endInfo = EndGameChecker.CheckEnd();
        if (endInfo.hasEnded is false) return;

        if (endInfo.isCheckMate)
        {
            UIManager.ShowCheckmateMessage(color);
            return;
        }

        UIManager.ShowDrawMessage(endInfo.drawType);
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
            FEN fen = FENManager.GetFEN();
            Debug.Log("FEN  -  " + fen);
        }

        if (Input.GetKeyDown(KeyCode.T)) 
        {
            environment.turnManager.DebugAllTurns();
        }
    }
}
