using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void Awake()
    {
        if(instance != null) 
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        var board = boardStarter.StartNewBoard();
        environment.StartRealEnvironment(board);
        setup.SetInitialPieces();
    }
}
