using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PiecesSetup))]
public class PerftManager : MonoBehaviour, IGameManager
{
    [SerializeField] private BoardStarter boardStarter;
    [SerializeField] private string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    [SerializeField] [Min(1)] private int depth = 1;

    public Board GameBoard { get; private set; }
    public Board TestBoard { get; private set; }

    public TurnManager TurnManager { get; private set; }
    public EndGameChecker EndGameChecker { get; private set; }
    public MoveChecker MoveChecker { get; private set; }
    public ZobristHashManager HashManager { get; private set; }

    private PiecesSetup piecesSetup;
    private PerftFunction function;

    private void Awake()
    {
        piecesSetup = GetComponent<PiecesSetup>();
        piecesSetup.SetManager(this);

        HashManager = new ZobristHashManager();
        HashManager.InitializeHashes();

        function = GetComponent<PerftFunction>();
    }

    private void Start()
    {
        GameBoard = boardStarter.StartNewBoard();
        GameBoard.FENManager.SetupByFEN(new FEN(FEN), piecesSetup.InstantiatePiece, this);

        TestBoard = GameBoard.Copy();

        TurnManager = new TurnManager(this);
        EndGameChecker = new EndGameChecker(this);

        function.Initialize(this);
    }

    [ContextMenu("Perft it")]
    public async void DoPerft()
    {
        var depth = this.depth;
        var data = await function.Perft(depth);
        if (data < 0)//(data == null)
        {
            Debug.LogWarning("Is already Perfiting. Wait for completion");
            return;
        }

        Debug.Log($"Depth {depth}:\n\n{data}");
    }
}
