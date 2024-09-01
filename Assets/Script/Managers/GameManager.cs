using UnityEngine;

[RequireComponent(typeof(PiecesSetup), typeof(PiecesCapturedController), typeof(BoardStarter))]
[RequireComponent(typeof(PlayTurnManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private string fen;
    [SerializeField] private bool startWithFen;

    public Board GameBoard { get; private set; }
    public Board TestBoard { get; private set; }

    public TurnManager TurnManager { get; private set; }
    public EndGameChecker EndGameChecker { get; private set; }
    public MoveChecker MoveChecker { get; private set; }

    public FENManager FENManager { get; private set; } 

    private PiecesSetup setup;
    private PiecesCapturedController captureController;
    private BoardStarter boardStarter;
    private PlayTurnManager playTurnManager;

    public UIManager UIManager => uiManager;


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

        FENManager = new FENManager(GameBoard);
        EndGameChecker = new EndGameChecker(this);
        MoveChecker = new MoveChecker(this);
        TurnManager = new TurnManager(this);

        ChooseSetup();
        UpdateTestBoard();

        playTurnManager.SetPlayers(new HumanPlayer(this), new MinimaxAI(this, 2), GameBoard.ActualTurn);
    }

    private void UpdateTestBoard()
    {
        TestBoard = GameBoard.Copy();
        TestBoard.Name = "Test Board";
    }

    private void SetupEnvironment(Board board) 
    {
        GameBoard = board;
        GameBoard.events.onTurnDone += OnEndTurn;
        GameBoard.events.onPromotionMade += HandlePromotionMove;
        GameBoard.events.onPieceCaptured += captureController.PieceCaptured;
        GameBoard.Name = "Game board";
    }

    private void OnEndTurn(PieceColor color)
    {
        UpdateTestBoard();
        var endInfo = EndGameChecker.CheckEnd(GameBoard);
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
            TurnManager.DebugAllTurns(GameBoard);
        }
    }
}
