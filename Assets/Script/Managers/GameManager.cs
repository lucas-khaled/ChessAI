using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardStarter boardStarter;
    [SerializeField] private PiecesSetup setup;

    public static Environment environment { get; private set; } = new();
    public static GameManager instance { get; private set; }

    public static Board Board => environment.board;
    public static BoardManager BoardManager => environment.boardManager;
    public static MoveMaker MoveMaker => environment.moveMaker;
    public static TurnManager TurnManager => environment.turnManager;
    public static EnvironmentEvents Events => environment.events;

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

        environment.events.onTurnDone += (_) => CheckForCheckMate();
    }

    private void CheckForCheckMate() 
    {
        if (environment.moveChecker.IsCheckMate())
            SelectionManager.LockSelection();
    }
}
