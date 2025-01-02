using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PiecesSetup))]
public class PerftManager : MonoBehaviour, IGameManager
{
    [SerializeField] private BoardStarter boardStarter;
    [SerializeField] private string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    [SerializeField] [Min(1)] private int depth = 1;
    [SerializeField] private PerftResults results;
    [SerializeField] private bool debugAllPerft = true;

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
        GameBoard.Name = "GameBoard";

        TestBoard = GameBoard.Copy();
        TestBoard.Name = "TestBoard";

        TurnManager = new TurnManager(this);
        EndGameChecker = new EndGameChecker(this);

        function.Initialize(this);
    }

    [ContextMenu("Perft it")]
    public async void DoPerft()
    {
        var depth = this.depth;
        var data = await function.Perft(depth, debugAll: debugAllPerft);
        if (data.IsValid() is false)
        {
            Debug.LogWarning("Is already Perfiting. Wait for completion");
            return;
        }

        Debug.Log($"Depth {depth}:\n\n{data}");

        ValidateResults(data);
    }

    private void ValidateResults(PerftData data)
    {
        Debug.Log("Validating results...");
        var result = results.GetByFEN(FEN);

        if (result == null) 
        {
            Debug.Log($"There is no fen {FEN} in the results");
            return;
        }

        var depthData = result.GetByDepth(depth);

        if(depthData == null) 
        {
            Debug.Log($"There is no depth data in depth {depth} of results");
            return;
        }

        var perftData = depthData.data;

        Debug.Log((data.nodes == perftData.nodes) 
            ? $"<color=green>There are the same amount of nodes: {data.nodes}</color>" 
            : $"<color=red>There are not the same amount of nodes: Yours {data.nodes} - Result {perftData.nodes} </color>");

        if(debugAllPerft)
        {
            ValidateAllData(data, perftData);
        }

        ValidateDivides(data, perftData);
    }

    private void ValidateAllData(PerftData data, PerftData perftData)
    {
        string debugString = string.Empty;

        debugString += (data.checkmates == perftData.checkmates) 
            ? $"<color=green> - Checkmates are the same {data.checkmates}</color>\n" 
            : $"<color=red> - Checkmates are not the same: Yours {data.checkmates} - Result: {perftData.checkmates}</color>\n";

        debugString += (data.checks == perftData.checks)
           ? $"<color=green> - Checks are the same {data.checks}</color>\n"
           : $"<color=red> - Checks are not the same: Yours {data.checks} - Result: {perftData.checks}</color>\n";

        debugString += (data.doubleChecks == perftData.doubleChecks)
           ? $"<color=green> - Double Checks are the same {data.doubleChecks}</color>\n"
           : $"<color=red> - Double Checks are not the same: Yours {data.doubleChecks} - Result: {perftData.doubleChecks}</color>\n";

        debugString += (data.castles == perftData.castles)
           ? $"<color=green> - Castles are the same {data.castles}</color>\n"
           : $"<color=red> - Castles are not the same: Yours {data.castles} - Result: {perftData.castles}</color>\n";

        debugString += (data.captures == perftData.captures)
           ? $"<color=green> - Captures are the same {data.captures}</color>\n"
           : $"<color=red> - Captures are not the same: Yours {data.captures} - Result: {perftData.captures}</color>\n";

        debugString += (data.enPassants == perftData.enPassants)
          ? $"<color=green> - EnPassants are the same {data.enPassants}</color>\n"
          : $"<color=red> - EnPassants are not the same: Yours {data.enPassants} - Result: {perftData.enPassants}</color>\n";

        debugString += (data.promotions == perftData.promotions)
           ? $"<color=green> - Promotions are the same {data.promotions}</color>\n"
           : $"<color=red> - Promotions are not the same: Yours {data.promotions} - Result: {perftData.promotions}</color>\n";

        Debug.Log(debugString);
    }

    private void ValidateDivides(PerftData data, PerftData resultData)
    {
        string debugString = string.Empty;
        List<PerftDivide> generatedDivides = new List<PerftDivide>(data.divideDict);

        for(int i = 0; i<resultData.divideDict.Count; i++)
        {
            var divide = resultData.divideDict[i];
            var correspondingDivide = generatedDivides.Find(d => d.move == divide.move);

            if(string.IsNullOrEmpty(correspondingDivide.move)) 
            {
                debugString += $"<color=red>  {divide.move} does not exist in generated divide</color>\n";
                continue;
            }

            debugString += (divide.nodeCount == correspondingDivide.nodeCount)
                ? $"<color=green>  {correspondingDivide.move} -> has the exact count of {correspondingDivide.nodeCount}</color>\n"
                : $"<color=red>  {correspondingDivide.move} -> has not the same count of result: Yours {correspondingDivide.nodeCount} - Result {divide.nodeCount}</color>\n";

            generatedDivides.Remove(correspondingDivide);
        }

        if(generatedDivides.Count > 0) 
        {
            foreach(var divide in generatedDivides) 
                debugString += $"<color=red>  Generated divide {divide.move} with {divide.nodeCount} nodes that not exists in result</color>";
        }

        Debug.Log(debugString);
    }
}
