using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PiecesSetup))]
public class BitBoardVisualizer : MonoBehaviour, IGameManager
{
    [SerializeField] private BoardStarter boardStarter;
    [SerializeField] private BitBoardUI boardUI;
    [SerializeField] private Color uiColor = Color.white;
    [SerializeField] private string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private List<BitBoardUI> bitBoardUIs = new List<BitBoardUI>();

    public Board GameBoard { get; private set; }
    public Board TestBoard { get; private set; }

    public TurnManager TurnManager { get; private set; }
    public EndGameChecker EndGameChecker { get; private set; }
    public MoveChecker MoveChecker { get; private set; }
    public ZobristHashManager HashManager { get; private set; }

    private PiecesSetup piecesSetup;

    private void Awake()
    {
        piecesSetup = GetComponent<PiecesSetup>();
        piecesSetup.SetManager(this);

        HashManager = new ZobristHashManager();
        HashManager.InitializeHashes();
    }

    void Start()
    {
        GameBoard = boardStarter.StartNewBoard();
        GameBoard.FENManager.SetupByFEN(new FEN(FEN), piecesSetup.InstantiatePiece, this);

        InitializeUI();
    }

    private void InitializeUI() 
    {
        int index = 0;
        foreach (var row in GameBoard.GetTiles())
        {
            foreach (var tile in row)
            {
                var boardUI = Instantiate(this.boardUI);
                boardUI.Set(tile, index, uiColor);
                index++;

                bitBoardUIs.Add(boardUI);
            }
        }
    }

    public void SetBitBoard(Bitboard bitboard, Color color) 
    {
        foreach (var bitUI in bitBoardUIs) 
        {
            long operation = bitUI.tile.Bitboard.value & bitboard.value;
            bool isActive = operation > 0;
            bitUI.gameObject.SetActive(isActive);
            bitUI.SetColor(color);
        }
    }
}
